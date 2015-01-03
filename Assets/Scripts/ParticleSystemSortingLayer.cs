using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class ParticleSystemSortingLayer : MonoBehaviour {
    public string sortingLayerName;

    void Start()
    {
        if ("".Equals(sortingLayerName)) return;

        particleSystem.renderer.sortingLayerName = sortingLayerName;
    }
}
