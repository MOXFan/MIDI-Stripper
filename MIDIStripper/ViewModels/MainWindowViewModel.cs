using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MIDIStripper
{
    class MainWindowViewModel
    {
        #region Commands

        // File menu
        public ICommand OpenFileCommand { get; private set; }
        public ICommand CloseFileCommand { get; private set; }
        public ICommand SaveFileCommand { get; private set; }
        public ICommand SaveFileAsCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        // Edit Menu
        public WpfCommand StripEmptyTracksCommand { get; private set; }
        public WpfCommand StripUnwantedMessagesCommand { get; private set; }

        // Non-menu
        public ICommand PromptIfUnsavedCommand { get; private set; }

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            OpenFileCommand = new WpfCommand(this.OpenFile);
            CloseFileCommand = new WpfCommand(this.CloseFile, this.IsFileLoaded);
            SaveFileCommand = new WpfCommand(this.SaveFile, this.IsFileLoaded);
            SaveFileAsCommand = new WpfCommand(this.SaveFileAs, this.IsFileLoaded);
            ExitCommand = new WpfCommand(this.Exit);

            StripEmptyTracksCommand = new WpfCommand(this.StripEmptyTracks, this.IsFileLoaded);
            StripUnwantedMessagesCommand = new WpfCommand(this.StripUnwantedMessages, this.IsFileLoaded);

            PromptIfUnsavedCommand = new WpfCommand(this.PromptIfUnsaved);

            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject())) return;

            HandleCommandLineArgs();
        }

        #endregion

        #region Properties

        public MidiFileModel CurrentFile { get; } = new MidiFileModel();

        #endregion

        #region Command Methods

        private bool IsFileLoaded(object ignored)
        {
            return CurrentFile.FileLoaded;
        }

        public void OpenFile(object ignored)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Standard MIDI Files|*.mid|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                bool loadSuccessful = CurrentFile.ReadMidiFile(dialog.FileName);

                if(loadSuccessful == false)
                {
                    MessageBox.Show(CurrentFile.StatusBarText, "Error loading file.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void CloseFile(object ignored)
        {
            if (CurrentFile.UnsavedChanges == true)
                PromptIfUnsaved(null);

            if (CurrentFile.CancelClose == false)
            {
                CurrentFile.CloseFile();
            }
        }

        public void SaveFile(object ignored)
        {
            if (CurrentFile.UnsavedChanges == false || CurrentFile == null)
            {
                CurrentFile.StatusBarText = "No changes to save.";
                return;
            }
            else if(File.Exists(CurrentFile.CurrentFilename))
            {
                MessageBoxResult result = MessageBox.Show($"Do you want to overwrite the existing file \"{CurrentFile.CurrentFilename}\"?", 
                    "Overwrite existing file?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        WriteMidiFile();
                        break;
                    case MessageBoxResult.No:
                        SaveFileAs(null);
                        break;
                    default:
                        CurrentFile.CancelClose = true;
                        break;
                }
            }
            else
            { WriteMidiFile(); }
        }

        public void SaveFileAs(object ignored)
        {
            if (CurrentFile.FileLoaded == false)
            {
                CurrentFile.StatusBarText = "No open file to save.";
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Standard MIDI Files|*.mid|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                WriteMidiFile(dialog.FileName);
            }
        }

        private void Exit(object ignored)
        {
            Application.Current.MainWindow.Close();
        }

        private void PromptIfUnsaved(object ignored)
        {
            if (CurrentFile.UnsavedChanges == true && CurrentFile.FileLoaded == true)
            {
                MessageBoxResult result = MessageBox.Show("You have unsaved work.  Do you want to save now?", "Save changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        CurrentFile.CancelClose = true;
                        SaveFile(null);
                        break;
                    case MessageBoxResult.Cancel:
                        CurrentFile.CancelClose = true;
                        break;
                    default:
                        CurrentFile.CancelClose = false;
                        break;
                }
            }
            else
            {
                CurrentFile.CancelClose = false;
            }
        }

        private void StripEmptyTracks(object ignored)
        {
            CurrentFile.StripEmptyTracks();
        }

        private void StripUnwantedMessages(object ignored)
        {
            CurrentFile.StripUnwantedMessages();
        }

        #endregion

        #region Methods

        private void WriteMidiFile()
        {
            WriteMidiFile(CurrentFile.CurrentFilename);
        }

        private void WriteMidiFile(string filename)
        {
            bool saveSuccessful = CurrentFile.WriteMidiFile(filename);

            if (saveSuccessful)
            {
                MessageBox.Show(CurrentFile.StatusBarText, "File saved.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(CurrentFile.StatusBarText, "Error saving file.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HandleCommandLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();

            if(args.Count() <= 1)
            { return; }

            string filename = args[1];
            CurrentFile.ReadMidiFile(filename);
        }

        #endregion
    }
}
