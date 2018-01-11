using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Core2.Selkie.Aop.Aspects;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Core2.Selkie.Aop.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal sealed class StatusRepositoryTests
    {
        [SetUp]
        public void Setup()
        {
            m_InfoOne = typeof( TestClass ).GetMethod("DoNothing",
                                                      new Type[0]);
            m_InfoTwo = typeof( TestClass ).GetMethod("DoNothing",
                                                      new[]
                                                      {
                                                          typeof( int )
                                                      });
            m_InfoOther = typeof( TestClass ).GetMethod("DoNothingElse");

            m_Sut = new StatusRepository();
        }

        private StatusRepository m_Sut;
        private MethodInfo m_InfoOne;
        private MethodInfo m_InfoTwo;
        private MethodInfo m_InfoOther;

        private class TestClass
        {
            [Status("DoNothing")]
            [UsedImplicitly]
            public void DoNothing()
            {
            }

            [Status("DoNothingWithParameterInt")]
            [UsedImplicitly]
            public void DoNothing(int value)
            {
            }

            [Status("DoNothingElse")]
            [UsedImplicitly]
            public void DoNothingElse()
            {
            }
        }

        [Test]
        public void Get_CreatesNewEntry_ForNewMethodInfo()
        {
            // Arrange
            const string expected = "DoNothing";

            // Act
            string actual = m_Sut.Get(m_InfoOne);

            // Assert
            Assert.NotNull(expected,
                           actual);
        }

        [Test]
        public void Get_ReturnsDifferentStatus_ForDifferentMethodInfo()
        {
            // Arrange
            string one = m_Sut.Get(m_InfoOne);
            string two = m_Sut.Get(m_InfoOther);

            // Act
            // Assert
            Assert.AreNotEqual(two,
                               one);
        }

        [Test]
        public void Get_ReturnsDifferentStatus_ForMethodInfosWithSameNameButDifferentParameters()
        {
            // Arrange
            string one = m_Sut.Get(m_InfoOne);
            string two = m_Sut.Get(m_InfoTwo);

            // Act
            // Assert
            Assert.AreNotEqual(two,
                               one);
        }

        [Test]
        public void Get_ReturnsSameStatus_ForExistingMethodInfo()
        {
            // Arrange
            string expected = m_Sut.Get(m_InfoOne);

            // Act
            string actual = m_Sut.Get(m_InfoOne);

            // Assert
            Assert.AreEqual(expected,
                            actual);
        }
    }
}