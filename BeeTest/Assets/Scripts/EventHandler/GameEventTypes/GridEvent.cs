using UnityEngine;
using System.Collections;

public enum GridEventMessage
{
	InitializationComplete,
	CellsCreated,
	NeighbourListsCreated,
	PathsCreated
}

public class GridEvent
	: GameEventWithMessage<GridEventMessage>
{
	public GridEvent(GridEventMessage message)
		: this(null, message) { }
	public GridEvent(GameObject eventSrc, GridEventMessage message)
		: base(typeof(GridEvent), eventSrc, message) { }
}