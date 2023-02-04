using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public bool playerSelect;
    public bool playerSelect_Down;

    public PlayerInput playerInput;
    public static List<PlayerInputs> allPlayers = new List<PlayerInputs>();

    public void PlayerSelect(InputAction.CallbackContext ctx)
    {
        bool pastValue = playerSelect;
        playerSelect = ctx.ReadValueAsButton();

        if (!pastValue && playerSelect)
        {
            playerSelect_Down = true;
        }
    }

    private void Awake()
    {
        allPlayers.Add(this);
        playerInput = GetComponent<PlayerInput>();
    }

    private void LateUpdate()
    {
        playerSelect_Down = false;
    }
}
