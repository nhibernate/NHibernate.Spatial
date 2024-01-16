using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;

namespace Open.Topology.TestRunner.Result
{
    public interface IResult
    {
        bool Equals(IResult other, double tolerance);

        string ToShortString();

        string ToLongString();

        string ToFormattedString();
    }

    public interface IResult<T> : IResult
    {
        T Value { get; }
    }

    public class BooleanResult : IResult<bool>
    {
        public BooleanResult(bool result)
        {
            Value = result;
        }

        public bool Value { get; }

        public bool Equals(IResult other, double tolerance)
        {
            if (!(other is IResult<bool> result))
            {
                return false;
            }
            return Value == result.Value;
        }

        public string ToShortString()
        {
            return Value ? "true" : "false";
        }

        public string ToLongString()
        {
            return ToShortString();
        }

        public string ToFormattedString()
        {
            return ToShortString();
        }
    }

    public class DoubleResult : IResult<double>
    {
        public DoubleResult(double result)
        {
            Value = result;
        }

        public double Value { get; }

        public bool Equals(IResult other, double tolerance)
        {
            if (!(other is IResult<double> result))
            {
                return false;
            }
            return Math.Abs(Value - result.Value) <= tolerance;
        }

        public string ToShortString()
        {
            return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToLongString()
        {
            return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToFormattedString()
        {
            return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public class IntegerResult : IResult<int>
    {
        public IntegerResult(int result)
        {
            Value = result;
        }

        public int Value { get; }

        public bool Equals(IResult other, double tolerance)
        {
            if (!(other is IResult<int> result))
            {
                return false;
            }
            return Math.Abs(Value - result.Value) <= tolerance;
        }

        public string ToShortString()
        {
            return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToLongString()
        {
            return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToFormattedString()
        {
            return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public class GeometryResult : IResult<Geometry>
    {
        public GeometryResult(Geometry result)
        {
            Value = result;
        }

        public Geometry Value { get; }

        public bool Equals(IResult other, double tolerance)
        {
            if (!(other is IResult<Geometry> otherGeometryResult))
            {
                return false;
            }
            var otherGeometry = otherGeometryResult.Value;

            var thisGeometryClone = Value.Copy();
            var otherGeometryClone = otherGeometry.Copy();
            thisGeometryClone.Normalize();
            otherGeometryClone.Normalize();

            return thisGeometryClone.EqualsExact(otherGeometryClone, tolerance);
        }

        public string ToShortString()
        {
            return Value.GeometryType;
        }

        public string ToLongString()
        {
            return Value.AsText();
        }

        public string ToFormattedString()
        {
            var wktWriter = new WKTWriter { Formatted = true };
            return wktWriter.WriteFormatted(Value);
        }
    }
}
