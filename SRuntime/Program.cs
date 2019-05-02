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
                app.ConstData[i] = data[index++];

            length = (ushort)(data[index++] + (data[index++] << 8));
            for (i = 0; i < length; i++)
                app.Data[i] = data[index++];

            return app.Run();
        }
    }
}
