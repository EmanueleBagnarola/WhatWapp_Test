using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCommand : ICommand
{
    public DrawCommand()
    {

    }

    public void Execute()
    {
    }

    public void Undo()
    {
        EventsManager.Instance.OnUndoDraw.Invoke();
    }
}
