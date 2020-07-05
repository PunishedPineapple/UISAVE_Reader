using System;

namespace UISAVE_Reader
{
	class Program
	{
		static void Main( string[] args )
		{
			string filePath = Console.ReadLine();
			UISAVE file = new UISAVE( filePath );
			foreach( ConfigSection section in file.Sections )
			{
				Console.WriteLine( section.ToString() );
				
			}

			Console.ReadLine();
		}
	}
}
