using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace Poof
{
	class PasteDB
	{
		private string dbLocation;
		private OleDbConnection connection;

		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		///////////////////////////// DB FUNCTIONS /////////////////////////////
		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The default constructor for an empty "Poof-compliant" database.
		/// </summary>
		public PasteDB()
		{
		}

		/// <summary>
		/// The default constructor for a "Poof-compliant" database with a file location.
		/// </summary>
		/// <param name="location"></param>
		public PasteDB(string location)
		{
			dbLocation = location;
		}

		/// <summary>
		/// Connects to the database.
		/// </summary>
		/// <returns>True on success, false on failure.</returns>
		public Boolean Connect()
		{
			connection = new OleDbConnection();
			connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbLocation;

			try
			{
				connection.Open();
			}
			catch (Exception ex)
			{
				//throw new Exception("Failed to connect to data base.");
				Debug.WriteLine("Failed to connect to database:");
				Debug.WriteLine(ex.ToString());
				return false;
			}

			return true;
		}

		/// <summary>
		/// Closes the database connection.
		/// </summary>
		/// <returns>True on success, false on failure.</returns>
		public Boolean Close()
		{
			connection.Close();

			return true;
		}

		public Boolean makeDB()
		{
			try
			{
				byte[] abytResource;
				Assembly objAssembly = Assembly.GetExecutingAssembly();

				// Read embedded resource
				Stream objStream = objAssembly.GetManifestResourceStream("Poof.poof_blank.accdb");
				abytResource = new Byte[objStream.Length];
				objStream.Read(abytResource, 0, (int)objStream.Length);

				// Write out to file system
				FileStream objFileStream = new FileStream(Path.Combine(dbLocation), FileMode.Create);
				objFileStream.Write(abytResource, 0, (int)objStream.Length);
				objFileStream.Close();

			}
			catch (Exception ex)
			{
				Debug.WriteLine("failed to write blank database:");
				Debug.WriteLine(ex.ToString());
				return false;
			}

			return true;
		}

		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		//////////////////////////////// CREATE ////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Adds a row to the database.
		/// </summary>
		/// <param name="newRow">The row to be added to the database. The ID number in $newRow isn't used, as the database generates its own ID.</param>
		/// <returns>True on success, false on failure.</returns>
		public Boolean addRow(PasteDBRow newRow)
		{
			try
			{
				string now = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

				string insertCommand = @"INSERT INTO Pictures (pictures_location, pictures_uploadAddress, pictures_lastPaste) VALUES ('" +
					newRow.location + @"', '" +
					newRow.uploadAddress + @"', '" +
					DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + @"');";

				OleDbCommand command = new OleDbCommand(insertCommand, connection);
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to insert new row:");
				Debug.WriteLine(ex.ToString());
				return false;
			}

			return true;
		}

		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		///////////////////////////////// READ /////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Searches the database for the row that best matches the supplied tags.
		/// </summary>
		/// <param name="tags">A list of tags that will be searched for.</param>
		/// <returns>A List of PasteDBRows corresponding to the results from the query. </returns>
		public List<PasteDBRow> getPastesByTag(String tag)
		{
			List<PasteDBRow> resultSet = new List<PasteDBRow>();

			string whereCommand = @"SELECT Pictures.pictures_ID, Pictures.pictures_location, pictures.pictures_uploadAddress, pictures.pictures_lastPaste FROM Pictures LEFT JOIN Tags ON Pictures.pictures_ID = Tags.pictures_ID WHERE ((Tags.tags_tag)='" + tag + @"')";
			//Debug.WriteLine(whereCommand);
			OleDbCommand command = new OleDbCommand(whereCommand, connection);

			// Add this to the set of results
			try
			{
				OleDbDataReader reader = command.ExecuteReader();

				// While there is still unread results
				while (reader.Read())
				{
					// Read each result into a PasteDBRow object
					PasteDBRow newRow = new PasteDBRow(
						Int32.Parse(reader.GetValue(0).ToString()),
						reader.GetValue(1).ToString(),
						reader.GetValue(2).ToString(),
						reader.GetValue(3).ToString()
					);

					// Add the newly-built row to the results
					resultSet.Add(newRow);
				}
			}
			catch (OleDbException ex)
			{
				Debug.WriteLine("Failed to get paste address from tag:");
				Debug.WriteLine(ex.ToString());
				return null;
			}
			finally
			{
				Debug.WriteLine("Results for tag '" + tag + "': " + resultSet.Count);
			}

			if (resultSet.Count > 0)
				return resultSet;
			else
				return null;
		}

		/// <summary>
		/// Searches the database for the row that matches the supplied filename.
		/// </summary>
		/// <param name="tags">The filename to search for.</param>
		/// <returns>A PasteDBRow that corresponds to the supplied filename. </returns>
		public PasteDBRow getPasteByFilename(String filename)
		{
			//TODO: Would be totally cool if this function would detect if filename was relative or absolute, and acted accordingly
			//Path.Combine(dbLocation, filename);
			if (File.Exists(Path.Combine(Path.GetDirectoryName(dbLocation), filename)))
			{
				filename = Path.Combine(Path.GetDirectoryName(dbLocation), filename);
			}

			PasteDBRow result = new PasteDBRow();

			string whereCommand = @"SELECT Pictures.pictures_ID, Pictures.pictures_location, pictures.pictures_uploadAddress, pictures.pictures_lastPaste FROM Pictures WHERE ((pictures.pictures_location)='" + filename + @"')";
			//Debug.WriteLine(whereCommand);
			OleDbCommand command = new OleDbCommand(whereCommand, connection);

			// Retrieve the result from the database
			try
			{
				OleDbDataReader reader = command.ExecuteReader();

				if (!reader.HasRows) return null;

				// While there is still unread results
				while (reader.Read())
				{
					result.id = int.Parse(reader.GetValue(0).ToString());
					result.location = reader.GetValue(1).ToString();
					result.uploadAddress = reader.GetValue(2).ToString();
					result.lastPasteDate = reader.GetValue(3).ToString();
				}
			}
			catch (OleDbException ex)
			{
				Debug.WriteLine("Failed to get paste address from filename:");
				Debug.WriteLine(ex.ToString());
				return null;
			}

			return result;
		}

		/// <summary>
		/// Searches the database for the row that matches the specified ID.
		/// </summary>
		/// <param name="ID">The ID of the row that you want to retrieve.</param>
		/// <returns>A PasteDBRow object containing all the data for the specified row.</returns>
		public PasteDBRow getPasteByID(int ID)
		{
			PasteDBRow result = new PasteDBRow();

			string whereCommand = @"SELECT Pictures.pictures_ID, Pictures.pictures_location, pictures.pictures_uploadAddress, pictures.pictures_lastPaste FROM Pictures WHERE ((pictures.pictures_ID)=" + ID + @")";
			//Debug.WriteLine(whereCommand);
			OleDbCommand command = new OleDbCommand(whereCommand, connection);

			// Retrieve the result from the database
			try
			{
				OleDbDataReader reader = command.ExecuteReader();

				// While there is still unread results
				while (reader.Read())
				{
					result.id = (int)reader.GetValue(0);
					result.location = reader.GetString(1);
					result.uploadAddress = reader.GetString(2);
					result.lastPasteDate = reader.GetValue(3).ToString();
				}
			}
			catch (OleDbException ex)
			{
				Debug.WriteLine("Failed to get paste address from ID:");
				Debug.WriteLine(ex.ToString());

				return null;
			}

			return result;
		}

		/// <summary>
		/// Adds the tags to the tag table, and links them to the specified Picture ID.
		/// </summary>
		/// <param name="pictureID">The Picture ID to associate the tags with.</param>
		/// <param name="tags">A list of the tags to add.</param>
		/// <returns>True or false, depending on the success of the database INSERT.</returns>
		public Boolean addTagsToPictureByLocation(String pictureLocation, List<String> tags)			//STUB
		{
			// Find filename in database
			PasteDBRow pictureToTag = getPasteByFilename(pictureLocation);

			Boolean success = true;

			foreach (String tag in tags)
			{
				//TODO: remove all punctuation marks from the tag, make lowercase

				String addTagCommand = "INSERT INTO Tags (tags_tag, pictures_ID) VALUES ('" + tag + "', " + pictureToTag.id + ")";
				//Debug.WriteLine(addTagCommand);
				OleDbCommand command = new OleDbCommand(addTagCommand, connection);
				if (command.ExecuteNonQuery() == 0) success = false;
			}

			return success;
		}

		public Boolean addTagsToPictureByID(int pictureID, List<String> tags)
		{
			Boolean success = true;

			PasteDBRow pictureToTag = getPasteByID(pictureID);

			foreach (String tag in tags)
			{
				//TODO: remove all punctuation marks from the tag, make lowercase

				String addTagCommand = "INSERT INTO Tags (tags_tag, pictures_ID) VALUES ('" + tag + "', " + pictureToTag.id + ")";
				//Debug.WriteLine(addTagCommand);
				OleDbCommand command = new OleDbCommand(addTagCommand, connection);
				if (command.ExecuteNonQuery() == 0) success = false;
			}

			return success;
		}

		/// <summary>
		/// Get a list of PasteDBRow, sorted in order of decreasing relevance, that match a list of supplied tags.
		/// </summary>
		/// <param name="tags">A list of tags to search for.</param>
		/// <returns>A sorted list of PasteDBRows.</returns>
		public List<PasteDBRow> getPastesByTags(List<String> tags)
		{
			//TODO: FINISH THIS. Currently implented as getTopPostsByTags() for testing
			throw new NotImplementedException();

			TallyList hits = new TallyList();

			foreach (String tag in tags)
			{
				List<PasteDBRow> results = getPastesByTag(tag);
				if (results != null)
				{
					foreach (PasteDBRow result in results)
					{
						hits.addNewHit(result.id);
					}
				}
			}

			if (!hits.isEmpty())
				//return (getPasteByID(hits.getMostHitID()));
				return null;
			else
				return null;
		}

		/// <summary>
		/// Get a PasteDBRow that matches a list of supplied tags.
		/// </summary>
		/// <param name="tags">A list of tags to search for.</param>
		/// <returns>A PasteDBRow that most closely matches the supplied tags.</returns>
		public PasteDBRow getTopPasteByTags(List<String> tags)
		{
			TallyList hits = new TallyList();

			foreach (String tag in tags)
			{
				List<PasteDBRow> results = getPastesByTag(tag);
				if (results != null)
				{
					foreach (PasteDBRow result in results)
					{
						hits.addNewHit(result.id);
					}
				}
			}

			if (!hits.isEmpty())
				return (getPasteByID(hits.getMostHitID()));
			else
				return null;
		}

		/// <summary>
		/// Returns all the rows in the database.
		/// </summary>
		/// <returns>Returns the rows in the database as a list of PasteDBRows.</returns>
		public List<PasteDBRow> returnAll()
		{
			string getPicturesCommand = @"SELECT Pictures.pictures_ID, Pictures.pictures_location, Pictures.pictures_UploadAddress, Pictures.pictures_lastPaste FROM Pictures";

			//Debug.WriteLine(getPicturesCommand);
			OleDbCommand command = new OleDbCommand(getPicturesCommand, connection);

			// Build the set of results
			List<PasteDBRow> resultSet = new List<PasteDBRow>();
			try
			{
				OleDbDataReader picturesReader = command.ExecuteReader();

				// While there is still unread results
				while (picturesReader.Read())
				{
					// Read each result into a PasteDBRow object
					PasteDBRow newRow = new PasteDBRow(
						Int32.Parse(picturesReader.GetValue(0).ToString()),
						picturesReader.GetValue(1).ToString(),
						picturesReader.GetValue(2).ToString(),
						picturesReader.GetValue(3).ToString()
					);
					
					// Search for all tags belonging to the current picture, and add them
					string getTagsCommand = @"SELECT Tags.tags_tag FROM Tags WHERE Tags.pictures_ID=" + newRow.id;
					OleDbCommand tagsCommand = new OleDbCommand(getTagsCommand, connection);
					OleDbDataReader tagsReader = tagsCommand.ExecuteReader();
					//Debug.WriteLine(getTagsCommand);
					while (tagsReader.Read())
					{
						newRow.tags.Add(tagsReader.GetString(0));
					}

					// Add the newly-built row to the results
					resultSet.Add(newRow);
				}
			}
			catch (OleDbException ex)
			{
				Debug.WriteLine("Failed to get all rows from database:");
				Debug.WriteLine(ex.ToString());
				return null;
			}
			finally
			{
				Debug.WriteLine("Results for 'return db': " + resultSet.Count);
			}

			return resultSet;
		}

		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		//////////////////////////////// UPDATE ////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Updates the row in the database.
		/// </summary>
		/// <param name="updatedRow">The row to be updated. The ID number in this row is which record to update.</param>
		/// <returns>True on success, false on failure. A failure includes an ID in $updatedRow that doesn't exist.</returns>
		public Boolean updateRow(PasteDBRow updatedRow)
		{
			string updateCommand = "UPDATE Pictures SET " +
				"Location='" + updatedRow.location + "', " +
				"Upload_Address='" + updatedRow.uploadAddress + "', " +
				"Last_Paste='" + updatedRow.lastPasteDate + "' " +
				"WHERE ID=" + updatedRow.id;

			//Debug.WriteLine(updateCommand);
			OleDbCommand command = new OleDbCommand(updateCommand, connection);
			command.ExecuteNonQuery();

			return true;
		}

		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		/////////////////////////////// DESTROY ////////////////////////////////
		////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Deletes a row from the Database.
		/// </summary>
		/// <param name="rowID">The ID of the row to delete.</param>
		/// <returns>True on success, false on failure. A failure includes attempting to delete a row from the database that doesn't exist.</returns>
		public Boolean deleteRow(int rowID)
		{
			string deleteCommand = "DELETE FROM Pictures " +
				"WHERE ID=" + rowID;

			//Debug.WriteLine(deleteCommand);
			OleDbCommand command = new OleDbCommand(deleteCommand, connection);
			command.ExecuteNonQuery();

			return true;
		}

		/// <summary>
		/// Deletes all the tags for the specified pictureID
		/// </summary>
		/// <param name="pictureID">The pictureID to search for.</param>
		/// <returns>True on successful deletion, false on failure.</returns>
		public Boolean deleteAllTagsForPictureID(int pictureID)
		{
			String deleteCommand = "DELETE FROM Tags " + 
				"WHERE pictures_ID=" + pictureID;

			//Debug.WriteLine(deleteCommand);
			OleDbCommand command = new OleDbCommand(deleteCommand, connection);
			command.ExecuteNonQuery();

			return false;
		}
	}
}
