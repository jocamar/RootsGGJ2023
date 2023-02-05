using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInGameUI : MonoBehaviour
{
    public GameObject voteLabel;
    public GameObject playingLabel;
    public GameObject voteValue;
    public GameObject playerLabel;
    public GameObject blockedLabel;

    private Player player = null;
    private int totalPlayers = -1;
    // Start is called before the first frame update
    void Start()
    {
        playerLabel.SetActive(false);
        voteLabel.SetActive(false);
        playingLabel.SetActive(false);
        voteValue.SetActive(false);
        blockedLabel.SetActive(false);
    }

    public void Initialize(Player p, int numPlayers)
    {
        totalPlayers = numPlayers;
        player = p;
        voteLabel.SetActive(false);
        playingLabel.SetActive(false);
        voteValue.SetActive(false);
        playerLabel.SetActive(true);
        blockedLabel.SetActive(false);
    }

    public void StartBlock()
    {
        blockedLabel.SetActive(true);
    }

    public void StopBlock()
    {
        blockedLabel.SetActive(false);
    }

    public void StartVoting()
    {
        voteLabel.SetActive(true);
        voteValue.SetActive(true);
    }

    public void StopVoting()
    {
        voteLabel.SetActive(false);
        voteValue.SetActive(false);
    }

    public void StartMoving()
    {
        playingLabel.SetActive(true);
    }

    public void StopMoving()
    {
        playingLabel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            voteValue.GetComponent<TextMeshProUGUI>().text = player.currentlySelectedVotePlayer < totalPlayers ? "" + (player.currentlySelectedVotePlayer + 1) : "NONE";

            if (player.lockedVote)
            {
                voteLabel.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                voteLabel.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
        }
    }
}
