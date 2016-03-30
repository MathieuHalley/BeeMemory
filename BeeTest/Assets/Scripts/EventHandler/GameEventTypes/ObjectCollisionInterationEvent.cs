using UnityEngine;
using System.Collections;

public enum ObjectCollisionInteractionMessage
{
	OnCollisionEnter,
	OnCollisionEnter2D,
	OnCollisionExit,
	OnCollisionExit2D,
	OnCollisionStay,
	OnCollisionStay2D
}

public class ObjectCollisionInteractionEvent
	: GameEventWithMessageAndData<ObjectCollisionInteractionMessage, Collision>
{
	public ObjectCollisionInteractionEvent(ObjectCollisionInteractionMessage message, Collision interactionData)
		: this(null, message, interactionData) { }
	public ObjectCollisionInteractionEvent(GameObject eventSrc, ObjectCollisionInteractionMessage message, Collision interactionData)
		: base(typeof(ObjectCollisionInteractionEvent), eventSrc, message, interactionData) { }
}

public class ObjectCollision2DInteractionEvent
	: GameEventWithMessageAndData<ObjectCollisionInteractionMessage, Collision2D>
{
	public ObjectCollision2DInteractionEvent(ObjectCollisionInteractionMessage message, Collision2D interactionData)
		: this(null, message, interactionData) { }
	public ObjectCollision2DInteractionEvent(GameObject eventSrc, ObjectCollisionInteractionMessage message, Collision2D interactionData)
		: base(typeof(ObjectCollision2DInteractionEvent), eventSrc, message, interactionData) { }
}