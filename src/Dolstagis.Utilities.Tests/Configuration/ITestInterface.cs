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

        DayOfWeek EnumSetting { get; }

        [DefaultValue(DayOfWeek.Thursday)]
        DayOfWeek EnumSettingWithDefault { get; }

        double DoubleSetting { get; }

        [DefaultValue(Math.PI)]
        double DoubleSettingWithDefault { get; }

        float FloatSetting { get; }

        [DefaultValue((float)Math.PI)]
        float FloatSettingWithDefault { get; }

        Guid GuidSetting { get; }

        [DefaultValue("01234567-89ab-cdef-0123-456789abcdef")]
        Guid GuidSettingWithDefault { get; }

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

        TimeSpan TimeSpanSetting { get; }

        [DefaultValue("1:00:00")]
        TimeSpan TimeSpanSettingWithDefault { get; }

        Uri UriSetting { get; }

        [DefaultValue("https://github.com/jammycakes")]
        Uri UriSettingWithDefault { get; }

        [DefaultValue("default")]
        string StringSettingWithDefault { get; }

        IEmpty NotSupportedTypeSetting { get; }

        void DoSomethingUnsupported();
    }
}
