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

    public bool isLabyrinth = false;    //Gamemode of Level

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
            //Have door open at start, mice eaten does not change UI
        }
    }

    public void UpdateScore(int newScore){
        ScoreText.SetText(newScore.ToString("0000000"));
    }

    public void UpdateLives(int newLives){
        LivesText.SetText(newLives.ToString("0"));
    }

    private void OpenExitDoor(){
        ExitArrow.SetActive(true);
    }
}
