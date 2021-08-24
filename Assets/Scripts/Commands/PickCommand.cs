using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Command called when the player moves a card from the drawn pile to the aces pile or the table pile
/// </summary>
public class PickCommand : ICommand
{
    private GUICard _guiCardReference = null;
    private int _drawnCardsIndex = 0;

    public PickCommand(GUICard guiCardReference, int drawnCardsIndex)
    {
        _guiCardReference = guiCardReference;
        _drawnCardsIndex = drawnCardsIndex;
    }

    public void Execute()
    {
        Debug.Log("EXECUTE PICK");
        EventsManager.Instance.OnPick.Invoke(_guiCardReference);
        EventsManager.Instance.OnScore.Invoke(5);
    }

    public void Undo()
    {
        Debug.Log("UNDO PICK");
        EventsManager.Instance.OnUndoPick.Invoke(_guiCardReference, _drawnCardsIndex);
        EventsManager.Instance.OnUndoScore.Invoke(5);
    }
}
