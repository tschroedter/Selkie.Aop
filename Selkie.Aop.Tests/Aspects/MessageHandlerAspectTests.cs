using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using NSubstitute;
using NUnit.Framework;
using Selkie.Aop.Aspects;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;

namespace Selkie.Aop.Tests.Aspects
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class MessageHandlerAspectTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieBus>();
            m_InvocationConverter = Substitute.For <IInvocationToTextConverter>();
            m_Repository = Substitute.For <ILoggerRepository>();
            m_Logger = Substitute.For <ILogger>();
            m_MessageConverter = Substitute.For <IExceptionToMessageConverter>();
            m_ExceptionLogger = Substitute.For <IExceptionLogger>();

            m_Repository.Get(Arg.Any <string>()).Returns(m_Logger);

            m_Sut = new MessageHandlerAspect(m_Bus,
                                             m_Repository,
                                             m_InvocationConverter,
                                             m_MessageConverter,
                                             m_ExceptionLogger);
        }

        private IInvocationToTextConverter m_InvocationConverter;
        private ISelkieBus m_Bus;
        private ILogger m_Logger;
        private IExceptionToMessageConverter m_MessageConverter;
        private IExceptionLogger m_ExceptionLogger;
        private MessageHandlerAspect m_Sut;
        private ILoggerRepository m_Repository;

        private IInvocation CreateInvocation()
        {
            var array = new[]
                        {
                            1,
                            2
                        };

            var arguments = new object[]
                            {
                                array
                            };
            var action = new Action(DoesNotThrow);
            var invocation = Substitute.For <IInvocation>();
            invocation.TargetType.Returns(typeof( LogAspectTests ));
            invocation.Method.Returns(action.GetMethodInfo());
            invocation.Arguments.Returns(arguments);

            return invocation;
        }

        private IInvocation CreateInvocationThatThrows()
        {
            var array = new[]
                        {
                            1,
                            2
                        };

            var arguments = new object[]
                            {
                                array
                            };
            var action = new Action(DoesThrow);
            var invocation = Substitute.For <IInvocation>();
            invocation.TargetType.Returns(typeof( LogAspectTests ));
            invocation.Method.Returns(action.GetMethodInfo());
            invocation.Arguments.Returns(arguments);
            invocation.When(x => x.Proceed())
                      .Do(x =>
                          {
                              DoesThrow();
                          });

            return invocation;
        }

        private void DoesNotThrow()
        {
        }

        private void DoesThrow()
        {
            throw new Exception("Test");
        }

        [Test]
        public void Intercept_CallsDebug_ForIsDebugEnabledIsTrue()
        {
            // Arrange
            m_Logger.IsDebugEnabled.Returns(true);
            m_InvocationConverter.Convert(Arg.Any <IInvocation>()).Returns("Test");
            IInvocation invocation = CreateInvocation();

            // Act
            m_Sut.Intercept(invocation);

            // Assert
            m_Logger.Received().Debug(Arg.Is <string>(x => x == "Test"));
        }

        [Test]
        public void Intercept_CallsLoggerError_WhenProceedThrowsExceptionAndIsErrorEnabledIsFalse()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();

            // Act
            m_Sut.Intercept(invocation);

            // Assert
            m_ExceptionLogger.Received().LogException(Arg.Any <IInvocation>(),
                                                      Arg.Any <Exception>());
        }

        [Test]
        public void Intercept_CallsProceed_WhenCalled()
        {
            // Arrange
            IInvocation invocation = CreateInvocation();

            // Act
            m_Sut.Intercept(invocation);

            // Assert
            invocation.Received().Proceed();
        }

        [Test]
        public void Intercept_DoesNotCallLoggerError_WhenProceedThrowsExceptionAndIsErrorEnabledIsFalse()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();
            m_Logger.IsErrorEnabled.Returns(false);

            // Act
            m_Sut.Intercept(invocation);

            // Assert
            m_Logger.DidNotReceiveWithAnyArgs().Error(Arg.Any <string>(),
                                                      Arg.Any <Exception>());
        }

        [Test]
        public void Intercept_DoesNotCallsDebug_ForIsDebugEnabledIsFalse()
        {
            // Arrange
            m_Logger.IsDebugEnabled.Returns(false);
            IInvocation invocation = CreateInvocation();

            // Act
            m_Sut.Intercept(invocation);

            // Assert
            m_Logger.DidNotReceiveWithAnyArgs().Debug(Arg.Any <string>());
        }

        [Test]
        public void Intercept_ThrowsException_WhenProceedThrowsException()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();

            // Act
            m_Sut.Intercept(invocation);

            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <ExceptionThrownMessage>());
        }
    }
}