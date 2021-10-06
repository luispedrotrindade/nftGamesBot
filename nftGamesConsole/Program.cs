using nftGamesBot;
using System;
using System.Threading;

namespace nftGamesConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PVU teste = new PVU();

            teste.SetupTest();


            teste.TheUntitledTestCaseTest();

            //while (true)
            //{
            //    try
            //    {
            //        teste.TheUntitledTestCaseTest();
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}
        }
    }
}
