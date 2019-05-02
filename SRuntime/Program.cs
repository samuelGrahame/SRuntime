using System;

namespace SRuntime
{
    unsafe class Program
    {
        static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return 0;

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

            return app.Run();
        }
    }
}
