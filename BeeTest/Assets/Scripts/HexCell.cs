using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HexCell : MouseInteractable
{
	public Material selectedMat;
	public Material highlightedMat;
	public Material nSelectedMat;
	public Material nHighlightedMat;

	public InteractionStatusMessage interactionStatus = InteractionStatusMessage.None;
	public InteractionStatusMessage nInteractionStatus = InteractionStatusMessage.None;

	public List<HexCell> neighbours;
	public List<GameObject> neighbourGOs;

	[SerializeField, SetProperty("CubePos")]
	private Vector3 cubePos;
	public Vector3 CubePos
	{
		get
		{
			return cubePos;
		}
		set
		{
			cubePos = value;
			positionHashCode = GetPositionHashCode();
		}
	}

	private Vector2? offsetPos = null;
	public Vector2 OffsetPos
	{
		get
		{
			
			if ( offsetPos == null )
			{
				offsetPos = Hex.CubeToOffset((Hex)cubePos);
			}
			return offsetPos.Value;
		}
		
	}

	private int? positionHashCode = null;
	public int PositionHashCode 
	{ 
		get 
		{
 			if ( positionHashCode == null )
			{
				positionHashCode = GetPositionHashCode();
			}
			return positionHashCode.Value; 
		}
	}

	private InteractionEffectManager cellInteractionManager;
	private InteractionEffectManager neighbourInteractionManager;
	
	public bool IsNeighbour(GameObject hexCellGO)
	{
		return (neighbours.Contains(hexCellGO.GetComponent<HexCell>())) ? true : false;
	}

	public bool IsNeighbour(HexCell hexCell)
	{
		return (neighbours.Contains(hexCell)) ? true : false;
	}

	public int GetPositionHashCode()
	{
		return ((Hex)cubePos).GetHashCode();
	}

	public int GetPositionHashCode(Hex pos)
	{
		return pos.GetHashCode();
	}

	public int GetPositionHashCode(Vector3 pos)
	{
		return GetPositionHashCode((Hex)pos);
	}

	private void Awake()
	{
		cellInteractionManager = new InteractionEffectManager(selectedMat, highlightedMat, this.gameObject);
		neighbourInteractionManager = new InteractionEffectManager(nSelectedMat, nHighlightedMat);

		EventManager.Instance
			.AddTargetedListener<InteractionStatusEvent>(OnCellInteractionEvent, this.gameObject)
			.AddTargetedListener<InteractionStatusEvent>(OnNeighbourInteractionEvent, this.gameObject)
			.AddTargetedListener<HexCellEvent>(OnCellEvent, this.gameObject)
			.AddGlobalListener<GridEvent>(OnGridEvent);
	}

	private void OnApplicationQuit()
	{
		EventManager.Instance
			.RemoveListener<InteractionStatusEvent>(OnCellInteractionEvent, this.gameObject)
			.RemoveListener<InteractionStatusEvent>(OnNeighbourInteractionEvent, this.gameObject)
			.RemoveListener<HexCellEvent>(OnCellEvent, this.gameObject)
			.RemoveListener<GridEvent>(OnGridEvent);
	}

	private void OnGridEvent(GridEvent gi)
	{
		switch ( gi.EventMessage )
		{
			case GridEventMessage.CellsCreated:
				SetNeighbours();
				break;
			case GridEventMessage.PathsCreated:
				cellInteractionManager.SetInitialMaterials();
				break;
			default:
				break;
		}
	}

	private void OnCellEvent(HexCellEvent ce)
	{
		switch ( ce.EventMessage )
		{
			case HexCellEventMessage.NeighbourListCreated:
				AddNeighbourInteractables();
				EventManager.Instance.QueueEvent(new HexCellEvent(this.gameObject, HexCellEventMessage.InitializationComplete));
				break;
			default:
				break;
		}
	}

	private void OnCellInteractionEvent(InteractionStatusEvent intStatus)
	{
		cellInteractionManager.OnInteractionEvent(intStatus);
	}

	private void OnNeighbourInteractionEvent(InteractionStatusEvent intStatus)
	{
		neighbourInteractionManager.OnInteractionEvent(intStatus);
	}

	private void SetNeighbours()
	{
		int curNeighbourHash;
		HexCell curNeighbour;

		//	Go through each KeyValuePair in hexGridCells and check if it 
		//	contains any neighbours for each cell.
		//	If it does, record them.
		for ( int i = 0; i < Hex.diagonals.Count; ++i )
		{
			curNeighbourHash = Hex.Neighbor((Hex)cubePos, i).GetHashCode();
			if ( HexGrid.Instance.HexGridGOs.ContainsKey(curNeighbourHash) )
			{
				curNeighbour = HexGrid.Instance.GetHexCell(curNeighbourHash);
				neighbours.Add(curNeighbour);
				neighbourGOs.Add(curNeighbour.gameObject);
			}
		}
		EventManager.Instance.QueueEvent(new HexCellEvent(this.gameObject, HexCellEventMessage.NeighbourListCreated));
	}

	private void AddNeighbourInteractables()
	{
		neighbourInteractionManager.SetInteractables(neighbours.ConvertAll<GameObject>(x => x.gameObject));
	}
}
