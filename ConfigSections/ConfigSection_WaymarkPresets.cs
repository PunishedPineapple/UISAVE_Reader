using System;
using System.Collections.Generic;
using System.Text;

namespace UISAVE_Reader.ConfigSections
{
	class ConfigSection_WaymarkPresets : ConfigSection
	{
		public struct Position
		{
			public double X;
			public double Y;
			public double Z;
		}
		public struct Waymark
		{
			public bool IsEnabled;
			public Position Pos;
			public override string ToString()
			{
				return IsEnabled ? ( Pos.X.ToString( " 000.00;-000.00" ) + ", " + Pos.Y.ToString( " 000.00;-000.00" ) + ", " + Pos.Z.ToString( " 000.00;-000.00" ) ) : "Unused";
			}
		}
		public struct WaymarkPreset
		{
			public UInt16 ZoneID;
			public Int32 UnixTimestamp;
			public Waymark[] Waymarks;

			public override string ToString()
			{
				string str = "";
				for( uint i = 0u; i < Waymarks.Length; ++i )
				{
					str += "Waymark Index " + i.ToString() + ": " + Waymarks[i].ToString() + "\r\n";
				}
				str += "\r\nZone ID: " + ZoneID.ToString() + "\r\nLast Modified: " + UnixTimestamp.ToString() + " (Unix Time)";
				return str;
			}
		}

		public ConfigSection_WaymarkPresets( uint fileOffset_Bytes, byte[] sectionHeader, byte[] sectionData ) :
			base( fileOffset_Bytes, sectionHeader, sectionData )
		{
		}

		protected override void Read()
		{
			//	Unknown data.  May contain information about how many bytes there are per preset or something similar, but it doesn't quite line up.
			Array.Copy( SectionData, 0, Bytes0to15, 0, 16 );
			uint offset = 16u;

			//	Process waymark presets.
			for( uint presetNumber = 0; presetNumber < Presets.Length; ++presetNumber )
			{
				//	Create waymark array.
				Presets[presetNumber].Waymarks = new Waymark[8];

				//	Waymark coordinates.
				for( uint waymarkNumber = 0u; waymarkNumber < Presets[presetNumber].Waymarks.Length; ++waymarkNumber )
				{
					Presets[presetNumber].Waymarks[waymarkNumber].Pos.X = BitConverter.ToInt32( new Span<byte>( SectionData, (int)offset + 0, sizeof( Int32 ) ).ToArray(), 0 ) / 1000.0;
					Presets[presetNumber].Waymarks[waymarkNumber].Pos.Y = BitConverter.ToInt32( new Span<byte>( SectionData, (int)offset + 4, sizeof( Int32 ) ).ToArray(), 0 ) / 1000.0;
					Presets[presetNumber].Waymarks[waymarkNumber].Pos.Z = BitConverter.ToInt32( new Span<byte>( SectionData, (int)offset + 8, sizeof( Int32 ) ).ToArray(), 0 ) / 1000.0;
					offset += 12u;
				}

				//	Byte for which waymarks are active.
				byte activeMask = SectionData[offset];
				for( uint i = 0u; i < Presets[presetNumber].Waymarks.Length; ++i )
				{
					uint testMask = 0x01;
					Presets[presetNumber].Waymarks[i].IsEnabled = ( (uint)activeMask & ( testMask << (int)i ) ) > 0;
				}

				//	Skip reserved byte.  Probably there for possible future expansion beyond eight waymarks.

				//	Two bytes for territory ID.
				Presets[presetNumber].ZoneID = BitConverter.ToUInt16( SectionData, (int)offset + 2 );

				//	Time last modified.
				Presets[presetNumber].UnixTimestamp = BitConverter.ToInt32( SectionData, (int)offset + 4 );

				//	Move on to the next preset.
				offset += 8u;
			}
		}

		public override string ToString()
		{
			string str = "Waymark Presets Section ( ID: " + SectionID.ToString() + ", Offset in File( includes section header ): 0x" + FileOffset.ToString( "X" ) + ", Length: 0x" + SectionData.Length.ToString( "X" ) + " bytes( excluding section header ) )\r\n";
			foreach( WaymarkPreset preset in Presets )
			{
				str += preset.ToString();
				str += "\r\n-----------\r\n";
			}
			return str;
		}

		public byte[] Bytes0to15 { get; protected set; } = new byte[16];
		public WaymarkPreset[] Presets { get; protected set; } = new WaymarkPreset[5];
	}
}
