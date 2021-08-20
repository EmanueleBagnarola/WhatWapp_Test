using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCommand : ICommand
{
    public ResetCommand()
    {

    }

    public void Execute()
    {
        Debug.Log("ResetCommand");
        EventsManager.Instance.OnReset.Invoke();
    }

    public void Undo()
    {
        EventsManager.Instance.OnUndoReset.Invoke();
    }
}
