using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Poof
{
	public partial class frm_Search : Form
	{
		private static PasteDB db;
		const int dgd_ROWHEIGHT = 200;
		bool visible = false;

		// Initailize the form
		public frm_Search()
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
				MessageBox.Show(
					"Unable to connect to the database. Even tried making a new one, but still no dice.\n\n Make sure '" + Properties.Settings.Default.homeDirectory + "' exists and is writable, then try again.",
					"Error connecting to the database",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				this.Close();
			}

			FillDataGrid(null);
		}

		// Forces the window to be hidden on start up
		protected override void SetVisibleCore(bool value)
		{
			if (!IsHandleCreated && value)
			{
				value = false;
				CreateHandle();
			}
			base.SetVisibleCore(value);
		}

		// Repopulate the data grid with the new search results
		public void FillDataGrid(List<String> tags)
		{
			dgd_Results.Rows.Clear();

			List<PasteDBRow> results;

			if (tags == null || tags.Count == 0 || tags[0] == "")
			{
				results = db.returnAll();
			}
			else
			{
				results = new List<PasteDBRow>();

				//List<PasteDBRow> dbResults = db.getTopPastesByTags(tags);
				PasteDBRow dbResults = db.getTopPasteByTags(tags);
				if (dbResults != null)
				{
					results.Add(dbResults);
				}
			}

			if (results.Count > 0)
			{
				// Add rows to datagrid
				object[] tempobj = new object[4];
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
					tempobj[3] = resultRow.id;

					dgd_Results.Rows.Add(tempobj);
				}

				// Size all the rows to be the proper height
				for (int rowCount = 0; rowCount < dgd_Results.Rows.Count; rowCount++)
				{
					dgd_Results.Rows[rowCount].Height = dgd_ROWHEIGHT;
				}
			}
		}

		// Search the database with each new letter that was typed
		private void tbx_Search_TextChanged(object sender, EventArgs e)
		{
			FillDataGrid(tbx_Search.Text.Split(' ').ToList());
		}

		// Add the new tags to the database
		private void dgd_Results_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			// Get ID of edited picture
			int pictureID = Int32.Parse(dgd_Results.Rows[e.RowIndex].Cells[3].Value.ToString());
			List<string> newTags = dgd_Results.Rows[e.RowIndex].Cells[2].Value.ToString().Split(' ').ToList();

			// Clear out all tags associated with that picture
			db.deleteAllTagsForPictureID(pictureID);

			// Add all the new tags from the edit
			db.addTagsToPictureByID(pictureID, newTags);
		}

		// Click a cell to edit it or select the row
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

		// Select a result
		private void dgd_Results_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			getPasteForRow(e.RowIndex);
		}

		// Open the options form
		private void btn_Options_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Open an options window here...");
		}

		// Rescan the library folder for new images
		private void btn_Scan_Click(object sender, EventArgs e)
		{
			btn_Scan.Enabled = false;
			pnl_UploadProgress.Show();

			BackgroundWorker workerThread = new BackgroundWorker();
			workerThread.DoWork += new DoWorkEventHandler(uploadNewPictures);
			workerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(uploadNewPictures_completed);
			workerThread.RunWorkerAsync();
		}

		// Handler for when background work is finished
		private void uploadNewPictures_completed(object sender, RunWorkerCompletedEventArgs e)
		{
			pnl_UploadProgress.Hide();
			btn_Scan.Enabled = true;

			FillDataGrid(null);
		}

		// Handler for doing background work
		private void uploadNewPictures(object sender, DoWorkEventArgs e)
		{
			string[] fileTypes = { "*.jpg", "*.jpeg", "*.gif", "*.png" };
			List<string> fileList = new List<string>();

			// Populate fileList with a list of all picture files in DefaultDir
			// Useful for getting a total count of the files that are being scanned
			foreach (String fileType in fileTypes)
			{
				String[] filenames = Directory.GetFiles(Properties.Settings.Default.homeDirectory, fileType);
				foreach (String filename in filenames)
				{
					fileList.Add(filename);
				}
			}

			//setUploadProgressMax(fileList.Count);
			pgb_UploadProgress.Invoke(
				(MethodInvoker)delegate()
				{
					pgb_UploadProgress.Maximum = fileList.Count;
				}
			);

			// Proceed through the file list, adding any pictures that aren't already in the database
			for (int count = 0; count < fileList.Count; count++)
			{
				// If file doesn't exist in database, attempt to upload the picture and store it in the database
				//Console.WriteLine(db.getPasteByFilename(fileList[count]).TagsAsString);
				if (db.getPasteByFilename(fileList[count]) == null)
				{
					try
					{
						Debug.WriteLine("Image " + (count + 1) + "/" + fileList.Count + " (" + fileList[count] + ") : Uploading to Imgur...");
						
						//setUploadProgressCurrent(count);
						pgb_UploadProgress.Invoke(
							(MethodInvoker)delegate()
							{
								pgb_UploadProgress.Value = count;
							}
						);

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
						Debug.WriteLine("Error uploading file");
						Debug.WriteLine(ex.ToString());
					}
				}
				else
				{
					Debug.WriteLine("Image " + (count + 1) + "/" + fileList.Count + " (" + fileList[count] + ") : Already in database!");
				}
			}
		}

		private void setUploadProgressMax(int max)
		{
			pgb_UploadProgress.Maximum = max;
		}

		private void setUploadProgressCurrent(int currentValue)
		{
			pgb_UploadProgress.Value = currentValue;
		}

		// Handle "special" keys while focus is in the textbox
		private void tbx_Search_KeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape:
					toTray();
					break;
				case Keys.Enter:
					getPasteForRow(0);
					toTray();
					break;
				case Keys.Up:
					//MessageBox.Show("Move datagrid selection up, if it's at the top, move focus to text box");
					break;
				case Keys.Down:
					//MessageBox.Show("move datagrid selection down, if it's at the bottom, move focus to text box");
					break;
			}
		}

		// Notification icon click
		private void nti_NotificationIcon_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (visible)
				{
					toTray();
				}
				else
				{
					fromTray();
				}
			}
			else if (e.Button == MouseButtons.Right)
			{
				cmu_NotificationIconContextMenu.Show();
			}
		}

		private void toTray()
		{
			this.Hide();
			visible = false;
		}

		private void fromTray()
		{
			tbx_Search.Clear();
			FillDataGrid(null);

			this.Show();
			visible = true;
		}

		// Notification context menu click
		private void cmu_NotificationIconContextMenu_MouseClick(object sender, MouseEventArgs e)
		{
			// We're just kind of assuming the user hit exit, since that's the only option there is
			this.Close();
		}

		private void getPasteForRow(int RowID)
		{
			Clipboard.SetText(dgd_Results.Rows[RowID].Cells[1].Value.ToString());
		}
	}
}
