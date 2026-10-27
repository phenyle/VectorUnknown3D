using UnityEngine;

public class DatabasePuzzle03 : MonoBehaviour
{
    public SpanValue[] spanValues;
    public Transform[] subLevelPlanes;

    private GameControllerPuzzle03 GCP03;

    public void InitDatabase()
    {
        Debug.Log("Connect GameController of Puzzle03");
        GCP03 = GetComponent<GameControllerPuzzle03>();

        
        //Init sub level environment
        //foreach(Transform T in subLevelPlanes)
        //{
        //    T.gameObject.SetActive(false);
        //}
        //subLevelPlanes[0].gameObject.SetActive(true);   
    }

    //public void SetPuzzleActive(int subPuzzleID)
    //{
    //    subLevelPlanes[subPuzzleID].gameObject.SetActive(true);
    //}

    //public Transform GetSubLevelPlanes(int subPuzzleID)
    //{
    //    return subLevelPlanes[subPuzzleID];
    //}
}
