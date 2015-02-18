using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dolstagis.Utilities.Configuration;
using Moq;
using NUnit.Framework;

namespace Dolstagis.Utilities.Tests.Configuration
{
    [TestFixture]
    public class SettingsBuilderFixture
    {
        [Test]
        public void CanCreateModuleBuilder()
        {
            var module = SettingsCache.CreateModule();
            Assert.IsNotNull(module);
        }


        [Test]
        public void CanImplementEmptyInterface()
        {
            var module = SettingsCache.CreateModule();
            var builder = new SettingsBuilder<IEmpty>(module);
            var empty = builder.Build(null);
            Assert.IsInstanceOf<IEmpty>(empty);
        }

        [Test]
        public void CanImplementTestInterface()
        {
            var source = new Mock<ISettingsSource>();
            source.Setup(x => x.GetString("Test", "StringSetting")).Returns("test");
            var module = SettingsCache.CreateModule();
            var builder = new SettingsBuilder<ITestInterface>(module);
            var settings = builder.Build(source.Object);
            Assert.IsInstanceOf<ITestInterface>(settings);
            Assert.AreEqual("test", settings.StringSetting);
            source.Verify(x => x.GetString("Test", "StringSetting"), Times.Once, "Source was not called correctly");
        }
    }
}
