using System;
using System.Diagnostics.CodeAnalysis;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Core2.Selkie.Aop.Aspects;
using Core2.Selkie.Aop.Messages;
using Core2.Selkie.EasyNetQ.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Core2.Selkie.Aop.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class StatusAspectTests
    {
        [SetUp]
        public void Setup()
        {
            m_Invocation = Substitute.For <IInvocation>();
            m_Invocation.TargetType.Returns(typeof( string ));

            m_LoggerRepository = Substitute.For <ILoggerRepository>();
            m_StatusRepository = Substitute.For <IStatusRepository>();
            m_Bus = Substitute.For <ISelkieBus>();
            m_ExceptionLogger = Substitute.For <IExceptionLogger>();
            m_ExceptionMessageConverter = Substitute.For <IExceptionToMessageConverter>();

            m_Logger = Substitute.For <ILogger>();
            m_LoggerRepository.Get("").ReturnsForAnyArgs(m_Logger);

            m_Sut = new StatusAspect(m_LoggerRepository,
                                     m_StatusRepository,
                                     m_Bus,
                                     m_ExceptionLogger,
                                     m_ExceptionMessageConverter);
        }

        private ILoggerRepository m_LoggerRepository;
        private IStatusRepository m_StatusRepository;
        private ISelkieBus m_Bus;
        private IExceptionLogger m_ExceptionLogger;
        private StatusAspect m_Sut;
        private ILogger m_Logger;
        private IInvocation m_Invocation;
        private IExceptionToMessageConverter m_ExceptionMessageConverter;

        [Test]
        public void Intercept_CallsExceptionLogger_WhenProceedThrows()
        {
            // Arrange
            m_Logger.IsErrorEnabled.Returns(true);
            m_Invocation.WhenForAnyArgs(x => x.Proceed())
                        .Do(x =>
                            {
                                throw new Exception("Test");
                            });

            // Act
            Assert.Throws <Exception>(() => m_Sut.Intercept(m_Invocation));

            // Assert
            m_ExceptionLogger.Received().LogException(Arg.Any <IInvocation>(),
                                                      Arg.Any <Exception>());
        }

        [Test]
        public void Intercept_DoesNotSendMessage_WhenIsInfoEnabledIsFalse()
        {
            // Arrange
            m_Logger.IsInfoEnabled.Returns(false);

            // Act
            m_Sut.Intercept(m_Invocation);

            // Assert
            m_StatusRepository.DidNotReceiveWithAnyArgs().Get(null);
        }

        [Test]
        public void Intercept_DoesNotSendMessage_WhenStatusIsEmpty()
        {
            // Arrange
            m_Logger.IsInfoEnabled.Returns(true);
            m_StatusRepository.Get(null).ReturnsForAnyArgs(string.Empty);

            // Act
            m_Sut.Intercept(m_Invocation);

            // Assert
            m_Bus.DidNotReceive().PublishAsync(Arg.Any <StatusMessage>());
        }

        [Test]
        public void Intercept_DoesNotSendMessage_WhenStatusIsNull()
        {
            // Arrange
            m_Logger.IsInfoEnabled.Returns(true);
            m_StatusRepository.Get(null).ReturnsForAnyArgs(( string ) null);

            // Act
            m_Sut.Intercept(m_Invocation);

            // Assert
            m_Bus.DidNotReceive().PublishAsync(Arg.Any <StatusMessage>());
        }

        [Test]
        public void Intercept_LogsMessage_WhenIsInfoEnabledIsTrue()
        {
            // Arrange
            m_Logger.IsInfoEnabled.Returns(true);
            m_StatusRepository.Get(null).ReturnsForAnyArgs("Status");

            // Act
            m_Sut.Intercept(m_Invocation);

            // Assert
            m_Logger.Received().Info("Status");
        }

        [Test]
        public void Intercept_SendMessage_WhenIsInfoEnabledIsTrue()
        {
            // Arrange
            m_Logger.IsInfoEnabled.Returns(true);
            m_StatusRepository.Get(null).ReturnsForAnyArgs("Status");

            // Act
            m_Sut.Intercept(m_Invocation);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Is <StatusMessage>(x => x.Text == "Status"));
        }

        [Test]
        public void Intercept_SendsExceptionMessage_WhenProceedThrows()
        {
            // Arrange
            var excpected = new ExceptionThrownMessage();
            m_ExceptionMessageConverter.CreateExceptionThrownMessage(Arg.Any <IInvocation>(),
                                                                     Arg.Any <Exception>())
                                       .Returns(excpected);

            m_Logger.IsErrorEnabled.Returns(true);
            m_Invocation.WhenForAnyArgs(x => x.Proceed())
                        .Do(x =>
                            {
                                throw new Exception("Test");
                            });

            // Act
            Assert.Throws <Exception>(() => m_Sut.Intercept(m_Invocation));

            // Assert
            m_Bus.Received().PublishAsync(excpected);
        }
    }
}