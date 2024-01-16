using System;
using System.Globalization;

namespace Open.Topology.TestRunner.Operations
{
    public class ArgumentConverter
    {
        /*
        public ArgumentConverter()
        {
        }
         */

        public object[] Convert(Type[] parameterTypes, object[] args)
        {
            object[] actualArgs = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                actualArgs[i] = Convert(parameterTypes[i], args[i]);
            }
            return actualArgs;
        }

        public object Convert(Type destClass, object srcValue)
        {
            if (srcValue is string value)
            {
                return ConvertFromString(destClass, value);
            }
            if (destClass.IsInstanceOfType(srcValue))
            {
                return srcValue;
            }
            ThrowInvalidConversion(destClass, srcValue);
            return null;
        }

        private static object ConvertFromString(Type destClass, string src)
        {
            if (destClass == typeof(bool) || destClass == typeof(bool))
            {
                switch (src)
                {
                    case "true":
                        return true;
                    case "false":
                        return false;
                    default:
                        ThrowInvalidConversion(destClass, src);
                        break;
                }
            }
            else if (destClass == typeof(int) ||
                     destClass == typeof(int))
            {
                // try as an int
                if (int.TryParse(src, out int val))
                {
                    return val;
                }

                ThrowInvalidConversion(destClass, src);
                /*
                try
                {
                    return new Integer(src);
                }
                catch (FormatException e)
                {
                    // eat this exception - it will be reported below
                }
                 */
            }
            else if (destClass == typeof(double) ||
                     destClass == typeof(double))
            {
                // try as a double
                if (double.TryParse(src, NumberStyles.Any, CultureInfo.InvariantCulture, out double dval))
                {
                    return dval;
                }
                /*
                try
                {
                    return new Double(src);
                }
                catch (FormatException e)
                {
                    // eat this exception - it will be reported below
                }
                 */
            }
            else if (destClass == typeof(string))
            {
                return src;
            }
            ThrowInvalidConversion(destClass, src);
            return null;
        }

        private static void ThrowInvalidConversion(Type destClass, object srcValue)
        {
            throw new ArgumentException("Cannot convert " + srcValue + " to " + destClass);
        }
    }
}
