using System;
using System.IO;
using System.Windows.Forms;

namespace Poof
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Console.WriteLine("starting...");
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new SearchForm());


			// On first startup, set the homeDirectory (default to @"%userprofile%\Poof\")
			if (Properties.Settings.Default.homeDirectory == "")
			{
				string homeDir = System.Environment.GetEnvironmentVariable("userprofile") + @"\Poof\";

				if (Directory.Exists(homeDir))
				{
					Properties.Settings.Default.homeDirectory = homeDir;
					Properties.Settings.Default.Save();
				}

				Properties.Settings.Default.Save();
			}
		}
	}
}
