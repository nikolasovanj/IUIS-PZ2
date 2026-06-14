using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class CommandStack : BindableBase
    {
        private readonly Stack<MyICommand> _undoStack = new Stack<MyICommand>();
        private readonly Stack<MyICommand> _redoStack = new Stack<MyICommand>();

        public bool CanUndo { get { return _undoStack.Count > 0; } }
        public bool CanRedo { get { return _redoStack.Count > 0; } }

        public void AddCommand(MyICommand command)
        {
            _undoStack.Push(command);
            _redoStack.Clear();
            Notify();
        }
        public void Undo()
        {
            if (!CanUndo) return;
            var cmd = _undoStack.Pop();
            cmd.Undo();
            _redoStack.Push(cmd);
            Notify();
        }
        public void Redo()
        {
            if (!CanRedo) return;
            var cmd = _redoStack.Pop();
            cmd.Execute();
            _undoStack.Push(cmd);
            Notify();
        }
        private void Notify()
        { 
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }
    }
}
