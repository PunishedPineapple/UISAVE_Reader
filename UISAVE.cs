using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UISAVE_Reader.ConfigSections;

namespace UISAVE_Reader
{
	class UISAVE
	{
		public UISAVE( string fileName )
		{
			FileName = fileName;
			MagicNumber = 0x31;
			ReadFile();
		}

		public void ReadFile()
		{
			//	Check that we can do stuff.
			if( !File.Exists( FileName ) ) throw new Exception( "File does not exist (" + FileName + ")" );
			if( !BitConverter.IsLittleEndian ) throw new Exception( "BitConverter is reporting Big-Endian, and we're not set up to deal with this." );

			//	Read in the raw data.
			byte[] rawData = File.ReadAllBytes( FileName );

			//	Parse the header.
			if( rawData.Length < 16 ) throw new Exception( "The file was not long enough to contain a full header.  Either the file or the header is corrupt, or the file was not completely read." );
			Array.Copy( rawData, 0, Bytes0to3, 0, 4 );
			Array.Copy( rawData, 4, Bytes4to7, 0, 4 );
			NumValidBytes = BitConverter.ToUInt32( rawData, 8 );
			Array.Copy( rawData, 12, Bytes12to15, 0, 4 );

			//	Obtain the rest of the valid data and unpad it.
			if( rawData.Length < NumValidBytes + 16 ) throw new Exception( "The file was shorter than the header indicated.  Either the file or the header is corrupt, or the file was not completely read." );
			byte[] correctedData = XORBytes( rawData, 16, (int)NumValidBytes, MagicNumber );

			//	The first two parts after the header are not true sections, so handle them specifically.
			Array.Copy( rawData, 0, Bytes16to23, 0, 8 );
			ContentID = BitConverter.ToUInt64( rawData, 8 );

			//	Locals for processing sections.
			uint currentOffset = 16u;
			UInt16 sectionID;
			UInt32 sectionLength;

			//	And now we can start processing the sections.
			while( currentOffset < NumValidBytes )
			{
				if( NumValidBytes > currentOffset + 16u )
				{
					//	Read Section Header
					sectionID = BitConverter.ToUInt16( correctedData, (int)currentOffset );
					//UNKNOWN: Six bytes of unknown use.
					sectionLength = BitConverter.ToUInt32( correctedData, (int)currentOffset + 8 );
					//UNKNOWN: Another four bytes of unknown use.
				}
				else
				{
					throw new Exception( "Expected section header, but encountered premature end of valid file region." );
				}

				if( NumValidBytes > currentOffset + sectionLength )
				{
					//	Create new section with data and add to list.
					Sections.Add( CreateConfigSection(
						sectionID,
						new Span<byte>( correctedData, (int)currentOffset, 16 ).ToArray(),
						new Span<byte>( correctedData, (int)currentOffset + 16, (int)sectionLength ).ToArray(),
						currentOffset ) );

					//	Move our current offset to the end of the section, including the header and the four bytes of trailing padding.  Assuming the padding was probably reserved for an unimplemented checksum or something.
					currentOffset += 16u + sectionLength + 4u;
				}
				else
				{
					throw new Exception( "Encountered premature end of valid file region while processing a section." );
				}
			}
		}

		protected ConfigSection CreateConfigSection( UInt16 sectionID, byte[] sectionHeader, byte[] sectionData, uint fileOffset_Bytes )
		{
			//*****TODO: Add flag to ConfigSection class whether it's a section that needs further XOR decoding (the 0xFF-started sections)?  Maybe just decode it right here or in our caller.*****

			/*if( sectionID == 0x0 )
			{
				return new ConfigSection_MailHistory( fileOffset_Bytes, sectionHeader, sectionData );
			}
			else if( sectionID == 0x4 )
			{
				return new ConfigSection_Social( fileOffset_Bytes, sectionHeader, sectionData );
			}
			else if( sectionID == 0x5 )
			{
				//	Teleport History
			}
			else if( sectionID == 0xD )
			{
				//	CWLS Info, contains at least CWLS ordering.
			}
			else */if( sectionID == 0x11 )
			{
				return new ConfigSection_WaymarkPresets( fileOffset_Bytes, sectionHeader, sectionData );
			}
			else
			{
				return new ConfigSection_Unknown( fileOffset_Bytes, sectionHeader, sectionData );
			}
		}

		protected byte[] XORBytes( byte[] source, int startIndex, int length, byte XORValue )
		{
			byte[] newData = new Span<byte>( source, startIndex, length ).ToArray();
			Array.Copy( source, startIndex, newData, 0, length );
			for( uint i = 0; i < newData.Length; ++i ) newData[i] ^= XORValue;
			return newData;
		}

		public string FileName { get; protected set; }

		public byte MagicNumber { get; protected set; }
		public byte[] Bytes0to3 { get; protected set; } = new byte[4];
		public byte[] Bytes4to7 { get; protected set; } = new byte[4];
		public UInt32 NumValidBytes { get; protected set; }
		public byte[] Bytes12to15 { get; protected set; } = new byte[4];
		public byte[] Bytes16to23 { get; protected set; } = new byte[8];
		public UInt64 ContentID { get; protected set; }

		public List<ConfigSection> Sections { get; protected set; } = new List<ConfigSection>();
	}
}
