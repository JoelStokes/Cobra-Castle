using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Logic for current game.
//PlayerController reaches out with updates, then GameManager contacts LevelManager to display updates
public class GameManager : MonoBehaviour
{
    //Score values
    private int score = 0;
    private int totalMice = 0;
    private int totalGoldenMice = 0;
    private int totalDoors = 0;

    private int miceValue = 10;
    private int goldenMiceValue = 25;
    private int doorValue = 75;

    private int floorMultiplier = 1;    //Currently unused, should add higher score counts for higher floors?

    private LevelManager currentLevelManager;

    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) //Get new Scene's LevelManager
    {
        currentLevelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    public void AddMouse(){
        totalMice++;
        UpdateScore();
    }

    public void AddGoldenMouse(){
        totalGoldenMice++;
        UpdateScore();
    }

    public void AddDoor(){
        totalDoors++;
        UpdateScore();
    }

    public void LoadNewScene(){ //Choose random scene to boot next
        if(){

        }
    }

    private void UpdateScore(){
        score = (totalMice * miceValue) + (totalGoldenMice * goldenMiceValue) + (totalDoors * doorValue);
    }
}
