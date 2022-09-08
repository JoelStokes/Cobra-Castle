using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject BodyPrefab;
    public Vector2 SpawnLocation;

    public Sprite tailSprite;
    public Sprite bodySprite;

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

    //Z Rotation Directions
    public enum Direction 
    {
        Up = 0,
        Right = 270,
        Down = 180,
        Left = 90
    }

    void Start()
    {
        for (int i=0; i<bodyLength; i++){   //Create initial body
            GameObject BodySegment = GameObject.Instantiate(BodyPrefab, transform.position, Quaternion.identity);

            if (i < bodyLength - 1){    //Reset sprites on body parts
                BodySegment.GetComponent<SpriteRenderer>().sprite = bodySprite;
            } else {
                BodySegment.GetComponent<SpriteRenderer>().sprite = tailSprite;
            }

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
        transform.eulerAngles = new Vector3(0, 0, (float)Direction.Right);
        previousMoves.Insert(0, SpawnLocation);

        for (int i=0; i<bodyLength; i++){
            Body[i].transform.position = new Vector3(SpawnLocation.x + ((i*-1)-1), SpawnLocation.y, transform.position.z);
            Body[i].transform.eulerAngles = new Vector3(0, 0, (float)Direction.Right);
            previousMoves.Add(new Vector2(SpawnLocation.x + ((i*-1)-1), SpawnLocation.y));
        }
    }

    private void CheckMoveLocation(int newX, int newY){
        Vector2 newMove = new Vector2(newX,newY);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, newMove, 1);

        if (hit.collider == null || hit.transform.tag == "Tail"){  //Empty Space free to move / Can never crash into Tail since it will change next spot
            PerformMove(newMove);
        } else if (hit.transform.tag == "Mouse") {    //Move & Eat mouse
            Destroy(hit.transform.gameObject);
            IncreaseBody();
            PerformMove(newMove);
        } else if (hit.transform.tag == "Door") {  //Exit door, must check if opened or closed
            //Check here if door open
            Die();
        } else {    //Will collide with wall
            Die();
        }
    }

    private void PerformMove(Vector2 newMove){   //Move player, rotate body parts to match direction, remove oldest previousMove
        transform.position = new Vector3(transform.position.x + newMove.x, transform.position.y + newMove.y, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, GetDirection(newMove, Vector2.zero, true));

        for (int i=0; i<bodyLength; i++){
            Body[i].transform.position = new Vector3(previousMoves[i].x, previousMoves[i].y, transform.position.z);
            if (i == 0){
                Body[i].transform.eulerAngles = new Vector3(0, 0, GetDirection(new Vector2(transform.position.x, transform.position.y), previousMoves[i], false));
            } else {
                Body[i].transform.eulerAngles = new Vector3(0, 0, GetDirection(previousMoves[i-1], previousMoves[i], false));
            }
        }
            
        previousMoves.Insert(0, (new Vector2(transform.position.x, transform.position.y)));
        previousMoves.RemoveAt(previousMoves.Count - 1);
    }

    private float GetDirection(Vector2 newMove, Vector2 oldMove, bool head){   //Calculate direction moving for body rotation
        Vector2 diff;
        if (!head){
            diff = new Vector2(Mathf.Abs(newMove.x) - Mathf.Abs(oldMove.x), Mathf.Abs(newMove.y) - Mathf.Abs(oldMove.y));
        } else {
            diff = newMove;
        }

        Debug.Log(newMove + "/ " + oldMove + "/ " + diff);

        if (diff.x > 0){
            return (float)Direction.Right;
        } else if (diff.x < 0){
            return (float)Direction.Left;
        } else if (diff.y > 0){
            return (float)Direction.Up;
        } else {
            return (float)Direction.Down;
        }
    }

    private void IncreaseBody(){
        bodyLength++;

        GameObject BodySegment = GameObject.Instantiate(BodyPrefab, new Vector3(
            previousMoves[previousMoves.Count - 1].x, previousMoves[previousMoves.Count - 1].y, transform.position.z), Quaternion.identity);
        BodySegment.GetComponent<SpriteRenderer>().sprite = tailSprite;
        Body[Body.Count-1].GetComponent<SpriteRenderer>().sprite = bodySprite;

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
