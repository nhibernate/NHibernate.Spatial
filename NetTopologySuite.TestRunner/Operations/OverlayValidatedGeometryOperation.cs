﻿using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Overlay;
using NetTopologySuite.Operation.Overlay.Validate;
using Open.Topology.TestRunner.Result;
using System;

namespace Open.Topology.TestRunner.Operations
{
    /**
     * A {@link GeometryOperation} which validates the result of overlay operations.
     * If an invalid result is found, an exception is thrown (this is the most
     * convenient and noticeable way of flagging the problem when using the TestRunner).
     * All other Geometry methods are executed normally.
     * <p>
     * In order to eliminate the need to specify the precise result of an overlay,
     * this class forces the final return value to be <tt>GEOMETRYCOLLECTION EMPTY</tt>.
     * <p>
     * This class can be used via the <tt>-geomop</tt> command-line option
     * or by the <tt>&lt;geometryOperation&gt;</tt> XML test file setting.
     *
     * @author Martin Davis
     *
     */
    public class OverlayValidatedGeometryOperation : IGeometryOperation
    {
        private const bool ReturnEmptyGeometryCollection = true;

        private const double AreaDiffTol = 5.0;

        private readonly GeometryMethodOperation _chainOp = new GeometryMethodOperation();

        public OverlayValidatedGeometryOperation()
        { }

        /// <summary>
        /// Creates a new operation which chains to the given {@link GeometryMethodOperation}
        /// for non-intercepted methods.
        /// </summary>
        /// <param name="chainOp">the operation to chain to</param>
        public OverlayValidatedGeometryOperation(GeometryMethodOperation chainOp)
        {
            _chainOp = chainOp;
        }

        public static SpatialFunction OverlayOpCode(string methodName)
        {
            if (methodName.Equals("intersection", StringComparison.InvariantCultureIgnoreCase))
            {
                return SpatialFunction.Intersection;
            }
            if (methodName.Equals("union", StringComparison.InvariantCultureIgnoreCase))
            {
                return SpatialFunction.Union;
            }
            if (methodName.Equals("difference", StringComparison.InvariantCultureIgnoreCase))
            {
                return SpatialFunction.Difference;
            }
            if (methodName.Equals("symDifference", StringComparison.InvariantCultureIgnoreCase))
            {
                return SpatialFunction.SymDifference;
            }
            throw new ArgumentOutOfRangeException(nameof(methodName));
        }

        public static double AreaDiff(Geometry g0, Geometry g1)
        {
            double areaA = g0.Area;
            double areaAdiffB = g0.Difference(g1).Area;
            double areaAintB = g0.Intersection(g1).Area;
            return areaA - areaAdiffB - areaAintB;
        }

        public static Geometry InvokeGeometryOverlayMethod(SpatialFunction opCode, Geometry g0, Geometry g1)
        {
            switch (opCode)
            {
                case SpatialFunction.Intersection:
                    return g0.Intersection(g1);

                case SpatialFunction.Union:
                    return g0.Union(g1);

                case SpatialFunction.Difference:
                    return g0.Difference(g1);

                case SpatialFunction.SymDifference:
                    return g0.SymmetricDifference(g1);
            }
            throw new ArgumentException(@"Unknown overlay op code");
        }

        public Type GetReturnType(XmlTestType op)
        {
            return GetReturnType(op.ToString());
        }

        public Type GetReturnType(string opName)
        {
            return _chainOp.GetReturnType(opName);
        }

        /// <summary>
        /// Invokes the named operation.
        /// </summary>
        /// <param name="opName">The name of the operation</param>
        /// <param name="geometry">The geometry to process</param>
        /// <param name="args">The arguments to the operation (which may be typed as Strings)</param>
        /// <exception cref="Exception">If some error was encountered trying to find or process the operation</exception>
        public IResult Invoke(XmlTestType opName, Geometry geometry, object[] args)
        {
            return Invoke(opName.ToString(), geometry, args);
        }

        public IResult Invoke(string opName, Geometry geometry, object[] args)
        {
            var opCode = OverlayOpCode(opName);

            // if not an overlay op, do the default
            if (opCode < 0)
            {
                return _chainOp.Invoke(opName, geometry, args);
            }
            return InvokeValidatedOverlayOp(opCode, geometry, args);
        }

        /**
         *
         * and optionally validating the result.
         *
         * @param opCode
         * @param g0
         * @param args
         * @return
         * @throws Exception
         */
        /// <summary>
        /// Invokes an overlay op, optionally using snapping,
        /// and optionally validating the result.
        /// </summary>
        public IResult InvokeValidatedOverlayOp(SpatialFunction opCode, Geometry g0, object[] args)
        {
            var g1 = (Geometry) args[0];

            var result = InvokeGeometryOverlayMethod(opCode, g0, g1);

            // validate
            Validate(opCode, g0, g1, result);
            AreaValidate(g0, g1);

            /*
             * Return an empty GeometryCollection as the result.
             * This allows the test case to avoid specifying an exact result
             */
            if (ReturnEmptyGeometryCollection)
            {
                result = result.Factory.CreateGeometryCollection(null);
            }

            return new GeometryResult(result);
        }

        private static void Validate(SpatialFunction opCode, Geometry g0, Geometry g1, Geometry result)
        {
            var validator = new OverlayResultValidator(g0, g1, result);

            // check if computed result is valid
            if (!validator.IsValid(opCode))
            {
                var invalidLoc = validator.InvalidLocation;
                string msg = "Operation result is invalid [OverlayResultValidator] ( " + WKTWriter.ToPoint(invalidLoc) + " )";
                ReportError(msg);
            }
        }

        private static void AreaValidate(Geometry g0, Geometry g1)
        {
            double areaDiff = AreaDiff(g0, g1);
            if (Math.Abs(areaDiff) > AreaDiffTol)
            {
                string msg = "Operation result is invalid [AreaTest] (" + areaDiff + ")";
                ReportError(msg);
            }
        }

        private static void ReportError(string msg)
        {
            throw new Exception(msg);
        }
    }
}
