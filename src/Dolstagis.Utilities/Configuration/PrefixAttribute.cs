using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dolstagis.Utilities.Configuration
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class PrefixAttribute : Attribute
    {
        public string Prefix { get; private set; }

        public PrefixAttribute(string prefix)
        {
            this.Prefix = prefix;
        }
    }
}
