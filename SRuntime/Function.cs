using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRuntime
{
    public unsafe struct Function
    {
        public fixed int ConstData[256];
        public fixed byte Data[65535];        
        fixed int registor[256];

        static TextWriter textWriter = Console.Out;

        public unsafe int Run()
        {
            int index = 0;

            fixed (int* registor = this.registor)
            fixed (int* ConstData = this.ConstData)
            fixed (byte* Data = this.Data)
            {
                do
                {
                    
                    switch (Data[index])
                    {
                        case 0: // LOAD LITERAL STORE IN REGISTOR - REGISTOR INDEX - COST INDEX
                            registor[Data[++index]] = ConstData[Data[++index]];
                            break;
                        case 1: // ADD - DESTINATION - REG1 - REG2
                            registor[Data[++index]] = registor[Data[++index]] + registor[Data[++index]];
                            break;
                        case 2: // SUB - DESTINATION - REG1 - REG2
                            registor[Data[++index]] = registor[Data[++index]] - registor[Data[++index]];
                            break;
                        case 3: // DIV - DESTINATION - REG1 - REG2
                            registor[Data[++index]] = registor[Data[++index]] / registor[Data[++index]];
                            break;
                        case 4: // MUL - DESTINATION - REG1 - REG2
                            registor[Data[++index]] = registor[Data[++index]] * registor[Data[++index]];
                            break;
                        case 5: // PUSH FORWARD
                            index += Data[++index];
                            break;
                        case 6: // PUSH BACKWARD
                            index -= Data[++index];
                            break;
                        case 7: // RETURN
                            return registor[Data[++index]];
                        case 8: // SKIP FALSE      
                            if(registor[Data[++index]] != 0)
                            {                             
                                index = Data[++index] + index;
                            }
                            index++;                            
                            
                            break;
                        case 9: // ECHO
                            textWriter.WriteLine(registor[Data[++index]]);
                            break;
                        case 10: // EQUAL
                            registor[Data[++index]] = registor[Data[++index]] == registor[Data[++index]] ? 1 : 0;
                            break;
                    }
                } while (index++ < 65535);
            }            

            return 0;
        }
    }
}
