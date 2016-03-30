using UnityEngine;
using System.Collections;

public class MouseEventsListener1 : MonoBehaviour 
{

	// Use this for initialization
	void OnEnable () 
	{
		//	Add a listener for this mouse interactions involving this object
		s_EventManager.Instance.AddListener<MouseInteractionEvent>(OnMouseInteraction);
	}

	void OnDisable()
	{
		//	Remove a listener for this mouse interactions involving this object
		s_EventManager.Instance.RemoveListener<MouseInteractionEvent>(OnMouseInteraction);
	}

	void OnMouseDown()
	{
		s_EventManager.Instance.QueueEvent(new MouseInteractionEvent(this.gameObject, MouseInteraction.OnMouseDown));
	}

	void OnMouseInteraction(MouseInteractionEvent e)
	{
		if ( e.interaction == MouseInteraction.OnMouseDown )
			print("boop1");
	}
}
