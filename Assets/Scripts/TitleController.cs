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

    //Menu Management
    public GameObject PressStart;
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameObject HelpMenu;
    public GameObject DeleteMenu;
    public GameObject Cursor;
    public Color32 deselectedColor;
    
    //Menu Buttons
    public TextMeshPro[] MainMenuButtons;
    public TextMeshPro[] OptionsButtons;
    public TextMeshPro[] HelpButtons;

    private string currentMenu = "start"; //start, main, options, help, delete
    private int currentSelection;   //Highlighted number on page
    private float cursorXAdjust = -.27f;
    private float cursorYAdjust = 1.1f;

    private GameManager gameManager;
    private CameraScroll cameraScroll;

    private bool animating = false;

    void Start(){   //Load previous score & highest score
        prevScore.SetText("Prev: " + PlayerPrefs.GetInt("Prev Score").ToString("000000"));
        highScore.SetText("High: " + PlayerPrefs.GetInt("High Score").ToString("000000"));

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cameraScroll = GameObject.Find("Main Camera").GetComponent<CameraScroll>();

        ChangeMenu("start");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
            ChangeSelection(1);
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
            ChangeSelection(-1);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
             && !animating){   //Start Scrolling
             InputSelection();
        }
    }

    private void ChangeMenu(string newMenu){
        MainMenu.SetActive(false);
        PressStart.SetActive(false);
        OptionsMenu.SetActive(false);
        HelpMenu.SetActive(false);
        DeleteMenu.SetActive(false);
        Cursor.SetActive(true);

        currentMenu = newMenu;

        switch (currentMenu){
            case "start":
                PressStart.SetActive(true);
                Cursor.SetActive(false);
                break;
            case "main":
                MainMenu.SetActive(true);
                break;
            case "options":
                OptionsMenu.SetActive(true);
                break;
            case "help":
                HelpMenu.SetActive(true);
                break;
            case "delete":
                DeleteMenu.SetActive(true);
                break;
        }

        ChangeOptionColors();
    }

    private void ChangeSelection(int adjustment){
        currentSelection -= adjustment;

        if (currentMenu == "main" || currentMenu == "options"){
            if (currentSelection >= MainMenuButtons.Length){
                currentSelection = 0;
            } else if (currentSelection < 0){
                currentSelection = MainMenuButtons.Length-1;
            }

            Debug.Log(MainMenuButtons[currentSelection].gameObject.name);
            Cursor.transform.position = new Vector3(cursorXAdjust, MainMenuButtons[currentSelection].gameObject.transform.position.y + cursorYAdjust, Cursor.transform.position.z);
        } else if (currentMenu == "help"){

        }

        ChangeOptionColors();
    }

    private void ChangeOptionColors(){
        switch (currentMenu){
            case "main":
                for (int i = 0; i < MainMenuButtons.Length; i++){
                    MainMenuButtons[i].color = deselectedColor;
                }
                Debug.Log(currentSelection);
                MainMenuButtons[currentSelection].color = Color.white;
                break;
            case "options":
                for (int i = 0; i < OptionsButtons.Length; i++){
                    OptionsButtons[i].color = deselectedColor;
                }
                MainMenuButtons[currentSelection].color = Color.white;
                break;
            case "help":
                for (int i = 0; i < HelpButtons.Length; i++){
                    HelpButtons[i].color = deselectedColor;
                }
                HelpButtons[currentSelection].color = Color.white;
                break;
        }
    }

    private void InputSelection(){
        if (currentMenu == "start"){
            ChangeMenu("main");
            currentSelection = 0;
            ChangeSelection(0); //Needed to reset cursor location
        } else if (currentMenu == "main"){
            switch (currentSelection){
                case 0:
                    BeginEndAnim();
                    break;
                case 1:
                    ChangeMenu("help");
                    currentSelection = 0;
                    break;
                case 2:
                    ChangeMenu("options");
                    currentSelection = 0;
                    break;
            }
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

    private void PassSoundChanges(){
        //Call GameManager with new SFX & Music changes
    }

    private void DeleteScores(){
        //Call GameManager for removal of high score
    }
}
