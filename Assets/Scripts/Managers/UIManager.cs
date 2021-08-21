using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField]
    private TextMeshProUGUI _scoreText = null;
    [SerializeField]
    private TextMeshProUGUI _movesText = null;

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
            _scoreText.text = "PUNTI\n" + _currentScoreCount.ToString();
        });

        EventsManager.Instance.OnUndoScore.AddListener((score) =>
        {
            _currentScoreCount -= score;

            if (_currentScoreCount < 0)
                _currentScoreCount = 0;

            _scoreText.text = "PUNTI\n" + _currentScoreCount.ToString();
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
