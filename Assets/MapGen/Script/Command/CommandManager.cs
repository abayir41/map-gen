using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Command
{
    public class CommandManager : MonoBehaviour
    {
        public static CommandManager Instance { get; private set; }
        
        private readonly List<ICommand> commandList = new List<ICommand>();
        private int _index;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UndoCommand();
            }
            
            if (Input.GetKeyDown(KeyCode.Y))
            {
                RedoCommand();
            }
        }

        public void RunCommand(ICommand command)
        {
            if(command == null) return;
            
            if (_index < commandList.Count)
                commandList.RemoveRange(_index, commandList.Count - _index);

            commandList.Add(command);
            command.Execute();
            _index++;
        }

        public void UndoCommand()
        {
            if (commandList.Count == 0)
                return;
            if (_index > 0)
            {
                commandList[_index - 1].Undo();
                _index--;
            }
        }

        public void RedoCommand()
        {
            if (commandList.Count == 0)
                return;

            if (_index < commandList.Count)
            {
                _index++;
                commandList[_index - 1].Execute();
            }
        }
    }
}