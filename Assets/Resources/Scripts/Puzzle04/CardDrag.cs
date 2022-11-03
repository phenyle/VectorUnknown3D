using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardDrag : EventTrigger
{
    private Vector3 startPos;
    private CardController cardController;
    public int cardIndexID = 0;

    private bool dragging = false;

    private Puzzle04Window P04W;

    public void Start()
    {
        P04W = GameControllerPuzzle04.instance.P04W;
        cardController = this.gameObject.GetComponent<CardController>();
        startPos = this.transform.position;
        dragging = false;
    }


    public void Update()
    {
        if (dragging)
        {
            this.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Dragging this: " + this.gameObject.name);
        dragging = true;
        this.transform.SetAsLastSibling();

        if (P04W.getAnswer1() != null && cardController == P04W.getAnswer1())
            P04W.setAnswer1(null);

        if (P04W.getAnswer2() != null && cardController == P04W.getAnswer2())
            P04W.setAnswer2(null); 
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Dropped this: " + this.gameObject.name);
        dragging = false;


        if (inAnsSlot1(this.gameObject))
        {
            this.transform.position = P04W.getAnsField1trans().position;
            
            //Look to see if another card is already there and returning the old card
            for (int i = 0; i < P04W.getCards().Count; i++)
            {
                //if this card is not the card we just dragged
                if (P04W.getCards()[i].gameObject.name.CompareTo(this.gameObject.name) != 0)
                {
                    //if this other card is already in AnsSlot1 return it to its start position
                    if (inAnsSlot1(P04W.getCards()[i]))
                    {
                        GameObject.Find(P04W.getCards()[i].gameObject.name).transform.position = GameObject.Find(P04W.getCards()[i].gameObject.name).GetComponent<CardDrag>().startPos;
                    }
                }

            }
            P04W.setAnswer1(cardController);
        }
        else if (inAnsSlot2(this.gameObject))
        {
            this.transform.position = P04W.getAnsField2trans().position;

            //Look to see if another card is already there and returning the old card
            for (int i = 0; i < P04W.getCards().Count; i++)
            {
                //if this card is not the card we just dragged
                if (P04W.getCards()[i].gameObject.name.CompareTo(this.gameObject.name) != 0)
                {
                    //if this other card is already in AnsSlot2 return it to its start position
                    if (inAnsSlot2(P04W.getCards()[i]))
                    {
                        GameObject.Find(P04W.getCards()[i].gameObject.name).transform.position = GameObject.Find(P04W.getCards()[i].gameObject.name).GetComponent<CardDrag>().startPos;
                    }
                }

            }
            P04W.setAnswer2(cardController);
        }
        else
            this.transform.position = P04W.GetStartPositions()[cardIndexID].position;
    }

    /// <summary>
    /// Check if this card has been dragged into AnswerSlot1 on the UI
    /// </summary>
    /// <returns> false if not in AnswerSlot1 boundaries
    /// otherwise returns true</returns>
    private bool inAnsSlot1(GameObject card)
    {
        Debug.Log("Dragged Card Pos: " + this.transform.position);

        Debug.Log("Ans Card1 Pos: " + P04W.getAnsField1trans().position);

        Transform ans1Trans = P04W.getAnsField1trans();
        RectTransform ans1Rect = ans1Trans.GetComponent<RectTransform>();

        //Check if outside left boundary
        if (card.transform.position.x < ans1Trans.position.x - ans1Rect.rect.width / 2.0f)
            return false;       
        //Check if outside right boundary
        else if (card.transform.position.x > ans1Trans.position.x + ans1Rect.rect.width / 2.0f)
            return false;        
       //Check if outside bottom boundary
       else if (card.transform.position.y < ans1Trans.position.y - ans1Rect.rect.height / 2.0f)
           return false;
       //Check if outside top boundary
       else if (card.transform.position.y > ans1Trans.position.y + ans1Rect.rect.height / 2.0f)
           return false;

        else
            return true;
    }

    /// <summary>
    /// Check if this card has been dragged into AnswerSlot2 on the UI
    /// </summary>
    /// <returns> false if not in AnswerSlot2 boundaries
    /// otherwise returns true</returns>
    private bool inAnsSlot2(GameObject card)
    {
        Debug.Log("Dragged Card Pos: " + this.transform.position);

        Debug.Log("Ans Card2 Pos: " + P04W.getAnsField2trans().position);

        Transform ans2Trans = P04W.getAnsField2trans();
        RectTransform ans2Rect = ans2Trans.GetComponent<RectTransform>();


        //Check if above left boundary
        if (card.transform.position.x < ans2Trans.position.x - ans2Rect.rect.width / 2.0f)
            return false;
        //Check if below right boundary
        else if (card.transform.position.x > ans2Trans.position.x + ans2Rect.rect.width / 2.0f)
            return false;
        //Check if above bottom boundary
        else if (card.transform.position.y < ans2Trans.position.y - ans2Rect.rect.height / 2.0f)
            return false;
        //Check if below top boundary
        else if (card.transform.position.y > ans2Trans.position.y + ans2Rect.rect.height / 2.0f)
            return false;
        else
            return true;
    }

    public Vector2 getStartPos()
    {
        return startPos;
    }

}
