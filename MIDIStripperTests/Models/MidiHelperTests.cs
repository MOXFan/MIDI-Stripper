using Melanchall.DryWetMidi.Smf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MIDIStripper.Tests
{
    [TestClass()]
    public class MidiHelperTests
    {
        #region GenerateTrackList Tests

        [TestMethod]
        public void GenerateTrackList_NullFile_ShouldReturnNullStringArray()
        {
            MidiFile testFile = null;
            string[] output = null;

            output = MidiHelper.GenerateTrackList(testFile);

            if (output != null)
            { Assert.Fail("Output string array not null."); }
        }

        [TestMethod]
        public void GenerateTrackList_EmptyFile_ShouldReturnEmptyStringArray()
        {
            MidiFile testFile = new MidiFile();
            string[] output = null;

            output = MidiHelper.GenerateTrackList(testFile);

            if (output.Count() != 0)
            { Assert.Fail($"Output string array not empty: Count == {output.Count()}"); }
        }

        [TestMethod]
        public void GenerateTrackList_ValidFile_ShouldListAllTrackNames()
        {
            string[] output = null;
            TrackChunk[] chunks = new TrackChunk[]
            {
                new TrackChunk(new MidiEvent[] { new SequenceTrackNameEvent("Track 0") }),
                new TrackChunk(new MidiEvent[] { new SequenceTrackNameEvent("Track 1") }),
                new TrackChunk(new MidiEvent[] { new SequenceTrackNameEvent("Track 2") })
            };

            MidiFile testFile = new MidiFile(chunks);

            output = MidiHelper.GenerateTrackList(testFile);

            if (output.Count() != 3)
            { Assert.Fail("Output array size incorrect."); }
            else if (output[0] != "0 - Track 0")
            { Assert.Fail($"Track 0 name incorrect: \"{output[0]}\"."); }
            else if (output[1] != "1 - Track 1")
            { Assert.Fail($"Track 1 name incorrect: \"{output[1]}\"."); }
            else if (output[2] != "2 - Track 2")
            { Assert.Fail($"Track 2 name incorrect: \"{output[2]}\"."); }
        }

        #endregion

        #region GenerateTrackData Tests

        [TestMethod]
        public void GenerateTrackData_NullTrack_ShouldReturnNullString()
        {
            string output = null;

            output = MidiHelper.GenerateTrackData(null);

            if (output != null)
            { Assert.Fail("Output string not null."); }
        }

        [TestMethod]
        public void GenerateTrackData_EmptyTrack_ShouldReturnEmptyString()
        {
            string output = null;
            TrackChunk input = new TrackChunk();

            output = MidiHelper.GenerateTrackData(input);

            if(output != "")
            { Assert.Fail($"Output string not empty: {output}"); }
        }

        [TestMethod]
        public void GenerateTrackData_ValidTrack_ShouldReturnStringRepresentingAllItems()
        {
            string output = null;
            MidiEvent[] trackEvents = new MidiEvent[]
            {
                new SequenceTrackNameEvent("Track 0"),
                new ProgramNameEvent("Unit Testing Sample MIDI File"),
                new TimeSignatureEvent(4, 4)
            };
            TrackChunk input = new TrackChunk(trackEvents);
            string expectedOutput = $"{trackEvents[0].ToString()}\n{trackEvents[1].ToString()}\n{trackEvents[2].ToString()}\n";
            
            output = MidiHelper.GenerateTrackData(input);

            if(output != expectedOutput)
            { Assert.Fail($"Output \"{output}\" should be \"{expectedOutput}\"."); }
        }

        #endregion

        #region GetTrackTest Tests

        [TestMethod]
        public void GetTrack_NullFile_ShouldReturnNullTrack()
        {
            TrackChunk output = null;
            MidiFile input = null;

            output = MidiHelper.GetTrack(input,0);

            if(output != null)
            { Assert.Fail("Output TrackChunk not null."); }
        }

        [TestMethod]
        public void GetTrack_EmptyFile_ShouldReturnNull()
        {
            TrackChunk output = null;
            MidiFile input = new MidiFile();
            
            output = MidiHelper.GetTrack(input, 0);

            if (output != null)
            { Assert.Fail("Output TrackChunk not null."); }
        }

        [TestMethod]
        public void GetTrack_ValidFile_ShouldReturnCorrectTrackChunk()
        {
            TrackChunk output = null;
            TrackChunk[] tracks = new TrackChunk[]
            {
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 0"),
                    new ProgramNameEvent("Unit Testing Sample MIDI File")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 1")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 2"),
                    new NoteOnEvent(),
                    new NoteOffEvent()
                })
            };
            MidiFile input = new MidiFile(tracks);

            output = MidiHelper.GetTrack(input, 1);

            if (output != tracks[1])
            { Assert.Fail("Returned incorrect TrackChunk."); }
        }

        [TestMethod]
        public void GetTrack_ValidFileButIndexExceedsCollectionSize_ShouldReturnNull()
        {
            TrackChunk output = null;
            TrackChunk[] tracks = new TrackChunk[]
            {
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 0"),
                    new ProgramNameEvent("Unit Testing Sample MIDI File")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 1")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 2"),
                    new NoteOnEvent(),
                    new NoteOffEvent()
                })
            };
            MidiFile input = new MidiFile(tracks);

            output = MidiHelper.GetTrack(input, 5);

            if (output != null)
            { Assert.Fail("Output not null."); }
        }

        [TestMethod]
        public void GetTrack_ValidFileButIndexNegative_ShouldReturnNull()
        {
            TrackChunk output = null;
            TrackChunk[] tracks = new TrackChunk[]
            {
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 0"),
                    new ProgramNameEvent("Unit Testing Sample MIDI File")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 1")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 2"),
                    new NoteOnEvent(),
                    new NoteOffEvent()
                })
            };
            MidiFile input = new MidiFile(tracks);

            output = MidiHelper.GetTrack(input, -10);

            if (output != null)
            { Assert.Fail("Output not null."); }
        }

        #endregion

        #region StripEmptyTracks Tests

        [TestMethod]
        public void StripEmptyTracks_NullFile_ShouldReturnNegativeOne()
        {
            int output = 0;
            MidiFile input = null;

            output = MidiHelper.StripEmptyTracks(ref input);

            if (output != -1)
            { Assert.Fail($"Output \"{output}\" should be \"-1\"."); }
        }

        [TestMethod]
        public void StripEmptyTracks_EmptyFile_ShouldReturnZero()
        {
            int output = -1;
            MidiFile input = new MidiFile();

            output = MidiHelper.StripEmptyTracks(ref input);

            if (output != 0)
            { Assert.Fail($"Output \"{output}\" should be \"0\"."); }
        }

        [TestMethod]
        public void StripEmptyTracks_ValidFile_ShouldReturnNumberOfEmptyTracksFound()
        {
            int output = -1;
            MidiFile input = new MidiFile(new TrackChunk[] 
            {
                new TrackChunk(new MidiEvent[]{ new SetTempoEvent() }),
                new TrackChunk(),
                new TrackChunk()
            });

            output = MidiHelper.StripEmptyTracks(ref input);

            if (output != 2)
            { Assert.Fail($"Output \"{output}\" should be \"2\"."); }
        }

        #endregion

        #region StripUnwantedMessages Tests

        [TestMethod]
        public void StripUnwantedMessages_NullFile_ShouldReturnNegativeOne()
        {
            int output = 0;
            MidiFile input = null;

            output = MidiHelper.StripUnwantedMessages(ref input);

            if (output != -1)
            { Assert.Fail($"Output \"{output}\" should be \"-1\"."); }
        }

        [TestMethod]
        public void StripUnwantedMessages_EmptyFile_ShouldReturnZero()
        {
            int output = -1;
            MidiFile input = new MidiFile();

            output = MidiHelper.StripUnwantedMessages(ref input);

            if (output != 0)
            { Assert.Fail($"Output \"{output}\" should be \"0\"."); }
        }

        [TestMethod]
        public void StripUnwantedMessages_ValidFileButNoUnwantedMessages_ShouldReturnZero()
        {
            int output = -1;
            MidiFile input = new MidiFile(new TrackChunk[]
            {
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 0"),
                    new ProgramNameEvent("Unit Testing Sample MIDI File")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 1")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 2"),
                    new NoteOnEvent(),
                    new NoteOffEvent()
                })
            });

            output = MidiHelper.StripUnwantedMessages(ref input);

            if (output != 0)
            { Assert.Fail($"Output \"{output}\" should be \"0\"."); }
        }

        [TestMethod]
        public void StripUnwantedMessages_ValidFile_ShouldReturnNumberOfUnwantedMessagesFound()
        {
            int output = -1;
            MidiFile input = new MidiFile(new TrackChunk[]
            {
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 0"),
                    new ProgramNameEvent("Unit Testing Sample MIDI File")
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 1"),
                    new ProgramChangeEvent()
                }),
                new TrackChunk(new MidiEvent[]
                {
                    new SequenceTrackNameEvent("Track 2"),
                    new ControlChangeEvent(),
                    new NoteOnEvent(),
                    new NoteOffEvent()
                })
            });

            output = MidiHelper.StripUnwantedMessages(ref input);

            if (output != 2)
            { Assert.Fail($"Output \"{output}\" should be \"2\"."); }
        }

        #endregion

        #region IsEmptyTrackChunk Tests

        [TestMethod]
        public void IsEmptyTrackChunk_NullTrack_ShouldReturnTrue()
        {
            bool output = false;
            TrackChunk input = null;

            output = MidiHelper.IsEmptyTrackChunk(input);

            if(output != true)
            { Assert.Fail("Output not true."); }
        }

        [TestMethod]
        public void IsEmptyTrackChunk_TrackContainsZeroEvents_ShouldReturnTrue()
        {
            bool output = false;
            TrackChunk input = new TrackChunk();

            output = MidiHelper.IsEmptyTrackChunk(input);

            if (output != true)
            { Assert.Fail("Output not true."); }
        }

        [TestMethod]
        public void IsEmptyTrackChunk_TrackContainsOnlyIrrelevantEvents_ShouldReturnTrue()
        {
            bool output = false;
            TrackChunk input = new TrackChunk(new MidiEvent[] 
            {
                new SequenceTrackNameEvent("Track 0"),
                new ProgramNameEvent("Unit Testing Sample MIDI File")
            });

            output = MidiHelper.IsEmptyTrackChunk(input);

            if (output != true)
            { Assert.Fail("Output not true."); }
        }

        [TestMethod]
        public void IsEmptyTrackChunk_TrackContainsNoteOnEvents_ShouldReturnFalse()
        {
            bool output = true;
            TrackChunk input = new TrackChunk(new MidiEvent[]
            {
                new SequenceTrackNameEvent("Track 1"),
                new NoteOnEvent()
            });

            output = MidiHelper.IsEmptyTrackChunk(input);

            if (output != false)
            { Assert.Fail("Output not false."); }
        }

        [TestMethod]
        public void IsEmptyTrackChunk_TrackContainsSetTempoEvents_ShouldReturnFalse()
        {
            bool output = true;
            TrackChunk input = new TrackChunk(new MidiEvent[]
            {
                new SequenceTrackNameEvent("Track 1"),
                new SetTempoEvent()
            });

            output = MidiHelper.IsEmptyTrackChunk(input);

            if (output != false)
            { Assert.Fail("Output not false."); }
        }

        #endregion

        #region IsMidiEventUnwanted Tests

        [TestMethod()]
        public void IsMidiEventUnwanted_NullMidiEvent_ShouldReturnFalse()
        {
            bool output = true;
            MidiEvent input = null;

            output = MidiHelper.IsMidiEventUnwanted(input);

            if (output != false)
            { Assert.Fail("Output not false."); }
        }

        [TestMethod()]
        public void IsMidiEventUnwanted_NoteOnEvent_ShouldReturnFalse()
        {
            bool output = true;
            MidiEvent input = new NoteOnEvent();

            output = MidiHelper.IsMidiEventUnwanted(input);

            if (output != false)
            { Assert.Fail("Output not false."); }
        }

        [TestMethod()]
        public void IsMidiEventUnwanted_ProgramChangeEvent_ShouldReturnTrue()
        {
            bool output = false;
            MidiEvent input = new ProgramChangeEvent();

            output = MidiHelper.IsMidiEventUnwanted(input);

            if (output != true)
            { Assert.Fail("Output not true."); }
        }

        [TestMethod()]
        public void IsMidiEventUnwanted_ControlChangeEvent_ShouldReturnTrue()
        {
            bool output = false;
            MidiEvent input = new ControlChangeEvent();

            output = MidiHelper.IsMidiEventUnwanted(input);

            if (output != true)
            { Assert.Fail("Output not true."); }
        }

        #endregion
    }
}