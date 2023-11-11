using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
	[SerializeField] private float defaultNodesPerScale = 1f;
	[SerializeField] private int nodeBufferCount = 2;

	public float NodesPerScale { get; set; } = 1f;
	private (int, int) NodeCounts { get; set; } = (0, 0);
	public Camera TargetCamera { get; set; } = null;
	public MapRenderer TargetRenderer { get; set; } = null;

	private List<(int, int)> FreeNodes { get; set; } = new List<(int, int)>();

	private (int, int) CornerNodeCoordinate { get; set; } = (0, 0);
	public List<List<bool>> FreeMapNodes { get; private set; } = new List<List<bool>>();

	private void Start()
	{
		Reset();
	}

	public void Reset()
	{
		TargetCamera = Camera.main;
		NodesPerScale = defaultNodesPerScale;

		CornerNodeCoordinate = (0, 0);

		FreeNodes.Clear();

		UpdateCameraSize();
		//UpdateNodeMap();

		if (TargetRenderer != null) { TargetRenderer.UpdateMesh(); }
	}

	public void UpdateCameraSize()
	{
		NodeCounts = (Mathf.CeilToInt(TargetCamera.aspect * TargetCamera.orthographicSize * NodesPerScale) + (2 * nodeBufferCount),
			Mathf.CeilToInt(TargetCamera.orthographicSize * NodesPerScale) + (2 * nodeBufferCount));

		FreeMapNodes.Clear();

		for (int x = 0; x < NodeCounts.Item1; x++)
		{
			FreeMapNodes.Add(new List<bool>());
			for (int y = 0; y < NodeCounts.Item2; y++)
			{
				int rnd = UnityEngine.Random.Range(0, 2);
				FreeMapNodes[x].Add(rnd != 0);
				Debug.Log($"{rnd} is {FreeMapNodes[x][y]}");
			}
		}
	}

	public void FalsifyNodeMap()
	{
		for (int x = 0; x < FreeMapNodes.Count; x++)
		{
			for (int y = 0; y < FreeMapNodes[x].Count; y++)
			{
				FreeMapNodes[x][y] = false;
			}
		}
	}

	public void UpdateNodeMap()
	{
		FalsifyNodeMap();

		(int, int) coordCeil = (CornerNodeCoordinate.Item1 + NodeCounts.Item1, CornerNodeCoordinate.Item2 + NodeCounts.Item2);

		foreach (var node in FreeNodes)
		{
			if (CornerNodeCoordinate.Item1 <= node.Item1 && node.Item1 < coordCeil.Item1
				&& CornerNodeCoordinate.Item2 <= node.Item2 && node.Item2 < coordCeil.Item2)
			{
				FreeMapNodes[node.Item1 - CornerNodeCoordinate.Item1][node.Item2 - CornerNodeCoordinate.Item2] = true;
			}
		}
	}
}