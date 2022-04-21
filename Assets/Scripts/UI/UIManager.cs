using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager ActiveUI { private set; get; }

    public GameObject titleMenu;
    public GameObject newGameButton;
    public GameObject mainMenuButton;

    public GameObject loadScreen;

    public GameObject newGameScreen;
    public TMP_InputField seedField;
    public Toggle rngSeed;

    public bool activeMouse = true;
    public bool inUI;
    public bool priorMouseActive;
    public int seed = 3;
    

    private void Awake()
    {
        if (ActiveUI != null && ActiveUI != this) Destroy(gameObject);
        else
        {
            ActiveUI = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // This should only be in the main menu scene....
    void Start()
    {
        if (loadScreen != null && loadScreen.activeSelf) loadScreen.SetActive(false);
        if (newGameScreen != null && newGameScreen.activeSelf) newGameScreen.SetActive(false);
        if (titleMenu != null && !titleMenu.activeSelf) titleMenu.SetActive(true);

        if (newGameButton != null) newGameButton.SetActive(true);
        if (mainMenuButton != null) mainMenuButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (titleMenu != null)
            {
                titleMenu.SetActive(!titleMenu.activeSelf);
                if(!inUI)
                {
                    priorMouseActive = activeMouse;
                    MouseActive(true);
                }
                else
                {
                    MouseActive(priorMouseActive);
                }
                inUI = !inUI;
            }
            else
                QuitGame();
        }
    }

    public void InitiateNew()
    {
        if (titleMenu != null) titleMenu.SetActive(false);
        if (newGameScreen != null) newGameScreen.SetActive(true);
        if (loadScreen != null) loadScreen.SetActive(false);
    }

    public void NewGame()
    {
        if (rngSeed.isOn) seed = Random.Range(int.MinValue, int.MaxValue);
        else seed = int.Parse(seedField.text);

        if (titleMenu != null) titleMenu.SetActive(false);
        if (newGameScreen != null) newGameScreen.SetActive(false);
        if (loadScreen != null) loadScreen.SetActive(true);

        if (newGameButton != null) newGameButton.SetActive(false);
        if (mainMenuButton != null) mainMenuButton.SetActive(true);

        inUI = false;
        SceneManager.LoadScene("World");
    }

    public void RandomSeed()
    {
        if (rngSeed.isOn)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            seedField.text = seed.ToString();
            seedField.interactable = false;
        }
        else
        {
            seedField.interactable = true;
            seed = int.Parse(seedField.text);
        }
    }

    public void MainMenu()
    {
        if (titleMenu != null) titleMenu.SetActive(true);
        if (newGameScreen != null) newGameScreen.SetActive(false);
        if (loadScreen != null) loadScreen.SetActive(false);

        if (newGameButton != null) newGameButton.SetActive(true);
        if (mainMenuButton != null) mainMenuButton.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void ReturnToMain()
    {
        if (titleMenu != null) titleMenu.SetActive(true);
        if (newGameScreen != null) newGameScreen.SetActive(false);
        if (loadScreen != null) loadScreen.SetActive(false);
    }

    public void QuitGame()
    {
        // Do game clean up here (like autosave and stuff).
        Application.Quit();
    }

    public void DoneLoading()
    {
        if (loadScreen != null) loadScreen.SetActive(false);
    }

    public void MouseActive(bool setMouse)
    {
        activeMouse = setMouse;
        if (!Application.isEditor)
        {
            if (activeMouse)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
