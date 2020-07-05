using System;
using System.Collections.Generic;
using System.Text;

namespace UISAVE_Reader.ConfigSections
{
	class ConfigSection_Social : ConfigSection
	{
		public ConfigSection_Social( uint fileOffset_Bytes, byte[] sectionHeader, byte[] sectionData ) :
			base( fileOffset_Bytes, sectionHeader, sectionData )
		{
		}

		protected override void Read()
		{
			//	Contains Friends Group Titles, Recent Contacts List, Blacklist, and Some friends list-related things.  Possibly a bit more.
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			throw new NotImplementedException();
		}
	}
}
