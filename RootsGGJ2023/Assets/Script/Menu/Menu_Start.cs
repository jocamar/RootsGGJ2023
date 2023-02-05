using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu_Start : MonoBehaviour
{
    [SerializeField]
    private InputActionReference select;

    // Update is called once per frame
    void Update()
    {
        if (select.action.triggered)
        {
            SceneGameManager.instance.StartPlayerSelectionScene();
        }
    }
}
