using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Castle.DynamicProxy;
using NSubstitute;
using NUnit.Framework;
using Selkie.Aop.Aspects;
using Selkie.Aop.Messages;

namespace Selkie.Aop.Tests.Aspects
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class ExceptionToMessageConverterTests
    {
        [SetUp]
        public void Setup()
        {
            m_Converter = Substitute.For <IInvocationToTextConverter>();

            m_Sut = new ExceptionToMessageConverter(m_Converter);
        }

        private IInvocationToTextConverter m_Converter;
        private ExceptionToMessageConverter m_Sut;

        private static Exception CreateExceptionWithInnerExceptions()
        {
            var innerExceptionTwo = new Exception("Inner Exception Two!");
            var innerExceptionOne = new Exception("Inner Exception One!",
                                                  innerExceptionTwo);
            var exception = new Exception("With Inner Exceptions!",
                                          innerExceptionOne);
            return exception;
        }

        [Test]
        public void CreateExceptionInformation_ReturnsInformationWithInvocationSet_ForException()
        {
            // Arrange
            var exception = new Exception("No Inner Exceptions!");

            m_Converter.Convert(Arg.Any <IInvocation>()).Returns("Test");

            // Act
            ExceptionInformation actual = m_Sut.CreateExceptionInformation(Substitute.For <IInvocation>(),
                                                                           exception);

            // Assert
            Assert.AreEqual("Test",
                            actual.Invocation);
        }

        [Test]
        public void CreateExceptionInformation_ReturnsInformationWithMessageSet_ForException()
        {
            // Arrange
            const string expected = "No Inner Exceptions!";
            var exception = new Exception(expected);

            // Act
            ExceptionInformation actual = m_Sut.CreateExceptionInformation(Substitute.For <IInvocation>(),
                                                                           exception);

            // Assert
            Assert.AreEqual(expected,
                            actual.Message);
        }

        [Test]
        public void CreateExceptionInformation_ReturnsInformationWithStackTraceSet_ForException()
        {
            // Arrange
            try
            {
                var exception = new Exception("No Inner Exceptions!");

                throw exception;
            }
            catch ( Exception exception )
            {
                // Act
                ExceptionInformation actual = m_Sut.CreateExceptionInformation(Substitute.For <IInvocation>(),
                                                                               exception);

                // Assert
                Assert.True(actual.StackTrace.StartsWith("   at Selkie.Aop.Tests.Aspects.ExceptionToMessageConver"));
            }
        }

        [Test]
        public void
            CreateExceptionThrownMessage_ReturnsMessageAndInnerExceptionsIsEmpty_ForExceptionWithoutInnerException()
        {
            // Arrange
            var exception = new Exception("No Inner Exceptions!");

            // Act
            ExceptionThrownMessage actual = m_Sut.CreateExceptionThrownMessage(Substitute.For <IInvocation>(),
                                                                               exception);

            // Assert
            Assert.AreEqual(0,
                            actual.InnerExceptions.Length);
        }

        [Test]
        public void CreateExceptionThrownMessage_ReturnsMessageAndSetsException_ForExceptionWithoutInnerException()
        {
            // Arrange
            var expected = "No Inner Exceptions!";
            var exception = new Exception(expected);

            // Act
            ExceptionThrownMessage actual = m_Sut.CreateExceptionThrownMessage(Substitute.For <IInvocation>(),
                                                                               exception);

            // Assert
            Assert.AreEqual(expected,
                            actual.Exception.Message);
        }

        [Test]
        public void CreateExceptionThrownMessage_ReturnsMessageAndSetsExceptions_ForExceptionInnerException()
        {
            // Arrange
            Exception exception = CreateExceptionWithInnerExceptions();

            // Act
            ExceptionThrownMessage actual = m_Sut.CreateExceptionThrownMessage(Substitute.For <IInvocation>(),
                                                                               exception);

            // Assert
            Assert.AreEqual("With Inner Exceptions!",
                            actual.Exception.Message);
        }

        [Test]
        public void CreateExceptionThrownMessage_ReturnsMessageAndSetsInnerExceptions_ForExceptionInnerException()
        {
            // Arrange
            Exception exception = CreateExceptionWithInnerExceptions();

            // Act
            ExceptionThrownMessage actual = m_Sut.CreateExceptionThrownMessage(Substitute.For <IInvocation>(),
                                                                               exception);

            // Assert
            Assert.AreEqual(2,
                            actual.InnerExceptions.Length);
        }

        [Test]
        public void CreateInnerExceptionInformations_ReturnsEmptyList_ForExceptionWithoutInnerException()
        {
            // Arrange
            var exception = new Exception("No Inner Exceptions!");
            var actual = new List <ExceptionInformation>();
            // Act
            m_Sut.CreateInnerExceptionInformations(actual,
                                                   exception);

            // Assert
            Assert.AreEqual(0,
                            actual.Count);
        }

        [Test]
        public void CreateInnerExceptionInformations_ReturnsFirstInnerException_ForExceptionInnerExceptions()
        {
            // Arrange
            Exception exception = CreateExceptionWithInnerExceptions();

            var actual = new List <ExceptionInformation>();
            // Act
            m_Sut.CreateInnerExceptionInformations(actual,
                                                   exception);

            // Assert
            Assert.AreEqual("Inner Exception One!",
                            actual [ 0 ].Message);
        }

        [Test]
        public void CreateInnerExceptionInformations_ReturnsNotEmptyList_ForExceptionInnerExceptions()
        {
            // Arrange
            Exception exception = CreateExceptionWithInnerExceptions();

            var actual = new List <ExceptionInformation>();
            // Act
            m_Sut.CreateInnerExceptionInformations(actual,
                                                   exception);

            // Assert
            Assert.AreEqual(2,
                            actual.Count);
        }

        [Test]
        public void CreateInnerExceptionInformations_ReturnsSecondInnerException_ForExceptionInnerExceptions()
        {
            // Arrange
            Exception exception = CreateExceptionWithInnerExceptions();

            var actual = new List <ExceptionInformation>();
            // Act
            m_Sut.CreateInnerExceptionInformations(actual,
                                                   exception);

            // Assert
            Assert.AreEqual("Inner Exception Two!",
                            actual [ 1 ].Message);
        }
    }
}