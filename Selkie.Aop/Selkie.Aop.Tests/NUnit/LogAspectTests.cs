using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using NSubstitute;
using NUnit.Framework;

namespace Selkie.Aop.Tests.NUnit
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
            var factory = Substitute.For <ILoggerRepository>();
            factory.Get(Arg.Any <string>()).ReturnsForAnyArgs(logger);

            var sut = new LogAspect(factory);
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

        private class Record
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Number { get; set; }
            public string Text { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [Test]
        public void ConvertArgumentsToString_ReturnsJsonString_ForIntegerArray()
        {
            // Arrange
            var array = new[]
                        {
                            1,
                            2
                        };

            var arguments = new object[]
                            {
                                array
                            };

            LogAspect sut = CreateSut();

            // Act
            string actual = sut.ConvertArgumentsToString(arguments);

            // Assert
            Assert.AreEqual("[1,2]",
                            actual);
        }

        [Test]
        public void ConvertArgumentsToString_ReturnsJsonString_ForIntegerMultipleArray()
        {
            // Arrange
            var array = new[]
                        {
                            new[]
                            {
                                1,
                                2
                            },
                            new[]
                            {
                                3,
                                4
                            }
                        };

            var arguments = new object[]
                            {
                                array
                            };

            LogAspect sut = CreateSut();

            // Act
            string actual = sut.ConvertArgumentsToString(arguments);

            // Assert
            Assert.AreEqual("[[1,2],[3,4]]",
                            actual);
        }

        [Test]
        public void ConvertArgumentsToString_ReturnsJsonString_ForRecord()
        {
            // Arrange
            var record = new Record
                         {
                             Number = 1,
                             Text = "Hello World"
                         };

            var arguments = new object[]
                            {
                                record
                            };

            LogAspect sut = CreateSut();

            // Act
            string actual = sut.ConvertArgumentsToString(arguments);

            // Assert
            Assert.AreEqual("{\"Number\":1,\"Text\":\"Hello World\"}",
                            actual);
        }

        [Test]
        public void ConvertArgumentsToString_ReturnsString_ForDouble()
        {
            // Arrange
            var arguments = new object[]
                            {
                                1.23
                            };

            LogAspect sut = CreateSut();

            // Act
            string actual = sut.ConvertArgumentsToString(arguments);

            // Assert
            Assert.AreEqual("1.23",
                            actual);
        }

        [Test]
        public void ConvertArgumentsToString_ReturnsString_ForEmptyString()
        {
            // Arrange
            var arguments = new object[]
                            {
                                ""
                            };

            LogAspect sut = CreateSut();

            // Act
            string actual = sut.ConvertArgumentsToString(arguments);

            // Assert
            Assert.AreEqual("\"\"",
                            actual);
        }

        [Test]
        public void ConvertArgumentsToString_ReturnsString_ForInteger()
        {
            // Arrange
            var arguments = new object[]
                            {
                                10
                            };

            LogAspect sut = CreateSut();

            // Act
            string actual = sut.ConvertArgumentsToString(arguments);

            // Assert
            Assert.AreEqual("10",
                            actual);
        }

        [Test]
        public void ConvertArgumentsToString_ReturnsString_ForNullInArguments()
        {
            // Arrange
            var arguments = new object[]
                            {
                                null
                            };

            LogAspect sut = CreateSut();

            // Act
            string actual = sut.ConvertArgumentsToString(arguments);

            // Assert
            Assert.AreEqual("null",
                            actual);
        }

        [Test]
        public void ConvertArgumentsToString_ReturnsString_ForString()
        {
            // Arrange
            var arguments = new object[]
                            {
                                "Hello World"
                            };

            LogAspect sut = CreateSut();

            // Act
            string actual = sut.ConvertArgumentsToString(arguments);

            // Assert
            Assert.AreEqual("\"Hello World\"",
                            actual);
        }

        [Test]
        public void CreateInvocationLogString_ReturnsString_ForInvocation()
        {
            // Arrange
            IInvocation invocation = CreateInvocation();
            LogAspect sut = CreateSut();

            // Act
            string actual = sut.CreateInvocationLogString(invocation);

            // Assert
            Assert.AreEqual("Called: LogAspectTests.DoesNotThrow([1,2])",
                            actual);
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
            logger.Received().Debug(Arg.Is <string>(x => x.StartsWith("Called:")));
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

            // todo get this working
            logger.Received().Error(Arg.Any <string>(),
                                    Arg.Any <Exception>());
        }
    }
}