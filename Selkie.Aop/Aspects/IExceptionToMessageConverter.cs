using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Selkie.Aop.Messages;

namespace Selkie.Aop.Aspects
{
    public interface IExceptionToMessageConverter
    {
        ExceptionInformation CreateExceptionInformation([NotNull] IInvocation invocation,
                                                        [NotNull] Exception exception);

        void CreateInnerExceptionInformations(List <ExceptionInformation> exceptions,
                                              Exception exception);

        ExceptionThrownMessage CreateExceptionThrownMessage(IInvocation invocation,
                                                            Exception exception);
    }
}