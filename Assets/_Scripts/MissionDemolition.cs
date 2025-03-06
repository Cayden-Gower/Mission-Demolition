using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum GameMode {
    idle, 
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; //private singleton

    [Header("Inscribed")]
    public Text uitLevel;
    public Text uitShots;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";


    void Start()
    {
        S = this; //Defines singleton

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }
    
    void StartLevel() {
        //Get rid of old castle if one exists
        if (castle != null) {
            Destroy (castle);
        }

        //Destroy old projectiles if they exist
        Projectile.DESTROY_PROJECTILES();

        //Instantiate new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        //Reset goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        //Zoom out to show both
        FollowCam.SWITCH_VIEW( FollowCam.eView.both);
    }

    void UpdateGUI() {
        //Show the data in the GUI Texts
        uitLevel.text = "Level: " +(level+1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }


    void Update()
    {
        //Check for level end
        if ( ( mode == GameMode.playing) && Goal.goalMet ) {
            //Change mode to stop checking for levelend
            mode = GameMode.levelEnd;

            //Zoom out to show both
            FollowCam.SWITCH_VIEW( FollowCam.eView.both);

            //Start next level in 2 seconds
            Invoke ("NextLevel", 2f);
        }
    }

    void NextLevel() {
        level++;
        if (level == levelMax) {
            level = 0;
            shotsTaken = 0;
            SceneManager.LoadScene("_Scene_1");
        }
        StartLevel();
    }

    //Static method that allows code anywhere to increment shotsTaken
    static public void SHOT_FIRED() {
        S.shotsTaken++;
    }

    //Static method that allows code anywhere to get a reference to S.castle
    static public GameObject GET_CASTLE() {
        return S.castle;
    }

}
