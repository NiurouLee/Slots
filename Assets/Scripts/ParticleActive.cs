using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ParticleActive : MonoBehaviour
{

#if UNITY_EDITOR
    public void OnEnable()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            if (!gameObject.TryGetComponent(out ParticleSystem particle)) return;

            // if (Selection.objects.All(r => r != gameObject))
            // {
            //     Selection.objects = Selection.objects.Concat(new[] {gameObject}).ToArray();
            // }

            Debug.Log("ParticleActive:OnEnable");
            particle.Play();
        }
    }

    public void OnDisable()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            // stuff
            Debug.Log("ParticleActive:OnDisable");
            if (!gameObject.TryGetComponent(out ParticleSystem particle)) return;
            particle.Stop();
        }
    }
#endif    
}
