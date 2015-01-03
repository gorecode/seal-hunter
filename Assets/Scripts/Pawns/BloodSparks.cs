using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BloodSparks : MonoBehaviour {
    ParticleSystem.Particle[] particles;

    public Vector2 minVelocity;
    public Vector2 maxVelocity;

    public void Emit(int count)
    {
        particleSystem.Emit(count);
        particles = new ParticleSystem.Particle[count];
        particleSystem.GetParticles(particles);
        
        for (int i = 0; i < particles.Length; i++)
        {
            Vector3 velolicty = Vector3.left * Random.Range(minVelocity.x, maxVelocity.x);

            velolicty.y = Random.Range(minVelocity.y, maxVelocity.y);
            
            particles[i].velocity = velolicty;
        }
        
        transform.particleSystem.SetParticles(particles, particles.Length);
    }

    void LateUpdate()
    {
        if (Application.isPlaying && !particleSystem.IsAlive()) 
        {
            GameObjectPool.Instance.Recycle(gameObject);
        }
    }
}
