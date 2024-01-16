using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Open.Topology.TestRunner
{
    public class XmlTestTimer
    {
        private readonly long freq;
        private long startTime, stopTime;

        // Constructor
        public XmlTestTimer()
        {
            startTime = 0;
            stopTime = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                // high-performance counter not supported
                throw new Win32Exception();
            }
        }

        // Returns the duration of the timer (in seconds)
        public double Duration => (stopTime - startTime)/(double) freq;

        // Start the timer
        public void Start()
        {
            // lets do the waiting threads there work
            Thread.Sleep(0);

            QueryPerformanceCounter(out startTime);
        }

        // Stop the timer
        public void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
    }
}
