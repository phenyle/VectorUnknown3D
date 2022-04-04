using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class VisVectController : MonoBehaviour
{
    public static VV_PlayerVectors playerVectors;
    public static VV_Axis axis;
    public static VV_Anchors anchors;
    public static VV_Grid grid;
    public static VV_Projections projections;


    [ReadOnly] public static Vector3 startPosition; //start position of this puzzle in WorldSpace
    [ReadOnly] public static Vector3 goalPosition; //goal position of this puzzle in WorldSpace    
    protected static Vector3 puzzleScale; //this is the scaling factor for each dimension needed to marry the player values to WorldSpace values

    protected static int quadrant = 0;

    protected static Vector3 X_negative, X_positive = Vector3.zero;
    protected static Vector3 Y_negative, Y_positive = Vector3.zero;
    protected static Vector3 Z_negative, Z_positive = Vector3.zero;


    public void InitVisVector(Vector3 start, Vector3 goal, Vector3 answer)
    {
        startPosition = start;
        goalPosition = goal;
        puzzleScale = CalcPuzzleScale(start - goal, answer);

        ToggleVisualVectors(true);
        //reset graph ends to center of graph
        X_negative = X_positive = start;
        Y_negative = Y_positive = start;
        Z_negative = Z_positive = start;

        quadrant = DetermineQuadrant(answer);
        LimitGraphSize();

        axis.Set_Xaxis(answer, X_negative, X_positive);
        axis.Set_Yaxis(answer, Y_negative, Y_positive);
        axis.Set_Zaxis(answer, Z_negative, Z_positive);

        axis.Set_XLabel(start, answer, puzzleScale);
        axis.Set_YLabel(start, answer, puzzleScale);
        axis.Set_ZLabel(start, answer, puzzleScale);

        if (quadrant <= 0)
            grid.Draw2DGrid(puzzleScale, answer);
        else
            grid.Draw3DGrid(puzzleScale, answer);

        playerVectors.UpdateVector1(startPosition, startPosition);
        playerVectors.UpdateVector2(startPosition, startPosition);

        ToggleVisualVectors(false);

        playerVectors.UpdateGoal(start, goal);
        anchors.UpdateGoal(start, goal);
    }

    /// <summary>
    /// actions to perform on event call for vector1 changing
    /// </summary>
    public void Vector1EventList()
    {
        if (GameControllerPuzzle04.instance.currPuzzle != null)
        {
            if (GameControllerPuzzle04.instance.P04W.card1 != null)
            {
                Vector3 endPosition = startPosition + RescaleVector(GameControllerPuzzle04.instance.P04W.card1.cardVector * GameControllerPuzzle04.instance.P04W.getScalar1Value());
                playerVectors.UpdateVector1(startPosition, endPosition);
                projections.UpdateVector1(startPosition, endPosition);
                anchors.UpdateVector1(startPosition, endPosition);
            }
            else
            {
                playerVectors.UpdateVector1(startPosition, startPosition);
                projections.UpdateVector1(startPosition, startPosition);
                anchors.UpdateVector1(startPosition, startPosition);
            }

            Vector2EventList();
        }
    }

    /// <summary>
    /// actions to perform on event call for vector2 changing
    /// </summary>
    public void Vector2EventList()
    {
        if (GameControllerPuzzle04.instance.currPuzzle != null)
            if (GameControllerPuzzle04.instance.P04W.card2 != null)
            {
                Vector3 endPosition = playerVectors.vector1.GetPosition(1) + RescaleVector(GameControllerPuzzle04.instance.P04W.card2.cardVector * GameControllerPuzzle04.instance.P04W.getScalar2Value());
                playerVectors.UpdateVector2(playerVectors.vector1.GetPosition(1), endPosition);
                projections.UpdateVector2(startPosition, playerVectors.vector1.GetPosition(1), endPosition);
                anchors.UpdateVector2(startPosition, endPosition);
            }
            else
            {
                playerVectors.UpdateVector2(playerVectors.vector1.GetPosition(1), playerVectors.vector1.GetPosition(1));
                projections.UpdateVector2(startPosition, playerVectors.vector1.GetPosition(1), playerVectors.vector1.GetPosition(1));
                anchors.UpdateVector2(startPosition, startPosition);
            }
    }

    public void FinalVectEventList()
    {
        if (GameControllerPuzzle04.instance.P04W.card1 != null && GameControllerPuzzle04.instance.P04W.card2 != null)
        {
            playerVectors.UpdateAnswer(startPosition, playerVectors.vector2.GetPosition(1));
            projections.UpdateAnswer(startPosition, playerVectors.vector2.GetPosition(1));
        }
    }


    private int DetermineQuadrant(Vector3 answer)
    {
        if (answer.z == 0) // 1D/2D grid
        {
            if (answer.y == 0)
                return -2;  // 1D grid, X direction answer
            else if (answer.x == 0)
                return -1;  // 1D grid, Y direction answer
            else
                return 0; // 2D grid
        }

        if (answer.x > 0)
            if (answer.y > 0)
                if (answer.z > 0)
                    return 1;
                else
                    return 2;
            else
                if (answer.z > 0)
                    return 3;
                else
                    return 4;
        else
            if (answer.y > 0)
                if (answer.z > 0)
                    return 5;
                else
                    return 6;
            else
                if (answer.z > 0)
                    return 7;
                else
                    return 8;
    }

    public void LimitGraphSize()
    {
        switch (quadrant)
        {
            case -2: //1D "grid" X direction || answer.y == 0
                X_negative += new Vector3(((-1) * puzzleScale.x * grid.GridSize), 0, 0);
                X_positive += new Vector3((puzzleScale.x * grid.GridSize), 0, 0);

                Y_negative += new Vector3(0, ((-1) * puzzleScale.x * grid.GridSize), 0);
                Y_positive += new Vector3(0, (puzzleScale.x * grid.GridSize), 0);
                break;

            case -1: //1D "grid" Y direction || answer.x == 0
                X_negative += new Vector3(((-1) * puzzleScale.y * grid.GridSize), 0, 0);
                X_positive += new Vector3((puzzleScale.y * grid.GridSize), 0, 0);

                Y_negative += new Vector3(0, ((-1) * puzzleScale.y * grid.GridSize), 0);
                Y_positive += new Vector3(0, (puzzleScale.y * grid.GridSize), 0);
                break;

            case 0: //2D grid
                X_negative += new Vector3(((-1) * puzzleScale.x * grid.GridSize), 0, 0);
                X_positive += new Vector3((puzzleScale.x * grid.GridSize), 0, 0);

                Y_negative += new Vector3(0, ((-1) * puzzleScale.y * grid.GridSize), 0);
                Y_positive += new Vector3(0, (puzzleScale.y * grid.GridSize), 0);
                break;

            case 1: // <1,1,1>
                X_negative += new Vector3(0f, 0f, 0f);
                X_positive += new Vector3(grid.GridSize * puzzleScale.x, 0f, 0f);

                Y_negative += new Vector3(0f, 0f, 0f);
                Y_positive += new Vector3(0f, grid.GridSize * puzzleScale.y, 0f);

                Z_negative += new Vector3(0f, 0f, 0f);
                Z_positive += new Vector3(0f, 0f, grid.GridSize * puzzleScale.z);
                break;

            case 2: // <1,1,-1>
                X_negative += new Vector3(0f, 0f, 0f);
                X_positive += new Vector3(grid.GridSize * puzzleScale.x, 0f, 0f);

                Y_negative += new Vector3(0f, 0f, 0f);
                Y_positive += new Vector3(0f, grid.GridSize * puzzleScale.y, 0f);

                Z_negative += new Vector3(0f, 0f, -1 * grid.GridSize * puzzleScale.z);
                Z_positive += new Vector3(0f, 0f, 0f);
                break;

            case 3: // <1,-1,1>
                X_negative += new Vector3(0f, 0f, 0f);
                X_positive += new Vector3(grid.GridSize * puzzleScale.x, 0f, 0f);

                Y_negative += new Vector3(0f, -1 * grid.GridSize * puzzleScale.y, 0f);
                Y_positive += new Vector3(0f, 0f, 0f);

                Z_negative += new Vector3(0f, 0f, 0f);
                Z_positive += new Vector3(0f, 0f, grid.GridSize * puzzleScale.z);
                break;

            case 4: // <1,-1,-1>
                X_negative += new Vector3(0f, 0f, 0f);
                X_positive += new Vector3(grid.GridSize * puzzleScale.x, 0f, 0f);

                Y_negative += new Vector3(0f, -1 * grid.GridSize * puzzleScale.y, 0f);
                Y_positive += new Vector3(0f, 0f, 0f);

                Z_negative += new Vector3(0f, 0f, -1 * grid.GridSize * puzzleScale.z);
                Z_positive += new Vector3(0f, 0f, 0f);
                break;
                 
            case 5: // <-1,1,1>
                X_negative += new Vector3(-1 * grid.GridSize * puzzleScale.x, 0f, 0f);
                X_positive += new Vector3(0f, 0f, 0f);

                Y_negative += new Vector3(0f, 0f, 0f);
                Y_positive += new Vector3(0f, grid.GridSize * puzzleScale.y, 0f);

                Z_negative += new Vector3(0f, 0f, 0f);
                Z_positive += new Vector3(0f, 0f, grid.GridSize * puzzleScale.z);
                break;

            case 6: // <-1,1,-1>
                X_negative += new Vector3(-1 * grid.GridSize * puzzleScale.x, 0f, 0f);
                X_positive += new Vector3(0f, 0f, 0f);

                Y_negative += new Vector3(0f, 0f, 0f);
                Y_positive += new Vector3(0f, grid.GridSize * puzzleScale.y, 0f);

                Z_negative += new Vector3(0f, 0f, -1 * grid.GridSize * puzzleScale.z);
                Z_positive += new Vector3(0f, 0f, 0f);
                break;

            case 7: // <-1,-1,1>
                X_negative += new Vector3(-1 * grid.GridSize * puzzleScale.x, 0f, 0f);
                X_positive += new Vector3(0f, 0f, 0f);

                Y_negative += new Vector3(0f, -1 * grid.GridSize * puzzleScale.y, 0f);
                Y_positive += new Vector3(0f, 0f, 0f);

                Z_negative += new Vector3(0f, 0f, 0f);
                Z_positive += new Vector3(0f, 0f, grid.GridSize * puzzleScale.z);
                break;

            case 8: // <-1,-1,-1>
                X_negative += new Vector3(-1 * grid.GridSize * puzzleScale.x, 0f, 0f);
                X_positive += new Vector3(0f, 0f, 0f);

                Y_negative += new Vector3(0f, -1 * grid.GridSize * puzzleScale.y, 0f);
                Y_positive += new Vector3(0f, 0f, 0f);

                Z_negative += new Vector3(0f, 0f, -1 * grid.GridSize * puzzleScale.z);
                Z_positive += new Vector3(0f, 0f, 0f);
                break;
        }
    }

    public void ToggleVisualVectors(bool val)
    {
        playerVectors.TogglePlayerVectors(val);
        axis.ToggleAxis(val);
        anchors.ToggleAnchors(val);
        grid.ToggleGrid(val);
        projections.ToggleProjections(val);
    }


    protected Vector3 RescaleVector(Vector3 input)
    {
        input.x *= puzzleScale.x;
        input.y *= puzzleScale.y;
        input.z *= puzzleScale.z;
        return input;
    }

    protected static void LineProperties(LineRenderer line, Color color, float width)
    {
        line.positionCount = 2;
        line.startColor = line.endColor = color;
        line.startWidth = line.endWidth = width;
        line.gameObject.GetComponent<Outline>().OutlineColor = color * 0.3f;
    }


    private Vector3 CalcPuzzleScale(Vector3 worldSpace, Vector3 answer)
    {
        Vector3 returnVector;
        //size you want, divided by the size you have
        //X-scale
        if (answer.x != 0)
            returnVector.x = Mathf.Abs(worldSpace.x / answer.x);
        else
            returnVector.x = 0;

        //Z-scale
        if (answer.y != 0)
            returnVector.y = Mathf.Abs(worldSpace.y / answer.y);
        else
            returnVector.y = 0;

        //Y-scale
        if (answer.z != 0)
            returnVector.z = Mathf.Abs(worldSpace.z / answer.z);
        else
            returnVector.z = 0;

        return returnVector;
    }
}
