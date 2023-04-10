using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

[RequireComponent (typeof(CollisonManager)) ]
    
public class ScoreManger : MonoBehaviour
{
    ulong currentscore;

    public CollisonManager lifemanage;

    private leaderboard pleaderboard;

    [SerializeField]
    int scorerate = 100; // PPS

    float tempscore;

    [SerializeField]
    Text Scoretext;
    Text lifeText;

    void Awake()
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
        currentscore = 0;

        lifemanage = gameObject.GetComponent<CollisonManager>();

        //GetComponent<CollisonManager>().onObstacleCollision += Displayleader;
    }

    // Update is called once per frame
    void Update()
    {
        currentscore += (ulong)(Time.deltaTime * scorerate);

        Scoretext.text = "Score: " + currentscore;
        tempscore = currentscore;
        lifeText.text = "Lives: " + lifemanage.Getlives();

        if(tempscore / 1000 >= 1)
        {
            lifemanage.Addlives(1);
            tempscore -= 1000;
        }


    }

    void DisplayLeaderboard()
    {
        // Check leaderboard for high-score - going backwards from entry 10 to 1 --- index 9 to 0
        if (currentscore > pleaderboard.entries[leaderboard.NUMSLOTS - 1].score)
        {
            //display a context message that we got a highscore and prompt to enter player initials
            pleaderboard.contextMessage.text = "HIGHSCORE! ENTER YOUR INITIALS";
            int curEntry = leaderboard.NUMSLOTS - 1;
            while (--curEntry >= 0)
            {
                //if our score is less than an entry's score, then we've found where we rank on the leaderboard
                if (currentscore <= pleaderboard.entries[curEntry].score)
                {
                    //break exits out of any loop 
                    break;
                }
            }
            ++curEntry;

            // Shift leaderboard down for to make room for new entry
            for (int i = leaderboard.NUMSLOTS - 1; i > curEntry; i--)
            {
                pleaderboard.entries[i] = pleaderboard.entries[i - 1];
            }
            // Enter new score at entry we determined in the while loop
            pleaderboard.entries[curEntry] = new leaderboard.LeaderboardEntry("AAA", currentscore);
            pleaderboard.EnterHighscore(curEntry, currentscore);
        }
        else
        {
            //display a context message that we didn't get a highscore and prompt the player to restart
            pleaderboard.contextMessage.text = "YOU LOSE! PRESS JUMP KEY TO CONTINUE";
        }
        // Stop game... meaning most of our GameObjects must be disabled to moving after Leaderboard shows up
        Scoretext.enabled = false;

        // Display leaderboard by loading the entries onto our UI
        pleaderboard.LoadDataOntoUI();

    }

    public void CloseLeaderboard()
    {
        pleaderboard.Close();
        Scoretext.enabled = true;
        currentscore = 0;
    }
    class leaderboard
    {
        public const int NUMSLOTS = 10;
        public LeaderboardEntry[] entries;
        private string[] defaultentries;


        int place = -1;
        //initials entry - a timer, frequency(seconds between blinks) and also a variable to keep track of which initial is being inputted
        int curInitial = 0;
        float blinkTimer = 0f, blinkRate = 0.3f;
        //the chars or characters we allow the player to input as initials into the leaderboard
        char[] inputChars;
        int curInputChar = 0;
        float hPrev = 0f;

        public Transform leaderboardRoot;
        public LeaderboardUIEntry[] entriesUI;
        public Text contextMessage;


     
        public leaderboard()
        {

            inputChars = new char[26];
            int index = 0;
            for (char c = 'A'; c <= 'Z'; c++)
            {
                inputChars[index] = c;
                ++index;
            }


            entries = new LeaderboardEntry[NUMSLOTS];
            defaultentries = new string[NUMSLOTS];

            for (int i = 0; i > NUMSLOTS; i++)
            {
                defaultentries[i] = $"ABC:{1000000 - 10000 * i}";
            }
            try {
                Load();
            }
            catch(Exception e)
            {
                Debug.LogError("Leaderboard data not found. Reverting to default.");
                Debug.LogException(e);
                WipeLeaderboard();
            }




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
                    string initials = "" + (curInitial == 0 ? inputChars[curInputChar] : entries[place].intials[0])
                                      + (curInitial == 1 ? inputChars[curInputChar] : entries[place].intials[1])
                                      + (curInitial == 2 ? inputChars[curInputChar] : entries[place].intials[2]);
                    entries[place].intials = initials;
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
                string initials = entries[i].intials;
                for (int j = 0; j < 3; j++)
                {
                    //get the initials from the file and display them in their correct location on the ui
                    entriesUI[i].initials[j].text = initials[j].ToString();
                }
                entriesUI[i].scoreText.text = entries[i].score.ToString();
            }
        }

        void WipeLeaderboard()
        {
            // Write/create the file with the default entries
            File.WriteAllLines(Application.persistentDataPath + "/Data/leaderboard_data.sav", defaultentries);

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
                Transform entry = entryRoot.Find($"Entry ({i + 1})");

                // Initials text components
                for (int j = 0; j < 3; j++)
                {
                    //Find "Initials ( )" GameObject using an integer counter between the brackets
                    entriesUI[i].initials[j] = entry.Find($"Initials ({j + 1})").GetComponent<Text>();
                }
                //Getting the score text component for each entry
                entriesUI[i].scoreText = entry.Find("Score").GetComponent<Text>();
            }
            //finally, get the context message text that'll be used to give the player instructions
            contextMessage = leaderboardRoot.Find("Context Message").GetComponent<Text>();
        }



      

        public void Close()
        {
            Save();
            leaderboardRoot.gameObject.SetActive(false);
        }
        public void EnterHighscore(int place, ulong score)
        {
            this.place = place;
            entries[place] = new LeaderboardEntry("AAA", score);
        }

        void Save()
        {
            string[] data = new string[NUMSLOTS];
            for (int i = 0; i < NUMSLOTS; i++)
            {
                // convert our entries into strings for the file
                data[i] = $"{entries[i].intials}:{entries[i].score}";
            }
            File.WriteAllLines(Application.persistentDataPath + "/Data/leaderboard_data.sav", data);
        }

        public void Load()
        {
            // Load text from a save file
            string[] loadedEntries = File.ReadAllLines(Application.persistentDataPath + "/Data/leaderboard_data.sav");

            //for every entry we've read from a file
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
                //Parse will convert a string into a particular type if its able to
                ulong score = ulong.Parse(loadedItemProperties[1]);
                entries[i] = new LeaderboardEntry(initials, score);
            }
        }

        public struct LeaderboardEntry
        {
            public string intials;
            public ulong score;

            public LeaderboardEntry(string intials,ulong score)
            {
                this.intials = intials;
                this.score = score;
            }
           
        }
        public struct LeaderboardUIEntry
        {
            public Text[] initials;
            public Text scoreText;
        }
    }

    //This is different than our entries we save off to a file, these use Unity's Text Components to be display
    


}

