using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu_PlayerSelection : MonoBehaviour
{
    [SerializeField]
    private InputActionReference select;

    [SerializeField]
    private InputActionReference movement;

    void Update()
    {
        if (movement.action.triggered)
        {
            switch (movement.action.ReadValue<Vector2>())
            {
                case Vector2 v when v.Equals(Vector2.up):
                    Debug.Log($"Up");
                    break;

                case Vector2 v when v.Equals(Vector2.down):
                    Debug.Log($"Down");
                    break;

                case Vector2 v when v.Equals(Vector2.left):
                    Debug.Log($"Left");
                    break;

                case Vector2 v when v.Equals(Vector2.right):
                    Debug.Log($"Right");
                    break;
            }
        }

        if (select.action.triggered)
        {
            if (GameManager.instance.GetCurrentPlayerNumber() < GameManager.instance.PlayerMaxNumber)
            {
                CreateNewPlayer();
            }
            else
            {
                SceneGameManager.instance.StartGameScene();
            }
        }
    }

    void CreateNewPlayer()
    {
        GameManager.instance.AddNewPlayer(1);
    }
}
