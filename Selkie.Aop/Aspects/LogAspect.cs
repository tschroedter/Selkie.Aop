using System;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Selkie.Windsor;

namespace Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Transient)]
    public class LogAspect : IInterceptor
    {
        public LogAspect([NotNull] ILoggerRepository repository,
                         [NotNull] IInvocationToTextConverter converter)
        {
            m_Repository = repository;
            m_Converter = converter;
        }

        private readonly IInvocationToTextConverter m_Converter;
        private readonly ILoggerRepository m_Repository;

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
                if ( logger.IsErrorEnabled )
                {
                    logger.Error(m_Converter.Convert(invocation),
                                 exception);
                }
                throw;
            }
        }
    }
}