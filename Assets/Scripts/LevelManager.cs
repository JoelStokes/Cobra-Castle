using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Logic for the specific level
//Contains Objects color management, Door, Arrow, & scene's Text
public class LevelManager : MonoBehaviour
{
    public GameObject ExitDoor;
    public GameObject ExitArrow;
    public TextMeshPro MiceText;
    public TextMeshPro LivesText;
    public TextMeshPro ScoreText;
    public GameObject TransitionObjs;
    public TextMeshPro LevelName;
    public Sprite OpenDoor;

    private GameObject Camera;

    public bool isLabyrinth = false;    //Gamemode of Level
    private float transitionX = 19; //X Position Transition Object warps to on right side

    private GameManager gameManager;
    private CameraScroll cameraScroll;

    void Start(){    
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cameraScroll = GameObject.Find("Main Camera").GetComponent<CameraScroll>();

        if (LevelName){
            LevelName.SetText(gameManager.GetLevelName());
        }
    }

    public void SetColor(Color FGColor, Color BGColor, Color FarColor){
        GameObject[] FGObjects = GameObject.FindGameObjectsWithTag("FG");
        GameObject[] BGObjects = GameObject.FindGameObjectsWithTag("BG");
        GameObject[] FarObject = GameObject.FindGameObjectsWithTag("Far");

        for (int i=0; i<FGObjects.Length; i++){
            FGObjects[i].GetComponent<SpriteRenderer>().color = FGColor;
        }

        for (int i=0; i<BGObjects.Length; i++){
            BGObjects[i].GetComponent<SpriteRenderer>().color = BGColor;
        }

        for (int i=0; i<FarObject.Length; i++){
            FarObject[i].GetComponent<SpriteRenderer>().color = FarColor;
        }    
    }

    public void UpdateMiceCount(int miceCount){
        if (!isLabyrinth){
            if (miceCount > 9){
                MiceText.SetText(miceCount + " Mice Left");
            } else if (miceCount > 1){
                MiceText.SetText("0" + miceCount + " Mice Left");
            } else if (miceCount == 1) {
                MiceText.SetText("1 Mouse Left");
            } else {
                MiceText.SetText("Exit Opened!");
                OpenExitDoor();
            }
        } else {
            MiceText.SetText("Escape!");
        }
    }

    public void UpdateScore(int newScore){
        ScoreText.SetText(newScore.ToString("000000"));
    }

    public void UpdateLives(int newLives){
        LivesText.SetText(newLives.ToString("0"));
    }

    private void OpenExitDoor(){
        ExitDoor.GetComponent<SpriteRenderer>().sprite = OpenDoor;
        ExitArrow.SetActive(true);
    }

    public void BeginEndAnim(){
        cameraScroll.BeginEndAnim();

        gameManager.SetNextLevel(isLabyrinth);
        LevelName.SetText(gameManager.GetLevelName());

        TransitionObjs.transform.position = new Vector3(transitionX,
            TransitionObjs.transform.position.y, TransitionObjs.transform.position.z);
    }
}
