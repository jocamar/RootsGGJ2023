using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject GameManagerUI;

    [SerializeField]
    private InputActionReference movement;

    [SerializeField]
    private GameObject PlayerInputs;

    enum GameState
    {
        STARTMENU,
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
    GameState currentGameState = GameState.STARTMENU;
    float currentWaitTime = 0f;
    int currentSaboteurSelectionPlayer = 0;
    int currentGameplayPlayer = 0;
    int randomImpostor = 0;
    bool startingSaboteurSelect = true;
    bool startingGameplayState = true;
    bool mapGenerated = false;
    bool printedPlayerStartMoveMsg = false;
    public GameObject PlayerMessage;

    public string CloseingEyes;
    public string ImpostorDisplay;
    TextMeshProUGUI PlayerMessage_text;

    int PLAYERNUMBER_MAX = 4;
    public int PlayerMaxNumber { get { return PLAYERNUMBER_MAX; } }
    int[] playerOrder;
    GameObject mapObject;
    Map map;
    int currentPositionX;
    int currentPositionY;
    int totalMoves = 30;

    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object !");
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerMessage_text = PlayerMessage.GetComponent<TextMeshProUGUI>();
    }

    public void StartPlayerSelection()
    {
        currentGameState = GameState.ASSIGNING_PLAYERS;
    }

    public void StartGame()
    {
        currentGameState = GameState.SELECTING_SABOTEUR;
        GameManagerUI.SetActive(true);
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
        currentGameplayPlayer = 0;
        currentGameState = GameState.ASSIGNING_PLAYERS;
        totalMoves = 30;
        Destroy(mapObject);
    }

    public int GetCurrentPlayerNumber()
    {
        return players.Count;
    }

    public bool IsPlayerInputInGame(PlayerInputs playerInputs)
    {
        return players.Find(x => x.playerInputs == playerInputs) != null;
    }

    public void AddNewPlayer(PlayerInputs playerInput)
    {
        Player newPlayer = new Player(playerInput, players.Count + 1);
        players.Add(newPlayer);

        PlayerSelected(newPlayer.playerIndex);
    }

    private void PlayerSelected(int playerNumber)
    {
        Debug.Log("Player " + playerNumber + " Selected!");
    }

    // Update is called once per frame
    void Update()
    {
        float originX = -2.5f;
        float originY = -2f;

        if (currentGameState == GameState.SELECTING_SABOTEUR)
        {
            if (startingSaboteurSelect)
            {
                Debug.Log("All players close your eyes!");
                PlayerMessage_text.text = CloseingEyes;
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
                PlayerMessage_text.text = ImpostorDisplay;
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
                    map.Initialize(this, 15, 15);

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
                    players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Clear();
                }

                if (movement.action.triggered)
                {
                    Debug.Log("Triggered move for player " + playerOrder[currentGameplayPlayer] + "!");
                    switch (movement.action.ReadValue<Vector2>())
                    {
                        case Vector2 v when v.Equals(Vector2.up):
                            players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Add(Player.MoveDirections.UP);
                            break;

                        case Vector2 v when v.Equals(Vector2.down):
                            players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Add(Player.MoveDirections.DOWN);
                            break;

                        case Vector2 v when v.Equals(Vector2.left):
                            players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Add(Player.MoveDirections.LEFT);
                            break;

                        case Vector2 v when v.Equals(Vector2.right):
                            players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Add(Player.MoveDirections.RIGHT);
                            break;
                    }
                }

                if (players[playerOrder[currentGameplayPlayer]].playerInputs.playerSelect_Down || players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Count >= 3)
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
                    if (map.GetTile(newX, newY).type == Map.TileType.Empty)
                    {
                        currentPositionX = newX;
                        currentPositionY = newY;
                        Map.Tile newTile = new Map.Tile();
                        newTile.type = Map.TileType.Root;
                        map.SetTile(currentPositionX, currentPositionY, newTile);

                        var newObj = Instantiate(rootTile, new Vector3(originX + currentPositionX * 0.3f, originY + currentPositionY * 0.3f), Quaternion.identity);
                        map.SetObject(currentPositionX, currentPositionY, newObj);
                    }
                    else if (map.GetTile(newX, newY).type == Map.TileType.End)
                    {
                        Debug.Log("Good guys lost!");
                        currentGameState = GameState.GAME_END;
                    }
                }

                totalMoves--;

                if (totalMoves <= 0)
                {
                    Debug.Log("Good guys lost!");
                    currentGameState = GameState.GAME_END;
                }

                currentGameState = GameState.DISCUSSION;
                currentWaitTime = 20.0f;
            }
        }
        else if (currentGameState == GameState.DISCUSSION)
        {
            Debug.Log("Discuss!");
            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0.0f)
            {
                currentGameState = GameState.VOTING;
                currentWaitTime = 10.0f;
            }
        }
        else if (currentGameState == GameState.VOTING)
        {
            Debug.Log("Vote!");
            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0.0f)
            {
                foreach (Player p in players)
                {
                    p.movesForCurrentRound.Clear();
                }

                currentGameState = GameState.GAMEPLAY;
                startingGameplayState = true;
                currentGameplayPlayer = 0;
            }
        }
    }
}
