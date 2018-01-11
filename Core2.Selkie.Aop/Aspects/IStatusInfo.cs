using JetBrains.Annotations;

namespace Core2.Selkie.Aop.Aspects
{
    public interface IStatusInfo
    {
        [UsedImplicitly]
        string Text { get; }
    }
}