using System;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Selkie.Aop.Messages;
using Selkie.EasyNetQ;
using Selkie.Windsor;

namespace Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Transient)]
    public class MessageHandlerAspect : IInterceptor
    {
        private readonly ISelkieBus m_Bus;
        private readonly IExceptionToMessageConverter m_ExceptionToMessageConverter;
        private readonly IExceptionLogger m_ExceptionLogger;
        private readonly IInvocationToTextConverter m_InvocationToTextConverter;
        private readonly ILoggerRepository m_Repository;

        public MessageHandlerAspect([NotNull] ISelkieBus bus,
                                    [NotNull] ILoggerRepository repository,
                                    [NotNull] IInvocationToTextConverter invocationToTextConverter,
                                    [NotNull] IExceptionToMessageConverter exceptionToMessageConverter,
                                    [NotNull] IExceptionLogger exceptionLogger)
        {
            m_Bus = bus;
            m_Repository = repository;
            m_InvocationToTextConverter = invocationToTextConverter;
            m_ExceptionToMessageConverter = exceptionToMessageConverter;
            m_ExceptionLogger = exceptionLogger;
        }

        public void Intercept(IInvocation invocation)
        {
            ILogger logger = m_Repository.Get(invocation.TargetType.FullName);

            if ( logger.IsDebugEnabled )
            {
                logger.Debug(m_InvocationToTextConverter.Convert(invocation));
            }

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
            }
        }

        private void SendMessage([NotNull] IInvocation invocation,
                                 [NotNull] Exception exception)
        {
            ExceptionThrownMessage message = m_ExceptionToMessageConverter.CreateExceptionThrownMessage(invocation,
                                                                                                        exception);

            m_Bus.PublishAsync(message);
        }
    }
}