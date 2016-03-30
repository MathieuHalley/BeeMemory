using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
Hexagon sizing
In the vertical orientation, the width of a hexagon is width = size * 2. The horizontal distance between adjacent hexes is horiz = width * 3/4.

The height of a hexagon is height = sqrt(3)/2 * width. The vertical distance between adjacent hexes is vert = height.
*/

public class HexGrid : MonoBehaviour
{
	public Dictionary<int, GameObject> HexGridGOs { get; private set; }

	public GameObject cellPrefab;

	public const float cellSize = 0.5f;
	public const int cols = 9;
	public const int rows = 8;
	public int cellCount;
	private int cellsWithNeighbours;

	// override so we don't have the typecast the object
	private static HexGrid _Instance = null;
	public static HexGrid Instance
	{
		get
		{
			if ( _Instance == null )
			{
				_Instance = GameObject.FindObjectOfType(typeof(HexGrid)) as HexGrid;
			}
			return _Instance;
		}
	}

	public void Awake ()
	{
		EventManager.Instance
			.AddGlobalListener<GridEvent>(OnGridEvent)
			.AddGlobalListener<HexCellEvent>(OnCellEvent);

		cellCount = cols * rows;
		cellsWithNeighbours = 0;
		HexGridGOs = new Dictionary<int,GameObject>(cols * rows);

		CreateGrid(cols, rows);
		//	Queue GridEventMessage: Grid Cells Created
		EventManager.Instance.QueueEvent(new GridEvent(this.gameObject, GridEventMessage.CellsCreated));
	}
	
	public void OnApplicationQuit()
	{
		EventManager.Instance
			.RemoveListener<GridEvent>(OnGridEvent)
			.RemoveListener<HexCellEvent>(OnCellEvent);		
	}

	//	Get a GameObject from its HexCell
	public GameObject GetHexCellGO(HexCell hexCell)
	{
		return hexCell.gameObject;
	}

	//	Get a GameObject from its PositionHash
	public GameObject GetHexCellGO(int cellHash)
	{
		return HexGridGOs[cellHash];
	}

	//	Get a HexCell from its GameObject
	public HexCell GetHexCell(GameObject hexGO)
	{
		return hexGO.GetComponent<HexCell>();
	}

	//	Get a HexCell from its PositionHash
	public HexCell GetHexCell(int cellHash)
	{
		return HexGridGOs[cellHash].GetComponent<HexCell>();
	}

	private void OnCellEvent(HexCellEvent me)
	{
//		print("HexGrid - OnCellEvent" + me.ToString());
		switch ( me.EventMessage )
		{
			case HexCellEventMessage.NeighbourListCreated:
				++cellsWithNeighbours;
				if ( cellsWithNeighbours >= cellCount )
				{
					//	Queue GridEventMessage: All cell neighbour lists are created
					EventManager.Instance.QueueEvent(new GridEvent(this.gameObject, GridEventMessage.NeighbourListsCreated));
				}
				break;
			default:
				break;
		}
	}

	private void OnGridEvent(GridEvent ge)
	{
		switch ( ge.EventMessage )
		{
			case GridEventMessage.PathsCreated:
			EventManager.Instance.QueueEvent(new GridEvent(this.gameObject, GridEventMessage.InitializationComplete));	
				break;
			default:
				break;
		}
	}

	private void CreateGrid(int col, int row)
	{
		float width, horiz, height, vert, xOffset, yOffset, xLocal, yLocal, alternateRowOffset;
		int cellPosHash;
		HexCell hexCell;
		GameObject newCellGO;

		width = cellSize * 2;				//	Cell width
		horiz = width * 0.75f;				//	Horizontal space between cells
		height = Mathf.Sqrt(3)/2 * width;	//	Cell height
		vert = height;						//	Vertical space between cells

		xOffset = horiz * col * 0.5f;
		yOffset = vert * row * 0.5f;

		for (int i = 0; i < col; ++i)
		{
			xLocal = horiz * i - xOffset;
			alternateRowOffset = (i % 2 != 0) ? height * 0.5f : 0;
			for ( int j = 0; j < row; ++j )
			{
				yLocal = vert * j + alternateRowOffset - yOffset;

				newCellGO = Instantiate<GameObject>(cellPrefab);
				newCellGO.name = "Cell (" + i + "," + j + ")";
				newCellGO.transform.parent = this.transform;
				newCellGO.transform.localPosition = new Vector3(xLocal, yLocal, 0);
				hexCell = newCellGO.GetComponent<HexCell>();
				hexCell.CubePos = (Vector3)Hex.OffsetToCube(i, j);

				//	Add the hexCell to the playfield & hexgrid dictionaries
				cellPosHash = hexCell.GetPositionHashCode();
				if ( !HexGridGOs.ContainsKey(cellPosHash) )
				{
					HexGridGOs.Add(cellPosHash, newCellGO);
				}
				else
				{
					HexGridGOs[cellPosHash] = newCellGO;
				}
			}
		}
	}
}