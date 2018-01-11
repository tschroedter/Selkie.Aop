using System;
using System.Text;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Core2.Selkie.Windsor;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Transient)]
    [UsedImplicitly]
    public class ExceptionLogger : IExceptionLogger
    {
        public ExceptionLogger([NotNull] ILogger logger,
                               [NotNull] IInvocationToTextConverter converter)
        {
            m_Logger = logger;
            m_Converter = converter;
        }

        private readonly IInvocationToTextConverter m_Converter;
        private readonly ILogger m_Logger;

        public void LogException(IInvocation invocation,
                                 Exception exception)
        {
            if ( !m_Logger.IsErrorEnabled )
            {
                return;
            }

            var builder = new StringBuilder();

            builder.AppendLine($"{GetType().Name}: Caught exception!" +
                               $" - Offending method call: {m_Converter.Convert(invocation)}");
            builder.AppendLine(exception.ToString());
            builder.AppendLine(exception.StackTrace);

            string message = builder.ToString();

            m_Logger.Error(message,
                           exception);
        }
    }
}