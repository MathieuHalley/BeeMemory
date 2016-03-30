using UnityEngine;
using System.Collections;

public interface IGameEventWithMessage<TEventMessage>
	: IGameEvent
	where TEventMessage : struct, System.IConvertible
{
	IGameEvent SetEventMessage(TEventMessage message);
}

public abstract class GameEventWithMessage<TEventMessage> : GameEvent, IGameEventWithMessage<TEventMessage>
	where TEventMessage : struct, System.IConvertible
{
	protected TEventMessage eventMessage;
	public TEventMessage EventMessage
	{
		get
		{
			return eventMessage;
		}
	}

	public GameEventWithMessage(GameObject eventSrc, TEventMessage message)
		: this(typeof(GameEventWithMessage<TEventMessage>), eventSrc, message) { }

	public GameEventWithMessage(GameEventKey eventKey, TEventMessage message)
		: base(eventKey)
	{
		SetEventMessage(message);		
	}

	public GameEventWithMessage(System.Type eventType, GameObject eventSrc, TEventMessage message)
		: base(eventType, eventSrc)
	{
		SetEventMessage(message);
	}

	public IGameEvent SetEventMessage(TEventMessage message)
	{
		eventMessage = message;
		return this;
	}

	public override string ToString()
	{
		return base.ToString() + " | Message: " + EventMessage.ToString();
	}
}