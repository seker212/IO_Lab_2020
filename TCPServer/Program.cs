using TCPServer;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            var Server = new ServerAPM();
            Server.Start();
            //Test.test();
        }
    }
}
