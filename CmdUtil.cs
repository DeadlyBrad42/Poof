using System;
using System.Text;

namespace Poof
{
	/// <remarks>
	/// The CmdUtil class contains functions and tools to ease the use of the console window.
	/// </remarks>
	static class CmdUtil
	{
		/// <summary>The DEFUALT_FOREGROUND property refers to the console color gray (8)</summary>
		public const ConsoleColor DEFAULT_FOREGROUND = ConsoleColor.Gray;
		/// <summary>The DEFUALT_BACKGROUND property refers to the console color black (0)</summary>
		public const ConsoleColor DEFAULT_BACKGROUND = ConsoleColor.Black;

		/// <summary>
		/// colorWrite writes the string text to the console window in the colors given, then resets the console's color to DEFAULT_FOREGROUND and DEFAULT_BACKGROUND.
		/// </summary>
		/// <param name="text">The string that's written to the console.</param>
		/// <param name="foregroundColor">(Optional) The foreground color to write the text in. If not set, text is written in DEFAULT_FOREGROUND.</param>
		/// <param name="backgroundColor">(Optional) The background color to write the text in. If not set, text is written in DEFAULT_BACKGROUND.</param>
		public static void colorWrite(string text, ConsoleColor foregroundColor = DEFAULT_FOREGROUND, ConsoleColor backgroundColor = DEFAULT_BACKGROUND)
		{
			colorChange(foregroundColor, backgroundColor);

			Console.Write(text);

			Console.ResetColor();
		}

		/// <summary>
		/// colorWriteLine writes the string text to the console window on a new line in the colors given, then resets the console's color to DEFAULT_FOREGROUND and DEFAULT_BACKGROUND.
		/// </summary>
		/// <param name="text">The string that's written to the console.</param>
		/// <param name="foregroundColor">(Optional) The foreground color to set the console. If not set, the console's text will be in DEFAULT_FOREGROUND.</param>
		/// <param name="backgroundColor">(Optional) The background color to write the text in. If not set, text is written in DEFAULT_BACKGROUND.</param>
		public static void colorWriteLine(string text, ConsoleColor foregroundColor = DEFAULT_FOREGROUND, ConsoleColor backgroundColor = DEFAULT_BACKGROUND)
		{
			colorWrite(text, foregroundColor, backgroundColor);
			colorWrite("\n");
		}

		/// <summary>
		/// An internal method that provides an easy way to change the colors. Providing no parameters sets the console colors to the default colors.
		/// </summary>
		/// <param name="foregroundColor">(Optional) The foreground color to set the console. If not set, the console's text will be in DEFAULT_FOREGROUND.</param>
		/// <param name="backgroundColor">(Optional) The background color to set the console. If not set, the console's text will be in DEFAULT_BACKGROUND.</param>
		private static void colorChange(ConsoleColor foregroundColor = DEFAULT_FOREGROUND, ConsoleColor backgroundColor = DEFAULT_BACKGROUND)
		{
			Console.ForegroundColor = foregroundColor;
			Console.BackgroundColor = backgroundColor;
		}

		/// <summary>
		/// Displays an error message in a white-on-red color scheme.
		/// </summary>
		/// <param name="errorMsg">(Optional) The error message to display. If not set, the output is "Something terrible has happened!"</param>
		static public void displayError(string errorMsg = "Something terrible has happened!")
		{
			colorWriteLine(errorMsg, ConsoleColor.White, ConsoleColor.Red);
		}

		/// <summary>
		/// Displays a fatal error message in a red-on-white color scheme.
		/// </summary>
		/// <param name="errorMsg">(Optional) The error message to display. If not set, the output is "An error occured. Poof will now see itself out."</param>
		static public void displayFatalError(string fatalErrorMsg = "An error occured. Poof will now see itself out.")
		{
			colorWriteLine(fatalErrorMsg, ConsoleColor.Red, ConsoleColor.White);
		}

		/// <summary>
		/// Displays a debug message in a white-on-blue color scheme.
		/// </summary>
		/// <param name="errorMsg">(Optional) The debug message to display. If not set, the output is "Poof has something it'd like to show you:"</param>
		static public void displayDebug(string debugMsg = "Poof has something it'd like to show you:")
		{
			colorWriteLine(debugMsg, ConsoleColor.White, ConsoleColor.Blue);
		}
	}
}
