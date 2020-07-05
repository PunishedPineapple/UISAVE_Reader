using System;
using System.Collections.Generic;
using System.Text;

namespace UISAVE_Reader.ConfigSections
{
	class ConfigSection_MailHistory : ConfigSection
	{
		public ConfigSection_MailHistory( uint fileOffset_Bytes, byte[] sectionHeader, byte[] sectionData ) :
			base( fileOffset_Bytes, sectionHeader, sectionData )
		{
		}

		protected override void Read()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			throw new NotImplementedException();
		}
	}
}
