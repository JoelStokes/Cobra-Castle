using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Add sprites for turns to update body pieces based on direction
    public GameObject BodyPrefab;
    public Vector2 SpawnLocation;

    private int lives = 3;
    private int bodyLength = 4;
    private List<GameObject> Body = new List<GameObject>(); //All connected body parts
    private List<Vector2> previousMoves = new List<Vector2>();  //List of previous moves to apply to body parts

    private float moveLimit;    //Modifier for how quickly the countdown is to the next move
    private float moveCounter;

    //Score Variables
    private int miceEaten;
    private int goldMiceEaten;
    private int doorsEntered;

    void Start()
    {
        for (int i=0; i<bodyLength; i++){   //Create initial body
            GameObject BodySegment = GameObject.Instantiate(BodyPrefab, transform.position, Quaternion.identity);
            Body.Add(BodySegment);
        }

        Spawn();
    }

    //Add some sort of input buffer system, cache the next move to prevent

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            CheckMoveLocation(1, 0);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            CheckMoveLocation(-1,0);
        } else if (Input.GetKeyDown(KeyCode.UpArrow)){
            CheckMoveLocation(0,1);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)){
            CheckMoveLocation(0,-1);
        }
    }

    private void Spawn(){   //Place player at start location with body trailing behind
        transform.position = new Vector3(SpawnLocation.x, SpawnLocation.y, transform.position.z);
        previousMoves.Insert(0, SpawnLocation);

        for (int i=0; i<bodyLength; i++){
            Body[i].transform.position = new Vector3(SpawnLocation.x + ((i*-1)-1), SpawnLocation.y, transform.position.z);
            previousMoves.Add(new Vector2(SpawnLocation.x + ((i*-1)-1), SpawnLocation.y));
        }
    }

    private void CheckMoveLocation(int newX, int newY){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(newX,newY), 1);

        if (hit.collider == null){  //Empty Space, free to move
            PerformMove(newX, newY);
        } else if (hit.transform.tag == "Mouse") {    //Move & Eat mouse
            Destroy(hit.transform.gameObject);
            IncreaseBody();
            PerformMove(newX, newY);
        } else if (hit.transform.tag == "Door") {  //Exit door, must check if opened or closed
            //Check here if door open
            Die();
        } else {    //Will collide with wall
            Die();
        }
    }

    private void PerformMove(int newX, int newY){
        transform.position = new Vector3(transform.position.x + newX, transform.position.y + newY, transform.position.z);

        for (int i=0; i<bodyLength; i++){
            Body[i].transform.position = new Vector3(previousMoves[i].x, previousMoves[i].y, transform.position.z);
        }
            
        previousMoves.Insert(0, (new Vector2(transform.position.x, transform.position.y)));
        previousMoves.RemoveAt(previousMoves.Count - 1);
    }

    private void IncreaseBody(){
        bodyLength++;
        GameObject BodySegment = GameObject.Instantiate(BodyPrefab, new Vector3(
            previousMoves[previousMoves.Count - 1].x, previousMoves[previousMoves.Count - 1].y, transform.position.z), Quaternion.identity);
        Body.Add(BodySegment);
        previousMoves.Add(previousMoves[previousMoves.Count - 1]);
    }

    private void Die(){
        if (lives > 1){
            previousMoves.Clear();
            Spawn();
            lives--;
        } else {    //Game Over-
            Destroy(gameObject);
        }
    }
}
