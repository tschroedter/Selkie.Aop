using System;
using NUnit.Framework;
using Selkie.Aop.Messages;

namespace Selkie.Aop.Tests.NUnit.Messages
{
    [TestFixture]
    public sealed class ExceptionThrownMessageTests
    {
        [SetUp]
        public void Setup()
        {
            m_Exception = new ArgumentException("Test");

            m_Sut = new ExceptionThrownMessage
                    {
                        Invocation = "Invocation",
                        Message = m_Exception.Message,
                        StackTrace = m_Exception.StackTrace
                    };
        }

        private ExceptionThrownMessage m_Sut;
        private ArgumentException m_Exception;

        [Test]
        public void Invocation_Roundtrip()
        {
            Assert.AreEqual("Invocation",
                            m_Sut.Invocation);
        }

        [Test]
        public void Message_Roundtrip()
        {
            Assert.AreEqual(m_Exception.Message,
                            m_Sut.Message);
        }

        [Test]
        public void StackTrace_Roundtrip()
        {
            Assert.AreEqual(m_Exception.StackTrace,
                            m_Sut.StackTrace);
        }
    }
}