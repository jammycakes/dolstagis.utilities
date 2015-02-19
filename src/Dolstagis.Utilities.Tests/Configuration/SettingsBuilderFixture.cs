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
            source.Setup(x => x.GetBool("Test", "BoolSetting")).Returns(true);
            source.Setup(x => x.GetChar("Test", "CharSetting")).Returns('a');
            source.Setup(x => x.GetDateTime("Test", "DateTimeSetting")).Returns(new DateTime(2015, 2, 18, 21, 24, 10));
            source.Setup(x => x.GetDouble("Test", "DoubleSetting")).Returns(Math.PI);
            source.Setup(x => x.GetFloat("Test", "FloatSetting")).Returns((float)Math.PI);
            source.Setup(x => x.GetInt("Test", "IntSetting")).Returns(15);
            source.Setup(x => x.GetLong("Test", "LongSetting")).Returns(0x10000000000);
            source.Setup(x => x.GetShort("Test", "ShortSetting")).Returns(16384);
            source.Setup(x => x.GetString("Test", "StringSetting")).Returns("test");

            source.Setup(x => x.GetBool(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns<string, string, bool>((a, b, c) => c);
            source.Setup(x => x.GetChar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<char>()))
                .Returns<string, string, char>((a, b, c) => c);
            source.Setup(x => x.GetDateTime(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns<string, string, DateTime>((a, b, c) => c);
            source.Setup(x => x.GetDouble(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
                .Returns<string, string, double>((a, b, c) => c);
            source.Setup(x => x.GetFloat(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float>()))
                .Returns<string, string, float>((a, b, c) => c);
            source.Setup(x => x.GetInt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Int32>()))
                .Returns<string, string, Int32>((a, b, c) => c);
            source.Setup(x => x.GetLong(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Int64>()))
                .Returns<string, string, Int64>((a, b, c) => c);
            source.Setup(x => x.GetShort(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Int16>()))
                .Returns<string, string, Int16>((a, b, c) => c);
            source.Setup(x => x.GetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string, string>((a, b, c) => c);

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


        [Test]
        public void BoolSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(true, settings.BoolSetting);
            source.Verify(x => x.GetBool("Test", "BoolSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void BoolSettingWithDefaultValueIsReturnedCorrectly()
        {
            Assert.IsTrue(settings.BoolSettingWithDefault);
            source.Verify(x => x.GetBool("Test", "BoolSettingWithDefault", true), Times.Once);
        }

        [Test]
        public void CharSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual('a', settings.CharSetting);
            source.Verify(x => x.GetChar("Test", "CharSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void CharSettingWithDefaultValueIsReturnedCorrectly()
        {
            Assert.AreEqual('a', settings.CharSettingWithDefault);
            source.Verify(x => x.GetChar("Test", "CharSettingWithDefault", 'a'), Times.Once);
        }



        [Test]
        public void DateTimeSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(new DateTime(2015, 2, 18, 21, 24, 10), settings.DateTimeSetting);
            source.Verify(x => x.GetDateTime("Test", "DateTimeSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void DateTimeSettingWithDefaultValueIsReturnedCorrectly()
        {
            var dt = new DateTime(2015, 1, 1, 12, 34, 56);
            Assert.AreEqual(dt, settings.DateTimeSettingWithDefault);
            source.Verify(x => x.GetDateTime("Test", "DateTimeSettingWithDefault", dt), Times.Once);
        }

        [Test]
        public void DoubleSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(Math.PI, settings.DoubleSetting);
            source.Verify(x => x.GetDouble("Test", "DoubleSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void DoubleSettingWithDefaultValueIsReturnedCorrectly()
        {
            Assert.AreEqual(Math.PI, settings.DoubleSettingWithDefault);
            source.Verify(x => x.GetDouble("Test", "DoubleSettingWithDefault", Math.PI), Times.Once);
        }

        [Test]
        public void FloatSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual((float)Math.PI, settings.FloatSetting);
            source.Verify(x => x.GetFloat("Test", "FloatSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void FloatSettingWithDefaultValueIsReturnedCorrectly()
        {
            Assert.AreEqual((float)Math.PI, settings.FloatSettingWithDefault);
            source.Verify(x => x.GetFloat("Test", "FloatSettingWithDefault", (float)Math.PI), Times.Once);
        }

        [Test]
        public void IntSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(15, settings.IntSetting);
            source.Verify(x => x.GetInt("Test", "IntSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void IntSettingWithDefaultValueIsReturnedCorrectly()
        {
            Assert.AreEqual(10, settings.IntSettingWithDefault);
            source.Verify(x => x.GetInt("Test", "IntSettingWithDefault", 10), Times.Once);
        }

        [Test]
        public void LongSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(0x10000000000, settings.LongSetting);
            source.Verify(x => x.GetLong("Test", "LongSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void LongSettingWithDefaultValueIsReturnedCorrectly()
        {
            Assert.AreEqual(10L, settings.LongSettingWithDefault);
            source.Verify(x => x.GetLong("Test", "LongSettingWithDefault", 10), Times.Once);
        }

        [Test]
        public void ShortSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual(16384, settings.ShortSetting);
            source.Verify(x => x.GetShort("Test", "ShortSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void ShortSettingWithDefaultValueIsReturnedCorrectly()
        {
            Assert.AreEqual(16384, settings.ShortSettingWithDefault);
            source.Verify(x => x.GetShort("Test", "ShortSettingWithDefault", 16384), Times.Once);
        }

        [Test]
        public void StringSettingIsRetrievedCorrectly()
        {
            Assert.AreEqual("test", settings.StringSetting);
            source.Verify(x => x.GetString("Test", "StringSetting"), Times.Once, "Source was not called correctly");
        }

        [Test]
        public void StringSettingWithDefaultValueIsReturnedCorrectly()
        {
            Assert.AreEqual("default", settings.StringSettingWithDefault);
            source.Verify(x => x.GetString("Test", "StringSettingWithDefault", "default"), Times.Once);
        }
    }
}
