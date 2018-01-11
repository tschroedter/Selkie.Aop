using System;
using Castle.DynamicProxy;
using Core2.Selkie.Aop.Messages;
using Core2.Selkie.EasyNetQ.Interfaces;
using Core2.Selkie.Windsor;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Transient)]
    [UsedImplicitly]
    public class PublishExceptionAspect : IInterceptor
    {
        public PublishExceptionAspect([NotNull] ISelkieBus bus,
                                      [NotNull] IExceptionLogger exceptionLogger,
                                      [NotNull] IExceptionToMessageConverter exceptionToMessageConverter)
        {
            m_Bus = bus;
            m_ExceptionLogger = exceptionLogger;
            m_ExceptionToMessageConverter = exceptionToMessageConverter;
        }

        private readonly ISelkieBus m_Bus;
        private readonly IExceptionLogger m_ExceptionLogger;
        private readonly IExceptionToMessageConverter m_ExceptionToMessageConverter;

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