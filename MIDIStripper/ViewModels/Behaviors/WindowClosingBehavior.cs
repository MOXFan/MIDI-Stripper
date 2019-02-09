using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MIDIStripper
{
    class WindowClosingBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CancelCloseProperty = DependencyProperty.Register("CancelClose", typeof(bool), typeof(WindowClosingBehavior), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty PromptIfUnsavedCommandProperty = DependencyProperty.Register("PromptIfUnsavedCommand", typeof(ICommand), typeof(WindowClosingBehavior));

        public bool CancelClose
        {
            get
            {
                return (bool)GetValue(CancelCloseProperty);
            }
            set
            {
                SetValue(CancelCloseProperty, value);
            }
        }

        public ICommand PromptIfUnsavedCommand
        {
            get
            {
                return (ICommand)GetValue(PromptIfUnsavedCommandProperty);
            }
            set
            {
                SetValue(PromptIfUnsavedCommandProperty, value);
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Closing += Window_Closing;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PromptIfUnsavedCommand.Execute(null);
            
            e.Cancel = CancelClose;
        }
    }
}
