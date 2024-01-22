using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using NHibernate.Driver;

namespace NHibernate.Spatial.Driver
{
    public class SpatiaLiteDriver : SQLite20Driver
    {
        static SpatiaLiteDriver()
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            if (path == null)
            {
                throw new InvalidOperationException("Cannot get PATH environment variable");
            }

            string executingDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase).LocalPath);
            if (string.IsNullOrEmpty(executingDirectory))
            {
                throw new InvalidOperationException("Cannot get executing directory");
            }

            string spatiaLitePath = Path.Combine(executingDirectory, "lib", "spatialite");
            if (Directory.Exists(spatiaLitePath) && !path.ToLower().Contains(spatiaLitePath))
            {
                Environment.SetEnvironmentVariable("PATH", spatiaLitePath + Path.PathSeparator + path);
            }
        }

        public override DbConnection CreateConnection()
        {
            var cn = base.CreateConnection();
            cn.StateChange += ConnectionStateChangeHandler;
            return cn;
        }

        private static void ConnectionStateChangeHandler(object sender, StateChangeEventArgs e)
        {
            if (e.OriginalState != ConnectionState.Broken &&
                e.OriginalState != ConnectionState.Closed &&
                e.OriginalState != ConnectionState.Connecting ||
                e.CurrentState != ConnectionState.Open)
            {
                return;
            }

            var connection = (SQLiteConnection) sender;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = ON;";
                command.ExecuteNonQuery();

                // NOTE: After upgrading System.Data.SQLite from 1.0.98.1 to 1.0.116.0, the
                //       "SELECT load_extension('mod_spatialite');" SQL query to load the
                //       extension failed with a "SQL logic error: not authorized" error.
                //       Therefore, need to use the SQLiteConnection method instead; see:
                //       https://www.sqlite.org/loadext.html
                connection.LoadExtension("mod_spatialite");

                command.CommandText = "SELECT InitSpatialMetadata(1) WHERE CheckSpatialMetadata() = 0;";
                command.ExecuteNonQuery();
            }
        }
    }
}
