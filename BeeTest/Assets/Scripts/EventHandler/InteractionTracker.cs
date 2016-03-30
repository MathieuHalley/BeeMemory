using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionTracker : MonoBehaviour 
{
	private InteractionEffectManager interactionTrackEffectManager;
	public Material selectedMat;
	public Material highlightedMat;

	[SerializeField, SetProperty("InteractionTrack")]
	private List<GameObject> intTrack;
	public List<GameObject> highlightedHexCellGOs;
	public List<GameObject> selectedHexCellGOs;
	private void Awake()
	{
		interactionTrackEffectManager = new InteractionEffectManager(selectedMat, highlightedMat);
		EventManager.Instance
			.AddGlobalListener<InteractionStatusEvent>(OnCellInteractionEvent)
			.AddGlobalListener<MouseInteractionEvent>(OnMouseInteractionEvent);
	}

	private void OnApplicationQuit()
	{
		EventManager.Instance
			.RemoveListener<InteractionStatusEvent>(OnCellInteractionEvent)
			.RemoveListener<MouseInteractionEvent>(OnMouseInteractionEvent);
	}

	private void OnCellInteractionEvent(InteractionStatusEvent intStatus)
	{
		switch( intStatus.EventMessage )
		{
			case InteractionStatusMessage.Selected:
				UpdateInteractionTrack(selectedHexCellGOs, intStatus.EventKey.eventSource);
				break;
			case InteractionStatusMessage.Highlighted:
				UpdateInteractionTrack(highlightedHexCellGOs, intStatus.EventKey.eventSource);
				break;
			default:
				break;
		}
		interactionTrackEffectManager.OnInteractionEvent(intStatus);
	}

	private void UpdateInteractionTrack(List<GameObject> interactionTrack, GameObject go)
	{
		if ( InteractionTrackContains(interactionTrack, go) )
		{
			interactionTrack.Add(go);
		}
		interactionTrackEffectManager.SetInteractables(interactionTrack);
		intTrack = interactionTrack;
	}

	private bool InteractionTrackContains(List<GameObject> interactionTrack, GameObject go)
	{
		return ( !interactionTrack.Contains(go) || interactionTrack[interactionTrack.Count - 1] != go ) ? true : false;
	}

	private void OnMouseInteractionEvent(MouseInteractionEvent mouseInteraction)
	{
		switch ( mouseInteraction.EventMessage )
		{
			case MouseInteractionMessage.OnMouseUp:
			case MouseInteractionMessage.OnMouseDown:
				interactionTrackEffectManager.SetInteractables(new List<GameObject>());
				break;
			default:
			break;
		}
	}
}
