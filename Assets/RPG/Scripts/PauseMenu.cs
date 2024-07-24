using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenu;

    public GameObject[] info;


    void Start()
    {
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    void Resume ()
    {
        pauseMenu.SetActive(false);
        for (int i = 0; i < info.Length; i++)
        {
            info[i].SetActive(false);
        }
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause ()
    {
        pauseMenu.SetActive(true);
        for (int i = 0; i < info.Length; i++)
        {
            info[i].SetActive(true);
        }
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
