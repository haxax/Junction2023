using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : Singleton<LocationManager>
{
    [SerializeField] private float refreshFrequency = 1.0f;
    [SerializeField] private float accuracyInMeters = 10f;

    private float refreshTimer = 1.0f;

    private LocationCoordinate currentLocation;
    public LocationCoordinate CurrentLocation
    {
        get => currentLocation;
        set
        {
            if (value.CurrentCoordinate != currentLocation.CurrentCoordinate)
            {
                currentLocation = value;
                OnLocationChanged.Invoke(currentLocation);
            }
        }
    }
    public event Action<LocationCoordinate> OnLocationChanged;

    protected override void Awake()
    {
        base.Awake();

        currentLocation = new LocationCoordinate(Vector2.zero, Vector2.zero);
        StartTracking();
        refreshTimer = refreshFrequency;
    }

    private void FixedUpdate()
    {
        LocationUpdate();
    }

    private void LocationUpdate()
    {
        if (Input.location.status != LocationServiceStatus.Running) { return; }

        refreshTimer -= Time.fixedDeltaTime;
        if (refreshTimer > 0f) { return; }

        refreshTimer += refreshFrequency;

        CurrentLocation = new LocationCoordinate(Input.location.lastData, CurrentLocation);
    }

    public void StartTracking()
    {
        if (Input.location.status == LocationServiceStatus.Initializing || Input.location.status == LocationServiceStatus.Running) { return; }

        Input.location.Start(accuracyInMeters, accuracyInMeters / 2.0f);
    }

    public void StopTracking()
    {
        Input.location.Stop();
    }

    public void SpoofLocationAdditive(Vector2 amount)
    {
        CurrentLocation = new LocationCoordinate(CurrentLocation.CurrentCoordinate + amount, CurrentLocation.CurrentCoordinate);
    }
}
