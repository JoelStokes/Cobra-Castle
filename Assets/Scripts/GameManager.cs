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

    public Scene[] MouseRooms;
    public Scene[] LabyrinthRooms;

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

    private LevelManager currentLevelManager;

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
        currentLevelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        
        currentLevelManager.SetColor(FGColor[floor], BGColor[floor], FarColor[floor]);

        miceRemaining = miceStart;
        UpdateUI();
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
    }

    public void AddDeath(){
        //Wait to be called after death animation is played out?

        if (lives > 0){
            lives--;
        } else {
            //Game Over Sequence
        }
        
        currentLevelManager.UpdateLives(lives);
    }

    public void LoadNewScene(){ //Choose random level to boot next from either mouse or labyrinth pool
        if(currentLevelManager.isLabyrinth){

        } else {

        }
    }

    private void UpdateUI(){    //Called on new scene load to make sure all variables match correct values
        currentLevelManager.UpdateLives(lives);
        currentLevelManager.UpdateMiceCount(miceRemaining);
        UpdateScore();
    }

    private void UpdateLives(){

    }

    private void UpdateScore(){
        score = (totalMice * miceValue) + (totalGoldenMice * goldenMiceValue) + (totalDoors * doorValue);
        currentLevelManager.UpdateScore(score);
    }

    public void LoadNewLevel(bool isLabyrinth){ //Find way to contain scene names since you can't use real Scene variables (would be wasteful)
        if (isLabyrinth){
            int level = Random.Range(0,LabyrinthRooms.Length);
            SceneManager.LoadScene(LabyrinthRooms[level]);
        } else {
            int level = Random.Range(0,MouseRooms.Length);
            SceneManager.LoadScene(MouseRooms[level]);
        }
    }
}
