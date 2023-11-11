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
				// Check if all are false;
				if (!Nodes[x + 1][y] && !Nodes[x][y] && !Nodes[x][y + 1] && !Nodes[x + 1][y + 1])
				{ boolValue = 0; }
				// Check if two or more are true;
				else if (Nodes[x + 1][y] & Nodes[x][y] & Nodes[x][y + 1] & Nodes[x + 1][y + 1]) { boolValue = 2; }
				// One is true;
				else
				{
					boolValue = 1;
					// Check if triangles should be flipped;
					if (Nodes[x + 1][y]) { isFlipped = true; }
					else if (Nodes[x][y + 1]) { isFlipped = true; }
					else { isFlipped = false; }
				}
				Debug.Log($"{Nodes[x][y]},{Nodes[x + 1][y]},{Nodes[x][y + 1]},{Nodes[x + 1][y + 1]} is {boolValue}");

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
			_vertices.Add(new Vector3(x, 0f, y));
			_vertices.Add(new Vector3((x + 1), 0f, y));
			_vertices.Add(new Vector3(x, 0f, (y + 1)));

			// B
			_vertices.Add(new Vector3((x + 1), 0f, (y + 1)));
			_vertices.Add(new Vector3(x, 0f, (y + 1)));
			_vertices.Add(new Vector3((x + 1), 0f, y));
		}
		else
		{
			// A
			_vertices.Add(new Vector3(x, 0f, (y + 1)));
			_vertices.Add(new Vector3(x, 0f, y));
			_vertices.Add(new Vector3((x + 1), 0f, (y + 1)));


			// B
			_vertices.Add(new Vector3((x + 1), 0f, y));
			_vertices.Add(new Vector3((x + 1), 0f, (y + 1)));
			_vertices.Add(new Vector3(x, 0f, y));
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
				if (Nodes[x][y] || Nodes[x][y + 1])
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
				if (Nodes[x + 1][y] || Nodes[x + 1][y + 1])
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
	{ return (((Nodes.Count - 1) * y) + x) * _tileVerticeCount; }

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