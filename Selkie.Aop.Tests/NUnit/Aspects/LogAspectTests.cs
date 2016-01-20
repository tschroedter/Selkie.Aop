using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using NSubstitute;
using NUnit.Framework;
using Selkie.Aop.Aspects;

namespace Selkie.Aop.Tests.NUnit.Aspects
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class LogAspectTests
    {
        private LogAspect CreateSut()
        {
            return CreateSut(Substitute.For <ILogger>());
        }

        private LogAspect CreateSut(ILogger logger)
        {
            LogAspect sut = CreateSut(logger,
                                      new InvocationToTextConverter());

            return sut;
        }

        private LogAspect CreateSut(ILogger logger,
                                    IInvocationToTextConverter converter)
        {
            var factory = Substitute.For <ILoggerRepository>();
            factory.Get(Arg.Any <string>()).ReturnsForAnyArgs(logger);

            var sut = new LogAspect(factory,
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
            LogAspect sut = CreateSut(logger,
                                      converter);

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
            LogAspect sut = CreateSut(logger);

            // Act
            sut.Intercept(invocation);

            // Assert
            logger.Received().Debug(Arg.Is <string>(x => x.StartsWith("LogAspectTests")));
        }

        [Test]
        public void Intercept_CallsProceed_WhenCalled()
        {
            // Arrange
            IInvocation invocation = CreateInvocation();
            LogAspect sut = CreateSut();

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
            LogAspect sut = CreateSut(logger);

            // Act
            Assert.Throws <Exception>(() => sut.Intercept(invocation));

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
            LogAspect sut = CreateSut(logger);

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
            var logger = Substitute.For <ILogger>();
            logger.IsErrorEnabled.Returns(true);
            LogAspect sut = CreateSut(logger);

            // Act
            Assert.Throws <Exception>(() => sut.Intercept(invocation));

            logger.Received().Error(Arg.Any <string>(),
                                    Arg.Any <Exception>());
        }
    }
}