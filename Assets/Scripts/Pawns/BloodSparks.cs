using UnityEngine;
using UnityEngineExt;
using System.Collections;

[ExecuteInEditMode]
public class BloodSparks : MonoBehaviour {
    public Vector2 minVelocity;
    public Vector2 maxVelocity;

    private float originalStartLifetime;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        originalStartLifetime = particleSystem.startLifetime;

        particleSystem.startLifetime = originalStartLifetime * 10;
    }

    public void Emit(int count)
    {
        if (particles == null || particles.Length != count) particles = new ParticleSystem.Particle[count];

        particleSystem.Stop();
        particleSystem.time = 0;
        particleSystem.Emit(count);
        particleSystem.GetParticles(particles);
        
        for (int i = 0; i < particles.Length; i++)
        {
            Vector3 velolicty = Vector3.left * Random.Range(minVelocity.x, maxVelocity.x);
            velolicty.y = Random.Range(minVelocity.y, maxVelocity.y);
            particles[i].velocity = velolicty;
        }
        
        particleSystem.SetParticles(particles, particles.Length);
    }

    void LateUpdate()
    {
        float ttl = originalStartLifetime - particleSystem.time;

        if (Application.isPlaying && ttl <= 0f) 
        {
            particleSystem.Pause();

            SlaughterBackground slaughter = PrefabLocator.INSTANCE.slaughterBackgroundController;

            if (slaughter != null) slaughter.RenderToTextureLater(gameObject);

            gameObject.Release();
        }
    }
}
