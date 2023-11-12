using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
	[SerializeField] private float movementSpeed = 1.0f;
	[SerializeField] private Transform graphicsPivot;
	[SerializeField] private QuickSpawn quickSpawn;

	private float distanceToPlayer = 0f;
	private (int, int) prevTile = (0, 0);
	void Update()
	{
		transform.position += (Player.Instance.transform.position - transform.position).normalized * Time.deltaTime * movementSpeed;
		if (Player.Instance.transform.position.x - transform.position.x < 0f) { graphicsPivot.localScale = new Vector3(-1, 1, 1); }
		else { graphicsPivot.localScale = new Vector3(1, 1, 1); }

		distanceToPlayer = Vector2.Distance(Player.Instance.transform.position, transform.position);

		if (distanceToPlayer > Player.Instance.CaptureDistance)
		{
			(int, int) newTile = (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
			if (newTile != prevTile)
			{
				prevTile = newTile;
				MapManager.Instance.InjectNodeValue(newTile.Item1, newTile.Item2, false);
			}
		}
	}


	public void SetQuickSpawnLocation()
	{
		quickSpawn.Location = MapManager.Instance.GetLocationAtWorldBounds();
	}
}