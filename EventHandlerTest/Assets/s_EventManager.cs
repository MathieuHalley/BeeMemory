using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class s_EventManager : MonoBehaviour
{
	public bool LimitQueueProcesing = false;
	public float QueueProcessTime = 0.0f;
	private static s_EventManager s_Instance = null;
	private Queue m_eventQueue = new Queue();

	public delegate void EventDelegate<T>(T e) 
		where T : GameEvent;
	private delegate void EventDelegate(GameEvent e);

	private TupleKeyDictionary<System.Type, GameObject, EventDelegate> lDelegates = new TupleKeyDictionary<System.Type, GameObject, EventDelegate>();
	private TupleKeyDictionary<System.Delegate, GameObject, EventDelegate> lDelegateLookup = new TupleKeyDictionary<System.Delegate, GameObject, EventDelegate>();
	private TupleKeyDictionary<System.Delegate, GameObject, bool> lOnceLookups = new TupleKeyDictionary<System.Delegate, GameObject, bool>();

	private Dictionary<System.Type, EventDelegate> gDelegates = new Dictionary<System.Type, EventDelegate>();
	private Dictionary<System.Delegate, EventDelegate> gDelegateLookup = new Dictionary<System.Delegate, EventDelegate>();
	private Dictionary<System.Delegate, bool> gOnceLookups = new Dictionary<System.Delegate, bool>();

	// override so we don't have the typecast the object
	public static s_EventManager Instance
	{
		get
		{
			if ( s_Instance == null )
			{
				s_Instance = GameObject.FindObjectOfType(typeof(s_EventManager)) as s_EventManager;
			}
			return s_Instance;
		}
	}

	private EventDelegate AddLocalDelegate<T>(EventDelegate<T> del, GameObject go) 
		where T : GameEvent
	{
		Eppy.Tuple<System.Delegate, GameObject> delLookupKey = new Eppy.Tuple<System.Delegate, GameObject>(del, go);

		// Early-out if we've already registered this delegate
		if (lDelegateLookup.ContainsKey(delLookupKey))
			return null;
		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		EventDelegate internalDelegate = (e) => del((T)e);

		lDelegateLookup[delLookupKey] = internalDelegate;

		EventDelegate tempDel;

		Eppy.Tuple<System.Type, GameObject> delsKey = new Eppy.Tuple<System.Type, GameObject>(typeof(T), go);

		if ( lDelegates.TryGetValue(delsKey, out tempDel) )
		{
			lDelegates[delsKey] = tempDel += internalDelegate;
		}
		else
		{
			lDelegates[delsKey] = internalDelegate;
		}

		return internalDelegate;
	}

	private EventDelegate AddGlobalDelegate<T>(EventDelegate<T> del)
		where T : GameEvent
	{
		// Early-out if we've already registered this delegate
		if (gDelegateLookup.ContainsKey(del))
			return null;
		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		EventDelegate internalDelegate = (e) => del((T)e);

		gDelegateLookup[del] = internalDelegate;

		EventDelegate tempDel;

		System.Type delsKey = typeof(T);

		if ( gDelegates.TryGetValue(delsKey, out tempDel) )
		{
			gDelegates[delsKey] = tempDel += internalDelegate;
		}
		else
		{
			gDelegates[delsKey] = internalDelegate;
		}

		return internalDelegate;
	}

	public void AddListener<T>(EventDelegate<T> del, GameObject go = null) 
		where T : GameEvent
	{
		if ( go == null )
			AddGlobalDelegate<T>(del);
		else
			AddLocalDelegate<T>(del, go);
	}

	public void AddListenerOnce<T>(EventDelegate<T> del, GameObject go = null)
		where T : GameEvent
	{
		EventDelegate result = (go == null) ? AddGlobalDelegate<T>(del) : AddLocalDelegate<T>(del, go);

		if ( result != null )
		{
			// remember this is only called once
			if ( go == null )
				gOnceLookups[result] = true;			
			else
				lOnceLookups[new Eppy.Tuple<System.Delegate, GameObject>(result, go)] = true;
		}
	}

	public void RemoveListener<T>(EventDelegate<T> del, GameObject go = null) 
		where T : GameEvent
	{
		if ( go == null )
			RemoveGlobalDelegate<T>(del);
		else
			RemoveLocalDelegate<T>(del, go);


	}

	public void RemoveLocalDelegate<T>(EventDelegate<T> del, GameObject go)
	where T : GameEvent
	{
		EventDelegate internalDelegate;
		Eppy.Tuple<System.Delegate, GameObject> delLookupKey = new Eppy.Tuple<System.Delegate, GameObject>(del, go);
		Eppy.Tuple<System.Type, GameObject> delsKey = new Eppy.Tuple<System.Type, GameObject>(typeof(T), go);

		if ( lDelegateLookup.TryGetValue(delLookupKey, out internalDelegate) )
		{
			EventDelegate tempDel;
			if ( lDelegates.TryGetValue(delsKey, out tempDel) )
			{
				tempDel -= internalDelegate;
				if ( tempDel == null )
					lDelegates.Remove(delsKey);
				else
					lDelegates[delsKey] = tempDel;
			}
			lDelegateLookup.Remove(delLookupKey);
		}
	}

	public void RemoveGlobalDelegate<T>(EventDelegate<T> del)
		where T : GameEvent
	{
		EventDelegate internalDelegate;
		System.Type delsKey = typeof(T);

		if ( gDelegateLookup.TryGetValue(del, out internalDelegate) )
		{
			EventDelegate tempDel;
			if ( gDelegates.TryGetValue(delsKey, out tempDel) )
			{
				tempDel -= internalDelegate;
				if ( tempDel == null )
					gDelegates.Remove(delsKey);
				else
					gDelegates[delsKey] = tempDel;
			}
			gDelegateLookup.Remove(del);
		}
	}

	public void RemoveAll()
	{
		gDelegates.Clear();
		gDelegateLookup.Clear();
		gOnceLookups.Clear();
		lDelegates.Clear();
		lDelegateLookup.Clear();
		lOnceLookups.Clear();
	}

	public bool HasListener<T>(EventDelegate<T> del, GameObject go = null) 
		where T : GameEvent
	{
		if ( go == null )
			return gDelegateLookup.ContainsKey(del);
		else
			return lDelegateLookup.ContainsKey(new Eppy.Tuple<System.Delegate, GameObject>(del, go));
	}

	public void TriggerEvent(GameEvent e)
	{
		EventDelegate del;
		Eppy.Tuple<System.Delegate, GameObject> onceLookupKey;

		//	Trigger all queued local events
		if ( lDelegates.TryGetValue(e.EventKey, out del) )
		{
			del.Invoke(e);

			// remove listeners which should only be called once
			foreach ( EventDelegate k in lDelegates[e.EventKey].GetInvocationList() )
			{
				onceLookupKey = new Eppy.Tuple<System.Delegate, GameObject>(k, e.gameObject);
				if ( lOnceLookups.ContainsKey(onceLookupKey) )
					lOnceLookups.Remove(onceLookupKey);
			}
		}
		else
		{
			Debug.LogWarning("Event: " + e.GetType() + " has no local listeners");
		}

		//	Trigger all queued global events
		if ( gDelegates.TryGetValue(e.GetType(), out del) )
		{
			del.Invoke(e);

			// remove listeners which should only be called once
			foreach ( EventDelegate k in gDelegates[e.GetType()].GetInvocationList() )
			{
				onceLookupKey = new Eppy.Tuple<System.Delegate, GameObject>(k, e.gameObject);
				if ( gOnceLookups.ContainsKey(del) )
					gOnceLookups.Remove(del);
			}
		}
		else
		{
			Debug.LogWarning("Event: " + e.GetType() + " has no global listeners");
		}
	}

	//Inserts the event into the current queue.
	public bool QueueEvent(GameEvent e)
	{
		if ( !lDelegates.ContainsKey(e.EventKey) && !gDelegates.ContainsKey(e.GetType()) )
		{
			Debug.LogWarning("s_EventManager: QueueEvent failed due to no listeners for event: " + e.GetType());
			return false;
		}

		m_eventQueue.Enqueue(e);
		return true;
	}

	//Every update cycle the queue is processed, if the queue processing is limited,
	//a maximum processing time per update can be set after which the events will have
	//to be processed next update loop.
	void Update()
	{
		float timer = 0.0f;
		while ( m_eventQueue.Count > 0 )
		{
			if ( LimitQueueProcesing )
			{
				if ( timer > QueueProcessTime )
					return;
			}

			GameEvent evt = m_eventQueue.Dequeue() as GameEvent;
			TriggerEvent(evt);

			if ( LimitQueueProcesing )
				timer += Time.deltaTime;
		}
	}

	public void OnApplicationQuit()
	{
		RemoveAll();
		m_eventQueue.Clear();
		s_Instance = null;
	}
}