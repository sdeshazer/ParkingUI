using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class LoadLinkInfo : MonoBehaviour {
    //Admin username to access files
    public string userName = "parkingapp";

    //Admin password to access files
    public string password = "opencv2018";

    //Link in which we are getting the files from
    public string url = "http://www.lcccodingclub.org/parkingapp/ParkingStatuses.yml";

    //Statuses of all the parking spots
    Dictionary<int, bool> parkingSpotStatuses;

    //Text displaying the open spots
    public UnityEngine.UI.Text openSpotsText;

    //Spot Not Open Color
    public Color parkingSpotFilledMat = Color.red;

    //Spot Open Color
    public Color parkingSpotEmptyMat = Color.green;

    //List of all the spots for later use
    public UnityEngine.UI.Image[] ParkingSpotButtons;

    //Private variable holding the text in the recieved document
    string text;

    void Start()
    {
        parkingSpotStatuses = new Dictionary<int, bool>();
        StartCoroutine(GetText());
    }

    void Update()
    {
        StartCoroutine(GetText());
    }

    //Authentication function that allows us to get data file
    string Authenticate(string username, string password)
    {
        //Convert the authentication key to a readable string
        string auth = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));

        //And then return that string
        return auth;
    }


    IEnumerator GetText()
    {
        WaitForSeconds waitTime = new WaitForSeconds(2f); //Do the memory allocation once

        //Get the authentication key
        string authorization = Authenticate(userName, password);

        while (true)
        {
            //Wait for memory to be allocated
            yield return waitTime;

            //Get the url
            string path = url;

            //Send a webrequest to the url
            UnityWebRequest www = UnityWebRequest.Get(path);

            //Send a authentication request to the url
            www.SetRequestHeader("Authorization", authorization);

            //Wait for the request to be sent
            yield return www.SendWebRequest();

            //If the request didnt go through or another error occurred
            if (www.isNetworkError)
            {
                //Show us the error
                Debug.Log("Error while Receiving: " + www.error);
            }

            //Else
            else
            {
                //Get the result data
                string result = www.downloadHandler.text;

                //Set our readable text as the data
                text = result;

                //Change the status of our parking spots
                ChangeParkingStatus();
            }
        }
    }

    //Parse the data file so it can be read
    List<string> GetTextLines()
    {
        //Split the text file into seperate strings if there is a comma
        string[] textLines = text.Split(',');

        //Call an empty list
        List<string> newLines = new List<string>();

        //Loop through the array of strings that was made from the data file
        for (int i = 0; i < textLines.Length; i++)
        {
            //If the data is not an empty line
            if (textLines[i] != string.Empty)
            {
                //Add it to our list
                newLines.Add(textLines[i]);
            }
        }

        //After all the lines are added return the new list of data
        return newLines;
    }

    void ChangeParkingStatus()
    {
        //Loop through the newly gathered data strings
        for (int i = 0; i < GetTextLines().Count; i++)
        {
            //status instance
            bool status;

            //Current spot being read
            string currentParkingSpot = GetTextLines()[i];

            //If the currently read parking spot is not actually an empty string
            if (currentParkingSpot != string.Empty)
            {
                //Parse the string into substrings of an id and value
                int[] numbers = (from Match m in Regex.Matches(currentParkingSpot, @"-?\d+") select int.Parse(m.Value)).ToArray();

                //If the numbers array contains an id and value or it is greater than 1
                if (numbers.Length > 1)
                {
                    //Set our parking spot id
                    int ParkingSpotID = numbers[0];
                    Debug.Log("Id: " + numbers[0]);

                    //If the parking spot id is greater the existing buttons amount
                    if (ParkingSpotID >= ParkingSpotButtons.Length)
                    {
                        //Tell us to add more buttons
                        Debug.LogError("Need More Buttons! Current Button Missing: " + ParkingSpotID);
                        break;
                    }

                    //If we have enough buttons:

                    //Get the status of the parking spot
                    int statusOfParkingSpot = numbers[1];
                    Debug.Log("Status: " + numbers[1]);

                    //If statusID is 0
                    if (statusOfParkingSpot == 0)
                    {
                        //Set status to false
                        status = false;
                    }

                    else if (statusOfParkingSpot == 1)
                    {
                        //Else set it to true
                        status = true;
                    }

                    else
                    {
                        status = false;
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
            }
        }

        //Display the amount of spots open
        ChangeSpotsOpen(openSpotsText, CalculateOpenSpots(parkingSpotStatuses));
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
        spotsOpenText.text = "Open Spots: " + openSpots;
    }

    //This changes the color of the spots based on status of the index
    void ChangeParkingSpotStatus(Dictionary<int, bool> statuses, int index)
    {
        //If the button does not contain a null value or an empty value
        if (ParkingSpotButtons[index] != null)
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

        //If the button is empty or null
        else
        {
            //Tell us it is
            Debug.LogError("This ParkingSpot is not set!" + "Unset Button Index: " + index);
        }
    }
}
