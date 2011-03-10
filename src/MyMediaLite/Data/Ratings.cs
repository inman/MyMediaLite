// Copyright (C) 2011 Zeno Gantner
//
// This file is part of MyMediaLite.
//
// MyMediaLite is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MyMediaLite is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MyMediaLite.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace MyMediaLite
{
	public enum RatingDataOrg { UNKNOWN, RANDOM, BY_USER, BY_ITEM }

	public class Ratings : IRatings
	{
		public IList<int> Users { get; private set; }
		public IList<int> Items { get; private set; }
		public IList<double> Values  { get; private set; } // TODO make generic

		public double this[int index] { get { return Values[index]; } }

		public int Count { get { return Values.Count; } }

		public RatingDataOrg organization = RatingDataOrg.UNKNOWN;

		// TODO try size reservation optimization
		public Ratings()
		{
			Users = new List<int>();
			Items = new List<int>();
			Values = new List<double>();
		}

		public int MaxUserID { get; private set; }
		public int MaxItemID { get; private set; }

		public IList<List<int>> ByUser { get; private set; }
		public IList<List<int>> ByItem { get; private set; }
		public int[] RandomIndex
		{
			get {
				if (random_index == null || random_index.Length != Values.Count)
				{
					Console.Error.Write("Shuffling ... ");
					random_index = new int[Values.Count];
					for (int index = 0; index < Values.Count; index++)
						random_index[index] = index;
					Util.Utils.Shuffle<int>(random_index);
					Console.Error.WriteLine("done.");
				}

				return random_index;
			}
		}
		private int[] random_index;

		// TODO speed up
		public double Average
		{
			get {
				double sum = 0;
				for (int index = 0; index < Values.Count; index++)
					sum += Values[index];
				return (double) sum / Values.Count;
			}
		}

		public HashSet<int> GetUsers()
		{
			var result_set = new HashSet<int>();
			for (int index = 0; index < Values.Count; index++)
				result_set.Add(Users[index]);
			return result_set;
		}

		public HashSet<int> GetItems()
		{
			var result_set = new HashSet<int>();
			for (int index = 0; index < Values.Count; index++)
				result_set.Add(Items[index]);
			return result_set;
		}

		public HashSet<int> GetUsers(IList<int> indices)
		{
			var result_set = new HashSet<int>();
			foreach (int index in indices)
				result_set.Add(Users[index]);
			return result_set;
		}

		public HashSet<int> GetItems(IList<int> indices)
		{
			var result_set = new HashSet<int>();
			foreach (int index in indices)
				result_set.Add(Items[index]);
			return result_set;
		}

		public double FindRating(int user_id, int item_id)
		{
			// TODO speed up
			for (int index = 0; index < Values.Count; index++)
				if (Users[index] == user_id && Items[index] == item_id)
					return Values[index];

			throw new Exception(string.Format("rating {0}, {1} not found.", user_id, item_id));
		}

		public double FindRating(int user_id, int item_id, ICollection<int> indexes)
		{
			// TODO speed up
			foreach (int index in indexes)
				if (Users[index] == user_id && Items[index] == item_id)
					return Values[index];

			throw new Exception(string.Format("rating {0}, {1} not found.", user_id, item_id));
		}

		public void AddRating(int user_id, int item_id, double rating)
		{
			Users.Add(user_id);
			Items.Add(item_id);
			Values.Add(rating);

			if (user_id > MaxUserID)
				MaxUserID = user_id;

			if (item_id > MaxItemID)
				MaxItemID = item_id;
		}
	}
}

