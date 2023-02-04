using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu_PlayerSelection : MonoBehaviour
{
    [SerializeField]
    private InputActionReference select;

    [SerializeField]
    private InputActionReference movement;

    [SerializeField]
    Transform Root_PlayerSprites;

    [SerializeField]
    Image playerSprite;

    [SerializeField]
    Color[] playerSpriteColorsSelection;

    private List<Image> playerSprites = new List<Image>();

    private void Start()
    {
        playerSprites.Add(playerSprite);
    }

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

                UpdatePlayerUI();
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

    void UpdatePlayerUI()
    {
        int index = GameManager.instance.GetCurrentPlayerNumber();
        playerSprites.Add(Instantiate(playerSprite, Root_PlayerSprites));
    }
}
