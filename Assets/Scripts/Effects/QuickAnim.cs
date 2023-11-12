using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAnim : MonoBehaviour
{
	[SerializeField] private float duration = 1f;
	[SerializeField] private Vector3 amount = Vector3.zero;
	[SerializeField] private Vector3 offset = Vector3.zero;
	[SerializeField] private AnimationCurve curve;
	[SerializeField] private AnimTarget target;

	private float progress = 0f;

	void Update()
	{
		progress += Time.deltaTime / duration;
		if (progress > 1f) { progress -= 1f; }

		switch (target)
		{
			case AnimTarget.position:
				transform.localPosition = offset + (amount * curve.Evaluate(progress));
				break;
			case AnimTarget.rotation:
				transform.localEulerAngles = offset + (amount * curve.Evaluate(progress));
				break;
			case AnimTarget.scale:
				transform.localScale = offset + (amount * curve.Evaluate(progress));
				break;
		}
	}
}
public enum AnimTarget
{
	position = 0,
	rotation = 1,
	scale = 2,
}
