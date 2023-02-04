using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu_Start : MonoBehaviour
{
    [SerializeField]
    private InputActionReference select, movement;

    // Update is called once per frame
    void Update()
    {
        if (select.action.triggered)
        {
            Debug.Log("Input triggered");
        }

        if (movement.action.triggered)
        {
            Debug.Log("Movement triggered");
        }
    }
}
