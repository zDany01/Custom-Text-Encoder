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
        static void Main()
        {
            string codecStatus = File.Exists("codec.json") ? "Loaded" : "Not Loaded";
            Console.Write($"Select Mode:\n1. Encode.\n2. Decode.\n3. Load Encoding Codec(Current status: {codecStatus}).\n4. Create Encoding Codec.\n5. Modify Encoding Codec.\nChoose an option: ");
            switch (Convert.ToInt32(Console.ReadLine()))
            {
                case 0: //debug
                    char[] normalChar = {}; string[] encodedChar = {};
                    SetupArray(JSONNode.Parse(File.ReadAllText("codec.json")), normalChar, encodedChar);
                    Console.ReadLine();
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    throw new NotImplementedException();
                    break;
            }
        }

        static string Chiper(string text, char[] normalChar, string[] encodedChar)
        {
            for (int i = 0; i < normalChar.Length; i++)
            {
                text = text.Replace(normalChar[i].ToString(), encodedChar[i]);
            }
            return text;
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
            normalChar = keys.ToArray();
            encodedChar = values.ToArray();
        }
    }
}
