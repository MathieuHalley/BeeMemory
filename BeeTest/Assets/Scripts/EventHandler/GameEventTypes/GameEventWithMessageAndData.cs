using UnityEngine;
using System.Collections;

public interface IGameEventWithMessageAndData<TEventData>
{
	TEventData EventData
	{
		get;
	}
	IGameEvent SetEventData(TEventData eData);
}

public abstract class GameEventWithMessageAndData<TEventMessage, TEventData> : GameEvent, IGameEventWithMessage<TEventMessage>, IGameEventWithMessageAndData<TEventData>
	where TEventMessage : struct, System.IConvertible
{
	protected TEventData eventData;
	public TEventData EventData
	{
		get
		{
			return eventData;
		}
	}

	protected TEventMessage eventMessage;
	public TEventMessage EventMessage
	{
		get
		{
			return eventMessage;
		}
	}

	public GameEventWithMessageAndData(GameObject eventSrc, TEventMessage message, TEventData eventData)
		: this(typeof(GameEventWithMessageAndData<TEventMessage, TEventData>), eventSrc, message, eventData) { }

	public GameEventWithMessageAndData(GameEventKey eventKey, TEventMessage message, TEventData eventData)
		: base(eventKey)
	{
		SetEventMessage(message);
		SetEventData(eventData);
	}

	public GameEventWithMessageAndData(System.Type eventType, GameObject eventSrc, TEventMessage message, TEventData eventData)
		: base(eventType, eventSrc)
	{
		SetEventMessage(message);
		SetEventData(eventData);
	}

	public IGameEvent SetEventData(TEventData interactionData)
	{
		eventData = interactionData;
		return this;
	}

	public IGameEvent SetEventMessage(TEventMessage message)
	{
		eventMessage = message;
		return this;
	}

	public override string ToString()
	{
		return base.ToString() +
			((EventData == null)
			? ""
			: " | " + EventData.GetType().ToString() + ": " + EventData.ToString());
	}
}