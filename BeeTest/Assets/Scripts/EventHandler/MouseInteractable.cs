using UnityEngine;
using System.Collections;

public class MouseInteractable : MonoBehaviour 
{
	private void OnMouseDown()
	{
		EventManager.Instance
			.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteractionMessage.OnMouseDown))
			.QueueEvent(new InteractionStatusEvent(this.gameObject, InteractionStatusMessage.Selected));
	}

	private void OnMouseDrag()
	{
		EventManager.Instance
			.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteractionMessage.OnMouseDrag))
			.QueueEvent(new InteractionStatusEvent(this.gameObject, InteractionStatusMessage.Selected));
	}

	private void OnMouseEnter()
	{
		EventManager.Instance
			.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteractionMessage.OnMouseEnter))
			.QueueEvent(new InteractionStatusEvent(this.gameObject, InteractionStatusMessage.Highlighted));
	}

	private void OnMouseExit()
	{
		EventManager.Instance
			.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteractionMessage.OnMouseExit))
			.QueueEvent(new InteractionStatusEvent(this.gameObject, ~InteractionStatusMessage.Highlighted));
	}

	private void OnMouseUp()
	{
		EventManager.Instance
			.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteractionMessage.OnMouseUp))
			.QueueEvent(new InteractionStatusEvent(this.gameObject, ~InteractionStatusMessage.Selected));
	}

	private void OnMouseUpAsButton()
	{
		EventManager.Instance
			.QueueEvent(new MouseInteractionEvent(MouseInteractionMessage.OnMouseUpAsButton))
			.QueueEvent(new InteractionStatusEvent(~InteractionStatusMessage.Selected));
	}	
}
