using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace Poof
{
	class Program
	{
		//private static string homeDir = @"C:\Users\Brad\Paste\";
		private static string commandPrefix = @"-";
		public static Boolean debug = true;
		private static PasteDB db;

		[STAThread]																// Single-threaded for use of the clipboard
		static void Main(string[] args)
		{
			// On first startup, set the homeDirectory (default to @"%userprofile%\Poof\")
			if (Properties.Settings.Default.homeDirectory == "")
			{
				Program.debugMsg("No home directory found. First start?");

				string homeDir = System.Environment.GetEnvironmentVariable("userprofile") + @"\Poof\";

				CmdUtil.colorWriteLine("Poof wants to set your paste directory to \"" + homeDir + "\"" + Environment.NewLine + ". If this is okay, press enter. Else, enter a new directory.", ConsoleColor.Green);
				string input = System.Console.ReadLine();

				if (input.Length > 0)
				{
					homeDir = input;
				}

				setHomeDirectory(homeDir);
				Properties.Settings.Default.Save();
			}

			// On debug, set up my test directory
			useTestDirectory();

			try
			{
				// Initialize the database
				db = new PasteDB(Path.Combine(Properties.Settings.Default.homeDirectory, "poof.accdb"));
				if (!db.Connect())
				{
					db.makeDB();
					db.Connect();
				}
				
				if (args.Length == 0)
				{
					// No arguments given. User doesn't know what they're doing; Display help text
					displayHelp();
				}
				else if (
					args[0].Length >= (commandPrefix.Length + 1) &&
					args[0].Substring(0, commandPrefix.Length).Equals(commandPrefix)
				)
				{
					// First argument is a command. Perform the command using all the remaining arguments
					performCommand(args);
				}
				else
				{
					// search the arguments as a list of tags for the best match
					List<String> tags = new List<string>();
					foreach (String arg in args)
					{
						tags.Add(arg);
					}

					PasteDBRow pasteResult = db.getTopPostByTags(tags);
					if (pasteResult != null)
					{
						displayPaste(pasteResult.uploadAddress);
					}
					else
					{
						// no results
						CmdUtil.colorWriteLine("Alas, no results were found.", ConsoleColor.DarkRed);
					}
				}

				db.Close();
			}
			catch(Exception ex)
			{
				Program.errorMsg(ex.ToString());
				CmdUtil.displayFatalError();
			}

			CmdUtil.colorWriteLine("\n«poof!»", ConsoleColor.DarkMagenta);
		}





		static public void displayHelp()
		{
			CmdUtil.colorWriteLine("Poof responds to the following commands:" + System.Environment.NewLine, ConsoleColor.Green);
			CmdUtil.colorWriteLine("\t" + commandPrefix + "s\tForce a scan of the directory, uploading any new pictures " + System.Environment.NewLine + "\t\tthat aren't in the database", ConsoleColor.Green);
			CmdUtil.colorWriteLine("\t" + commandPrefix + "t\tTakes a filename followed by tags, and updates the image " + System.Environment.NewLine + "\t\twith those tags", ConsoleColor.Green);
			CmdUtil.colorWriteLine("\t" + commandPrefix + "h\tSets the home directory (Will force a rescan)", ConsoleColor.Green);
			CmdUtil.colorWriteLine("\t" + commandPrefix + "l\tlists the database's contents", ConsoleColor.Green);
		}

		static public void performCommand(string[] commands)
		{
			if (commands[0].Equals(commandPrefix + "s"))
			{
				// Scan library
				Program.debugMsg("Scan library selected.");
				scanLibrary();
			}
			else if (commands[0].Equals(commandPrefix + "t"))
			{
				// Tag a picture
				Program.debugMsg("Tag a picture selected.");

				// First argument following the tag flag is the filename
				string filename = commands[1];
				// Remaining arguments following the tag flag are the tags to append
				//  Copy the tags from an array to a List
				string[] newTagsArray = new string[commands.Length - 2];
				Array.Copy(commands, 2, newTagsArray, 0, commands.Length - 2);
				List<String> newTags = new List<String>(newTagsArray);

				if (db.addTagsToPictureByLocation(filename, newTags))
				{
					Program.debugMsg("Tagged " + filename + " with " + newTags[0] + "...");				//STUB
				}
				else
				{
					Program.errorMsg("Error tagging picture");					//STUB
				}
			}
			else if (commands[0].Equals(commandPrefix + "h"))
			{
				// Set home directory
				Program.debugMsg("Set home directory selected.");

				if (commands.Length == 1)
				{
					CmdUtil.colorWriteLine("Your current home directory is " + Properties.Settings.Default.homeDirectory, ConsoleColor.Green);
				}
				else
				{
					if (setHomeDirectory(commands[1]))
					{
						CmdUtil.colorWriteLine("Poof successfully changed the home directory to \"" + commands[1] + "\".", ConsoleColor.Green);
					}
					else
					{
						CmdUtil.colorWriteLine("Unfortunately, Poof can't set the home directory to a location that doesn't exist.", ConsoleColor.Green);
					}
				}
			}
			else if (commands[0].Equals(commandPrefix + "l"))
			{
				// Output the database
				Program.debugMsg("output database selected.");
				outputDatabase();												//STUB
			}
			else
			{
				Program.errorMsg("Poof doesn't understand that command. The list of supported commands are:");
				displayHelp();
			}
		}

		static private Boolean scanLibrary()
		{
			string[] fileTypes = {"*.jpg", "*.jpeg", "*.gif", "*.png"};
			List<string> fileList = new List<string>();

			Boolean success = true;

			// Populate fileList with a list of all picture files in DefaultDir
			// Useful for getting a total count of the files that are being scanned
			foreach(String fileType in fileTypes)
			{
				String[] filenames = Directory.GetFiles(Properties.Settings.Default.homeDirectory, fileType);
				foreach(String filename in filenames)
				{
					fileList.Add(filename);
				}
			}

			// Proceed through the file list, adding any pictures that aren't already in the database
			for (int count = 0; count < fileList.Count; count++)
			{
				// If file doesn't exist in database, attempt to upload the picture and store it in the database
				//Console.WriteLine(db.getPasteByFilename(fileList[count]).TagsAsString);
				if(db.getPasteByFilename(fileList[count]) == null)
				{
					try
					{
						CmdUtil.colorWriteLine("Image " + (count + 1) + "/" + fileList.Count + " : Uploading to Imgur...");

						ImageUploadAPI imgur = new ImgurUpload();

						String uploadAddress = imgur.UploadPicture(fileList[count]);

						if (uploadAddress != null || uploadAddress != "")
						{
							PasteDBRow newRow = new PasteDBRow(0, fileList[count], imgur.UploadPicture(fileList[count]), "");
							db.addRow(newRow);
						}
						else
						{
							throw new Exception("An error occured trying to upload the file.");
						}
					}
					catch (Exception ex)
					{
						errorMsg(ex.ToString());
						CmdUtil.colorWriteLine("Error uploading image " + (count+1) + " to Imgur", ConsoleColor.Red);

						success = false;
					}
				}
				else
				{
					CmdUtil.colorWriteLine("Image " + (count + 1) + "/" + fileList.Count + " : Already in database!");
				}
			}

			return success;
		}

		static private Boolean setHomeDirectory(String newHomeDir)
		{
			if (!Directory.Exists(newHomeDir))
			{
				return false;
			}
			else
			{
				Properties.Settings.Default.homeDirectory = newHomeDir;
				Properties.Settings.Default.Save();

				return true;
			}
		}

		static private Boolean outputDatabase()									//TODO
		{
			//TODO: Output database in human-readable format

			
			List<PasteDBRow> results = db.returnAll();

			foreach (PasteDBRow row in results)
			{
				string filename = Path.GetFileName(row.location);
				//Console.WriteLine(filename + "\t" + "\t" + row.uploadAddress);
				//CmdUtil.colorWriteLine(filename + "\t" + "\t" + row.uploadAddress, ConsoleColor.Green);
				CmdUtil.colorWrite(String.Format("{0,-30}", filename), ConsoleColor.White);
				CmdUtil.colorWriteLine(String.Format("{0,30}", row.uploadAddress), ConsoleColor.Gray);
				if(row.tags.Count > 0)
					CmdUtil.colorWrite("\t" + String.Join(", ", row.tags), ConsoleColor.Gray);
				System.Console.WriteLine(System.Environment.NewLine);
			}

			return true;
		}

		static private void displayPaste(string address)
		{
			CmdUtil.colorWrite("The address ");
			CmdUtil.colorWrite(address, ConsoleColor.Black, ConsoleColor.Yellow);
			CmdUtil.colorWrite(" was written to the clipboard.\n");
			Clipboard.SetText(address);
		}

		static public void debugMsg(string message)
		{
#if DEBUG
				CmdUtil.displayDebug(message);
#endif
		}

		static public void errorMsg(string message)
		{
#if DEBUG
			CmdUtil.displayError(message);
#else
			CmdUtil.displayError();
#endif
		}

		static public void useTestDirectory()
		{
#if DEBUG
			CmdUtil.displayDebug("Resetting the default db location to " + System.Environment.GetEnvironmentVariable("userprofile") + @"\Poof\");
			setHomeDirectory(System.Environment.GetEnvironmentVariable("userprofile") + @"\Poof\");
#endif
		}
	}
}
