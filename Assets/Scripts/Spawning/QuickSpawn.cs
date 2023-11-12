using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSpawn : MonoBehaviour
{
	[SerializeField] private Poolable prefab;
	[SerializeField] private bool spawnAtCurrentLocation = false;
	[SerializeField] private Vector2 location;
	public Vector2 Location { get => location; set => location = value; }

	[ContextMenu("Spawn")]
	public void Spawn()
	{
		if (spawnAtCurrentLocation) { prefab.Spawn(); }
		else { prefab.Spawn(Location); }
	}
}