using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.DynamicProxy;
using NSubstitute;
using NUnit.Framework;
using Selkie.Aop.Aspects;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;

namespace Selkie.Aop.Tests.NUnit.Aspects
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class PublishExceptionAspectTests
    {
        private PublishExceptionAspect CreateSut()
        {
            return CreateSut(Substitute.For <ISelkieBus>());
        }

        private PublishExceptionAspect CreateSut(ISelkieBus bus)
        {
            PublishExceptionAspect sut = CreateSut(bus,
                                                   new InvocationToTextConverter());

            return sut;
        }

        private PublishExceptionAspect CreateSut(ISelkieBus bus,
                                                 IInvocationToTextConverter converter)
        {
            var sut = new PublishExceptionAspect(bus,
                                                 converter);

            return sut;
        }

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
            invocation.TargetType.Returns(typeof ( LogAspectTests ));
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
            invocation.TargetType.Returns(typeof ( LogAspectTests ));
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
            PublishExceptionAspect sut = CreateSut();

            // Act
            sut.Intercept(invocation);

            // Assert
            invocation.Received().Proceed();
        }

        [Test]
        public void Intercept_DoesNotSendMessage_WhenCallingProceed()
        {
            // Arrange
            IInvocation invocation = CreateInvocation();
            var bus = Substitute.For <ISelkieBus>();
            PublishExceptionAspect sut = CreateSut(bus);

            // Act
            sut.Intercept(invocation);

            bus.DidNotReceiveWithAnyArgs().PublishAsync(Arg.Any <ExceptionThrownMessage>());
        }

        [Test]
        public void Intercept_SendsMessage_WhenCallingProceedThrowsException()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();
            var bus = Substitute.For <ISelkieBus>();
            PublishExceptionAspect sut = CreateSut(bus);

            // Act
            Assert.Throws <Exception>(() => sut.Intercept(invocation));

            bus.Received().PublishAsync(Arg.Any <ExceptionThrownMessage>());
        }
    }
}