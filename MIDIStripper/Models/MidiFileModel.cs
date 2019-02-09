using Melanchall.DryWetMidi.Smf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace MIDIStripper
{
    public class MidiFileModel : INotifyPropertyChanged
    {
        #region Private Member Variables

        private MidiFile _currentMidiFile = null;
        private int _selectedTrackIndex = 0;
        private string _selectedTrackData = "";
        private string _statusBarText = "";
        private bool _cancelClose = false;
        private bool _unsavedChanges = false;

        #endregion

        #region Constructors

        public MidiFileModel() { }

        public MidiFileModel(MidiFile inputFile)
        {
            _currentMidiFile = inputFile;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public bool CancelClose
        {
            get
            {
                return _cancelClose;
            }
            set
            {
                _cancelClose = value;
                NotifyPropertyChanged("CancelClose");
            }
        }

        public bool UnsavedChanges
        {
            get
            {
                return _unsavedChanges;
            }
            set
            {
                _unsavedChanges = value;
                if (value == true)
                {
                    CancelClose = true;
                }
                NotifyPropertyChanged("UnsavedChanges");
            }
        }

        public ObservableCollection<string> TrackList { get; private set; } = new ObservableCollection<string>();

        public int SelectedTrackIndex
        {
            get
            {
                return _selectedTrackIndex;
            }
            set
            {
                _selectedTrackIndex = value;
                NotifyPropertyChanged("SelectedTrackIndex");
                UpdateSelectedTrackData();
            }
        }

        public string SelectedTrackData
        {
            get
            {
                return _selectedTrackData;
            }
            private set
            {
                _selectedTrackData = value;
                NotifyPropertyChanged("SelectedTrackData");
            }
        }

        public string StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                _statusBarText = value;
                NotifyPropertyChanged("StatusBarText");
            }
        }

        public string CurrentFilename { get; private set; }
            
        public bool FileLoaded
        {
            get
            { return IsFileLoaded(); }
        }

        public MidiFile CurrentFile { get { return _currentMidiFile; } }

        #endregion

        #region Public Methods

        public void UpdateTrackList()
        {
            if (FileLoaded)
            { TrackList = new ObservableCollection<string>(MidiHelper.GenerateTrackList(_currentMidiFile)); }
            else
            { TrackList.Clear(); }

            SelectedTrackIndex = 0;
            NotifyPropertyChanged("TrackList");
        }

        public void LoadMidiFile(MidiFile input)
        {
            if (input == null)
            { CloseFile(); }
            else
            {
                _currentMidiFile = input;
                CurrentFilename = null;
                UpdateTrackList();
                SelectedTrackIndex = 0;
                StatusBarText = "Loaded MIDI File from memory.";
            }
        }

        public bool ReadMidiFile(string filename)
        {
            bool loadSuccessful = true;

            if (File.Exists(filename))
            {
                try
                { 
                _currentMidiFile = MidiFile.Read(filename);
                }
                catch(Exception e)
                {
                    loadSuccessful = false;
                    StatusBarText = $"Unable to load file \"{filename}\": {e.ToString()}";
                }

                if (loadSuccessful)
                {
                    CurrentFilename = filename;
                    UpdateTrackList();
                    SelectedTrackIndex = 0;
                    StatusBarText = $"Loaded file \"{filename}\" with {TrackList.Count} tracks.";
                }
            }
            else
            {
                loadSuccessful = false;
                StatusBarText = $"File not found: \"{filename}\".";
            }

            return loadSuccessful;
        }
        
        public bool WriteMidiFile(string filename)
        {
            bool saveSuccessful = true;
            try
            {
                _currentMidiFile.Write(filename, true);
            }
            catch (Exception e)
            {
                saveSuccessful = false;
                StatusBarText = $"Error saving \"{filename}\": {e.ToString()}";
            }

            if (saveSuccessful)
            {
                CurrentFilename = filename;
                CancelClose = false;
                UnsavedChanges = false;
                StatusBarText = $"File saved to \"{filename}\".";
            }

            return saveSuccessful;
        }

        public void CloseFile()
        {
            _currentMidiFile = null;
            CurrentFilename = null;
            UpdateTrackList();
            UnsavedChanges = false;
            StatusBarText = "File closed.";
        }

        public bool IsFileLoaded()
        {
                if(_currentMidiFile == null)
                { return false; }
                else
                { return true; }
        }

        public void StripEmptyTracks()
        {
            if (_currentMidiFile == null)
            { StatusBarText = "No file loaded."; }
            else
            {
                int deletedTrackCount = MidiHelper.StripEmptyTracks(ref _currentMidiFile);

                if (deletedTrackCount == 0)
                { StatusBarText = "No empty tracks found."; }
                else
                {
                    UpdateTrackList();
                    int remainingTrackCount = _currentMidiFile.Chunks.Count;
                    StatusBarText = $"{deletedTrackCount} empty {PluralizeTracks(deletedTrackCount)} deleted.  {remainingTrackCount} {PluralizeTracks(remainingTrackCount)} remaining.";
                    UnsavedChanges = true;
                }
            }
        }

        public void StripUnwantedMessages()
        {
            if (_currentMidiFile == null)
            { StatusBarText = "No CC/PC messages found."; }
            else
            {
                int deletedMessageCount = MidiHelper.StripUnwantedMessages(ref _currentMidiFile);

                UpdateSelectedTrackData();
                StatusBarText = $"{deletedMessageCount} CC/PC {PluralizeMessages(deletedMessageCount)} deleted.";
                UnsavedChanges = true;
            }
        }

        #endregion

        #region Private Methods

        private string PluralizeTracks(int trackCount)
        {
            if(trackCount == 1)
            { return "track"; }
            else
            { return "tracks"; }
        }

        private string PluralizeMessages(int messageCount)
        {
            if (messageCount == 1)
            { return "message"; }
            else
            { return "messages"; }
        }

        private void UpdateSelectedTrackData()
        {
            SelectedTrackData = MidiHelper.GenerateTrackData(MidiHelper.GetTrack(_currentMidiFile, _selectedTrackIndex));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            { return; }
            else
            { PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }

        #endregion
    }
}
