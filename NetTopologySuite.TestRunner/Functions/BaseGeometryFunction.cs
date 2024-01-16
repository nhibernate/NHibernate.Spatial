using NetTopologySuite.Geometries;
using Open.Topology.TestRunner.Utility;
using System;
using System.Text;

namespace Open.Topology.TestRunner.Functions
{
    /// <summary>
    /// A base for implementations of
    /// <see cref="IGeometryFunction"/> which provides most
    /// of the required structure.
    /// Extenders must supply the behaviour for the
    /// actual function invocation.
    /// </summary>
    /// <author>Martin Davis</author>
    public abstract class BaseGeometryFunction : IGeometryFunction, IComparable<IGeometryFunction>
    {
        protected string category;
        protected string name;
        protected string[] parameterNames;
        protected Type[] parameterTypes;
        protected Type returnType;

        protected BaseGeometryFunction(
            string category,
            string name,
            string[] parameterNames,
            Type[] parameterTypes,
            Type returnType)
        {
            this.category = category;
            this.name = name;
            this.parameterNames = parameterNames;
            this.parameterTypes = parameterTypes;
            this.returnType = returnType;
        }

        public string Category => category;

        public string Name => name;

        public string[] ParameterNames => parameterNames;

        public Type[] ParameterTypes => parameterTypes;

        public Type ReturnType => returnType;

        public string Signature
        {
            get
            {
                var paramTypes = new StringBuilder();
                paramTypes.Append("Geometry");
                foreach (var type in parameterTypes)
                {
                    paramTypes.Append(",");
                    paramTypes.Append(ClassUtility.GetClassname(type));
                }
                return name + "(" + paramTypes + ")"
                       + " -> "
                       + ClassUtility.GetClassname(returnType);
            }
        }

        public static bool IsBinaryGeomFunction(IGeometryFunction func)
        {
            return func.ParameterTypes.Length >= 1
                   && typeof(Geometry).IsAssignableFrom(func.ParameterTypes[0]);
        }

        public abstract object Invoke(Geometry geom, object[] args);

        /// <summary>
        /// Two functions are the same if they have the
        /// same signature (name, parameter types and return type).
        /// </summary>
        /// <returns>true if this object is the same as the <tt>obj</tt> argument</returns>
        public int CompareTo(IGeometryFunction o)
        {
            int cmp = name.CompareTo(o.Name);
            if (cmp != 0)
            {
                return cmp;
            }
            return CompareTo(returnType, o.ReturnType);

            //TODO: compare parameter lists as well
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IGeometryFunction func))
            {
                return false;
            }
            if (!name.Equals(func.Name))
            {
                return false;
            }
            if (returnType != func.ReturnType)
            {
                return false;
            }

            var funcParamTypes = func.ParameterTypes;
            if (parameterTypes.Length != funcParamTypes.Length)
            {
                return false;
            }
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (parameterTypes[i] != funcParamTypes[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected static double? GetDoubleOrNull(object[] args, int index)
        {
            if (args.Length <= index)
            {
                return null;
            }
            if (args[index] == null)
            {
                return null;
            }
            return (double) args[index];
        }

        protected static int? GetIntegerOrNull(object[] args, int index)
        {
            if (args.Length <= index)
            {
                return null;
            }
            if (args[index] == null)
            {
                return null;
            }
            return (int) args[index];
        }

        private static int CompareTo(Type c1, Type c2)
        {
            return c1.Name.CompareTo(c2.Name);
        }
    }
}
