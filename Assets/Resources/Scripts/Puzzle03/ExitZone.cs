using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    public Transform puzzle03Player;
    public Transform puzzle03PlayerZone;
    public int puzzleID;

    private GameControllerPuzzle03 GCP03;

    private void Start()
    {
        GCP03 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle03>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Puzzle03Player")
        {
            Destroy(puzzle03PlayerZone.gameObject);
            Destroy(puzzle03Player.gameObject);

            GCP03.FinishSubLevel();

            GameRoot.instance.audioService.PlayFXAudio(Constants.audioP03FinishSubPuzzle);
        }
    }
}
