using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private TextMeshProUGUI _movesText = null;

    [SerializeField]
    private Transform _textInfoLandscapeParent = null;
    [SerializeField]
    private Transform _textInfoPortraitParent = null;

    [SerializeField]
    private Transform _uiButtonsLandscapeParent = null;
    [SerializeField]
    private Transform _uiButtonsPortraitParent = null;

    private int _currentScoreCount = 0;
    private int _currentMovesCount = 0;

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

        EventsManager.Instance.OnDeviceOrientationUpdate.AddListener((deviceOrientation) =>
        {
            switch (deviceOrientation)
            {
                case DeviceOrientation.Portrait:
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

    private void ResetUndoButton()
    {
        _canUndo = true;
    }
}
