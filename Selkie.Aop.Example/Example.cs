using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Castle.Core.Logging;
using Castle.Windsor;
using Selkie.Aop.Example.Data;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Windsor.Extensions;

namespace Selkie.Aop.Example
{
    [ExcludeFromCodeCoverage]
    //ncrunch: no coverage start
    public class Example
    {
        private readonly WindsorContainer m_Container;

        public Example(WindsorContainer container,
                       Installer installer)
        {
            m_Container = container;
            m_Container.Install(installer);
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
        }

        private void TestPublishExceptionAspect(ISomething something,
                                                Record record)
        {
            try
            {
                var bus = m_Container.Resolve <ISelkieBus>();
                bus.SubscribeAsync <ExceptionThrownMessage>(GetType().FullName,
                                                            ExceptionThrownHandler);
                something.DoSomethingThrows(record);
            }
            catch ( Exception exception )
            {
                var logger = m_Container.Resolve <ILogger>();

                logger.Debug("Try/Catch {0}".Inject(exception.Message));

                m_Container.Release(logger);
            }
        }

        private void TestLogAspect(ISomething something,
                                   Record record)
        {
            var logger = m_Container.Resolve <ILogger>();

            something.DoSomething("");
            logger.Debug("Augment 10 returns {0}".Inject(something.Augment(10)));
            something.DoSomething(record);

            m_Container.Release(logger);
        }

        private void ExceptionThrownHandler(ExceptionThrownMessage message)
        {
            var logger = m_Container.Resolve <ILogger>();

            logger.Debug(MessageToText(message));

            m_Container.Release(logger);
        }

        private string MessageToText(ExceptionThrownMessage message)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Invocation: {0}".Inject(message.Invocation));
            builder.AppendLine("Message: {0}".Inject(message.Message));
            builder.AppendLine("StackTrace: {0}".Inject(message.StackTrace));

            return builder.ToString();
        }
    }
}