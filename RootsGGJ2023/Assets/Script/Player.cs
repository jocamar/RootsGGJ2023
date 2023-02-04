using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player
{
    public Player(PlayerInputs playerInputs, int playerIndex)
    {
        this.playerInputs = playerInputs;
        this.playerIndex = playerIndex;
    }

    public int playerIndex;
    public PlayerInputs playerInputs;
    public List<MoveDirections> movesForCurrentRound = new List<MoveDirections>();

    public enum MoveDirections
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}
