using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager ActiveUI { private set; get; }

    public GameObject titleMenu;
    public GameObject loadScreen;

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

    // Start is called before the first frame update
    void Start()
    {
        if (loadScreen != null && loadScreen.activeSelf) loadScreen.SetActive(false);        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

    public void NewGame()
    {
        seed = Random.Range(0, int.MaxValue);
        if (titleMenu != null) titleMenu.SetActive(false);
        if (loadScreen != null) loadScreen.SetActive(true);
        inUI = false;
        SceneManager.LoadScene("World");
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
