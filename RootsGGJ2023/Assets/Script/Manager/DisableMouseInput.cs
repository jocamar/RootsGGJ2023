using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisableMouseInput : MonoBehaviour
{
    private void Awake()
    {
        if (!Application.isEditor)
        {
            InputSystem.DisableDevice(InputSystem.GetDevice<Mouse>());
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
