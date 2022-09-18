using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Determine List of empty playable area within camera to use for mouse spawning
public class MouseManager : MonoBehaviour
{
    public Vector2 topLeft;
    public Vector2 bottomRight;
    public GameObject MousePrefab;
    public GameObject GoldenMousePrefab;

    private List<Vector2> SpawnGrid = new List<Vector2>();

    private int mouseCount = 0;
    private int mouseLim = 10;
    private bool goldenMouseActive = false;
    private GameObject GoldenMouse;

    private int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        layerMask =~ LayerMask.GetMask("GoldenMouse");  //Prevent Golden Mouse Raycast from hitting self

        PopulateGridList();
        SpawnMouse();
    }

    private void PopulateGridList(){    //Check for colliders on each grid segment. If none, add to SpawnGrid as viable mouse spawning location
        for (int x = (int)topLeft.x; x < bottomRight.x+1; x++){
            for (int y = (int)topLeft.y; y > bottomRight.y-1; y--){
                Collider2D collider = Physics2D.OverlapPoint(new Vector2(x,y), ~0, -1, 1);
                if (collider == null){  //Add to list if no colliders found
                    SpawnGrid.Add(new Vector2(x,y));
                }
            }
        }
    }

    public void SpawnMouse(){   //Spawn Mouse/Mice. Do While segments ensure no spots have Player or other mouse on them before spawning
        mouseCount++;

        Instantiate(MousePrefab, GetOpenLocation(), Quaternion.identity);

        if (mouseCount % 3 == 0 && !goldenMouseActive){    //Spawn Golden Mouse alongside regular mouse. Ensure mice spawn on different positions. Can't have 2 golden mice
            GoldenMouse = GameObject.Instantiate(GoldenMousePrefab, GetOpenLocation(), Quaternion.identity);
            goldenMouseActive = true;
        }
    }

    private Vector3 GetOpenLocation(){
        int randomLocation;
        Collider2D collider;

        do {    //Loop until position with no collider is found to avoid placing mouse on taken tile
            randomLocation = Random.Range(0, SpawnGrid.Count-1);
            collider = Physics2D.OverlapPoint(new Vector2(SpawnGrid[randomLocation].x, SpawnGrid[randomLocation].y), ~0, -1, 1);
        } while (collider != null);
        return new Vector3(SpawnGrid[randomLocation].x, SpawnGrid[randomLocation].y, 0);
    }

    public void MoveGoldenMouse(){
        if (goldenMouseActive){
            RaycastHit2D hit;
            float raycastDistance = 1;

            List<Vector2> moveList = new List<Vector2>();   //List of selectable directions, each time random fails pop from list. When none left, default to no move
            moveList.Add(new Vector2(1,0));
            moveList.Add(new Vector2(-1,0));
            moveList.Add(new Vector2(0,1));
            moveList.Add(new Vector2(0,-1));
            Vector2 moveDirection;

            do{
                if (moveList.Count-1 > -1){
                    int randomValue = Random.Range(0, moveList.Count);
                    moveDirection = moveList[randomValue];
                    moveList.RemoveAt(randomValue);
                } else {
                    moveDirection = Vector2.zero;   //Failsafe, set move & raycastDistance to 0, Golden Mouse is trapped and can't move
                    raycastDistance = 0;
                }

                hit = Physics2D.Raycast(new Vector2(GoldenMouse.transform.position.x, GoldenMouse.transform.position.y), moveDirection, raycastDistance, layerMask);
            } while(hit.collider != null);

            GoldenMouse.transform.position = new Vector3(GoldenMouse.transform.position.x + moveDirection.x, 
                GoldenMouse.transform.position.y + moveDirection.y, GoldenMouse.transform.position.z);
        }
    }

    public void EatGoldenMouse(){
        goldenMouseActive = false;
        mouseCount++;   //Should Golden Mouse count equal a higher number? Or is bonus points at end worth it?
    }
}
