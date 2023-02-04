using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public enum MoveDirections
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public int number;
    public int joypad;
    public List<MoveDirections> movesForCurrentRound = new List<MoveDirections>();
}
