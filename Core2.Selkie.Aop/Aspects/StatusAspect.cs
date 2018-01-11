using System;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Core2.Selkie.Aop.Messages;
using Core2.Selkie.EasyNetQ.Interfaces;
using Core2.Selkie.Windsor;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Transient)]
    public class StatusAspect : IInterceptor
    {
        public StatusAspect([NotNull] ILoggerRepository repository,
                            [NotNull] IStatusRepository statusRepository,
                            [NotNull] ISelkieBus bus,
                            [NotNull] IExceptionLogger exceptionLogger,
                            [NotNull] IExceptionToMessageConverter exceptionToMessageConverter)
        {
            m_Repository = repository;
            m_StatusRepository = statusRepository;
            m_Bus = bus;
            m_ExceptionLogger = exceptionLogger;
            m_ExceptionToMessageConverter = exceptionToMessageConverter;
        }

        private readonly ISelkieBus m_Bus;
        private readonly IExceptionLogger m_ExceptionLogger;
        private readonly IExceptionToMessageConverter m_ExceptionToMessageConverter;
        private readonly ILoggerRepository m_Repository;
        private readonly IStatusRepository m_StatusRepository;

        public void Intercept(IInvocation invocation)
        {
            ILogger logger = m_Repository.Get(invocation.TargetType.FullName);

            if ( logger.IsInfoEnabled )
            {
                string text = m_StatusRepository.Get(invocation.MethodInvocationTarget);

                SendMessagWhenRequired(text,
                                       logger);
            }

            try
            {
                invocation.Proceed();
            }
            catch ( Exception exception )
            {
                m_ExceptionLogger.LogException(invocation,
                                               exception);

                SendExceptionThrownMessage(invocation,
                                           exception);

                throw;
            }
        }

        private void SendExceptionThrownMessage(IInvocation invocation,
                                                Exception exception)
        {
            ExceptionThrownMessage message = m_ExceptionToMessageConverter.CreateExceptionThrownMessage(invocation,
                                                                                                        exception);

            m_Bus.PublishAsync(message);
        }

        private void SendMessagWhenRequired(string text,
                                            ILogger logger)
        {
            if ( string.IsNullOrEmpty(text) )
            {
                return;
            }

            logger.Info(text);

            var message = new StatusMessage
                          {
                              Text = text
                          };

            m_Bus.PublishAsync(message);
        }
    }
}