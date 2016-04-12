using System;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Windsor;

namespace Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Transient)]
    public class PublishExceptionAspect : IInterceptor
    {
        private readonly ISelkieBus m_Bus;
        private readonly IExceptionLogger m_ExceptionLogger;
        private readonly IExceptionToMessageConverter m_ExceptionToMessageConverter;

        public PublishExceptionAspect([NotNull] ISelkieBus bus,
                                      [NotNull] IExceptionLogger exceptionLogger,
                                      [NotNull] IExceptionToMessageConverter exceptionToMessageConverter)
        {
            m_Bus = bus;
            m_ExceptionLogger = exceptionLogger;
            m_ExceptionToMessageConverter = exceptionToMessageConverter;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch ( Exception exception )
            {
                m_ExceptionLogger.LogException(invocation,
                                               exception);

                SendMessage(invocation,
                            exception);

                throw;
            }
        }

        private void SendMessage(IInvocation invocation,
                                 Exception exception)
        {
            ExceptionThrownMessage message = m_ExceptionToMessageConverter.CreateExceptionThrownMessage(invocation,
                                                                                                        exception);

            m_Bus.PublishAsync(message);
        }
    }
}