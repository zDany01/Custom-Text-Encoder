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
        static bool CheckCodec(string filePath)
        {
            Console.Write("Codec status: ");
            if (File.Exists(filePath))
            {
                string fileContents = File.ReadAllText(filePath);
                if (fileContents.StartsWith("{") && fileContents.EndsWith("}"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Loaded");
                    Console.ResetColor();
                    return true;
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Not Loaded");
            Console.ResetColor();
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

        static void Main()
        {
            int result = 0;
            do {
                Console.Clear();
                string codecPath = "";
                bool isCodecLoaded = CheckCodec(codecPath);
                //string codecStatus = File.Exists("codec.json") ? "Loaded" : "Not Loaded";
                Console.Write($"Select Mode:\n1. Encode\n2. Decode\n3. Load Encoding Format\n4. Create Encoding Format\n5. Modify Encoding Format\nChoose an option: ");
                int.TryParse(Console.ReadLine(), out result);
                Debug.WriteLine(result);
                switch (result)
                {
                    case -1:
                        Console.ReadLine();
                        break;
                    case 1:
                        Console.Clear();
                        if (!isCodecLoaded)
                        {
                            Console.Write("Choose an encoding format before!\n\nType 0 to create a new encoding format or press enter to restart the program: ");
                        }
                        break;
                    case 2:
                    case 3:
                    case 4:
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
