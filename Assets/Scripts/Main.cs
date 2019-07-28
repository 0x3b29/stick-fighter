using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private GameObject txtReplayInstructions;
    private GameObject txtPlayerStats;
    private bool roundFinished;

    public static Main instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Cursor.visible = false;

        txtReplayInstructions = GameObject.Find("TxtReplay");
        txtPlayerStats = GameObject.Find("TxtPlayerStats");
        txtReplayInstructions.SetActive(false);
        roundFinished = false;
    
        // After reloading scene, the counter has to be set
        UpdateCounter();
    }

    public void Replay()
    {
        if (roundFinished)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Quit()
    {
        if (roundFinished)
        {
            Application.Quit();
        }
    }

    // This function is called by the player that lost only
    public void FinishRound(int playerThatLost)
    {
        roundFinished = true;
        txtReplayInstructions.SetActive(true);

        if (playerThatLost == 1)
        {
            PersistentCounter.instance.incPlayer2Score();
        }
        else
        {
            PersistentCounter.instance.incPlayer1Score();
        }

        // Update counter for immediate feedback when round finished
        UpdateCounter();
    }

    void UpdateCounter()
    {
        txtPlayerStats.GetComponent<UnityEngine.UI.Text>().text = PersistentCounter.instance.getPlayer1Wins() + " : " + PersistentCounter.instance.getPlayer2Wins();
    }
}
