using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public TextMeshPro MiceCount;
    public TextMeshPro GoldMiceCount;
    public TextMeshPro DoorCount;
    public TextMeshPro ScoreCount;

    // Start is called before the first frame update
    void Start()
    {
        //Animation through score reveal, can hit "Start" to skip to final score reveal?
        //If Animation done or skipped & start pressed, return to Title
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
            SceneManager.LoadScene("Title");
        }

    }

    public void SetTextValues(int mice, int goldMice, int doors, int score){
        MiceCount.SetText(mice.ToString("D2") + " x15");
        GoldMiceCount.SetText(goldMice.ToString("D2") + " x25");
        DoorCount.SetText(doors.ToString("D2") + " x75");
        ScoreCount.SetText("Total: " + score.ToString());
    }
}