using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAirTest : MonoBehaviour
{
    
    
    // Start is called before the first frame update
    void Start()
    {
        AddAir();
    }

    public void AddAir()
    {
#if !PRODUCTION_PACKAGE || UNITY_EDITOR
        try
        {
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.gameObject.AddComponent<PocoManager>();
            }
            else
            {
                this.gameObject.AddComponent<PocoManager>();
            }


        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
#endif     
    }
   
}
