using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu_PlayerSelection : MonoBehaviour
{
    [SerializeField]
    private InputActionReference select;

    void Update()
    {
        if (select.action.triggered)
        {
            SceneGameManager.instance.StartGameScene();
        }
    }
}
