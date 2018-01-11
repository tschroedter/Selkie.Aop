using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Core2.Selkie.Aop.Messages;
using Core2.Selkie.Windsor;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Transient)]
    public class ExceptionToMessageConverter : IExceptionToMessageConverter
    {
        public ExceptionToMessageConverter([NotNull] IInvocationToTextConverter converter)
        {
            m_Converter = converter;
        }

        private readonly IInvocationToTextConverter m_Converter;

        public ExceptionThrownMessage CreateExceptionThrownMessage(IInvocation invocation,
                                                                   Exception exception)
        {
            ExceptionInformation outerException = CreateExceptionInformation(invocation,
                                                                             exception);

            var innerExceptions = new List <ExceptionInformation>();

            CreateInnerExceptionInformations(innerExceptions,
                                             exception);

            var message = new ExceptionThrownMessage
                          {
                              Exception = outerException,
                              InnerExceptions = innerExceptions.ToArray()
                          };

            return message;
        }

        public ExceptionInformation CreateExceptionInformation(IInvocation invocation,
                                                               Exception exception)
        {
            string invocationDetails = m_Converter.Convert(invocation);

            var information = new ExceptionInformation
                              {
                                  Invocation = invocationDetails,
                                  Message = exception.Message,
                                  StackTrace = exception.StackTrace
                              };

            return information;
        }

        public void CreateInnerExceptionInformations(List <ExceptionInformation> exceptions,
                                                     Exception exception)
        {
            Exception innerException = exception.InnerException;

            if ( innerException == null )
            {
                return;
            }

            var information = new ExceptionInformation
                              {
                                  Invocation = "InnerException",
                                  Message = innerException.Message,
                                  StackTrace = innerException.StackTrace
                              };

            exceptions.Add(information);

            // ReSharper disable once TailRecursiveCall
            CreateInnerExceptionInformations(exceptions,
                                             innerException);
        }
    }
}