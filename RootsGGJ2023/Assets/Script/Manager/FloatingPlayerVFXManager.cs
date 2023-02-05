using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlayerVFXManager : MonoBehaviour
{
    public static FloatingPlayerVFXManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object !");
            Destroy(this);
        }
    }


}
