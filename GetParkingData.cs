using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class GetParkingData : MonoBehaviour
{
    //Statuses of all the parking spots
    Dictionary<int, bool> parkingSpotStatuses;

    //Checks if all the spots have been processed
    bool processed;

    //Text displaying the open spots
    public UnityEngine.UI.Text openSpotsText;

    //Spot Not Open Color
    public Color parkingSpotFilledMat = Color.red;

    //Spot Open Color
    public Color parkingSpotEmptyMat = Color.green;

    //List of all the spots for later use
    public UnityEngine.UI.Image[] ParkingSpotButtons;

    //Directory in which the file is located
    public string directoryLocation = "C://Users//Arg0s2//Documents//";

    //The file name
    public string scriptName = "ParkingStatuses";

    //File type of the file
    public string fileType = ".yml";

    void Start()
    {
        //Create a instance of parkingSpotStatuses
        parkingSpotStatuses = new Dictionary<int, bool>();

        //Set processed to false
        processed = false;
    }

    void Update()
    {
        processed = false;
        //Every update while processed is false
        if (processed == false)
        {
            //Get the status of all the parking spots
            GetParkingSpotStatuses();
        }
    }

    void GetParkingSpotStatuses()
    {
        //status instance
        bool status;

        StreamReader reader = new StreamReader(directoryLocation + scriptName + fileType, true);

        while (!reader.EndOfStream)
        {
            string currentParkingSpot = reader.ReadLine();
            int[] numbers = (from Match m in Regex.Matches(currentParkingSpot, @"-?\d+") select int.Parse(m.Value)).ToArray();
            int ParkingSpotID = numbers[0];
            //Debug.Log("Id: " + numbers[0]);
            int statusOfParkingSpot = numbers[1];
            //Debug.Log("Status: " + numbers[1]);

            //If statusID is 0
            if (statusOfParkingSpot == 0)
            {
                //Set status to false
                status = false;
            }

            else
            {
                //Else set it to true
                status = true;
            }

            //If parkingSpotStatuses contains the key i then 
            if (!parkingSpotStatuses.ContainsKey(ParkingSpotID))
            {
                //Add it to the statuses with the ID i
                //And the status generated
                parkingSpotStatuses.Add(ParkingSpotID, status);
            }

            //If parkingSpotStatuses does contain the key i
            else
            {
                //Set the status of key i to the generated status
                parkingSpotStatuses[ParkingSpotID] = status;
            }

            //Change the color of the parkingspot based on the status
            ChangeParkingSpotStatus(parkingSpotStatuses, ParkingSpotID);
        }

        //Display the amount of spots open
        ChangeSpotsOpen(openSpotsText, CalculateOpenSpots(parkingSpotStatuses));
        reader.Close();
        //Processing is done
        //processed = true;
    }

    //Calculate the open spots
    int CalculateOpenSpots(Dictionary<int, bool> spots)
    {
        //spotOpenCount instance is 0 
        int spotOpenCount = 0;

        //Go through the list of spots
        for (int i = 0; i < spots.Count; i++)
        {
            //If the key i of spots is false
            if (spots[i] == false)
            {
                //Then add to the open spots count
                spotOpenCount += 1;
            }
        }
        //After running through the list return the open spots count
        return spotOpenCount;
    }

    //Change the text based off of the int numberOpen
    void ChangeSpotsOpen(UnityEngine.UI.Text spotsOpenText, int numberOpen)
    {
        //Set the open spots text to the int numberOpen
        string openSpots = "" + numberOpen;

        //Set the display text to show the number of open spots
        spotsOpenText.text = "Open#: " + openSpots;
    }

    //This changes the color of the spots based on status of the index
    void ChangeParkingSpotStatus(Dictionary<int, bool> statuses, int index)
    {
        //If the list contains the index and it is true
        if (statuses[index] == true)
        {
            //Set the color to the spot closed color
            ParkingSpotButtons[index].color = parkingSpotFilledMat;
        }

        //If the list contains the index and it is false
        else
        {
            //Set the color to the spot open color
            ParkingSpotButtons[index].color = parkingSpotEmptyMat;
        }
    }

    //Allows the user to refresh the parking spot status
    public void Refresh()
    {
        //Allow processing 
        processed = false;
    }
}
