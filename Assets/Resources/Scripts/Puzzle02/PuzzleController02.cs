using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PuzzleController02 : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;

    [Header("--------Ship Parts--------")]
    [HideInInspector]
    public GameObject shipParent;
    public GameObject shipObject;
    public TextMeshPro vectText;
    public TextMeshPro vectTextTop;
    public int[] shipPos;


    [HideInInspector]
    public List<int[][]> matrixList; //a generate list of 4 possible answer matricies
    [HideInInspector]
    public List<int[]> vectorList; //a trimmed list of 4 possible answer vectors
    [HideInInspector]
    public int[] ansVector; //the expected answer vector picked from vectorList
    [HideInInspector]
    public int[][] ansMatrix; //the expected answer matrix generated using the ansVector and shipPosition;

    private int gcd;

    private List<int[]> level1vectors = new List<int[]>()
                                          { new int[3]{ 1, 0, 0 },
                                            new int[3]{ 0, 0, 1 },
                                            new int[3]{ -1, 0, 0 },
                                            new int[3]{ 0, 0, -1 },
                                            new int[3]{ 1, 0, 1 },
                                            new int[3]{ 1, 0, -1 },
                                            new int[3]{ -1, 0, 1 },
                                            new int[3]{ -1, 0, -1 },
                                            };

    private List<int[]> level2vectors = new List<int[]>()
                                          { new int[3]{ 2, 0, 0 },
                                            new int[3]{ 0, 0, 2 },
                                            new int[3]{ -2, 0, 0 },
                                            new int[3]{ 0, 0, -2 },
                                            new int[3]{ 1, 0, 2 },
                                            new int[3]{ 2, 0, 1 },
                                            new int[3]{ -1, 0, 2 },
                                            new int[3]{ 1, 0, -2 },
                                            new int[3]{ -2, 0, 1 },
                                            new int[3]{ 2, 0, -1 },
                                            new int[3]{ 2, 0, 2 },
                                            new int[3]{ 2, 0, -2 },
                                            new int[3]{ -2, 0, 2 },
                                            new int[3]{ -2, 0, -2 },
                                            new int[3]{ 3, 0, 0 },
                                            new int[3]{ 0, 0, 3 },
                                            new int[3]{ -3, 0, 0 },
                                            new int[3]{ 3, 0, 3 },
                                            new int[3]{ 3, 0, -3 },
                                            new int[3]{ -3, 0, 3 },
                                            new int[3]{ -3, 0, -3 },                                                                                        new int[3]{ 1, 0, 2 },
                                            new int[3]{ 3, 0, 1 },
                                            new int[3]{ -1, 0, 3 },
                                            new int[3]{ 1, 0, -3 },
                                            new int[3]{ -3, 0, 1 },
                                            new int[3]{ 3, 0, -1 },
                                            new int[3]{ 2, 0, 3 },
                                            new int[3]{ 3, 0, 2 },
                                            new int[3]{ -2, 0, 3 },
                                            new int[3]{ 2, 0, -3 },
                                            };

    private List<int[]> level3vectors = new List<int[]>()
                                          { new int[3]{ 2, 0, 0 },
                                            new int[3]{ 0, 0, 2 },
                                            new int[3]{ -2, 0, 0 },
                                            new int[3]{ 0, 0, -2 },
                                            new int[3]{ 1, 0, 2 },
                                            new int[3]{ 2, 0, 1 },
                                            new int[3]{ -1, 0, 2 },
                                            new int[3]{ 1, 0, -2 },
                                            new int[3]{ -2, 0, 1 },
                                            new int[3]{ 2, 0, -1 },
                                            new int[3]{ 2, 0, 2 },
                                            new int[3]{ 2, 0, -2 },
                                            new int[3]{ -2, 0, 2 },
                                            new int[3]{ -2, 0, -2 },
                                            new int[3]{ 3, 0, 0 },
                                            new int[3]{ 0, 0, 3 },
                                            new int[3]{ -3, 0, 0 },
                                            new int[3]{ 3, 0, 3 },
                                            new int[3]{ 3, 0, -3 },
                                            new int[3]{ -3, 0, 3 },
                                            new int[3]{ -3, 0, -3 },                                                                                        new int[3]{ 1, 0, 2 },
                                            new int[3]{ 3, 0, 1 },
                                            new int[3]{ -1, 0, 3 },
                                            new int[3]{ 1, 0, -3 },
                                            new int[3]{ -3, 0, 1 },
                                            new int[3]{ 3, 0, -1 },
                                            new int[3]{ 2, 0, 3 },
                                            new int[3]{ 3, 0, 2 },
                                            new int[3]{ -2, 0, 3 },
                                            new int[3]{ 2, 0, -3 },
                                            };

    public void Awake()
    {
        vectTextTop.gameObject.SetActive(false);
        vectText.gameObject.SetActive(false);
        shipParent = this.gameObject;
        shipObject = shipParent.transform.Find("shipBody").gameObject;
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
    }

    // Start is called before the first frame update
    void Start()
    {
        shipPos = new int[3];
    }


    public void ToggleTopText(bool val)
    {
        vectTextTop.gameObject.SetActive(val);
    }

    public void ToggleFrontText(bool val)
    {
        vectText.gameObject.SetActive(val);
    }

    public void SetPosition(Vector3 pos)
    {
        shipParent.transform.position = pos;
        shipPos = new int[3];
        shipPos[0] = (int)pos.x;
        shipPos[2] = (int)pos.z;
        SetText();
    }

    public void SetShipDirection()
    {
        shipObject.transform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f));
    }


    private enum fakeTypes
    {
        swap,
        mod,
        zero,
        wrongVect,
        generate
    }

    public void CreateVectorsList()
    {
        shipParent = this.gameObject;

        vectorList = new List<int[]>();
        switch (GCP02.Difficulty)
        {
            case 1:
                vectorList = GenerateVectors(level1vectors);
                break;
            case 2:
                vectorList = GenerateVectors(level2vectors);
                break;
            case 3:
                vectorList = GenerateVectors(level3vectors);
                break;
        }
        ansVector = vectorList[Random.Range(0, vectorList.Count)];
    }

    public void CreateMatrixList()
    {
        matrixList = new List<int[][]>();
        ansMatrix = GenerateAnsMatrix(ansVector);
        AdjustShipPos();
        matrixList.Add(ansMatrix);

        int picker;
        int antiLock = 0;
        fakeTypes choice;
        List<fakeTypes> picked = new List<fakeTypes>();
        //need to create 3 more fakey matrixes
        for (int i = 0; i < 3; i++)
        {
            picker = Random.Range(0, System.Enum.GetNames(typeof(fakeTypes)).Length);
            choice = (fakeTypes)System.Enum.ToObject(typeof(fakeTypes), picker);
            if (!picked.Contains(choice))
            {
                picked.Add(choice);
                antiLock = 0;
                matrixList.Add(GenerateFakeMatricies(choice));
            }
            else
            {
                antiLock++;
                if (antiLock > 5)
                {
                    picked.Remove(fakeTypes.mod);
                    picked.Remove(fakeTypes.wrongVect);
                }

                i--;
            }
        }
    }


    /// <summary>
    /// Find the solution for [ [a, c], [b, d] ] given [alpha, beta] and [x, y]
    /// where [alpha,beta] = alpha     (this is typically the ansVector)
    /// and [x,y] = shipPosition
    /// </summary>
    private int[][] GenerateAnsMatrix(int[] alpha)
    {
        int[][] matrix = new int[2][];
        // for reference:
        // 
        //NOTE do not get Y and Z mixed up..... try not to get confused with Y and Z

        //First, find the GCD of the ship position
        gcd = GCD((int)shipParent.transform.position.x, (int)shipParent.transform.position.z);

        int r1 = Random.Range(0, (int)(shipParent.transform.position.x / gcd));
        int r2 = Random.Range(0, (int)(shipParent.transform.position.z / gcd));

        int a = gcd * r1;
        int c = gcd * r2;

        int b = 0;
        int d = 0;
        //beta * b = x - alpha * a OR b = ( x - alpha * a ) / beta
        //Edge Case: beta = 0 | alpha[2] = 0
        if(alpha[2] == 0)
        {
            a = Mathf.RoundToInt((int)(shipParent.transform.position.x / alpha[0]));
            c = Mathf.RoundToInt((int)(shipParent.transform.position.z / alpha[0]));
            b = gcd * r1;
            d = gcd * r2;
        }
        else
        {
            b = Mathf.RoundToInt(((int)shipParent.transform.position.x - (alpha[0] * a)) / alpha[2]);
            d = Mathf.RoundToInt(((int)shipParent.transform.position.z - (alpha[0] * c)) / alpha[2]);
        }


        int[] temp;       
        temp = new int[2] { a, c };
        matrix[0] = temp;
        temp = new int[2] { b, d };
        matrix[1] = temp;

        return matrix;
    }

    private void AdjustShipPos()
    {
        //calculate new pos if it changed
        int x = (ansVector[0] * ansMatrix[0][0]) + (ansVector[2] * ansMatrix[1][0]);
        int z = (ansVector[0] * ansMatrix[0][1]) + (ansVector[2] * ansMatrix[1][1]);

        //alter ship position if changed to match output of matrix multiplication
        SetPosition(new Vector3(x, 0, z));
    }

    private int[][] GenerateFakeMatricies(fakeTypes fake)
    {
        int[][] fakeMatrix = new int[2][];
        fakeMatrix[0] = new int[2];
        fakeMatrix[1] = new int[2];
        int coinflip;
        switch (fake)
        {
            case fakeTypes.swap: //swap vectors (only effective if alpha != beta )
                if(ansVector[0] != ansVector[2])
                {
                    fakeMatrix[0] = ansMatrix[1];
                    fakeMatrix[1] = ansMatrix[0];
                }

                break;

            case fakeTypes.mod: //make a tweaked copy of the ansMatrix
                    //we need some value to add/remove from the matrix values
                    //some that is believable but not obvious
                fakeMatrix = ansMatrix;
                int quad;

                for(int j = 0; j < 4; j++)
                {
                    quad = Random.Range(0, 4);
                    coinflip = Random.Range(0, 1);

                    switch (quad)
                    {
                        case 0:
                            if (coinflip > 0.5f)
                                fakeMatrix[0][0] += gcd;
                            else
                                fakeMatrix[0][0] -= gcd;

                            break;
                        case 1:
                            if (coinflip > 0.5f)
                                fakeMatrix[1][0] += gcd;
                            else
                                fakeMatrix[1][0] -= gcd;
                            break;
                        case 2:
                            if (coinflip > 0.5f)
                                fakeMatrix[0][1] += gcd;
                            else
                                fakeMatrix[0][1] -= gcd;
                            break;
                        case 3:
                            if (coinflip > 0.5f)
                                fakeMatrix[1][1] += gcd;
                            else
                                fakeMatrix[1][1] -= gcd;
                            break;
                    }


                }

                break;

            case fakeTypes.zero: //generate with zeros

                coinflip = Random.Range(0, 1);
                if (coinflip > 0.5f)
                {
                    fakeMatrix[0][0] = 0;
                    fakeMatrix[1][0] = (int)shipParent.transform.position.x;
                }
                else
                {
                    fakeMatrix[0][0] = (int)shipParent.transform.position.x;
                    fakeMatrix[1][0] = 0;
                }

                coinflip = Random.Range(0, 1);
                if (coinflip > 0.5f)
                {
                    fakeMatrix[0][1] = 0;
                    fakeMatrix[1][1] = (int)shipParent.transform.position.z;
                }
                else
                {
                    fakeMatrix[0][1] = (int)shipParent.transform.position.z;
                    fakeMatrix[1][1] = 0;
                }

                break;

            case fakeTypes.wrongVect: //generate for vector not on vectorList
                int[] fakeVect = new int[2];
                int rand;
                switch (GCP02.Difficulty)
                {
                    case 1:
                        do
                        {
                            rand = Random.Range(0, level1vectors.Count);
                            fakeVect = level1vectors[rand];
                        } while (vectorList.Contains(fakeVect));

                        break;
                    case 2:
                        do
                        {
                            rand = Random.Range(0, level2vectors.Count);
                            fakeVect = level2vectors[rand];
                        } while (vectorList.Contains(fakeVect));
                                
                        break;
                    case 3:
                        do
                        {
                            rand = Random.Range(0, level3vectors.Count);
                            fakeVect = level3vectors[rand];
                        } while (vectorList.Contains(fakeVect));
                                
                        break;
                }

                fakeMatrix = GenerateAnsMatrix(fakeVect);

                break;

            case fakeTypes.generate: //just generate some random goobly from the ship position

                fakeMatrix[0][0] = Random.Range(1, (int)shipParent.transform.position.x);
                fakeMatrix[1][0] = Random.Range(1, (int)shipParent.transform.position.x);

                fakeMatrix[0][1] = Random.Range(1, (int)shipParent.transform.position.z);
                fakeMatrix[1][1] = Random.Range(1, (int)shipParent.transform.position.z);

                break;

        }

        return fakeMatrix;

    }


    /// <summary>
    /// Dwindles down a reference list of possible vector choices to just 4 options
    /// and returns those four as a list
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    private List<int[]> GenerateVectors(in List<int[]> reference)
    {
        //Copy the reference list without modifiying the reference
        List<int[]> temp = new List<int[]>();
        foreach (int[] arry in reference)
            temp.Add(arry);

        int rando;
        do
        {
            rando = Random.Range(0, temp.Count);
            temp.RemoveAt(rando);

        } while (temp.Count >= 5);

        return temp;
    }

    private int GCD(int a, int b)
    {
        if (b == 0)
        {
            return a;
        }

        int r = a % b;

        return GCD(b, r);
    }

    public void answerShuffle()
    {
        //Shuffle Cards
        int count = vectorList.Count;
        int last = count - 1;
        for (int i = 0; i < last; i++)
        {
            int r = Random.Range(i, count);
            int[] temp = vectorList[i];
            vectorList[i] = vectorList[r];
            vectorList[r] = temp;
        }

        count = matrixList.Count;
        last = count - 1;
        for (int i = 0; i < last; i++)
        {
            int r = Random.Range(i, count);
            int[][] temp = matrixList[i];
            matrixList[i] = matrixList[r];
            matrixList[r] = temp;
        }
    }


    private void SetText()
    {
        if (GCP02.Difficulty < 3)
        {
            vectText.text = shipParent.transform.position.x.ToString() + "\n" + shipParent.transform.position.z.ToString();
            vectTextTop.text = shipParent.transform.position.x.ToString() + "\n" + shipParent.transform.position.z.ToString();
        }
        else
        {
            vectText.text = shipParent.transform.position.x.ToString() + "\n" + shipParent.transform.position.z.ToString() + "\n" + shipParent.transform.position.y.ToString();
            vectTextTop.text = shipParent.transform.position.x.ToString() + "\n" + shipParent.transform.position.z.ToString() + "\n" + shipParent.transform.position.y.ToString();
        }
    }
}
