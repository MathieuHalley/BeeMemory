using UnityEngine;
using System.Collections;

public struct GameEventKey
{
	public System.Type eventType;
	public GameObject eventSource;

	public GameEventKey(System.Type eventTp, GameObject eventSrc)
	{
		eventType = eventTp;
		eventSource = eventSrc;
	}

	public override string ToString()
	{
		return this.GetType().ToString() +
			" | Event Type: " + eventType.ToString() +
			" | Event Source: " + ((eventSource != null) ? eventSource.ToString() : "Global");
	}
}

//	Decorator Component
public interface IGameEvent
{
	GameEventKey EventKey { get; }
	IGameEvent SetEventKey(GameEventKey eKey);
	IGameEvent SetEventKey(System.Type eventType, GameObject eventSrc);
	string ToString();
}

//	Concrete
public abstract class GameEvent : IGameEvent
{
	private GameEventKey eventKey;
	public GameEventKey EventKey
	{
		get
		{
			return eventKey;
		}
	}

	public GameEvent(GameEventKey eKey)
	{
		SetEventKey(eKey);
	}

	public GameEvent(System.Type eventType, GameObject eventSrc = null)
	{
		SetEventKey(eventType, eventSrc);
	}

	public IGameEvent SetEventKey(GameEventKey eKey)
	{
		eventKey = eKey;
		return this;
	}

	public IGameEvent SetEventKey(System.Type eventType, GameObject eventSrc = null)
	{
		eventKey = new GameEventKey(eventType, eventSrc);
		return this;
	}

	public override string ToString()
	{
		return this.GetType().ToString() + " " + EventKey.ToString();
	}
}