using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIT.Lib
{
    public static class Type
    {
        /// <summary>
        /// xPression: Empty, Alpha, Numeric, AlphaNumeric, NonAlphaNumeric, Mix
        /// </summary>
        public enum xPression { Empty, Alpha, Numeric, AlphaNumeric, NonAlphaNumeric, Mix };

        /// <summary>
        /// Geography Name Type: FullName, Abbreviation
        /// for World Countries: United States, US
        /// for US States: Washington, WA
        /// </summary>
        public enum GeographyNameType { FullName, Abbreviation };
    }
}