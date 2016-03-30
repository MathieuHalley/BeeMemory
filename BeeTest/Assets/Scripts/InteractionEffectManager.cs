using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionEffectManager 
{
	public static Dictionary<int, Material> initialMats;
	public Material selectedMat;
	public Material highlightedMat;
	public List<GameObject> interactables;
	public InteractionStatusMessage interactionStatus;

	public InteractionEffectManager(Material selected, Material highlighted)
	{
		selectedMat = selected;
		highlightedMat = highlighted;
		interactionStatus = InteractionStatusMessage.None;
		interactables = new List<GameObject>();
		if ( initialMats == null )
		{
			initialMats = new Dictionary<int, Material>();
		}
	}

	public InteractionEffectManager(Material selected, Material highlighted, GameObject go) 
		: this(selected, highlighted)
	{
		AddInteractable(go);
		SetInitialMaterial(go);
	}

	public InteractionEffectManager(Material selected, Material highlighted, List<GameObject> gos) 
		: this(selected, highlighted)
	{
		SetInteractables(gos);
		SetInitialMaterials();
	}


	public void OnInteractionEvent(InteractionStatusEvent intStatus)
	{
		switch ( intStatus.EventMessage )
		{
			case InteractionStatusMessage.Selected:
				UpdateInteractionStatus(InteractionStatusMessage.Selected);
				break;
			case InteractionStatusMessage.Highlighted:
				UpdateInteractionStatus(InteractionStatusMessage.Highlighted);
				break;
			case ~InteractionStatusMessage.Selected:
				UpdateInteractionStatus(~InteractionStatusMessage.Selected);
				break;
			case ~InteractionStatusMessage.Highlighted:
				UpdateInteractionStatus(~InteractionStatusMessage.Highlighted);
				break;
			default:
				UpdateInteractionStatus(InteractionStatusMessage.None);
				break;
		}
	}

	public void AddInteractable(GameObject go)
	{
		interactables.Add(go);
	}

	public void RemoveInteractable(GameObject go)
	{
		interactables.Remove(go);
	}

	public void SetInteractables(List<GameObject> gos)
	{
		interactables = gos;
	}

	public void SetInitialMaterials()
	{
		if ( initialMats == null )
		{
			initialMats = new Dictionary<int, Material>(interactables.Count);
		}

		foreach ( GameObject go in interactables )
		{
			SetInitialMaterial(go);
		}
	}

	public void UpdateInteractionStatus(InteractionStatusMessage intStatus)
	{
		string matsStr = "";
		foreach (KeyValuePair<int,Material> mats in initialMats)
		{
			matsStr += mats.ToString() + ", ";
		}

//		Debug.Log(matsStr);

		if ( intStatus > 0 )
		{
			interactionStatus |= intStatus;
		}
		else if ( intStatus < 0 )
		{
			interactionStatus &= intStatus;
		}
		else
		{
			interactionStatus = InteractionStatusMessage.None;
		}

		UpdateInteractionEffects();
	}

	public void UpdateInteractionEffects()
	{
		Renderer renderer;
		int hash;
		foreach ( GameObject go in interactables )
		{
			renderer = go.GetComponent<Renderer>();
			hash = go.GetHashCode();
 
			if ( IsSelected() )
			{
				renderer.material = selectedMat;
			}
			else if ( IsHighlighted() )
			{
				renderer.material = highlightedMat;
			}
			else if (initialMats.ContainsKey(hash) && initialMats[hash] != null) 
			{
				renderer.material = initialMats[hash];
			}
		}
	}

	public bool IsSelected()
	{
		return ((interactionStatus & InteractionStatusMessage.Selected) == InteractionStatusMessage.Selected ) ? true : false;
	}

	public bool IsHighlighted()
	{
		return ((interactionStatus & InteractionStatusMessage.Highlighted) == InteractionStatusMessage.Highlighted ) ? true : false;
	}

	private void SetInitialMaterial(GameObject go)
	{
		initialMats[go.GetHashCode()] = (interactionStatus == InteractionStatusMessage.None) ? go.GetComponent<Renderer>().material : null;
	}

	private void RemoveInitialMaterial(GameObject go)
	{
		initialMats.Remove(go.GetHashCode());
	}
}
