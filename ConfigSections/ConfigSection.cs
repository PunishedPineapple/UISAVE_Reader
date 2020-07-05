using System;
using System.Collections.Generic;
using System.Text;

namespace UISAVE_Reader
{
	abstract class ConfigSection
	{
		protected ConfigSection( uint fileOffset_Bytes, byte[] sectionHeader, byte[] sectionData )
		{
			FileOffset = fileOffset_Bytes;

			if( sectionHeader.Length != 16 ) throw new Exception( "Section header was of an unexpected size.  File Offset: 0x" + fileOffset_Bytes.ToString( "X" ) );
			SectionHeader = new Span<byte>( sectionHeader ).ToArray();
			SectionID = BitConverter.ToUInt16( SectionHeader, 0 );
			SectionLength_Bytes = BitConverter.ToUInt32( SectionHeader, 8 );

			if( sectionData.Length != SectionLength_Bytes ) throw new Exception( "Section data was of an unexpected size.  File Offset: 0x" + fileOffset_Bytes.ToString( "X" ) );
			SectionData = new Span<byte>( sectionData ).ToArray();

			Read();
		}

		protected abstract void Read();
		public abstract override string ToString();

		public uint FileOffset { get; protected set; }
		public byte[] SectionHeader { get; protected set; }
		public UInt16 SectionID { get; protected set; }
		public UInt32 SectionLength_Bytes { get; protected set; }
		public byte[] SectionData { get; protected set; }
	}
}
