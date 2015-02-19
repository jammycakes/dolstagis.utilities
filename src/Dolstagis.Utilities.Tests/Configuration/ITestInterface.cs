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
        bool BoolSetting { get; }

        [DefaultValue(true)]
        bool BoolSettingWithDefault { get; }

        char CharSetting { get; }

        [DefaultValue('a')]
        char CharSettingWithDefault { get; }

        DateTime DateTimeSetting { get; }

        [DefaultValue("2015-01-01 12:34:56")]
        DateTime DateTimeSettingWithDefault { get; }

        double DoubleSetting { get; }

        [DefaultValue(Math.PI)]
        double DoubleSettingWithDefault { get; }

        int IntSetting { get; }

        [DefaultValue(10)]
        int IntSettingWithDefault { get; }

        long LongSetting { get; }

        [DefaultValue(10)]
        long LongSettingWithDefault { get; }

        short ShortSetting { get; }

        [DefaultValue(16384)]
        short ShortSettingWithDefault { get; }

        string StringSetting { get; }

        [DefaultValue("default")]
        string StringSettingWithDefault { get; }

        IEmpty NotSupportedTypeSetting { get; }

        void DoSomethingUnsupported();
    }
}
