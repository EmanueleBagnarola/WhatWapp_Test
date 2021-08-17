using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance reference
    /// </summary>
    public static GameManager Instance = null;

    /// <summary>
    /// Return the current game type
    /// </summary>
    public readonly GameType GameType = GameType.Classic;
    
    /// <summary>
    /// Return the current game state
    /// </summary>
    public GameState GameState
    {
        get
        {
            return _currentGameState;
        }
    }
    /// <summary>
    /// Stores the current game state
    /// </summary>
    private GameState _currentGameState = GameState.Running;

    private void Awake()
    {
        // Init Singleton ------
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        //----------------------
    }

    private void Start()
    {
        InitLog();

        StartGame();
    }

    /// <summary>
    /// Disable Unity Log outside the editor
    /// </summary>
    private void InitLog()
    {
        #if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
        #else
        Debug.unityLogger.logEnabled = false;
        #endif
    }

    /// <summary>
    /// Start the game and call OnStartGame event
    /// </summary>
    private void StartGame()
    {
        EventsManager.Instance.OnStartGame.Invoke();
    }
}
