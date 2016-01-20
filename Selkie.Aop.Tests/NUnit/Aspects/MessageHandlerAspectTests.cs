using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;
using Selkie.Aop.Aspects;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;

namespace Selkie.Aop.Tests.NUnit.Aspects
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class MessageHandlerAspectTests
    {
        private MessageHandlerAspect CreateSut()
        {
            return CreateSut(Substitute.For <ISelkieBus>(),
                             Substitute.For <IInvocationToTextConverter>(),
                             Substitute.For <ILogger>());
        }


        private MessageHandlerAspect CreateSut([NotNull] ISelkieBus bus,
                                               [NotNull] IInvocationToTextConverter converter,
                                               [NotNull] ILogger logger)
        {
            var repository = Substitute.For <ILoggerRepository>();
            repository.Get(Arg.Any <string>()).ReturnsForAnyArgs(logger);

            var sut = new MessageHandlerAspect(bus,
                                               repository,
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
        public void Intercept_CallsConverter_ForIsDebugEnabledIsTrue()
        {
            // Arrange
            var logger = Substitute.For <ILogger>();
            logger.IsDebugEnabled.Returns(true);
            var converter = Substitute.For <IInvocationToTextConverter>();
            IInvocation invocation = CreateInvocation();
            MessageHandlerAspect sut = CreateSut(Substitute.For <ISelkieBus>(),
                                                 converter,
                                                 logger);

            // Act
            sut.Intercept(invocation);

            // Assert
            converter.Received().Convert(Arg.Any <IInvocation>());
        }

        [Test]
        public void Intercept_CallsDebug_ForIsDebugEnabledIsTrue()
        {
            // Arrange
            var logger = Substitute.For <ILogger>();
            logger.IsDebugEnabled.Returns(true);
            IInvocation invocation = CreateInvocation();
            MessageHandlerAspect sut = CreateSut(Substitute.For <ISelkieBus>(),
                                                 new InvocationToTextConverter(),
                                                 logger);

            // Act
            sut.Intercept(invocation);

            // Assert
            logger.Received().Debug(Arg.Is <string>(x => x.StartsWith("LogAspectTests")));
        }

        [Test]
        public void Intercept_CallsLoggerError_WhenProceedThrowsExceptionAndIsErrorEnabledIsFalse()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();
            var logger = Substitute.For <ILogger>();
            logger.IsErrorEnabled.Returns(true);
            MessageHandlerAspect sut = CreateSut(Substitute.For <ISelkieBus>(),
                                                 new InvocationToTextConverter(),
                                                 logger);

            // Act
            sut.Intercept(invocation);

            // Assert
            logger.Received().Error(Arg.Any <string>(),
                                    Arg.Any <Exception>());
        }

        [Test]
        public void Intercept_CallsProceed_WhenCalled()
        {
            // Arrange
            IInvocation invocation = CreateInvocation();
            MessageHandlerAspect sut = CreateSut();

            // Act
            sut.Intercept(invocation);

            // Assert
            invocation.Received().Proceed();
        }

        [Test]
        public void Intercept_DoesNotCallLoggerError_WhenProceedThrowsExceptionAndIsErrorEnabledIsFalse()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();
            var logger = Substitute.For <ILogger>();
            logger.IsErrorEnabled.Returns(false);
            MessageHandlerAspect sut = CreateSut(Substitute.For <ISelkieBus>(),
                                                 new InvocationToTextConverter(),
                                                 logger);

            // Act
            sut.Intercept(invocation);

            // Assert
            logger.DidNotReceiveWithAnyArgs().Error(Arg.Any <string>(),
                                                    Arg.Any <Exception>());
        }

        [Test]
        public void Intercept_DoesNotCallsDebug_ForIsDebugEnabledIsFalse()
        {
            // Arrange
            var logger = Substitute.For <ILogger>();
            logger.IsDebugEnabled.Returns(false);
            IInvocation invocation = CreateInvocation();
            MessageHandlerAspect sut = CreateSut(Substitute.For <ISelkieBus>(),
                                                 new InvocationToTextConverter(),
                                                 logger);

            // Act
            sut.Intercept(invocation);

            // Assert
            logger.DidNotReceiveWithAnyArgs().Debug(Arg.Any <string>());
        }

        [Test]
        public void Intercept_ThrowsException_WhenProceedThrowsException()
        {
            // Arrange
            IInvocation invocation = CreateInvocationThatThrows();
            var bus = Substitute.For <ISelkieBus>();
            MessageHandlerAspect sut = CreateSut(bus,
                                                 new InvocationToTextConverter(),
                                                 Substitute.For <ILogger>());

            // Act
            sut.Intercept(invocation);

            // Assert
            bus.Received().PublishAsync(Arg.Any <ExceptionThrownMessage>());
        }
    }
}