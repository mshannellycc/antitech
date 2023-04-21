using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

//This notifies Unity that in order for ScoreManager to work there must exist a CollisionManager attached to the same gameobject
[RequireComponent(typeof(CollisonManager))]
public class ScoreManager : MonoBehaviour
{

    ulong currentScore;
    ulong tempscore;

    // Inspector parameters
    [SerializeField]
    public int scoreRate = 100;
    // Points gained per second
    [SerializeField]
    Text playerScoreText;

    public GameObject Mushroom;

    [SerializeField]
    Text lifeText;

    CollisonManager lifemanage;
    MarksRoads speedmanage;

    private Leaderboard leaderboard;

    public int lanes = 3;

    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Data"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Data");
        }
        // Subscribe game reset code to obstacle collision event
        GetComponent<CollisonManager>().outoflives += DisplayLeaderboard;

    }

    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
        leaderboard = new Leaderboard();
        lifemanage = gameObject.GetComponent<CollisonManager>();
        speedmanage = gameObject.GetComponent<MarksRoads>();
        Init();
    }

    public ulong getScore()
    {
        return currentScore;
    }

    public void addlives(int add)
    {
        lifemanage.Addlives(1);
    }
    void Init()
    {
        //
        Transform canvas = transform.Find("Canvas");
        leaderboard.InitUI(canvas);
        
    }


   

    // Update is called once per frame
    void Update()
    {
       // PropLogic laneread = speedmanage.roadObjects[speedmanage.roadObjects.Count].GetComponent<PropLogic>();
        //you can check if the gameobject in the scene is active by using GameObject.ActiveSelf
        if (leaderboard.leaderboardRoot.gameObject.activeSelf)
        {
            leaderboard.Update();
        }
        else
        {
           // Debug.Log("ScoreUpdate!" + currentScore.ToString());
            //We will up the rate per second, casting to a ulong
            currentScore += (ulong)(Time.deltaTime * scoreRate * (speedmanage.roadSpeed));
            playerScoreText.text = "SCORE: " + currentScore.ToString("000000");
            tempscore += (ulong)(Time.deltaTime * scoreRate * (speedmanage.roadSpeed));
            lifeText.text = "Lives: " + lifemanage.Getlives();

           if (tempscore /1000 >= 1){
                SoundScript.playeffect("playerJumpSound");
                tempscore -= 1000;
            }

        }
    }

    void DisplayLeaderboard()
    {
        // Check leaderboard for high-score - going backwards from entry 10 to 1 --- index 9 to 0
        if (currentScore > leaderboard.entries[Leaderboard.NUMSLOTS - 1].score)
        {
            //display a context message that we got a highscore and prompt to enter player initials
            leaderboard.contextMessage.text = "HIGHSCORE! ENTER YOUR INITIALS";
            int curEntry = Leaderboard.NUMSLOTS - 1;
            while (--curEntry >= 0)
            {
                //if our score is less than an entry's score, then we've found where we rank on the leaderboard
                if (currentScore <= leaderboard.entries[curEntry].score)
                {
                    //break exits out of any loop 
                    break;
                }
            }
            ++curEntry;

            // Shift leaderboard down for to make room for new entry
            for (int i = Leaderboard.NUMSLOTS - 1; i > curEntry; i--)
            {
                leaderboard.entries[i] = leaderboard.entries[i - 1];
            }
            // Enter new score at entry we determined in the while loop
            leaderboard.entries[curEntry] = new Leaderboard.LeaderboardEntry("AAA", currentScore);
            leaderboard.EnterHighscore(curEntry, currentScore);
        }
        else
        {
            //display a context message that we didn't get a highscore and prompt the player to restart
            leaderboard.contextMessage.text = "YOU LOSE! PRESS JUMP KEY TO CONTINUE";
        }
        // Stop game... meaning most of our GameObjects must be disabled to moving after Leaderboard shows up
        playerScoreText.enabled = false;

        // Display leaderboard by loading the entries onto our UI
        leaderboard.LoadDataOntoUI();

    }

    public void CloseLeaderboard()
    {
        leaderboard.Close();
        playerScoreText.enabled = true;
        currentScore = 0;
    }

    // Leaderboard data
    class Leaderboard
    {
        public const int NUMSLOTS = 6;
        
        public LeaderboardEntry[] entries;
        private string[] defaultEntries;
        //place of the new highscore - by default its -1 to say there is no new highscore
        int place = -1;
        //initials entry - a timer, frequency(seconds between blinks) and also a variable to keep track of which initial is being inputted
        int curInitial = 0;
        float blinkTimer = 0f, blinkRate = 0.3f;
        //the chars or characters we allow the player to input as initials into the leaderboard
        char[] inputChars;
        int curInputChar = 0;
        float hPrev = 0f;

        // User Interface
        //Keep track of our leaderboard's gameobject to perform a find child components we need
        public Transform leaderboardRoot;
        public LeaderboardUIEntry[] entriesUI;
        public Text contextMessage;

        //Create a public constructor, but this class is private, meaning only ScoreManager can create a new leaderboard
        public Leaderboard()
        {
            
            // Initialize input characters in array
            inputChars = new char[26];
            int index = 0;
            for (char c = 'A'; c <= 'Z'; c++)
            {
                inputChars[index] = c;
                ++index;
            }

            entries = new LeaderboardEntry[NUMSLOTS];
            defaultEntries = new string[NUMSLOTS];
            //create a default leaderboard filled with the initials of the player and their score
            for (int i = 0; i < NUMSLOTS; i++)
            {
                defaultEntries[i] = $"ABC:{100000 - 10000*i}";
            }

            try
            {
                Load();
            }
            catch (Exception e)
            {
                Debug.LogError("Leaderboard data not found. Reverting to default.");
                Debug.LogException(e);
                WipeLeaderboard();
            }
            
        }

        public void InitUI(Transform canvas)
        {
            // Initialize entries - we have a top 10 entry
            entriesUI = new LeaderboardUIEntry[NUMSLOTS];
            for (int i = 0; i < entriesUI.Length; i++)
            {
                //each entry has only 3 initials
                entriesUI[i].initials = new Text[3];
            }
            //use the canvas to find the Leaderboard UI and then our entries
            leaderboardRoot = canvas.Find("GameScreen").Find("Leaderboard_UI");
            Transform entryRoot = leaderboardRoot.Find("Entries");

            //for every entry on our "Leaderboard_UI"
            for (int i = 0; i < entriesUI.Length; i++)
            {
                //Find "Entry ( )" GameObject using an integer counter between the brackets
                Transform entry = entryRoot.Find($"Entry ({i+1})");

                // Initials text components
                for (int j = 0; j < 3; j++)
                {
                    //Find "Initials ( )" GameObject using an integer counter between the brackets
                    entriesUI[i].initials[j] = entry.Find($"Initials ({j+1})").GetComponent<Text>();
                }
                //Getting the score text component for each entry
                entriesUI[i].scoreText = entry.Find("Score").GetComponent<Text>();
            }
            //finally, get the context message text that'll be used to give the player instructions
            contextMessage = leaderboardRoot.Find("Context Message").GetComponent<Text>();
        }

        public void EnterHighscore(int place, ulong score)
        {
            this.place = place;
            entries[place] = new LeaderboardEntry("AAA", score);
        }

        public void Close()
        {
            Save();
            leaderboardRoot.gameObject.SetActive(false);
        }

        public void Update()
        {
            //if we did not get a place on the leaderboard
            if (place < 0)
            {
                //depending on if you're using the new input system (left) or old (right)
                if (playerController.Instance.jump == 1 || Input.GetButtonDown(inputNames.jumpAxis))
                {
                    //Reset level
                   //GameManager.Instance.ResetLevel();
                }
            }
            else
            {
                blinkTimer += Time.deltaTime;
                //blinking initials on or off every blinkRate
                if (blinkTimer >= blinkRate)
                {
                    //reset timer
                    blinkTimer = 0;
                    // toggle initial visibility
                    entriesUI[place].initials[curInitial].enabled = !entriesUI[place].initials[curInitial].enabled;
                }

                float hDelta = playerController.Instance.hNew - hPrev;
                if (Mathf.Abs(hDelta) > 0.3f && Mathf.Abs(playerController.Instance.hNew) > 0f)
                {
                    // go to next/prev character
                    int direction = Mathf.RoundToInt(playerController.Instance.hNew);
                    curInputChar = Helpers.Mod(curInputChar + direction, inputChars.Length);
                    entriesUI[place].initials[curInitial].text = inputChars[curInputChar].ToString();
                }
                if (playerController.Instance.jump == 1 || Input.GetButtonDown(inputNames.jumpAxis))
                {
                    string initials = "" + (curInitial == 0 ? inputChars[curInputChar] : entries[place].initials[0])
                                      + (curInitial == 1 ? inputChars[curInputChar] : entries[place].initials[1])
                                      + (curInitial == 2 ? inputChars[curInputChar] : entries[place].initials[2]);
                    entries[place].initials = initials;
                    entriesUI[place].initials[curInitial].enabled = true;

                    //when the player has entered in all initials for their highscore, we prompt for exiting highscores
                    // move to next initial
                    if (++curInitial > 2)
                    {
                        // finished entering initials
                        curInitial = 0;
                        place = -1;
                        //save the score to the file
                        Save();
                        //prompt user to continue with a context message
                        contextMessage.text = "NICE! PRESS JUMP TO CONTINUE";
                    }
                }
                else if (playerController.Instance.slide == 1 || Input.GetButtonDown(inputNames.slideAxis))
                {
                    entriesUI[place].initials[curInitial].enabled = true;
                    curInitial = Mathf.Clamp(curInitial - 1, 0, 3);
                }

                hPrev = playerController.Instance.hPrev;
            }
        }

        public void LoadDataOntoUI()
        {
            // "Leaderboard_UI" GameObject must be active or enabled in our Unity Scene to be visible
            leaderboardRoot.gameObject.SetActive(true);
            // Set entries' text - for every entry from index 0 to 9
            for (int i = 0; i < entriesUI.Length; i++)
            {
                string initials = entries[i].initials;
                for (int j = 0; j < 3; j++)
                {
                    //get the initials from the file and display them in their correct location on the ui
                    entriesUI[i].initials[j].text = initials[j].ToString();
                }
                entriesUI[i].scoreText.text = entries[i].score.ToString();
            }
        }

        // Wipes leaderboard to default state
        void WipeLeaderboard()
        {             
            // Write/create the file with the default entries
            File.WriteAllLines(Application.persistentDataPath + "/Data/leaderboard_data.sav", defaultEntries);

            //Attempt to load in the file again from the persistent data location
            try
            {
                Load();
            } 
            catch (Exception e)
            {
                Debug.LogError("Could not revert to default leaderboard data. Something is seriously wrong.");
                Debug.LogException(e);
            }
        }

        public void Load()
        {
            // Load text from a save file
            string[] loadedEntries = File.ReadAllLines(Application.persistentDataPath + "/Data/leaderboard_data.sav");
            
            for (int i = 0; i < loadedEntries.Length; i++)
            {
                // components of an entry are separated by ':'
                string[] loadedItemProperties = loadedEntries[i].Split(':');
                /* 
                 * LOADED ITEM PROPERTIES:
                 * [0] = Initials
                 * [1] = Score
                */
                string initials = loadedItemProperties[0];
                ulong score = ulong.Parse(loadedItemProperties[1]);
                entries[i] = new LeaderboardEntry(initials, score);
            }
        }

        // Overwrites leaderboard data with current leaderboard standings
        void Save()
        {
            string[] data = new string[NUMSLOTS];
            for (int i = 0; i < NUMSLOTS; i++)
            {
                // convert our entries into strings for the file 
                data[i] = $"{entries[i].initials}:{entries[i].score}";
            } 
            File.WriteAllLines(Application.persistentDataPath + "/Data/leaderboard_data.sav", data);
        }

        //Each entry will be made out of a struct, which is a data type
        public struct LeaderboardEntry
        {
            public string initials;
            public ulong score;
            //structs can also have constructors!
            public LeaderboardEntry(string initials, ulong score)
            {
                this.initials = initials;
                this.score = score;
            } 
        }

        //This is different than our entries we save off to a file, these use Unity's Text Components to be display
        public struct LeaderboardUIEntry
        {
            public Text[] initials;
            public Text scoreText;
        }
    }
}
