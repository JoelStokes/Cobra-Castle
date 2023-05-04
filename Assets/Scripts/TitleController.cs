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
    public GameObject HelpMenu2;
    public GameObject DeleteMenu;
    public GameObject Cursor;
    public Color32 deselectedColor;
    
    //Menu Buttons
    public TextMeshPro[] MainMenuButtons;
    //public TextMeshPro[] OptionsButtons;
    public TextMeshPro[] HelpButtons;
    public TextMeshPro[] HelpButtons2;

    private string currentMenu = "start"; //start, main, options, help, delete
    private int currentSelection;   //Highlighted number on page
    private float cursorXAdjust = -.2f;
    private float cursorYAdjust = 1.1f;

    private GameManager gameManager;
    private CameraScroll cameraScroll;

    //SFX
    public AudioClip startSFX;
    public AudioClip menuMoveSFX;
    public AudioClip menuConfirmSFX;
    public AudioClip menuBackSFX;
    public float sfxVolume;

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
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
            ChangeSelection(1, true);
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
            ChangeSelection(-1, true);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
             && !animating){   //Start Scrolling
             InputSelection();
             //BeginEndAnim();
        }
    }

    private void ChangeMenu(string newMenu){
        MainMenu.SetActive(false);
        PressStart.SetActive(false);
        OptionsMenu.SetActive(false);
        HelpMenu.SetActive(false);
        HelpMenu2.SetActive(false);
        DeleteMenu.SetActive(false);
        Cursor.SetActive(true);

        currentMenu = newMenu;

        if (currentMenu != "start"){
            gameManager.PlaySFX(menuConfirmSFX, sfxVolume, false);
        }

        switch (currentMenu){
            case "start":
                PressStart.SetActive(true);
                Cursor.SetActive(false);
                break;
            case "main":
                if (currentSelection != 0){
                    ChangeSelection(1, false);
                }
                MainMenu.SetActive(true);
                break;
            /*case "options":
                OptionsMenu.SetActive(true);
                break;*/
            case "help":
                if (currentSelection != 0){
                    ChangeSelection(1, false);
                }
                HelpMenu.SetActive(true);
                break;
            case "help2":
                HelpMenu2.SetActive(true);
                break;
            case "delete":
                DeleteMenu.SetActive(true);
                break;
        }

        UpdateCursorPosition();
        ChangeOptionColors();
    }

    private void ChangeSelection(int adjustment, bool playAudio){
        if (playAudio && currentMenu != "start"){
            gameManager.PlaySFX(menuMoveSFX, sfxVolume, false);
        }

        currentSelection -= adjustment;

        if (currentMenu == "main" || currentMenu == "options" || currentMenu == "help" || currentMenu == "help2"){
            if (currentSelection >= MainMenuButtons.Length){
                currentSelection = 0;
            } else if (currentSelection < 0){
                currentSelection = MainMenuButtons.Length-1;
            }
        }
            
        UpdateCursorPosition();
        ChangeOptionColors();
    }

    private void UpdateCursorPosition(){
        if (currentMenu == "main"){
            Cursor.transform.position = new Vector3(cursorXAdjust, MainMenuButtons[currentSelection].gameObject.transform.position.y + cursorYAdjust, Cursor.transform.position.z);
        } else if (currentMenu != "start") {
            Cursor.transform.position = new Vector3(HelpButtons[currentSelection].gameObject.transform.position.x - (cursorXAdjust/2), HelpButtons[currentSelection].gameObject.transform.position.y + cursorYAdjust, Cursor.transform.position.z);
        }
    }

    private void ChangeOptionColors(){
        switch (currentMenu){
            case "main":
                for (int i = 0; i < MainMenuButtons.Length; i++){
                    MainMenuButtons[i].color = deselectedColor;
                }
                MainMenuButtons[currentSelection].color = Color.white;
                break;
            /*case "options":
                for (int i = 0; i < OptionsButtons.Length; i++){
                    OptionsButtons[i].color = deselectedColor;
                }
                MainMenuButtons[currentSelection].color = Color.white;
                break;*/
            case "help":
                for (int i = 0; i < HelpButtons.Length; i++){
                    HelpButtons[i].color = deselectedColor;
                }
                HelpButtons[currentSelection].color = Color.white;
                break;
            case "help2":
                for (int i = 0; i < HelpButtons2.Length; i++){
                    HelpButtons2[i].color = deselectedColor;
                }
                HelpButtons2[currentSelection].color = Color.white;
                break;
        }
    }

    private void InputSelection(){
        if (currentMenu == "start"){
            ChangeMenu("main");
            currentSelection = 0;
            ChangeSelection(0, false); //Needed to reset cursor location
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
        } else if (currentMenu == "help"){
            switch (currentSelection){
                case 0:
                    ChangeMenu("help2");
                    break;
                case 1:
                    ChangeMenu("main");
                    currentSelection = 0;
                    break;
            }
        } else if (currentMenu == "help2"){
            switch (currentSelection){
                case 0:
                    ChangeMenu("help");
                    break;
                case 1:
                    ChangeMenu("main");
                    currentSelection = 0;
                    break;
            }
        }
    }

    private void BeginEndAnim(){
        gameManager.PlaySFX(startSFX, sfxVolume, false);

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
