using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    //private Inputs_Player inputPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Instance already exist, destroying object !");
            Destroy(this);
        }

        //inputPlayer = new Inputs_Player();
    }

    //private void OnEnable()
    //{
    //    inputPlayer.Enable();
    //}

    //private void OnDisable()
    //{
    //    inputPlayer.Disable();
    //}

    //public Vector2 GetPlayerMovement()
    //{
    //    return inputPlayer.Player.Movement.ReadValue<Vector2>();
    //}

    //public Vector2 GetMouseDelta()
    //{
    //    return inputPlayer.Player.Look.ReadValue<Vector2>();
    //}

    //public bool GetPlayerJump()
    //{
    //    return inputPlayer.Player.Jump.triggered;
    //}

    //public bool GetPlayerEscapeKey()
    //{
    //    return inputPlayer.Player.EscapeKey.triggered;
    //}

    //public bool GetPlayerShoot()
    //{
    //    return inputPlayer.Player.Shoot.triggered;
    //}

    //public bool GetPlayerReload()
    //{
    //    return inputPlayer.Player.Reload.triggered;
    //}
}
