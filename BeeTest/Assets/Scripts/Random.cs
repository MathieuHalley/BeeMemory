using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShuffleList<T>
{
	public List<T> Shuffle(List<T> list)
	{
		int n = list.Count;
		while ( n > 1 )
		{
			--n;
			int k = Random.Range(0, n);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
		return list;
	}
}


public class RandomDictionaryValues<TKey,TValue>
{
	Dictionary<TKey, TValue> dict;

	public RandomDictionaryValues(Dictionary<TKey, TValue> d)
	{
		dict = d;
	}

	public IEnumerable<TValue> UniqueRandomValues()
	{
		Dictionary<TKey, TValue> values = dict;
		TKey randomKey;
		TValue randomValue;

		while ( values.Count > 0 )
		{
			randomKey = values.Keys.ElementAt(Random.Range(0, values.Count));  // hat tip @yshuditelu 
			randomValue = values[randomKey];
			values.Remove(randomKey);
			yield return randomValue;
		} 
	}

	public IEnumerable<TValue> UniqueRandomValues(int count)
	{
		return UniqueRandomValues().Take(count);
	}

	public IEnumerable<TValue> RandomValues()
	{
		List<TValue> values = Enumerable.ToList(dict.Values);

		while ( true )
		{
			yield return values[Random.Range(0, dict.Count)];			
		}
	}

	public IEnumerable<TValue> RandomValues(int count)
	{
		return RandomValues().Take(count);
	}
}
