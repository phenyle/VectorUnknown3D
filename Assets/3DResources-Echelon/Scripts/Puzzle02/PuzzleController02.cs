using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class PuzzleController02 : MonoBehaviour
{
    private GameControllerPuzzle02 GCP02;

    [Header("--------Ship Parts--------")]
    [HideInInspector]
    public GameObject shipParent;
    public GameObject shipObject;
    public TextMeshPro vectText;
    public TextMeshPro vectTextTop;
    public LineRenderer shipLine;
    public Material lineMatDef;
    public Material lineMatActive;
    public int[] shipPos;

    [Header("--------FX Parts--------")]
    public GameObject explosion;
    public GameObject fires;
    public Animation anime;
    public Animator anim;

    [Header("--------Settings--------")]
    private float minDistApart = 10;
    public int xMin = -75;
    public int xMax = 75;
    public int zMin = 20;
    public int zMax = 100;
    public int rValue = 10;
    private int attempt = 1;
    public int id;

    [Header("--------Scoring--------")]
    public StarBar starBar;
    public FaceObject scoreFace;


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

    [Header("--------Debug--------")]
    public string debugAnswer = "";
    public string debugAnsVector = "";
    public string debugAnsMatrix = "";
    public string debugVectrList = "";
    public string debugMatrxList = "";

    [HideInInspector]
    [System.Serializable]
    public struct obsData
    {
        public int obsID;
        public float obsTime;
        public int Ideal;
        public int steps;
        public float score;
        public int[] answer;
        public bool userUniqueSolution;

        public int[] e_vect;
        public int[][] e_matrix;

        public int[] i_vect;
        public int[][] i_matrix;
    }


    [HideInInspector]
    public obsData puzzleData;
    [HideInInspector]
    public string attemptsList;
    [HideInInspector]
    public int attemptNum;



    public void Awake()
    {
        vectTextTop.gameObject.SetActive(false);
        vectText.gameObject.SetActive(false);
        shipParent = this.gameObject;
        shipObject = shipParent.transform.Find("shipBody").gameObject;
        GCP02 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle02>();
        //ReScaleLevel(GCP02.Difficulty);
    }

    private void ReScaleLevel(int diffLvl)
    {
        minDistApart = Mathf.CeilToInt(10 * (diffLvl / 3.0f));
        xMin = Mathf.CeilToInt(xMin * (diffLvl / 3.0f));
        xMax = Mathf.FloorToInt(xMax * (diffLvl / 3.0f));
        zMin = Mathf.CeilToInt(zMin * (diffLvl / 3.0f));
        zMax = Mathf.FloorToInt(zMax * (diffLvl / 3.0f));
        rValue = Mathf.RoundToInt(rValue * (GCP02.Difficulty / 3.0f));
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("IdleFloat");
        anim.playbackTime = Random.Range(anim.recorderStartTime, anim.recorderStopTime);
 //       anim.playbackTime = Random.Range(0.0f, anim
        //anime = GetComponent<Animation>();
        //anime.Play("Float");
        //anime["Float"].time = Random.Range(0.0f, anime["Float"].length);

        shipPos = new int[3];
        puzzleData = new obsData();
        puzzleData.obsTime = 0.0f;
        puzzleData.Ideal = 3 + GCP02.Difficulty;
        attemptNum = 0;
        attemptsList = "";
        SetShipDirection();
    }

    public void StopAnimation()
    {
        scoreFace.transform.position = new Vector3(scoreFace.transform.position.x, 2, scoreFace.transform.position.z);
 //       animator.StopPlayback();
        anime.Stop();
        shipObject.gameObject.SetActive(false);
    }

    public void ToggleTopText(bool val)
    {
        vectTextTop.gameObject.SetActive(val);
    }

    public void ToggleFrontText(bool val)
    {
        vectText.gameObject.SetActive(val);
    }

    public void SetID(int idnum)
    {
        id = idnum;
    }


    public void SetPosition(Vector3 pos)
    {
        foreach (int[][] matrix in matrixList)
        {
            debugMatrxList += "{{ " + matrix[0][0].ToString() + ", " + matrix[0][1].ToString() + " }, { " + matrix[1][0].ToString() + ", " + matrix[1][1].ToString() + " }}\n";
        }
        debugAnsMatrix = "{{ " + ansMatrix[0][0].ToString() + ", " + ansMatrix[0][1].ToString() + " }, { " + ansMatrix[1][0].ToString() + ", " + ansMatrix[1][1].ToString() + " }}";
        debugAnsVector = "{ " + ansVector[0].ToString() + ", " + ansVector[1].ToString() + " }";


        shipParent.transform.position = pos;
        shipPos = new int[3];
        shipPos[0] = (int)pos.x;
        shipPos[2] = (int)pos.z;
        SetText();
        SetLine();
        SetShipDirection();
    }


    public void SetShipDirection()
    {
        Vector3 temp = -shipParent.transform.position.normalized;

        vectText.transform.position = shipParent.transform.position + temp * 4 + Vector3.up * 6;
        vectText.transform.localScale = Vector3.one;

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
        debugAnsVector = "{ " + ansVector[0].ToString() + ", " + ansVector[2].ToString() + " }";

        foreach (int[] vects in vectorList)
        {
            debugVectrList += "{ " + vects[0].ToString() + ", " + vects[2].ToString() + " }\n";

        }
    }

    public void CreateMatrixList()
    {
        matrixList = new List<int[][]>();
        ansMatrix = GenerateAnsMatrix(ansVector);
        AdjustShipPos();

        debugAnsMatrix = "{ { " + ansMatrix[0][0].ToString() + ", " + ansMatrix[0][1].ToString() + " }, { " + ansMatrix[1][0].ToString() + ", " + ansMatrix[1][1].ToString() + " } }";

        matrixList.Add(ansMatrix);

        int picker;
        int antiLock = 0;
        fakeTypes choice;
        List<fakeTypes> picked = new List<fakeTypes>();
        //need to create 3 more fakey matrixes

        do
        {
            picker = Random.Range(0, System.Enum.GetNames(typeof(fakeTypes)).Length);
            choice = (fakeTypes)System.Enum.ToObject(typeof(fakeTypes), picker);
            if (!picked.Contains(choice))
            {
                if (choice == fakeTypes.swap && ansVector[0] == ansVector[2])
                {
                    picked.Add(choice);
                    antiLock++;
                }
                else
                {
                    picked.Add(choice);
                    antiLock = 0;
                    int[][] temp = GenerateFakeMatricies(choice);
                    if (!matrixList.Contains(temp)) 
                        matrixList.Add(temp);
                    else
                    {
                        picked.Remove(choice);
                        antiLock++;
                    }
                }

            }
            else
            {
                antiLock++;
                if (antiLock > 20)
                {
                    picked.Remove(fakeTypes.mod);
                    picked.Remove(fakeTypes.wrongVect);
                }

            }
        } while (matrixList.Count < 4);


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
        int x = (ansVector[0] * ansMatrix[0][0]) + (ansVector[1] * ansMatrix[1][0]);
        int z = (ansVector[0] * ansMatrix[0][1]) + (ansVector[1] * ansMatrix[1][1]);

        debugAnswer = "{" + x.ToString() + ", " + z.ToString() + "}";
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
                int quad;
                
                for(int i = 0; i < ansMatrix.Length; i++)
                {
                    for (int j = 0; j < ansMatrix[i].Length; j++)
                        fakeMatrix[i][j] = ansMatrix[i][j];
                }


                for(int j = 0; j < 8; j++)
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
    /// 
    /// Done as a Remove until 4 left because I'm too lazy to write something about
    /// randomly adding 4 and then checking as I add them that I don't already have it added
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

        } while (temp.Count > 4);

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

        foreach (int[][] matrix in matrixList)
        {
            debugMatrxList += "{ { " + matrix[0][0].ToString() + ", " + matrix[0][1].ToString() + " }, { " + matrix[1][0].ToString() + ", " + matrix[1][1].ToString() + " } }\n";

        }

    }

    private void SetLine()
    {
        shipLine.positionCount = 2;
        shipLine.SetPosition(0, Vector3.zero + Vector3.up * 11);
        shipLine.SetPosition(1, new Vector3(shipParent.transform.position.x, 0, shipParent.transform.position.z) + Vector3.up * 11);
        shipLine.material = lineMatDef;
        shipLine.gameObject.SetActive(false);
    }

    private void SetText()
    {
        vectText.text = shipParent.transform.position.x.ToString() + "\n" + shipParent.transform.position.z.ToString();
        vectTextTop.text = shipParent.transform.position.x.ToString() + "\n" + shipParent.transform.position.z.ToString();
    }

    public void CalculateScore(float maxTime)
    {
        float baseScore = 0.0f;
        //if it was solved under the ideal steps, give a small bonus (20%) to base score
        if (puzzleData.steps >= puzzleData.Ideal)
            baseScore = (float)puzzleData.Ideal / (float)puzzleData.steps;
        else
            baseScore = 1.2f;

        float timeScore = -(Mathf.Pow(puzzleData.obsTime, 2) / Mathf.Pow(maxTime, 2)) + 1;
        if (timeScore > 1.0f) timeScore = 1.0f;
        else if (timeScore < 0.0f) timeScore = 0.0f;

        puzzleData.score = baseScore * timeScore;
    }



    public string CreateAttemptCSV(int[][] a_matrix, int[] a_vect)
    {
        string temp = "";
        attemptNum++;

        temp += attemptNum.ToString() + ", "; //attempt number
        temp += puzzleData.obsTime.ToString("0.00") + ", "; //time attempted at

        temp += "{ { " + a_matrix[0][0].ToString() + ", " + a_matrix[0][1].ToString() + " }, { " + a_matrix[1][0].ToString() + ", " + a_matrix[1][1].ToString() + " } }, ";
        temp += "{ " + a_vect[0].ToString() + ", " + a_vect[1].ToString() + " }\n";

        attemptsList += temp;
        return temp;
    }

    public void AltCreateMatrixList()
    {
        ansMatrix = matrixList[Random.Range(0, 4)];
    }


    private int[][] AltGenerateAnswerMatrix()
    {
        float[] pos = new float[2];
        int[][] matrix = new int[2][] { new int[2], new int[2] };
        int[] vect = ansVector;

        float thetaMax = Mathf.Deg2Rad * 120.0f;
        float thetaMin = Mathf.Deg2Rad * 60.0f;
        float theta;

        Vector2 x1 = new Vector2(1, 0);
        theta = Random.Range(0.0f, 2*Mathf.PI);

        float s = Mathf.Cos(theta) * x1.x - Mathf.Sin(theta) * x1.y;
        float t = Mathf.Sin(theta) * x1.x + Mathf.Cos(theta) * x1.y;
        int randScal = Random.Range(Mathf.RoundToInt((float)rValue * 0.66f), rValue);
        Vector2 k = new Vector2(s, t) * randScal;
        k.x = Mathf.RoundToInt(k.x);
        k.y = Mathf.RoundToInt(k.y);

        x1 = k;

        //Rotating the second Vector
        theta = Random.Range(thetaMin, thetaMax);
        if (Random.Range(0.0f, 1.0f) < 0.5f)
            theta = (theta * -1);

        s = Mathf.Cos(theta) * x1.normalized.x - Mathf.Sin(theta) * x1.normalized.y;
        t = Mathf.Sin(theta) * x1.normalized.x + Mathf.Cos(theta) * x1.normalized.y;

        k = new Vector2(s, t);

        //scoring phase
        float bestScore = 99;
        float bestScal = 0;
        float magV1 = x1.magnitude;
        //We're only going to look at x2 that at least half the mag x1 but not larger than 1.5 the mag of x1 
        for (int i = Mathf.RoundToInt(magV1 * 0.5f); i < Mathf.RoundToInt(magV1 * 1.5f); i++)
        {
            Vector2 tempRound = new Vector2(Mathf.Round(k.x * i), Mathf.Round(k.y * i));
            float score = Mathf.Abs((tempRound - (k * i)).magnitude);
            if(score < bestScore)
            {
                bestScore = score;
                bestScal = i;
            }
        }

        Vector2 x2 = new Vector2(Mathf.Round(k.x * bestScal), Mathf.Round(k.y * bestScal));

        matrix[0][0] = (int)x1.x;
        matrix[0][1] = (int)x1.y;
        matrix[1][0] = (int)x2.x;
        matrix[1][1] = (int)x2.y;

        return matrix;
    }

    public void AltCreateVectorsList()
    {
        vectorList = new List<int[]>();
        //call function to get one correct vector
        ansVector = AltGenerateAnswerVector2();
        vectorList.Add(ansVector);

        //make modified copies of the correct vector as redherrings
        for (int i = 0; i < 3; i++)
        {
            int[] temp = new int[] { (int)Random.Range(-10, 10) + ansVector[0], (int)Random.Range(-10, 10) + ansVector[1] };
            if (vectorList.Contains(temp))
                i--;
            else
                vectorList.Add(temp);
        }

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

        AdjustShipPos();
    }


    public int[] AltGenerateAnswerVector()
    {
        //Range Variables
        int[] alphaRanges = new int[2];
        int[] betaRanges = new int[2];
        int alpha, beta;

        //coinflip to start from alpha or beta
        if (Random.Range(0.0f, 1.0f) < 0.5f)
        {
            alphaRanges = FindRanges(0);
            alpha = Random.Range(alphaRanges[0], alphaRanges[1]);
            betaRanges = RedefineRanges(1, alpha);
            beta = Random.Range(alphaRanges[0], alphaRanges[1]);
        }
        else
        {
            betaRanges = FindRanges(1);
            beta = Random.Range(betaRanges[0], betaRanges[1]);
            alphaRanges = RedefineRanges(0, beta);
            alpha = Random.Range(alphaRanges[0], alphaRanges[1]);
        }

        return new int[2] { alpha, beta };
    }

    public int[] AltGenerateAnswerVector2()
    {


        //Range Variables
        int[] alphaRanges = new int[2];
        int[] betaRanges = new int[2];
        int alpha, beta;

        Matrix<double> matrix = DenseMatrix.OfArray(new double[,]
        {
            {ansMatrix[0][0], ansMatrix[1][0] },
            {ansMatrix[0][1], ansMatrix[1][1] }
        });

//        Debug.Log("int Array Matrix:\n" + ansMatrix[0][0].ToString() + ", " + ansMatrix[1][0].ToString() + "\n" + ansMatrix[0][1].ToString() + ", " + ansMatrix[1][1].ToString());

        Debug.Log("ShipID# " + id.ToString() + "\tNumerics Matrix:\n" + matrix.ToString());

        Matrix<double> invertedMatrix = matrix.Inverse();

 //       Debug.Log("Inverted Matrix:\n" + invertedMatrix.ToString());

        // Calc Mins
        Matrix<double> minValues = DenseMatrix.OfArray(new double[,]
        {
            { xMin },
            { zMin }
        });
        Matrix<double> minRange = (invertedMatrix * minValues).PointwiseCeiling();

        //Calc Maxs
        Matrix<double> maxValues = DenseMatrix.OfArray(new double[,]
        {
            { xMax },
            { zMax }
        });
        Matrix<double> maxRange = (invertedMatrix * maxValues).PointwiseFloor();

        Matrix<double> scalMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 0 },
                { 0 }
            });

        Vector3 resultPos;

        alpha = Random.Range((int)minRange[0, 0], (int)maxRange[0, 0]);

        beta = Random.Range((int)minRange[1, 0], (int)maxRange[1, 0]);

        scalMatrix[0, 0] = alpha;
        scalMatrix[1, 0] = beta;

        Matrix<double> resultMatrix = (matrix * scalMatrix);
        resultPos = new Vector3((float)resultMatrix[0, 0], 0, (float)resultMatrix[1, 0]);

        Debug.Log("ShipID# " + id.ToString() + "\tShip Pos Result\n" + resultPos.ToString());

        if(!ShipInBounds( resultPos ) || !ShipsSpacedOut( resultPos ))
        {
            //matrixList.Clear();
            //matrixList.TrimExcess();
            //ansMatrix = new int[2][];
            vectorList.Clear();
            vectorList.TrimExcess();
            ansVector = new int[2];
            attempt++;
            Debug.LogWarning("ShipID# " + id.ToString() + "\tCleared Lists\nMatrixCount: " + matrixList.Count().ToString() + "\tVectorCount: " + vectorList.Count().ToString());
            //AltCreateMatrixList();
            return AltGenerateAnswerVector2();
        }

        return new int[2] { alpha, beta };
    }

    private bool ShipInBounds(Vector3 pos)
    {
        if (pos.x < xMin || pos.x > xMax)
        {
            Debug.LogWarning("ShipID# " + id.ToString() + "\tX out of bounds at: " + pos.x.ToString());
            return false;
        }
        else if (pos.z < zMin || pos.z > zMax)
        {
            Debug.LogWarning("ShipID# " + id.ToString() + "\tZ out of bounds at: " + pos.z.ToString());
            return false;
        }
        else
            return true;
    }

    private bool ShipsSpacedOut(Vector3 pos)
    {
        float distApart;
        //Check that this location is not too close to previous ships
        for (int i = 0; i < GCP02.GetShipList().Count; i++)
        {
            if(this.gameObject != GCP02.GetShipList()[i].gameObject)
            {
                distApart = (pos - GCP02.GetShipList()[i].transform.position).magnitude;
                if (distApart < minDistApart)
                {
                    Debug.LogWarning("ShipID# " + id.ToString() + "\tShips too close, MinDist: " + minDistApart.ToString() + "\nThis Pos: " + pos.ToString() + "\tOtherShip: " + GCP02.GetShipList()[i].transform.position.ToString());

                    return false;
                }
            }
        }

        return true;
    }

    private int[] FindRanges(int dimen)
    {
        int xMin = -75;
        int xMax = 75;
        int zMin = 20;
        int zMax = 100;

        int[][] intRanges = new int[2][] { new int[2], new int[2] }; //[0][] Xranges, [1][] Zranges;  [][0] Max, [][1] Min
        int[] retRanges = new int[2];
        if (ansMatrix[dimen][0] != 0)
        {
            intRanges[0][0] = Mathf.FloorToInt((float)xMax / (float)ansMatrix[dimen][0]);
            intRanges[0][1] = Mathf.CeilToInt((float)xMin / (float)ansMatrix[dimen][0]);
        }
        else
        {
            intRanges[0][0] = 0;
            intRanges[0][1] = 0;
        }
        if (ansMatrix[dimen][1] != 0)
        {
            intRanges[1][0] = Mathf.FloorToInt((float)zMax / (float)ansMatrix[dimen][1]);
            intRanges[1][1] = Mathf.CeilToInt((float)zMin / (float)ansMatrix[dimen][1]);
        }
        else
        {
            intRanges[1][0] = 0;
            intRanges[1][1] = 0;
        }

        retRanges[0] = intRanges[0][0] < intRanges[1][0] ? intRanges[0][0] : intRanges[1][0];
        retRanges[1] = intRanges[0][1] < intRanges[1][1] ? intRanges[0][1] : intRanges[1][1];

        return retRanges;
    }


    private int[] RedefineRanges(int dimen, int scalOther)
    {
        int xMin = -75;
        int xMax = 75;
        int zMin = 20;
        int zMax = 100;
        int dimenOther;

        if (dimen == 0)
            dimenOther = 1;
        else
            dimenOther = 0;


        int[][] modRanges = new int[2][] { new int[2], new int[2] }; //[0][] Xranges, [1][] Zranges;  [][0] Max, [][1] Min
        int[] retRanges = new int[2];
        if (ansMatrix[dimen][0] != 0)
        {
            modRanges[0][0] = Mathf.FloorToInt(((float)xMax - ((float)scalOther * (float)ansMatrix[dimenOther][0])) / (float)ansMatrix[dimen][0]);
            modRanges[0][1] = Mathf.CeilToInt(((float)xMin - ((float)scalOther * (float)ansMatrix[dimenOther][0])) / (float)ansMatrix[dimen][0]);
        }
        else
        {
            modRanges[0][0] = 0;
            modRanges[0][1] = 0;
        }
        if (ansMatrix[dimen][1] != 0)
        {
            modRanges[1][0] = Mathf.FloorToInt(((float)zMax - ((float)scalOther * (float)ansMatrix[dimenOther][1])) / (float)ansMatrix[dimen][1]);
            modRanges[1][1] = Mathf.CeilToInt(((float)zMin - ((float)scalOther * (float)ansMatrix[dimenOther][1])) / (float)ansMatrix[dimen][1]);
        }
        else
        {
            modRanges[1][0] = 0;
            modRanges[1][1] = 0;
        }

        retRanges[0] = modRanges[0][0] < modRanges[1][0] ? modRanges[0][0] : modRanges[1][0];
        retRanges[1] = modRanges[0][1] < modRanges[1][1] ? modRanges[0][1] : modRanges[1][1];

        return retRanges;
    }

    public void SetGameController(GameControllerPuzzle02 controller)
    {
        GCP02 = controller;
    }
}   
