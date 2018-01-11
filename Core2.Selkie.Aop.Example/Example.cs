using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Core2.Selkie.Aop.Example.Data;
using Core2.Selkie.Aop.Messages;
using Core2.Selkie.EasyNetQ.Interfaces;

namespace Core2.Selkie.Aop.Example
{
    [ExcludeFromCodeCoverage]
    public class Example : IDisposable
    {
        public Example(IWindsorContainer container,
                       IWindsorInstaller installer)
        {
            m_Container = container;
            m_Container.Install(installer);
            m_Bus = m_Container.Resolve <ISelkieBus>();
            m_Logger = m_Container.Resolve <ILogger>();
        }

        private readonly ISelkieBus m_Bus;
        private readonly IWindsorContainer m_Container;
        private readonly ILogger m_Logger;

        public void Dispose()
        {
            m_Container.Release(m_Bus);
            m_Container.Release(m_Logger);
        }

        public void Main()
        {
            var something = m_Container.Resolve <ISomething>();
            var record = new Record(1.0,
                                    2.0,
                                    "Hello World",
                                    3.0);

            TestLogAspect(something,
                          record);

            TestPublishExceptionAspect(something,
                                       record);

            StatusAspect(something,
                         record);
        }

        private void ExceptionThrownHandler(ExceptionThrownMessage message)
        {
            m_Logger.Debug(MessageToText(message));
        }

        private string MessageToText(ExceptionThrownMessage message)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Invocation: {message.Exception.Invocation}");
            builder.AppendLine($"Message: {message.Exception.Message}");
            builder.AppendLine($"StackTrace: {message.Exception.StackTrace}");

            return builder.ToString();
        }

        private void StatusAspect(ISomething something,
                                  Record record)
        {
            something.DoSomethingStatus("");
            something.DoSomethingStatus(record);
        }

        private void StatusHandler(StatusMessage message)
        {
            m_Logger.Debug($"StatusHandler: Text = {message.Text}");
        }

        private void TestLogAspect(ISomething something,
                                   Record record)
        {
            m_Bus.SubscribeAsync <StatusMessage>(GetType().FullName,
                                                 StatusHandler);

            something.DoSomething("");
            m_Logger.Debug($"Augment 10 returns {something.Augment(10)}");
            something.DoSomething(record);
        }

        private void TestPublishExceptionAspect(ISomething something,
                                                Record record)
        {
            try
            {
                m_Bus.SubscribeAsync <ExceptionThrownMessage>(GetType().FullName,
                                                              ExceptionThrownHandler);

                something.DoSomethingThrows(record);
            }
            catch ( Exception exception )
            {
                m_Logger.Debug($"Try/Catch {exception.Message}");
            }
        }
    }
}