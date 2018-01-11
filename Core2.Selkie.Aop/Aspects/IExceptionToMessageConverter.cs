using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Core2.Selkie.Aop.Messages;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop.Aspects
{
    public interface IExceptionToMessageConverter
    {
        [UsedImplicitly]
        ExceptionInformation CreateExceptionInformation([NotNull] IInvocation invocation,
                                                        [NotNull] Exception exception);

        [UsedImplicitly]
        ExceptionThrownMessage CreateExceptionThrownMessage(IInvocation invocation,
                                                            Exception exception);

        [UsedImplicitly]
        void CreateInnerExceptionInformations(List <ExceptionInformation> exceptions,
                                              Exception exception);
    }
}