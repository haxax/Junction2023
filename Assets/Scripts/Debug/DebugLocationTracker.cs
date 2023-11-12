using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLocationTracker : MonoBehaviour
{
	[SerializeField] private float frequency = 5.0f;
	[SerializeField] private UnityEngine.UI.Text debugTxt;

	private float timer = 1.0f;
	private uint updateCounter = 0;
	private uint failedUpdateCounter = 0;

	private void Start()
	{
		if (!Application.isMobilePlatform) { this.enabled = false; }
		LocationManager.Instance.OnLocationChanged += LocationUpdate;
		timer = frequency;
	}

	private void FixedUpdate()
	{
		timer -= Time.fixedDeltaTime;
		if (timer > 0f) { return; }
		timer += frequency;
		failedUpdateCounter++;
		updateCounter--;
		LocationUpdate(new LocationCoordinate(Vector2.zero, Vector2.zero));
	}

	public void LocationUpdate(LocationCoordinate location)
	{
		updateCounter++;

		debugTxt.text = $"Updates: {updateCounter} Fails: {failedUpdateCounter}" +
			$"\n X: {location.CurrentCoordinate.x} ({location.CurrentCoordinate.x - location.PreviousCoordinate.x})" +
			$"   -   Y: {location.CurrentCoordinate.y} ({location.CurrentCoordinate.y - location.PreviousCoordinate.y})" +
			$"\n Offset: {LocationManager.Instance.LocationOffset})" +
			$"\n True Geo: {Input.location.lastData.LocationToMeters()})\n\n";
	}

	private void OnDestroy()
	{
		if (LocationManager.Instance != null) { LocationManager.Instance.OnLocationChanged -= LocationUpdate; }
	}
}
