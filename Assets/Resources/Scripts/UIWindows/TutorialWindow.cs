using UnityEngine;
using UnityEngine.UI;

public class TutorialWindow : WindowRoot
{
    public bool isInit;

    [Header("UI Element")]
    public GameObject MovementTutorial;
    public GameObject W;
    public GameObject A;
    public GameObject S;
    public GameObject D;
	public GameObject LShift;

	public GameObject CameraTutorial;
	public GameObject I;
	public GameObject J;
	public GameObject K;
	public GameObject L;

	public GameObject TopViewTutorial;
    public GameObject Z;
    public GameObject Z2;

	public GameObject Tab;
    public GameObject FieldInput;
    public GameObject rotateCamera;

    public GameObject SelectionTutorial;
    public GameObject findCorrectOption;
    public GameObject pressE;

    public GameObject Congrats;

    public GameObject Questions;
    public InputField textInput;
    public Button submitBotton;

    [Header("GameObject")]
    public GameObject mainCamera;
    public GameObject topCamera;

    private TutorialContorller TC;

    private bool topCameraActive = false;
    private bool MovementComplete = false;
    private bool wComplete = false;
    private bool aComplete = false;
    private bool sComplete = false;
    private bool dComplete = false;
	private bool LShiftComplete = false;

	private bool CameraComplete = false;
	private bool iComplete = false;
	private bool jComplete = false;
	private bool kComplete = false;
	private bool lComplete = false;

	private bool zComplete = false;
    private bool zpressedonce = false;
    private bool zpressedtwice = false;

	private bool tabComplete = false;
    private bool rotateComplete = false;
    public bool selectionComplete = false;

    public string inputedText;
    public GameObject player;
    private Vector3 startPosition;
    private Vector3 previousPosition;
    public int timer = 0;


    private void Start()
    {
		if (isInit)
        {
            InitWindow();
        }
    }

    protected override void InitWindow()
    {
        Debug.Log("Init Tutorial window");
        base.InitWindow();

        //TC = GameObject.FindGameObjectWithTag("GameController").GetComponent<TutorialContorller>();

        //TC.InitGameController();

        player = GameObject.FindGameObjectWithTag("Player");
        startPosition = player.transform.position;

        mainCamera = Camera.main.gameObject;
        topCamera = GameObject.FindGameObjectWithTag("TopCamera").gameObject;

		topCamera.SetActive(false);

		topCameraActive = false;
		MovementComplete = false;
		wComplete = false;
		aComplete = false;
		sComplete = false;
		dComplete = false;
		LShiftComplete = false;

		CameraComplete = false;
		iComplete = false;
		jComplete = false;
		kComplete = false;
		lComplete = false;

		zComplete = false;
		zpressedonce = false;
		zpressedtwice = false;

		tabComplete = false;
		rotateComplete = false;
		selectionComplete = false;

		textInput.text = "Number ...";

		inputedText = "";

		W.GetComponent<Text>().color = Color.black;
		A.GetComponent<Text>().color = Color.black;
		S.GetComponent<Text>().color = Color.black;
		D.GetComponent<Text>().color = Color.black;

		I.GetComponent<Text>().color = Color.black;
		J.GetComponent<Text>().color = Color.black;
		K.GetComponent<Text>().color = Color.black;
		L.GetComponent<Text>().color = Color.black;
		LShift.GetComponent<Text>().color = Color.black;

		Tab.GetComponent<Text>().color = Color.black;
		Z.GetComponent<Text>().color = Color.black;
		Z2.GetComponent<Text>().color = Color.black;

		MovementTutorial.SetActive(true);
		CameraTutorial.SetActive(false);
        TopViewTutorial.SetActive(false);
        FieldInput.SetActive(false);
        Questions.SetActive(false);
        SelectionTutorial.SetActive(false);
        Congrats.SetActive(false);
        SetActive(pressE, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!topCameraActive)
            {
                mainCamera.SetActive(false);
                topCamera.SetActive(true);
                topCameraActive = true;
            }
            else if (topCameraActive)
            {
                mainCamera.SetActive(true);
                topCamera.SetActive(false);
                topCameraActive = false;
            }

        }
        if (!MovementComplete)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                W.GetComponent<Text>().color = Color.green;
                wComplete = true;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                A.GetComponent<Text>().color = Color.green;
                aComplete = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                S.GetComponent<Text>().color = Color.green;
                sComplete = true;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                D.GetComponent<Text>().color = Color.green;
                dComplete = true;
            }
			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				LShift.GetComponent<Text>().color = Color.green;
				LShiftComplete = true;
			}
			if (dComplete && wComplete && aComplete && sComplete && LShiftComplete)
            {
                MovementTutorial.SetActive(false);
                CameraTutorial.SetActive(true);
                MovementComplete = true;
            }
        }

		if (MovementComplete && !CameraComplete)
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				I.GetComponent<Text>().color = Color.green;
				iComplete = true;
			}
			if (Input.GetKeyDown(KeyCode.J))
			{
				J.GetComponent<Text>().color = Color.green;
				jComplete = true;
			}
			if (Input.GetKeyDown(KeyCode.K))
			{
				K.GetComponent<Text>().color = Color.green;
				kComplete = true;
			}
			if (Input.GetKeyDown(KeyCode.L))
			{
				L.GetComponent<Text>().color = Color.green;
				lComplete = true;
			}
			if (lComplete && iComplete && jComplete && kComplete)
			{
				CameraTutorial.SetActive(false);
				TopViewTutorial.SetActive(true);
				CameraComplete = true;
			}
		}

		if (MovementComplete && CameraComplete && !zComplete)
        {
            if (Input.GetKeyDown(KeyCode.Z) && zpressedonce == false)
            {
                Z.GetComponent<Text>().color = Color.green;
                zpressedonce = true;
            }

            else if (Input.GetKeyDown(KeyCode.Z) && zpressedonce)
            {
                Z2.GetComponent<Text>().color = Color.green;
                zpressedtwice = true;
            }

            if (zpressedonce && zpressedtwice)
            {
                TopViewTutorial.SetActive(false);
                FieldInput.SetActive(true);
                Questions.SetActive(true);
                zComplete = true;
            }
        }

        if (MovementComplete && CameraComplete && zComplete && !rotateComplete)
        {
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				Tab.GetComponent<Text>().color = Color.green;
				tabComplete = true;
			}
			if (inputedText == "5")
            {
                Debug.Log("Correct awnser inputted");
                FieldInput.SetActive(false);
                Questions.SetActive(false);
                SelectionTutorial.SetActive(true);
                rotateComplete = true;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log(inputedText);
            }
        }

        if (player.transform.position.x - previousPosition.x < 0.01 && player.transform.position.y - previousPosition.y < 0.01 && player.transform.position.y - previousPosition.y < 0.01)
        {
            if (selectionComplete && MovementComplete && CameraComplete && zComplete && rotateComplete)
            {
                SelectionTutorial.SetActive(false);
                Congrats.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = startPosition;
            //Debug.Log("R pressed");
            GameRoot.ShowTips("", true, false);
        }


        previousPosition = player.transform.position;
    }

    public void ButtonPressed()
    {
        inputedText = textInput.text;
        Debug.Log("Button pressed" + inputedText);
    }
}
