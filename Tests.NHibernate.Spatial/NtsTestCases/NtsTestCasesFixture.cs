using NetTopologySuite.Geometries;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Spatial.Criterion;
using NUnit.Framework;
using Open.Topology.TestRunner;
using System;
using System.IO;
using NHibernate.Spatial;
using Tests.NHibernate.Spatial.NtsTestCases.Model;

namespace Tests.NHibernate.Spatial.NtsTestCases
{
    /// <summary>
    /// This fixture reuses some NTS test runner data.
    /// </summary>
    public abstract class NtsTestCasesFixture : AbstractFixture
    {
        private const string DataPath = "../../../../Tests.NHibernate.Spatial/NtsTestCases/Data";

        protected ISession _session;

        protected override Type[] Mappings
        {
            get
            {
                return new[]
                {
                    typeof(NtsTestCase),
                };
            }
        }

        protected virtual string TestEqualsExactDataPath => Path.Combine(DataPath, "general", "TestEqualsExact.xml");

        protected virtual string TestFunctionAADataPath => Path.Combine(DataPath, "general", "TestFunctionAA.xml");

        protected virtual string TestFunctionAAPrecDataPath => Path.Combine(DataPath, "general", "TestFunctionAAPrec.xml");

        protected virtual string TestRelateAADataPath => Path.Combine(DataPath, "general", "TestRelateAA.xml");

        protected virtual string TestRelateACDataPath => Path.Combine(DataPath, "general", "TestRelateAC.xml");

        protected virtual string TestRectanglePredicateDataPath => Path.Combine(DataPath, "general", "TestRectanglePredicate.xml");

        protected virtual string TestSimpleDataPath => Path.Combine(DataPath, "general", "TestSimple.xml");

        protected virtual string TestValidDataPath => Path.Combine(DataPath, "general", "TestValid.xml");

        protected virtual string TestWithinDistanceDataPath => Path.Combine(DataPath, "general", "TestWithinDistance.xml");

        protected virtual string TestRelateACValidateDataPath => Path.Combine(DataPath, "validate", "TestRelateAC.xml");

        protected virtual string TestRelateLAValidateDataPath => Path.Combine(DataPath, "validate", "TestRelateLA.xml");

        protected override bool CheckDatabaseWasCleanedOnTearDown => false;

        protected override void OnTestFixtureSetUp()
        {
            using (var session = sessions.OpenSession())
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                long id = 0;

                // General
                LoadTestCases(session, ref id, Path.Combine(basePath, TestEqualsExactDataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestFunctionAADataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestFunctionAAPrecDataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestRelateAADataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestRelateACDataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestRectanglePredicateDataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestSimpleDataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestValidDataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestWithinDistanceDataPath));

                // Validate
                LoadTestCases(session, ref id, Path.Combine(basePath, TestRelateACValidateDataPath));
                LoadTestCases(session, ref id, Path.Combine(basePath, TestRelateLAValidateDataPath));
            }
        }

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
            _session = null;
        }

        /// <summary>
        /// Prepares an entity for saving.
        /// </summary>
        /// <param name="ntsTestCase"></param>
        private static void Prepare(NtsTestCase ntsTestCase)
        {
            ntsTestCase.GeometryA = Prepare(ntsTestCase.GeometryA);
            ntsTestCase.GeometryB = Prepare(ntsTestCase.GeometryB);
            ntsTestCase.GeometryResult = Prepare(ntsTestCase.GeometryResult);
        }

        /// <summary>
        /// Prepares a geometry for saving.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private static Geometry Prepare(Geometry geometry)
        {
            geometry = geometry == null
                ? GeometryCollection.Empty
                : ConvertToSqlGeometryType(geometry);
            geometry.SRID = -1;
            return geometry;
        }

        /// <summary>
        /// Some geometries are not OGC SQL Geometry Types,
        /// so we convert them to .
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private static Geometry ConvertToSqlGeometryType(Geometry geometry)
        {
            if (geometry is LinearRing ring)
            {
                return new Polygon(ring, (LinearRing[]) null);
            }
            return geometry;
        }

        private void LoadTestCases(ISession session, ref long id, string filename)
        {
            var document = new XmlTestDocument();
            document.LoadFile(filename);
            foreach (XmlTestCollection testCase in document.Tests)
            {
                foreach (XmlTest test in testCase)
                {
                    var ntsTestCase = new NtsTestCase();
                    switch (test.TestType)
                    {
                        case XmlTestType.Intersection:
                        case XmlTestType.Union:
                        case XmlTestType.Difference:
                        case XmlTestType.SymmetricDifference:
                        case XmlTestType.Boundary:
                        case XmlTestType.Centroid:
                        case XmlTestType.ConvexHull:
                        case XmlTestType.Envelope:
                        case XmlTestType.InteriorPoint:
                            ntsTestCase.GeometryResult = (Geometry) test.Result;
                            break;

                        case XmlTestType.Contains:
                        case XmlTestType.CoveredBy:
                        case XmlTestType.Covers:
                        case XmlTestType.Crosses:
                        case XmlTestType.Disjoint:
                        case XmlTestType.Equals:
                        case XmlTestType.EqualsExact:
                        case XmlTestType.EqualsNorm:
                        case XmlTestType.Intersects:
                        case XmlTestType.IsEmpty:
                        case XmlTestType.IsSimple:
                        case XmlTestType.IsValid:
                        case XmlTestType.Overlaps:
                        case XmlTestType.Touches:
                        case XmlTestType.Within:
                            ntsTestCase.BooleanResult = (bool) test.Result;
                            break;

                        case XmlTestType.IsWithinDistance:
                        case XmlTestType.Relate:
                            ntsTestCase.Parameter = (string) test.Argument2;
                            ntsTestCase.BooleanResult = (bool) test.Result;
                            break;

                        default:
                            continue;
                    }
                    ntsTestCase.Operation = test.TestType.ToString();
                    ntsTestCase.Description = testCase.Name + ": " + test.Description;

                    if (test.IsDefaultTarget)
                    {
                        ntsTestCase.GeometryA = test.A;
                        ntsTestCase.GeometryB = test.B;
                    }
                    else
                    {
                        ntsTestCase.GeometryA = test.B;
                        ntsTestCase.GeometryB = test.A;
                    }

                    ntsTestCase.Id = ++id;

                    Prepare(ntsTestCase);

                    session.Save(ntsTestCase);
                }
            }
            session.Flush();
        }

        #region Supporting test functions

        private delegate SpatialProjection SpatialProjectionBinaryDelegate(string propertyName, string anotherPropertyName);

        private delegate AbstractCriterion SpatialRelationCriterionDelegate(string propertyName, object anotherGeometry);

        protected delegate SpatialProjection SpatialProjectionUnaryDelegate(string propertyName);

        protected delegate AbstractCriterion SpatialCriterionUnaryDelegate(string propertyName);

        private void TestGeometryBinaryOperation(string operationCriterion, SpatialProjectionBinaryDelegate projection)
        {
            var results = _session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", operationCriterion))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property("Description"))
                    .Add(Projections.Property("GeometryResult"))
                    .Add(projection("GeometryA", "GeometryB"))
                )
                .List();

            Assert.Greater(results.Count, 0);

            foreach (object[] result in results)
            {
                string description = (string) result[0];
                var expected = (Geometry) result[1];
                var operation = (Geometry) result[2];

                expected.Normalize();
                operation.Normalize();

                bool equals = expected.EqualsExact(operation, 1.5);
                if (!equals)
                {
                    Console.WriteLine(operationCriterion + ": " + description);
                }

                //Assert.IsTrue(equals, description);
            }
        }

        private void TestGeometryUnaryOperation(string operationCriterion, SpatialProjectionUnaryDelegate projection)
        {
            var results = _session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", operationCriterion))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property("Description"))
                    .Add(Projections.Property("GeometryResult"))
                    .Add(projection("GeometryA"))
                )
                .List();

            Assert.Greater(results.Count, 0);

            foreach (object[] result in results)
            {
                string description = (string) result[0];
                var expected = (Geometry) result[1];
                var operation = (Geometry) result[2];

                expected.Normalize();
                operation.Normalize();

                bool equals = expected.EqualsExact(operation, 1.5);
                if (!equals)
                {
                    Console.WriteLine(operationCriterion + ": " + description);
                }

                //Assert.IsTrue(equals, description);
            }
        }

        private void TestBooleanBinaryOperation(string operationCriterion, SpatialProjectionBinaryDelegate projection, SpatialRelationCriterionDelegate criterion)
        {
            var results = _session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", operationCriterion))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property("Description"))
                    .Add(Projections.Property("BooleanResult"))
                    .Add(projection("GeometryA", "GeometryB"))
                )
                .List();

            Assert.Greater(results.Count, 0);

            long countTrue = 0;
            bool error = false;

            foreach (object[] result in results)
            {
                string description = (string) result[0];
                bool expected = (bool) result[1];
                bool operation = (bool) result[2];

                if (expected != operation)
                {
                    Console.WriteLine(description);
                    error = true;
                }

                if (operation)
                {
                    countTrue++;
                }
            }

            Assert.False(error);

            // RowCount uses "count(*)" which in PostgreSQL returns Int64 and
            // in MS SQL Server return Int32.
            long countRows = Convert.ToInt64(_session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", operationCriterion))
                .Add(criterion("GeometryA", "GeometryB"))
                .SetProjection(Projections.RowCount())
                .UniqueResult());

            Assert.AreEqual(countTrue, countRows);
        }

        protected void TestBooleanUnaryOperation(string operationCriterion, SpatialProjectionUnaryDelegate projection, SpatialCriterionUnaryDelegate criterion)
        {
            var results = _session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", operationCriterion))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property("Description"))
                    .Add(Projections.Property("BooleanResult"))
                    .Add(projection("GeometryA"))
                )
                .List();

            Assert.Greater(results.Count, 0);

            long countTrue = 0;

            foreach (object[] result in results)
            {
                string description = (string) result[0];
                bool expected = (bool) result[1];
                bool operation = (bool) result[2];

                Assert.AreEqual(expected, operation, description);

                if (operation)
                {
                    countTrue++;
                }
            }

            // RowCount uses "count(*)" which in PostgreSQL returns Int64 and
            // in MS SQL Server return Int32.
            long countRows = Convert.ToInt64(_session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", operationCriterion))
                .Add(criterion("GeometryA"))
                .SetProjection(Projections.RowCount())
                .UniqueResult());

            Assert.AreEqual(countTrue, countRows);
        }

        #endregion Supporting test functions

        #region Analysis

        [Test]
        public void Intersection()
        {
            TestGeometryBinaryOperation("Intersection", SpatialProjections.Intersection);
        }

        [Test]
        public void Union()
        {
            TestGeometryBinaryOperation("Union", SpatialProjections.Union);
        }

        [Test]
        public void Difference()
        {
            TestGeometryBinaryOperation("Difference", SpatialProjections.Difference);
        }

        [Test]
        public void SymmetricDifference()
        {
            TestGeometryBinaryOperation("SymmetricDifference", SpatialProjections.SymDifference);
        }

        [Test]
        public void ConvexHull()
        {
            TestGeometryUnaryOperation("ConvexHull", SpatialProjections.ConvexHull);
        }

        #endregion Analysis

        #region Relations

        [Test]
        public virtual void BooleanRelate()
        {
            var results = _session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", "Relate"))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property("Id"))
                    .Add(Projections.Property("Description"))
                    .Add(Projections.Property("Parameter"))
                    .Add(Projections.Property("BooleanResult"))
                    .Add(SpatialProjections.Relate("GeometryA", "GeometryB", "Parameter"))
                )
                .List();

            Assert.Greater(results.Count, 0);

            foreach (object[] result in results)
            {
                long id = (long) result[0];
                string description = (string) result[1];
                string parameter = (string) result[2];
                bool expected = (bool) result[3];
                bool operation = (bool) result[4];

                Assert.AreEqual(expected, operation);

                // Spatial restriction
                long rowCount = _session.CreateCriteria(typeof(NtsTestCase))
                    .Add(Restrictions.Eq("Id", id))
                    .Add(SpatialRestrictions.Relate("GeometryA", "GeometryB", parameter))
                    .SetProjection(Projections.RowCountInt64())
                    .UniqueResult<long>();

                Assert.AreEqual(expected, Convert.ToBoolean(rowCount));
            }
        }

        [Test]
        public virtual void StringRelate()
        {
            var results = _session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", "Relate"))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property("Description"))
                    .Add(Projections.Property("Parameter"))
                    .Add(SpatialProjections.Relate("GeometryA", "GeometryB"))
                )
                .List();

            Assert.Greater(results.Count, 0);

            foreach (object[] result in results)
            {
                string description = (string) result[0];
                string expected = (string) result[1];
                string operation = (string) result[2];

                Assert.AreEqual(expected, operation);
            }
        }

        [Test]
        public void Contains()
        {
            TestBooleanBinaryOperation("Contains", SpatialProjections.Contains, SpatialRestrictions.Contains);
        }

        [Test]
        public virtual void CoveredBy()
        {
            TestBooleanBinaryOperation("CoveredBy", SpatialProjections.CoveredBy, SpatialRestrictions.CoveredBy);
        }

        [Test]
        public virtual void Covers()
        {
            TestBooleanBinaryOperation("Covers", SpatialProjections.Covers, SpatialRestrictions.Covers);
        }

        [Test]
        public void Crosses()
        {
            TestBooleanBinaryOperation("Crosses", SpatialProjections.Crosses, SpatialRestrictions.Crosses);
        }

        [Test]
        public void Disjoint()
        {
            TestBooleanBinaryOperation("Disjoint", SpatialProjections.Disjoint, SpatialRestrictions.Disjoint);
        }

        [Test]
        public void Equals()
        {
            TestBooleanBinaryOperation("EqualsNorm", SpatialProjections.Equals, SpatialRestrictions.Eq);
        }

        [Test]
        public virtual void EqualsExact()
        {
            TestBooleanBinaryOperation("EqualsExact", SpatialProjections.EqualsExact, SpatialRestrictions.EqExact);
        }

        [Test]
        public void Intersects()
        {
            TestBooleanBinaryOperation("Intersects", SpatialProjections.Intersects, SpatialRestrictions.Intersects);
        }

        [Test]
        public void Overlaps()
        {
            TestBooleanBinaryOperation("Overlaps", SpatialProjections.Overlaps, SpatialRestrictions.Overlaps);
        }

        [Test]
        public void Touches()
        {
            TestBooleanBinaryOperation("Touches", SpatialProjections.Touches, SpatialRestrictions.Touches);
        }

        [Test]
        public virtual void Within()
        {
            TestBooleanBinaryOperation("Within", SpatialProjections.Within, SpatialRestrictions.Within);
        }

        [Test]
        public virtual void IsWithinDistance()
        {
            var testCases = _session.QueryOver<NtsTestCase>()
                .Where(x => x.Operation == SpatialRelation.IsWithinDistance.ToString())
                .List();

            Assert.Greater(testCases.Count, 0);

            foreach (var testCase in testCases)
            {
                double distance = double.Parse(testCase.Parameter);

                // Spatial projection
                object[] result = _session.CreateCriteria(typeof(NtsTestCase))
                    .Add(Restrictions.Eq(nameof(testCase.Id), testCase.Id))
                    .SetProjection(Projections.ProjectionList()
                        .Add(Projections.Property("BooleanResult"))
                        .Add(SpatialProjections.IsWithinDistance("GeometryA", "GeometryB", distance))
                    )
                    .UniqueResult<object[]>();

                bool expected = (bool) result[0];
                bool operation = (bool) result[1];
                Assert.AreEqual(expected, operation);

                // Spatial restriction
                long rowCount = _session.CreateCriteria(typeof(NtsTestCase))
                    .Add(Restrictions.Eq(nameof(testCase.Id), testCase.Id))
                    .Add(SpatialRestrictions.IsWithinDistance("GeometryA", "GeometryB", distance))
                    .SetProjection(Projections.RowCountInt64())
                    .UniqueResult<long>();

                Assert.AreEqual(testCase.BooleanResult, Convert.ToBoolean(rowCount));
            }
        }

        #endregion Relations

        #region Validations

        [Test]
        [Ignore("No data to test")]
        public void IsClosed()
        {
            TestBooleanUnaryOperation("IsClosed", SpatialProjections.IsClosed, SpatialRestrictions.IsClosed);
        }

        [Test]
        [Ignore("No data to test")]
        public void IsEmpty()
        {
            TestBooleanUnaryOperation("IsEmpty", SpatialProjections.IsEmpty, SpatialRestrictions.IsEmpty);
        }

        [Test]
        [Ignore("No data to test")]
        public void IsRing()
        {
            TestBooleanUnaryOperation("IsRing", SpatialProjections.IsRing, SpatialRestrictions.IsRing);
        }

        [Test]
        public virtual void IsSimple()
        {
            TestBooleanUnaryOperation("IsSimple", SpatialProjections.IsSimple, SpatialRestrictions.IsSimple);
        }

        [Test]
        public virtual void IsValid()
        {
            TestBooleanUnaryOperation("IsValid", SpatialProjections.IsValid, SpatialRestrictions.IsValid);
        }

        #endregion Validations
    }
}
