using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickEffect : MonoBehaviour
{
	[SerializeField] private ParticleSystem _particleSystem;
	[SerializeField] private Poolable poolableReference;
	public void OnSpawn()
	{
		_particleSystem.Play();
		this.enabled = true;
	}

	private void Update()
	{
		if (_particleSystem.isPlaying) { return; }
		_particleSystem.Stop();
		this.enabled = false;
		poolableReference.Despawn();
	}
}
