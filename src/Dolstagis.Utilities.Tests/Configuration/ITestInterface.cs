using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dolstagis.Utilities.Configuration;

namespace Dolstagis.Utilities.Tests.Configuration
{
    [Prefix("Test")]
    public interface ITestInterface
    {
        string StringSetting { get; }

        int IntSetting { get; }

        bool BoolSetting { get; }

        DateTime DateTimeSetting { get; }

        long LongSetting { get; }

        double DoubleSetting { get; }

        IEmpty NotSupportedTypeSetting { get; }

        void DoSomethingUnsupported();
    }
}
