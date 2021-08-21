using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField]
    private TextMeshProUGUI _movesText = null;

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
