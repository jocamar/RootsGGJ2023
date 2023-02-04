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
    bool printedStartVoteMsg = false;
    bool printedStartDiscussionMsg = false;
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
    int currBlockedPlayer = -1;

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
        printedStartDiscussionMsg = false;
        printedStartVoteMsg = false;
        currentSaboteurSelectionPlayer = 0;
        currentGameplayPlayer = 0;
        currentGameState = GameState.ASSIGNING_PLAYERS;
        totalMoves = 30;
        Destroy(mapObject);
        currBlockedPlayer = -1;
    }

    public int GetCurrentPlayerNumber()
    {
        return players.Count;
    }

    public void AddNewPlayer(int joypadNumber)
    {
        Player newPlayer = new Player();
        newPlayer.number = players.Count + 1;
        newPlayer.joypad = joypadNumber;
        players.Add(newPlayer);

        PlayerSelected(newPlayer.number);
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

                if ((Input.GetKeyDown("joystick " + players[playerOrder[currentGameplayPlayer]].joypad + " button 0") && players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Count >= 1)
                            || players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Count >= 3)
                {
                    Debug.Log("Player " + (playerOrder[currentGameplayPlayer] + 1) + " has finished!");
                    currentGameplayPlayer++;

                    if (currentGameplayPlayer == currBlockedPlayer)
                        currentGameplayPlayer++;

                    printedPlayerStartMoveMsg = false;

                    if (currentGameplayPlayer >= 4)
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

                printedStartDiscussionMsg = false;
                currentGameState = GameState.DISCUSSION;
                currentWaitTime = 20.0f;
            }
        }
        else if (currentGameState == GameState.DISCUSSION)
        {
            if (!printedStartDiscussionMsg)
            {
                Debug.Log("Discuss!");
                printedStartDiscussionMsg = true;
            }

            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0.0f)
            {
                currBlockedPlayer = -1;
                printedStartVoteMsg = false;
                currentGameState = GameState.VOTING;
                currentWaitTime = 10.0f;
            }
        }
        else if (currentGameState == GameState.VOTING)
        {
            currentWaitTime -= Time.deltaTime;

            if (!printedStartVoteMsg)
            {
                Debug.Log("Start Voting!");
                printedStartVoteMsg = true;
            }

            if (movement.action.triggered)
            {
                Debug.Log("Triggered select vote!");
                switch (movement.action.ReadValue<Vector2>())
                {
                    case Vector2 v when v.Equals(Vector2.up):
                        players[0].currentlySelectedVotePlayer = (players[0].currentlySelectedVotePlayer+1) % (PLAYERNUMBER_MAX+1);
                        break;

                    case Vector2 v when v.Equals(Vector2.down):
                        players[0].currentlySelectedVotePlayer = (players[0].currentlySelectedVotePlayer - 1) % (PLAYERNUMBER_MAX+1);
                        break;
                }
            }

            if (Input.GetKeyDown("joystick 1 button 0"))
            {
                Debug.Log("Player 1 locked vote!");
                players[0].lockedVote = true;
            }

            if (Input.GetKeyDown("joystick 2 button 0"))
            {
                Debug.Log("Player 2 locked vote!");
                players[1].lockedVote = true;
            }

            if (Input.GetKeyDown("joystick 3 button 0"))
            {
                Debug.Log("Player 3 locked vote!");
                players[2].lockedVote = true;
            }

            if (Input.GetKeyDown("joystick 4 button 0"))
            {
                Debug.Log("Player 4 locked vote!");
                players[3].lockedVote = true;
            }

            if (currentWaitTime <= 0.0f || AllPlayersLockedVote())
            {
                int mostVotedPlayer = GetMostVotedPlayer();
                if (mostVotedPlayer >= 0)
                {
                    Debug.Log("Player " + mostVotedPlayer + " was blocked!");
                    currBlockedPlayer = mostVotedPlayer;
                }

                foreach (Player p in players)
                {
                    p.movesForCurrentRound.Clear();
                    p.currentlySelectedVotePlayer = 0;
                    p.lockedVote = false;
                }

                currentGameState = GameState.GAMEPLAY;
                startingGameplayState = true;
                currentGameplayPlayer = 0;
            }
        }
    }

    bool AllPlayersLockedVote()
    {
        foreach (Player p in players)
        {
            if (!p.lockedVote)
                return false;
        }
        return true;
    }

    int GetMostVotedPlayer()
    {
        int[] votes = { 0, 0, 0, 0 };
        foreach (Player p in players)
        {
            if (p.lockedVote)
                votes[p.currentlySelectedVotePlayer]++;
        }

        int highestVoted = -1;
        if (GetHighest(votes, out highestVoted))
            return highestVoted;
        return -1;
    }

    static bool GetHighest(int[] array, out int highest)
    {
        if (array.Length == 0)
        {
            highest = 0;
            return false;
        }

        highest = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > highest)
            {
                highest = array[i];
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != highest)
            {
                return true;
            }
        }

        return false;
    }
}
