using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShrineFox.IO
{
    public static class Extensions
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: return null;
                case "": return "";
                default: return input[0].ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}
