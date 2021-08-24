using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    public int Score
    {
        get
        {
            return _currentScoreCount;
        }
    }

    [SerializeField]
    private Transform[] _UIButtons = null;
    [SerializeField]
    private Transform[] _InfoTextLabels = null;

    [SerializeField]
    private TextMeshProUGUI _scoreText = null;
    [SerializeField]
    private TextMeshProUGUI _timerText = null;
    [SerializeField]
    private TextMeshProUGUI _movesText = null;

    [Header("UI Windows")]
    [SerializeField]
    private UIWindow[] UIWindows = null;

    [Header("Device Orientation Parents")]
    [Header("Landscape Parents")]
    [SerializeField]
    private Transform _textInfoLandscapeParent = null;
    [SerializeField]
    private Transform _textInfoPortraitParent = null;

    [Header("Portrait Parents")]
    [SerializeField]
    private Transform _uiButtonsLandscapeParent = null;
    [SerializeField]
    private Transform _uiButtonsPortraitParent = null;

    private int _currentScoreCount = 0;
    private float _currentTimerCount = 0;
    private int _currentMovesCount = 0;

    private bool _pauseTimer = true;
    private bool _canUndo = true;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        InitEvents();
    }

    private void Update()
    {
        HandleTimer();
    }

    private void HandleTimer()
    {
        if (_pauseTimer)
            return;

        _currentTimerCount += Time.deltaTime;

        _timerText.text = "TEMPO\n" + TimerStringFormat.GetTimerString(_currentTimerCount);
    }

    public void OnPauseButton()
    {
        OpenUIWindow(UIWindowID.UIWindowPause);
        GameManager.Instance.UpdateGameState(GameState.Pause);
        AudioManager.Instance.Play("Click");
    }

    public void OnUndoButton()
    {
        if (_canUndo)
        {
            GameManager.Instance.UndoCommand();
            Invoke(nameof(ResetUndoButton), 0.43f);
            _canUndo = false;
        }
    }

    public void OnRestartButton()
    {
        GameManager.Instance.OnGameSceneButton();
    }

    public void OpenUIWindow(UIWindowID windowID)
    {
        for (int i = 0; i < UIWindows.Length; i++)
        {
            UIWindow uiWindow = UIWindows[i];

            if(uiWindow.UIWindowID == windowID)
            {
                uiWindow.OpenWindow();
            }
        }
    }

    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener((List<CardData> cardDataList) =>
        {
            _pauseTimer = false;
        });

        EventsManager.Instance.OnCommand.AddListener(() =>
        {
            _currentMovesCount++;
            _movesText.text = "MOSSE\n" + _currentMovesCount.ToString();
        });

        EventsManager.Instance.OnScore.AddListener((score) =>
        {
            _currentScoreCount += score;

            if (_currentScoreCount < 0)
                _currentScoreCount = 0;

            _scoreText.text = "PUNTI\n" + _currentScoreCount.ToString();
        });

        EventsManager.Instance.OnUndoScore.AddListener((score) =>
        {
            _currentScoreCount -= score;

            if (_currentScoreCount < 0)
                _currentScoreCount = 0;

            _scoreText.text = "PUNTI\n" + _currentScoreCount.ToString();
        });

        EventsManager.Instance.OnGameStateChanged.AddListener((gameState) =>
        {
            switch (gameState)
            {
                case GameState.Running:
                    _pauseTimer = false;
                    break;
                case GameState.Pause:
                    _pauseTimer = true;
                    break;
            }
        });

        EventsManager.Instance.OnDeviceOrientationUpdate.AddListener((deviceOrientation) =>
        {
            switch (deviceOrientation)
            {
                case DeviceOrientation.Portrait:

                    _textInfoLandscapeParent.GetComponent<Image>().enabled = false;
                    _textInfoPortraitParent.GetComponent<Image>().enabled = true;

                    for (int i = 0; i < _InfoTextLabels.Length; i++)
                    {
                        Transform infoTextLabel = _InfoTextLabels[i];
                        infoTextLabel.SetParent(_textInfoPortraitParent);
                    }
                    for (int i = 0; i < _UIButtons.Length; i++)
                    {
                        Transform uiButton = _UIButtons[i];
                        uiButton.SetParent(_uiButtonsPortraitParent);
                    }
                    break;

                case DeviceOrientation.PortraitUpsideDown:

                    _textInfoLandscapeParent.GetComponent<Image>().enabled = false;
                    _textInfoPortraitParent.GetComponent<Image>().enabled = true;

                    for (int i = 0; i < _InfoTextLabels.Length; i++)
                    {
                        Transform infoTextLabel = _InfoTextLabels[i];
                        infoTextLabel.SetParent(_textInfoPortraitParent);
                    }
                    for (int i = 0; i < _UIButtons.Length; i++)
                    {
                        Transform uiButton = _UIButtons[i];
                        uiButton.SetParent(_uiButtonsPortraitParent);
                    }
                    break;

                case DeviceOrientation.LandscapeLeft:

                    _textInfoLandscapeParent.GetComponent<Image>().enabled = true;
                    _textInfoPortraitParent.GetComponent<Image>().enabled = false;

                    for (int i = 0; i < _InfoTextLabels.Length; i++)
                    {
                        Transform infoTextLabel = _InfoTextLabels[i];
                        infoTextLabel.SetParent(_textInfoLandscapeParent);
                    }
                    for (int i = 0; i < _UIButtons.Length; i++)
                    {
                        Transform uiButton = _UIButtons[i];
                        uiButton.SetParent(_uiButtonsLandscapeParent);
                    }
                    break;

                case DeviceOrientation.LandscapeRight:

                    _textInfoLandscapeParent.GetComponent<Image>().enabled = true;
                    _textInfoPortraitParent.GetComponent<Image>().enabled = false;

                    for (int i = 0; i < _InfoTextLabels.Length; i++)
                    {
                        Transform infoTextLabel = _InfoTextLabels[i];
                        infoTextLabel.SetParent(_textInfoLandscapeParent);
                    }
                    for (int i = 0; i < _UIButtons.Length; i++)
                    {
                        Transform uiButton = _UIButtons[i];
                        uiButton.SetParent(_uiButtonsLandscapeParent);
                    }
                    break;
            }
        });

        EventsManager.Instance.OnGameWon.AddListener(() =>
        {
            OpenUIWindow(UIWindowID.UIWindowWin);
        });
    }

    private void ResetUndoButton()
    {
        _canUndo = true;
    }
}
