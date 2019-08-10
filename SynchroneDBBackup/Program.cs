using System;
using System.IO;
using System.Text;
using System.Linq;
using ServerInfoBackup;

//https://blog.bitscry.com/2017/05/30/appsettings-json-in-net-core-console-app/
//https://issue.life/questions/54692345
namespace SynchroneDBBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            Config conf = new Config("appsettings.json");

            Operations oper = new Operations();
            oper.Initial(conf);

            oper.Run();

            Console.WriteLine("Конец");
        }
    }
}
