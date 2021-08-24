using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Command called when the player drew a card from the deck
/// </summary>
public class DrawCommand : ICommand
{
    public DrawCommand()
    {

    }

    public void Execute()
    {
        Debug.Log("EXECUTE DRAW COMMAND");
        AudioManager.Instance.Play("MoveCommand");
    }

    public void Undo()
    {
        Debug.Log("UNDO DRAW COMMAND");

        EventsManager.Instance.OnUndoDraw.Invoke();
    }
}
