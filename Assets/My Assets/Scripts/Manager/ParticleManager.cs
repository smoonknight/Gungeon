using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    public Particle[] particles;

    public GameObject GetParticle(ParticleType type)
    {
        Particle particle = Array.Find(particles, prop => prop.particleType == type);
        return particle.particlePrefab;
    }

    public void PlayParticleOnTime(ParticleType type, Vector3 location)
    {
        Particle particle = Array.Find(particles, prop => prop.particleType == type);
        
        GameObject particleSpawn = Instantiate(particle.particlePrefab, location, Quaternion.identity);
        
        Destroy(particleSpawn, particleSpawn.GetComponent<ParticleSystem>().main.duration);
    }

    public void PlayParticleOnTime(ParticleType type, Vector3 location, int duration)
    {
        Particle particle = Array.Find(particles, prop => prop.particleType == type);
        
        GameObject particleSpawn = Instantiate(particle.particlePrefab, location, Quaternion.identity);
        
        Destroy(particleSpawn, duration);
    }
    [System.Serializable]
    public struct Particle
    {
        public ParticleType particleType;
        public GameObject particlePrefab;
    }

    public enum ParticleType
    {
        smoke,
        dust,
        fire,
        plasma,
        explosion,
        fireball,
    }
}