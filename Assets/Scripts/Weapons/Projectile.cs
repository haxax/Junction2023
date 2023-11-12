using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
	[SerializeField] private float damage = 1f;
	[SerializeField] private float speed = 1.0f;
	[SerializeField] private AnimationCurve speedCurve = new AnimationCurve();
	[SerializeField] private UnityEvent OnReachTarget;

	private float flySpeed = 0f;
	private float flyTimer = 0f;

	private Vector3 Origin { get; set; }
	private Transform Target { get; set; }

	public void ShootAt(Transform target)
	{
		Target = target;
		Origin = transform.position;

		flySpeed = Vector2.Distance(Origin, Target.position) / speed;
		flyTimer = 0f;

		this.enabled = true;
	}

	private void Update()
	{
		flyTimer += Time.deltaTime / flySpeed;
		if (flyTimer > 1.0f && Target.gameObject.activeInHierarchy)
		{
			flyTimer = 1.0f; OnReachTarget.Invoke();
			Killable killable = Target.GetComponent<Killable>();
			killable?.DealDmg(damage);
		}

		if (Target.gameObject.activeInHierarchy) { transform.position = Origin + ((Target.position - Origin) * speedCurve.Evaluate(flyTimer)); }
		else { OnReachTarget.Invoke(); }

	}
}
