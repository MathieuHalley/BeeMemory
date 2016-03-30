using UnityEngine;
using System.Collections;

public enum HexCellEventMessage
{
	InitializationComplete,
	NeighbourListCreated
}

public class HexCellEvent
	: GameEventWithMessage<HexCellEventMessage>
{
	public HexCellEvent(HexCellEventMessage message)
		: this(null, message) { }
	public HexCellEvent(GameObject eventSrc, HexCellEventMessage message)
		: base(typeof(HexCellEvent), eventSrc, message) { }
}