using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Poof
{
	public partial class SearchForm : Form
	{
		private static PasteDB db;
		const int dgd_ROWHEIGHT = 200;

		public SearchForm()
		{
			InitializeComponent();

			try
			{
				// Initialize the database
				db = new PasteDB(Path.Combine(Properties.Settings.Default.homeDirectory, "poof.accdb"));
				if (!db.Connect())
				{
					db.makeDB();
					db.Connect();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			FillDataGrid(null);
		}

		public void FillDataGrid(List<String> tags)
		{
			dgd_Results.Rows.Clear();

			List<PasteDBRow> results;

			if (tags == null || tags.Count == 0)
			{
				results = db.returnAll();
			}
			else
			{
				results = new List<PasteDBRow>();
				results.Add(db.getTopPostByTags(tags));
			}

			// Add rows to datagrid
			object[] tempobj = new object[3];
			foreach (PasteDBRow resultRow in results)
			{
				// Set image preview
				Image preview = Image.FromFile(resultRow.location);
				int newHeight = preview.Height;
				int newWidth = preview.Width;
				if (preview.Width > preview.Height)
				{
					newWidth = dgd_ROWHEIGHT;
					newHeight = (int)(((double)dgd_ROWHEIGHT / preview.Width) * preview.Height);
				}
				else
				{
					newHeight = dgd_ROWHEIGHT;
					newWidth = (int)(((double)dgd_ROWHEIGHT / preview.Height) * preview.Width);
				}
				tempobj[0] = new Bitmap(preview, newWidth, newHeight);

				// Add in the upload address, and tag list
				tempobj[1] = resultRow.uploadAddress;
				tempobj[2] = resultRow.TagsAsString;

				dgd_Results.Rows.Add(tempobj);
			}

			// Size all the rows to be the proper height
			for (int rowCount = 0; rowCount < dgd_Results.Rows.Count; rowCount++)
			{
				dgd_Results.Rows[rowCount].Height = dgd_ROWHEIGHT;
			}
		}

		private void tbx_Search_TextChanged(object sender, EventArgs e)
		{
			FillDataGrid(tbx_Search.Text.Split(' ').ToList());
		}

		private void dgd_Results_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			MessageBox.Show("Cell edited!");
			// Clear out all tags associated with that picture
			// Add all the new tags from the edit
		}

		private void dgd_Results_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			// Ignore any invalid click events
			if (e.RowIndex < 0 || e.RowIndex > dgd_Results.Rows.Count) return;

			// Clicking a cell in the tag column edits it, else select it
			if (e.ColumnIndex == 2)
			{
				dgd_Results.CurrentCell = dgd_Results.Rows[e.RowIndex].Cells[2];
				dgd_Results.BeginEdit(false);
			}
			else
			{
				dgd_Results.Rows[e.RowIndex].Selected = true;
			}
		}

		private void dgd_Results_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			MessageBox.Show("Cell double-clicked! -- Copy address, minimize to tray");
		}
	}
}
