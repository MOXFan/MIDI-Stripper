

using System;
using System.Windows.Input;

namespace MIDIStripper
{
    public class WpfCommand : ICommand
    {
        private readonly Action<object> _instructionDelegate;
        private readonly Predicate<object> _conditionalObject;

        public WpfCommand(Action<object> newInstruction, Predicate<object> newConditional)
        {
            if (newInstruction == null)
                throw new NullReferenceException("newInstruction argument to WpfCommand constructor.");

            _instructionDelegate = newInstruction;
            _conditionalObject = newConditional;
        }

        public WpfCommand(Action<object> newInstruction) : this(newInstruction, null)
        {

        }

        public event EventHandler CanExecuteChanged
        {
            add
            {                
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if(_conditionalObject == null)
            {
                return true;
            }
            else
            {
                return _conditionalObject.Invoke(parameter);
            }
        }

        public void Execute(object parameter)
        {
            if (_instructionDelegate == null)
            {
                throw new NullReferenceException("Instruction Delegate not assigned to WpfCommand.");
            }
            else
            {
                _instructionDelegate.Invoke(parameter);
            }
        }
    }
}
