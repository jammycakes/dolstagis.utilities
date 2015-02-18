using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Dolstagis.Utilities.Configuration;
using Moq;
using NUnit.Framework;

namespace Dolstagis.Utilities.Tests.Configuration
{
    [TestFixture]
    public class SettingsBuilderFixture
    {
        private Mock<ISettingsSource> source;
        private ModuleBuilder module;
        private SettingsBuilder<ITestInterface> settingsBuilder;

        private ITestInterface settings;

        [TestFixtureSetUp]
        public void Run()
        {
            Arrange();
            Act();
        }

        private void Arrange()
        {
            source = new Mock<ISettingsSource>();
            source.Setup(x => x.GetString("Test", "StringSetting")).Returns("test");
            source.Setup(x => x.GetInt("Test", "IntSetting")).Returns(15);
            source.Setup(x => x.GetBool("Test", "BoolSetting")).Returns(true);
            source.Setup(x => x.GetDouble("Test", "DoubleSetting")).Returns(Math.PI);
            source.Setup(x => x.GetLong("Test", "LongSetting")).Returns(0x10000000000);
            source.Setup(x => x.GetDateTime("Test", "DateTimeSetting")).Returns(new DateTime(2015, 2, 18, 21, 24, 10));

            module = SettingsCache.CreateModule();
            settingsBuilder = new SettingsBuilder<ITestInterface>(module);
        }

        private void Act()
        {
            settings = settingsBuilder.Build(source.Object);
        }


        [Test]
        public void CanCreateModuleBuilder()
        {
            Assert.IsNotNull(module);
        }


        [Test]
        public void CanImplementEmptyInterface()
        {
            var builder = new SettingsBuilder<IEmpty>(module);
            var empty = builder.Build(null);
            Assert.IsInstanceOf<IEmpty>(empty);
        }

        [Test]
        public void SettingsIsCreated()
        {
            Assert.IsInstanceOf<ITestInterface>(settings);
        }


        [Test]
        public void StringSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual("test", settings.StringSetting);
            source.Verify(x => x.GetString("Test", "StringSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void IntSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(15, settings.IntSetting);
            source.Verify(x => x.GetInt("Test", "IntSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void BoolSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(true, settings.BoolSetting);
            source.Verify(x => x.GetBool("Test", "BoolSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void DoubleSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(Math.PI, settings.DoubleSetting);
            source.Verify(x => x.GetDouble("Test", "DoubleSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void LongSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(0x10000000000, settings.LongSetting);
            source.Verify(x => x.GetLong("Test", "LongSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void DateTimeSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(new DateTime(2015, 2, 18, 21, 24, 10), settings.DateTimeSetting);
            source.Verify(x => x.GetDateTime("Test", "DateTimeSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void MethodCallsShouldThrowNotSupportedExceptionWhenCalled()
        {
            settings.DoSomethingUnsupported();
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void PropertiesWithUnsupportedTypesShouldThrowNotSupportedExceptionWhenGetterCalled()
        {
            var obj = settings.NotSupportedTypeSetting;
            if (obj != null)
                Assert.Fail();
        }
    }
}
