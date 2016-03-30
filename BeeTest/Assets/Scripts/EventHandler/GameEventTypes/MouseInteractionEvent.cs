using UnityEngine;
using System.Collections;

public enum MouseInteractionMessage
{
	OnMouseDown,
	OnMouseDrag,
	OnMouseEnter,
	OnMouseExit,
	OnMouseOver,
	OnMouseUp,
	OnMouseUpAsButton
}

public class MouseInteractionEvent
	: GameEventWithMessage<MouseInteractionMessage>
{
	public MouseInteractionEvent(MouseInteractionMessage message)
		: this(null, message) { }
	public MouseInteractionEvent(GameObject eventSrc, MouseInteractionMessage message)
		: base(typeof(MouseInteractionEvent), eventSrc, message) { }
}