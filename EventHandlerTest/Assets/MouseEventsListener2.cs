using UnityEngine;
using System.Collections;

public class MouseEventsListener2 : MonoBehaviour
{

	// Use this for initialization
	void OnEnable()
	{
		//	Add a listener for this mouse interactions involving this object
		s_EventManager.Instance.AddListener<MouseInteractionEvent>(OnMouseGlobalInteraction);
		s_EventManager.Instance.AddListener<MouseInteractionEvent>(OnMouseLocalInteraction, this.gameObject);
	}

	void OnDisable()
	{
		//	Remove a listener for this mouse interactions involving this object
		s_EventManager.Instance.RemoveListener<MouseInteractionEvent>(OnMouseGlobalInteraction);
		s_EventManager.Instance.RemoveListener<MouseInteractionEvent>(OnMouseLocalInteraction, this.gameObject);
	}

	void OnMouseDown()
	{
		s_EventManager.Instance.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteraction.OnMouseDown));
	}

	void OnMouseGlobalInteraction(MouseInteractionEvent e)
	{
		if ( e.interaction == MouseInteraction.OnMouseDown )
			print("something was clicked on");
	}
	void OnMouseLocalInteraction(MouseInteractionEvent e)
	{
		if ( e.interaction == MouseInteraction.OnMouseDown )
			print("I was clicked on");
	}
}
