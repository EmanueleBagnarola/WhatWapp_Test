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

    public DeviceOrientation CurrentDeviceOrientation
    {
        get
        {
            return _currentDeviceOrientation;
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

    /// <summary>
    ///  How long to wait until we check device orientation again.
    /// </summary>
    private static float _deviceOrientationCheckDelay = 0.5f;

    /// <summary>
    /// Current Device Orientation
    /// </summary>
    [SerializeField]
    private DeviceOrientation _currentDeviceOrientation = DeviceOrientation.Portrait;

    /// <summary>
    /// Keep the device orientation check running?
    /// </summary>
    private static bool _isAlive = true;

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

        StartCoroutine(CheckForDeviceOrientationChange());
    }

    private void Update()
    {
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

    public void UndoCommand()
    {
        _commandSystem.UndoCommand();
    }

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

    IEnumerator CheckForDeviceOrientationChange()
    {
        _currentDeviceOrientation = Input.deviceOrientation;

        while (_isAlive)
        {
            // Check for an Orientation Change
            switch (Input.deviceOrientation)
            {
                case DeviceOrientation.Unknown:            // Ignore
                case DeviceOrientation.FaceUp:            // Ignore
                case DeviceOrientation.FaceDown:        // Ignore
                    break;
                default:
                    if (_currentDeviceOrientation != Input.deviceOrientation)
                    {
                        _currentDeviceOrientation = Input.deviceOrientation;
                        EventsManager.Instance.OnDeviceOrientationUpdate.Invoke(_currentDeviceOrientation);
                    }
                    break;
            }

            yield return new WaitForSeconds(_deviceOrientationCheckDelay);
        }
    }

    void OnDestroy()
    {
        _isAlive = false;
    }
}
