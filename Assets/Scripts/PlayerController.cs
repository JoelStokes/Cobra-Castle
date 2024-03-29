using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject BodyPrefab;   //Pieces to instantiate on room start
    public Vector2 SpawnLocation;   //Where player starts in room

    //All sprites used in changing snake body
    public Sprite tailDefaultImg;
    public Sprite tailCurveRightImg;
    public Sprite tailCurveLeftImg;
    public Sprite bodyDefaultImg;
    public Sprite bodyUpRightImg;   //90* flip is Right Down
    public Sprite bodyUpLeftImg;
    public Sprite bodyDownRightImg;
    public Sprite bodyDownLeftImg;
    public Sprite headHurt;
    public Sprite headDefault;

    private int bodyLength = 4;
    private List<GameObject> Body = new List<GameObject>(); //All connected body parts
    private List<SpriteRenderer> BodyRenderers = new List<SpriteRenderer>();    //All body part SpriteRenderers to change sprites based on move directions
    private List<Vector2> previousMoves = new List<Vector2>();  //List of previous moves to apply to body parts
    private MouseManager mouseManager;
    private SpriteRenderer HeadRenderer;

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

    //Movement Timers & Buffers
    private float moveTimer = 0;
    private float moveLimit = .2f;
    private float goldenMouseTimer = 0;
    private float goldenMouseLimit;
    private bool sprinting = false;
    private Vector2 movingDirection;
    private Vector2 nextMove;
    private Vector2 moveBuffer;
    private Vector2 empty;  //Since Vectors can't be null, set to this for empty checks
    private int layerMask;

    private GameManager gameManager;
    public bool started = false;
    public bool controllable = true;
    private bool isDead = false;
    private Animator HeadAnim;

    //SFX
    public AudioClip walkSFX;
    public AudioClip hurtSFX;
    public float hurtVolume;
    public float walkVolume;

    void Start()
    {
        layerMask =~ LayerMask.GetMask("Head");  //Prevent Move from detecting self. Needed head box to prevent mouse spawns in face

        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        HeadRenderer = gameObject.GetComponent<SpriteRenderer>();
        HeadAnim = gameObject.GetComponent<Animator>();

        goldenMouseLimit = moveLimit * 2;

        empty = new Vector2(50,50);
        moveBuffer = empty;

        for (int i=0; i<bodyLength; i++){   //Create initial body
            GameObject BodySegment = GameObject.Instantiate(BodyPrefab, transform.position, Quaternion.identity);
            BodyRenderers.Add(BodySegment.GetComponent<SpriteRenderer>());

            if (i < bodyLength - 1){    //Reset sprites on body parts
                BodyRenderers[BodyRenderers.Count-1].sprite = bodyDefaultImg;
            } else {
                BodyRenderers[BodyRenderers.Count-1].sprite = tailDefaultImg;
            }

            Body.Add(BodySegment);
        }

        Body[Body.Count-1].tag = "Tail";
        Spawn();
    }

    //Add some sort of input buffer system, cache the next move to prevent

    void Update()
    {
        if (!isDead){
            if (controllable){
                Vector2 newInput = empty;
                if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && transform.rotation.eulerAngles.z != (float)Direction.Left){
                    newInput = new Vector2(1, 0);
                } else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && transform.rotation.eulerAngles.z != (float)Direction.Right){
                    newInput = new Vector2(-1,0);
                } else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && transform.rotation.eulerAngles.z != (float)Direction.Down){
                    newInput = new Vector2(0,1);
                } else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && transform.rotation.eulerAngles.z != (float)Direction.Up){
                    newInput = new Vector2(0,-1);
                }

                if (nextMove == empty && newInput  != empty){ //Help prevent eaten moves in fast-paced game, make extra moves go into input buffer to come out next move cycle
                    nextMove = newInput;
                } else if (newInput != empty) {
                    moveBuffer = newInput;
                }

                if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift)){
                    sprinting = true;
                } else {
                    sprinting = false;
                }
            }
        }
    }

    void FixedUpdate() {
        if (started && !isDead){
            moveTimer += Time.deltaTime;
            goldenMouseTimer += Time.deltaTime;

            if (moveTimer > (moveLimit - gameManager.GetFloorSpeed()) 
                || (moveTimer > (moveLimit - gameManager.GetFloorSpeed())/2 && sprinting)){
                if (nextMove != empty){
                    CheckMoveLocation(nextMove);
                } else {
                    CheckMoveLocation(movingDirection);
                }
            }

            if (goldenMouseTimer > goldenMouseLimit){
                goldenMouseTimer = 0;
                mouseManager.MoveGoldenMouse();
            }
        } else if (isDead){
            AnimatorClipInfo[] m_CurrentClipInfo = HeadAnim.GetCurrentAnimatorClipInfo(0);
            if (m_CurrentClipInfo[0].clip.name == "HurtPlayerFinished" || m_CurrentClipInfo[0].clip.name == "StaticPlayer"){
                gameManager.AddDeath(); //If no remaining lives, scene will change before Spawn call hit. Allows death animation to play out before scene change
                Spawn();
                isDead = false;
            }

        }
    }

    private void Spawn(){   //Place player at start location with body trailing behind
        previousMoves.Clear();
        transform.position = new Vector3(SpawnLocation.x, SpawnLocation.y, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, (float)Direction.Right);
        previousMoves.Insert(0, SpawnLocation);

        HeadRenderer.sprite = headDefault;

        for (int i=0; i<bodyLength; i++){
            Body[i].transform.position = new Vector3(SpawnLocation.x + ((i*-1)-1), SpawnLocation.y, transform.position.z);
            Body[i].transform.eulerAngles = new Vector3(0, 0, (float)Direction.Right);
            previousMoves.Add(new Vector2(SpawnLocation.x + ((i*-1)-1), SpawnLocation.y));
        }

        nextMove = new Vector2(1, 0);   //Reset move direction right to prevent crashing at spawn
    }

    private void CheckMoveLocation(Vector2 newMove){
        moveTimer = 0;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, newMove, 1, layerMask);
        movingDirection = newMove;
        nextMove = empty;
        if (moveBuffer != empty){
            nextMove = moveBuffer;
            moveBuffer = empty;
        }

        if (hit.collider == null || hit.transform.tag == "Tail"){  //Empty Space free to move / Can never crash into Tail since it will change next spot
            PerformMove(newMove);
        } else if (hit.transform.tag == "Mouse") {    //Move & Eat mouse
            Destroy(hit.transform.gameObject);
            IncreaseBody();
            PerformMove(newMove);
            gameManager.AddMouse();
            if (!gameManager.CheckMiceFinished()){
                mouseManager.SpawnMouse();
            }
        } else if (hit.transform.tag == "GoldenMouse") {    //Move & Eat special mouse
            Destroy(hit.transform.gameObject);
            IncreaseBody();
            gameManager.AddGoldenMouse();
            PerformMove(newMove);
            mouseManager.EatGoldenMouse();  //Need to prevent new mouse spawns if in Labyrinth
        } else if (hit.transform.tag == "Door") {  //Exit door, must check if opened or closed
            if (gameManager.CheckMiceFinished()){
                gameManager.AddDoor();
            } else {
                Die();
            }
        } else {    //Will collide with wall
            Die();
        }
    }

    private void PerformMove(Vector2 newMove){   //Move player, rotate body parts to match direction, remove oldest previousMove
        transform.position = new Vector3(transform.position.x + newMove.x, transform.position.y + newMove.y, transform.position.z);
        transform.eulerAngles = new Vector3(0, 0, GetHeadDirection(newMove));

        for (int i=0; i<bodyLength; i++){
            Body[i].transform.position = new Vector3(previousMoves[i].x, previousMoves[i].y, transform.position.z);
            if (i == 0){    //Body next to head (no nextPos)
                Body[i].transform.eulerAngles = new Vector3(0, 0, 
                    HandleBodyDirection(previousMoves[i], previousMoves[i+1], new Vector2(transform.position.x, transform.position.y), BodyRenderers[i]));
            } else if (i < bodyLength-1){    //General Body
                Body[i].transform.eulerAngles = new Vector3(0, 0, 
                    HandleBodyDirection(previousMoves[i], previousMoves[i+1], previousMoves[i-1], BodyRenderers[i]));
            } else {    //Tail
                Body[i].transform.eulerAngles = new Vector3(0, 0, 
                    GetTailDirection(previousMoves[i], previousMoves[i+1], previousMoves[i-1], BodyRenderers[i]));
            }
        }

        gameManager.PlaySFX(walkSFX, walkVolume, false);
            
        previousMoves.Insert(0, (new Vector2(transform.position.x, transform.position.y)));
        previousMoves.RemoveAt(previousMoves.Count - 1);
    }

    private float GetHeadDirection(Vector2 newMove){    //Return current rotation of player's head
        if (newMove.x > 0){
            return (float)Direction.Right;
        } else if (newMove.x < 0){
            return (float)Direction.Left;
        } else if (newMove.y > 0){
            return (float)Direction.Up;
        } else {
            return (float)Direction.Down;
        }
    }

    private float HandleBodyDirection(Vector2 currentMove, Vector2 oldMove, Vector2 nextMove, SpriteRenderer currentRenderer){ //Return rotation of body part & update sprite
        if (currentMove.x > oldMove.x && nextMove.x > currentMove.x){   //Straight Right
            currentRenderer.sprite = bodyDefaultImg;
            return (float)Direction.Right;
        } else if (currentMove.x > oldMove.x && nextMove.y > currentMove.y) {  //Right Up
            currentRenderer.sprite = bodyDownRightImg;            
            return (float)Direction.Left;
        } else if (currentMove.x > oldMove.x && nextMove.y < currentMove.y) {  //Right Down
            currentRenderer.sprite = bodyUpRightImg;
            return (float)Direction.Right;
        } else if (currentMove.x < oldMove.x && nextMove.x < currentMove.x) {  //Straight Left
            currentRenderer.sprite = bodyDefaultImg;
            return (float)Direction.Left;
        } else if (currentMove.x < oldMove.x && nextMove.y > currentMove.y) {  //Left Up
            currentRenderer.sprite = bodyDownLeftImg;
            return (float)Direction.Right;
        } else if (currentMove.x < oldMove.x && nextMove.y < currentMove.y) {  //Left Down
            currentRenderer.sprite = bodyUpLeftImg;
            return (float)Direction.Left;
        } else if (currentMove.y > oldMove.y && nextMove.y > currentMove.y) {  //Straight Up
            currentRenderer.sprite = bodyDefaultImg;
            return (float)Direction.Up;
        } else if (currentMove.y < oldMove.y && nextMove.y < currentMove.y) {  //Straight Down
            currentRenderer.sprite = bodyDefaultImg;
            return (float)Direction.Down;
        } else if (currentMove.y > oldMove.y && nextMove.x > currentMove.x) {  //Up Right
            currentRenderer.sprite = bodyUpRightImg;
            return (float)Direction.Up;
        } else if (currentMove.y < oldMove.y && nextMove.x > currentMove.x) {  //Down Right
            currentRenderer.sprite = bodyDownRightImg;
            return (float)Direction.Up;
        } else if (currentMove.y > oldMove.y && nextMove.x < currentMove.x) {  //Up Left
            currentRenderer.sprite = bodyUpLeftImg;
            return (float)Direction.Up;
        } else if (currentMove.y < oldMove.y && nextMove.x < currentMove.x) {  //Down Left
            currentRenderer.sprite = bodyDownLeftImg;
            return (float)Direction.Up;
        } else {
            Debug.Log("Error! Unusual Direction Returned: " + currentMove + "/ " + oldMove + "/ " + nextMove);
            return (float)Direction.Down;
        }
    }

    private float GetTailDirection (Vector2 currentMove, Vector2 oldMove, Vector2 nextMove, SpriteRenderer currentRenderer){ //Return rotation of tail & update sprite
         if (currentMove.x > oldMove.x && nextMove.x > currentMove.x){   //Straight Right
            currentRenderer.sprite = tailDefaultImg;
            return (float)Direction.Right;
        } else if (currentMove.x > oldMove.x && nextMove.y > currentMove.y) {  //Right Up
            currentRenderer.sprite = tailCurveLeftImg;
            return (float)Direction.Right;
        } else if (currentMove.x > oldMove.x && nextMove.y < currentMove.y) {  //Right Down
            currentRenderer.sprite = tailCurveRightImg;
            return (float)Direction.Right;
        } else if (currentMove.x < oldMove.x && nextMove.x < currentMove.x) {  //Straight Left
            currentRenderer.sprite = tailDefaultImg;
            return (float)Direction.Left;
        } else if (currentMove.x < oldMove.x && nextMove.y > currentMove.y) {  //Left Up
            currentRenderer.sprite = tailCurveRightImg;
            return (float)Direction.Left;
        } else if (currentMove.x < oldMove.x && nextMove.y < currentMove.y) {  //Left Down
            currentRenderer.sprite = tailCurveLeftImg;
            return (float)Direction.Left;
        } else if (currentMove.y > oldMove.y && nextMove.y > currentMove.y) {  //Straight Up
            currentRenderer.sprite = tailDefaultImg;
            return (float)Direction.Up;
        } else if (currentMove.y < oldMove.y && nextMove.y < currentMove.y) {  //Straight Down
            currentRenderer.sprite = tailDefaultImg;
            return (float)Direction.Down;
        } else if (currentMove.y > oldMove.y && nextMove.x > currentMove.x) {  //Up Right
            currentRenderer.sprite = tailCurveRightImg;
            return (float)Direction.Up;
        } else if (currentMove.y < oldMove.y && nextMove.x > currentMove.x) {  //Down Right
            currentRenderer.sprite = tailCurveLeftImg;
            return (float)Direction.Down;
        } else if (currentMove.y > oldMove.y && nextMove.x < currentMove.x) {  //Up Left
            currentRenderer.sprite = tailCurveLeftImg;
            return (float)Direction.Up;
        } else if (currentMove.y < oldMove.y && nextMove.x < currentMove.x) {  //Down Left
            currentRenderer.sprite = tailCurveRightImg;
            return (float)Direction.Down;
        } else {    //Mouse eaten, currentMove & oldMove the same
            return Body[Body.Count-1].transform.eulerAngles.z;
        }
    }

    private void IncreaseBody(){
        bodyLength++;

        GameObject BodySegment = GameObject.Instantiate(BodyPrefab, new Vector3(
            previousMoves[previousMoves.Count - 1].x, previousMoves[previousMoves.Count - 1].y, transform.position.z), Quaternion.identity);
            
        Body.Add(BodySegment);
        BodyRenderers.Add(BodySegment.GetComponent<SpriteRenderer>());
        previousMoves.Add(previousMoves[previousMoves.Count - 1]);

        BodyRenderers[BodyRenderers.Count-1].sprite = BodyRenderers[BodyRenderers.Count-2].sprite;
        Body[Body.Count-1].transform.rotation = Body[Body.Count-2].transform.rotation;

        Body[Body.Count-1].tag = "Tail";    //Update current tail segment to prevent death on tail touch (since tail touch technically can never happen)
        Body[Body.Count-2].tag = "Body";
    }

    private void Die(){
        HeadRenderer.sprite = headHurt;

        gameManager.PlaySFX(hurtSFX, hurtVolume, false);
        gameManager.ShakeCamera();

        HeadAnim.Play("HurtPlayer");

        for (int i=0; i<Body.Count; i++){
            Body[i].GetComponent<Animator>().Play("HurtPlayer");
        }

        isDead = true;
    }
}
