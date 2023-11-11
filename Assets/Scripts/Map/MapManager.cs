using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private float defaultNodesPerScale = 1f;
    [SerializeField] private int nodeBufferCount = 2;
    [SerializeField] private float perlinMultiplier = 1.0f;

    public float PerlinMultiplier => perlinMultiplier;
    public float NodesPerScale { get; set; } = 1f;
    private (int, int) NodeCounts { get; set; } = (0, 0);
    public Camera TargetCamera { get; set; } = null;
    public MapRenderer TargetRenderer { get; set; } = null;

    private List<(int, int)> FreeNodes { get; set; } = new List<(int, int)>();

    public (int, int) CornerNodeCoordinate { get; private set; } = (0, 0);
    public List<List<bool>> FreeMapNodes { get; private set; } = new List<List<bool>>();

    private void Start()
    {
        Reset();
        LocationManager.Instance.OnLocationChanged += LocationUpdate;
    }

    public void Reset()
    {
        TargetCamera = Camera.main;
        NodesPerScale = defaultNodesPerScale;

        CornerNodeCoordinate = (0, 0);

        FreeNodes.Clear();

        UpdateCameraSize();
    }

    public void UpdateCameraSize()
    {
        NodeCounts = (Mathf.CeilToInt((TargetCamera.aspect * 2 * TargetCamera.orthographicSize * NodesPerScale) + 1 + (2 * nodeBufferCount)),
            Mathf.CeilToInt((TargetCamera.orthographicSize * 2 * NodesPerScale) + 1 + (2 * nodeBufferCount)));

        FreeMapNodes.Clear();

        for (int x = 0; x < NodeCounts.Item1; x++)
        {
            FreeMapNodes.Add(new List<bool>());
            for (int y = 0; y < NodeCounts.Item2; y++)
            {
                //int rnd = UnityEngine.Random.Range(0, 2);
                //if (rnd < 1) { FreeNodes.Add((x, y)); }

                FreeMapNodes[x].Add(false);
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

    [ContextMenu("Update")]
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

        TargetRenderer.UpdateAllNodes();
    }

    public void InjectNodeValue(int coordX, int coordY, bool state)
    {
        (int, int) fixedCoord = (coordX - CornerNodeCoordinate.Item1, coordY - CornerNodeCoordinate.Item2);
        if (0 > fixedCoord.Item1 || NodeCounts.Item1 <= fixedCoord.Item1) { return; }
        if (0 > fixedCoord.Item2 || NodeCounts.Item2 <= fixedCoord.Item2) { return; }

        if (state == FreeMapNodes[fixedCoord.Item1][fixedCoord.Item2]) { return; }

        FreeMapNodes[fixedCoord.Item1][fixedCoord.Item2] = state;

        for (int i = FreeNodes.Count - 1; i >= 0; i--)
        {
            if (FreeNodes[i] == (coordX, coordY)) { FreeNodes.RemoveAt(i); }
        }

        if (state) { FreeNodes.Add((coordX, coordY)); }

        TargetRenderer.UpdateNode(fixedCoord.Item1, fixedCoord.Item2);
    }

    public void LocationUpdate(LocationCoordinate coordinate)
    {
        (int, int) NewCornerNodeCoordinate = (Mathf.RoundToInt(coordinate.CurrentCoordinate.x - (NodeCounts.Item1 / 2f)),
        Mathf.RoundToInt(coordinate.CurrentCoordinate.y - (NodeCounts.Item2 / 2f)));

        if (NewCornerNodeCoordinate == CornerNodeCoordinate) { return; }
        CornerNodeCoordinate = NewCornerNodeCoordinate;

        UpdateNodeMap();
    }
}