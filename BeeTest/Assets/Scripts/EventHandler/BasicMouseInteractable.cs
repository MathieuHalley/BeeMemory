using UnityEngine;
using System.Collections;

public class BasicMouseInteractable : MonoBehaviour {

	private void OnMouseDown()
	{
		EventManager.Instance
			.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteractionMessage.OnMouseDown));
	}
	private void OnMouseOver()
	{
		EventManager.Instance
			.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteractionMessage.OnMouseOver));
	}
}
