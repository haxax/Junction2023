using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1.0f;

    private float distanceToPlayer = 0f;
    private (int, int) prevTile = (0, 0);
    void Update()
    {
        transform.position += (Player.Instance.transform.position - transform.position).normalized * Time.deltaTime * movementSpeed;
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
}