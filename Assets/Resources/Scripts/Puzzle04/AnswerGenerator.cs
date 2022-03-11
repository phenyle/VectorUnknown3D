using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerGenerator
{
    private Vector3 minVector;
    private Vector3 modVector;

    private Direction puzzleDir;
    private int range;

    private int difficulty;

    List<Vector3> cardVectors;

    private int escal1;
    private Vector3 evect1;
    private int escal2;
    private Vector3 evect2;

    private int lockPreventCount = 0;

    public AnswerGenerator(int diff, Vector3 initVector, int dynRange, Direction dir)
    {
        difficulty = diff;
        minVector = initVector;
        puzzleDir = dir;
        range = dynRange;
        modVector = minVector + DynamicGenerator();
        cardVectors = new List<Vector3>();
    }

    private Vector3 DynamicGenerator()
    {
        Vector3 temp = Vector3.zero;

        switch (difficulty)
        {
            case 1:
                switch (puzzleDir)
                {
                    //Zero vectors are allowed here for "extra" vectors
                    case Direction.X:
                        if (minVector.x > 0)
                            temp.x += Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                        else
                            temp.x -= Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                        temp.y = 0.0f;
                        temp.z = 0.0f;
                        break;

                    case Direction.Y:
                        temp.x = 0.0f;
                        if (minVector.y > 0)
                            temp.y += Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                        else
                            temp.y -= Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                        temp.z = 0.0f;
                        break;
                }

                break;

            case 2:
                if (minVector.x > 0)
                    temp.x += Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                else
                    temp.x -= Random.Range(Mathf.CeilToInt(range / 2), range + 1);

                if (minVector.y > 0)
                    temp.y += Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                else
                    temp.y -= Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                temp.z = 0.0f;
                break;

            case 3:
                //No zero vectors allowed
                if (minVector.x > 0)
                    temp.x += Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                else
                    temp.x -= Random.Range(Mathf.CeilToInt(range / 2), range + 1);

                if (minVector.y > 0)
                    temp.y += Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                else
                    temp.y -= Random.Range(Mathf.CeilToInt(range / 2), range + 1);

                if (minVector.z > 0)
                    temp.z += Random.Range(Mathf.CeilToInt(range / 2), range + 1);
                else
                    temp.z -= Random.Range(Mathf.CeilToInt(range / 2), range + 1);

                break;
        }

        return temp;
    }

    public List<Vector3> AnswerCards(int maxScalar)
    {       
        Vector3 answer1, answer2, extra1, extra2;
        int scal1;

        if (difficulty == 1)
        {
            if (puzzleDir == Direction.X)
            {
                cardVectors.Add(new Vector3(1, 0, 0));
                cardVectors.Add(new Vector3(-1, 0, 0));

                if (Random.value > 0.5f)
                    cardVectors.Add(new Vector3(2, 0, 0));
                else
                    cardVectors.Add(new Vector3(-2, 0, 0));

                if (Random.value > 0.5f)
                    cardVectors.Add(new Vector3(3, 0, 0));
                else
                    cardVectors.Add(new Vector3(-3, 0, 0));

            }
            if (puzzleDir == Direction.Y)
            {
                cardVectors.Add(new Vector3(0, 1, 0));
                cardVectors.Add(new Vector3(0, -1, 0));

                if (Random.value > 0.5f)
                    cardVectors.Add(new Vector3(0, 2, 0));
                else
                    cardVectors.Add(new Vector3(0, -2, 0));

                if (Random.value > 0.5f)
                    cardVectors.Add(new Vector3(0, 3, 0));
                else
                    cardVectors.Add(new Vector3(0, -3, 0));
            }
        }
        else
        {
            scal1 = Random.Range(Mathf.RoundToInt(maxScalar / 2), maxScalar + 1);
            do { answer1 = VectorGenerator(); } while ((answer1 * scal1) == modVector);
            cardVectors.Add(answer1);

            answer2 = VectorPairFinder(scal1, answer1, maxScalar);
            cardVectors.Add(answer2);

            do { extra1 = VectorGenerator(); } while (extra1 == Vector3.zero);
            cardVectors.Add(extra1);

            do { extra2 = VectorGenerator(); } while (extra2 == Vector3.zero);
            cardVectors.Add(extra2);
        }

        answerShuffle();

        return cardVectors;

    }
    public Puzzle04Controller.obsData PuzzleData(Puzzle04Controller.obsData puzzleData)
    {
        if (difficulty != 1)
        {
            puzzleData.e_Scal1 = escal1;
            puzzleData.e_Vect1 = evect1;
            puzzleData.e_Scal2 = escal2;
            puzzleData.e_Vect2 = evect2;
        }
        else
        {
            puzzleData.e_Scal1 = 0;
            puzzleData.e_Vect1 = Vector3.zero;
            puzzleData.e_Scal2 = 1;
            puzzleData.e_Vect2 = minVector;
        }
        puzzleData.answer = modVector;

        return puzzleData;
    }

    public Vector3 FinalAnswer()
    {
        return modVector;
    }

    private Vector3 VectorGenerator()
    {
        Vector3 temp = Vector3.zero;

        switch (difficulty)
        {
            case 1:
                switch (puzzleDir)
                {
                    //Zero vectors are allowed here for "extra" vectors
                    case Direction.X:
                        do
                        {
                            temp.x = Random.Range(-3, 4);
                            temp.y = 0.0f;
                            temp.z = 0.0f;
                        } while (temp == Vector3.zero || !testUnique(temp));
                        break;


                    case Direction.Y:
                        do
                        {
                            temp.x = 0.0f;
                            temp.y = Random.Range(-3, 4);
                            temp.z = 0.0f;
                        } while (temp == Vector3.zero || !testUnique(temp));
                        break;

                }

                break;

            case 2:
                //No zero vectors allowed
                do
                {
                    temp.x = Random.Range(-3, 4);
                    temp.y = Random.Range(-3, 4);
                    temp.z = 0.0f;
                } while (temp == Vector3.zero || !testUnique(temp));
                break;

            case 3:
                //No zero vectors allowed
                do
                {
                    temp.x = Random.Range(-3, 4);
                    temp.y = Random.Range(-3, 4);
                    temp.z = Random.Range(-3, 4);
                } while (temp == Vector3.zero || !testUnique(temp));

                break;
        }

        return temp;
    }

    private Vector3 VectorPairFinder(int scal1, Vector3 answer1, int maxScalar)
    {
        Vector3 answer2 = Vector3.zero;
        float maxDimension = 0.0f;
        int scal2 = 0;

        //first find which version of scal is closest to goal
        if (V1reversedScal(scal1, answer1))
            scal1 *= -1;

        //Set answer2 to exact displacement
        answer2 = minVector - (answer1 * scal1);

        //find the largest Dimension of displacement (answer2)        
        maxDimension = Mathf.Abs(answer2.x);
        if (maxDimension < Mathf.Abs(answer2.y))
            maxDimension = Mathf.Abs(answer2.y);
        if (maxDimension < Mathf.Abs(answer2.z))
            maxDimension = Mathf.Abs(answer2.z);

        //We'll use that maxDimension to determine what the scalar should be
        for (int i = 1; i <= maxScalar; i++)
        {
            //if there is no suitable scalar we'll shave 1/8 off and try again
            if (i == maxScalar)
            {
                answer2 *= 0.875f;

                maxDimension = Mathf.Abs(answer2.x);
                if (maxDimension < Mathf.Abs(answer2.y))
                    maxDimension = Mathf.Abs(answer2.y);
                if (maxDimension < Mathf.Abs(answer2.z))
                    maxDimension = Mathf.Abs(answer2.z);

                i = 1;
            }
            //if the largest dimension can be reduced to less than 3 by some scalar
            else if ((maxDimension / i) <= 3.0f)
            {
                scal2 = i;
                break;
            }
        }

        //With scal2 set, we need to chop the displacment vector (answer2) into that size and round the results
        answer2.x = Mathf.RoundToInt(answer2.x / scal2);
        answer2.y = Mathf.RoundToInt(answer2.y / scal2);
        answer2.z = Mathf.RoundToInt(answer2.z / scal2);




        // If this Pair vector is the same vector as the first Vector OR the scaled sums results
        // in some final vector with a ZERO, then start over


        //Now that we have both answer1 and answer2 created in the set of V, we need to adjust the finalGoal to match these rounded vectors
        modVector = scal1 * answer1 + scal2 * answer2;
        //set the expected card and scal values for the database object
        escal1 = scal1;
        evect1 = answer1;
        escal2 = scal2;
        evect2 = answer2;

        if (!testUnique(answer2) || !ValidateProposedAnswer(scal1 * answer1 + scal2 * answer2))
        {
            cardVectors.Clear();
            do { answer1 = VectorGenerator(); } while (answer1 == Vector3.zero || (answer1 * scal1) == minVector);
            cardVectors.Add(answer1);
            if (lockPreventCount > 100)
            {
                lockPreventCount = 0;
                maxScalar++;
                if (maxScalar > 10)
                    maxScalar = 10;
            }
            return VectorPairFinder(scal1, answer1, maxScalar);
        }
        else
            return answer2;
    }

    private bool ValidateProposedAnswer(Vector3 testFinal)
    {
        bool goodSize = true;
        if (minVector.x != 0)
        {
            if (Mathf.Abs(testFinal.x) < Mathf.Abs(minVector.x * 0.75f))
                goodSize = false;
            if (Mathf.Abs(testFinal.x) > Mathf.Abs(minVector.x * 1.5f))
                goodSize = false;

        }
        if (minVector.y != 0)
        {
            if (Mathf.Abs(testFinal.y) < Mathf.Abs(minVector.y * 0.75f))
                goodSize = false;
            if (Mathf.Abs(testFinal.y) > Mathf.Abs(minVector.y * 1.5f))
                goodSize = false;

        }
        if (minVector.z != 0)
        {
            if (Mathf.Abs(testFinal.z) < Mathf.Abs(minVector.z * 0.75f))
                goodSize = false;
            if (Mathf.Abs(testFinal.z) > Mathf.Abs(minVector.z * 1.5f))
                goodSize = false;

        }

        //switch (difficulty)
        //{
        //    case 1:
        //        switch (puzzleDir)
        //        {
        //            //Zero vectors are allowed here for "extra" vectors
        //            case Direction.X:
        //                //compare the magnitude sizes
        //                if (Mathf.Abs(testFinal.x) < Mathf.Abs(minVector.x))
        //                    larger = false;

        //                //compare the positivity/negativity
        //                if ((minVector.x > 0 && (testFinal.x) <= 0) || (minVector.x < 0 && (testFinal.x) >= 0))
        //                    larger = false;

        //                break;

        //            case Direction.Y:
        //                //compare the magnitude sizes
        //                if (Mathf.Abs(testFinal.y) < Mathf.Abs(minVector.y))
        //                    larger = false;

        //                //compare the positivity/negativity
        //                if ((minVector.y > 0 && (testFinal.y) <= 0) || (minVector.y < 0 && (testFinal.y) >= 0))
        //                    larger = false;
        //                break;
        //        }

        //        break;

        //    case 2:
        //        //compare the magnitude sizes
        //        if (Mathf.Abs(testFinal.x) < Mathf.Abs(minVector.x))
        //            larger = false;
        //        if (Mathf.Abs(testFinal.y) < Mathf.Abs(minVector.y))
        //            larger = false;

        //        //compare the positivity/negativity
        //        if ((minVector.x > 0 && (testFinal.x) <= 0) || (minVector.x < 0 && (testFinal.x) >= 0))
        //            larger = false;
        //        if ((minVector.y > 0 && (testFinal.y) <= 0) || (minVector.y < 0 && (testFinal.y) >= 0))
        //            larger = false;
        //        break;

        //    case 3:
        //        //compare the magnitude sizes
        //        if (Mathf.Abs(testFinal.x) < Mathf.Abs(minVector.x))
        //            larger = false;
        //        if (Mathf.Abs(testFinal.y) < Mathf.Abs(minVector.y))
        //            larger = false;
        //        if (Mathf.Abs(testFinal.z) < Mathf.Abs(minVector.z))
        //            larger = false;
        //        //compare the positivity/negativity
        //        if ((minVector.x > 0 && (testFinal.x) <= 0) || (minVector.x < 0 && (testFinal.x) >= 0))
        //            larger = false;
        //        if ((minVector.y > 0 && (testFinal.y) <= 0) || (minVector.y < 0 && (testFinal.y) >= 0))
        //            larger = false;
        //        if ((minVector.z > 0 && (testFinal.z) <= 0) || (minVector.z < 0 && (testFinal.z) >= 0))
        //            larger = false;
        //        break;
        //}
        if (!goodSize)
            lockPreventCount++;

        return goodSize;
    }



    private bool V1reversedScal(int scal, Vector3 vector)
    {
        float origDisplacementMag = (minVector - (vector * scal)).magnitude;
        float reversedDisplacementMag = (minVector - (vector * (scal * (-1)))).magnitude;

        //basically, if the original displacement is smaller leave it as is, otherwise flip the scalar when calculating "answer2".
        return origDisplacementMag > reversedDisplacementMag;
    }

    private bool testUnique(Vector3 test)
    {
        for(int i = 0; i < cardVectors.Count; i++)
        {
            if (test == cardVectors[i])
                return false;
        }
        return true;
    }

    public void answerShuffle()
    {
        //Shuffle Cards
        int count = cardVectors.Count;
        int last = count - 1;
        for (int i = 0; i < last; i++)
        {
            int r = Random.Range(i, count);
            Vector3 temp = cardVectors[i];
            cardVectors[i] = cardVectors[r];
            cardVectors[r] = temp;
        }
    }

}
