using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCoordinate
{
	public LocationCoordinate(LocationInfo newInfo, LocationCoordinate previousCoordinate)
	{
		CurrentCoordinate = newInfo.LocationToMeters();
		PreviousCoordinate = previousCoordinate.CurrentCoordinate;
	}

	public LocationCoordinate(Vector2 currentCoordinate, Vector2 previousCoordinate)
	{
		CurrentCoordinate = currentCoordinate;
		PreviousCoordinate = previousCoordinate;
	}

	public Vector2 PreviousCoordinate { get; private set; } = Vector2.zero;
	public Vector2 CurrentCoordinate { get; private set; } = Vector2.zero;
}