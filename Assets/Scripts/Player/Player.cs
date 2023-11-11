using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private float captureDistance = 2f;
    public float CaptureDistance => captureDistance;
}
