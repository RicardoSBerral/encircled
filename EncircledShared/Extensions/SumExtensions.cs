using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CocosSharp;

namespace Encircled.Extensions
{
	public static class SumExtensions
	{
		public static IEnumerable<KeyValuePair<T, int>> CumulativeSum<T> (this IEnumerable<KeyValuePair<T, int>> sequence)
		{
			int sum = 0;
			foreach (var item in sequence)
			{
				sum += item.Value;
				yield return new KeyValuePair<T, int> (item.Key, sum);
			}
		}

		public static T Roulette<T> (this Dictionary<T,int> dictionary)
		{
			int total = dictionary.Values.Sum ();
			int random = CCRandom.Next (0, total);

			return dictionary.AsEnumerable ().
				CumulativeSum ().
				First (item => item.Value > random)
				.Key;
		}
	}
}

