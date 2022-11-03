using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    public TextMeshProUGUI textVector;
    public GameObject[] brackets;

    public Vector3 cardVector { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject bracket in brackets)
            bracket.SetActive(false);
        brackets[GameControllerPuzzle04.instance.Difficulty - 1].SetActive(true);
    }

    public void SetVector(Vector3 vector)
    {
        cardVector = vector;
        SetText();
    }

    public void SetBrackets()
    {
        foreach (GameObject bracket in brackets)
            bracket.SetActive(false);
        brackets[GameControllerPuzzle04.instance.Difficulty - 1].SetActive(true);
    }

    private void SetText()
    {
        switch(GameControllerPuzzle04.instance.Difficulty)
        {
            case 1:
                if(GameControllerPuzzle04.instance.currPuzzle.getDirection() == Direction.X)
                    textVector.SetText(cardVector.x.ToString("0"));
                if (GameControllerPuzzle04.instance.currPuzzle.getDirection() == Direction.Y)
                    textVector.SetText(cardVector.y.ToString("0"));
                break;
            case 2:
                textVector.SetText(cardVector.x.ToString("0") + "\n" +
                                    cardVector.y.ToString("0"));
                break;
            case 3:
                textVector.SetText(cardVector.x.ToString("0") + "\n" + 
                                    cardVector.z.ToString("0") + "\n" +
                                    cardVector.y.ToString("0"));

                break;
        }
    }
}
