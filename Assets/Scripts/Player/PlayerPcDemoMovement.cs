using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPcDemoMovement : MonoBehaviour
{
	[SerializeField] private float keyMovementAmount = 0.5f;
	[SerializeField] private float movementSpeed = 1.0f;
	[SerializeField] private Transform graphicsPivot;

	void Start()
	{

	}

	private Vector2 movementAmount = Vector2.zero;
	private (float, float, float, float) keyTimers = new(0f, 0f, 0f, 0f);
	private (int, int) prevTile = (0, 0);

	void Update()
	{
		movementAmount = Vector2.zero;
		if (keyTimers.Item1 > 0f) { keyTimers.Item1 -= Time.deltaTime; }
		if (keyTimers.Item2 > 0f) { keyTimers.Item2 -= Time.deltaTime; }
		if (keyTimers.Item3 > 0f) { keyTimers.Item3 -= Time.deltaTime; }
		if (keyTimers.Item4 > 0f) { keyTimers.Item4 -= Time.deltaTime; }

		if (keyTimers.Item1 <= 0f && Input.GetKey(KeyCode.W)) { movementAmount.y += keyMovementAmount; keyTimers.Item1 += (keyMovementAmount / movementSpeed); }
		if (keyTimers.Item2 <= 0f && Input.GetKey(KeyCode.S)) { movementAmount.y -= keyMovementAmount; keyTimers.Item2 += (keyMovementAmount / movementSpeed); }
		if (keyTimers.Item3 <= 0f && Input.GetKey(KeyCode.D)) { movementAmount.x += keyMovementAmount; keyTimers.Item3 += (keyMovementAmount / movementSpeed); }
		if (keyTimers.Item4 <= 0f && Input.GetKey(KeyCode.A)) { movementAmount.x -= keyMovementAmount; keyTimers.Item4 += (keyMovementAmount / movementSpeed); }

		if (movementAmount != Vector2.zero) { LocationManager.Instance.SpoofLocationAdditive(movementAmount); }

		transform.position += (LocationManager.Instance.CurrentLocation.CurrentPosition - transform.position) * Time.deltaTime * movementSpeed;
		if (LocationManager.Instance.CurrentLocation.CurrentPosition.x - transform.position.x < 0f) { graphicsPivot.localScale = new Vector3(-1, 1, 1); }
		else { graphicsPivot.localScale = new Vector3(1, 1, 1); }


		(int, int) newTile = (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
		if (newTile != prevTile)
		{
			prevTile = newTile;
			MapManager.Instance.InjectNodeValue(newTile.Item1, newTile.Item2, true);
		}
	}
}
