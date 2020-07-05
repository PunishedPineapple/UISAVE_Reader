using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UISAVE_Reader.ConfigSections
{
	class ConfigSection_Unknown : ConfigSection
	{
		public ConfigSection_Unknown( uint fileOffset_Bytes, byte[] sectionHeader, byte[] sectionData ) :
			base( fileOffset_Bytes, sectionHeader, sectionData )
		{
		}

		protected override void Read()
		{
		}

		public override string ToString()
		{
			return "Unknown Section Type (ID: " + SectionID.ToString() + ", Offset in File (includes section header): 0x" + FileOffset.ToString( "X" ) + ", Length: 0x" + SectionData.Length.ToString( "X" ) + " bytes (excluding section header))";
		}
	}
}
