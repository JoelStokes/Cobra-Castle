using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleController : MonoBehaviour
{
    public float screenAnimEdge;

    public TextMeshPro prevScore;
    public TextMeshPro highScore;
    public TextMeshPro levelName;

    private GameManager gameManager;
    private CameraScroll cameraScroll;

    private bool animating = false;

    void Start(){   //Load previous score & highest score
        prevScore.SetText("Prev: " + PlayerPrefs.GetInt("Prev Score").ToString("000000"));
        highScore.SetText("High: " + PlayerPrefs.GetInt("High Score").ToString("000000"));

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cameraScroll = GameObject.Find("Main Camera").GetComponent<CameraScroll>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
             && !animating){   //Start Scrolling
            BeginEndAnim();
        }
    }

    private void BeginEndAnim(){
        gameManager.SetNextLevel(true);
        levelName.SetText(gameManager.GetLevelName());
        animating = true;
        cameraScroll.ChangeEndPos(screenAnimEdge);
        cameraScroll.ChangeCurrentPos(0f);

        cameraScroll.BeginEndAnim();
    }
}
