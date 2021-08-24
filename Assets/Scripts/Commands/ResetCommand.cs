using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Command called when the player reverted the deck reset
/// </summary>
public class ResetCommand : ICommand
{
    private int _lastScoreCount = 0;

    public ResetCommand()
    {

    }

    public void Execute()
    {
        //Debug.Log("ResetCommand");
        EventsManager.Instance.OnReset.Invoke();

        _lastScoreCount = UIManager.Instance.Score;

        EventsManager.Instance.OnUndoScore.Invoke(_lastScoreCount);
    }

    public void Undo()
    {
        EventsManager.Instance.OnUndoReset.Invoke();

        EventsManager.Instance.OnScore.Invoke(_lastScoreCount);
    }
}
