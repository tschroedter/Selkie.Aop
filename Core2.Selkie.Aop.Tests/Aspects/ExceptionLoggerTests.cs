using System;
using System.Diagnostics.CodeAnalysis;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Core2.Selkie.Aop.Aspects;
using NSubstitute;
using NUnit.Framework;

namespace Core2.Selkie.Aop.Tests.Aspects
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ExceptionLoggerTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ILogger>();
            m_Converter = Substitute.For <IInvocationToTextConverter>();

            m_Sut = new ExceptionLogger(m_Logger,
                                        m_Converter);
        }

        private IInvocationToTextConverter m_Converter;
        private ExceptionLogger m_Sut;
        private ILogger m_Logger;

        [Test]
        public void LogException_CallsLoggerError_WhenIsErrorEnabledIsTrue()
        {
            // Arrange
            m_Logger.IsErrorEnabled.Returns(true);
            var invocation = Substitute.For <IInvocation>();
            var exception = new Exception("Test");

            // Act
            m_Sut.LogException(invocation,
                               exception);

            // Assert
            m_Logger.Received().Error(Arg.Any <string>(),
                                      Arg.Any <Exception>());
        }

        [Test]
        public void LogException_DoesNotCallLoggerError_WhenIsErrorEnabledIsFalse()
        {
            // Arrange
            m_Logger.IsErrorEnabled.Returns(false);
            var invocation = Substitute.For <IInvocation>();
            var exception = new Exception("Test");

            // Act
            m_Sut.LogException(invocation,
                               exception);

            // Assert
            m_Logger.DidNotReceive().Error(Arg.Any <string>(),
                                           Arg.Any <Exception>());
        }
    }
}