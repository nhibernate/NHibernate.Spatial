using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Open.Topology.TestRunner.Utility
{
    /// <summary>
    /// Reads a <seealso cref="Geometry"/> from a string which is in either WKT or WKBHex format
    /// </summary>
    public class WKTOrWKBReader
    {
        private const int MaxCharsToCheck = 6;

        private readonly WKTReader _wktReader;

        private readonly WKBReader _wkbReader;

        public WKTOrWKBReader(NtsGeometryServices ntsGeometryServices)
        {
            _wktReader = new WKTReader(ntsGeometryServices)
            {
                IsOldNtsCoordinateSyntaxAllowed = false
            };
#pragma warning disable 612
            _wkbReader = new WKBReader();
#pragma warning restore 612
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomStr"></param>
        /// <returns></returns>
        /// <exception cref="ParseException"></exception>
        public Geometry Read(string geomStr)
        {
            string trimStr = geomStr.Trim();
            if (IsHex(trimStr, MaxCharsToCheck))
            {
                return _wkbReader.Read(WKBReader.HexToBytes(trimStr));
            }
            return _wktReader.Read(trimStr);
        }

        private static bool IsHex(string str, int maxCharsToTest)
        {
            for (int i = 0; i < maxCharsToTest && i < str.Length; i++)
            {
                char ch = str[i];
                if (!IsHexDigit(ch))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsHexDigit(char ch)
        {
            if (char.IsDigit(ch))
            {
                return true;
            }
            char chLow = char.ToLower(ch);
            if (chLow >= 'a' && chLow <= 'f')
            {
                return true;
            }
            return false;
        }
    }
}
