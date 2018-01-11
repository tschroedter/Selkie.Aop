using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.DynamicProxy;
using NSubstitute;
using NUnit.Framework;
using Core2.Selkie.Aop.Aspects;
using Core2.Selkie.Aop.Messages;
using Core2.Selkie.EasyNetQ.Interfaces;

namespace Core2.Selkie.Aop.Tests.Aspects
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class PublishExceptionAspectTests
    {
        [SetUp]
        public void Setup()
        {
            m_Bus = Substitute.For <ISelkieBus>();
            m_Logger = Substitute.For <IExceptionLogger>();
            m_Converter = Substitute.For <IExceptionToMessageConverter>();

            m_Sut = new PublishExceptionAspect(m_Bus,
                                               m_Logger,
                                               m_Converter);
        }

        private ISelkieBus m_Bus;
        private IExceptionLogger m_Logger;
        private IExceptionToMessageConverter m_Converter;
        private PublishExceptionAspect m_Sut;

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
        public void Intercept_DoesNotSendMessage_WhenCallingProceed()
        {
            // Arrange
            IInvocation invocation = CreateInvocation();

            // Act
            m_Sut.Intercept(invocation);

            m_Bus.DidNotReceiveWithAnyArgs().PublishAsync(Arg.Any <ExceptionThrownMessage>());
        }

        [Test]
        public void Intercept_LogsMessage_WhenCallingProceedThrowsException()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();

            // Act
            Assert.Throws <Exception>(() => m_Sut.Intercept(invocation));

            m_Logger.Received().LogException(Arg.Any <IInvocation>(),
                                             Arg.Any <Exception>());
        }

        [Test]
        public void Intercept_SendsMessage_WhenCallingProceedThrowsException()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();

            // Act
            Assert.Throws <Exception>(() => m_Sut.Intercept(invocation));

            m_Bus.Received().PublishAsync(Arg.Any <ExceptionThrownMessage>());
        }
    }
}