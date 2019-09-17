using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramdom = UnityEngine.Random;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrialSamplingTask : MonoBehaviour
{
    public static int boxsize = 300; //for a square, length = width = 300 in the current task.
    public static int boxcoord = 250;
    public static int dotsize = 20; //the dot is stored in a 20*20 box.
    public static int outsideposofbox = boxcoord + boxsize / 2; //the x position of the left side of the left box is -400, and the right side of the right box is 400.
    public static int insideposofbox = boxcoord - boxsize / 2; //the x position of the right side of the left box is -100, and 
    public static int gridlines = 15; //since the box is a square, the number of rows and columns of the grids are the same. // do we need to define the value of the gridlines = 15?
    public static GameObject DotPrefab;
    public static GameObject canvas;

    //the possible positions of the items;
    //these var are arrays, which can be cleared and .Length
    private static List<Vector2> leftgridpositions = new List<Vector2>();
    private static List<Vector2> rightgridpositions = new List<Vector2>();

    //the number of dots of left/right boxes.
    //these are arrays.
    public static int leftbox = 225;
    public static int rightbox = 100;


    // Use this for initialization
    void Start()
    {
        Debug.Log("Running                               start");

        canvas = GameObject.Find("Canvas");
        DotPrefab = (GameObject)Resources.Load("whiteDot");

        //SetKPInstance();

        //If the bool returned by LayoutObjectAtRandom() is false, then retry again:
        //Destroy all items. Initialize list again and try to place them once more.

        //GameObject[] items1 = GameObject.FindGameObjectsWithTag("Item");

        //foreach (GameObject item in items1)
        //{
        //    Destroy(item);
        //}

        InitialiseListSamplingTask();

        LayoutDotAtRandom();

    }
    /// Macro function that initializes the Board
    //public void SetupTrial()
    //{
    //    previousitems.Clear();
    //    itemClicks.Clear();
    //    GameManager.valueValue = 0;
    //    GameManager.weightValue = 0;
    //    itemsvisited = 0;

    //    canvas = GameObject.Find("Canvas");

    //    SetKPInstance();

    //    //If the bool returned by LayoutObjectAtRandom() is false, then retry again:
    //    //Destroy all items. Initialize list again and try to place them once more.
    //    int nt = 0;
    //    bool itemsPlaced = false;
    //    while (nt < 10 && !itemsPlaced)
    //    {
    //        GameObject[] items1 = GameObject.FindGameObjectsWithTag("Item");

    //        foreach (GameObject item in items1)
    //        {
    //            Destroy(item);
    //        }

    //        InitialiseList();

    //        itemsPlaced = LayoutObjectAtRandom();
    //        nt++;
    //    }
    //}

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

        Debug.Log("Number of possible Left positions: " + leftgridpositions.Count);
        Debug.Log("Number of possible positions: " + rightgridpositions.Count);
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
}
