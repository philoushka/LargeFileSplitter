using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeFileSplitter
{
    public static class StringExtensions
    {
        public static string FormatWith(this string input, params object[] args)
        {
            if (input == null)
                throw new ArgumentNullException("format");

            return string.Format(input, args);
        }

    }
}
