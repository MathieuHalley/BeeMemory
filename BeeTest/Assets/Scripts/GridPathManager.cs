using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridPathManager : MonoBehaviour 
{
	private InteractionEffectManager pathInteractionEffectManager;
	public GameObject pathPrefab;

	public List<List<GameObject>> paths;
	public List<Material> pathMaterials;
	public List<Color> pathColors;

	public Material selectedMat;
	public Material highlightedMat;

	public int minPathLength = 3;
	public int maxPathLength = 10;

	public int pathCellCount;
	public int PathCellCount
	{
		get
		{
			pathCellCount = GetPathCellCount();
			return pathCellCount;
		}
	}

	public int GetPathCellCount()
	{
		int pathCellCount = 0;

		for (int i = 0; i < paths.Count; ++i)
		{
			pathCellCount += paths[i].Count;
		}
		return pathCellCount;
	}

	public bool IsOnAPath(GameObject cellGO)
	{
		return ( GetPath(cellGO) != null ) ? true : false;
	}

	public List<GameObject> GetPath(GameObject go)
	{
		if ( go == null || paths == null )
		{
			return null;
		}

		foreach ( List<GameObject> path in paths )
		{
			if ( path.Contains(go) )
			{
				return path;
			}
		}
		return null;
	}

	public int GetPathID(GameObject go)
	{
		List<GameObject> path = GetPath(go);
		if ( path == null )
		{
			return -1;
		}

		for ( int i = 0; i < paths.Count; ++i )
		{
			if (paths[i].Contains(go))
				return i;
		}

		return -1;
	}

	private void Awake()
	{
		pathInteractionEffectManager = new InteractionEffectManager(selectedMat, highlightedMat);
		EventManager.Instance
			.AddGlobalListener<GridEvent>(OnGridEvent)
			.AddGlobalListener<InteractionStatusEvent>(OnPathInteractionEvent);
	}

	private void OnApplicationQuit()
	{
		EventManager.Instance
			.RemoveListener<GridEvent>(OnGridEvent)
			.RemoveListener<InteractionStatusEvent>(OnPathInteractionEvent);
	}

	private void OnGridEvent(GridEvent ge)
	{
		switch ( ge.EventMessage )
		{
			case GridEventMessage.NeighbourListsCreated:
				CreatePaths(3,10,8);
				DrawPaths();
				break;
			case GridEventMessage.PathsCreated:
				EventManager.Instance.QueueEvent(new GridEvent(this.gameObject, GridEventMessage.InitializationComplete));
				break;
			default:
				break;
		}
	}

	private void OnPathInteractionEvent(InteractionStatusEvent intStatus)
	{
		List<GameObject> path = GetPath(intStatus.EventKey.eventSource);
		
		if ( path == null )
		{
			return;
		}

		pathInteractionEffectManager.SetInteractables(path);
		pathInteractionEffectManager.OnInteractionEvent(intStatus);
	}
	/*
	 * Divide the grid into a number of paths, each between minLength and maxLength
	 * Once the grid has been divided into paths, select numPaths from the candidates
	 */
	private void CreatePaths(int minLength, int maxLength, int numPaths)
	{
		GameObject candidate;
		List<GameObject> curPath = new List<GameObject>();
		//	candidateList is initially a shuffled list of all of the hexes on the grid
		List<GameObject> candidateList = HexGrid.Instance.HexGridGOs.Values.ToList();

		minPathLength = minLength;
		maxPathLength = maxLength;

		paths = new List<List<GameObject>>(numPaths);
		candidateList = new ShuffleList<GameObject>().Shuffle(candidateList);

		for ( int i = 0; i < candidateList.Count; ++i )
		{
			for ( int j = candidateList.Count - 1; j >= 0; --j )
			{
				if ( j >= candidateList.Count )
				{
					continue;
				}

				candidate = candidateList[j];
				if ( IsOnAPath(candidate) )
				{
					candidateList.RemoveAt(j);
					continue;
				}

				curPath = CreatePath(candidate, maxPathLength);

				if ( curPath.Count >= minPathLength )
				{
					paths.Add(curPath);
				}
			}

			if ( paths.Count >= numPaths )
			{
				break;
			}
		}
		ExtendShortPaths();
		EventManager.Instance.QueueEvent(new GridEvent(this.gameObject, GridEventMessage.PathsCreated));
	}

	private List<GameObject> CreatePath(GameObject seed, int maxPathLength = 10)
	{
		List<GameObject> path = new List<GameObject>();
		List<GameObject> seedList = new List<GameObject>(1);
		GameObject candidate;
		seedList.Add(seed);

		for ( int i = 0; i < seedList.Count ; ++i )
		{
			candidate = seedList[Random.Range(0,i)];
			if ( !IsOnAPath(candidate) && !path.Contains(candidate) )
			{
				path.Add(candidate);
				seedList.Remove(candidate);
				seedList.AddRange(GetNeighbours(candidate));
			}
			if ( path.Count >= maxPathLength )
			{
				break;
			}
		}

		return path;

	}

	private void ExtendShortPaths()
	{
		SortPathsByLength();

		int pathID;
		for ( int i = 0; i < paths.Count; ++i )
		{
			//	Try to find any empty hexCells
			foreach ( GameObject pathCandidate in GetPathNeighbours(paths[i]) )
			{
				if ( !IsOnAPath(pathCandidate) )
				{
					paths[i].Add(pathCandidate);
				}
			}
			//	If there aren't any empty hexCells and the 
			if ( paths[i].Count <= minPathLength )
			{
				foreach ( GameObject pathNeighbour in GetPathNeighbours(paths[i]) )
				{
					pathID = GetPathID(pathNeighbour);
					if ( pathID != -1 && paths[pathID].Count > minPathLength + 2 )
					{
						paths[pathID].Remove(pathNeighbour);
						paths[i].Add(pathNeighbour);
					}
					if ( paths[i].Count >= maxPathLength )
					{
						break;
					}
				}
			}
		}
	}

	private void DrawPaths(bool drawLines = true)
	{
		for ( int i = 0; i < paths.Count; ++i )
		{
			if ( drawLines )
			{
				DrawPath(paths[i], pathMaterials[i % pathMaterials.Count], ( pathColors.Count > 0 ) ? pathColors[i % pathColors.Count] : Color.white);
			}
			else
			{
				DrawPath(paths[i], pathMaterials[i % pathMaterials.Count]);
			}
		}
	}

	private void DrawPath(List<GameObject> path, Material pathMaterial)
	{
		if ( path == null )
			return;

		for ( int i = 0; i < path.Count; ++i )
		{
			path[i].GetComponent<Renderer>().sharedMaterial = pathMaterial;
		}
	}

	private void DrawPath(List<GameObject> path, Material pathMaterial, Color pathColor)
	{
		if ( path == null )
			return;

		for ( int i = 0; i < path.Count; ++i )
		{
			path[i].GetComponent<Renderer>().sharedMaterial = pathMaterial;
		}

		if ( path.Count <= 1 )
		{
			return;
		}

		for ( int i = 0; i < path.Count - 1; ++i )
		{
			for ( int j = 1; j < path.Count; ++j )
			{
				if ( HexGrid.Instance.GetHexCell(path[i]).IsNeighbour(path[j]) && i != j )
				{
					Debug.DrawLine(
						path[i].transform.position,
						path[j].transform.position,
						pathColor,
						Mathf.Infinity
					);
				}
			}
		}
	}

	private List<GameObject> GetNeighbours(GameObject hexCellGO)
	{
		return hexCellGO.GetComponent<HexCell>().neighbourGOs;
	}

	private List<GameObject> GetPathNeighbours(List<GameObject> path)
	{
		List<GameObject> neighbours = new List<GameObject>();

		for ( int i = 0; i < path.Count; ++i )
		{
			foreach( GameObject neighbourCandidate in path[i].GetComponent<HexCell>().neighbourGOs )
			{
				if ( !path.Contains(neighbourCandidate) )
				{
					neighbours.Add(neighbourCandidate);
				}
			}
		}
		return neighbours;
	}

	private void SortPathsByLength()
	{
		paths.Sort(delegate(List<GameObject> x, List<GameObject> y)
		{
			if ( x.Count > y.Count )
			{
				return 1;
			}
			else if ( x.Count < y.Count )
			{
				return -1;
			}
			else
			{
				return 0;
			}
		});
	}
}