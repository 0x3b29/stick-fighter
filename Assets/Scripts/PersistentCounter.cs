using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentCounter : MonoBehaviour
{
    public static PersistentCounter instance;
    public static int player1Wins;
    public static int player2Wins;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            DestroyImmediate(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void incPlayer1Score()
    {
        player1Wins++;
    }

    public void incPlayer2Score()
    {
        player2Wins++;
    }

    public int getPlayer1Wins()
    {
        return player1Wins;
    }

    public int getPlayer2Wins()
    {
        return player2Wins;
    }
}
