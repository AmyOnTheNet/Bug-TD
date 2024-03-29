﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timedDestroy : MonoBehaviour
{
	public float destroyDelay;
	public bool fx;

	private float timer;
	// Start is called before the first frame update
	void Start()
	{
		if (fx) 
		{
			SetDestroyFX();
		}else {
			timer = destroyDelay;
		}
	}

	// Update is called once per frame
	void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0f)
		{
			Destroy(gameObject);
		}
	}

	ParticleSystem m_System;
	ParticleSystem.Particle[] m_Particles;

	public void SetDestroyFX(){
		
		InitializeIfNeeded();

		if (GetComponent<TrailRenderer>()) 
		{
			GetComponent<TrailRenderer>().autodestruct = true;
		}

		if (GetComponent<ParticleSystem>()) 
		{
			m_System = GetComponent<ParticleSystem>();

			int numParticlesAlive = m_System.GetParticles(m_Particles);
			
			m_System.Stop();

			timer = m_Particles[0].startLifetime;
		}
		
	}

	void InitializeIfNeeded()
	{
		if (m_System == null)
			m_System = GetComponent<ParticleSystem>();

		if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
			m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
	}
}
