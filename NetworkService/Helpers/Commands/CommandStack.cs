using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetworkService.Helpers.Commands
{
    public class CommandStack : BindableBase
    {
        private readonly Stack<MyICommand> _undoStack = new Stack<MyICommand>();
        private readonly Stack<MyICommand> _redoStack = new Stack<MyICommand>();

        public bool CanUndo { get { return _undoStack.Count > 0; } }
        public bool CanRedo { get { return _redoStack.Count > 0; } }
        public ObservableCollection<string> History { get; } = new ObservableCollection<string>();
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
        public void UndoAll()
        {
            while (CanUndo)
            {
                var cmd = _undoStack.Pop();
                cmd.Undo();
                _redoStack.Push(cmd);
                Notify();
            }
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
            OnPropertyChanged(nameof(History));
        }
        public void Clear()
        {
            _redoStack.Clear();
            _undoStack.Clear();
            Notify();
        }
    }
}
