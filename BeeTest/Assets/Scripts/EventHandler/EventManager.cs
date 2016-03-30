using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Eppy;
using System.Linq;

public struct ListenerLookupKey
{
	public System.Delegate eventDelegate;
	public GameObject eventSource;

	public ListenerLookupKey(System.Delegate eventDel, GameObject eventSrc)
	{
		eventDelegate = eventDel;
		eventSource = eventSrc;
	}
}

public class EventManager : MonoBehaviour
{
	public delegate void EventDelegate<TEvent>(TEvent e) where TEvent : IGameEvent;
	public delegate void EventDelegate(IGameEvent e);
	public bool LimitQueueProcesing = false;
	public float QueueProcessTime = 0.0f;

	// override so we don't have the typecast the object
	public static EventManager Instance
	{
		get
		{
			if ( _Instance == null )
			{
				_Instance = GameObject.FindObjectOfType(typeof(EventManager)) as EventManager;
			}
			return _Instance;
		}
	}

	private static EventManager _Instance = null;
	private Queue eventQueue = new Queue();

	//	Event reference dictionaries
	//	Dictionary for event listeners
	private Dictionary<GameEventKey, EventDelegate> eventDelegates
		= new Dictionary<GameEventKey, EventDelegate>();
	//	Lookup dictionary for event listeners that will be called an unspecified number of times
	private Dictionary<ListenerLookupKey, EventDelegate> listenerLookup
		= new Dictionary<ListenerLookupKey, EventDelegate>();
	//	Lookup dictionary for event listeners that will be called a limited number of times
	private Dictionary<ListenerLookupKey, int> limitedListenerLookup
		= new Dictionary<ListenerLookupKey, int>();

	#region Listener Control
	/// <summary>
	///		Add a delegate to listen for an event. If callCount isn't '-1' the delegate will only be called a certain number of times.
	/// </summary>
	/// <typeparam name="TEvent">The type of IGameEvent that the delegate will be listening for</typeparam>
	/// <param name="del">The delegate that will be called when the event occurs</param>
	/// <param name="callCount">The number of times the delegate will be called before being removed (default = -1)</param>
	/// <returns>this EventManager</returns>	
	public EventManager AddGlobalListener<TEvent>(EventDelegate<TEvent> del, int callCount = -1)
		where TEvent : IGameEvent
	{
		EventDelegate listenerDelegate = GetListener<TEvent>(del);
		ListenerLookupKey lookupKey = new ListenerLookupKey(listenerDelegate, null);

		//	If del hasn't been added as a listener to the TEvent type of IGameEvent yet, add it as either a global or local listener.
		if ( listenerDelegate == null )
		{
			listenerDelegate = AddDelegate<TEvent>(del);
			if ( callCount > 0 )
			{
				SetLimitedListenerCallCount(lookupKey, GetLimitedListenerCallCount(lookupKey) + callCount);
			}
		}
		return this;
	}

	/// <summary>
	///		Add a delegate to listen for an event. If callCount isn't '-1' the delegate will only be called a certain number of times.
	/// </summary>
	/// <typeparam name="TEvent">The type of GameEvent that the delegate will be listening for</typeparam>
	/// <param name="del">The delegate that will be called when the event occurs</param>
	/// <param name="eventSource">The game object the delegate will pay attention to</param>
	/// <param name="callCount">The number of times the delegate will be called before being removed (default = -1)</param>
	/// <returns>this EventManager</returns>	
	public EventManager AddTargetedListener<TEvent>(EventDelegate<TEvent> del, GameObject eventSource, int callCount = -1)
		where TEvent : IGameEvent
	{
		EventDelegate listenerDelegate = GetListener<TEvent>(del, eventSource);
		ListenerLookupKey lookupKey = new ListenerLookupKey(listenerDelegate, eventSource);

		//	If del hasn't been added as a listener to the TEvent type of IGameEvent yet, add it as either a global or local listener.
		if ( listenerDelegate == null )
		{
			listenerDelegate = AddDelegate<TEvent>(del, eventSource);
			if ( callCount > 0 )
			{
				SetLimitedListenerCallCount(lookupKey, GetLimitedListenerCallCount(lookupKey) + callCount);
			}
		}
		return this;
	}

	/// <summary>
	///		Stops a specific delegate listening to an event.
	/// </summary>
	/// <typeparam name="TEvent">The type of IGameEvent that the delegate is listening for</typeparam>
	/// <param name="del">The delegate that is called when the event occurs</param>
	/// <param name="eventSource">The game object the delegate is paying attention to if it's only interested in local events (default = null)</param>
	/// <returns>this EventManager</returns>	
	public EventManager RemoveListener<TEvent>(EventDelegate<TEvent> del, GameObject eventSource = null)
		where TEvent : IGameEvent
	{
		RemoveDelegate<TEvent>(del, eventSource);
		return this;
	}

	/// <summary>
	///		Checks if the delegate is actually listening for this event.
	/// </summary>
	/// <typeparam name="TEvent">The type of IGameEvent that the delegate is listening for</typeparam>
	/// <param name="del">The delegate that is called when the event occurs</param>
	/// <param name="eventSource">The game object the delegate is paying attention to if it's only interested in local events (default = null)</param>
	/// <returns>true = The delegate is listening for this type of IGameEvent | false = the delegate is not listening for this type of IGameEvent</returns>
	public bool HasListener<TEvent>(EventDelegate<TEvent> del, GameObject eventSource = null)
		where TEvent : IGameEvent
	{
		return HasListener(new ListenerLookupKey((System.Delegate)del, eventSource));
	}
	/// <summary>
	///		Checks if the delegate is actually listening for this event.
	/// </summary>
	/// <param name="listenerLookupKey"></param>
	/// <returns>true = The delegate is listening for a IGameEvent with this lookupKey | false = the delegate is not listening for a IGameEvent with this lookupKey</returns>
	public bool HasListener(ListenerLookupKey listenerLookupKey)
	{
		return listenerLookup.ContainsKey(listenerLookupKey);
	}

	/// <summary>
	///		Get the delegate that is listening to the specific IGameEvent
	/// </summary>
	/// <typeparam name="TEvent">The type of IGameEvent that the delegate is listening for</typeparam>
	/// <param name="del">The delegate that is called when the event occurs</param>
	/// <param name="eventSource">The game object the delegate is paying attention to if it's only interested in local events (default = null)</param>
	/// <returns>this EventManager</returns>	
	public EventDelegate GetListener<TEvent>(EventDelegate<TEvent> del, GameObject eventSource = null)
		where TEvent : IGameEvent
	{
		ListenerLookupKey listenerLookupKey = new ListenerLookupKey(del, eventSource);
		return HasListener(listenerLookupKey) ? listenerLookup[listenerLookupKey] : null;
	}
	#endregion

	/// <summary>
	///		Attempts to enqueue the event into the current queue. The event will only be enqueued if there is a delegate to listen to it.
	/// </summary>
	/// <param name="e">The event that will be inserted into the queue</param>
	/// <returns>this EventManager</returns>	
	public EventManager QueueEvent(IGameEvent newEvent)
	{
		if ( eventDelegates.ContainsKey(newEvent.EventKey) || eventDelegates.ContainsKey(new GameEventKey(newEvent.EventKey.eventType, null)) )
		{
			eventQueue.Enqueue(newEvent);
		}
		else
		{
			Debug.LogWarning("EventManager: QueueEvent failed due to no listeners for event: " + newEvent.ToString());

			Debug.Break();
		}

		return this;
	}

	/// <summary>
	///		Attempt to trigger a IGameEvent as a global and/or local event.
	/// </summary>
	/// <param name="e">The IGameEvent to be triggered</param>
	private void TriggerEvent(IGameEvent e)
	{
		ListenerLookupKey lookupKey;
		GameEventKey targetedEventKey = e.EventKey;
		GameEventKey globalEventKey = new GameEventKey(e.EventKey.eventType, null);
		List<System.Delegate> invocationList = new List<System.Delegate>();

		bool targetedListenersExist = eventDelegates.ContainsKey(targetedEventKey);
		bool globalListenersExist = eventDelegates.ContainsKey(globalEventKey);

		if ( !targetedListenersExist && !globalListenersExist )
		{
			Debug.LogWarning("EventManager: TriggerEvent failed due to no listeners for event: " + e.ToString());
			return;
		}
		if ( targetedListenersExist )
		{
			invocationList.AddRange(eventDelegates[targetedEventKey].GetInvocationList());
		}
		if ( globalListenersExist )
		{
			invocationList.AddRange(eventDelegates[globalEventKey].GetInvocationList());
		}

		foreach ( EventDelegate eventDelegate in invocationList )
		{
			lookupKey = new ListenerLookupKey(eventDelegate, e.EventKey.eventSource);

			eventDelegate.Invoke(e);
			//	Decrement the number of of times any limited listeners will be called when this event is triggered
			//	Remove listeners that won't be triggered any more
			SetLimitedListenerCallCount(lookupKey, GetLimitedListenerCallCount(lookupKey) - 1);
		}
	}
	
	/// <summary>
	///		Add a delegate to the collection of active eventDelegates listening for local events
	/// </summary>
	/// <typeparam name="TEvent">The type of IGameEvent that the delegate will be listening for</typeparam>
	/// <param name="del">The delegate that will be called when the event occurs</param>
	/// <param name="eventSource">The game object the delegate will pay attention to</param>
	/// <returns></returns>
	private EventDelegate AddDelegate<TEvent>(EventDelegate<TEvent> del, GameObject eventSource = null)
		where TEvent : IGameEvent
	{
		GameEventKey delsKey = new GameEventKey(typeof(TEvent), eventSource);
		EventDelegate internalDelegate;
		EventDelegate tempDel;

		// Early-out if we've already registered this delegate
		if ( HasListener<TEvent>(del, eventSource) )
		{
			return null;
		}
		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		internalDelegate = (e) => del((TEvent)e);

		listenerLookup[new ListenerLookupKey(del, eventSource)] = internalDelegate;

		if ( eventDelegates.TryGetValue(delsKey, out tempDel) )
		{
			tempDel += internalDelegate;
			eventDelegates[delsKey] = tempDel;
		}
		else
		{
			eventDelegates[delsKey] = internalDelegate;
		}

		return internalDelegate;
	}

	/// <summary>
	///		Stops a specific delegate listening to an event, potentially only on a specific gameObject.
	/// </summary>
	/// <typeparam name="TEvent">The type of IGameEvent that the delegate is listening for</typeparam>
	/// <param name="del">The delegate that is called when the event occurs</param>
	/// <param name="eventSource">The game object the delegate is paying attention to if it's only interested in local events</param>
	private EventManager RemoveDelegate<TEvent>(EventDelegate<TEvent> del, GameObject eventSource = null)
		where TEvent : IGameEvent
	{
		ListenerLookupKey listenerLookupKey = new ListenerLookupKey(del, eventSource);
		GameEventKey delsKey = new GameEventKey(typeof(TEvent), eventSource);
		EventDelegate internalDelegate;
		EventDelegate tempDel;

		if ( listenerLookup.TryGetValue(listenerLookupKey, out internalDelegate) )
		{
			if ( eventDelegates.TryGetValue(delsKey, out tempDel) )
			{
				tempDel -= internalDelegate;
				if ( tempDel == null )
				{
					eventDelegates.Remove(delsKey);
				}
				else
				{
					eventDelegates[delsKey] = tempDel;
				}
			}
			listenerLookup.Remove(listenerLookupKey);
		}

		return this;
	}

	private int GetLimitedListenerCallCount(ListenerLookupKey limitedLookupKey)
	{
		return (limitedListenerLookup.ContainsKey(limitedLookupKey))
			? limitedListenerLookup[limitedLookupKey] : -1;
	}

	private void SetLimitedListenerCallCount(ListenerLookupKey limitedLookupKey, int newCallCount)
	{
		if ( !limitedListenerLookup.ContainsKey(limitedLookupKey) )
		{
			return;
		}
		else if ( newCallCount <= 0 )
		{
			limitedListenerLookup.Remove(limitedLookupKey);
		}
		else
		{
			limitedListenerLookup[limitedLookupKey] = Mathf.Max(newCallCount, -1);
		}
	}

	/// <summary>
	///		Every update cycle the queue is processed, if the queue processing is limited,
	///		a maximum processing time per update can be set after which the events will have
	///		to be processed next update loop.
	/// </summary>
	void Update()
	{
		float timer = 0.0f;
		while ( eventQueue.Count > 0 )
		{
			if ( LimitQueueProcesing &&  timer > QueueProcessTime )
			{
				return;
			}

			TriggerEvent(eventQueue.Dequeue() as GameEvent);

			if ( LimitQueueProcesing )
			{
				timer += Time.deltaTime;
			}
		}
	}

	/// <summary>
	///		Clear records offrom all of the global and local eventDelegates from the event manager
	/// </summary>
	private void RemoveAll()
	{
		eventDelegates.Clear();
		listenerLookup.Clear();
		limitedListenerLookup.Clear();
	}

	private void OnApplicationQuit()
	{
		RemoveAll();
		eventQueue.Clear();
		_Instance = null;
	}
}