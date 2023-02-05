using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSpriteController : MonoBehaviour
{
    public GameObject RootH;
    public GameObject RootV;
    public GameObject RootEndRight;
    public GameObject RootEndLeft;
    public GameObject RootEndUp;
    public GameObject RootEndDown;
    public GameObject RootCurveLeftDown;
    public GameObject RootCurveLeftUp;
    public GameObject RootCurveRightUp;
    public GameObject RootCurveRightDown;

    Player.MoveDirections myPrev;
    Player.MoveDirections myNext;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void HideAll()
    {
        RootH.SetActive(false);
        RootV.SetActive(false);
        RootEndRight.SetActive(false);
        RootEndLeft.SetActive(false);
        RootEndUp.SetActive(false);
        RootEndDown.SetActive(false);
        RootCurveLeftDown.SetActive(false);
        RootCurveLeftUp.SetActive(false);
        RootCurveRightDown.SetActive(false);
        RootCurveRightUp.SetActive(false);
    }

    void ActivateCorrectSprite()
    {
        if (myPrev == Player.MoveDirections.LEFT || myPrev == Player.MoveDirections.RIGHT)
        {
            if (myNext == Player.MoveDirections.LEFT || myNext == Player.MoveDirections.RIGHT)
            {
                RootH.SetActive(true);
                return;
            }
        }

        if (myPrev == Player.MoveDirections.UP || myPrev == Player.MoveDirections.DOWN)
        {
            if (myNext == Player.MoveDirections.UP || myNext == Player.MoveDirections.DOWN)
            {
                RootV.SetActive(true);
                return;
            }
        }

        if (myPrev == Player.MoveDirections.UP)
        {
            if (myNext == Player.MoveDirections.NONE)
            {
                RootEndUp.SetActive(true);
                return;
            }
            else if (myNext == Player.MoveDirections.LEFT)
            {
                RootCurveLeftDown.SetActive(true);
                return;
            }
            else if (myNext == Player.MoveDirections.RIGHT)
            {
                RootCurveRightDown.SetActive(true);
                return;
            }
        }

        if (myPrev == Player.MoveDirections.DOWN)
        {
            if (myNext == Player.MoveDirections.NONE)
            {
                RootEndDown.SetActive(true);
                return;
            }
            else if (myNext == Player.MoveDirections.LEFT)
            {
                RootCurveLeftUp.SetActive(true);
                return;
            }
            else if (myNext == Player.MoveDirections.RIGHT)
            {
                RootCurveRightUp.SetActive(true);
                return;
            }
        }

        if (myPrev == Player.MoveDirections.LEFT)
        {
            if (myNext == Player.MoveDirections.NONE)
            {
                RootEndLeft.SetActive(true);
                return;
            }
            else if (myNext == Player.MoveDirections.UP)
            {
                RootCurveRightUp.SetActive(true);
                return;
            }
            else if (myNext == Player.MoveDirections.DOWN)
            {
                RootCurveRightDown.SetActive(true);
                return;
            }
        }

        if (myPrev == Player.MoveDirections.RIGHT)
        {
            if (myNext == Player.MoveDirections.NONE)
            {
                RootEndRight.SetActive(true);
                return;
            }
            else if (myNext == Player.MoveDirections.UP)
            {
                RootCurveLeftUp.SetActive(true);
                return;
            }
            else if (myNext == Player.MoveDirections.DOWN)
            {
                RootCurveLeftDown.SetActive(true);
                return;
            }
        }
    }

    public void SetMoveDirections(Player.MoveDirections prev, Player.MoveDirections next)
    {
        myPrev = prev;
        myNext = next;
        HideAll();
        ActivateCorrectSprite();
    }

    public void ChangeNextDir(Player.MoveDirections next)
    {
        myNext = next;
        HideAll();
        ActivateCorrectSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
