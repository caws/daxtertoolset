using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaxterRepacker
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
                    repack(args[a]);
                }

            }
            Console.WriteLine("\nFinished.");
        }

        public static void repack(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs, new ASCIIEncoding()))
                {

                    int pos = 0;
                    byte[] bytes;
                    int limite;
                    long newFilePointer;
                    long newFileSize;
                    byte[] intBytes;
                    string local;

                    //string pasta
                    //System.IO.Directory.CreateDirectory(Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath));
                    Console.WriteLine(Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath));

                    local = Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath);
                    pos = 0x24;

                    System.IO.File.Copy(filepath, Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath) + "-new.bin", true);

                    //abrindo novo arqvuio
                    FileStream novoBin = new FileStream(Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath) + "-new.bin", FileMode.Open, FileAccess.ReadWrite);
                    BinaryReader newbin = new BinaryReader(novoBin, new ASCIIEncoding());

                    br.BaseStream.Seek(pos, 0);
                    bytes = br.ReadBytes(4);
                    limite = BitConverter.ToInt32(bytes, 0);

                    newbin.BaseStream.SetLength(limite);
                    
                    while (pos < limite)
                    {
                        Console.WriteLine("Pegando ponteiro posicao " + pos.ToString("X"));
                        //abrindo arquivo para ler
                        FileStream fs2 = new FileStream(local + "\\" + pos.ToString("X") + ".dat", FileMode.Open, FileAccess.ReadWrite);
                        BinaryReader br2 = new BinaryReader(fs2, new ASCIIEncoding());

                        newFilePointer = newbin.BaseStream.Length;
                        newFileSize = br2.BaseStream.Length;

                        //escrevendo nova posicao
                        newbin.BaseStream.Seek(pos, 0);
                        intBytes = BitConverter.GetBytes(newFilePointer);
                        newbin.BaseStream.Write(intBytes, 0, 4);

                        pos = pos + 4;

                        //escrevendo novo tamanho
                        newbin.BaseStream.Seek(pos, 0);
                        intBytes = BitConverter.GetBytes(newFileSize);
                        newbin.BaseStream.Write(intBytes, 0, 4);

                        pos = pos + 0x0c;

                        br2.BaseStream.Seek(0, 0);
                        bytes = br2.ReadBytes((int)newFileSize);

                        newbin.BaseStream.Seek(newFilePointer,0);
                        newbin.BaseStream.Write(bytes, 0, (int)newFileSize);

                        do
                        {
                            Console.WriteLine(newbin.BaseStream.Length.ToString("X").Substring(newbin.BaseStream.Length.ToString("X").Length - 1, 1));
                            newbin.BaseStream.Seek(newbin.BaseStream.Length, 0);
                            newbin.BaseStream.WriteByte(0x00);
                        }
                        while ((newbin.BaseStream.Length.ToString("X").Substring(newbin.BaseStream.Length.ToString("X").Length - 1, 1) != "0"));
                    }

                    newbin.BaseStream.Seek(0x1c,0);
                    intBytes = BitConverter.GetBytes(newbin.BaseStream.Length);
                    newbin.BaseStream.Write(intBytes, 0, 4);


                    for (int a = 0; a < 0x40; a++)
                    {
                        newbin.BaseStream.Seek(newbin.BaseStream.Length, 0);
                        newbin.BaseStream.WriteByte(0xCC);
                    }

                }
            }
        }
    }
}