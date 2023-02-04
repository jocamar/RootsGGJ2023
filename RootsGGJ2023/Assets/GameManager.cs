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
    public GameObject rootTile;
    public GameObject endTile;
    public GameObject mapPrefab;

    List<Player> players = new List<Player>();
    GameState currentGameState = GameState.ASSIGNING_PLAYERS;
    float currentWaitTime = 0f;
    int currentSaboteurSelectionPlayer = 0;
    int currentGameplayPlayer = 0;
    int randomImpostor = 0;
    bool startingSaboteurSelect = true;
    bool startingGameplayState = true;
    bool mapGenerated = false;
    bool printedPlayerStartMoveMsg = false;

    int[] playerOrder;
    GameObject mapObject;
    Map map;
    int currentPositionX;
    int currentPositionY;
    int totalMoves = 30;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    int[] GetRandomOrder()
    {
        int[] array = { 0, 1, 2, 3 };
        for (int i = 0; i < array.Length; i++)
        {
            int temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
        return array;
    }

    public void Reset()
    {
        mapGenerated = false;
        players.Clear();
        currentWaitTime = 0.0f;
        randomImpostor = 0;
        startingSaboteurSelect = true;
        startingGameplayState = true;
        printedPlayerStartMoveMsg = false;
        currentSaboteurSelectionPlayer = 0;
        currentGameState = GameState.ASSIGNING_PLAYERS;
        totalMoves = 30;
        Destroy(mapObject);
    }

    // Update is called once per frame
    void Update()
    {
        float originX = -2.5f;
        float originY = -2f;

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
            if (startingGameplayState)
            {
                if (!mapGenerated)
                {
                    mapObject = Instantiate(mapPrefab);
                    map = mapObject.GetComponent<Map>();

                    map.GenerateMap(10);

                    for (int x = 0; x < 15; x++)
                    {
                        for (int y = 0; y < 15; y++)
                        {
                            if (map.GetTile(x, y).type == Map.TileType.Obstacle)
                            {
                                var obj = Instantiate(obstacleTile, new Vector3(originX + x * 0.3f, originY + y * 0.3f), Quaternion.identity);
                                map.SetObject(x, y, obj);
                            }

                            if (map.GetTile(x, y).type == Map.TileType.Empty)
                            {
                                var obj = Instantiate(emptyTile, new Vector3(originX + x * 0.3f, originY + y * 0.3f), Quaternion.identity);
                                map.SetObject(x, y, obj);
                            }

                            if (map.GetTile(x, y).type == Map.TileType.Start)
                            {
                                var obj = Instantiate(startTile, new Vector3(originX + x * 0.3f, originY + y * 0.3f), Quaternion.identity);
                                map.SetObject(x, y, obj);
                            }

                            if (map.GetTile(x, y).type == Map.TileType.End)
                            {
                                var obj = Instantiate(endTile, new Vector3(originX + x * 0.3f, originY + y * 0.3f), Quaternion.identity);
                                map.SetObject(x, y, obj);
                            }
                        }
                    }

                    currentPositionX = map.myStartX;
                    currentPositionY = map.myStartY;
                    mapGenerated = true;
                    currentWaitTime = 2.0f;
                }

                currentWaitTime -= Time.deltaTime;

                if (currentWaitTime <= 0.0f)
                {
                    startingGameplayState = false;
                    playerOrder = GetRandomOrder();
                }
            }
            else
            {
                if (!printedPlayerStartMoveMsg)
                {
                    Debug.Log("Player " + (playerOrder[currentGameplayPlayer] + 1) + " make your move!");
                    printedPlayerStartMoveMsg = true;
                }

                if (Input.GetKeyDown("joystick " + players[playerOrder[currentGameplayPlayer]].joypad + " button 0"))
                {
                    Debug.Log("Player " + (playerOrder[currentGameplayPlayer] + 1) + " has finished!");
                    currentGameplayPlayer++;
                    printedPlayerStartMoveMsg = false;

                    if (currentGameplayPlayer >= 2)
                    {
                        currentGameState = GameState.PATH_REVEAL;
                    }
                }

                if (Input.GetKeyDown("joystick " + players[playerOrder[currentGameplayPlayer]].joypad + " button 0"))
                {
                    Debug.Log("Player " + (playerOrder[currentGameplayPlayer] + 1) + " has finished!");
                    currentGameplayPlayer++;
                    printedPlayerStartMoveMsg = false;

                    if (currentGameplayPlayer >= 2)
                    {
                        currentGameState = GameState.PATH_REVEAL;
                    }
                }
            }
        }
        else if (currentGameState == GameState.PATH_REVEAL)
        {
            List<Player.MoveDirections> completePath = new List<Player.MoveDirections>();
            foreach (int i in playerOrder)
            {
                completePath.AddRange(players[i].movesForCurrentRound);
            }

            foreach (Player.MoveDirections dir in completePath)
            {
                int newX = currentPositionX;
                int newY = currentPositionY;
                if (dir == Player.MoveDirections.DOWN)
                {
                    newY--;
                }
                else if (dir == Player.MoveDirections.UP)
                {
                    newY++;
                }
                else if (dir == Player.MoveDirections.RIGHT)
                {
                    newX++;
                }
                else if (dir == Player.MoveDirections.LEFT)
                {
                    newX--;
                }

                if (newX < map.GetWidth() && newX >= 0 && newY >= 0 && newY < map.GetHeight())
                {
                    if (map.GetTile(newX, newY).type == Map.TileType.Empty || map.GetTile(newX, newY).type == Map.TileType.End)
                    {
                        currentPositionX = newX;
                        currentPositionY = newY;
                        Map.Tile newTile = new Map.Tile();
                        newTile.type = Map.TileType.Root;
                        map.SetTile(currentPositionX, currentPositionY, newTile);

                        var newObj = Instantiate(rootTile, new Vector3(originX + currentPositionX * 0.3f, originY + currentPositionY * 0.3f), Quaternion.identity);
                        map.SetObject(currentPositionX, currentPositionY, newObj);
                    }
                }

                totalMoves--;

                if (totalMoves <= 0)
                {
                    Debug.Log("Good guys lost!");
                }
            }
        }
    }
}
