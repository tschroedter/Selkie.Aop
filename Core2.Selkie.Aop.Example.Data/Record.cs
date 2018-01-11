using System.Diagnostics.CodeAnalysis;

// ReSharper disable MemberCanBePrivate.Global

namespace Core2.Selkie.Aop.Example.Data
{
    [ExcludeFromCodeCoverage]
    public class Record
    {
        public Record(double n1,
                      double n2,
                      string operation,
                      double result)
        {
            OperandNumberOne = n1;
            OperandNumberTwo = n2;
            Operation = operation;
            Result = result;
        }

        public double OperandNumberOne { get; }

        public double OperandNumberTwo { get; }

        public string Operation { get; }

        public double Result { get; }

        public override string ToString()
        {
            return string.Format("Record: {0} {1} {2} = {3}",
                                 OperandNumberOne,
                                 Operation,
                                 OperandNumberTwo,
                                 Result);
        }
    }
}