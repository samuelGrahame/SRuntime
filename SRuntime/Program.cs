using System;
using System.Diagnostics;

namespace SRuntime
{
    unsafe class Program
    {
        static int Main(string[] args)
        {            
            if (args == null || args.Length == 0)
                return 0;

            //args = new string[] { @"C:\Users\samuel grahame\Desktop\program.data" };

            Function app;

            var data = System.IO.File.ReadAllBytes(args[0]);
            var index = 0;
            int i;

            ushort length = data[index++];
            for (i = 0; i < length; i++)
                app.ConstData[i] = BitConverter.ToInt32(new byte[] { data[index++], data[index++], data[index++], data[index++] });

            length = BitConverter.ToUInt16(new byte[] { data[index++], data[index++] });

            for (i = 0; i < length; i++)
                app.Data[i] = data[index++];

            Test(10, ref app);
            Test(100, ref app);
            Test(1000, ref app);
            Test(10000, ref app);

            Console.ReadKey();

            return 0;
        }

        static void Test(int intervals, ref Function func)
        {
            var st = Stopwatch.StartNew();

            for (int i = 0; i < intervals; i++)
            {
                func.Run();                
            }

            st.Stop();

            Console.WriteLine("Total Time " + (st.ElapsedTicks * 100.0f) + "ns - Avg Time " + ((st.ElapsedTicks * 100.0f) / intervals) + "ns");
        }
    }
}
