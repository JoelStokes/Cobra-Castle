using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Logic for the specific level
//Contains Objects to color, Door, Arrow, & scene's Text
public class LevelManager : MonoBehaviour
{
    public SpriteRenderer[] FGObjects;
    public SpriteRenderer[] BGObjects;
    public SpriteRenderer[] FarObject;  //Only Farthest BG, UI Blocks

    public GameObject ExitDoor;
    public GameObject ExitArrow;
    public TextMeshPro MiceText;
    public TextMeshPro LivesText;
    public TextMeshPro ScoreText;

    private GameObject Camera;

    private float animSpeed = .075f;
    private float animTimer = 0;
    private float animEdgePos = 18;
    private float animCurrentPos = 0;
    private bool animating = true;
    private bool startAnim = true;

    public bool isLabyrinth = false;    //Gamemode of Level

    private GameManager gameManager;

    void Start(){
        animCurrentPos = -animEdgePos;

        Camera = GameObject.Find("Main Camera");
        Camera.transform.position = new Vector3(animCurrentPos, Camera.transform.position.y, Camera.transform.position.z);
    
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update(){  //Camera Scroll for start/end of level
        if (animating){
            animTimer += Time.deltaTime;

            if (animTimer >= animSpeed){
                animCurrentPos++;
                Camera.transform.position = new Vector3(animCurrentPos, Camera.transform.position.y, Camera.transform.position.z);

                if ((animCurrentPos < 0 && startAnim) || (animCurrentPos < animEdgePos && !startAnim)){
                    animTimer = 0;
                } else {
                    animating = false;
                    if (startAnim){
                        gameManager.StartPlayer();
                    } else {
                        gameManager.LoadNewLevel(isLabyrinth);
                    }
                }
            }
        }
    }

    public void SetColor(Color FGColor, Color BGColor, Color FarColor){
        for (int i=0; i<FGObjects.Length; i++){
            FGObjects[i].color = FGColor;
        }

        for (int i=0; i<BGObjects.Length; i++){
            BGObjects[i].color = BGColor;
        }

        for (int i=0; i<FarObject.Length; i++){
            FarObject[i].color = FarColor;
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
        ExitArrow.SetActive(true);
    }

    public void BeginEndAnim(){
        startAnim = false;
        animating = true;
    }
}
