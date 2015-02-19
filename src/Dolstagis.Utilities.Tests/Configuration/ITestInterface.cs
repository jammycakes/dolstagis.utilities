using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [DefaultValue("default")]
        string StringSettingWithDefault { get; }
        [DefaultValue(10)]
        int IntSettingWithDefault { get; }

        [DefaultValue(10)]
        long LongSettingWithDefault { get; }

        [DefaultValue(true)]
        bool BoolSettingWithDefault { get; }

        [DefaultValue(Math.PI)]
        double DoubleSettingWithDefault { get; }

        [DefaultValue("2015-01-01 12:34:56")]
        DateTime DateTimeSettingWithDefault { get; }
    }
}
