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

    List<Player> players;
    GameState currentGameState = GameState.ASSIGNING_PLAYERS;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset()
    {
        players.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.ASSIGNING_PLAYERS)
        {
            int currPlayerBeingAssigned = players.Count + 1;

            if (Input.GetKeyDown("joystick 1 button 0"))
            {
                Player newPlayer = new Player();
                newPlayer.number = currPlayerBeingAssigned;
                newPlayer.joypad = 1;
                players.Add(newPlayer);
            }
            else if (Input.GetKeyDown("joystick 2 button 0"))
            {
                Player newPlayer = new Player();
                newPlayer.number = currPlayerBeingAssigned;
                newPlayer.joypad = 2;
                players.Add(newPlayer);
            }
            else if (Input.GetKeyDown("joystick 3 button 0"))
            {
                Player newPlayer = new Player();
                newPlayer.number = currPlayerBeingAssigned;
                newPlayer.joypad = 3;
                players.Add(newPlayer);
            }
            else if (Input.GetKeyDown("joystick 4 button 0"))
            {
                Player newPlayer = new Player();
                newPlayer.number = currPlayerBeingAssigned;
                newPlayer.joypad = 4;
                players.Add(newPlayer);
            }

            if (players.Count >= 4)
                currentGameState = GameState.SELECTING_SABOTEUR;
        }

        string[] joystickNames = Input.GetJoystickNames();
        for (int i = 0; i < joystickNames.Length; i++)
        {
            float horizontal = Input.GetAxis("Horizontal" + (i + 1));
            float vertical = Input.GetAxis("Vertical" + (i + 1));
            bool fire = Input.GetKey("joystick button 0");

            // Use the input values here, for example:
            Debug.Log("Joystick " + (i + 1) + ": " + horizontal + ", " + vertical + ", " + fire);
        }
    }
}
