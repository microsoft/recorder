using System;
using Lumia.Sense.Testing;

namespace record
{
        public enum Status
        {
            Empty,
            Recording,
            Stopped
        };

    public class Recording
    {

        public Status ItemState { get; set; }
        public DateTime StarTime { get; set; }

        public SenseRecorder Recorder { get; set; }
    }
}