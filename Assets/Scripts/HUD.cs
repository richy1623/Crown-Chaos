using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    public static bool gameIsPaused = false;
    
    public float timeRemaining = 15*60;
    public bool timerIsRunning = false;

    private float eliminationPopupTime;
    private bool eliminationPopupShowing = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject leaderboardUI;
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject eliminationPopup;
    [SerializeField] private TextMeshProUGUI timer;

    private AudioManager audioManager;
    private Leaderboard leaderboard;

    public static HUD instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        timerIsRunning = true;
        audioManager = AudioManager.instance;
        leaderboard = Leaderboard.Instance;
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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showLeaderboard();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
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

                timer.text = minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
            }
            else
            {
                Debug.Log("Times up");
                timeRemaining = 0;
                timerIsRunning = false;
                leaderboard.endGame();
            }
        }

        if(eliminationPopupShowing)
        {
            if (eliminationPopupTime > 0)
            {
                eliminationPopupTime -= Time.deltaTime;
            }
            else
            {
                eliminationPopup.SetActive(false);
                eliminationPopupShowing = false;
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
        audioManager.interruptEndMusic();
        Time.timeScale = 1f;
        gameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void showDefeat(string name, int elims)
    {
        Time.timeScale = 0f;
        defeatScreen.transform.Find("defeat_status").GetComponent<TextMeshProUGUI>().text = name + " won with " + elims + " eliminations";
        defeatScreen.SetActive(true);
        audioManager.playDefeat();
    }

    public void showVictory(int elims)
    {
        Time.timeScale = 0f;
        victoryScreen.transform.Find("victory_status").GetComponent<TextMeshProUGUI>().text = "You won with " + elims + " eliminations";
        victoryScreen.SetActive(true);
        audioManager.playVictory();
    }

    public void showElimination(string playerName)
    {
        eliminationPopup.transform.Find("player_name").GetComponent<TextMeshProUGUI>().text = playerName;
        eliminationPopup.SetActive(true);
        eliminationPopupTime = 4f;
        eliminationPopupShowing = true;
    }
}
