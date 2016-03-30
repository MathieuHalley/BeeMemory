using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Eppy;


public class TupleKeyDictionary<TKey1,TKey2,TValue> : Dictionary<Tuple<TKey1,TKey2>,TValue>
{
	public bool ContainsKeyItem1(TKey1 key1)
	{
		foreach ( Tuple<TKey1, TKey2> keyPair in Keys )
		{
			if ( Compare<TKey1>(keyPair.Item1, key1) )
				return true;
		}
		return false;
	}

	public bool ContainsKeyItem2(TKey2 key2)
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
