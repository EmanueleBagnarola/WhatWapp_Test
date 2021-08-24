using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance reference
    /// </summary>
    public static GameManager Instance = null;

    [Header("Set true to enable the ability to move any card on any card")]
    [Header("To force Device Orientation UI adaptation press L for Landscape or P for portrait")]
    public bool EnableTest = false;
    
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
    /// Return the current device orientation (Landscape variants and Portrait variants)
    /// </summary>
    public DeviceOrientation CurrentDeviceOrientation
    {
        get
        {
            return _currentDeviceOrientation;
        }
    }

    /// <summary>
    /// The system that handles the gameplay rules
    /// </summary>
    public MoveSystem MoveSystem
    {
        get
        {
            return _moveSystem;
        }
    }

    /// <summary>
    /// The system that handles the Command pattern
    /// </summary>
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

    /// <summary>
    ///  How long to wait until we check device orientation again.
    /// </summary>
    private static float _deviceOrientationCheckDelay = 0.5f;

    /// <summary>
    /// Current Device Orientation
    /// </summary>
    //[SerializeField]
    private DeviceOrientation _currentDeviceOrientation;

    /// <summary>
    /// Keep the device orientation check running?
    /// </summary>
    private static bool _isAlive = true;

    /// <summary>
    /// Stores the count of the current completed aces count. If 4, then the game is won
    /// </summary>
    private int _completedAcePileCount = 0;

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

    private void Start()
    {
        DontDestroyOnLoad(this);

        InitLog();
        InitSystems();

        StartCoroutine(StartingCheckForDeviceOrientationChange());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Event called when a scene is loaded
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="loadSceneMode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(CheckForDeviceOrientationChange(0.1f));
    }

    private void Update()
    {
        // Editor debug inputs
        if (Input.GetKeyDown(KeyCode.L))
        {
            EventsManager.Instance.OnDeviceOrientationUpdate.Invoke(DeviceOrientation.LandscapeRight);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            EventsManager.Instance.OnDeviceOrientationUpdate.Invoke(DeviceOrientation.Portrait);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdateCompletedAcePileCount(OperationType.Add);
        }
    }

    /// <summary>
    /// Undo the last command
    /// </summary>
    public void UndoCommand()
    {
        _commandSystem.UndoCommand();
    }

    /// <summary>
    /// Update the completed aces pile count
    /// </summary>
    /// <param name="operationType"></param>
    public void UpdateCompletedAcePileCount(OperationType operationType)
    {
        switch (operationType)
        {
            case OperationType.Add:
                _completedAcePileCount++;
                break;

            case OperationType.Remove:
                _completedAcePileCount--;
                break;
        }

        if(_completedAcePileCount >= 4)
        {
            EventsManager.Instance.OnGameWon.Invoke();
            AudioManager.Instance.Play("Victory");
        }
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

    public void OnHomeSceneButton()
    {
        AudioManager.Instance.Play("Click");
        SceneManager.LoadScene(SceneID.Scene_Home.ToString());
    }

    public void OnGameSceneButton()
    {
        AudioManager.Instance.Play("Click");
        SceneManager.LoadScene(SceneID.Scene_Game.ToString());
    }

    public void UpdateGameState(GameState gameState)
    {
        EventsManager.Instance.OnGameStateChanged.Invoke(gameState);

        _currentGameState = gameState;
    }

    private IEnumerator CheckForDeviceOrientationChange(float delay)
    {
        yield return new WaitForSeconds(delay);

        _currentDeviceOrientation = Input.deviceOrientation;

        while (_isAlive)
        {
            // Check for an Orientation Change
            if (_currentDeviceOrientation != Input.deviceOrientation)
            {
                _currentDeviceOrientation = Input.deviceOrientation;
                EventsManager.Instance.OnDeviceOrientationUpdate.Invoke(_currentDeviceOrientation);
            }

            yield return new WaitForSeconds(_deviceOrientationCheckDelay);
        }
    }

    private IEnumerator StartingCheckForDeviceOrientationChange()
    {
        yield return new WaitForSeconds(0.1f);

        _currentDeviceOrientation = Input.deviceOrientation;
        EventsManager.Instance.OnDeviceOrientationUpdate.Invoke(_currentDeviceOrientation);
    }

    void OnDestroy()
    {
        _isAlive = false;
    }
}
