using System;

namespace Open.Topology.TestRunner
{
    public class XmlTestErrorEventArgs : EventArgs
    {
        public XmlTestErrorEventArgs(Exception ex)
        {
            Thrown = ex;
        }

        public Exception Thrown { get; }
    }

    public delegate void XmlTestErrorEventHandler(object sender, XmlTestErrorEventArgs args);

    /// <summary>
    /// Summary description for XmlTestExceptionManager.
    /// </summary>
    public class XmlTestExceptionManager
    {
        public static event XmlTestErrorEventHandler ErrorEvent;

        public static void Publish(Exception ex)
        {
            if (ErrorEvent != null)
            {
                ErrorEvent(typeof(XmlTestExceptionManager), new XmlTestErrorEventArgs(ex));
            }
        }
    }
}
