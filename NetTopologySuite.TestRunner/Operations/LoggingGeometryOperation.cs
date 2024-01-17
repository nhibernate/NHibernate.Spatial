﻿using NetTopologySuite.Geometries;
using Open.Topology.TestRunner.Result;
using System;

namespace Open.Topology.TestRunner.Operations
{
    /// <summary>
    /// A <see cref="IGeometryOperation"/> which logs
    /// the input and output from another <see cref="IGeometryOperation"/>.
    /// The log is sent to <see cref="Console.Out"/>.
    /// </summary>
    /// <author>mbdavis</author>
    public class LoggingGeometryOperation : IGeometryOperation
    {
        private readonly IGeometryOperation _geomOp;

        public LoggingGeometryOperation()
            : this(new GeometryMethodOperation())
        { }

        public LoggingGeometryOperation(IGeometryOperation geomOp)
        {
            _geomOp = geomOp;
        }

        public Type GetReturnType(XmlTestType op)
        {
            return GetReturnType(op.ToString());
        }

        public Type GetReturnType(string opName)
        {
            return GeometryMethodOperation.GetGeometryReturnType(opName);
        }

        public IResult Invoke(XmlTestType opName, Geometry geometry, object[] args)
        {
            Console.WriteLine("Operation <" + opName + ">");
            Console.WriteLine("Geometry: " + geometry);
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("Arg[" + i + "]: " + args[i]);
            }
            var result = _geomOp.Invoke(opName, geometry, args);
            Console.WriteLine("Result==> " + result.ToFormattedString());
            return result;
        }
    }
}
