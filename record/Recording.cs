/*
 * Copyright (c) 2015 Microsoft
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.  
 */
using System;
using Lumia.Sense.Testing;

namespace record
{
    /// <summary>
    /// Enum to describe the status of a recording
    /// </summary>
    public enum Status
    {
        Empty,
        Recording,
        Stopped
    };

    /// <summary>
    /// Helper class to describe a recording
    /// </summary>
    public class Recording
    {
        /// <summary>
        /// Status instance for item state
        /// </summary>
        public Status ItemState { get; set; }

        /// <summary>
        /// Start time for a recording
        /// </summary>
        public DateTime StarTime { get; set; }

        /// <summary>
        /// Sense Recorder instance
        /// </summary>
        public SenseRecorder Recorder { get; set; }
    }
}