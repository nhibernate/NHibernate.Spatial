using NHibernate.Connection;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Tests.NHibernate.Spatial
{
    /// <summary>
    /// This connection provider keeps a list of all open connections,
    /// it is used when testing to check that tests clean up after themselves.
    /// </summary>
    public class DebugConnectionProvider : DriverConnectionProvider
    {
        private readonly HashSet<DbConnection> connections = new HashSet<DbConnection>();

        public bool HasOpenConnections
        {
            get
            {
                // check to see if all connections that were at one point opened
                // have been closed through the CloseConnection
                // method
                if (connections.Count == 0)
                {
                    // there are no connections, either none were opened or
                    // all of the closings went through CloseConnection.
                    return false;
                }

                // Disposing of an ISession does not call CloseConnection (should it???)
                // so a Diposed of ISession will leave a DbConnection in the list but
                // the DbConnection will be closed (atleast with MsSql it works this way).
                foreach (var conn in connections)
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        return true;
                    }
                }

                // all of the connections have been Disposed and were closed that way
                // or they were Closed through the CloseConnection method.
                return false;
            }
        }

        public override DbConnection GetConnection()
        {
            var connection = base.GetConnection();
            connections.Add(connection);
            return connection;
        }

        public override void CloseConnection(DbConnection conn)
        {
            base.CloseConnection(conn);
            connections.Remove(conn);
        }

        public void CloseAllConnections()
        {
            while (connections.Count != 0)
            {
                using (var en = connections.GetEnumerator())
                {
                    en.MoveNext();
                    CloseConnection(en.Current);
                }
            }
        }
    }
}
