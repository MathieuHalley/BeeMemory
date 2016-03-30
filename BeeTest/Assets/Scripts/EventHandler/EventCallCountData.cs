using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventCallCountData
{
	private Dictionary<int, int> localEventCallCount;
	private int globalEventCallCount = -1;

	public EventCallCountData(EventManager.EventDelegate del)
	{
		localEventCallCount = new Dictionary<int, int>();
	}

	/// <summary>
	///		
	/// </summary>
	/// <param name="goListenedTo">The GameObject being listened to for instances of this event</param>
	/// <param name="callCount">The number of times this event will be notified</param>
	/// <returns>Returns this EventCallCountData object</returns>
	public EventCallCountData AddLocalEventListener(GameObject goListenedTo, int callCount = -1)
	{
		localEventCallCount.Add(goListenedTo.GetHashCode(), callCount);
		return this;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="goListenedTo"></param>
	/// <returns></returns>
	public int GetLocalEventCallCount(GameObject goListenedTo)
	{
		return localEventCallCount[goListenedTo.GetHashCode()];
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public int GetGlobalEventCallCount()
	{
		return globalEventCallCount;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="goListenedTo"></param>
	/// <param name="callCount"></param>
	/// <returns>Returns this EventCallCountData object</returns>
	public EventCallCountData SetLocalEventCallCount(GameObject goListenedTo, int callCount)
	{
		localEventCallCount[goListenedTo.GetHashCode()] = callCount;
		return this;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="callCount"></param>
	/// <returns>Returns this EventCallCountData object</returns>
	public EventCallCountData SetGlobalEventCallCount(int callCount)
	{
		globalEventCallCount = callCount;
		return this;
	}
	
	/// <summary>
	///		Decrements the 
	/// </summary>
	/// <param name="removeCountsAtZero"></param>
	/// <returns>Returns this EventCallCountData object</returns>
	public EventCallCountData DecrementEventCallCounts(bool removeCountsAtZero = true)
	{
		foreach ( KeyValuePair<int,int> callCount in localEventCallCount )
		{
			if ( localEventCallCount[callCount.Key] > 0)
			{
				--localEventCallCount[callCount.Key];
			}
			if ( localEventCallCount[callCount.Key] == 0 && removeCountsAtZero == true )
			{
				localEventCallCount.Remove(callCount.Key);
			}
		}

		if ( globalEventCallCount > 0 )
		{
			--globalEventCallCount;
		}
		return this;
	}
}
