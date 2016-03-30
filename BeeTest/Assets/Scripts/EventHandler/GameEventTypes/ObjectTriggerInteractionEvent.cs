using UnityEngine;
using System.Collections;

public enum ObjectTriggerInteractionMessage
{
	OnTriggerEnter,
	OnTriggerEnter2D,
	OnTriggerExit,
	OnTriggerExit2D,
	OnTriggerStay,
	OnTriggerStay2D
}

public class ObjectTriggerInteractionEvent
	: GameEventWithMessageAndData<ObjectTriggerInteractionMessage, Collider>
{
	public ObjectTriggerInteractionEvent(ObjectTriggerInteractionMessage message, Collider interactionData)
		: this(null, message, interactionData) { }
	public ObjectTriggerInteractionEvent(GameObject eventSrc, ObjectTriggerInteractionMessage message, Collider interactionData)
		: base(eventSrc, message, interactionData) { }
}

public class ObjectTrigger2DInteractionEvent
	: GameEventWithMessageAndData<ObjectTriggerInteractionMessage, Collider2D>
{
	public ObjectTrigger2DInteractionEvent(ObjectTriggerInteractionMessage message, Collider2D interactionData)
		: this(null, message, interactionData) { }
	public ObjectTrigger2DInteractionEvent(GameObject eventSrc, ObjectTriggerInteractionMessage message, Collider2D interactionData)
		: base(eventSrc, message, interactionData) { }
}