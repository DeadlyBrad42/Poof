using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Poof
{
	static class Debug
	{
		[Conditional("DEBUG")]
		public static void WriteLine(String str)
		{
			System.Diagnostics.Debug.WriteLine(str);
		}
	}
}
