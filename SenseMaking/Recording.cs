using System;
using Lumia.Sense.Testing;

namespace SenseMaking
{
        public enum State
        {
            Empty,
            Recording,
            Stopped
        };

    public class Recording
    {
        public State ItemState { get; set; }
        public DateTime StarTime { get; set; }
        public SenseRecorder Recorder { get; set; }
    }
}