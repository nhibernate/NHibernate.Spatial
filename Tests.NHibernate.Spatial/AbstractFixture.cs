using log4net;
using log4net.Config;
using NetTopologySuite.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Spatial.Dialect;
using NHibernate.Spatial.Mapping;
using NHibernate.Spatial.Metadata;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace Tests.NHibernate.Spatial
{
    // Copied and modified from NHibernate.Test/TestCase.cs
    public abstract class AbstractFixture
    {
        protected static readonly WKTReader Wkt = new WKTReader
        {
            IsOldNtsCoordinateSyntaxAllowed = false
        };

        protected Configuration configuration;
        protected ISessionFactory sessions;
        private static readonly ILog Log = LogManager.GetLogger(typeof(AbstractFixture));
        private ISpatialDialect spatialDialect;
        private ISession lastOpenedSession;
        private DebugConnectionProvider connectionProvider;

        static AbstractFixture()
        {
            // Configure log4net here since configuration through an attribute doesn't always work.
            var repositoryAssembly = Assembly.GetEntryAssembly();
            var logRepository = LogManager.GetRepository(repositoryAssembly);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        protected abstract Type[] Mappings { get; }

        /// <summary>
        /// Creates the tables used in this TestCase
        /// </summary>
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            try
            {
                Configure();
                BuildSessionFactory();
                ConfigureSpatialMetadata();

                bool created = true;
                try
                {
                    CreateSchema();
                }
                catch
                {
                    created = false;
                }
                if (!created)
                {
                    DropSchema();
                    CreateSchema();
                }
            }
            catch (Exception e)
            {
                Log.Error("Error while setting up the test fixture", e);
                throw;
            }
            OnTestFixtureSetUp();
        }

        /// <summary>
        /// Removes the tables used in this TestCase.
        /// </summary>
        /// <remarks>
        /// If the tables are not cleaned up sometimes SchemaExport runs into
        /// Sql errors because it can't drop tables because of the FKs.  This
        /// will occur if the TestCase does not have the same hbm.xml files
        /// included as a previous one.
        /// </remarks>
        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            OnTestFixtureTearDown();
            DropSchema();
            Cleanup();
        }

        /// <summary>
        /// Set up the test. This method is not overridable, but it calls
        /// <see cref="OnSetUp" /> which is.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            OnSetUp();
        }

        /// <summary>
        /// Checks that the test case cleans up after itself. This method
        /// is not overridable, but it calls <see cref="OnTearDown" /> which is.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            OnTearDown();

            bool wasClosed = CheckSessionWasClosed();
            bool wasCleaned = !CheckDatabaseWasCleanedOnTearDown || CheckDatabaseWasCleaned();
            bool wereConnectionsClosed = CheckConnectionsWereClosed();
            bool fail = !wasClosed || !wasCleaned || !wereConnectionsClosed;

            if (fail)
            {
                Assert.Fail("Test didn't clean up after itself");
            }
        }

        public void DeleteMappings(ISession session)
        {
            foreach (var type in Mappings)
            {
                session.Delete("from " + type.FullName);
            }
            session.Flush();
            session.Clear();
        }

        public int ExecuteStatement(string sql)
        {
            if (configuration == null)
            {
                configuration = new Configuration();
            }

            using (var prov = ConnectionProviderFactory.NewConnectionProvider(configuration.Properties))
            {
                var conn = prov.GetConnection();

                try
                {
                    using (var tran = conn.BeginTransaction())
                    using (var comm = conn.CreateCommand())
                    {
                        comm.CommandText = sql;
                        comm.Transaction = tran;
                        comm.CommandType = CommandType.Text;
                        int result = comm.ExecuteNonQuery();
                        tran.Commit();
                        return result;
                    }
                }
                finally
                {
                    prov.CloseConnection(conn);
                }
            }
        }

        protected virtual void OnTestFixtureSetUp()
        { }

        protected virtual void OnTestFixtureTearDown()
        { }

        protected virtual void OnSetUp()
        { }

        protected virtual void OnTearDown()
        { }

        protected virtual void OnBeforeCreateSchema()
        { }

        protected virtual void OnAfterDropSchema()
        { }

        protected ISession OpenSession()
        {
            lastOpenedSession = sessions.OpenSession();
            return lastOpenedSession;
        }

        protected void ApplyCacheSettings(Configuration configuration)
        {
            if (CacheConcurrencyStrategy == null)
            {
                return;
            }

            foreach (var clazz in configuration.ClassMappings)
            {
                bool hasLob = false;
                foreach (var prop in clazz.PropertyClosureIterator)
                {
                    if (prop.Value.IsSimpleValue)
                    {
                        var type = ((SimpleValue) prop.Value).Type;
                        if (Equals(type, NHibernateUtil.BinaryBlob))
                        {
                            hasLob = true;
                        }
                    }
                }
                if (!hasLob && !clazz.IsInherited)
                {
                    configuration.SetCacheConcurrencyStrategy(clazz.MappedClass.Name, CacheConcurrencyStrategy);
                }
            }

            foreach (var coll in configuration.CollectionMappings)
            {
                configuration.SetCacheConcurrencyStrategy(coll.Role, CacheConcurrencyStrategy);
            }
        }

        private void ConfigureSpatialMetadata()
        {
            bool rebuildSessionFactory = false;

            if (spatialDialect.SupportsSpatialMetadata(MetadataClass.GeometryColumn))
            {
                Metadata.AddMapping(configuration, MetadataClass.GeometryColumn);
                rebuildSessionFactory = true;
            }

            if (spatialDialect.SupportsSpatialMetadata(MetadataClass.SpatialReferenceSystem))
            {
                Metadata.AddMapping(configuration, MetadataClass.SpatialReferenceSystem);
                rebuildSessionFactory = true;
            }

            if (rebuildSessionFactory)
            {
                sessions = configuration.BuildSessionFactory();
            }
        }

        private bool CheckSessionWasClosed()
        {
            if (lastOpenedSession != null && lastOpenedSession.IsOpen)
            {
                Log.Error("Test case didn't close a session, closing");
                lastOpenedSession.Close();
                return false;
            }

            return true;
        }

        private bool CheckDatabaseWasCleaned()
        {
            if (sessions.GetAllClassMetadata().Count == 0)
            {
                // Return early in the case of no mappings, also avoiding
                // a warning when executing the HQL below.
                return true;
            }

            bool empty = false;
            using (var s = sessions.OpenSession())
            {
                foreach (var type in Mappings)
                {
                    var objects = s.CreateQuery("from " + type.FullName).List();
                    empty = objects.Count == 0;
                    if (!empty)
                    {
                        break;
                    }
                }
            }

            if (!empty)
            {
                Log.Error("Test case didn't clean up the database after itself, re-creating the schema");
                DropSchema();
                CreateSchema();
            }

            return empty;
        }

        private bool CheckConnectionsWereClosed()
        {
            if (connectionProvider == null || !connectionProvider.HasOpenConnections)
            {
                return true;
            }

            Log.Error("Test case didn't close all open connections, closing");
            connectionProvider.CloseAllConnections();
            return false;
        }

        private void Configure()
        {
            configuration = CreateConfiguration();
        }

        private Configuration CreateConfiguration()
        {
            var configuration = new Configuration();

            Configure(configuration);

            foreach (var type in Mappings)
            {
                configuration.AddClass(type);
            }

            ApplyCacheSettings(configuration);

            return configuration;
        }

        private void CreateSchema()
        {
            OnBeforeCreateSchema();

            // Isolated configuration doesn't include SpatialReferenceSystem mapping,
            var configuration = CreateConfiguration();
            configuration.AddAuxiliaryDatabaseObject(new SpatialAuxiliaryDatabaseObject(configuration));
            new SchemaExport(configuration).Create(false, true);
        }

        private void DropSchema()
        {
            // Isolated configuration doesn't include SpatialReferenceSystem mapping,
            var configuration = CreateConfiguration();
            configuration.AddAuxiliaryDatabaseObject(new SpatialAuxiliaryDatabaseObject(configuration));
            new SchemaExport(configuration).Drop(false, true);
            OnAfterDropSchema();
        }

        private void BuildSessionFactory()
        {
            sessions = configuration.BuildSessionFactory();
            spatialDialect = (ISpatialDialect) ((ISessionFactoryImplementor) sessions).Dialect;
            connectionProvider = ((ISessionFactoryImplementor) sessions).ConnectionProvider as DebugConnectionProvider;
        }

        private void Cleanup()
        {
            sessions.Close();
            sessions = null;
            spatialDialect = null;
            connectionProvider = null;
            lastOpenedSession = null;
            configuration = null;
        }

        #region Properties overridable by subclasses

        protected virtual void Configure(Configuration configuration)
        { }

        protected virtual string CacheConcurrencyStrategy =>

            //get { return "nonstrict-read-write"; }
            null;

        protected virtual bool CheckDatabaseWasCleanedOnTearDown => true;

        #endregion Properties overridable by subclasses
    }
}
