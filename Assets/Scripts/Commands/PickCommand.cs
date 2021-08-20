using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        EventsManager.Instance.OnPick.Invoke(_guiCardReference);
    }

    public void Undo()
    {
        EventsManager.Instance.OnUndoPick.Invoke(_guiCardReference, _drawnCardsIndex);
    }
}
