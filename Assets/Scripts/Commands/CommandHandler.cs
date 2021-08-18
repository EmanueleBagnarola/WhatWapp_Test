using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandHandler
{
    private List<ICommand> _commandList = new List<ICommand>();
    private int _index;

    public void AddCommand(ICommand command)
    {
        if (_index < _commandList.Count)
            _commandList.RemoveRange(_index, _commandList.Count - _index);

        _commandList.Add(command);

        //command.Execute();

        _index++;
    }

    public void UndoCommand()
    {
        if (_commandList.Count == 0)
            return;

        if (_index > 0)
        {
            _commandList[_index - 1].Undo();
            _commandList.RemoveAt(_index - 1);
            _index--;
        }
    }
}

