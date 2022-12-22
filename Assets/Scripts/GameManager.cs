using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Logic for current game.
//PlayerController reaches out with updates, then GameManager contacts LevelManager to display updates
public class GameManager : MonoBehaviour
{
    public Color[] FGColor; //Color arrays for each levels bright, dark, & darkest color. Array position will match & go up by 1 per floor
    public Color[] BGColor;
    public Color[] FarColor;

    private int lives = 3;
    private int miceStart = 12;
    private int miceRemaining;

    //Score values
    private int score = 0;
    private int totalMice = 0;
    private int totalGoldenMice = 0;
    private int totalDoors = 0;

    private int miceValue = 10;
    private int goldenMiceValue = 25;
    private int doorValue = 75;

    private int floor = 0;    //Currently unused, should add higher score counts for higher floors?
    private float floorSpeed = .05f;
    private bool isLabyrinth = false;
    private bool doorTouched = false;
    private int nextLevel;
    private List<string> LabyrinthLevels = new List<string>();
    private List<string> MouseLevels = new List<string>();

    private LevelManager currentLevelManager;
    private PlayerController currentPlayerController;

    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        PopulateSceneList();
    }

    void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) //Get new Scene's LevelManager
    {
        doorTouched = false;
        if (scene.name != "Title" && scene.name != "GameOver"){ //No Level Info to load on Title or Game Over
            currentLevelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            currentPlayerController = GameObject.Find("Player").GetComponent<PlayerController>();

            currentLevelManager.SetColor(FGColor[floor], BGColor[floor], FarColor[floor]);


            if (currentLevelManager.isLabyrinth){
                miceRemaining = 0;
            } else {
                miceRemaining = miceStart;
            }

            UpdateUI();
        }
    }

    private void PopulateSceneList(){
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i=0; i < sceneCount; i++){
            string path = SceneUtility.GetScenePathByBuildIndex(i); //GetSceneByBuildIndex only works for loaded scenes...
            int slash = path.LastIndexOf('/');
            string uncroppedName = path.Substring(slash + 1);
            int dot = uncroppedName.LastIndexOf('.');
            string name = uncroppedName.Substring(0, dot);
            
            if (name.Contains("L-")){
                LabyrinthLevels.Add(name);
            } else if (name.Contains("M-")){
                MouseLevels.Add(name);
            }
        }
    }

    public void StartPlayer(){
        currentPlayerController.started = true;
    }

    public void AddMouse(){
        totalMice++;
        miceRemaining--;
        currentLevelManager.UpdateMiceCount(miceRemaining);
        UpdateScore();
    }

    public void AddGoldenMouse(){
        totalGoldenMice++;
        miceRemaining--;
        currentLevelManager.UpdateMiceCount(miceRemaining);
        UpdateScore();
    }

    public void AddDoor(){
        if (!doorTouched){  //Prevent repeated levels during transition animation
            totalDoors++;
            UpdateScore();

            if (currentLevelManager.isLabyrinth){
                floor++;
            }

            currentLevelManager.BeginEndAnim();
            doorTouched = true;
        }
    }

    public void AddDeath(){
        //Wait to be called after death animation is played out?

        if (lives > 0){
            lives--;
        } else {
            PlayerPrefs.SetInt("Prev Score", score);

            int highScore = PlayerPrefs.GetInt("High Score");   //If not existant, should return 0
            if (score > highScore){
                PlayerPrefs.SetInt("High Score", score);
            }

            SceneManager.LoadScene("GameOver");
        }
        
        currentLevelManager.UpdateLives(lives);
    }

    private void UpdateUI(){    //Called on new scene load to make sure all variables match correct values
        currentLevelManager.UpdateLives(lives);
        currentLevelManager.UpdateMiceCount(miceRemaining);
        UpdateScore();
    }

    private void UpdateScore(){
        score = (totalMice * miceValue) + (totalGoldenMice * goldenMiceValue) + (totalDoors * doorValue);
        currentLevelManager.UpdateScore(score);
    }

    public void SetNextLevel(bool isLabyrinth){ Loading same level over and over
        if (isLabyrinth){
            nextLevel = Random.Range(0, MouseLevels.Count);
        } else {
            nextLevel = Random.Range(0, LabyrinthLevels.Count);
        }
    }

    public void LoadNewLevel(){
        if (CheckIsLabyrinth()){
            SceneManager.LoadScene(MouseLevels[nextLevel]);
        } else {
            SceneManager.LoadScene(LabyrinthLevels[nextLevel]);
        }
    }

    private void ResetValues(){
        score = 0;
        totalDoors = 0;
        totalMice = 0;
        totalGoldenMice = 0;
        floor = 0;
        miceStart = 12;
        lives = 3;
    }

    public bool CheckMiceFinished(){
        return (miceRemaining <= 0);
    }

    public bool CheckIsLabyrinth(){
        if (currentLevelManager){
            return (currentLevelManager.isLabyrinth);
        } else {
            return true;    //With alternating levels, "true" at start so mouse loads first
        }
    }

    public float GetFloorSpeed(){
        return (floor * floorSpeed);
    }

    public string GetLevelName(){
        string labyrinthLetter;
        string levelName;
        if (isLabyrinth){
            labyrinthLetter = "B";
            levelName = LabyrinthLevels[nextLevel];
        } else {
            labyrinthLetter = "A";
            levelName = MouseLevels[nextLevel];
        }

        string name = (floor+1) + "-" + labyrinthLetter + ": " + levelName.Substring(2,levelName.Length-2);
        return name;
    }
}