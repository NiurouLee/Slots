using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class PlayParticleEditor : MonoBehaviour
{

    void OnEnable()
    {
        ParticleSystem[] results = this.transform.parent.GetComponentsInChildren< ParticleSystem>();
        
        GameObject[] temp = new GameObject[results.Length];

        for(int i = 0; i < results.Length; i++)
        {
            temp[i] = results[i].gameObject;
        }
        
        //不再自动选中
        //Selection.objects = temp;
        
        this.GetComponent<ParticleSystem>().Play();
    }
}
#endif