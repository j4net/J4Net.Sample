using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO change it!
            var jar = "D:\\Downloads\\J4Net.Sample\\java";
            Environment.SetEnvironmentVariable("JVM_DLL_PATH", Environment.GetEnvironmentVariable("JAVA_HOME") + "\\jre\\bin\\server\\jvm.dll");
            Environment.SetEnvironmentVariable("JAVA_OPTS", "-Xms64m -Xmx64m -Xss1m -Djava.class.path=" + jar);

            var sample = new Sample(1, 2, "Masha");
            Console.WriteLine(sample.Inc(1));
            Sample.Print("=)");

            //todo exception
            Console.WriteLine(sample.Inc(0));
        }
    }
}
