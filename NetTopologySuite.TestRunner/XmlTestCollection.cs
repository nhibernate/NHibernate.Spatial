using System.Collections;

namespace Open.Topology.TestRunner
{
    /// <summary>
    /// Summary description for XmlTestCollection.
    /// </summary>
    public class XmlTestCollection : CollectionBase
    {
        #region Constructors and Destructors

        public XmlTestCollection()
        {
            Name = string.Empty;
        }

        #endregion Constructors and Destructors

        #region Events

        public event XmlTextEventHandler TestEvent;

        #endregion Events

        #region Private Members

        #endregion Private Members

        #region Public Properties

        public XmlTest this[int index]
        {
            get => (XmlTest) List[index];

            set => List[index] = value;
        }

        public string Name { get; set; }

        #endregion Public Properties

        #region Public Methods

        public int Add(XmlTest value)
        {
            return List.Add(value);
        }

        public int IndexOf(XmlTest value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, XmlTest value)
        {
            List.Insert(index, value);
        }

        public void Remove(XmlTest value)
        {
            List.Remove(value);
        }

        public bool RunTests()
        {
            if (List.Count > 0)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    var testItem = (XmlTest) List[i];

                    if (testItem != null)
                    {
                        bool run = testItem.Run();
                        if (TestEvent != null)
                        {
                            var args = new XmlTestEventArgs(i, run, testItem);
                            TestEvent(this, args);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        #endregion Public Methods
    }
}
