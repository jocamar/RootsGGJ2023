using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public bool playerSelect;
    public bool playerSelect_Down;

    public bool playerSabotage;
    public bool playerSabotage_Down;

    public bool movementReset;

    public PlayerInput playerInput;
    public static List<PlayerInputs> allPlayers = new List<PlayerInputs>();
    public Vector2 movementOutput;

    private List<ShakeRequest> allShakeRequest = new List<ShakeRequest>();

    public Gamepad gamepad => Gamepad.all.FirstOrDefault(g => playerInput.devices.Any(d => d.deviceId == g.deviceId));

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
            Shake();
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
            Shake();
        }
    }

    public void PlayerSabotage(InputAction.CallbackContext ctx)
    {
        bool pastValue = playerSabotage;
        playerSabotage = ctx.ReadValueAsButton();

        if (!pastValue && playerSabotage)
        {
            playerSabotage_Down = true;
            Shake();
        }
    }

    public void Shake(float power = 0.5f, float time = 0.05f, float location = 0f)
    {
        allShakeRequest.Add(new ShakeRequest(power, time, location));
    }

    private void OnApplicationQuit()
    {
        InputSystem.ResetHaptics();
    }

    public class ShakeRequest
    {
        public float power;
        public float time;
        public float location;

        public ShakeRequest(float power, float time, float location)
        {
            this.power = power;
            this.time = time;
            this.location = location;
        }
    }

    private void Awake()
    {
        allPlayers.Add(this);
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (gamepad != null)
        {
            float bestPowerLeft = 0;
            float bestPowerRight = 0;
            List<ShakeRequest> validShakeRequest = new List<ShakeRequest>();
            foreach (var shakeRequest in allShakeRequest)
            {
                if (shakeRequest.time > 0)
                {
                    validShakeRequest.Add(shakeRequest);
                    float powerLeft = 1 - Mathf.Clamp01(Mathf.Abs(-1 - shakeRequest.location) - 1);
                    float powerRight = 1 - Mathf.Clamp01(Mathf.Abs(1 - shakeRequest.location) - 1);
                    bestPowerLeft = Mathf.Max(powerLeft, bestPowerLeft);
                    bestPowerRight = Mathf.Max(powerRight, bestPowerRight);
                }
                shakeRequest.time = Mathf.MoveTowards(shakeRequest.time, 0, Time.deltaTime);
            }
            allShakeRequest = validShakeRequest;
            if (bestPowerLeft > 0 || bestPowerRight > 0)
            {
                gamepad.SetMotorSpeeds(bestPowerLeft, bestPowerRight);
            }
            else
            {
                gamepad.ResetHaptics();
            }
        }
        else
        {
            allShakeRequest = new List<ShakeRequest>();
        }
    }

    private void LateUpdate()
    {
        playerSelect_Down = false;
        playerSabotage_Down = false;
        movementOutput = Vector2.zero;
    }
}