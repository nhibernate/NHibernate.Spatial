﻿using NetTopologySuite.Geometries;
using Open.Topology.TestRunner.Result;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Open.Topology.TestRunner.Operations
{
    /**
     * Invokes a named operation on a set of arguments,
     * the first of which is a {@link Geometry}.
     * This class provides operations which are the methods
     * defined on the Geometry class.
     * Other {@link GeometryOperation} classes can delegate to
     * instances of this class to run standard Geometry methods.
     *
     * @author Martin Davis
     * @version 1.7
     */
    public class GeometryMethodOperation : IGeometryOperation
    {
        private static readonly MethodInfo[] GeometryMethods = typeof(Geometry).GetMethods();

        private readonly object[] _convArg = new object[1];

        public static bool IsBooleanFunction(string name)
        {
            return GetGeometryReturnType(name) == typeof(bool);
        }

        public static bool IsIntegerFunction(string name)
        {
            return GetGeometryReturnType(name) == typeof(int);
        }

        public static bool IsDoubleFunction(string name)
        {
            return GetGeometryReturnType(name) == typeof(double);
        }

        public static bool IsGeometryFunction(string name)
        {
            return typeof(Geometry).IsAssignableFrom(GetGeometryReturnType(name));
        }

        public static Type GetGeometryReturnType(string functionName)
        {
            foreach (var geometryMethod in GeometryMethods)
            {
                if (geometryMethod.Name.Equals(functionName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var returnClass = geometryMethod.ReturnType;
                    /*
                     * Filter out only acceptable classes. (For instance, don't accept the
                     * relate()=>IntersectionMatrix method)
                     */
                    if (returnClass == typeof(bool)
                        || typeof(Geometry).IsAssignableFrom(returnClass)
                        || returnClass == typeof(double) || returnClass == typeof(int))
                    {
                        return returnClass;
                    }
                }
            }
            return null;
        }

        public Type GetReturnType(XmlTestType op)
        {
            return GetReturnType(op.ToString());
        }

        public Type GetReturnType(string opName)
        {
            return GetGeometryReturnType(opName);
        }

        public IResult Invoke(XmlTestType opName, Geometry geometry, object[] args)
        {
            return Invoke(opName.ToString(), geometry, args);
        }

        public IResult Invoke(string opName, Geometry geometry, object[] args)
        {
            object[] actualArgs = new object[args.Length];
            var geomMethod = GetGeometryMethod(opName, args, actualArgs);
            if (geomMethod == null)
            {
                throw new NTSTestReflectionException(opName, args);
            }
            return InvokeMethod(geomMethod, geometry, actualArgs);
        }

        private static int NonNullItemCount(object[] objects)
        {
            return objects.Count(obj => obj != null);
        }

        private MethodInfo GetGeometryMethod(string opName, object[] args, object[] actualArgs)
        {
            // could index methods by name for efficiency...
            foreach (var geometryMethod in GeometryMethods)
            {
                if (!geometryMethod.Name.Equals(opName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                if (ConvertArgs(geometryMethod.GetParameters(), args, actualArgs))
                {
                    return geometryMethod;
                }
            }
            return null;
        }

        private bool ConvertArgs(ParameterInfo[] parameterTypes, object[] args, object[] actualArgs)
        {
            if (parameterTypes.Length != NonNullItemCount(args))
            {
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                bool isCompatible = ConvertArg(parameterTypes[i].ParameterType, args[i], _convArg);
                if (!isCompatible)
                {
                    return false;
                }
                actualArgs[i] = _convArg[0];
            }
            return true;
        }

        private bool ConvertArg(Type destClass, object srcValue, object[] convArg)
        {
            convArg[0] = null;
            if (srcValue is string value)
            {
                return convertArgFromString(destClass, value, convArg);
            }
            if (destClass.IsInstanceOfType(srcValue))
            {
                convArg[0] = srcValue;
                return true;
            }
            return false;
        }

        private bool convertArgFromString(Type destClass, string srcStr, object[] convArg)
        {
            convArg[0] = null;
            if (destClass == typeof(bool) || destClass == typeof(bool))
            {
                switch (srcStr)
                {
                    case "true":
                        convArg[0] = true;
                        return true;
                    case "false":
                        convArg[0] = false;
                        return true;
                    default:
                        return false;
                }
            }

            if (destClass == typeof(int) || destClass == typeof(int))
            {
                // try as an int
                try
                {
                    convArg[0] = int.Parse(srcStr, NumberStyles.Any, CultureInfo.InvariantCulture);
                    return true;
                }
                catch (FormatException)
                {
                    // eat this exception
                }
                return false;
            }

            if (destClass == typeof(double) || destClass == typeof(double))
            {
                // try as an int
                try
                {
                    convArg[0] = double.Parse(srcStr, NumberStyles.Any, CultureInfo.InvariantCulture);
                    return true;
                }
                catch (FormatException)
                {
                    // eat this exception
                }
                return false;
            }
            if (destClass == typeof(string) || destClass == typeof(string))
            {
                convArg[0] = srcStr;
                return true;
            }
            return false;
        }

        private IResult InvokeMethod(MethodInfo method, Geometry geometry, object[] args)
        {
            try
            {
                if (method.ReturnType == typeof(bool))
                {
                    return new BooleanResult((bool) method.Invoke(geometry, args));
                }
                if (typeof(Geometry).IsAssignableFrom(method.ReturnType))
                {
                    return new GeometryResult((Geometry) method.Invoke(geometry, args));
                }
                if (method.ReturnType == typeof(double))
                {
                    return new DoubleResult((double) method.Invoke(geometry, args));
                }
                if (method.ReturnType == typeof(int))
                {
                    return new IntegerResult((int) method.Invoke(geometry, args));
                }
            }
            catch (TargetInvocationException e)
            {
                var t = e.InnerException;
                throw t;
            }
            throw new NTSTestReflectionException("Unsupported result type: " + method.ReturnType);
        }
    }
}
