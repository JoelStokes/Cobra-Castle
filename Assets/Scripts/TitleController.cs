using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleController : MonoBehaviour
{
    public TextMeshPro prevScore;
    public TextMeshPro highScore;

    void Start(){   //Load previous score & highest score
        prevScore.SetText("Prev: " + PlayerPrefs.GetInt("Prev Score").ToString("000000"));
        highScore.SetText("High: " + PlayerPrefs.GetInt("High Score").ToString("000000"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
            GameObject.Find("GameManager").GetComponent<GameManager>().LoadNewLevel(true);
        }
    }

    //Need to start animation so Title looks like regular level transitions
    //21.5 for scene change
}
