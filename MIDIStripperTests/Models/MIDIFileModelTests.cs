using Melanchall.DryWetMidi.Smf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MIDIStripper.Tests
{
    [TestClass()]
    public class MidiFileModelTests
    {
        #region LoadMidiFile Tests

        [TestMethod]
        public void LoadMidiFile_NullFile_ShouldCallCloseFile()
        {
            MidiFileModel testObject = new MidiFileModel();
            MidiFile input = null;

           testObject.LoadMidiFile(input);

            if(testObject.StatusBarText != "File closed.")
            { Assert.Fail($"StatusBarText \"{testObject.StatusBarText}\" should be \"File closed.\"."); }
        }

        [TestMethod]
        public void LoadMidiFile_ValidFile_ShouldReplaceCurrentFileWithInput()
        {
            MidiFileModel testObject = new MidiFileModel(new MidiFile());
            MidiFile input = new MidiFile(new MidiChunk[] {new TrackChunk(new MidiEvent[] {new NoteOnEvent() })});

            testObject.LoadMidiFile(input);

            if (testObject.CurrentFile == null)
            { Assert.Fail("CurrentFile is null."); }
            else if (testObject.CurrentFile != input)
            { Assert.Fail("CurrentFile does not match input MidiFile."); }
        }

        #endregion

        #region CloseFile Tests

        [TestMethod]
        public void CloseFile_ShouldSetCurrentFileToNull()
        {
            MidiFileModel testObject = new MidiFileModel(new MidiFile());

            testObject.LoadMidiFile(null);

            if(testObject.CurrentFile != null)
            { Assert.Fail("CurrentFile not null."); }
        }

        #endregion

        #region IsFileLoaded Tests

        [TestMethod]
        public void IsFileLoaded_CurrentFileIsNull_ShouldReturnFalse()
        {
            MidiFileModel testObject = new MidiFileModel();
            bool output = testObject.IsFileLoaded();

            if( output != false)
            { Assert.Fail("Return value not false."); }
        }

        [TestMethod]
        public void IsFileLoaded_CurrentFileIsNotNull_ShouldReturnTrue()
        {
            MidiFileModel testObject = new MidiFileModel(new MidiFile());
            bool output = testObject.IsFileLoaded();

            if (output != true)
            { Assert.Fail("Return value not true."); }
        }

        #endregion

        #region StripEmptyTracks Tests

        [TestMethod]
        public void StripEmptyTracks_NoFileLoaded_ShouldReportNoFileLoaded()
        {
            MidiFileModel testObject = new MidiFileModel();

            testObject.StripEmptyTracks();

            if(testObject.StatusBarText != "No file loaded.")
            { Assert.Fail($"Incorrect StatusBarText: \"{testObject.StatusBarText}\""); }
        }

        [TestMethod]
        public void StripEmptyTracks_FileWithNoEmptyTracksLoaded_ShouldReportNoTracksRemoved()
        {
            MidiFileModel testObject = new MidiFileModel(new MidiFile());

            testObject.StripEmptyTracks();

            if(testObject.StatusBarText != "No empty tracks found.")
            { Assert.Fail($"Incorrect StatusBarText: \"{testObject.StatusBarText}\""); }
        }

        [TestMethod]
        public void StripEmptyTracks_FileWithEmptyTracksLoaded_ShouldReportNumberOfTracksRemovedAndRemaining()
        {
            MidiFileModel testObject = new MidiFileModel(new MidiFile(new MidiChunk[] 
            {
                new TrackChunk(new MidiEvent[]{new SetTempoEvent() }),
                new TrackChunk(new MidiEvent[]{ }),
                new TrackChunk(new MidiEvent[]{ })
            }));

            testObject.StripEmptyTracks();

            if (testObject.StatusBarText != "2 empty tracks deleted.  1 track remaining.")
            { Assert.Fail($"Incorrect StatusBarText: \"{testObject.StatusBarText}\""); }
        }

        #endregion

        #region StripUnwantedMessages Tests

        [TestMethod]
        public void StripUnwantedMessages_NoFileLoaded_ShouldReportNoMessagesRemoved()
        {
            MidiFileModel testObject = new MidiFileModel();

            testObject.StripUnwantedMessages();

            if(testObject.StatusBarText != "No CC/PC messages found.")
            { Assert.Fail($"Invalid StatusBarText: \"{testObject.StatusBarText}\"."); }
        }

        [TestMethod]
        public void StripUnwantedMessages_FileWithOneMessageLoaded_ShouldReportOneMessageRemoved()
        {
            MidiFileModel testObject = new MidiFileModel(new MidiFile(new MidiChunk[] { new TrackChunk(new MidiEvent[] { new ProgramChangeEvent() }) }));

            testObject.StripUnwantedMessages();

            if (testObject.StatusBarText != "1 CC/PC message deleted.")
            { Assert.Fail($"Invalid StatusBarText: \"{testObject.StatusBarText}\"."); }
        }

        [TestMethod]
        public void StripUnwantedMessages_FileWithThreeMessageLoaded_ShouldReportThreeMessagesRemoved()
        {
            MidiFileModel testObject = new MidiFileModel(new MidiFile(new MidiChunk[] { new TrackChunk(new MidiEvent[] 
            {
                new ProgramChangeEvent(),
                new ProgramChangeEvent(),
                new ControlChangeEvent()
            }) }));

            testObject.StripUnwantedMessages();

            if (testObject.StatusBarText != "3 CC/PC messages deleted.")
            { Assert.Fail($"Invalid StatusBarText: \"{testObject.StatusBarText}\"."); }
        }

        #endregion
    }
}