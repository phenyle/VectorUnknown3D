using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

public enum Direction
{
    X,
    Y,
    Z,
    XY,
    XYZ
};

public class Puzzle04Controller : MonoBehaviour
{
    [Header("---Object Assignment---")]
    public GameObject player;
    //    public GameObject portal;
    public GameObject startPoint;
    public GameObject goal;
    public GameObject goalPoint;
    public GameObject landPoint;
    public GameObject reset;
    public GameObject finish;
    public int puzzleID;
    public bool isActive;

    [Header("---View Controls---")]
    public GameObject mainCamera;
    public GameObject puzzleCamera;
    public GameObject visVectCamera;
    public GameObject cameraTarget;
    public GameObject CameraStartPosition;
    private Vector3 prevCameraPosition;

    [Header("---Puzzle Vectors---")]
    public Vector3 InitFinalVector; //this is the vector input from the Unity Inspector, it's the basis for the FinalAnswer
    public bool isDynamic; //this bool tells the system to slightly randomize the InitFinalVector before setting it as the startingAnswer
    [Range(1, 10)]
    public int FinalDynamicRange;
    public Direction LimitAxis = new Direction();
    private Vector3 DynamicVector;

    [Header("---Answer Card Vectors---")]
    [Range(1, 10)]
    public int MaxCardScalar;
    //   [Range(1, 10)]
    //   public int MaxCardMagnitudes;

    //Object Tools
    [Header("Puzzle Tools")]
    public PortalTrigger portal;
    public GrappleCode grappleKit;
    public GapTriggersController gapTriggers;
    private bool inPortal = false;
    private VisualVector VV01;
    public ReleaseGrappleTrigger releaseTrigger;
    private ObjectGlide cameraGlide;
    public float grappleArcHeight = 0.0f;
    private CameraRotate cameraRotate;
    public UnityEvent answerEvents;

    [Header("---Conversation Select---")]
    [Range(0, 9)]
    [Tooltip("Sets the dialog number used.\n0 val means no dialog\n1-9 selects that specific dialog")]
    public int convoTriggerNumber;


    //Answer Cards
    private Vector3 startingAnswer; //this is the starting vector after being slightly randomized (when applicable) used for answer card calculation
    private Vector3 finalAnswerVector; //this is the Final answer vector that is tailored to match what the pair of answer cards can reproduce
    private List<Vector3> cardVectors;

    //Player Answers
    private int scalar1;
    private int scalar2;
    private Vector3 card1;
    private Vector3 card2;
    private Vector3 playerAnswer;
    private Vector3 prevPlayerAnswer = Vector3.zero;
    private bool ans1Entered; //condition flag that a card is slotted in position1 in the UI
    private bool ans2Entered; //condition flag that a card is slotted in position2 in the UI
    private bool correct;

    private GameControllerPuzzle04 GCP04;

    //MISC
    private bool chgScal1 = false;
    private bool chgScal2 = false;
    private float time1 = 0;
    private float time2 = 0;
    private float waitTimeToEnterAttempt = 1.0f;
    private int attempts = 0;
    private int prevScal1;
    private int prevScal2;
    private Vector3 prevVect1;
    private Vector3 prevVect2;

    //Database Records
    private float obstacleTime;

    [HideInInspector]
    [System.Serializable]
    public struct obsData
    {
        public int obsID;
        public float obsTime;
        public Vector3 answer;
        public bool userUniqueSolution;

        public int e_Scal1;
        public int e_Scal2;
        public Vector3 e_Vect1;
        public Vector3 e_Vect2;

        public int i_Scal1;
        public int i_Scal2;
        public Vector3 i_Vect1;
        public Vector3 i_Vect2;

        

        ////Puzzle generated and Expected values (e)
        //public struct expectedData {
        //    public int e_Scal1;
        //    public int e_Scal2;
        //    public Vector3 e_Vect1;
        //    public Vector3 e_Vect2;
        //}
        //public expectedData expected;

        ////Player generated and Input values (i)
        //public struct inputData {
        //    public int i_Scal1;
        //    public int i_Scal2;
        //    public Vector3 i_Vect1;
        //    public Vector3 i_Vect2;
        //}
        //public inputData input;

        public struct attemptsData
        {
            public int attemptNum;
            public float time;
            public int a_Scal1;
            public Vector3 a_Vect1;
            public int a_Scal2;
            public Vector3 a_Vect2;
            public Vector3 a_Ans;
        }
    }

    [HideInInspector]
    public obsData puzzleData;
    [HideInInspector]
    public string attemptsList;

    // Awake is called when the Instance is Initialized
    void Awake()
    {     
        GCP04 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerPuzzle04>();
        VV01 = this.GetComponent<VisualVector>();
        cameraRotate = this.GetComponent<CameraRotate>();
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        puzzleCamera = GameObject.FindGameObjectWithTag("PuzzleCamera");
        cameraGlide = GameObject.FindGameObjectWithTag("cameraTools").GetComponent<ObjectGlide>();
        cameraRotate = GameObject.FindGameObjectWithTag("cameraTools").GetComponent<CameraRotate>();
        visVectCamera = GameObject.Find("VisVectorCamera");
        //have to flicker visvect clear flags on start, because of a weird bug.
        //it should be set to depth, but on Level 3 it only works if it gets reset to depth after
        //starting the level.  So, that's what this does.
        visVectCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
        visVectCamera.GetComponent<Camera>().enabled = false;


        grappleKit = GameObject.Find("GrappleKit").GetComponent<GrappleCode>();
        
        gapTriggers.SetPuzzleController(this);
        portal.SetPuzzleController(this);
        inPortal = false;
        releaseTrigger.SetPuzzleController(this);
        VV01.SetPuzzleController(this);

        cardVectors = new List<Vector3>();

        scalar1 = 0;
        scalar2 = 0;

        card1 = new Vector3(0f, 0f, 0f);
        card2 = new Vector3(0f, 0f, 0f);

        //If vector is dynamic, generate an additional vector to add
        //to starting answer
        if (isDynamic)
        {
            DynamicVector = DynamicGenerator();
        }
        else
            DynamicVector = new Vector3(0f, 0f, 0f);


        //Final Answer is created after generating the cards       
        startingAnswer = InitFinalVector + DynamicVector;


        //Move camera target inbetween the starting position and the goal
        cameraTarget.transform.position += (goalPoint.transform.position - startPoint.transform.position) / 2;

        correct = false;

        obstacleTime = 0.0f;
        puzzleData = new obsData();
        attemptsList = "";
        //puzzleData.expected = new obsData.expectedData();
        //puzzleData.input = new obsData.inputData();
        puzzleData.obsID = puzzleID;

        //       intAnswerGenerator();
        //AnswerGenerators();
        AnswerCards();

        if(GCP04.Difficulty == 1)
            finalAnswerVector = SetFinalAnswer(0, Vector3.zero, 1, startingAnswer);

    }

    // Update is called once per frame
    void Update()
    {
        if (inPortal)
        {
            //if (cameraGlide.isAtDestination())
            //    cameraRotate.enabled = true;




            if (!DialogueManager.isInDialogue && !GameRoot.isPause)
                obstacleTime += Time.deltaTime;


            isActive = true;

            scalar1 = GCP04.P04W.getScalar1Value();
            scalar2 = GCP04.P04W.getScalar2Value();

            if (GCP04.P04W.getAnswer1() != null)
            {
                card1 = GCP04.P04W.getAnswer1().GetComponent<CardDrag>().getCardVector();
                ans1Entered = true;
            }
            else
            {
                card1 = Vector3.zero;
                ans1Entered = false;
            }

            if (GCP04.P04W.getAnswer2() != null)
            {
                card2 = GCP04.P04W.getAnswer2().GetComponent<CardDrag>().getCardVector();
                ans2Entered = true;
            }
            else
            {
                card2 = Vector3.zero;
                ans2Entered = false;
            }


            //prevanswer
            playerAnswer = scalar1 * card1 + scalar2 * card2;

            grappleKit.demoTarget.transform.position = VV01.vector2end.transform.position;

            if (playerAnswer != Vector3.zero && playerAnswer != prevPlayerAnswer)
            {
                grappleKit.enabled = true;
                grappleKit.DemoAnim = true;
                grappleKit.SetDemoTargetPos(VV01.vector2end.transform.position);
            }

            GCP04.P04W.setFinalAnswerDisplay(this);


            //Keyboard Scalar Controls
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (scalar1 > -10)
                    GCP04.P04W.scalar1.value--;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (scalar1 < 10)
                    GCP04.P04W.scalar1.value++;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (scalar2 > -10)
                    GCP04.P04W.scalar2.value--;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (scalar2 < 10)
                    GCP04.P04W.scalar2.value++;
            }


            EnterAttempts();

            prevScal1 = scalar1;
            prevScal2 = scalar2;
            prevVect1 = card1;
            prevVect2 = card2;
            prevPlayerAnswer = prevScal1 * prevVect1 + prevScal2 * prevVect2;

            if (Input.GetKeyDown(KeyCode.E))
            {
                OnPortalExit();
                GameRoot.instance.LockPlayerAndCam(false);
            }

        }
        else
        {
            isActive = false;
        }

    }


    #region Answer Generation
    /// <summary>
    /// Generates a modified vector based from the MinVectorSize and MaxDynamicScalar.
    /// It first divides each vector dimension by a random float and rounds to an int
    /// then in multiplies the whole vector by a random float and rounds each dimension
    /// to an int.
    /// This returns a vector that is generally in the direction of the MinVectorSize
    /// while being varied enough that each playthrough has different results
    /// </summary>
    /// <returns></returns>
    private Vector3 DynamicGenerator()
    {
        Vector3 temp = Vector3.zero;

        switch (GCP04.Difficulty)
        {
            case 1:
                switch (LimitAxis)
                {
                    //Zero vectors are allowed here for "extra" vectors
                    case Direction.X:
                        if(InitFinalVector.x > 0)
                            temp.x += UnityEngine.Random.Range(0, FinalDynamicRange);
                        else
                            temp.x -= UnityEngine.Random.Range(0, FinalDynamicRange / 2);
                        temp.y += 0.0f;
                        temp.z += 0.0f;
                        break;

                    case Direction.Y:
                        temp.x += 0.0f;
                        if(InitFinalVector.y > 0)
                            temp.y += UnityEngine.Random.Range(0, FinalDynamicRange);
                        else
                            temp.y -= UnityEngine.Random.Range(0, FinalDynamicRange / 2);
                        temp.z += 0.0f;
                        break;
                }

                break;

            case 2:
                if (InitFinalVector.x > 0)
                    temp.x += UnityEngine.Random.Range(0, FinalDynamicRange);
                else
                    temp.x -= UnityEngine.Random.Range(0, FinalDynamicRange / 2);

                if (InitFinalVector.y > 0)
                    temp.y += UnityEngine.Random.Range(0, FinalDynamicRange);
                else
                    temp.y -= UnityEngine.Random.Range(0, FinalDynamicRange / 2);
                temp.z += 0.0f;
                break;

            case 3:
                //No zero vectors allowed
                if (InitFinalVector.x > 0)
                    temp.x += UnityEngine.Random.Range(0, FinalDynamicRange);
                else
                    temp.x -= UnityEngine.Random.Range(0, FinalDynamicRange / 2);

                if (InitFinalVector.y > 0)
                    temp.y += UnityEngine.Random.Range(0, FinalDynamicRange);
                else
                    temp.y -= UnityEngine.Random.Range(0, FinalDynamicRange / 2);

                if(InitFinalVector.z > 0)
                    temp.z += UnityEngine.Random.Range(0, FinalDynamicRange);
                else
                    temp.z -= UnityEngine.Random.Range(0, FinalDynamicRange / 2);

                break;
        }

        return temp;
    }


    private void AnswerCards()
    {
        Vector3 answer1, answer2, extra1, extra2;
        int scal1;


        if(GCP04.Difficulty == 1)
        {
            if(this.getDirection() == Direction.X)
            {
                cardVectors.Add(new Vector3(1, 0, 0));
                cardVectors.Add(new Vector3(-1, 0, 0));

                if(UnityEngine.Random.value > 0.5f)                
                    cardVectors.Add(new Vector3(2, 0, 0));
                else
                    cardVectors.Add(new Vector3(-2, 0, 0));

                if (UnityEngine.Random.value > 0.5f)
                    cardVectors.Add(new Vector3(3, 0, 0));
                else
                    cardVectors.Add(new Vector3(-3, 0, 0));

            }
            if (this.getDirection() == Direction.Y)
            {
                cardVectors.Add(new Vector3(0, 1, 0));
                cardVectors.Add(new Vector3(0, -1, 0));

                if (UnityEngine.Random.value > 0.5f)
                    cardVectors.Add(new Vector3(0, 2, 0));
                else
                    cardVectors.Add(new Vector3(0, -2, 0));

                if (UnityEngine.Random.value > 0.5f)
                    cardVectors.Add(new Vector3(0, 3, 0));
                else
                    cardVectors.Add(new Vector3(0, -3, 0));
            }
        }
        else
        {
            scal1 = UnityEngine.Random.Range(Mathf.RoundToInt(MaxCardScalar / 2), MaxCardScalar + 1);
            do { answer1 = VectorGenerator(); } while (answer1 == Vector3.zero || (answer1 * scal1) == InitFinalVector);
            cardVectors.Add(answer1);

            answer2 = VectorPairFinder(scal1, answer1);
            cardVectors.Add(answer2);

            do { extra1 = VectorGenerator(); } while (extra1 == Vector3.zero);
            cardVectors.Add(extra1);

            do { extra2 = VectorGenerator(); } while (extra2 == Vector3.zero);
            cardVectors.Add(extra2);
        }

        answerShuffle();
    }

    /// <summary>
    /// This function generates a Vector in the set of V where V is defined as:
    ///  V = { [x,y,z] | x,y,z >= -3 and x,y,z <= 3 and x=y=z != 0 }
    /// </summary>
    /// <returns>returns a Vector3 in the set of V </returns>
    private Vector3 VectorGenerator()
    {
        Vector3 temp = Vector3.zero;

        switch (GCP04.Difficulty)
        {
            case 1:
                switch (LimitAxis)
                {
                    //Zero vectors are allowed here for "extra" vectors
                    case Direction.X:
                        temp.x = UnityEngine.Random.Range(-3, 4);
                        temp.y = 0.0f;
                        temp.z = 0.0f;
                        break;

                    case Direction.Y:
                        temp.x = 0.0f;
                        temp.y = UnityEngine.Random.Range(-3, 4);
                        temp.z = 0.0f;
                        break;
                }

                break;

            case 2:
                //No zero vectors allowed
                do
                {
                    temp.x = UnityEngine.Random.Range(-3, 4);
                    temp.y = UnityEngine.Random.Range(-3, 4);
                    temp.z = 0.0f;
                } while (temp == Vector3.zero || !testUnique(temp));
                break;

            case 3:
                //No zero vectors allowed
                do
                {
                    temp.x = UnityEngine.Random.Range(-3, 4);
                    temp.y = UnityEngine.Random.Range(-3, 4);
                    temp.z = UnityEngine.Random.Range(-3, 4);
                } while (temp == Vector3.zero || !testUnique(temp));

                break;
        }

        return temp;
    }

    private Vector3 VectorPairFinder(int scal1, Vector3 answer1)
    {
        Vector3 answer2 = Vector3.zero;
        float maxDimension = 0.0f;
        int scal2 = 0;

        //first find which version of scal is closest to goal
        if (V1reversedScal(scal1, answer1))
            scal1 *= -1;

        //Set answer2 to exact displacement
        answer2 = startingAnswer - (answer1 * scal1);

        //find the largest Dimension of displacement (answer2)        
        maxDimension = Mathf.Abs(answer2.x);
        if (maxDimension < Mathf.Abs(answer2.y))
            maxDimension = Mathf.Abs(answer2.y);
        if (maxDimension < Mathf.Abs(answer2.z))
            maxDimension = Mathf.Abs(answer2.z);

        //We'll use that maxDimension to determine what the scalar should be
        for (int i = 1; i <= MaxCardScalar + 1; i++)
        {
            //if there is no suitable scalar we'll shave 1/8 off and try again
            if (i == MaxCardScalar + 1)
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
            else if ((maxDimension / i ) <= 3)
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
        if (!testUnique(answer2) || !ValidateProposedAnswer(scal1 * answer1 + scal2 * answer2))
        {
            cardVectors.Remove(answer1);
            do { answer1 = VectorGenerator(); } while (answer1 == Vector3.zero || (answer1 * scal1) == InitFinalVector);
            cardVectors.Add(answer1);

            VectorPairFinder(scal1, answer1);
        }

        //Now that we have both answer1 and answer2 created in the set of V, we need to adjust the finalGoal to match these rounded vectors
        finalAnswerVector = SetFinalAnswer(scal1, answer1, scal2, answer2);
        //set the expected card and scal values for the database object

        return answer2;
    }

    /// <summary>
    /// One last check that this Pair vector isn't just the same vector as the first vector
    /// The edge case for this is if Vector1(answer1) is parallel to the answer, in that case
    /// Vector2 will also be parallel, and possibly the same values.  If that happens
    /// we throw out Vector1 and recalc Vector2.
    /// </summary>
    /// <param name="testFinal"></param>
    /// <returns></returns>
    private bool ValidateProposedAnswer(Vector3 testFinal)
    {
        switch (GCP04.Difficulty)
        {
            case 1:
                switch (LimitAxis)
                {
                    //Zero vectors are allowed here for "extra" vectors
                    case Direction.X:
                        //compare the magnitude sizes
                        if (Mathf.Abs(testFinal.x) < Mathf.Abs(InitFinalVector.x / 3))
                            return false;

                        //compare the positivity/negativity
                        if ((InitFinalVector.x > 0 && (testFinal.x) <= 0) || (InitFinalVector.x < 0 && (testFinal.x) >= 0))
                            return false;

                        break;

                    case Direction.Y:
                        //compare the magnitude sizes
                        if (Mathf.Abs(testFinal.y) < Mathf.Abs(InitFinalVector.y / 3))
                            return false;

                        //compare the positivity/negativity
                        if ((InitFinalVector.y > 0 && (testFinal.y) <= 0) || (InitFinalVector.y < 0 && (testFinal.y) >= 0))
                            return false;
                        break;
                }

                break;

            case 2:
                //compare the magnitude sizes
                if (Mathf.Abs(testFinal.x) < Mathf.Abs(InitFinalVector.x / 3))
                    return false;
                if (Mathf.Abs(testFinal.y) < Mathf.Abs(InitFinalVector.y / 3))
                    return false;

                //compare the positivity/negativity
                if ((InitFinalVector.x > 0 && (testFinal.x) <= 0) || (InitFinalVector.x < 0 && (testFinal.x) >= 0))
                    return false;
                if ((InitFinalVector.y > 0 && (testFinal.y) <= 0) || (InitFinalVector.y < 0 && (testFinal.y) >= 0))
                    return false;
                break;

            case 3:
                //compare the magnitude sizes
                if (Mathf.Abs(testFinal.x) < Mathf.Abs(InitFinalVector.x / 3))
                    return false;
                if (Mathf.Abs(testFinal.y) < Mathf.Abs(InitFinalVector.y / 3))
                    return false;
                if (Mathf.Abs(testFinal.z) < Mathf.Abs(InitFinalVector.z / 3))
                    return false;
                //compare the positivity/negativity
                if ((InitFinalVector.x > 0 && (testFinal.x) <= 0) || (InitFinalVector.x < 0 && (testFinal.x) >= 0))
                    return false;
                if ((InitFinalVector.y > 0 && (testFinal.y) <= 0) || (InitFinalVector.y < 0 && (testFinal.y) >= 0))
                    return false;
                if ((InitFinalVector.z > 0 && (testFinal.z) <= 0) || (InitFinalVector.z < 0 && (testFinal.z) >= 0))
                    return false;
                break;
        }
        return true;
    }

    private Vector3 SetFinalAnswer(int scal1, Vector3 vect1, int scal2, Vector3 vect2)
    {
        puzzleData.e_Scal1 = scal1;
        puzzleData.e_Vect1 = vect1;
        puzzleData.e_Scal2 = scal2;
        puzzleData.e_Vect2 = vect2;
        puzzleData.answer = (vect1 * scal1) + (vect2 * scal2);
        return (vect1 * scal1) + (vect2 * scal2);
    }

    private bool V1reversedScal(int scal, Vector3 vector)
    {
        float origDisplacementMag = (startingAnswer - (vector * scal)).magnitude;
        float reversedDisplacementMag = (startingAnswer - (vector * (scal * (-1)))).magnitude;

        //basically, if the original displacement is smaller leave it as is, otherwise flip the scalar when calculating "answer2".
        return origDisplacementMag > reversedDisplacementMag;
    }

    /// <summary>
    /// Shuffles the order of the answer cards
    /// </summary>
    public void answerShuffle()
    {
        //Shuffle Cards
        int count = cardVectors.Count;
        int last = count - 1;
        for (int i = 0; i < last; i++)
        {
            int r = UnityEngine.Random.Range(i, count);
            Vector3 temp = cardVectors[i];
            cardVectors[i] = cardVectors[r];
            cardVectors[r] = temp;
        }
    }

    /// <summary>
    /// Tests if the input vector is duplicate or a scalar multiple
    /// of a card that already exists in the cardVectors list.
    /// If it's not a duplicate, it returns "True" that it's unique
    /// If it is a dupicate it returns "False".
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public bool testUnique(Vector3 test)
    {
        bool unique = true;

        //Test that the new Random Vector is not a Duplicate or scalar
        //cycle through scalars:
        for (int i = -MaxCardScalar; i <= MaxCardScalar; i++)
        {
            //cycle through cards:
            for (int j = 0; j < cardVectors.Count; j++)
            {
                if (test * i == cardVectors[j] || test == cardVectors[j] * i)
                {
                    unique = false;
                }
                else
                    unique = true;

                if (!unique)
                    break;
            }
            if (!unique)
                break;
        }

        return unique;
    }
    #endregion

    #region Get/Sets
    public Vector3 getMinVector()
    {
        return InitFinalVector;
    }

    public Vector3 getDynamicVector()
    {
        return DynamicVector;
    }

    public Vector3 getAnswerVector()
    {
        return finalAnswerVector;
    }

    public Vector3 getPlayerAnswer()
    {
        return playerAnswer;
    }

    public Direction getDirection()
    {
        return LimitAxis;
    }

    public List<Vector3> getAnswerCards()
    {
        return cardVectors;
    }

    public int getScalar1()
    {
        return scalar1;
    }
    public int getScalar2()
    {
        return scalar2;
    }

    public void setScalar1(int val)
    {
        scalar1 = val;
    }
    public void setScalar2(int val)
    {
        scalar2 = val;
    }
    public void setAnsCard1(Vector3 val)
    {
        card1 = val;
    }
    public void setAnsCard2(Vector3 val)
    {
        card2 = val;
    }

    public Vector3 getAnsCard1()
    {
        return card1;
    }
    public Vector3 getAnsCard2()
    {
        return card2;
    }

    public bool isAns1Entered()
    {
        return ans1Entered;
    }
    public bool isAns2Entered()
    {
        return ans2Entered;
    }

    public Transform getCameraTransform()
    {
        return CameraStartPosition.transform;
    }

    public GameObject getCameraTarget()
    {
        return cameraTarget;
    }

    public void setPrevCameraPos(Vector3 currPos)
    {
        prevCameraPosition = currPos;
    }

    public Vector3 getPrevCameraPos()
    {
        return prevCameraPosition;
    }

    public VisualVector getVisualVector()
    {
        return VV01;
    }

    public GameControllerPuzzle04 getGameController()
    {
        return GCP04;
    }

    public bool getCorrect()
    {
        return correct;
    }
    #endregion

    /// <summary>
    /// This function creates an attemptData CSV string with the current attempt values and returns it.
    /// This is done so that when checking for possible attempt entries to add to the attemptsList
    /// we aren't needlessly creating heap memory items.  The heap memory is only reserved WHEN
    /// EnterAttempt() needs to add a new node
    /// </summary>
    /// <returns></returns>
    private string CreateAttemptCSV()
    {
        string temp = "";

        temp += attempts.ToString() + ", "; //attempt number
        temp += obstacleTime.ToString("0.00") + ", "; //time attempted at
        temp += scalar1.ToString() + ", "; //scalar value for vector 1
        temp += card1.x.ToString() + ", " + card1.y.ToString() + ", " + card1.z.ToString() + ", "; // vector1 information
        temp += scalar2.ToString() + ", "; //scalar value for vector 2
        temp += card2.x.ToString() + ", " + card2.y.ToString() + ", " + card2.z.ToString() + "\n"; // vector 2 information

        return temp;
    }

    private void EnterAttempts()
    {
        //If card 1 or 2 are changed automatically add it to attempts
        if ((card1 != prevVect1 || card2 != prevVect2) && (card1 != Vector3.zero || card2 != Vector3.zero))
        {
            chgScal1 = false;
            chgScal2 = false;
            attempts++;
            attemptsList += CreateAttemptCSV();
//            puzzleData.attemptsList.Add(CreateAttemptNode());
        }
        
        //We'll note that the player changed a scalar but not automatically add it
        //instead we'll reset a wait clock each time they change it.  If after changing
        //a scalar they wait for a certain time at one value THEN we'll add that attempt to the list
        if (scalar1 != prevScal1)
        {
            chgScal1 = true;
            time1 = 0.0f;
        }

        if (scalar2 != prevScal2)
        {
            chgScal2 = true;
            time2 = 0.0f;
        }

        if (chgScal1)
        {
            time1 += Time.deltaTime;
            if(time1 > waitTimeToEnterAttempt)
            {
                chgScal1 = false;
                attempts++;
                attemptsList += CreateAttemptCSV();
                //                puzzleData.attemptsList.Add(CreateAttemptNode());
            }
        }

        if (chgScal2)
        {
            time2 += Time.deltaTime;
            if (time2 > waitTimeToEnterAttempt)
            {
                chgScal2 = false;
                attempts++;
                attemptsList += CreateAttemptCSV();
                //                puzzleData.attemptsList.Add(CreateAttemptNode());
            }
        }

    }

    public bool checkFinalAnswer()
    {
        if (ans1Entered && ans2Entered)
        {
            if (playerAnswer == finalAnswerVector)
            {
                //on first time answering correctly
                if (!correct)
                    GCP04.P04W.SetComplete();
                correct = true;

                GameRoot.instance.LockCamera(false);
                grappleKit.InitGrapple(correct, finalAnswerVector, goalPoint, player.transform.position, grappleArcHeight);
                grappleKit.grappleToGoal(this);

                puzzleData.obsTime = this.obstacleTime;
                puzzleData.i_Scal1 = this.scalar1;
                puzzleData.i_Vect1 = this.card1;
                puzzleData.i_Scal2 = this.scalar2;
                puzzleData.i_Vect2 = this.card2;

                //Did the player arrive at the answer with the expected Scal/Vectors?
                if (puzzleData.e_Scal1 != puzzleData.i_Scal1 ||
                    puzzleData.e_Scal2 != puzzleData.i_Scal2 ||
                    puzzleData.e_Vect1 != puzzleData.i_Vect1 ||
                    puzzleData.e_Vect2 != puzzleData.i_Vect2)
                    puzzleData.userUniqueSolution = true;
                else
                    puzzleData.userUniqueSolution = false;

                GCP04.totMoves += attempts;
                GCP04.questionsList += "[" +
                                        finalAnswerVector.x.ToString() + ", " +
                                        finalAnswerVector.y.ToString() + ", " +
                                        finalAnswerVector.z.ToString() + "]\n";

                GCP04.SaveLocalPuzzleData(this);

                return true;
            }
            else
            {
                correct = false;
                grappleKit.SetFailShot(VV01.vector2end.transform.position);
                turnOffVisualVectors();
                //grappleKit.InitGrapple(correct, finalAnswerVector, VV01.vector2end, player.transform.position, grappleArcHeight);
                //grappleKit.grappleToGoal(this);


                return false;
            }
        }
        else
        {
            GameRoot.instance.dynamicWindow.ShowTips("You must select two answer cards", true, true);

            return false;
        }

    }



    //------------------------------------
    // TOOL ACTIONS
    //------------------------------------
    #region Tool Actions
    // PORTAL---------------
    #region Portal
    public void OnPortalEnter()
    {
        inPortal = true;
        GCP04.currPuzzle = this;
        GCP04.SetPuzzlePause(true);

        GameRoot.isPuzzleLock = true;
        GameRoot.instance.LockPlayerAndCam(true);

        visVectCamera.GetComponent<Camera>().enabled = true;
        visVectCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;

        cameraRotate.SetCameraTarget(cameraTarget.transform.position);

        puzzleCamera.transform.position = mainCamera.transform.position;
        puzzleCamera.transform.rotation = mainCamera.transform.rotation;      
        
        puzzleCamera.GetComponent<Camera>().enabled = true;
        mainCamera.GetComponent<Camera>().enabled = false;
       

        cameraGlide.SetObjectMoveSpeed(0.25f);
        cameraGlide.SetObjectRotateSpeed(0.07f);
        cameraGlide.SetEventSys(GameRoot.camEvents);
        cameraGlide.GlideAndLookMovingTarget(puzzleCamera, CameraStartPosition, cameraTarget, prevCameraPosition);

        GameRoot.camEvents.AddListener(activateCamRotate);       

        GCP04.P04W.setPuzzleController(this);        

        //Dialog Trigger
        if (convoTriggerNumber != 0)
        {
            GCP04.conversation(convoTriggerNumber);
            DialogueManager.isPuzzleLock = true;
        }

        GCP04.P04W.ShowInputPanel(true);
        GCP04.P04W.ShowFeedbackPanel(true);
        GCP04.P04W.ShowCardPanel(true);
        GCP04.P04W.ResetCardPositions();
        GCP04.P04W.ShowButtonPanel(true);
        GCP04.P04W.exitPanel.gameObject.SetActive(true);
        GCP04.inPuzzle = true;

        GCP04.P04W.resetButtons();

        grappleKit.SetPuzzleController(this);
        grappleKit.ropeStart.transform.position = this.startPoint.transform.position;
        grappleKit.ropeEnd.transform.position = this.startPoint.transform.position;


        assignCards();
        GCP04.P04W.setAnswer1(null);
        setAnsCard1(Vector3.zero);
        GCP04.P04W.setAnswer2(null);
        setAnsCard2(Vector3.zero);

        turnOnVisualVectors();

        GameRoot.instance.ShowCursor(true);
    }

    public void OnPortalExit()
    {
        inPortal = false;
        isActive = false;
        GCP04.SetPuzzlePause(true);

        GameRoot.isPuzzleLock = false;

        visVectCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
        visVectCamera.GetComponent<Camera>().enabled = false;        

        if (getCorrect())
        {
            ToggleAllTriggers(false);
            cameraGlide.SetObjectMoveSpeed(6.0f);
            cameraGlide.SetObjectRotateSpeed(0.5f);
        }

        cameraRotate.enabled = false;
        cameraGlide.SetEventSys(GameRoot.camEvents);
        cameraGlide.GlideAndLookMovingTarget(puzzleCamera, mainCamera, player, puzzleCamera.transform.position);

        GameRoot.camEvents.AddListener(activateCamPlayer);

        GCP04.P04W.ShowInputPanel(false);
        GCP04.P04W.ShowFeedbackPanel(false);
        GCP04.P04W.ShowCardPanel(false);
        GCP04.P04W.ShowButtonPanel(false);
        GCP04.P04W.exitPanel.gameObject.SetActive(false);
        GCP04.inPuzzle = false;

        GCP04.P04W.resetButtons();

        GCP04.P04W.resetCardPos();

        GCP04.P04W.setAnswer1(null);
        setAnsCard1(Vector3.zero);
        GCP04.P04W.setAnswer2(null);
        setAnsCard2(Vector3.zero);

        GCP04.P04W.scalar1.value = 1;
        GCP04.P04W.scalar2.value = 1;

        turnOffVisualVectors();

        DialogueManager.isPuzzleLock = false;

        GameRoot.instance.ShowCursor(false);
    }

    private void assignCards()
    {
        GCP04.P04W.setCardValues(getAnswerCards());
        GCP04.P04W.setCardDisplay(this);
        GCP04.P04W.setFeedbackDisplay(this);
    }

    #endregion

    //GAP TRIGGERS------------------
    #region Gap Triggers
    public void ToggleAllTriggers(bool value)
    {
        gapTriggers.ToggleFallTriggers(value);
        gapTriggers.ToggleFinishTriggers(value);
        gapTriggers.ToggleResetTriggers(value);
    }

    #endregion

    // VISUAL VECTOR----------------
    #region Visual Vector
    public void turnOnVisualVectors()
    {
        VV01.ToggleBasicVectors(true);

        switch (GCP04.Difficulty)
        {
            case 1:
                switch (getDirection())
                {
                    case Direction.X:
                        VV01.setAxisNodes(true, false, false);
                        VV01.setGridBarNodes(true, false, false);
                        break;
                    case Direction.Y:
                        VV01.setAxisNodes(false, true, false);
                        VV01.setGridBarNodes(false, true, false);
                        break;
                }
                break;
            case 2:
                VV01.setAxisNodes(true, true, false);
                VV01.setGridBarNodes(true, true, false);
                break;
            case 3:
                VV01.setAxisNodes(true, true, true);
                VV01.setGridBarNodes(true, true, true);
                break;
        }

    }

    public void turnOffVisualVectors()
    {
        //Basic is player vectors, goal vector, orbs
        VV01.ToggleBasicVectors(false);

        VV01.getXvector().SetActive(false);
        VV01.getYvector().SetActive(false);
        VV01.getZvector().SetActive(false);

        VV01.deactivateAxis();
        VV01.deactivateBarGrid();
        VV01.toggleProjections(false);
        VV01.toggleAnchors(false);
    }

    #endregion

    //CAMERA GLIDE/ROTATE------------------
    #region Camera Glide/Rotate
    private void activateCamRotate()
    {
        GCP04.SetPuzzlePause(false);  
        GameRoot.camEvents.RemoveListener(activateCamRotate);
 //       player.GetComponent<ThirdPersonCharacter>().enabled = true;
        cameraRotate.enabled = true;
    }

    private void activateCamPlayer()
    {
        GCP04.SetPuzzlePause(false);
        GameRoot.camEvents.RemoveListener(activateCamPlayer);
        cameraRotate.enabled = false;

        GameRoot.instance.LockPlayerAndCam(false);

        visVectCamera.GetComponent<Camera>().enabled = false;
        puzzleCamera.GetComponent<Camera>().enabled = false;
        mainCamera.GetComponent<Camera>().enabled = true;
    }
    #endregion
    #endregion
}

