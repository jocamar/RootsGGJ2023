using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 1f;   //Movement Speed of the Player
    public Vector2 movement; //Movement Axis
    [SerializeField]
    CharacterController characterController;      //Player Rigidbody Component

    // Update is called once per frame
    void Update()
    {
        Vector2 _moveDirection = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
        characterController.Move(new Vector3(_moveDirection.x * 25, 0, _moveDirection.y * 25) * Time.deltaTime);
    }

}
