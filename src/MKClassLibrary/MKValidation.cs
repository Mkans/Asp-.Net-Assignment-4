using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKClassLibrary
{
    public class MKValidation
    {        public static string Capitalise(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("Error !!!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
