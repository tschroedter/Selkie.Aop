using JetBrains.Annotations;

namespace Core2.Selkie.Aop.Example.Data
{
    public interface ISomething
    {
        [UsedImplicitly]
        int Property { get; set; }

        int Augment(int input);
        void DoSomething(string input);

        void DoSomething(Record record);
        void DoSomethingStatus(string input);
        void DoSomethingStatus(Record record);
        void DoSomethingThrows(Record record);
    }
}