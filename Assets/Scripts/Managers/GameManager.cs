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

    public MoveSystem MoveSystem
    {
        get
        {
            return _moveSystem;
        }
    }

    public CommandSystem CommandHandler
    {
        get
        {
            return _commandSystem;
        }
    }

    /// <summary>
    /// Stores the current game state
    /// </summary>
    private GameState _currentGameState = GameState.Running;

    private MoveSystem _moveSystem;
    private CommandSystem _commandSystem;

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

    private IEnumerator Start()
    {
        InitLog();

        yield return new WaitForSeconds(1);

        InitSystems();

        StartGame();
    }

    public void UndoCommand()
    {
        _commandSystem.UndoCommand();
    }

    private void InitSystems()
    {
        _moveSystem = new MoveSystem();
        _commandSystem = new CommandSystem();

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
