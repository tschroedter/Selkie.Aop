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
        private readonly IInvocationToTextConverter m_Converter;

        public PublishExceptionAspect([NotNull] ISelkieBus bus,
                                      [NotNull] IInvocationToTextConverter converter)
        {
            m_Bus = bus;
            m_Converter = converter;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch ( Exception exception )
            {
                var message = new ExceptionThrownMessage
                              {
                                  Invocation = m_Converter.Convert(invocation),
                                  Message = exception.Message,
                                  StackTrace = exception.StackTrace
                              };

                m_Bus.PublishAsync(message);

                throw;
            }
        }
    }
}