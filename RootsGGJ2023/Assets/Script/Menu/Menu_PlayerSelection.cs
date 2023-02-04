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

    [SerializeField]
    Image launchGameLoading;

    private float percentageLaunchGame;

    private List<Image> playerSprites = new List<Image>();

    private const float TIMETOLAUNCHGAME = 1.5f;

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

        bool onePlayerIsPressingKey = false;

        foreach (var player in PlayerInputs.allPlayers)
        {
            if (player.playerSelect && GameManager.instance.IsPlayerInputInGame(player))
                onePlayerIsPressingKey = true;

            if (player.playerSelect_Down)
            {
                if (!GameManager.instance.IsPlayerInputInGame(player) && GameManager.instance.GetCurrentPlayerNumber() < GameManager.instance.PlayerMaxNumber)
                {
                    CreateNewPlayer(player);
                    UpdatePlayerUI();
                }
            }
        }

        percentageLaunchGame = Mathf.MoveTowards(percentageLaunchGame, onePlayerIsPressingKey ? 1 : 0, Time.deltaTime / TIMETOLAUNCHGAME);
        launchGameLoading.fillAmount = percentageLaunchGame;

        if (percentageLaunchGame == 1)
        {
            SceneGameManager.instance.StartGameScene();
        }
    }

    void CreateNewPlayer(PlayerInputs player)
    {
        GameManager.instance.AddNewPlayer(player);
    }

    void UpdatePlayerUI()
    {
        int index = GameManager.instance.GetCurrentPlayerNumber();
        playerSprites.Add(Instantiate(playerSprite, Root_PlayerSprites));
        playerSprites[index].color = playerSpriteColorsSelection[index-1];
    }
}
