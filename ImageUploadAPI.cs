using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poof
{
	/// <remarks>
	/// An abstract class representing the minimum functions needed to upload a picture to an image hosting service
	/// </remarks>
	abstract class ImageUploadAPI
	{
		/// <summary>
		/// Uploads the picture to a webserver and returns the new address.
		/// </summary>
		/// <param name="location">The Windows file path to the picture.</param>
		/// <returns>A string containing the URL address for the file on a web server.</returns>
		public abstract string UploadPicture(string location);
	}
}
