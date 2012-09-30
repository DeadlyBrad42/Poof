using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poof
{
	/// <remarks>
	/// A class that encloses all the data contained in one row of a "Poof-compliant" database.
	/// </remarks>
	class PasteDBRow
	{
		private const int colCount = 5;
		public int id;
		public string location;
		public string uploadAddress;
		public string lastPasteDate;
		public List<string> tags;

		public PasteDBRow()
		{
			tags = new List<string>();
		}

		public PasteDBRow(int id, String location, String uploadAddress, String lastPasteDate)
		{
			this.id = id;
			this.location = location;
			this.uploadAddress = uploadAddress;
			this.lastPasteDate = lastPasteDate;

			tags = new List<string>();
		}

		public static int RowSize()
		{
			return colCount;
		}

		public string TagsAsString
		{
			get
			{
				return String.Join(" ", tags);
			}
		}
	}
}
