using System;
using System.Collections.Generic;
using System.Text;

namespace _dsConvertor
{
    class Program
    {
        static void Main(string[] args)
        {
            _3dsConvertor.Convertor convertor = new _3dsConvertor.Convertor();

            //if (args.Length != 0)
            //    convertor.ConvertToFile(args[0], args[1]);

            //else
            {
                string pathInput,pathOutput;

                Console.WriteLine("Convert From .3DS Format To TEXT Format.");

                pathInput = Console.ReadLine();
                pathOutput = pathInput.Remove(pathInput.Length - 3);
                pathOutput += "txt";

                convertor.ConvertToFile(pathInput, pathOutput);
            }

        }
    }
}
