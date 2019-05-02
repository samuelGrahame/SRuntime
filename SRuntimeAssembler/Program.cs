using SRuntime;
using System;
using System.Collections.Generic;

namespace SRuntimeAssembler
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
        }

        unsafe static void Compile(string source, string destination)
        {
            int constLength = 0;
            ushort index = 0;
            Function app;

            var pushChecks = new List<int>();
            var pushCheckInstructionIndex = new List<int>();
            var funcHeaders = new List<int>();
                        
            foreach (var line in System.IO.File.ReadAllLines(source))
            {
                if(string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var indexOfOpCode = index;
                funcHeaders.Add(index);
                if (line.StartsWith("const"))
                {
                    //const 1
                    app.ConstData[constLength++] = int.Parse(line.Split(' ')[1]);                     
                }else if (line.StartsWith("set"))
                {
                    // set 0 0                    
                    app.Data[index++] = 0;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                    app.Data[index++] = byte.Parse(arr[2]);                    
                }
                else if (line.StartsWith("add"))
                {
                    // add 2 0 1                 
                    app.Data[index++] = 1;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                    app.Data[index++] = byte.Parse(arr[2]);
                    app.Data[index++] = byte.Parse(arr[3]);
                }
                else if (line.StartsWith("sub"))
                {
                    // sub 2 0 1                 
                    app.Data[index++] = 2;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                    app.Data[index++] = byte.Parse(arr[2]);
                    app.Data[index++] = byte.Parse(arr[3]);
                }
                else if (line.StartsWith("div"))
                {
                    // div 2 0 1                 
                    app.Data[index++] = 3;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                    app.Data[index++] = byte.Parse(arr[2]);
                    app.Data[index++] = byte.Parse(arr[3]);
                }
                else if (line.StartsWith("mul"))
                {
                    // mul 2 0 1                 
                    app.Data[index++] = 4;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                    app.Data[index++] = byte.Parse(arr[2]);
                    app.Data[index++] = byte.Parse(arr[3]);
                }
                else if (line.StartsWith("forward"))
                {
                    // forward 1                 
                    app.Data[index++] = 5;
                    var arr = line.Split(' ');
                    pushChecks.Add(index);
                    pushCheckInstructionIndex.Add(indexOfOpCode);
                    app.Data[index++] = byte.Parse(arr[1]);
                }
                else if (line.StartsWith("back"))
                {
                    // back 1               
                    app.Data[index++] = 6;
                    var arr = line.Split(' ');
                    pushChecks.Add(index);
                    pushCheckInstructionIndex.Add(indexOfOpCode);
                    app.Data[index++] = byte.Parse(arr[1]);
                }
                else if (line.StartsWith("ret"))
                {
                    // ret 0               
                    app.Data[index++] = 7;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                }
                else if (line.StartsWith("sk.false"))
                {
                    // sk.false 0             
                    app.Data[index++] = 8;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                    pushChecks.Add(index);
                    pushCheckInstructionIndex.Add(indexOfOpCode);
                    app.Data[index++] = 1;
                }
            }
            for (int i = 0; i < pushChecks.Count; i++)
            {
                int valueToChange = app.Data[pushChecks[i]];
                int indexOfOp = pushCheckInstructionIndex[i];
                var newIndex = funcHeaders[indexOfOp + valueToChange];
                app.Data[pushChecks[i]] = (byte)(newIndex - pushChecks[i]);
            }

            var bytes = new List<byte>();
            int length = 0;
            for (int i = 255; i >= 0; i--)
            {
                if(app.ConstData[i] != 0)
                {
                    length = 255 - i;
                    bytes.Add((byte)(length));
                    break;
                }
            }            
            for (int i = 0; i < length; i++)
            {
                bytes.AddRange(BitConverter.GetBytes(app.ConstData[i]));
            }

            for (int i = 65535; i >= 0; i--)
            {
                if (app.Data[i] != 0)
                {
                    length = 65535 - i;
                    bytes.AddRange( BitConverter.GetBytes((ushort)(length)));
                    break;
                }
            }
            for (int i = 0; i < length; i++)
            {
                bytes.Add(app.Data[i]);
            }
            System.IO.File.WriteAllBytes(destination, bytes.ToArray());
        }
    }
}
