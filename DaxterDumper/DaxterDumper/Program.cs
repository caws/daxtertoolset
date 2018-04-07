using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaxterDumper
{
    class Program
    {
        public static object MessageBox { get; private set; }

        static void Main(string[] args)
        {
            if ((args.Length == 0) || !File.Exists(args[0]))
            {
                Console.WriteLine("Please provide an existing file name.");
            }
            else
            {
                dumparText(args[0]);
            }
            Console.WriteLine("\nFinished.");
        }

        public static void dumparText(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs, new ASCIIEncoding()))
                {

                    int i = 0;
                    int pos = 0;
                    byte[] bytes;
                    int textpointer;
                    int limite;

                    FileStream fs2 = new FileStream(filepath + ".txt", FileMode.Create, FileAccess.ReadWrite);
                    BinaryReader br2 = new BinaryReader(fs2, new ASCIIEncoding());

                    pos = 0x10;

                    br.BaseStream.Seek(pos, 0);
                    bytes = br.ReadBytes(4);
                    limite = BitConverter.ToInt32(bytes, 0);

                    pos = 0x20;

                    while (pos < limite)
                    {
                        Console.WriteLine("Pegando ponteiro posicao " + pos.ToString("X"));
                        //lendo endereço

                        br.BaseStream.Seek(pos, 0);
                        bytes = br.ReadBytes(4);
                        textpointer = BitConverter.ToInt32(bytes, 0);

                        pos = pos + 0x10;

                        br.BaseStream.Seek(textpointer, 0);
                        byte i2 = 0x01;
                        i = 2;
                        while (i2 != 0)
                        {
                            i2 = br.ReadByte();
                            if (i2 == 0)
                            {
                                br2.BaseStream.WriteByte(0x0d);
                                br2.BaseStream.WriteByte(0x0a);
                                break;
                            }
                            br2.BaseStream.WriteByte(i2);
                        }
                    }
                }
            }
        }
    }
}