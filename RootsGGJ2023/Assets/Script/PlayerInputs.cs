using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public bool playerSelect;
    public bool playerSelect_Down;

    public bool movementReset;

    public PlayerInput playerInput;
    public static List<PlayerInputs> allPlayers = new List<PlayerInputs>();
    public Vector2 movementOutput;

    public static Vector2 SnapVector(Vector2 value)
    {
        if (value.magnitude < 0.5f) return Vector2.zero;

        float dot = Vector2.Dot(Vector2.up, value);

        if (dot > 0.5f) return Vector2.up;
        if (dot < -0.5f) return Vector2.down;
        if (value.x > 0) return Vector2.right;
        else return Vector2.left;
    }

    public void PlayerMovement(InputAction.CallbackContext ctx)
    {
        Vector2 movement = SnapVector(ctx.ReadValue<Vector2>());

        if (movement.magnitude > 0 && movementReset)
        {
            movementOutput = movement;
            movementReset = false;
        }

        if (movement == Vector2.zero) movementReset = true;
    }

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
        movementOutput = Vector2.zero;
    }
}
