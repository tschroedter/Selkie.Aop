using System.Reflection;

namespace Core2.Selkie.Aop.Aspects
{
    public interface IStatusRepository
    {
        string Get(MethodInfo methodInvocationTarget);
    }
}