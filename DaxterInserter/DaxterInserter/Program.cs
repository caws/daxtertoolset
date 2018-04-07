using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaxterInserter
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
                    inserter(args[a]);
                }

            }
            Console.WriteLine("\nFinished.");
        }

        public static void inserter(string filepath)
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
                    int endPointerTable;
                    int newTextPointer;
                    byte[] intBytes;

                    //string pasta
                    //System.IO.Directory.CreateDirectory(Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath));
                    Console.WriteLine(Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath));
                    string local = Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath);
                    System.IO.File.Copy(filepath, Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath) + "-new.dat", true);
                    //abrindo novo arqvuio
                    FileStream novoDat = new FileStream(Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath) + "-new.dat", FileMode.Open, FileAccess.ReadWrite);
                    BinaryReader newDat = new BinaryReader(novoDat, new ASCIIEncoding());
                    //abrindo arquivo de texto
                    FileStream dumpedText = new FileStream(filepath + ".txt", FileMode.Open, FileAccess.ReadWrite);
                    BinaryReader dmpText = new BinaryReader(dumpedText, new ASCIIEncoding());

                    pos = 0x10;

                    br.BaseStream.Seek(pos, 0);
                    bytes = br.ReadBytes(4);
                    endPointerTable = BitConverter.ToInt32(bytes, 0);

                    pos = 0x20;

                    br.BaseStream.Seek(pos, 0);
                    bytes = br.ReadBytes(4);
                    limite = BitConverter.ToInt32(bytes, 0);

                    newDat.BaseStream.SetLength(limite);

                    while (pos < endPointerTable)
                    {
                        Console.WriteLine("Pegando ponteiro posicao " + pos.ToString("X"));
                        //escrevendo novo ponteiro
                        newDat.BaseStream.Seek(pos, 0);
                        intBytes = BitConverter.GetBytes(newDat.BaseStream.Length);
                        newDat.BaseStream.Write(intBytes, 0, 4);
                        pos = pos + 0x10;

                        byte letra = 1;
                        while (letra != 0x0d)
                        {
                            letra = dmpText.ReadByte();
                            //Console.WriteLine(letra.ToString("X"));
                            if (letra != 0x0d)
                            {
                                newDat.BaseStream.Seek(newDat.BaseStream.Length, 0);
                                newDat.BaseStream.WriteByte(letra);
                            }
                            //Console.ReadKey();
                        }
                        //inserindo null entre strings
                        newDat.BaseStream.Seek(newDat.BaseStream.Length, 0);
                        newDat.BaseStream.WriteByte(0x00);
                        //corringindo ponteiro no texto
                        dmpText.ReadByte();
                        Console.WriteLine("Posi " + dmpText.BaseStream.Position.ToString("X"));
                    }
                }
            }
        }
    }
}