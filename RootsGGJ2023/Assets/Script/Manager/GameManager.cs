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
    public GameObject PlayerMessage;
    public GameObject DiscussMessage;

    public GameObject P1OpenAudio;
    public GameObject P2OpenAudio;
    public GameObject P3OpenAudio;
    public GameObject P4OpenAudio;
    public GameObject P1CloseAudio;
    public GameObject P2CloseAudio;
    public GameObject P3CloseAudio;
    public GameObject P4CloseAudio;
    public GameObject AllOpen;

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
    public GameObject NutrientsMessage;

    public List<GameObject> Player_UIs;

    public string CloseingEyes;
    public string ImpostorDisplay;
    public string Discuss;
    public int discussionTimer;
    int Displaytimer;
    TextMeshProUGUI PlayerMessage_text;
    TextMeshProUGUI PlayerNutrients_text;
    TextMeshProUGUI DiscussMessage_text;

    int PLAYERNUMBER_MAX = 4;
    public int PlayerMaxNumber { get { return PLAYERNUMBER_MAX; } }
    int[] playerOrder;
    GameObject mapObject;
    Map map;
    int currentPositionX;
    int currentPositionY;
    int totalMoves = 30;
    int currBlockedPlayer = -1;

    float mapTileScale = 0.5f;

    bool delayAfterPlayerMoved = false;
    bool saboteurHasUsedDisrupt = false;

    Player.MoveDirections lastMoveDirection = Player.MoveDirections.NONE;
    GameObject lastRootTile = null;

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
        PlayerNutrients_text = NutrientsMessage.GetComponent<TextMeshProUGUI>();
        DiscussMessage_text = DiscussMessage.GetComponent<TextMeshProUGUI>();
    }

    public void StartPlayerSelection()
    {
        currentGameState = GameState.ASSIGNING_PLAYERS;
    }

    public void StartGame()
    {
        lastMoveDirection = Player.MoveDirections.NONE;
        currentGameState = GameState.SELECTING_SABOTEUR;
        GameManagerUI.SetActive(true);
    }

    int[] GetRandomOrder()
    {
        List<int> playerArray = new List<int>();

        for (int p = 0; p < players.Count; p++)
            playerArray.Add(p);

        for (int i = 0; i < playerArray.Count; i++)
        {
            int temp = playerArray[i];
            int randomIndex = Random.Range(i, playerArray.Count);
            playerArray[i] = playerArray[randomIndex];
            playerArray[randomIndex] = temp;
        }
        return playerArray.ToArray();
    }

    public void ResetManager()
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
        lastMoveDirection = Player.MoveDirections.NONE;
        lastRootTile = null;
        delayAfterPlayerMoved = false;
        saboteurHasUsedDisrupt = false;
    }

    public int GetCurrentPlayerNumber()
    {
        return players.Count;
    }

    public bool IsPlayerInputInGame(PlayerInputs playerInputs)
    {
        return players.Find(x => x.playerInputs == playerInputs) != null;
    }

    public void AddNewPlayer(PlayerInputs playerInput, Color color)
    {
        Player newPlayer = new Player(playerInput, players.Count + 1, false, false, color);
        players.Add(newPlayer);

        PlayerSelected(newPlayer.playerIndex, color);
    }

    private void PlayerSelected(int playerNumber, Color color)
    {
        Debug.Log($"Player {playerNumber} Selected!, he is color {color}");
    }

    // Update is called once per frame
    void Update()
    {
        float originX = -3f;
        float originY = -3f;

        if (currentGameState == GameState.SELECTING_SABOTEUR)
        {
            if (startingSaboteurSelect)
            {
                Debug.Log("All players close your eyes!");
                PlayerMessage_text.text = CloseingEyes;
                startingSaboteurSelect = false;
                randomImpostor = Random.Range(0, players.Count);
                currentWaitTime = 5.0f;
            }

            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0.0f)
            {
                if (currentSaboteurSelectionPlayer >= players.Count)
                {
                    AllOpen.GetComponent<AudioSource>().Play();
                    currentGameState = GameState.GAMEPLAY;
                    PlayerNutrients_text.text = "" + totalMoves;
                }
                else
                {
                    GameObject[] audiosOpen = { P1OpenAudio, P2OpenAudio, P3OpenAudio, P4OpenAudio };
                    GameObject[] audiosClose = { P1CloseAudio, P2CloseAudio, P3CloseAudio, P4CloseAudio };

                    Debug.Log("Player " + (currentSaboteurSelectionPlayer + 1) + " Open your eyes!");
                    audiosOpen[currentSaboteurSelectionPlayer].GetComponent<AudioSource>().Play();
                    if (currentSaboteurSelectionPlayer + 1 < players.Count)
                        audiosClose[currentSaboteurSelectionPlayer].GetComponent<AudioSource>().PlayDelayed(3.0f);

                    if (currentSaboteurSelectionPlayer == randomImpostor)
                    {
                        Debug.Log("You are the impostor!");
                        PlayerMessage_text.text = ImpostorDisplay;
                        Player player = players[randomImpostor];
                        PlayerMessage_text.color = player.color;
                        player.isSaboteur = true;
                    }
                    else
                    {
                        PlayerMessage_text.text = "You are not the impostor! :)";
                        Debug.Log("You are not the impostor!");
                    }
                    currentSaboteurSelectionPlayer++;
                    currentWaitTime = 6.0f;
                }
            }
        }
        else if (currentGameState == GameState.GAMEPLAY)
        {
            if (startingGameplayState)
            {
                PlayerMessage_text.text = "";

                for (int i = 0; i < players.Count; i++)
                {
                    Player_UIs[i].GetComponent<PlayerInGameUI>().Initialize(players[i], players.Count);
                }

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
                                var obj = Instantiate(obstacleTile, new Vector3(originX + x * mapTileScale, originY + y * mapTileScale), Quaternion.identity);
                                map.SetObject(x, y, obj);
                            }

                            if (map.GetTile(x, y).type == Map.TileType.Empty)
                            {
                                var obj = Instantiate(emptyTile, new Vector3(originX + x * mapTileScale, originY + y * mapTileScale), Quaternion.identity);
                                map.SetObject(x, y, obj);
                            }

                            if (map.GetTile(x, y).type == Map.TileType.Start)
                            {
                                var obj = Instantiate(startTile, new Vector3(originX + x * mapTileScale, originY + y * mapTileScale), Quaternion.identity);
                                map.SetObject(x, y, obj);
                            }

                            if (map.GetTile(x, y).type == Map.TileType.End)
                            {
                                var obj = Instantiate(endTile, new Vector3(originX + x * mapTileScale, originY + y * mapTileScale), Quaternion.identity);
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
                if (!delayAfterPlayerMoved)
                {
                    if (!printedPlayerStartMoveMsg)
                    {
                        if (playerOrder[currentGameplayPlayer] != currBlockedPlayer)
                        {
                            Player_UIs[playerOrder[currentGameplayPlayer]].GetComponent<PlayerInGameUI>().StartMoving();
                            Debug.Log("Player " + (playerOrder[currentGameplayPlayer] + 1) + " make your move!");
                        }

                        printedPlayerStartMoveMsg = true;
                        players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Clear();
                    }

                    if (playerOrder[currentGameplayPlayer] != currBlockedPlayer)
                    {
                        Player player = players[playerOrder[currentGameplayPlayer]];
                        Player.MoveDirections moveDirection = player.playerInputs.movementOutput switch
                        {
                            Vector2 v when v.Equals(Vector2.up) => Player.MoveDirections.UP,
                            Vector2 v when v.Equals(Vector2.down) => Player.MoveDirections.DOWN,
                            Vector2 v when v.Equals(Vector2.left) => Player.MoveDirections.LEFT,
                            Vector2 v when v.Equals(Vector2.right) => Player.MoveDirections.RIGHT,
                            _ => Player.MoveDirections.NONE,
                        };

                        if (moveDirection != Player.MoveDirections.NONE)
                        {
                            Debug.Log("Triggered move for player " + playerOrder[currentGameplayPlayer] + "!");
                            player.movesForCurrentRound.Add(moveDirection);
                        }
                    }

                    if ((players[playerOrder[currentGameplayPlayer]].playerInputs.playerSelect_Down && players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Count >= 0)
                            || players[playerOrder[currentGameplayPlayer]].movesForCurrentRound.Count >= 3
                            || playerOrder[currentGameplayPlayer] == currBlockedPlayer)
                    {
                        delayAfterPlayerMoved = true;
                        currentWaitTime = 2.0f;
                    }
                }
                else
                {
                    currentWaitTime -= Time.deltaTime;

                    if (currentWaitTime <= 0.0f)
                    {
                        Player_UIs[playerOrder[currentGameplayPlayer]].GetComponent<PlayerInGameUI>().StopMoving();
                        Debug.Log("Player " + (playerOrder[currentGameplayPlayer] + 1) + " has finished!");
                        currentGameplayPlayer++;
                        delayAfterPlayerMoved = false;

                        if (currentGameplayPlayer >= players.Count)
                        {
                            currentGameState = GameState.PATH_REVEAL;
                        }
                        else
                        {
                            if (playerOrder[currentGameplayPlayer] == currBlockedPlayer)
                                currentGameplayPlayer++;

                            printedPlayerStartMoveMsg = false;

                            if (currentGameplayPlayer >= players.Count)
                            {
                                currentGameState = GameState.PATH_REVEAL;
                            }
                        }
                    }
                }

                if (!saboteurHasUsedDisrupt && randomImpostor < players.Count && players[randomImpostor].playerInputs.playerSabotage_Down)
                {
                    Debug.Log("Saboteur used Disrupt!");
                    players[playerOrder[currentGameplayPlayer]].isDisrupt = true;
                    saboteurHasUsedDisrupt = true;
                }
            }
        }
        else if (currentGameState == GameState.PATH_REVEAL)
        {
            List<Player.MoveDirections> completePath = new List<Player.MoveDirections>();
            Player.MoveDirections tentativeLastMoveDir = Player.MoveDirections.NONE;
            foreach (int i in playerOrder)
            {
                if (players[i].movesForCurrentRound.Count <= 0)
                    continue;

                if (players[i].isDisrupt == true)
                {
                    int moveCount = players[i].movesForCurrentRound.Count;
                    int replacedMove = Random.Range(0, moveCount);

                    Player.MoveDirections blockedDirection = tentativeLastMoveDir != Player.MoveDirections.NONE ? tentativeLastMoveDir : lastMoveDirection;


                    int randomMove;

                    do
                    {
                        randomMove = Random.Range(0, 3);
                    }
                    while (randomMove == (int)blockedDirection);

                    switch (randomMove)
                    {
                        case 0:
                            players[i].movesForCurrentRound.RemoveAt(replacedMove);
                            players[i].movesForCurrentRound.Insert(replacedMove, Player.MoveDirections.UP);
                            break;
                        case 1:
                            players[i].movesForCurrentRound.RemoveAt(replacedMove);
                            players[i].movesForCurrentRound.Insert(replacedMove, Player.MoveDirections.DOWN);
                            break;
                        case 2:
                            players[i].movesForCurrentRound.RemoveAt(replacedMove);
                            players[i].movesForCurrentRound.Insert(replacedMove, Player.MoveDirections.LEFT);
                            break;
                        case 3:
                            players[i].movesForCurrentRound.RemoveAt(replacedMove);
                            players[i].movesForCurrentRound.Insert(replacedMove, Player.MoveDirections.RIGHT);
                            break;
                    }
                }

                tentativeLastMoveDir = players[i].movesForCurrentRound[players[i].movesForCurrentRound.Count - 1];
                completePath.AddRange(players[i].movesForCurrentRound);
            }

            foreach (GameObject obj in Player_UIs)
                obj.GetComponent<PlayerInGameUI>().StopBlock();

            GameObject prevRootTile = null;
            for (int i = 0; i < completePath.Count; i++)
            {
                int newX = currentPositionX;
                int newY = currentPositionY;
                if (completePath[i] == Player.MoveDirections.DOWN)
                {
                    newY--;
                }
                else if (completePath[i] == Player.MoveDirections.UP)
                {
                    newY++;
                }
                else if (completePath[i] == Player.MoveDirections.RIGHT)
                {
                    newX++;
                }
                else if (completePath[i] == Player.MoveDirections.LEFT)
                {
                    newX--;
                }

                if (newX < map.GetWidth() && newX >= 0 && newY >= 0 && newY < map.GetHeight())
                {
                    if (map.GetTile(newX, newY).type == Map.TileType.Empty)
                    {
                        Player.MoveDirections dir = completePath[i];
                        Player.MoveDirections dirAfter = i < completePath.Count - 1 ? completePath[i + 1] : Player.MoveDirections.NONE;

                        if (i == 0 && lastRootTile != null)
                        {
                            lastRootTile.GetComponent<RootSpriteController>().ChangeNextDir(completePath[i]);
                        }

                        if (prevRootTile != null)
                        {
                            prevRootTile.GetComponent<RootSpriteController>().ChangeNextDir(dir);
                        }

                        lastMoveDirection = dir;
                        currentPositionX = newX;
                        currentPositionY = newY;
                        Map.Tile newTile = new Map.Tile();
                        newTile.type = Map.TileType.Root;
                        map.SetTile(currentPositionX, currentPositionY, newTile);

                        var newObj = Instantiate(rootTile, new Vector3(originX + currentPositionX * mapTileScale, originY + currentPositionY * mapTileScale), Quaternion.identity);
                        newObj.GetComponent<RootSpriteController>().SetMoveDirections(dir, dirAfter);
                        map.SetObject(currentPositionX, currentPositionY, newObj);
                        prevRootTile = newObj;

                        if (dirAfter == Player.MoveDirections.NONE)
                            lastRootTile = newObj;
                    }
                    else if (map.GetTile(newX, newY).type == Map.TileType.End)
                    {
                        Debug.Log("Good guys lost!");
                        currentGameState = GameState.GAME_END;
                    }
                    else if (i >= completePath.Count - 1)
                    {
                        if (prevRootTile != null)
                        {
                            prevRootTile.GetComponent<RootSpriteController>().ChangeNextDir(Player.MoveDirections.NONE);
                        }
                    }
                }

                totalMoves--;

                PlayerNutrients_text.text = ""+totalMoves;

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

            DiscussMessage_text.text = Discuss + currentWaitTime;

            if (currentWaitTime <= 0.0f)
            {
                DiscussMessage_text.text = "";
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

                for (int i = 0; i < players.Count; i++)
                    Player_UIs[i].GetComponent<PlayerInGameUI>().StartVoting();
            }

            
            foreach (Player p in players)
            {
                if (p.lockedVote)
                    continue;

                Player.MoveDirections moveDirection = p.playerInputs.movementOutput switch
                {
                    Vector2 v when v.Equals(Vector2.up) => Player.MoveDirections.UP,
                    Vector2 v when v.Equals(Vector2.down) => Player.MoveDirections.DOWN,
                    _ => Player.MoveDirections.NONE,
                };

                if (moveDirection == Player.MoveDirections.UP) p.currentlySelectedVotePlayer = (p.currentlySelectedVotePlayer + 1) % (players.Count + 1);
                if (moveDirection == Player.MoveDirections.DOWN)
                {
                    int newVote = p.currentlySelectedVotePlayer - 1;
                    if (newVote < 0)
                        newVote = players.Count;
                    p.currentlySelectedVotePlayer = newVote;
                }
            }

            foreach (Player p in players)
            {
                if (p.playerInputs.playerSelect_Down)
                {
                    Debug.Log("Player " + (p.playerIndex+1) + " locked vote!");
                    p.lockedVote = true;
                }
            }

            if (currentWaitTime <= 0.0f || AllPlayersLockedVote())
            {
                int mostVotedPlayer = GetMostVotedPlayer();
                if (mostVotedPlayer >= 0)
                {
                    Debug.Log("Player " + mostVotedPlayer + " was blocked!");
                    currBlockedPlayer = mostVotedPlayer;
                    if (currBlockedPlayer < Player_UIs.Count)
                        Player_UIs[currBlockedPlayer].GetComponent<PlayerInGameUI>().StartBlock();
                }

                for (int i = 0; i < players.Count; i++)
                    Player_UIs[i].GetComponent<PlayerInGameUI>().StopVoting();

                foreach (Player p in players)
                {
                    p.movesForCurrentRound.Clear();
                    p.currentlySelectedVotePlayer = 0;
                    p.lockedVote = false;
                    p.isDisrupt = false;
                }

                currentGameState = GameState.GAMEPLAY;
                startingGameplayState = true;
                saboteurHasUsedDisrupt = false;
                currentGameplayPlayer = 0;
                printedPlayerStartMoveMsg = false;
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
            if (p.lockedVote && p.currentlySelectedVotePlayer < votes.Length)
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

        
        int highestVal = array[0];
        highest = 0;

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > highestVal)
            {
                highestVal = array[i];
                highest = i;
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == highestVal && i != highest)
            {
                return false;
            }
        }

        return true;
    }
}
