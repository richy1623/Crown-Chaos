using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public static bool gameIsPaused = false;
    
    public float timeRemaining = 15*60;
    public bool timerIsRunning = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject leaderboardUI;
    [SerializeField] private TextMeshProUGUI timer;

    private void Start()
    {
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused)
            {
                resume();
            }
            else
            {
                pause();
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            showLeaderboard();
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            hideLeaderboard();
        }

        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                float minutes = Mathf.FloorToInt(timeRemaining / 60);
                float seconds = Mathf.FloorToInt(timeRemaining % 60);

                timer.text = minutes + ":" + seconds;
            }
            else
            {
                Debug.Log("Times up");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }




    }

    public void resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    void showLeaderboard()
    {
        leaderboardUI.SetActive(true);
    }

    void hideLeaderboard()
    {
        leaderboardUI.SetActive(false);
    }

    public void loadMenu()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }
}
