using Melanchall.DryWetMidi.Smf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIDIStripper
{
    public static class MidiHelper
    {
        #region Methods

        public static string[] GenerateTrackList(MidiFile file)
        {
            List<string> output = new List<string>();

            if (file == null)
            { return null; }

            int trackIndex = 0;
            foreach (TrackChunk track in file.GetTrackChunks())
            {
                string trackName = "Unnamed";
                foreach (MidiEvent currentEvent in track.Events.Where(x => x.GetType() == typeof(SequenceTrackNameEvent)))
                {
                    trackName = ((SequenceTrackNameEvent)currentEvent).Text;
                }
                output.Add($"{trackIndex++} - {trackName}");
            }

            return output.ToArray();
        }

        public static string GenerateTrackData(TrackChunk track)
        {
            if(track == null)
            { return null; }

            StringBuilder output = new StringBuilder();
            EventsCollection trackEvents = track.Events;

            foreach (MidiEvent currentEvent in trackEvents)
            {
                output.Append($"{currentEvent.ToString()}\n");
            }

            return output.ToString();
        }

        public static TrackChunk GetTrack(MidiFile file, int trackIndex)
        {
            if(file == null)
            {
                return null;
            }

            TrackChunk[] chunks = file.GetTrackChunks().ToArray<TrackChunk>();

            if( trackIndex < 0 || trackIndex >= chunks.Count())
            {
                return null;
            }
            else
            {
                return chunks[trackIndex];
            }
        }

        public static int StripEmptyTracks(ref MidiFile file)
        {
            if (file == null)
            { return -1; }

            int deletedTrackCount = 0;

            deletedTrackCount += file.Chunks.RemoveAll(IsEmptyTrackChunk);

            return deletedTrackCount;
        }

        public static int StripUnwantedMessages(ref MidiFile file)
        {
            if(file == null)
            { return -1; }

            int deletedMessageCount = 0;

            foreach(TrackChunk currentTrack in file.GetTrackChunks())
            {
                deletedMessageCount += currentTrack.Events.RemoveAll(IsMidiEventUnwanted);
            }

            return deletedMessageCount;
        }

        #endregion

        #region Predicates

        public static bool IsEmptyTrackChunk(MidiChunk chunk)
        {
            if (chunk == null)
            { return true; }
            else if (chunk.GetType() != typeof(TrackChunk))
            { return false; }

            TrackChunk track = (TrackChunk)chunk;

            foreach (MidiEvent currentEvent in track.Events)
            {
                Type eventType = currentEvent.GetType();

                if (eventType == typeof(NoteOnEvent) || eventType == typeof(SetTempoEvent))
                    return false;
            }

            return true;
        }

        public static bool IsMidiEventUnwanted(MidiEvent message)
        {
            if(message == null)
            { return false; }

            Type eventType = message.GetType();

            if (eventType == typeof(ProgramChangeEvent) || eventType == typeof(ControlChangeEvent))
                return true;
            else
                return false;
        }

        #endregion
    }
}
