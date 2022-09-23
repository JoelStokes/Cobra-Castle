using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Logic for the specific level
//Contains Objects to color, Door, Arrow, & scene's Text
public class LevelManager : MonoBehaviour
{
    public Color[] FGColor; //Color arrays for each levels bright, dark, & darkest color. Array position will match & go up by 1 per floor
    public Color[] BGColor;
    public Color[] FarColor;

    public SpriteRenderer[] FGObjects;
    public SpriteRenderer[] BGObjects;
    public SpriteRenderer[] FarObject;  //Only Farthest BG, UI Blocks

    public GameObject ExitDoor;
    public GameObject ExitArrow;
    public TextMeshPro MiceText;
    public TextMeshPro LivesText;
    public TextMeshPro ScoreText;

    public bool isLabyrinth = false;    //Gamemode of Level

    private int miceCounter = 12;

    // Start is called before the first frame update
    void Start()
    {
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

    public void UpdateMiceCount(){
        miceCounter--;

        if (miceCounter > 9){
            MiceText.SetText(miceCounter + " Mice Left");
        } else if (miceCounter > 1){
            MiceText.SetText("0" + miceCounter + " Mice Left");
        } else if (miceCounter == 1) {
            MiceText.SetText("1 Mouse Left");
        } else {
            MiceText.SetText("Exit Opened!");
            OpenExitDoor();
        }
    }

    public void UpdateScore(){

    }

    public void UpdateLives(){

    }

    private void OpenExitDoor(){
        ExitArrow.SetActive(true);
    }
}
