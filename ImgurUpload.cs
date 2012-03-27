using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;

namespace Poof
{
	/// <remarks>
	/// A class representing which houses the functions needed to upload a picture to the Imgur hosting service (http://imgur.com/)
	/// </remarks>
	class ImgurUpload : ImageUploadAPI
	{
		const string ImgurApiKey = "6a6c3f34e9ef1f399ee983706c256397";

		public override string UploadPicture(string location)
		{
			// Read a file
			FileStream fs;
			try
			{
				fs = File.OpenRead(location);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			// load the image file into a byte array
			byte[] imageData = new byte[fs.Length];
			fs.Read(imageData, 0, imageData.Length);
			fs.Close();

			// Create the request to send to the server
			string requestString = "image=" + EscapeDataString(imageData) + "&key=" + ImgurApiKey;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.imgur.com/2/upload");
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ServicePoint.Expect100Continue = false;

			// Send the request
			StreamWriter sw = new StreamWriter(request.GetRequestStream());
			sw.Write(requestString);
			sw.Close();

			// Retrieve the response
			WebResponse response = request.GetResponse();
			Stream responseStream = response.GetResponseStream();
			StreamReader responseReader = new StreamReader(responseStream);

			// response is in XML. Parse&Read it for imgur url
			return GetTagFromXML(responseReader.ReadToEnd(), "original");
		}

		/// <summary>
		/// Escapes a image's binary data and outputs it as a Base64 string.
		/// </summary>
		/// <param name="imageData">An image's binary data.</param>
		/// <returns>A Base64-encoded string of the image's data.</returns>
		private string EscapeDataString(byte[] imageData)
		{
			// Convert the string to Base64, and prepare to escape it
			string imageData_B64 = System.Convert.ToBase64String(imageData);
			StringBuilder escaped = new StringBuilder("", imageData_B64.Length);

			// Uri.EscapeDataString has a limit of 32766 characters
			const int DataStringLimit = 32766;

			for (int count = 0; count < (imageData_B64.Length / DataStringLimit) + 1; count++)
			{
				// The length of the string to append to escaped is either DataStringLimit,
				//  or the remainder of characters for the final append.
				int stringLength = DataStringLimit;
				if((imageData_B64.Length - (count * DataStringLimit)) < DataStringLimit)
				{
					stringLength = imageData_B64.Length % DataStringLimit;
				}

				escaped.Append(
					Uri.EscapeDataString(
						imageData_B64.Substring((count * DataStringLimit), stringLength)
					)
				);
			}

			return escaped.ToString();
		}

		/// <summary>
		/// Grabs the enclosed value of a tag in an XML document.
		/// </summary>
		/// <param name="Xml">The XML document, as a single string.</param>
		/// <param name="targetTag">The tag to find in the XML document</param>
		/// <returns>Returns the inner text of the tag.</returns>
		private string GetTagFromXML(string Xml, string targetTag)
		{
			XmlDocument document = new XmlDocument();
			document.Load(new StringReader(Xml));

			XmlElement element = document.DocumentElement;

			return document.GetElementsByTagName(targetTag)[0].InnerText;
		}
	}
}
