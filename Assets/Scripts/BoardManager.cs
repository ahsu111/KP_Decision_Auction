using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;


// This Script (a component of Game Manager) Initializes the Borad (i.e. screen).
public class BoardManager : MonoBehaviour
{
    // Resoultion width and Height
    // CAUTION! Modifying this does not modify the Screen resolution.
    // items will be displayed inside a 1600x900 box
    public static int resolutionWidth = 1600;
    public static int resolutionHeight = 900;

    // leave some margin for the "TooHeavy" text and centre timer
    public static int bottommargin = 100;
    public static int centremargin = 200;

    // Number of Columns and rows of the grid (the possible positions of the items).
    // 1920 x 1080; 16:9; 100 pixels should be sufficient
    public static int columns;
    public static int rows;

    //Prefab of the item interface configuration
    public static GameObject KSItemPrefab;

    //A canvas where all the board is going to be placed
    public static GameObject canvas;

    //The possible positions of the items;
    public static List<Vector3> gridPositions = new List<Vector3>();

    //Weights and value vectors for this trial. CURRENTLY SET UP TO ALLOW ONLY INTEGERS.
    //ws and vs must be of the same length
    public static int[] ws;
    public static int[] vs;

    public static int nitems;

    //If randomization of buttons:
    //1: No/Yes 0: Yes/No
    public static int randomYes;//=Random.Range(0,2);

    //The answer Input by the player
    //0:No / 1:Yes / 2:None
    public static int answer;

    public static string question;

    //Should the key be working?
    public static bool keysON = false;


    //Structure with the relevant parameters of an item.
    //gameItem: is the game object
    //coorValue1: The coordinates of one of the corners of the encompassing rectangle of the Value Part of the Item. The coordinates are taken relative to the center of the item.
    //coorValue2: The coordinates of the diagonally opposite corner of the Value Part of the Item.
    //coordWeight1 and coordWeight2: Same as before but for the weight part of the item.
    private struct Item
    {
        public GameObject gameItem;
        public Vector2 center;
        public int ItemNumber;
        public Button ItemButton;
    }

    //The items for the scene are stored here.
    private static Item[] items;


    // Current Instance number
    public static int currInstance;


    // Vars relating to BDM Auction
    public static float subjectBid;
    public static float computerBid;

    public static float maxNum = 2.0f;

    public static GameObject subbar;
    public static GameObject subbid;
    public static GameObject result;
    public static GameObject compbar;
    public static GameObject computerbid;

    public static Image subjectBar;
    public static Text subjectBidding;
    public static Image computerBar;
    public static Text computerBidding;
    public static Text auctionResult;


    public static Button[] button_list;
    public static int selected_button;

    public static bool auction_finished = false;

    // Relating to sampling task
    public static int boxsize = 300; //for a square, length = width = 300 in the current task.
    public static int boxcoord = 250;
    public static int dotsize = 20; //the dot is stored in a 20*20 box.
    public static int outsideposofbox = boxcoord + boxsize / 2; //the x position of the left side of the left box is -400, and the right side of the right box is 400.
    public static int insideposofbox = boxcoord - boxsize / 2; //the x position of the right side of the left box is -100, and 
    public static int gridlines = boxsize / dotsize; //since the box is a square, the number of rows and columns of the grids are the same. // do we need to define the value of the gridlines = 15?
    public static GameObject DotPrefab;

    //the possible positions of the items;
    //these var are arrays, which can be cleared and .Length
    private static List<Vector2> leftgridpositions = new List<Vector2>();
    private static List<Vector2> rightgridpositions = new List<Vector2>();

    //the number of dots of left/right boxes.
    //these are arrays.
    public static int leftbox = 225;
    public static int rightbox = 100;

    //This Initializes the GridPositions which are the possible places where the items will be placed.
    void InitialiseList()
    {
        gridPositions.Clear();

        // Generate a list of possible positions, this is shaped like a box with a centre cut out
        for (int x = -resolutionWidth / 2; x < resolutionWidth / 2; x += resolutionWidth / columns)
        {
            for (int y = -resolutionHeight / 2 + bottommargin; y < resolutionHeight / 2; y += ((resolutionHeight - bottommargin) / rows))
            {
                if (Math.Abs(x) > centremargin || Math.Abs(y) > centremargin)
                {
                    gridPositions.Add(new Vector2(x, y));
                }
            }
        }

        //Debug.Log("Number of possible positions: " + gridPositions.Count);
    }

    //Call only for visualizing grid in the Canvas.
    void seeGrid()
    {
        GameObject hangerpref = (GameObject)Resources.Load("Hanger");
        for (int ss = 0; ss < gridPositions.Count; ss++)
        {
            GameObject hanger = Instantiate(hangerpref, gridPositions[ss], Quaternion.identity) as GameObject;
            canvas = GameObject.Find("Canvas");
            hanger.transform.SetParent(canvas.GetComponent<Transform>(), false);
            hanger.transform.position = gridPositions[ss];
        }
    }

    //Randomizes YES/NO button positions (left or right) and allocates corresponding script to save the correspondent answer.
    void RandomizeButtons()
    {
        Button btnLeft = GameObject.Find("LEFTbutton").GetComponent<Button>();
        Button btnRight = GameObject.Find("RIGHTbutton").GetComponent<Button>();

        randomYes = GameManager.buttonRandomization[GameManager.trial - 1];

        if (randomYes == 1)
        {
            btnLeft.GetComponentInChildren<Text>().text = "No";
            btnRight.GetComponentInChildren<Text>().text = "Yes";
        }
        else
        {
            btnLeft.GetComponentInChildren<Text>().text = "Yes";
            btnRight.GetComponentInChildren<Text>().text = "No";
        }
    }

    //Initializes the instance for this trial:
    //1. Sets the question string using the instance (from the .txt files)
    //2. The weight and value vectors are uploaded
    //3. The instance prefab is uploaded
    void setKnapsackInstance()
    {
        int randInstance = GameManager.instanceRandomization[GameManager.TotalTrials - 1];
        question = "$" + GameManager.kinstances[randInstance].profit + System.Environment.NewLine + GameManager.kinstances[randInstance].capacity + "kg?";

        ws = GameManager.kinstances[randInstance].weights;
        vs = GameManager.kinstances[randInstance].values;

        KSItemPrefab = (GameObject)Resources.Load("KSItem3");

        DotPrefab = (GameObject)Resources.Load("whiteDot");
    }

    //Shows the question on the screen
    public void setQuestion()
    {
        Text Quest = GameObject.Find("Question").GetComponent<Text>();
        Quest.text = question;
    }


    /// <summary>
    /// Instantiates an Item and places it on the position from the input
    /// </summary>
    /// <returns>The item structure</returns>
    /// The item placing here is temporary; The real placing is done by the placeItem() method.
    Item GenerateItem(int itemNumber, Vector3 tempPosition)
    {
        //Instantiates the item and places it.
        GameObject instance = Instantiate(KSItemPrefab, tempPosition,
            Quaternion.identity) as GameObject;
        instance.transform.SetParent(canvas.GetComponent<Transform>(), false);

        //Gets the subcomponents of the item 
        GameObject bill = instance.transform.Find("Bill").gameObject;
        GameObject weight = instance.transform.Find("Weight").gameObject;

        //Sets the Text of the items
        bill.GetComponentInChildren<Text>().text = "$" + vs[itemNumber];
        weight.GetComponentInChildren<Text>().text = ws[itemNumber].ToString() + "kg";


        // Calculates the area of the Value and Weight sections of the item according to approach 2 
        // and then Scales the sections so they match the corresponding area.
        Vector3 curr_billscale = bill.transform.localScale;
        float billscale = (float)Math.Pow(vs[itemNumber] / vs.Average(), 0.6) * curr_billscale.x - 0.15f;

        if (billscale < 0.7f * curr_billscale.x)
        {
            billscale = 0.7f * curr_billscale.x;
        }
        else if (billscale > 1.0f * curr_billscale.x)
        {
            billscale = 1.0f * curr_billscale.x;
        }

        bill.transform.localScale = new Vector3(billscale,
            billscale, billscale);

        Vector3 curr_weightscale = weight.transform.localScale;
        float weightscale = (float)Math.Pow(ws[itemNumber] / ws.Average(), 0.6) * curr_weightscale.x - 0.15f;

        if (weightscale < 0.7f * curr_weightscale.x)
        {
            weightscale = 0.7f * curr_weightscale.x;
        }
        else if (weightscale > 1.0f * curr_weightscale.x)
        {
            weightscale = 1.0f * curr_weightscale.x;
        }

        weight.transform.localScale = new Vector3(weightscale,
            weightscale, weightscale);

        Item itemInstance = new Item
        {
            gameItem = instance,
        };

        itemInstance.ItemNumber = itemNumber;

        return (itemInstance);

    }

    /// <summary>
    /// Places the item on the input position
    /// </summary>
    void placeItem(Item itemToLocate, Vector3 position)
    {
        //Setting the position in a separate line is importatant in order to set it according to global coordinates.
        itemToLocate.gameItem.transform.position = position;
    }


    //Returns a random position from the grid and removes the item from the list.
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // Places all the objects from the instance (ws,vs) on the canvas. 
    // Returns TRUE if all items where positioned, FALSE otherwise.
    private bool LayoutObjectAtRandom()
    {
        int objectCount = ws.Length;
        items = new Item[objectCount];

        for (int i = 0; i < objectCount; i++)
        {
            bool objectPositioned = false;

            Item itemToLocate = GenerateItem(i, new Vector2(-2000, -2000));
            //Debug.Log("Local: " + itemToLocate.gameItem.transform.localPosition);
            //Debug.Log("Global: " + itemToLocate.gameItem.transform.position);
            while (!objectPositioned && gridPositions.Count > 0)
            {
                Vector2 randomPosition = RandomPosition();
                //Instantiates the item and places it.
                itemToLocate.gameItem.transform.localPosition = randomPosition;
                itemToLocate.center = new Vector2(itemToLocate.gameItem.transform.localPosition.x,
                    itemToLocate.gameItem.transform.localPosition.y);

                items[i] = itemToLocate;
                objectPositioned = true;
            }

            if (!objectPositioned)
            {
                Debug.Log("Not enough space to place all items... " +
                    "ran out of randomPositions");
                return false;
            }
        }
        return true;
    }

    /// Macro function that initializes the Board
    /// 1: Trial / 2: trial game answer
    public void SetupScene(string sceneToSetup)
    {

        if (sceneToSetup == "TrialKP")
        {
            canvas = GameObject.Find("Canvas");

            setKnapsackInstance();
            setQuestion();
            keysON = true;
            //If the bool returned by LayoutObjectAtRandom() is false, then retry again:
            //Destroy all items. Initialize list again and try to place them once more.
            int nt = 0;
            bool itemsPlaced = false;
            while (nt < 10 && !itemsPlaced)
            {
                GameObject[] items1 = GameObject.FindGameObjectsWithTag("Item");

                foreach (GameObject item in items1)
                {
                    Destroy(item);
                }

                InitialiseList();

                itemsPlaced = LayoutObjectAtRandom();
                nt++;
            }
        }
        // Setting up sampling task
        else if (sceneToSetup == "TrialSampling")
        {
            keysON = false;

            // init answer as none
            answer = 2;

            canvas = GameObject.Find("Canvas");

            //InitialiseList();
            int randInstance = GameManager.instanceRandomization[GameManager.TotalTrials - 1];

            Debug.Log(randInstance);
            DotPrefab = (GameObject)Resources.Load("whiteDot");

            //If the bool returned by LayoutObjectAtRandom() is false, then retry again:
            //Destroy all items. Initialize list again and try to place them once more.

            GameObject[] items1 = GameObject.FindGameObjectsWithTag("Item");
            foreach (GameObject item in items1)
            {
                Destroy(item);
            }

            leftbox = GameManager.sinstances[randInstance].LeftBox;
            rightbox = GameManager.sinstances[randInstance].RightBox;

            InitialiseListSamplingTask();

            LayoutDotAtRandom();

        }
        else if (sceneToSetup == "TrialAnswer")
        {
            answer = 2;
            setKnapsackInstance();
            RandomizeButtons();
            keysON = true;

            //1234
            //			InitialiseList ();
            //			seeGrid();
        }

        else if (sceneToSetup == "BDM_Auction")
        {
            keysON = true;
            auction_finished = false;

            result = GameObject.Find("auctionResult");
            auctionResult = result.GetComponent<Text>();
            auctionResult.text = "";

            float init_fill = UnityEngine.Random.Range(0.0f, 1.0f);
            subbar = GameObject.Find("subjectBar");
            subjectBar = subbar.GetComponent<Image>();
            subbar.GetComponent<Image>().fillAmount = init_fill;
            subjectBid = maxNum * init_fill;

            subbid = GameObject.Find("subjectBidding");
            subjectBidding = subbid.GetComponent<Text>();
            subjectBidding.text = "$" + subjectBid.ToString("0.#");

            compbar = GameObject.Find("computerBar");
            computerBar = compbar.GetComponent<Image>();
            compbar.GetComponent<Image>().fillAmount = 0.0f;

            computerbid = GameObject.Find("computerBidding");
            computerBidding = computerbid.GetComponent<Text>();
            computerBidding.text = "";
        }

        else if (sceneToSetup == "LikertScale")
        {
            keysON = true;
            selected_button = -1;

            Button ButtonZero = GameObject.Find("ButtonZero").GetComponent<Button>();
            Button ButtonOne = GameObject.Find("ButtonOne").GetComponent<Button>();
            Button ButtonTwo = GameObject.Find("ButtonTwo").GetComponent<Button>();
            Button ButtonThree = GameObject.Find("ButtonThree").GetComponent<Button>();
            Button ButtonFour = GameObject.Find("ButtonFour").GetComponent<Button>();
            Button ButtonFive = GameObject.Find("ButtonFive").GetComponent<Button>();

            button_list = new Button[] { ButtonZero, ButtonOne, ButtonTwo, ButtonThree, ButtonFour, ButtonFive };

            int init_choice = Random.Range(0, 6);
            Debug.Log(init_choice + "    " + button_list[init_choice]);

            selected_button = init_choice;

            button_list[init_choice].Select();
            button_list[init_choice].OnSelect(null);

            foreach (var btn in button_list)
            {
                btn.onClick.AddListener(ButtonClicked);
            }
        }
    }


    //this initializes the gridpositions which are the possible placees wheeree the dots will be placed.
    //the size of the black box containing white dots is 300*300;
    //the size of the frame containg white dot is 20*20;
    //Q: does the number of possible positions indicated here fixed? e.g. 15*15 = 225 all possible places to place dots?
    void InitialiseListSamplingTask()
    {
        leftgridpositions.Clear();
        for (int x = -outsideposofbox; x < -insideposofbox; x += ((outsideposofbox - insideposofbox) / gridlines))
        {
            for (int y = -(outsideposofbox - insideposofbox) / 2; y < (outsideposofbox - insideposofbox) / 2; y += ((outsideposofbox - insideposofbox) / gridlines))
            {
                leftgridpositions.Add(new Vector2(x + 10, y + 10));
            }
        }
        rightgridpositions.Clear();
        for (int x = insideposofbox; x < outsideposofbox; x += ((outsideposofbox - insideposofbox) / gridlines))
        {
            for (int y = -(outsideposofbox - insideposofbox) / 2; y < (outsideposofbox - insideposofbox) / 2; y += ((outsideposofbox - insideposofbox) / gridlines))
            {
                rightgridpositions.Add(new Vector2(x + 10, y + 10));
            }
        }

        //Debug.Log("Number of possible Left positions: " + leftgridpositions.Count);
        //Debug.Log("Number of possible positions: " + rightgridpositions.Count);
    }


    // returns a random position from the grid and reemoves the item from the list.
    // do we need to use for loop to generate multiple random index and places for a trial for left/right boxes
    public static Vector2 dotRandomPosition(string LorR)
    {
        if (LorR == "Left")
        {
            int leftdotRandomIndex = UnityEngine.Random.Range(0, leftgridpositions.Count); //issue with this Random.

            Vector2 leftDotRandomPosition = leftgridpositions[leftdotRandomIndex];

            leftgridpositions.RemoveAt(leftdotRandomIndex);

            return leftDotRandomPosition;
        }
        else
        {
            int rightdotRandomIndex = UnityEngine.Random.Range(0, rightgridpositions.Count);

            Vector2 rightDotRandomPosition = rightgridpositions[rightdotRandomIndex];

            rightgridpositions.RemoveAt(rightdotRandomIndex);

            return rightDotRandomPosition; //how to return multiple results of a function in C#?
        }
    }

    // Places all the objects from the instance (ws,vs) on the canvas. 
    // Returns TRUE if all items where positioned, FALSE otherwise.
    public static bool LayoutDotAtRandom()
    {
        for (int i = 0; i < leftbox; i++)
        {
            GameObject dotToLocate = GenerateDot(i, new Vector2(-2000, -2000));

            Vector2 dotPosition = dotRandomPosition("Left");

            //Instantiates the item and places it.
            dotToLocate.transform.localPosition = dotPosition;
        }
        for (int i = 0; i < rightbox; i++)
        {
            GameObject dotToLocate = GenerateDot(i, new Vector2(-2000, -2000));

            Vector2 dotPosition = dotRandomPosition("Right");

            //Instantiates the item and places it.
            dotToLocate.transform.localPosition = dotPosition;
        }

        return true;
    }


    // Instantiates an Item and places it on the position from the input
    // The item placing here is temporary; The real placing is done by the LayoutObjectAtRandom() method.
    public static GameObject GenerateDot(int itemNumber, Vector2 tempPosition)
    {
        //Instantiates the item and places it.
        GameObject instance = Instantiate(DotPrefab, tempPosition,
            Quaternion.identity) as GameObject;

        instance.transform.SetParent(canvas.GetComponent<Transform>(), false);

        //Item itemInstance = new Item
        //{
        //    gameItem = instance,
        //};

        //itemInstance.ItemNumber = itemNumber;


        return (instance);
    }

    public static void ButtonClicked()
    {
        //Debug.Log("Button Clicked " + EventSystem.current.currentSelectedGameObject.name);// GetComponent<Text>().text);
        //GameManager.SetTimeStamp();
        //GameManager.ChangeToNextScene(itemClicks, false);
    }

    //Updates the timer rectangle size accoriding to the remaining time.
    public void updateTimer()
    {
        // timer = GameObject.Find ("Timer").GetComponent<RectTransform> ();
        // timer.sizeDelta = new Vector2 (timerWidth * (GameManager.tiempo / GameManager.totalTime), timer.rect.height);
        Image timer = GameObject.Find("Timer").GetComponent<Image>();
        timer.fillAmount = GameManager.tiempo / GameManager.totalTime;
    }

    //Sets the triggers for pressing the corresponding keys
    //123: Perhaps a good practice thing to do would be to create a "close scene" function that takes as parameter the answer and closes everything (including keysON=false) and then forwards to 
    //changeToNextScene(answer) on game manager
    private void setKeyInput()
    {

        if (GameManager.escena == "TrialKP")
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GameManager.saveTimeStamp("ParticipantSkip");
                GameManager.changeToNextScene(0, 0, 1);
            }
        }
        if (GameManager.escena == "TrialSampling")
        {
            //1: No/Yes 0: Yes/No

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //Left
                keysON = false;

                answer = new List<int>() { 0, 1 }[randomYes];
                
                int correct = (GameManager.sinstances[GameManager.instanceRandomization[GameManager.TotalTrials - 1]].solution == answer) ? 1 : 0;

                if (correct == 1 && GameManager.trial <= 5)
                {
                    GameManager.Result1.GetComponent<Text>().text = "Your answer is correct";

                    GameManager.tiempo = 3f;
                }
                else if (correct != 1 && GameManager.trial <= 5)
                {
                    GameManager.Result1.GetComponent<Text>().text = "Your answer is incorrect";

                    GameManager.tiempo = 3f;
                }
                else
                {
                    GameManager.changeToNextScene(0, 0, 1);

                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //Right
                keysON = false;

                answer = new List<int>() { 1, 0 }[randomYes];

                int correct = (GameManager.sinstances[GameManager.instanceRandomization[GameManager.TotalTrials - 1]].solution == answer) ? 1 : 0;

                if (correct == 1 && GameManager.trial <= 5)
                {
                    GameManager.Result1.GetComponent<Text>().text = "Your answer is correct";

                    GameManager.tiempo = 3f;
                }
                else if (correct != 1 && GameManager.trial <= 5)
                {
                    GameManager.Result1.GetComponent<Text>().text = "Your answer is incorrect";

                    GameManager.tiempo = 3f;
                }
                else
                {
                    GameManager.changeToNextScene(0, 0, 1);

                }
            }
        }
        else if (GameManager.escena == "TrialAnswer")
        {
            //1: No/Yes 0: Yes/No

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //Left
                keysON = false;

                answer = new List<int>() { 0, 1 }[randomYes];

                GameObject boto = GameObject.Find("LEFTbutton") as GameObject;
                highlightButton(boto);
                //GameManager.changeToNextScene (1,0,2);


                GameObject.Find("LEFTbutton").SetActive(false);
                GameObject.Find("RIGHTbutton").SetActive(false);

                int correct = (GameManager.kinstances[GameManager.instanceRandomization[GameManager.TotalTrials - 1]].solution == answer) ? 1 : 0;

                if (correct == 1 && GameManager.trial <= 5)
                {
                    GameManager.Result1.GetComponent<Text>().text = "Your answer is correct";
                }
                else if (correct != 1 && GameManager.trial <= 5)
                {
                    GameManager.Result1.GetComponent<Text>().text = "Your answer is incorrect";
                }
                else
                {
                    GameManager.changeToNextScene(0, 0, 1);

                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //Right
                keysON = false;

                answer = new List<int>() { 1, 0 }[randomYes];

                GameObject boto = GameObject.Find("RIGHTbutton") as GameObject;
                highlightButton(boto);
                //GameManager.changeToNextScene (0,0,2);


                GameObject.Find("LEFTbutton").SetActive(false);
                GameObject.Find("RIGHTbutton").SetActive(false);

                int correct = (GameManager.kinstances[GameManager.instanceRandomization[GameManager.TotalTrials - 1]].solution == answer) ? 1 : 0;

                if (correct == 1 && GameManager.trial <= 5)
                {
                    GameManager.Result1.GetComponent<Text>().text = "Your answer is correct";
                }
                else if (correct != 1 && GameManager.trial <= 5)
                {
                    GameManager.Result1.GetComponent<Text>().text = "Your answer is incorrect";
                }
                else
                {
                    GameManager.changeToNextScene(0, 0, 1);

                }
            }
        }
        else if (GameManager.escena == "SetUp")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.setTimeStamp();
                GameManager.changeToNextScene(0, 0, 2);
            }
        }
        else if (GameManager.escena == "BDM_Auction")
        {
            if (!auction_finished && Input.GetKey(KeyCode.LeftArrow))
            {
                if (!(subjectBid <= 0))
                {
                    subjectBid -= 0.020f;
                    subbar.GetComponent<Image>().fillAmount = subjectBid / maxNum;

                }
                subjectBidding.text = "$" + (subjectBid).ToString("0.#");
                subbar.GetComponent<Image>().fillAmount = Convert.ToSingle((subjectBid).ToString("0.#")) / maxNum;
            }

            if (!auction_finished && Input.GetKey(KeyCode.RightArrow))
            {

                if (subjectBid < maxNum)
                {
                    subjectBid += 0.020f;
                    subbar.GetComponent<Image>().fillAmount = subjectBid / maxNum;
                }
                subjectBidding.text = "$" + (subjectBid).ToString("0.#");
                subbar.GetComponent<Image>().fillAmount = Convert.ToSingle((subjectBid).ToString("0.#")) / maxNum;
            }

            if (!auction_finished && Input.GetKeyDown(KeyCode.UpArrow))
            {
                float computerBid1 = UnityEngine.Random.Range(0.0f, 1.0f);
                computerBid = maxNum * computerBid1;


                float EPSILON = 0.001f;

                while (Math.Abs(Math.Round(computerBid, 1) - Math.Round(subjectBid, 1)) < EPSILON)
                {
                    computerBid = maxNum * UnityEngine.Random.Range(0.0f, 1.0f);
                }

                compbar.GetComponent<Image>().fillAmount = computerBid1;

                computerbid = GameObject.Find("computerBidding");
                computerBidding = computerbid.GetComponent<Text>();
                computerBidding.text = "$" + (computerBid).ToString("0.#");


                //Debug.Log(subjectBid + "   " + computerBid);


                if (subjectBid > computerBid)
                {
                    auctionResult.text = "Answer Recorded; You won the auction";
                }

                else
                {
                    auctionResult.text = "Answer Recorded; You lost the auction";
                }

                //auction_finished = true;

                GameManager.tiempo = 3.0f;
                GameManager.totalTime = GameManager.tiempo;
            }
        }
        else if (GameManager.escena == "LikertScale")
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GameObject.Find("RecordText").GetComponent<Text>().text = "Answer Recorded";
                GameManager.tiempo = 3.0f;
                GameManager.totalTime = GameManager.tiempo;
            }
        }
    }

    private void highlightButton(GameObject butt)
    {
        Text texto = butt.GetComponentInChildren<Text>();
        texto.color = Color.gray;
    }

    public void setupInitialScreen()
    {
        //Button
        Debug.Log("Start button");
        GameObject start = GameObject.Find("Start") as GameObject;
        start.SetActive(false);

        //Participant Input
        InputField pID = GameObject.Find("ParticipantID").GetComponent<InputField>();

        InputField.SubmitEvent se = new InputField.SubmitEvent();
        //se.AddListener(submitPID(start));
        se.AddListener((value) => submitPID(value, start));
        pID.onEndEdit = se;
    }

    private void submitPID(string pIDs, GameObject start)
    {
        GameObject pID = GameObject.Find("ParticipantID");
        GameObject pIDT = GameObject.Find("ParticipantIDText");
        pID.SetActive(false);
        pIDT.SetActive(false);

        Text inputID = pIDT.GetComponent<Text>();
        inputID.text = "Randomisation Number";

        //Set Participant ID
        GameManager.participantID = pIDs;

        //Set Participant ID
        GameManager.randomisationID = pIDs.Substring(1);
        //Set Participant ID
        GameManager.GAMETYPE = pIDs.Substring(0,1).ToLower();

        Debug.Log(GameManager.GAMETYPE);

        //Activate Start Button and listener
        start.SetActive(true);
        keysON = true;
    }


    public static string getItemCoordinates()
    {
        string coordinates = "";
        foreach (Item it in items)
        {
            //Debug.Log("item");
            //Debug.Log(it.center);
            //Debug.Log(it.coordWeight1);
            coordinates = coordinates + "(" + it.center.x + "," + it.center.y + ")";
        }
        return coordinates;
    }

    // Use this for initialization
    void Start()
    {
        GameManager.saveTimeStamp(GameManager.escena);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(keysON);
        if (keysON)
        {
            setKeyInput();
        }

    }



}
