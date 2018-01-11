using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.DynamicProxy;
using Core2.Selkie.Aop.Tests.Aspects;
using NSubstitute;
using NUnit.Framework;

namespace Core2.Selkie.Aop.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public sealed class InvocationToTextConverterTests
    {
        private InvocationToTextConverter CreateSut()
        {
            return new InvocationToTextConverter();
        }

        private class Record
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Number { get; set; }

            public string Text { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
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
            invocation.TargetType.Returns(typeof( LogAspectTests ));
            invocation.Method.Returns(action.GetMethodInfo());
            invocation.Arguments.Returns(arguments);

            return invocation;
        }

        private void DoesNotThrow()
        {
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

            InvocationToTextConverter sut = CreateSut();

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

            InvocationToTextConverter sut = CreateSut();

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

            InvocationToTextConverter sut = CreateSut();

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

            InvocationToTextConverter sut = CreateSut();

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

            InvocationToTextConverter sut = CreateSut();

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

            InvocationToTextConverter sut = CreateSut();

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

            InvocationToTextConverter sut = CreateSut();

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

            InvocationToTextConverter sut = CreateSut();

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
            InvocationToTextConverter sut = CreateSut();

            // Act
            string actual = sut.Convert(invocation);

            // Assert
            Assert.AreEqual("LogAspectTests.DoesNotThrow([1,2])",
                            actual);
        }
    }
}