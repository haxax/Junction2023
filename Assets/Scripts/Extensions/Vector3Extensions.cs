using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
	public static Vector3 ToPerlinHeight(this Vector3 v)
	{
		v.z = Mathf.PerlinNoise((v.x + MapManager.Instance.CornerNodeCoordinate.Item1) / MapManager.Instance.PerlinMultiplier,
			(v.y + MapManager.Instance.CornerNodeCoordinate.Item2) / MapManager.Instance.PerlinMultiplier)
			 * MapManager.Instance.PerlinMultiplier;
		return v;
	}

	public static bool IsWithinBounds(this Vector3 v)
	{
		if (v.x < MapManager.Instance.CornerNodeCoordinate.Item1 || v.x > MapManager.Instance.CornerNodeCoordinate.Item1 + MapManager.Instance.FreeMapNodes.Count - 1)
		{ return false; }
		if (v.y < MapManager.Instance.CornerNodeCoordinate.Item2 || v.y > MapManager.Instance.CornerNodeCoordinate.Item2 + MapManager.Instance.FreeMapNodes[0].Count - 1)
		{ return false; }
		return true;
	}
}
