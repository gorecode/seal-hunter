using UnityEngine;
using System.Collections;

public class BloodSparks : MonoBehaviour {
    private ParticleSystem myParticleSystem;

    void Awake()
    {
        myParticleSystem = transform.particleSystem;
    }

    public void Emit()
    {
        myParticleSystem.Play();
    }

    void LateUpdate()
    {
        if (!myParticleSystem.IsAlive()) 
        {
            GameObjectPool.Instance.Recycle(gameObject);
        }
    }
}
