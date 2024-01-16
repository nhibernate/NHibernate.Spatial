using System;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.Models;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class PostGis20MiscTestsFixture : AbstractFixture
    {
        private ISession _session;

        protected override Type[] Mappings => new[]
        {
            typeof(PolygonZ),
            typeof(PolygonM),
            typeof(PolygonZM),
        };

        protected override bool CheckDatabaseWasCleanedOnTearDown => false;

        /// <summary>
        /// Test that Z values are properly round-tripped (see #120).
        /// </summary>
        [Test]
        public void PolygonZValuesAreStoredAndRetrievedCorrectly()
        {
            // Arrange
            const string wkt = "POLYGONZ ((0 0 1, 0 100 2, 100 100 3, 100 0 4, 0 0 5))";
            using (var session = sessions.OpenSession())
            {
                var obj = new PolygonZ
                {
                    Id = 1,
                    Geom = Wkt.Read(wkt)
                };
                session.Save(obj);
                session.Flush();
            }
            var expected = Wkt.Read(wkt);
            var expectedCoord = expected.Coordinate;

            // Act
            var polygonZ = _session.Load<PolygonZ>(1);
            var polygonZCoord = polygonZ.Geom.Coordinate;

            // Assert
            // NOTE: Cannot use EqualsTopologically or EqualsExact here since
            //       both ignore Z/M components
            Assert.AreEqual(expectedCoord.X, polygonZCoord.X);
            Assert.AreEqual(expectedCoord.Y, polygonZCoord.Y);
            Assert.AreEqual(expectedCoord.Z, polygonZCoord.Z);
        }

        /// <summary>
        /// Test that M values are properly round-tripped (see #120).
        /// </summary>
        [Test]
        public void PolygonMValuesAreStoredAndRetrievedCorrectly()
        {
            // Arrange
            const string wkt = "POLYGONM ((0 0 1, 0 100 2, 100 100 3, 100 0 4, 0 0 5))";
            using (var session = sessions.OpenSession())
            {
                var obj = new PolygonM
                {
                    Id = 1,
                    Geom = Wkt.Read(wkt)
                };
                session.Save(obj);
                session.Flush();
            }
            var expected = Wkt.Read(wkt);
            var expectedCoord = expected.Coordinate;

            // Act
            var polygonM = _session.Load<PolygonM>(1);
            var polygonMCoord = polygonM.Geom.Coordinate;

            // Assert
            // NOTE: Cannot use EqualsTopologically or EqualsExact here since
            //       both ignore Z/M components
            Assert.AreEqual(expectedCoord.X, polygonMCoord.X);
            Assert.AreEqual(expectedCoord.Y, polygonMCoord.Y);
            Assert.AreEqual(expectedCoord.M, polygonMCoord.M);
        }

        /// <summary>
        /// Test that ZM values are properly round-tripped (see #120).
        /// </summary>
        [Test]
        public void PolygonZMValuesAreStoredAndRetrievedCorrectly()
        {
            // Arrange
            const string wkt = "POLYGONZM ((0 0 1 11, 0 100 2 22, 100 100 3 33, 100 0 4 44, 0 0 5 55))";
            using (var session = sessions.OpenSession())
            {
                var obj = new PolygonZM
                {
                    Id = 1,
                    Geom = Wkt.Read(wkt)
                };
                session.Save(obj);
                session.Flush();
            }
            var expected = Wkt.Read(wkt);
            var expectedCoord = expected.Coordinate;

            // Act
            var polygonZM = _session.Load<PolygonZM>(1);
            var polygonZMCoord = polygonZM.Geom.Coordinate;

            // Assert
            // NOTE: Cannot use EqualsTopologically or EqualsExact here since
            //       both ignore Z/M components
            Assert.AreEqual(expectedCoord.X, polygonZMCoord.X);
            Assert.AreEqual(expectedCoord.Y, polygonZMCoord.Y);
            Assert.AreEqual(expectedCoord.Z, polygonZMCoord.Z);
            Assert.AreEqual(expectedCoord.M, polygonZMCoord.M);
        }

        protected override void Configure(Configuration config)
        {
            TestConfiguration.Configure(config);
        }

        protected override void OnTestFixtureSetUp()
        { }

        protected override void OnTestFixtureTearDown()
        {
            using (var session = sessions.OpenSession())
            {
                DeleteMappings(session);
            }
        }

        protected override void OnSetUp()
        {
            _session = sessions.OpenSession();
        }

        protected override void OnTearDown()
        {
            _session.Clear();
            _session.Close();
            _session.Dispose();
        }
    }
}
