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
                for (int a = 0; a < args.Length; a++)
                {
                    unpack(args[a]);
                }

            }
            Console.WriteLine("\nFinished.");
        }

        public static void unpack(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs, new ASCIIEncoding()))
                {

                    int i = 0;
                    int pos = 0;
                    byte[] bytes;
                    int filePointer;
                    int fileSize;
                    int limite;
                    string local;

                    //criando pasta
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath));
                    Console.WriteLine(Path.GetDirectoryName(filepath)+"\\"+Path.GetFileNameWithoutExtension(filepath));

                    local = Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath);
                    pos = 0x24;

                    br.BaseStream.Seek(pos, 0);
                    bytes = br.ReadBytes(4);
                    limite = BitConverter.ToInt32(bytes, 0);

                    while (pos < limite)
                    {
                        Console.WriteLine("Pegando ponteiro posicao " + pos.ToString("X"));
                        //criando arquivo para escrever
                        FileStream fs2 = new FileStream(local + "\\"+pos.ToString("X") + ".dat", FileMode.Create, FileAccess.ReadWrite);
                        BinaryReader br2 = new BinaryReader(fs2, new ASCIIEncoding());

                        //lendo endereço
                        br.BaseStream.Seek(pos, 0);
                        bytes = br.ReadBytes(4);
                        filePointer = BitConverter.ToInt32(bytes, 0);

                        pos = pos + 4;

                        br.BaseStream.Seek(pos, 0);
                        bytes = br.ReadBytes(4);
                        fileSize = BitConverter.ToInt32(bytes, 0);

                        pos = pos + 0x0c;

                        br.BaseStream.Seek(filePointer, 0);
                        bytes = br.ReadBytes(fileSize);

                        br2.BaseStream.Write(bytes, 0, fileSize);
                    }
                }
            }
        }
    }
}