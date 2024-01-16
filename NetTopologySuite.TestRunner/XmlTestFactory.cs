using System;
using System.Globalization;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Open.Topology.TestRunner.Operations;
using Open.Topology.TestRunner.Result;
using Open.Topology.TestRunner.Utility;

namespace Open.Topology.TestRunner
{
    /// <summary>
    /// Summary description for XmlTestFactory.
    /// </summary>
    public class XmlTestFactory
    {
        protected GeometryFactory ObjGeometryFactory;
        private static NumberFormatInfo _nfi;
        private readonly WKTOrWKBReader _objReader;
        private readonly IGeometryOperation _geometryOperation;
        private readonly IResultMatcher _resultMatcher;

        public XmlTestFactory(PrecisionModel pm, IGeometryOperation geometryOperation, IResultMatcher resultMatcher)
        {
            var geometryServices = new NtsGeometryServices(pm);
            _geometryOperation = geometryOperation;
            _resultMatcher = resultMatcher;
            _objReader = new WKTOrWKBReader(geometryServices);
        }

        protected enum Target
        {
            A = 1,
            B = 2,
            C = 3
        }

        public XmlTest Create(XmlTestInfo testInfo, double tolerance)
        {
            var xmlTest = new XmlTest(testInfo.GetValue("desc"),
                testInfo.IsDefaultTarget(), tolerance, _geometryOperation, _resultMatcher);

            // Handle test type or name.
            string strTestType = testInfo.GetValue("name");
            if (string.IsNullOrEmpty(strTestType))
            {
                return null;
            }

            ParseType(strTestType, xmlTest);

            // Handle the Geometry A:
            string wkt = testInfo.GetValue("a");
            if (!string.IsNullOrEmpty(wkt))
            {
                ParseGeometry(Target.A, wkt, xmlTest);
            }

            // Handle the Geometry B:
            wkt = testInfo.GetValue("b");
            if (!string.IsNullOrEmpty(wkt))
            {
                ParseGeometry(Target.B, wkt, xmlTest);
            }

            string arg2 = testInfo.GetValue("arg2");
            if (!string.IsNullOrEmpty(arg2))
            {
                switch (arg2)
                {
                    case "a":
                    case "A":
                        xmlTest.Argument1 = xmlTest.A;
                        break;
                    case "b":
                    case "B":
                        xmlTest.Argument1 = xmlTest.B;
                        break;
                    default:
                        xmlTest.Argument1 = arg2;
                        break;
                }
            }

            string arg3 = testInfo.GetValue("arg3");
            if (!string.IsNullOrEmpty(arg3))
            {
                xmlTest.Argument2 = arg3;
            }

            string strResult = testInfo.GetValue("result");
            if (string.IsNullOrEmpty(strResult))
            {
                return null;
            }

            ParseResult(strResult, xmlTest);

            return xmlTest;
        }

        protected static IFormatProvider GetNumberFormatInfo()
        {
            if (_nfi == null)
            {
                _nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
            }
            return _nfi;
        }

        protected bool ParseType(string testType, XmlTest xmlTestItem)
        {
            testType = testType.ToLower();

            switch (testType)
            {
                case "getarea":
                    xmlTestItem.TestType = XmlTestType.Area;
                    break;
                case "getboundary":
                    xmlTestItem.TestType = XmlTestType.Boundary;
                    break;
                case "getboundarydimension":
                    xmlTestItem.TestType = XmlTestType.BoundaryDimension;
                    break;
                case "buffer":
                    xmlTestItem.TestType = XmlTestType.Buffer;
                    break;
                case "buffermitredjoin":
                    xmlTestItem.TestType = XmlTestType.BufferMitredJoin;
                    break;
                case "getcentroid":
                    xmlTestItem.TestType = XmlTestType.Centroid;
                    break;
                case "contains":
                    xmlTestItem.TestType = XmlTestType.Contains;
                    break;
                case "convexhull":
                    xmlTestItem.TestType = XmlTestType.ConvexHull;
                    break;
                case "crosses":
                    xmlTestItem.TestType = XmlTestType.Crosses;
                    break;
                case "densify":
                    xmlTestItem.TestType = XmlTestType.Densify;
                    break;
                case "difference":
                    xmlTestItem.TestType = XmlTestType.Difference;
                    break;
                case "getdimension":
                    xmlTestItem.TestType = XmlTestType.Dimension;
                    break;
                case "disjoint":
                    xmlTestItem.TestType = XmlTestType.Disjoint;
                    break;
                case "distance":
                    xmlTestItem.TestType = XmlTestType.Distance;
                    break;
                case "getenvelope":
                    xmlTestItem.TestType = XmlTestType.Envelope;
                    break;
                case "equals":
                    xmlTestItem.TestType = XmlTestType.Equals;
                    break;
                case "getinteriorpoint":
                    xmlTestItem.TestType = XmlTestType.InteriorPoint;
                    break;
                case "intersection":
                    xmlTestItem.TestType = XmlTestType.Intersection;
                    break;
                case "intersects":
                    xmlTestItem.TestType = XmlTestType.Intersects;
                    break;
                case "isempty":
                    xmlTestItem.TestType = XmlTestType.IsEmpty;
                    break;
                case "issimple":
                    xmlTestItem.TestType = XmlTestType.IsSimple;
                    break;
                case "isvalid":
                    xmlTestItem.TestType = XmlTestType.IsValid;
                    break;
                case "iswithindistance":
                    xmlTestItem.TestType = XmlTestType.IsWithinDistance;
                    break;
                case "getlength":
                    xmlTestItem.TestType = XmlTestType.Length;
                    break;
                case "getnumpoints":
                    xmlTestItem.TestType = XmlTestType.NumPoints;
                    break;
                case "overlaps":
                    xmlTestItem.TestType = XmlTestType.Overlaps;
                    break;
                case "relate":
                    xmlTestItem.TestType = XmlTestType.Relate;
                    break;
                case "getsrid":
                    xmlTestItem.TestType = XmlTestType.SRID;
                    break;
                case "symmetricdifference":
                case "symdifference":
                    xmlTestItem.TestType = XmlTestType.SymmetricDifference;
                    break;
                case "touches":
                    xmlTestItem.TestType = XmlTestType.Touches;
                    break;
                case "union":
                    xmlTestItem.TestType = XmlTestType.Union;
                    break;
                case "within":
                    xmlTestItem.TestType = XmlTestType.Within;
                    break;
                case "covers":
                    xmlTestItem.TestType = XmlTestType.Covers;
                    break;
                case "coveredby":
                    xmlTestItem.TestType = XmlTestType.CoveredBy;
                    break;
                case "equalsexact":
                    xmlTestItem.TestType = XmlTestType.EqualsExact;
                    break;
                case "equalsnorm":
                    xmlTestItem.TestType = XmlTestType.EqualsNorm;
                    break;
                case "minclearance":
                    xmlTestItem.TestType = XmlTestType.MinClearance;
                    break;
                case "minclearanceline":
                    xmlTestItem.TestType = XmlTestType.MinClearanceLine;
                    break;
                default:
                    throw new ArgumentException($"The operation type \"{testType}\" is not valid: ");
            }

            return true;
        }

        protected bool ParseResult(string result, XmlTest xmlTestItem)
        {
            switch (xmlTestItem.TestType)
            {
                // Here we expect double
                case XmlTestType.Area:
                case XmlTestType.Distance:
                case XmlTestType.Length:
                case XmlTestType.MinClearance:
                {
                    try
                    {
                        xmlTestItem.Result = double.Parse(result, GetNumberFormatInfo());
                        return true;
                    }
                    catch (Exception ex)
                    {
                        XmlTestExceptionManager.Publish(ex);
                        return false;
                    }
                }

                // Here we expect integer
                case XmlTestType.BoundaryDimension:
                case XmlTestType.Dimension:
                case XmlTestType.NumPoints:
                case XmlTestType.SRID:
                {
                    try
                    {
                        xmlTestItem.Result = int.Parse(result);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        XmlTestExceptionManager.Publish(ex);
                        return false;
                    }
                }

                // Here we expect a point
                case XmlTestType.Boundary:
                case XmlTestType.Buffer:
                case XmlTestType.BufferMitredJoin:
                case XmlTestType.Centroid:
                case XmlTestType.ConvexHull:
                case XmlTestType.Densify:
                case XmlTestType.Difference:
                case XmlTestType.Envelope:
                case XmlTestType.InteriorPoint:
                case XmlTestType.Intersection:
                case XmlTestType.SymmetricDifference:
                case XmlTestType.Union:
                case XmlTestType.MinClearanceLine:
                {
                    try
                    {
                        xmlTestItem.Result = _objReader.Read(result);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        XmlTestExceptionManager.Publish(ex);
                        return false;
                    }
                }

                // Here we expect boolean
                case XmlTestType.Contains:
                case XmlTestType.Crosses:
                case XmlTestType.Disjoint:
                case XmlTestType.Equals:
                case XmlTestType.Intersects:
                case XmlTestType.IsEmpty:
                case XmlTestType.IsSimple:
                case XmlTestType.IsValid:
                case XmlTestType.IsWithinDistance:
                case XmlTestType.Overlaps:
                case XmlTestType.Relate:
                case XmlTestType.Touches:
                case XmlTestType.Within:
                case XmlTestType.Covers:
                case XmlTestType.CoveredBy:
                case XmlTestType.EqualsExact:
                case XmlTestType.EqualsNorm:
                {
                    try
                    {
                        xmlTestItem.Result = bool.Parse(result);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        XmlTestExceptionManager.Publish(ex);
                        return false;
                    }
                }

                default:
                    string format = $"Test not implemented: {xmlTestItem.TestType}";
                    throw new NotImplementedException(format);
            }
        }

        protected bool ParseGeometry(Target targetType, string targetText, XmlTest xmlTestItem)
        {
            Geometry geom;
            try
            {
                geom = _objReader.Read(targetText);
            }
            catch (Exception ex)
            {
                xmlTestItem.Thrown = ex;
                XmlTestExceptionManager.Publish(ex);
                return false;
            }

            if (geom == null)
            {
                return false;
            }

            switch (targetType)
            {
                case Target.A:
                    xmlTestItem.A = geom;
                    break;

                case Target.B:
                    xmlTestItem.B = geom;
                    break;
            }

            return true;
        }
    }
}
