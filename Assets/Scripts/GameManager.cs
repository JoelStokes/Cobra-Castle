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

    public int mouseSceneStart;
    public int labyrinthScenesStart;

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
    private int nextLevel;

    private LevelManager currentLevelManager;
    private PlayerController currentPlayerController;

    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) //Get new Scene's LevelManager
    {
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
        totalDoors++;
        UpdateScore();

        if (currentLevelManager.isLabyrinth){
            floor++;
        }

        currentLevelManager.BeginEndAnim();
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

    public void SetNextLevel(bool isLabyrinth){
        if (isLabyrinth){
            nextLevel = Random.Range(mouseSceneStart, labyrinthScenesStart);
        } else {
            nextLevel = Random.Range(labyrinthScenesStart, SceneManager.sceneCountInBuildSettings);
        }
    }

    public void LoadNewLevel(){ //Scene #1 is Title, #2 is Game Over
        if (nextLevel == 0){
            SetNextLevel();
        }

        SceneManager.LoadScene(nextLevel);
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
        return (currentLevelManager.isLabyrinth);
    }

    public float GetFloorSpeed(){
        return (floor * floorSpeed);
    }

    public string GetLevelName(){
        string labyrinthLetter;
        if (isLabyrinth){
            labyrinthLetter = "B";
        } else {
            labyrinthLetter = "A";
        }

        string name = (floor+1) + "-" + labyrinthLetter + ": " + nextLevel; Swap to Level Naming system
        return name;
    }
}