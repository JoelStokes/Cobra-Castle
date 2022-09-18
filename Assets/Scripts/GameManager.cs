using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Persist Level to Level to handle full game flow
public class GameManager : MonoBehaviour
{
    //Score values
    private int score = 0;
    private int totalMice = 0;
    private int totalGoldenMice = 0;
    private int totalDoors = 0;

    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void AddScore(){
        
    }
}
