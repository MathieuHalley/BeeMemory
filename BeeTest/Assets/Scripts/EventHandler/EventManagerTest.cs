using UnityEngine;
using System.Collections;

public class EventManagerTest : MonoBehaviour 
{
	public GameObject cube;
	// Use this for initialization
	void Start () 
	{
		EventManager.Instance
			.AddGlobalListener<MouseInteractionEvent>(OnMouseInteraction)
			.AddGlobalListener<MouseInteractionEvent>(OnMouseOtherInteraction,3);		
	}

	void OnMouseInteraction(MouseInteractionEvent me)
	{
		print("OnMouseInteraction" + me.ToString());
		switch ( me.EventMessage )
		{
			case MouseInteractionMessage.OnMouseDown:
			print("boop");
			break;
			case MouseInteractionMessage.OnMouseOver:
			print("MouseOver");
			break;
			default:
			break;
		}
	}

	void OnInteraction(InteractionStatusEvent e)
	{
		print("OnInteraction" + e.ToString());
	}

	void OnMouseOtherInteraction(MouseInteractionEvent me)
	{
		print("OnMouseOtherInteraction" + me.ToString());
		switch ( me.EventMessage )
		{
			case MouseInteractionMessage.OnMouseDown:
			print("boop");
			break;
			case MouseInteractionMessage.OnMouseOver:
			print("MouseOver");
			break;
			default:
			break;
		}
	}
}
