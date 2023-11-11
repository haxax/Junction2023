using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    // Component references.
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private MeshFilter _filter;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField] private MeshCollider _collider;

    [HideInInspector][SerializeField] private Vector3 _defaultNormalDirection = new Vector3(0f, 0f, 1f);
    [SerializeField] private int _texturePixelSize = 256;
    [SerializeField] private int _trueTexture = 0;
    [SerializeField] private int _falseTexture = 1;

    private int _texturesPerRow = 2;
    private float _pixelPercentageX, _pixelPercentageY = 0f;

    private readonly int _tileVerticeCount = 6;

    // Raw mesh data, loaded from the level file.
    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector2> _uvs = new List<Vector2>();
    private List<Vector3> _normals = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private void Awake()
    {
        Reset();
    }

    private void Start()
    {
        MapManager.Instance.TargetRenderer = this;
        UpdateMesh();
        GenerateMesh();
    }

    public void Reset()
    {
        // Calculate percentage for each pixel on the material texture.
        _pixelPercentageX = 1.00000f / _material.mainTexture.width;
        _pixelPercentageY = 1.00000f / _material.mainTexture.height;

        _texturesPerRow = Mathf.FloorToInt(_material.mainTexture.width / _texturePixelSize);

        // Set components correctly
        _mesh = new Mesh();
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _filter.mesh = _mesh;
        _renderer.material = _material;

        // Wipe previously loaded data.
        _vertices = new List<Vector3>();
        _uvs = new List<Vector2>();
        _normals = new List<Vector3>();
        _triangles = new List<int>();
    }

    List<List<bool>> Nodes => MapManager.Instance.FreeMapNodes;
    public void UpdateMesh()
    {
        bool isFlipped = false;
        int boolValue = 0;
        for (int x = 0; x < Nodes.Count - 1; x++)
        {
            for (int y = 0; y < Nodes[x].Count - 1; y++)
            {
                isFlipped = false;
                boolValue = Convert.ToInt32(Nodes[x + 1][y]) + Convert.ToInt32(Nodes[x][y]) + Convert.ToInt32(Nodes[x][y + 1]) + Convert.ToInt32(Nodes[x + 1][y + 1]);

                if (boolValue == 1)
                {
                    // Check if triangles should be flipped;
                    if (Nodes[x + 1][y]) { isFlipped = true; }
                    else if (Nodes[x][y + 1]) { isFlipped = true; }
                    else { isFlipped = false; }
                }

                AddTileVertices(x, y, isFlipped);
                AddTileUvs(x, y, boolValue, isFlipped);
                AddTileNormals();
                AddTileTriangles(x, y);
            }
        }
    }
    private void AddTileVertices(int x, int y, bool isFlipped)
    {
        if (!isFlipped)
        {
            // A
            _vertices.Add(new Vector3(x, y));
            _vertices.Add(new Vector3(x, y + 1));
            _vertices.Add(new Vector3(x + 1, y));

            // B
            _vertices.Add(new Vector3((x + 1), (y + 1)));
            _vertices.Add(new Vector3(x + 1, y));
            _vertices.Add(new Vector3(x, y + 1));
        }
        else
        {
            // A
            _vertices.Add(new Vector3(x + 1, y));
            _vertices.Add(new Vector3(x, y));
            _vertices.Add(new Vector3((x + 1), (y + 1)));


            // B
            _vertices.Add(new Vector3(x, y + 1));
            _vertices.Add(new Vector3((x + 1), (y + 1)));
            _vertices.Add(new Vector3(x, y));
        }
    }

    private void AddTileUvs(int x, int y, int boolValue, bool isFlipped)
    {
        // A Side
        int textureId = 0;
        switch (boolValue)
        {
            case 0:
                textureId = _falseTexture; break;
            case 1:
                if (Nodes[x][y] || Nodes[x + 1][y])
                { textureId = _trueTexture; }
                else { textureId = _falseTexture; }
                break;
            case 2:
            default:
                textureId = _trueTexture; break;
        }

        // Calculate index in the texture.
        int textureColumn = textureId % _texturesPerRow;
        int textureRow = Mathf.FloorToInt(textureId / _texturesPerRow);

        // Calculate pixel location of top left corner of the texture.
        int uvStartx = (textureColumn * _texturePixelSize);
        int uvStarty = (textureRow * _texturePixelSize);

        if (!isFlipped)
        {
            AddUv(uvStartx + 1, uvStarty + 1);
            AddUv(uvStartx - 1 + _texturePixelSize, uvStarty + 1);
            AddUv(uvStartx + 1, uvStarty - 1 + _texturePixelSize);
        }
        else
        {
            AddUv(uvStartx + 1, uvStarty - 1 + _texturePixelSize);
            AddUv(uvStartx + 1, uvStarty + 1);
            AddUv(uvStartx - 1 + _texturePixelSize, uvStarty - 1 + _texturePixelSize);
        }


        // B Side
        switch (boolValue)
        {
            case 0:
                textureId = _falseTexture; break;
            case 1:
                if (Nodes[x][y + 1] || Nodes[x + 1][y + 1])
                { textureId = _trueTexture; }
                else { textureId = _falseTexture; }
                break;
            case 2:
            default:
                textureId = _trueTexture; break;
        }

        // Calculate index in the texture.
        textureColumn = textureId % _texturesPerRow;
        textureRow = Mathf.FloorToInt(textureId / _texturesPerRow);

        // Calculate pixel location of top left corner of the texture.
        uvStartx = (textureColumn * _texturePixelSize);
        uvStarty = (textureRow * _texturePixelSize);

        if (!isFlipped)
        {
            AddUv(uvStartx - 1 + _texturePixelSize, uvStarty - 1 + _texturePixelSize);
            AddUv(uvStartx + 1, uvStarty - 1 + _texturePixelSize);
            AddUv(uvStartx - 1 + _texturePixelSize, uvStarty + 1);
        }
        else
        {
            AddUv(uvStartx - 1 + _texturePixelSize, uvStarty + 1);
            AddUv(uvStartx - 1 + _texturePixelSize, uvStarty - 1 + _texturePixelSize);
            AddUv(uvStartx + 1, uvStarty + 1);
        }
    }
    /// <summary> Adds new UV. Converts pixel location into float. </summary>
    /// <param name="x">Location within texture in pixels. X-axis.</param>
    /// <param name="y">Location within texture in pixels. Y-axis.</param>
    private void AddUv(float x, float y)
    { _uvs.Add(new Vector2(x * _pixelPercentageX, 1.0000f - (y * _pixelPercentageY))); }

    private void AddTileNormals()
    {
        for (int i = 0; i < _tileVerticeCount; i++)
        { _normals.Add(_defaultNormalDirection); }
    }

    private void AddTileTriangles(int x, int y)
    {
        int startVertex = TileStartVertexId(x, y);

        // Top left triangle. (Top left) -> (Top right) -> (Bottom left).
        _triangles.Add(startVertex + 0);
        _triangles.Add(startVertex + 1);
        _triangles.Add(startVertex + 2);

        // Bottom right triangle. (Bottom right) -> (Bottom left) -> (Top right).
        _triangles.Add(startVertex + 3);
        _triangles.Add(startVertex + 4);
        _triangles.Add(startVertex + 5);
    }

    /// <summary> Return the start index of a tile's vertices.</summary>
    private int TileStartVertexId(int x, int y)
    { return (((Nodes[0].Count - 1) * x) + y) * _tileVerticeCount; }

    public void UpdateNode(int X, int Y)
    {
        (int, int) xMinMax = (Mathf.Clamp(X - 1, 0, Nodes.Count), Mathf.Clamp(X + 1, 0, Nodes.Count));
        (int, int) yMinMax = (Mathf.Clamp(Y - 1, 0, Nodes[0].Count), Mathf.Clamp(Y + 1, 0, Nodes[0].Count));
        UpdateNodes(xMinMax, yMinMax);
    }
    public void UpdateAllNodes()
    {
        UpdateNodes((0, Nodes.Count - 1), (0, Nodes[0].Count - 1));
    }
    public void UpdateNodes((int, int) xMinMax, (int, int) yMinMax)
    {
        this.enabled = true;
        bool isFlipped = false;
        int boolValue = 0;

        for (int x = xMinMax.Item1; x < xMinMax.Item2; x++)
        {
            for (int y = yMinMax.Item1; y < yMinMax.Item2; y++)
            {
                isFlipped = false;
                boolValue = Convert.ToInt32(Nodes[x + 1][y]) + Convert.ToInt32(Nodes[x][y]) + Convert.ToInt32(Nodes[x][y + 1]) + Convert.ToInt32(Nodes[x + 1][y + 1]);

                if (boolValue == 1)
                {
                    // Check if triangles should be flipped;
                    if (Nodes[x + 1][y]) { isFlipped = true; }
                    else if (Nodes[x][y + 1]) { isFlipped = true; }
                    else { isFlipped = false; }
                }
                UpdateTileVertices(x, y, isFlipped);
                UpdateTileUvs(x, y, boolValue, isFlipped);
            }
        }
    }
    private void UpdateTileVertices(int x, int y, bool isFlipped)
    {
        int startVert = TileStartVertexId(x, y);
        if (!isFlipped)
        {
            // A
            _vertices[startVert] = new Vector3(x, y).ToPerlinHeight();
            _vertices[startVert + 1] = new Vector3(x, y + 1).ToPerlinHeight();
            _vertices[startVert + 2] = new Vector3(x + 1, y).ToPerlinHeight();

            // B
            _vertices[startVert + 3] = new Vector3((x + 1), (y + 1)).ToPerlinHeight();
            _vertices[startVert + 4] = new Vector3(x + 1, y).ToPerlinHeight();
            _vertices[startVert + 5] = new Vector3(x, y + 1).ToPerlinHeight();
        }
        else
        {
            // A
            _vertices[startVert] = new Vector3(x + 1, y).ToPerlinHeight();
            _vertices[startVert + 1] = new Vector3(x, y).ToPerlinHeight();
            _vertices[startVert + 2] = new Vector3((x + 1), (y + 1)).ToPerlinHeight();


            // B
            _vertices[startVert + 3] = new Vector3(x, y + 1).ToPerlinHeight();
            _vertices[startVert + 4] = new Vector3((x + 1), (y + 1)).ToPerlinHeight();
            _vertices[startVert + 5] = new Vector3(x, y).ToPerlinHeight();
        }
    }
    private void UpdateTileUvs(int x, int y, int boolValue, bool isFlipped)
    {
        int startVert = TileStartVertexId(x, y);

        // A Side
        int textureId = 0;
        switch (boolValue)
        {
            case 0:
                textureId = _falseTexture; break;
            case 1:
                if (Nodes[x][y] || Nodes[x + 1][y])
                { textureId = _trueTexture; }
                else
                { textureId = _falseTexture; }
                break;
            case 2:
            default:
                textureId = _trueTexture; break;
        }


        // Calculate index in the texture.
        int textureColumn = textureId % _texturesPerRow;
        int textureRow = Mathf.FloorToInt(textureId / _texturesPerRow);

        // Calculate pixel location of top left corner of the texture.
        int uvStartx = (textureColumn * _texturePixelSize);
        int uvStarty = (textureRow * _texturePixelSize);

        if (!isFlipped)
        {
            UpdateUv(startVert, uvStartx + 1, uvStarty + 1);
            UpdateUv(startVert + 1, uvStartx - 1 + _texturePixelSize, uvStarty + 1);
            UpdateUv(startVert + 2, uvStartx + 1, uvStarty - 1 + _texturePixelSize);
        }
        else
        {
            UpdateUv(startVert, uvStartx + 1, uvStarty - 1 + _texturePixelSize);
            UpdateUv(startVert + 1, uvStartx + 1, uvStarty + 1);
            UpdateUv(startVert + 2, uvStartx - 1 + _texturePixelSize, uvStarty - 1 + _texturePixelSize);
        }


        // B Side
        switch (boolValue)
        {
            case 0:
                textureId = _falseTexture; break;
            case 1:
                if (Nodes[x][y + 1] || Nodes[x + 1][y + 1])
                { textureId = _trueTexture; }
                else
                { textureId = _falseTexture; }
                break;
            case 2:
            default:
                textureId = _trueTexture; break;
        }

        // Calculate index in the texture.
        textureColumn = textureId % _texturesPerRow;
        textureRow = Mathf.FloorToInt(textureId / _texturesPerRow);

        // Calculate pixel location of top left corner of the texture.
        uvStartx = (textureColumn * _texturePixelSize);
        uvStarty = (textureRow * _texturePixelSize);

        if (!isFlipped)
        {
            UpdateUv(startVert + 3, uvStartx - 1 + _texturePixelSize, uvStarty - 1 + _texturePixelSize);
            UpdateUv(startVert + 4, uvStartx + 1, uvStarty - 1 + _texturePixelSize);
            UpdateUv(startVert + 5, uvStartx - 1 + _texturePixelSize, uvStarty + 1);
        }
        else
        {
            UpdateUv(startVert + 3, uvStartx - 1 + _texturePixelSize, uvStarty + 1);
            UpdateUv(startVert + 4, uvStartx - 1 + _texturePixelSize, uvStarty - 1 + _texturePixelSize);
            UpdateUv(startVert + 5, uvStartx + 1, uvStarty + 1);
        }
    }
    private void UpdateUv(int index, float x, float y)
    { _uvs[index] = new Vector2(x * _pixelPercentageX, 1.0000f - (y * _pixelPercentageY)); }


    private void FixedUpdate()
    {
        UpdateVertices();
        UpdateUvs();
        RecalculateMesh();
        this.enabled = false;
    }

    #region Mesh Generating
    public void GenerateMesh()
    {
        UpdateVertices();
        UpdateUvs();
        UpdateNormals();
        UpdateTriangles();
        RecalculateMesh();
    }

    private void UpdateVertices()
    { _mesh.vertices = _vertices.ToArray(); }
    private void UpdateUvs()
    { _mesh.uv = _uvs.ToArray(); }
    private void UpdateNormals()
    { _mesh.normals = _normals.ToArray(); }
    private void UpdateTriangles()
    { _mesh.triangles = _triangles.ToArray(); }

    private void RecalculateMesh()
    {
        transform.position = new Vector2(MapManager.Instance.CornerNodeCoordinate.Item1, MapManager.Instance.CornerNodeCoordinate.Item2);
        _mesh.RecalculateNormals();
        _mesh.RecalculateUVDistributionMetrics();
    }
    #endregion


    private void SetupCollider()
    {
        _collider.sharedMesh = _mesh;
        _collider.convex = false;
    }
}