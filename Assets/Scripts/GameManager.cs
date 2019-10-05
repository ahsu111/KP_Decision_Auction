using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.UI;
//using UnityEditor;
//using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    // Stopwatch to calculate time of events.
    public static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
    // Time at which the stopwatch started. Time of each event is 
    // calculated according to this moment.
    public static string initialTimeStamp;

    //Game Manager: It is a singleton (i.e. it is always one and the same it is nor destroyed nor duplicated)
    public static GameManager instance = null;

    //The reference to the script managing the board (interface/canvas).
    private BoardManager boardScript;

    //Current Scene
    public static string escena;

    //Time spent so far on this scene
    public static float tiempo;

    //Some of the following parameters are a default to be used if they are not specified in the input files.
    //Otherwise they are rewritten (see loadParameters() )
    //Total time for these scene
    public static float totalTime;

    // Time spent at the instance
    public static float timeTaken;

    //Current trial initialization
    public static int trial = 0;

    //Current block initialization
    public static int block = 0;

    //Total trial (As if no blocks were used)
    public static int TotalTrials = 0;

    public static bool showTimer;

    //Modifiable Variables:
    //Minimum and maximum for randomized interperiod Time
    public static float timeRest1min = 5;
    public static float timeRest1max = 9;

    //InterBlock rest time
    public static float timeRest2 = 10;

    //Time given for each trial (The total time the items are shown -With and without the question-)
    public static float timeQuestion = 10.0f;
    public static float timeSampling = 2.0f;
    public static float timeAuction = 20.0f;

    //Time to display sampling dots
    public static float timedots = 0.3f;

    //Time given for answering
    public static float timeAnswer = 3f;
    public static bool dotshown = false;
    
    //Total number of trials in each block
    public static int numberOfTrials = 30;

    //Total number of blocks
    public static int numberOfBlocks = 3;

    //Number of instance file to be considered. From i1.txt to i_.txt..
    public static int numberOfInstances = 3;

    //The order of the instances to be presented
    public static int[] instanceRandomization;

    //The order of the instances to be presented
    public static int[] FeedbackList;

    //The order of the left/right No/Yes randomization
    public static int[] buttonRandomization;

    //This is the string that will be used as the file name where the data is stored. DeCurrently the date-time is used.
    public static string participantID = "Empty";

    //This is the randomisation number (#_param2.txt that is to be used for oder of instances for this participant)
    public static string randomisationID = "Empty";

    public static string dateID = @System.DateTime.Now.ToString("dd MMMM, yyyy, HH-mm");

    public static string Identifier;

    //Is the question shown on scene 1?
    //private static int questionOn;

    //Input and Outout Folders with respect to the Application.dataPath;
    public static string inputFolder = "/StreamingAssets/Input/";
    public static string inputFolderKSInstances = "/StreamingAssets/Input/KPInstances/";
    public static string outputFolder = "/StreamingAssets/Output/";

    // Complete folder path of inputs and ouputs
    public static string folderPathLoad;
    public static string folderPathLoadInstances;
    public static string folderPathSave;

    public static GameObject Result1;
    public static GameObject ResultBox;

    // A list of floats to record participant performance
    // Performance should always be equal to or greater than 1.
    // Due to the way it's calculated (participant answer/optimal solution), performance closer to 1 is better.
    public static List<float> perf = new List<float>();
    public static float performance;
    public static List<float> paylist = new List<float>();
    public static float pay;

    // Keep track of total payment
    // Default value is the show up fee
    public static float payAmount = 8f;

    // this value is set in the boardmanager submitpid function
    public static float payPerTrial = -1f;

    public static int solutionQ;
    public static int correct;

    //A structure that contains the parameters of each instance
    public struct KInstance
    {
        public int capacity;
        public int profit;

        public int[] weights;
        public int[] values;

        public string id;
        public string type;

        public int solution;
    }

    //A structure that contains the parameters of each sampling instance
    public struct SInstance
    {
        public int LeftBox;
        public int RightBox;

        public string id;
        public string type;

        // Solution is 0 if left box contains more dots, 1 otherwise
        public int solution;
    }

    public static bool first_scene_done = false;
    public static bool both_scenes_done = false;

    public static bool feedbackOn = false;

    //An array of all the instances to be uploaded form .txt files.
    public static KInstance[] kinstances;// = new KSInstance[numberOfInstances];

    //An array of all the instances to be uploaded form .txt files.
    public static SInstance[] sinstances;

    // To record the type of game, KP or Sampling
    public static string GAMETYPE = "GAME TYPE NOT SET";

    public static bool notrecordedrest = false; 

    // Use this for initialization
    void Awake()
    {

        // Complete folder path of inputs and ouputs
        folderPathLoad = Application.dataPath + inputFolder;
        folderPathLoadInstances = Application.dataPath + inputFolderKSInstances;
        folderPathSave = Application.dataPath + outputFolder;
        //Makes the Game manager a Singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //Initializes the game
        boardScript = instance.GetComponent<BoardManager>();

        InitGame();

    }


    //Initializes the scene. One scene is setup, other is trial, other is Break....
    void InitGame()
    {

        /*
		Scene Order: escena
		0=setup
		1=trial game
		2=trial game answer
		3= intertrial rest
		4= interblock rest
		5= end
		*/
        Scene scene = SceneManager.GetActiveScene();
        escena = scene.name;
        Debug.Log("escena " + escena);
        if (escena == "SetUp")
        {

            both_scenes_done = false;
            //Only uploads parameters and instances once.
            block++;
            boardScript.setupInitialScreen();
        }
        else if (escena == "TrialKP")
        {
            trial++;
            TotalTrials = trial + (block - 1) * numberOfTrials;
            Debug.Log(TotalTrials);

            showTimer = true;
            boardScript.SetupScene(escena);

            tiempo = timeQuestion;
            totalTime = timeQuestion;
        }
        else if (escena == "TrialSampling")
        {
            trial++;
            TotalTrials = trial + (block - 1) * numberOfTrials;
            //Debug.Log(TotalTrials);
            showTimer = false;

            GameManager.dotshown = false;

            Result1 = GameObject.Find("ResultText");

            Result1.GetComponent<Text>().text = "";

            ResultBox = GameObject.Find("ResultRectangle");

            GameObject.Find("ResultRectangle").SetActive(false);

            boardScript.SetupScene(escena);

            tiempo = timedots;
            totalTime = timedots;
        }
        else if (escena == "TrialAnswer")
        {
            showTimer = true;

            Result1 = GameObject.Find("ResultText");

            Result1.GetComponent<Text>().text = "";

            ResultBox = GameObject.Find("ResultRectangle");

            GameObject.Find("ResultRectangle").SetActive(false);

            boardScript.SetupScene(escena);

            //Debug.Log(Result1);
            //Result1.SetActive(false);

            tiempo = timeAnswer;
            totalTime = timeAnswer;
        }
        else if (escena == "InterTrialRest")
        {
            showTimer = false;

            if (GAMETYPE == "s")
            {
                GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = Color.white;

                GameObject.Find("Text").GetComponent<Text>().color = Color.black;

            }

            if ((trial != 5 || both_scenes_done) && trial != numberOfTrials)
            {
                tiempo = Random.Range(timeRest1min, timeRest1max);
                totalTime = tiempo;
            }
            else
            {
                tiempo = 0f;
                totalTime = tiempo;
            }
        }
        else if (escena == "InterBlockRest")
        {
            both_scenes_done = false;
            trial = 0;
            block++;
            showTimer = true;

            if (GAMETYPE == "s")
            {
                GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = Color.white;

                GameObject.Find("Text").GetComponent<Text>().color = Color.black;

            }

            tiempo = timeRest2;
            totalTime = tiempo;
        }
        else if (escena == "End")
        {
            showTimer = false;

            GameObject.Find("Skip").GetComponent<Button>().onClick.AddListener(SkipClicked);
        }
        else if (escena == "Payment")
        {
            showTimer = false;

            Text perf = GameObject.Find("PerfText").GetComponent<Text>();
            perf.text = DisplayPerf();

            Text pay = GameObject.Find("PayText").GetComponent<Text>();
            pay.text = "Total Payment: $" + Math.Ceiling(payAmount).ToString();
        }
        else if (escena == "BDM_Auction")
        {
            boardScript.SetupScene(escena);

            // NEED TO CHANGE THE TIME HERE
            tiempo = timeAuction;
            totalTime = tiempo;
            showTimer = true;

        }
        else if (escena == "LikertScale")
        {
            boardScript.SetupScene(escena);

            // NEED TO CHANGE THE TIME HERE
            tiempo = timeAuction;
            totalTime = tiempo;
            showTimer = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (escena != "SetUp" && escena != "End" &&
            escena != "Payment")
        {
            startTimer();
            pauseManager();
        }
    }

    //To pause press alt+p
    //Pauses/Unpauses the game. Unpausing take syou directly to next trial
    //Warning! When Unpausing the following happens:
    //If paused/unpaused in scene 1 or 2 (while items are shown or during answer time) then saves the trialInfo with an error: "pause" without information on the items selected.
    //If paused/unpaused on ITI or IBI then it generates a new row in trial Info with an error ("pause"). i.e. there are now 2 rows for the trial.
    private void pauseManager()
    {
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
            if (Time.timeScale == 1)
            {
                errorInScene("Pause");
            }
        }
    }

    //Saves the data of a trial to a .txt file with the participants ID as filename using StreamWriter.
    //If the file doesn't exist it creates it. Otherwise it adds on lines to the existing file.
    //Each line in the File has the following structure: "trial;answer;timeSpent".
    // itemsSelected in the final solutions (irrespective if it was submitted); xycorrdinates; Error message if any.".
    public static void save(float timeSpent, string error)
    {
        if (escena == "LikertScale")
        {
            // Reverse buttons is 1 if no/yes; 0 if yes/no
            string dataTrialText = GameManager.block + ";" + escena + ";" + BoardManager.selected_button + ";NA;" + timeSpent + ";NA;NA;NA;" + error;


            //Debug.Log("DATA TEXT: "+dataTrialText);
            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TrialInfo.txt", true))
            {
                outputFile.WriteLine(dataTrialText);
            }

        }
        else if (escena == "BDM_Auction")
        {
            // Reverse buttons is 1 if no/yes; 0 if yes/no
            string dataTrialText = GameManager.block + ";" + escena + ";" + BoardManager.subjectBid + ";" + BoardManager.computerBid +";" + timeSpent + ";NA;NA;" + GameManager.pay + ";"
                + error;


            //Debug.Log("DATA TEXT: "+dataTrialText);
            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TrialInfo.txt", true))
            {
                outputFile.WriteLine(dataTrialText);
            }

        }
        else if (GAMETYPE == "k")
        {
            string xyCoordinates = BoardManager.getItemCoordinates();

            solutionQ = kinstances[BoardManager.randInstance - 1].solution;
            correct = (solutionQ == BoardManager.answer) ? 1 : 0;

            // Reverse buttons is 1 if no/yes; 0 if yes/no
            string dataTrialText = GameManager.block + ";" + GameManager.trial + ";" + solutionQ + ";" + BoardManager.answer + ";" + correct
                 + ";" + timeSpent + ";" + BoardManager.randomYes + ";" + BoardManager.randInstance + ";" + GameManager.pay + ";"
                + xyCoordinates + ";" + feedbackOn + ";" + error;

            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TrialInfo.txt", true))
            {
                outputFile.WriteLine(dataTrialText);
            }
        }
        else if (GAMETYPE == "s")
        {
            solutionQ = sinstances[BoardManager.randInstance - 1].solution;
            correct = (solutionQ == BoardManager.answer) ? 1 : 0;

            // Reverse buttons is 1 if no/yes; 0 if yes/no
            string dataTrialText = GameManager.block + ";" + GameManager.trial + ";" + solutionQ + ";" + BoardManager.answer + ";" + correct
                 + ";" + timeSpent + ";" + BoardManager.randInstance + ";" + GameManager.pay + ";" + feedbackOn + ";" + error;


            //Debug.Log("DATA TEXT: "+dataTrialText);
            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TrialInfo.txt", true))
            {
                outputFile.WriteLine(dataTrialText);
            }
        }

        //Options of streamwriter include: Write, WriteLine, WriteAsync, WriteLineAsync
    }

    /// <summary>
    /// Saves the time stamp for a particular event type to the "TimeStamps" File
    /// </summary>
    /// Event type: 1=ItemsWithQuestion;2=AnswerScreen;3=InterTrialScreen;4=InterBlockScreen;5=EndScreen
    public static void saveTimeStamp(string eventType)
    {
        if (escena == "LikertScale")
        {
            string dataTrialText = GameManager.block + ";" + escena +
                ";" + GameManager.timeStamp();

            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TimeStamps.txt", true))
            {
                outputFile.WriteLine(dataTrialText);
            }
        }
        else if (escena == "BDM_Auction")
        {
            string dataTrialText = GameManager.block + ";" + escena +
                ";" + GameManager.timeStamp();

            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TimeStamps.txt", true))
            {
                outputFile.WriteLine(dataTrialText);
            }
        }
        else
        {
            string dataTrialText = GameManager.block + ";" + GameManager.trial +
                ";" + eventType + ";" + GameManager.timeStamp();

            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TimeStamps.txt", true))
            {
                outputFile.WriteLine(dataTrialText);
            }
        }
    }

    /// <summary>
    /// Saves the headers for both files (Trial Info and Time Stamps)
    /// In the trial file it saves:  1. The participant ID. 2. Instance details.
    /// In the TimeStamp file it saves: 1. The participant ID. 2.The time onset of the stopwatch from which the time stamps are measured. 3. the event types description.
    /// </summary>
    private static void saveHeaders()
    {

        Identifier = participantID + "_" + dateID + "_";

        if (GAMETYPE == "s")
        {
            //Saves InstanceInfo
            //an array of string, a new variable called lines3
            string[] lines3 = new string[4 + GameManager.numberOfInstances];
            lines3[0] = "PartcipantID:" + participantID;
            lines3[1] = "RandID:" + randomisationID;
            lines3[2] = "InitialTimeStamp:" + GameManager.initialTimeStamp;
            lines3[3] = "instanceNumber;left;right;answer";
            int l = 4;
            int ksn = 1;
            foreach (SInstance si in GameManager.sinstances)
            {
                //With instance type and problem ID
                lines3[l] = ksn + ";" + si.LeftBox + ";" + si.RightBox + ";" +
                    si.solution;
                l++;
                ksn++;
            }

            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "InstancesInfo.txt", true))
            {
                foreach (string line in lines3)
                    outputFile.WriteLine(line);
            }

            // Trial Info file headers
            string[] lines = new string[4];
            lines[0] = "PartcipantID:" + participantID;
            lines[1] = "RandID:" + randomisationID;
            lines[2] = "InitialTimeStamp:" + GameManager.initialTimeStamp;
            lines[3] = "block;trial;solution;answer;correct;timeSpent;" +
                "instanceNumber;pay;feedbackON;error";


            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TrialInfo.txt", true))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }

            // Time Stamps file headers
            string[] lines1 = new string[4];
            lines1[0] = "PartcipantID:" + participantID;
            lines1[1] = "RandID:" + randomisationID;
            lines1[2] = "InitialTimeStamp:" + GameManager.initialTimeStamp;
            lines1[3] = "block;trial;eventType;elapsedTime";
            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TimeStamps.txt", true))
            {
                foreach (string line in lines1)
                    outputFile.WriteLine(line);
            }
        }

        else if (GAMETYPE == "k")
        {
            //Saves InstanceInfo
            //an array of string, a new variable called lines3
            string[] lines3 = new string[4 + GameManager.numberOfInstances];
            lines3[0] = "PartcipantID:" + participantID;
            lines3[1] = "RandID:" + randomisationID;
            lines3[2] = "InitialTimeStamp:" + GameManager.initialTimeStamp;
            lines3[3] = "instanceNumber;capacity;profit;weights;values;id;type;sol";
            int l = 4;
            int ksn = 1;
            foreach (KInstance ks in GameManager.kinstances)
            {
                //With instance type and problem ID
                lines3[l] = ksn + ";" + ks.capacity + ";" + ks.profit + ";" +
                    string.Join(",", ks.weights.Select(p => p.ToString()).ToArray()) +
                    ";" + string.Join(",", ks.values.Select(p => p.ToString()).ToArray())
                    + ";" + ks.id + ";" + ks.type + ";" + ks.solution;
                l++;
                ksn++;
            }
            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "InstancesInfo.txt", true))
            {
                foreach (string line in lines3)
                    outputFile.WriteLine(line);
            }


            // Trial Info file headers
            string[] lines = new string[4];
            lines[0] = "PartcipantID:" + participantID;
            lines[1] = "RandID:" + randomisationID;
            lines[2] = "InitialTimeStamp:" + GameManager.initialTimeStamp;
            lines[3] = "block;trial;solution;answer;correct;timeSpent;ReverseButtons;instanceNumber;pay;" +
                "xyCoordinates;feedbackON;error";


            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TrialInfo.txt", true))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }

            // Time Stamps file headers
            string[] lines1 = new string[4];
            lines1[0] = "PartcipantID:" + participantID;
            lines1[1] = "RandID:" + randomisationID;
            lines1[2] = "InitialTimeStamp:" + GameManager.initialTimeStamp;
            lines1[3] = "block;trial;eventType;elapsedTime";
            using (StreamWriter outputFile = new StreamWriter(folderPathSave +
                Identifier + "TimeStamps.txt", true))
            {
                foreach (string line in lines1)
                    outputFile.WriteLine(line);
            }

            //// Headers for Clicks file
            //string[] lines2 = new string[4];
            //lines2[0] = "PartcipantID:" + participantID;
            //lines2[1] = "RandID:" + randomisationID;
            //lines2[2] = "InitialTimeStamp:" + GameManager.initialTimeStamp;
            //lines2[3] = "block;trial;itemnumber(100=Reset);Out(0)/In(1)/Reset(2)/Other;time";
            //using (StreamWriter outputFile = new StreamWriter(folderPathSave + Identifier + "Clicks.txt", true))
            //{
            //    WriteToFile(outputFile, lines2);
            //}
        }
    }

    // Helper function to write lines to an outputfile
    private static void WriteToFile(StreamWriter outputFile, string[] lines)
    {
        foreach (string line in lines)
        {
            outputFile.WriteLine(line);
        }

        outputFile.Close();
    }
    /*
	 * Loads all of the instances to be uploaded form .txt files. Example of input file:
	 * Name of the file: i3.txt
	 * Structure of each file is the following:
	 * weights:[2,5,8,10,11,12]
	 * values:[10,8,3,9,1,4]
	 * capacity:15
	 * profit:16
	 *
	 * The instances are stored as kinstances structures in the array of structures: kinstances
	 */
    public static void loadKPInstance()
    {
        string folderPathLoad = Application.dataPath + inputFolderKSInstances;
        kinstances = new KInstance[numberOfInstances];

        for (int k = 1; k <= numberOfInstances; k++)
        {

            var dict = new Dictionary<string, string>();

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(folderPathLoad + "k" + k + ".txt"))
                {

                    string line;
                    while (!string.IsNullOrEmpty((line = sr.ReadLine())))
                    {
                        string[] tmp = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        // Add the key-value pair to the dictionary:
                        dict.Add(tmp[0], tmp[1]);//int.Parse(dict[tmp[1]]);
                    }
                    // Read the stream to a string, and write the string to the console.
                    //String line = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.Log("The file could not be read:");
                Debug.Log(e.Message);
            }

            dict.TryGetValue("weights", out string weightsS);
            dict.TryGetValue("values", out string valuesS);
            dict.TryGetValue("capacity", out string capacityS);
            dict.TryGetValue("profit", out string profitS);
            dict.TryGetValue("solution", out string solutionS);

            kinstances[k - 1].weights = Array.ConvertAll(weightsS.Substring(1, weightsS.Length - 2).Split(','), int.Parse);

            kinstances[k - 1].values = Array.ConvertAll(valuesS.Substring(1, valuesS.Length - 2).Split(','), int.Parse);

            kinstances[k - 1].capacity = int.Parse(capacityS);

            kinstances[k - 1].profit = int.Parse(profitS);

            kinstances[k - 1].solution = int.Parse(solutionS);

            dict.TryGetValue("problemID", out kinstances[k - 1].id);
            dict.TryGetValue("instanceType", out kinstances[k - 1].type);

        }

    }
    public static void loadSamplingInstance()
    {
        string folderPathLoad = Application.dataPath + inputFolderKSInstances;
        sinstances = new SInstance[numberOfInstances];

        for (int k = 1; k <= numberOfInstances; k++)
        {

            var dict = new Dictionary<string, string>();

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(folderPathLoad + "s" + k + ".txt"))
                {

                    string line;
                    while (!string.IsNullOrEmpty((line = sr.ReadLine())))
                    {
                        string[] tmp = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        dict.Add(tmp[0], tmp[1]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("The file could not be read:");
                Debug.Log(e.Message);
            }

            dict.TryGetValue("LeftBox", out string LeftBoxS);
            dict.TryGetValue("RightBox", out string RightBoxS);
            dict.TryGetValue("solution", out string solutionS);

            //sinstances[k - 1].LeftBox = Array.ConvertAll(weightsS.Substring(1, weightsS.Length - 2).Split(','), int.Parse);

            //sinstances[k - 1].RightBox = Array.ConvertAll(valuesS.Substring(1, valuesS.Length - 2).Split(','), int.Parse);            

            sinstances[k - 1].LeftBox = int.Parse(LeftBoxS);

            sinstances[k - 1].RightBox = int.Parse(RightBoxS);

            //sinstances[k - 1].capacity = int.Parse(capacityS);

            //sinstances[k - 1].profit = int.Parse(profitS);

            sinstances[k - 1].solution = int.Parse(solutionS);

        }

    }
    //Loads the parameters form the text files: param.txt and layoutParam.txt
    private static void loadParameters()
    {
        //string folderPathLoad = Application.dataPath.Replace("Assets","") + "DATA/Input/";
        string folderPathLoad = Application.dataPath + inputFolder;
        string folderPathLoadInstances = Application.dataPath + inputFolderKSInstances;
        var dict = new Dictionary<string, string>();

        try
        {   // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(folderPathLoad + "layoutParam.txt"))
            {

                // (This loop reads every line until EOF or the first blank line.)
                string line;
                while (!string.IsNullOrEmpty((line = sr.ReadLine())))
                {
                    // Split each line around ':'
                    string[] tmp = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    // Add the key-value pair to the dictionary:
                    dict.Add(tmp[0], tmp[1]);//int.Parse(dict[tmp[1]]);
                }
            }


            using (StreamReader sr1 = new StreamReader(folderPathLoad + "param.txt"))
            {

                // (This loop reads every line until EOF or the first blank line.)
                string line1;
                while (!string.IsNullOrEmpty((line1 = sr1.ReadLine())))
                {
                    //Debug.Log (1);
                    // Split each line around ':'
                    string[] tmp = line1.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    // Add the key-value pair to the dictionary:
                    dict.Add(tmp[0], tmp[1]);//int.Parse(dict[tmp[1]]);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }


        try
        {
            using (StreamReader sr2 = new StreamReader(folderPathLoadInstances + GAMETYPE + randomisationID + "_param2.txt"))
            {

                // (This loop reads every line until EOF or the first blank line.)
                string line2;
                while (!string.IsNullOrEmpty((line2 = sr2.ReadLine())))
                {
                    //Debug.Log (1);
                    // Split each line around ':'
                    string[] tmp = line2.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    // Add the key-value pair to the dictionary:
                    dict.Add(tmp[0], tmp[1]);//int.Parse(dict[tmp[1]]);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("The randomisation file could not be read. Perhaps it doesn't exist.");
            Debug.Log(e.Message);
            //EditorUtility.DisplayDialog ("The randomisation file could not be read.", e.Message,"Got it! I'll restart the game.");

        }

        assignVariables(dict);

    }

    //Assigns the parameters in the dictionary to variables
    private static void assignVariables(Dictionary<string, string> dictionary)
    {

        //Assigns Parameters
        dictionary.TryGetValue("timeRest1min", out string timeRest1minS);
        dictionary.TryGetValue("timeRest1max", out string timeRest1maxS);
        dictionary.TryGetValue("timeRest2", out string timeRest2S);

        dictionary.TryGetValue("timeQuestion", out string timeQuestionS);

        dictionary.TryGetValue("timeAnswer", out string timeAnswerS);

        dictionary.TryGetValue("numberOfTrials", out string numberOfTrialsS);

        dictionary.TryGetValue("numberOfBlocks", out string numberOfBlocksS);

        dictionary.TryGetValue("numberOfInstances", out string numberOfInstancesS);

        dictionary.TryGetValue("instanceRandomization", out string instanceRandomizationS);
        dictionary.TryGetValue("feedback", out string feedbackS);


        timeRest1min = Convert.ToSingle(timeRest1minS);
        timeRest1max = Convert.ToSingle(timeRest1maxS);
        timeRest2 = Convert.ToSingle(timeRest2S);
        timeQuestion = int.Parse(timeQuestionS);
        timeAnswer = int.Parse(timeAnswerS);
        numberOfTrials = int.Parse(numberOfTrialsS);
        numberOfBlocks = int.Parse(numberOfBlocksS);
        numberOfInstances = int.Parse(numberOfInstancesS);
        instanceRandomization = Array.ConvertAll(instanceRandomizationS.Substring(1, instanceRandomizationS.Length - 2).Split(','), int.Parse);

        Debug.Log(instanceRandomization[0]);
        FeedbackList = Array.ConvertAll(feedbackS.Substring(1, feedbackS.Length - 2).Split(','), int.Parse);
        
        ////Assigns LayoutParameters
        dictionary.TryGetValue("columns", out string columnsS);
        dictionary.TryGetValue("rows", out string rowsS);

        BoardManager.columns = int.Parse(columnsS);
        BoardManager.rows = int.Parse(rowsS);
    }


    //Takes care of changing the Scene to the next one (Except for when in the setup scene)
    public static void changeToNextScene(int skipped)
    {
        BoardManager.keysON = false;
        if (escena == "SetUp")
        {
            loadParameters();
            if (GAMETYPE == "k")
            {
                loadKPInstance();
                saveHeaders();
                SceneManager.LoadScene("InterTrialRest");
            }
            else if (GAMETYPE == "s")
            {
                loadSamplingInstance();
                saveHeaders();
                SceneManager.LoadScene("InterTrialRest");
            }
        }
        else if (escena == "TrialKP")
        {
            if (skipped == 1)
            {
                timeTaken = timeQuestion - tiempo;
            }
            else
            {
                timeTaken = timeQuestion;
            }
            SceneManager.LoadScene("TrialAnswer");
        }
        else if (escena == "TrialSampling")
        {
            if (BoardManager.answer != 2)
            {
                saveTimeStamp("ParticipantAnswer");
            }
            SceneManager.LoadScene("InterTrialRest");
        }
        else if (escena == "TrialAnswer")
        {
            if (BoardManager.answer != 2)
            {
                saveTimeStamp("ParticipantAnswer");
            }
            SceneManager.LoadScene("InterTrialRest");
        }
        else if (escena == "InterTrialRest")
        {
            Debug.Log(block + "   " + trial);
            if (!(trial == 0) && !notrecordedrest)
            {
                // Calc Perf
                perf.Add(correct);

                pay = payPerTrial * correct;

                paylist.Add(pay);

                payAmount += pay;
                Debug.Log("current pay: $" + payAmount);

                // Save participant answer
                save(timeTaken, "");
            }
            notrecordedrest = false;

            changeToNextTrial();
        }
        else if (escena == "InterBlockRest")
        {
            if (GAMETYPE == "k")
            {
                SceneManager.LoadScene("InterTrialRest");
            }
            else if (GAMETYPE == "s")
            {
                SceneManager.LoadScene("InterTrialRest");
            }
        }
        else if (escena == "LikertScale")
        {
            save(timeTaken, "");
            saveTimeStamp("ParticipantAnswer");

            if (first_scene_done == false)
            {
                first_scene_done = true;
                SceneManager.LoadScene("BDM_Auction");
            }
            else if (BoardManager.subjectBid > BoardManager.computerBid)
            {
                if (block < numberOfBlocks)
                {
                    SceneManager.LoadScene("InterBlockRest");
                }
                else
                {
                    SceneManager.LoadScene("End");
                }
            }
            else
            {
                if (GAMETYPE == "k")
                {
                    notrecordedrest = true;
                    both_scenes_done = true;
                    SceneManager.LoadScene("InterTrialRest");
                }
                else if (GAMETYPE == "s")
                {
                    notrecordedrest = true;
                    both_scenes_done = true;
                    SceneManager.LoadScene("InterTrialRest");
                }
            }
        }

        else if (escena == "BDM_Auction")
        {
            // Calc Pay
            if (BoardManager.subjectBid < BoardManager.computerBid)
            {
                pay = 0f;
            }
            else if (BoardManager.subjectBid > BoardManager.computerBid)
            {
                pay = BoardManager.maxNum - BoardManager.computerBid;
            }
            else
            {
                pay = -999f;
            }

            paylist.Add(pay);

            payAmount += pay;
            Debug.Log("current pay after Auction: $" + payAmount);

            save(timeTaken, "");
            saveTimeStamp("ParticipantAnswer");
            
            if (first_scene_done == false)
            {
                first_scene_done = true;
                SceneManager.LoadScene("LikertScale");
            }
            else if (BoardManager.subjectBid > BoardManager.computerBid)
            {
                if (block < numberOfBlocks)
                {
                    SceneManager.LoadScene("InterBlockRest");
                }
                else
                {
                    SceneManager.LoadScene("End");
                }
            }
            else
            {
                if (GAMETYPE == "k")
                {
                    notrecordedrest = true;
                    both_scenes_done = true;
                    SceneManager.LoadScene("InterTrialRest");
                }
                else if (GAMETYPE == "s")
                {
                    notrecordedrest = true;
                    both_scenes_done = true;
                    SceneManager.LoadScene("InterTrialRest");
                }
            }
        }
        else if (escena == "End")
        {
            SceneManager.LoadScene("Payment");
        }
    }


    //Redirects to the next scene depending if the trials or blocks are over.
    private static void changeToNextTrial()
    {
        //Checks if trials are over
        if (trial < numberOfTrials)
        {
            if (trial == 5 && !both_scenes_done)
            {
                first_scene_done = false;
                both_scenes_done = false;
                int rand_scene = Random.Range(0, 2);

                SceneManager.LoadScene(new List<string>() { "BDM_Auction", "LikertScale" }[rand_scene]);
            }
            else
            {
                if (GAMETYPE == "k")
                {
                    SceneManager.LoadScene("TrialKP");
                }
                else if (GAMETYPE == "s")
                {
                    SceneManager.LoadScene("TrialSampling");
                }
            }
        }
        else if (block < numberOfBlocks)
        {
            SceneManager.LoadScene("InterBlockRest");
        }
        else
        {
            SceneManager.LoadScene("End");
        }
    }

    // Function to display user performance (last scene)
    public static string DisplayPerf()
    {
        string perfText = "Performance: ";

        for (int i = 0; i < paylist.Count(); i++)
        {
            Debug.Log(i);
            // Payment calculation
            perfText += " $" + paylist[i] + ";";
        }
        return perfText;
    }

    /// <summary>
    /// In case of an error: Skip trial and go to next one.
    /// Example of error: Not enough space to place all items
    /// </summary>
    /// Receives as input a string with the errorDetails which is saved in the output file.
    public static void errorInScene(string errorDetails)
    {
        Debug.Log(errorDetails);

        BoardManager.keysON = false;
        BoardManager.answer = 3;
        BoardManager.randomYes = -1;
        save(timeQuestion, errorDetails);
        changeToNextTrial();
    }


    /// <summary>
    /// Starts the stopwatch. Time of each event is calculated according to this moment.
    /// Sets "initialTimeStamp" to the time at which the stopwatch started.
    /// </summary>
    public static void setTimeStamp()
    {
        initialTimeStamp = @System.DateTime.Now.ToString("HH-mm-ss-fff");
        stopWatch.Start();
        Debug.Log(initialTimeStamp);
    }

    /// <summary>
    /// Calculates time elapsed
    /// </summary>
    /// <returns>The time elapsed in milliseconds since the "setTimeStamp()".</returns>
    public static string timeStamp()
    {
        long milliSec = stopWatch.ElapsedMilliseconds;
        return (milliSec / 1000f).ToString();
    }


    //Updates the timer (including the graphical representation)
    //If time runs out in the trial or the break scene. It switches to the next scene.
    void startTimer()
    {
        tiempo -= Time.deltaTime;
        //Debug.Log(tiempo + "       Dots shown: " + dotshown + "       Keys on: " + BoardManager.keysON);
        if (showTimer)
        {
            boardScript.updateTimer();
        }

        if (escena == "TrialSampling" && tiempo < 0 && dotshown == false)
        {
            BoardManager.keysON = true;
            tiempo = timeSampling;
            totalTime = tiempo;

            GameObject[] items1 = GameObject.FindGameObjectsWithTag("Item");

            foreach (GameObject item in items1)
            {
                Destroy(item);
            }

            dotshown = true;
        }
        //When the time runs out:
        else if (tiempo < 0)
        {
            changeToNextScene(0);
        }

    }


    // Change to next scene if the user clicks skip
    static void SkipClicked()
    {
        Debug.Log("Skip Clicked");
        changeToNextScene(1);
    }
}

