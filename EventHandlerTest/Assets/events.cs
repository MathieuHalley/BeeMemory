using UnityEngine;
using System.Collections;

public class GameEvent
{
	protected Eppy.Tuple<System.Type, GameObject> eventKey;
	public Eppy.Tuple<System.Type, GameObject> EventKey 
	{ 
		get 
		{
			if ( eventKey == null )
			{
				eventKey = new Eppy.Tuple<System.Type, GameObject>(GetType(), gameObject);
			}
			return eventKey;
		}
	} 
	public GameObject gameObject { get; protected set; }
}

public enum MouseInteraction
{
	OnMouseDown,
	OnMouseDrag,
	OnMouseEnter,
	OnMouseExit,
	OnMouseOver,
	OnMouseUp,
	OnMouseUpAsButton
}

public enum CollisionInteraction
{
	OnCollisionEnter,
	OnCollisionEnter2D,
	OnCollisionExit,
	OnCollisionExit2D,
	OnCollisionStay,
	OnCollisionStay2D
}

public enum TriggerInteraction
{
	OnTriggerEnter,
	OnTriggerEnter2D,
	OnTriggerExit,
	OnTriggerExit2D,
	OnTriggerStay,
	OnTriggerStay2D
}

public enum GenericMessage
{
	Awake,
	FixedUpdate,
	LateUpdate,
	OnEnable,
	OnDisable,
	OnDestroy,
	OnGUI,
	Start,
	Update,
}

public class MouseInteractionEvent : GameEvent
{
	public MouseInteraction interaction { get; private set; }

	public MouseInteractionEvent(GameObject go, MouseInteraction mi)
	{
		gameObject = go;
		interaction = mi;
	}
}

public class CollisionInteractionEvent : GameEvent
{
	public Collision collision { get; private set; }
	public Collision2D collision2D { get; private set; }
	public CollisionInteraction interaction { get; private set; }

	public CollisionInteractionEvent(GameObject go, CollisionInteraction ci, Collision c)
	{
		gameObject = go;
		interaction = ci;
		collision = c;
		collision2D = null;
	}

	public CollisionInteractionEvent(GameObject go, CollisionInteraction ci, Collision2D c)
	{
		gameObject = go;
		interaction = ci;
		collision = null;
		collision2D = c;
	}
}

public class TriggerInteractionEvent : GameEvent
{
	public Collider collider { get; private set; }
	public Collider2D collider2D { get; private set; }
	public TriggerInteraction interaction { get; private set; }

	public TriggerInteractionEvent(GameObject go, TriggerInteraction ti, Collider c)
	{
		gameObject = go;
		interaction = ti;
		collider = c;
		collider2D = null;
	}

	public TriggerInteractionEvent(GameObject go, TriggerInteraction ti, Collider2D c)
	{
		gameObject = go;
		interaction = ti;
		collider = null;
		collider2D = c;
	}
}

public class GeneralMessageEvent : GameEvent
{
	public GenericMessage message { get; private set; }

	public GeneralMessageEvent(GameObject go, GenericMessage g)
	{
		gameObject = go;
		message = g;
	}
}