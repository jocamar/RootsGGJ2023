using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GYM_Misc : MonoBehaviour
{
    [SerializeField]
    private InputActionReference select;

    [SerializeField]
    private InputActionReference movement;

    // Update is called once per frame
    void Update()
    {
        if (movement.action.triggered)
        {
            Debug.Log(Gamepad.current.deviceId);
        }

        if (select.action.triggered)
        {
            Debug.Log(Gamepad.current.deviceId);
        }
    }
}
