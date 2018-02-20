using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace node
{
    class Program
    {
        static void Main(String[] args)
        {
            NodeService nodeService = new NodeService();
            nodeService.Start(10009);
            Console.ReadLine();
        }
    }
}
