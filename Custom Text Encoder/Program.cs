using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleJSON;
using System.Diagnostics;

namespace Custom_Text_Encoder
{
    internal class Program
    {
        static void WriteWithColor<T>(T value, ConsoleColor color, bool endLine = false)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (endLine)
            {
                Console.WriteLine(value);
            }
            else
            {
                Console.Write(value);
            }
            Console.ForegroundColor = oldColor;
        }

        static bool CheckCodec(string filePath)
        {
            Console.Write("Codec status: ");
            if (File.Exists(filePath))
            {
                string fileContents = File.ReadAllText(filePath);
                if (fileContents.StartsWith("{") && fileContents.EndsWith("}"))
                {
                    WriteWithColor("Loaded", ConsoleColor.Green, true);
                    return true;
                }
            }
            WriteWithColor("Not Loaded", ConsoleColor.Red, true);
            return false;
        }

        static void SetupArray(JSONNode jsonFile, char[] normalChar, string[] encodedChar)
        {
            List<char> keys = new List<char>();
            List<string> values = new List<string>();
            foreach (var item in jsonFile)
            {
                keys.Add(item.Key[0]);
                values.Add(item.Value);
            }
#pragma warning disable IDE0059
            normalChar = keys.ToArray();
            encodedChar = values.ToArray();
#pragma warning restore IDE0059
        }

        static string Chiper(string text, char[] normalChar, string[] encodedChar)
        {
            for (int i = 0; i < normalChar.Length; i++)
            {
                text = text.Replace(normalChar[i].ToString(), encodedChar[i]);
            }
            return text;
        }

        static void CreateJSONCodec(string filePath, List<char> chars, List<string> encodedChars)
        {

        }

        static void Main()
        {
            int result;
            do
            {
                Console.Clear();
                string codecPath = "";
                bool isCodecLoaded = CheckCodec(codecPath);
                Console.Write($"Select Mode:\n1. Encode\n2. Decode\n3. Load Encoding Format\n4. Create Encoding Format\n5. Modify Encoding Format\nChoose an option: ");
                int.TryParse(Console.ReadLine(), out result);
                Console.Clear();
                switch (result)
                {
                    case -1: //DEBUG
                        Console.ReadLine();
                        break;
                    case 1:
                        if (!isCodecLoaded)
                        {
                            Console.Write("Choose an encoding format before!\n\nType 0 to create a new encoding format or press enter to restart the program: ");
                            if (Console.ReadLine() == "0") { Console.Clear(); goto case 4; }
                            else result = 0;
                        }
                        break;
                    case 2:
                    case 3:
                    case 4:
                        Console.Write("Hi, to create a new encoding format you need to follow these rules:\nThe text format must be the following: ");
                        WriteWithColor('X', ConsoleColor.Yellow);
                        Console.Write('-');
                        WriteWithColor("YYY", ConsoleColor.Green);
                        Console.Write(" where\n - ");
                        WriteWithColor('X', ConsoleColor.Yellow);
                        Console.Write(" is the character that you want to encode\n - ");
                        WriteWithColor('Y', ConsoleColor.Green);
                        Console.Write(" is the encoded form of that character\nFor example if i want to encode all \"A\" to become \"z\", I'll write A-z\nThe program will accept input until you write ");
                        WriteWithColor("STOP", ConsoleColor.Red, true);
                        Console.WriteLine("You can write multiple entries in two ways:\n 1. Separated by a comma(Ex: A-z, L-Q, 0-ADA, 3-0x03)\n 2. By writing one at time, Ex:\nA-z\nL-Q\n0-ADA\n3-0x03\nIllegal characters: \',-");
                        Console.Write("Write Here, press enter for a new line: ");
                        string line;
                        List<char> chars = new List<char>();
                        List<string> encodedChars = new List<string>();
                        do
                        {
                            line = Console.ReadLine();
                            line = line.Replace(" ", null);
                            if (line.Contains("-") && !line.EndsWith("-") && !line.StartsWith("-"))
                            {
                                foreach (string subString in line.Split(','))
                                {
                                    string[] splittedString = subString.Split('-');
                                    if (splittedString.Length == 2 && splittedString[0].Length == 1 && !subString.Contains(',') && !subString.Contains('\'')) //splittedString.Length ottiene il numero di sotto-stringhe in cui è stato divisa la variabile, se questo numero non è 2 vuol dire che è stato messo più di un trattino(tipo a-b-z) perchè con un solo trattino si ottengono solamente 2 sotto-stringhe
                                    {
                                        Debug.WriteLine($"\u2713{subString}");
                                        chars.Add(splittedString[0][0]);
                                        encodedChars.Add(splittedString[1]);
                                    }
                                    else
                                    {
                                        WriteWithColor($"Ignored {subString}, incorrect text formatting", ConsoleColor.Yellow, true);
                                    }
                                }
                                //Create jsonFile call function from here
                            }
                            else
                            {
                                WriteWithColor("Ignored line, incorrect text formatting", ConsoleColor.Yellow, true);
                            }
                        } while (!line.Contains("STOP"));
                        break;
                    case 5:
                        throw new NotImplementedException(); break;
                    default:
                        break;
                }
            } while (result == 0);
#if DEBUG
            Console.WriteLine("\n\n\n[DEBUG] Press a key to close the program.");
            Console.ReadKey();
#endif
        }
    }
}
