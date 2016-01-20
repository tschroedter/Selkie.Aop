using System;
using Castle.Windsor;
using Selkie.Aop.Example.Data;

namespace Selkie.Aop.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new WindsorContainer();
            var installer = new Installer();

            var anthill = new Example(container,
                                      installer);

            anthill.Main();

            Console.ReadLine();

            Environment.Exit(0);
        }

        public class Example
        {
            private readonly WindsorContainer m_Container;

            public Example(WindsorContainer container,
                           Installer installer)
            {
                m_Container = container;
                m_Container.Install(installer);
            }

            public void Main()
            {
                var something = m_Container.Resolve<ISomething>();

                something.DoSomething("");
                Console.WriteLine("Augment 10 returns " + something.Augment(10));
                something.DoSomething(new Record(1.0, 2.0, "Hello World", 3.0));
            }
        }
    }
}
