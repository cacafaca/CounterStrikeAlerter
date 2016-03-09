using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Common
{
    public static class Logger
    {
        public static void TraceWriteLine(object value)
        {
            System.Diagnostics.Trace.WriteLine(value, DefaultCategory);
        }

        public static void TraceWriteLine(object value, string category)
        {
            if (!string.IsNullOrEmpty(category))
                category = DefaultCategory + "/" + category;
            else
                category = DefaultCategory;
            System.Diagnostics.Trace.WriteLine(value, category);
        }

        private static string DefaultCategory = "CSA";
    }
}
