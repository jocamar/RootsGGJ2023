using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        ASSIGNING_PLAYERS,
        SELECTING_SABOTEUR,
        GAMEPLAY,
        PATH_REVEAL,
        DISCUSSION,
        VOTING,
        GAME_END
    }

    public GameObject emptyTile;
    public GameObject obstacleTile;
    public GameObject startTile;
    public GameObject endTile;

    List<Player> players = new List<Player>();
    GameState currentGameState = GameState.ASSIGNING_PLAYERS;
    float currentWaitTime = 0f;
    int currentSaboteurSelectionPlayer = 0;
    int randomImpostor = 0;
    bool startingSaboteurSelect = true;
    Map map = new Map(15, 15);
    

    // Start is called before the first frame update
    void Start()
    {
        map.GenerateMap(10);

        float originX = -2.5f;
        float originY = -2f;

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (map.GetTile(x, y).type == Map.TileType.Obstacle)
                {
                    Instantiate(obstacleTile, new Vector3(originX + x * 0.3f, originY + y * 0.3f), Quaternion.identity);
                }

                if (map.GetTile(x, y).type == Map.TileType.Empty)
                {
                    Instantiate(emptyTile, new Vector3(originX + x * 0.3f, originY + y * 0.3f), Quaternion.identity);
                }

                if (map.GetTile(x, y).type == Map.TileType.Start)
                {
                    Instantiate(startTile, new Vector3(originX + x * 0.3f, originY + y * 0.3f), Quaternion.identity);
                }

                if (map.GetTile(x, y).type == Map.TileType.End)
                {
                    Instantiate(endTile, new Vector3(originX + x * 0.3f, originY + y * 0.3f), Quaternion.identity);
                }
            }
        }
    }

    public void Reset()
    {
        players.Clear();
        currentWaitTime = 0.0f;
        randomImpostor = 0;
        startingSaboteurSelect = true;
        currentSaboteurSelectionPlayer = 0;
        currentGameState = GameState.ASSIGNING_PLAYERS;
        map = new Map(30, 30);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.ASSIGNING_PLAYERS)
        {
            int currPlayerBeingAssigned = players.Count + 1;
            bool selectedAPlayer = false;

            if (Input.GetKeyDown("joystick 1 button 0"))
            {
                Player newPlayer = new Player();
                newPlayer.number = currPlayerBeingAssigned;
                newPlayer.joypad = 1;
                players.Add(newPlayer);
                selectedAPlayer = true;
            }
            else if (Input.GetKeyDown("joystick 2 button 0"))
            {
                Player newPlayer = new Player();
                newPlayer.number = currPlayerBeingAssigned;
                newPlayer.joypad = 2;
                players.Add(newPlayer);
                selectedAPlayer = true;
            }
            else if (Input.GetKeyDown("joystick 3 button 0"))
            {
                Player newPlayer = new Player();
                newPlayer.number = currPlayerBeingAssigned;
                newPlayer.joypad = 3;
                players.Add(newPlayer);
                selectedAPlayer = true;
            }
            else if (Input.GetKeyDown("joystick 4 button 0"))
            {
                Player newPlayer = new Player();
                newPlayer.number = currPlayerBeingAssigned;
                newPlayer.joypad = 4;
                players.Add(newPlayer);
                selectedAPlayer = true;
            }

            if (selectedAPlayer)
                Debug.Log("Player " + currPlayerBeingAssigned + " Selected!");

            if (players.Count >= 2)
                currentGameState = GameState.SELECTING_SABOTEUR;
        }
        else if (currentGameState == GameState.SELECTING_SABOTEUR)
        {
            if (startingSaboteurSelect)
            {
                Debug.Log("All players close your eyes!");
                startingSaboteurSelect = false;
                randomImpostor = Random.Range(0, 4);
                currentWaitTime = 1.0f;
            }

            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0.0f)
            {
                Debug.Log("Player " + (currentSaboteurSelectionPlayer + 1) + " Open your eyes!");
                
                if (currentSaboteurSelectionPlayer == randomImpostor)
                    Debug.Log("You are the impostor!");

                currentSaboteurSelectionPlayer++;
                currentWaitTime = 1.0f;

                if (currentSaboteurSelectionPlayer >= 4)
                    currentGameState = GameState.GAMEPLAY;
            }
        }
        else if (currentGameState == GameState.GAMEPLAY)
        {
        }

        /*string[] joystickNames = Input.GetJoystickNames();
        for (int i = 0; i < joystickNames.Length; i++)
        {
            float horizontal = Input.GetAxis("Horizontal" + (i + 1));
            float vertical = Input.GetAxis("Vertical" + (i + 1));
            bool fire = Input.GetKey("joystick button 0");

            // Use the input values here, for example:
            Debug.Log("Joystick " + (i + 1) + ": " + horizontal + ", " + vertical + ", " + fire);
        }*/
    }
}
