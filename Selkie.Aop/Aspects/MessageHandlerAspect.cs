using System;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Transient)]
    public class MessageHandlerAspect : IInterceptor
    {
        private readonly ISelkieBus m_Bus;
        private readonly IInvocationToTextConverter m_Converter;
        private readonly ILoggerRepository m_Repository;

        public MessageHandlerAspect([NotNull] ISelkieBus bus,
                                    [NotNull] ILoggerRepository repository,
                                    [NotNull] IInvocationToTextConverter converter)
        {
            m_Bus = bus;
            m_Repository = repository;
            m_Converter = converter;
        }

        public void Intercept(IInvocation invocation)
        {
            ILogger logger = m_Repository.Get(invocation.TargetType.FullName);

            if ( logger.IsDebugEnabled )
            {
                logger.Debug(m_Converter.Convert(invocation));
            }

            try
            {
                invocation.Proceed();
            }
            catch ( Exception exception )
            {
                LogException(invocation,
                             logger,
                             exception);

                SendMessage(invocation,
                            exception);
            }
        }

        private void LogException(IInvocation invocation,
                                  ILogger logger,
                                  Exception exception)
        {
            if ( !logger.IsErrorEnabled )
            {
                return;
            }

            string invocationDetails = m_Converter.Convert(invocation);

            string inject = "{0}: Caught exception! - Offending method call: {1}".Inject(GetType().Name,
                                                                                         invocationDetails);
            logger.Error(inject,
                         exception);
        }

        private void SendMessage(IInvocation invocation,
                                 Exception exception)
        {
            string invocationDetails = m_Converter.Convert(invocation);

            var message = new ExceptionThrownMessage
                          {
                              Invocation = invocationDetails,
                              Message = exception.Message,
                              StackTrace = exception.StackTrace
                          };

            m_Bus.PublishAsync(message);
        }
    }
}