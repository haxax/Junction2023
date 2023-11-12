using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAtStart : MonoBehaviour
{
	public List<SpawnCombo> Spawns = new List<SpawnCombo>();

	void Start()
	{
		foreach (SpawnCombo c in Spawns)
		{
			for (int i = c.Amount; i > 0; i--)
			{
				c.Prefab.Spawn(MapManager.Instance.GetLocationAtWorldBounds());
			}
		}
	}
}

[System.Serializable]
public class SpawnCombo
{
	[SerializeField] private int amount = 1;
	[SerializeField] private Poolable prefab;

	public int Amount => amount;
	public Poolable Prefab => prefab;
}