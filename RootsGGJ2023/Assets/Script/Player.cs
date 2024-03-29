using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player
{
    public Player(PlayerInputs playerInputs, int playerIndex, bool isDisrupt, bool isSaboteur, Color color)
    {
        this.playerInputs = playerInputs;
        this.playerIndex = playerIndex;
        this.isDisrupt = isDisrupt;
        this.isSaboteur = isSaboteur;
        this.color = color;
    }

    public int playerIndex;
    public PlayerInputs playerInputs;
    public Color color;
    public bool isDisrupt;
    public bool isSaboteur;
    public List<MoveDirections> movesForCurrentRound = new List<MoveDirections>();

    public enum MoveDirections
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public int currentlySelectedVotePlayer = 0;
    public bool lockedVote = false;
}
