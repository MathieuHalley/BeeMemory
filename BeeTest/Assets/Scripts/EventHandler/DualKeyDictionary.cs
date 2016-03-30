using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Eppy;


public class DualKeyDictionary<TKey1,TKey2,TValue> : Dictionary<Tuple<TKey1,TKey2>,TValue>
{
	public TValue this[TKey1 key1, TKey2 key2]
	{
		get
		{
			return this[new Tuple<TKey1, TKey2>(key1, key2)];
		}
		set
		{
			Tuple<TKey1, TKey2> tempKey = new Tuple<TKey1, TKey2>(key1, key2);
			if ( !ContainsKey(tempKey) )
			{
				Add(tempKey, value);
			}
			this[tempKey] = value;
		}
	}

	public void Add(TKey1 key1, TKey2 key2, TValue value)
	{
		Add(new Tuple<TKey1, TKey2>(key1, key2), value);
	}

	public bool ContainsKey(TKey1 key1, TKey2 key2)
	{
		return ContainsKey( new Tuple<TKey1, TKey2>(key1, key2) );
	}

	public bool ContainsKey(TKey1 key1)
	{
		foreach ( Tuple<TKey1, TKey2> keyPair in Keys )
		{
			if ( Compare<TKey1>(keyPair.Item1, key1) )
				return true;
		}
		return false;
	}

	public bool ContainsKey(TKey2 key2)
	{
		foreach ( Tuple<TKey1, TKey2> keyPair in Keys )
		{
			if ( Compare<TKey2>(keyPair.Item2, key2) )
				return true;
		}
		return false;
	}

	static bool Compare<T>(T a,T b)
	{
		return ( a.Equals(b) ) ? true : false;
	}
}
