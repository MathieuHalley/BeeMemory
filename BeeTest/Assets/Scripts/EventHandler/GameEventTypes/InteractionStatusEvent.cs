using UnityEngine;
using System.Collections;

[System.Flags]
public enum InteractionStatusMessage
{
	None = 0,
	Highlighted = 1,
	Selected = 2,
}

public class InteractionStatusEvent
	: GameEventWithMessage<InteractionStatusMessage>
{
	public InteractionStatusEvent(InteractionStatusMessage message)
		: this(null, message) { }
	public InteractionStatusEvent(GameObject eventSrc, InteractionStatusMessage message)
		: base(typeof(InteractionStatusEvent), eventSrc, message) { }
}