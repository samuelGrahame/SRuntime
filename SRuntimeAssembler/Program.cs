using SRuntime;
using System;
using System.Collections.Generic;

namespace SRuntimeAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            Compile(@"..\..\..\..\Idea.txt", @"..\..\..\..\program.data");
        }

        unsafe static void Compile(string source, string destination)
        {
            int constLength = 0;
            ushort index = 0;
            Function app;

            var pushChecks = new List<int>();
            var pushFuncIndex = new List<int>();
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
                    pushFuncIndex.Add(funcHeaders.Count - 1);
                    app.Data[index++] = byte.Parse(arr[1]);
                }
                else if (line.StartsWith("back"))
                {
                    // back 1               
                    app.Data[index++] = 6;
                    var arr = line.Split(' ');
                    pushChecks.Add(index);
                    pushFuncIndex.Add(funcHeaders.Count - 1);
                    app.Data[index++] = byte.Parse(arr[1]);
                }
                else if (line.StartsWith("ret"))
                {
                    // ret 0               
                    app.Data[index++] = 7;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                }
                else if (line.StartsWith("skip.false"))
                {
                    // sk.false 0             
                    app.Data[index++] = 8;
                    var arr = line.Split(' ');

                    app.Data[index++] = byte.Parse(arr[1]);
                    pushChecks.Add(index);
                    pushFuncIndex.Add(funcHeaders.Count - 1);
                    app.Data[index++] = 1;
                }
                else if (line.StartsWith("print"))
                {
                    // ret 0          
                    app.Data[index++] = 9;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                }
                else if (line.StartsWith("equal"))
                {
                    // ret 0          
                    app.Data[index++] = 10;
                    var arr = line.Split(' ');
                    app.Data[index++] = byte.Parse(arr[1]);
                    app.Data[index++] = byte.Parse(arr[2]);
                    app.Data[index++] = byte.Parse(arr[3]);
                }
            }
            for (int i = 0; i < pushChecks.Count; i++)
            {
                int valueToChange = app.Data[pushChecks[i]];

                int mode = app.Data[pushChecks[i] - 1];
                if(mode == 6)
                {
                    var newIndex = funcHeaders[pushFuncIndex[i] - valueToChange];
                    app.Data[pushChecks[i]] = (byte)(newIndex - pushChecks[i]);
                }
                else
                {
                    var newIndex = funcHeaders[pushFuncIndex[i] + valueToChange];
                    app.Data[pushChecks[i]] = (byte)(newIndex - pushChecks[i]);
                }
            }

            var bytes = new List<byte>();
            int length = 0;
            for (int i = 255; i >= 0; i--)
            {
                if(app.ConstData[i] != 0)
                {
                    length = i + 1;
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
                    length = i + 1;
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
