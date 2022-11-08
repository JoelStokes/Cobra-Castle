using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manage Start & End of level scrolls
public class CameraScroll : MonoBehaviour
{
    public bool scrollOnStart = false;

    private float animSpeed = .07f;
    private float animTimer = 0;
    private float animEdgePos = 18;
    private float animCurrentPos = 0;
    private bool animating = false;
    private bool startAnim = true;

    private GameManager gameManager;
    private GameObject CameraObj;

    void Start()
    {
        animCurrentPos = -animEdgePos;

        CameraObj = GameObject.Find("Main Camera");

        if (scrollOnStart){
            CameraObj.transform.position = new Vector3(animCurrentPos, CameraObj.transform.position.y, CameraObj.transform.position.z);
            animating = true;
        }

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    void Update(){  //Camera Scroll for start/end of level
        if (animating){
            animTimer += Time.deltaTime;

            if (animTimer >= animSpeed){
                animCurrentPos++;
                CameraObj.transform.position = new Vector3(animCurrentPos, CameraObj.transform.position.y, CameraObj.transform.position.z);

                if ((animCurrentPos < 0 && startAnim) || (animCurrentPos < animEdgePos && !startAnim)){
                    animTimer = 0;
                } else {
                    animating = false;
                    if (startAnim){
                        gameManager.StartPlayer();
                    } else {
                        gameManager.LoadNewLevel();
                    }
                }
            }
        }
    }

    public void BeginEndAnim(){
        startAnim = false;
        animating = true;
    }

    public void ChangeEndPos(float newEdgePos){ //Only needed for irregular scene sizes (like Title)
        animEdgePos = newEdgePos;
    }

    public void ChangeCurrentPos(float newCurrentPos){ //Only needed for irregular scene sizes (like Title)
        animCurrentPos = newCurrentPos;
    }
}
