using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Poof
{
	class TallyList
	{
		List<int> pictureID;
		List<int> hits;

		public TallyList()
		{
			pictureID = new List<int>();
			hits = new List<int>();
		}

		public Boolean addNewHit(int newPictureID)
		{
			Program_cli.debugMsg("Got a hit for ID:" + newPictureID);

			if (pictureID.Contains(newPictureID))
			{
				hits[pictureID.IndexOf(newPictureID)]++;
				return false;
			}
			else
			{
				pictureID.Add(newPictureID);
				hits.Add(0);
				return true;
			}
		}

		public int getMostHitID()
		{
			int currentMax = -1;
			int currentMaxLocation = -1;

			for(int count = 0; count < hits.Count; count++)
			{
				if (hits[count] > currentMax)
				{
					currentMax = hits[count];
					currentMaxLocation = count;
				}
			}

			Program_cli.debugMsg("Most hit ID: " + pictureID[currentMaxLocation]);
			return pictureID[currentMaxLocation];
		}

		public Boolean isEmpty()
		{
			return (pictureID.Count == 0 && hits.Count == 0) ;
		}
	}
}
