using System;
using System.Diagnostics.CodeAnalysis;
using Castle.Windsor;

namespace Selkie.Aop.Example
{
    [ExcludeFromCodeCoverage]
    //ncrunch: no coverage start
    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new WindsorContainer();
            var installer = new Installer();

            var example = new Example(container,
                                      installer);

            example.Main();

            Console.ReadLine();

            example.Dispose();

            Environment.Exit(0);
        }
    }
}