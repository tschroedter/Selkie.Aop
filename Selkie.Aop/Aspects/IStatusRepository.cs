using System.Reflection;

namespace Selkie.Aop.Aspects
{
    public interface IStatusRepository
    {
        string Get(MethodInfo methodInvocationTarget);
    }
}