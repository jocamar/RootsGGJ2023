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
    Text playerNumber;

    [SerializeField]
    Color[] playerSpriteColorsSelection;

    [SerializeField]
    Image launchGameLoading;

    [SerializeField]
    float textPlayerNumberPositionOffset = -200;

    private float percentageLaunchGame;

    private List<Image> playerSprites = new List<Image>();
    private List<Text> playerNumbers = new List<Text>();

    private const float TIMETOLAUNCHGAME = 1.5f;

    private void Start()
    {
        playerSprites.Add(playerSprite);
        playerNumbers.Add(playerNumber);
    }

    void Update()
    {
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

    [System.Obsolete]
    void CreateNewPlayer(PlayerInputs player)
    {
        player.EnableVFX(playerSpriteColorsSelection[GameManager.instance.GetCurrentPlayerNumber()]);
        GameManager.instance.AddNewPlayer(player, playerSpriteColorsSelection[GameManager.instance.GetCurrentPlayerNumber()]);
    }

    void UpdatePlayerUI()
    {
        int index = GameManager.instance.GetCurrentPlayerNumber();
        playerSprites.Add(Instantiate(playerSprite, Root_PlayerSprites));
        playerNumbers.Add(Instantiate(playerNumber, playerSprites[index].transform));

        Color color = playerSpriteColorsSelection[index - 1];

        Text playerText = playerNumbers[index];
        playerText.text = $"Player {index}";
        playerText.transform.localPosition = new Vector3(playerText.transform.position.x, playerText.transform.position.y + textPlayerNumberPositionOffset, 0);
        playerText.color = color;

        playerSprites[index].color = color;
    }
}