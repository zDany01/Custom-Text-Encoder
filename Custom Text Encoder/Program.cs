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
        static string codecPath = "";
        static bool isCodecLoaded;
        static ConsoleColor LMED = Convert.ToBoolean(ChooseCodec(true)) ? Console.ForegroundColor : ConsoleColor.DarkGray;
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

        static string RemoveIllegalChar(string text)
        {
            foreach (char c in "\\/:*\"?<>|") { text = text.Replace(c.ToString(), null); }
            return text;
        }

        static bool CheckCodec(string filePath)
        {
            LMED = Convert.ToBoolean(ChooseCodec(true)) ? Console.ForegroundColor : ConsoleColor.DarkGray;
            Console.Write("Codec status: ");
            if (File.Exists(filePath))
            {
                WriteWithColor("Loaded", ConsoleColor.Green);
                Console.Write("(");
                WriteWithColor(filePath.Remove(filePath.Length - 5, 5), ConsoleColor.DarkCyan);
                Console.WriteLine(")\n");
                return true;
            }
            WriteWithColor("Not Loaded\n", ConsoleColor.Red, true);
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

        static void CreateJSONCodec(string filePath, List<string> chars, List<string> encodedChars)
        {
            JSONNode file = new JSONObject();
            for (int i = 0; i < chars.Count; i++)
            {
                file.Add(chars[i], encodedChars[i]);
            }
            File.WriteAllText(filePath, file.AsObject.ToString(0)); //Questo ToString() non è quello di Microsoft, è una funzione stessa della libreria EasyJSON, il numero indica se usare la formattazione(mantenere gli spazi)

        }

        static string ChooseCodec(bool onlyCheck = false)
        {
            List<string> codecs = new List<string>();
            foreach (string filePath in Directory.GetFiles(Environment.CurrentDirectory))
            {
                string fileName = filePath.Replace(Environment.CurrentDirectory, null).Remove(0, 1);
                if (fileName.EndsWith(".json"))
                {
                    string fileContents = File.ReadAllText(filePath);
                    if (fileContents.StartsWith("{") && fileContents.EndsWith("}"))
                    {
                        codecs.Add(fileName);
                    }
                }
            }

            if (codecs.Count > 0)
            {
                if (onlyCheck) { return bool.TrueString; }
                int codecNumber;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Select a codec: ");
                    for (int i = 1; i <= codecs.Count; i++)
                    {
                        Console.WriteLine($"{i}. {codecs[i - 1].Remove(codecs[i - 1].Length - 5, 5)}"); //remove .json extension
                    }
                    Console.Write("Choose or type ");
                    WriteWithColor("ABORT", ConsoleColor.DarkRed);
                    Console.Write(": ");
                    string reply = Console.ReadLine();
                    if (reply == "ABORT") goto Exit;
                    int.TryParse(reply, out codecNumber);
                } while (codecNumber <= 0 || codecNumber > codecs.Count);
                return codecs[codecNumber - 1];
            }
            if (onlyCheck) return bool.FalseString;
Exit:
            return string.Empty;
        }
        static void Main()
        {
            int result;
            do
            {
                Console.Clear();
                isCodecLoaded = CheckCodec(codecPath);
                Console.WriteLine($"Select Mode\n1. Encode\n2. Decode");
                WriteWithColor("3. Load Encoding Format", LMED, true);
                Console.WriteLine("4. Create Encoding Format");
                WriteWithColor("5. Modify Encoding Format\n6. Export Codec", LMED, true);
                Console.WriteLine("7. Import Codec");
                WriteWithColor("8. Delete Codec", LMED, true);
                Console.Write("\nChoose an option: ");
                int.TryParse(Console.ReadLine(), out result);
                Console.Clear();
                switch (result)
                {
                    #region "TODO Encode/Decode"
                    case 1:
                    case 2:
                        if (!isCodecLoaded)
                        {
                            Console.Write("Choose an encoding format before!\n\nType 0 to create a new encoding format, Type 1 to load an existing codec or press enter to restart the program: ");
                            string reply = Console.ReadLine();
                            if (reply == "0") { goto case 4; } else if (reply == "1") { goto case 3; } else result = 0;
                        }
                        throw new NotImplementedException();
                    #endregion
                    #region "Load Codec"
                    case 3:
                        codecPath = ChooseCodec();
                        result = 0;
                        break;
                    #endregion
                    #region "Create Codec"
                    case 4:
                        Console.Clear();
                        Console.Write("Hi, to create a new encoding format you need to follow these rules:\nThe text format must be the following: ");
                        WriteWithColor('X', ConsoleColor.Yellow);
                        Console.Write('-');
                        WriteWithColor("YYY", ConsoleColor.DarkGreen);
                        Console.Write(" where\n - ");
                        WriteWithColor('X', ConsoleColor.Yellow);
                        Console.Write(" is the character that you want to encode\n - ");
                        WriteWithColor('Y', ConsoleColor.Green);
                        Console.Write(" is the encoded form of that character\nFor example if i want to encode all \"A\" to become \"z\", I'll write A-z\nThe program will accept input until you write ");
                        WriteWithColor("STOP", ConsoleColor.Red);
                        Console.Write(" to abort or ");
                        WriteWithColor("SAVE", ConsoleColor.Green);
                        Console.WriteLine(" to save");
                        Console.WriteLine("You can write multiple entries in two ways:\n 1. Separated by a comma(Ex: A-z, L-Q, 0-ADA, 3-0x03)\n 2. By writing one at time, Ex:\nA-z\nL-Q\n0-ADA\n3-0x03\n\nIllegal characters are \',-");
                        Console.Write("Write Here, press enter for a new line: ");
                        string line;
                        List<string> chars = new List<string>();
                        List<string> encodedChars = new List<string>();
                        while (true)
                        {
                            line = Console.ReadLine();
                            if (line.Contains("STOP") || line.Contains("SAVE")) { break; }
                            line = line.Replace(" ", null);
                            if (line.Contains("-") && !line.EndsWith("-") && !line.StartsWith("-"))
                            {
                                foreach (string subString in line.Split(','))
                                {
                                    string[] splittedString = subString.Split('-');
                                    if (splittedString[0] == splittedString[1]) { WriteWithColor("The char and its encoded version are the same, skipping...", ConsoleColor.Yellow, true); continue; }
                                    if (splittedString.Length == 2 && splittedString[0].Length == 1 && !subString.Contains(',') && !subString.Contains('\'')) //splittedString.Length ottiene il numero di sotto-stringhe in cui è stato divisa la variabile, se questo numero non è 2 vuol dire che è stato messo più di un trattino(tipo a-b-z) perchè con un solo trattino si ottengono solamente 2 sotto-stringhe
                                    {
                                        int charPosition = chars.IndexOf(splittedString[0]);
                                        if (charPosition != -1) //check if char is already in the list
                                        {
                                            WriteWithColor($"\"{splittedString[0]}\" already exists, it will be overwritten", ConsoleColor.Yellow, true);
#if DEBUG
                                            Debug.WriteLine($"\u26A0{splittedString[0]}({encodedChars[charPosition]} => {splittedString[1]})");
#endif
                                            encodedChars[charPosition] = splittedString[1];
                                        }
                                        else
                                        {
                                            chars.Add(splittedString[0]);
                                            encodedChars.Add(splittedString[1]);
#if DEBUG
                                            Debug.WriteLine($"\u2713{subString}");
#endif
                                        }
                                    }
                                    else
                                    {
                                        WriteWithColor($"Ignored {subString}, incorrect text formatting", ConsoleColor.DarkYellow, true);
                                    }
                                }
                            }
                            else
                            {
                                WriteWithColor("Ignored line, incorrect text formatting", ConsoleColor.DarkYellow, true);
                            }
                        }
                        if (line.Contains("SAVE") && chars.Count > 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Codec overview: ");
                            for (int i = 0; i < chars.Count; i++)
                            {
                                Console.WriteLine($"{chars[i]} => {encodedChars[i]}");
                            }
                            Console.Write("Insert filename: ");
                            string filePath = RemoveIllegalChar(Console.ReadLine());

                            if (!filePath.EndsWith(".json")) filePath += ".json";
                            if (File.Exists(filePath))
                            {
                                Console.Write($"File {filePath.Replace(Environment.CurrentDirectory, null).Replace(".json", null)} already exists. Do you want to overwrite it? (Y/N): ");
                                if (Console.ReadLine().ToUpper() == "N") goto skipFile;
                            }
                            CreateJSONCodec(filePath, chars, encodedChars);
                            Console.Write("\nFile saved successfully, do you want to load it? (Y/N): ");
                            if (Console.ReadLine().ToUpper() == "Y") codecPath = filePath;
                        }
skipFile:
                        result = 0;
                        break;
                    #endregion
                    #region "Export Codec"
                    case 6:
                        string codec_6 = ChooseCodec();
                        if (codec_6 != string.Empty)
                        {
                            string newFilePath = Environment.GetEnvironmentVariable("userprofile") + "\\Desktop\\" + codec_6.Replace(Environment.CurrentDirectory, null);
                            if (File.Exists(newFilePath))
                            {
                                Console.Write("Seems like you've already exported this. Do you want to overwrite the file? (Y/n): ");
                                if(Console.ReadLine().ToUpper() == "N") goto skipFile;
                            }
                            File.Copy(codec_6, newFilePath, true);
                            Console.Clear();
                            Console.WriteLine("Codec exported on Desktop.");
                            System.Threading.Thread.Sleep(1000);
                        }
                        result = 0;
                        break;
                    #endregion
                    #region "Delete Codec"
                    case 8:
                        for (int i = 0; i < Console.WindowWidth / 2 - 4; i++)
                        {
                            Console.Write(' ');
                        }
                        WriteWithColor("WARNING!", ConsoleColor.DarkRed, true);
                        WriteWithColor(string.Format($"{{0,{Console.WindowWidth / 2 + 44}}}", "If you choose to delete a Codec, unless you have a backup, you will permanently lost it!"), ConsoleColor.Red, true);
                        WriteWithColor(string.Format($"{{0,{Console.WindowWidth / 2 + 43}}}", "There is no confirmation for deleting files, so choose wisely. if you understood, then"), ConsoleColor.Red, true);
                        for (int i = 0; i < Console.WindowWidth / 2 - 12; i++)
                        {
                            Console.Write(' ');
                        }
                        WriteWithColor("Press a key to continue...", ConsoleColor.DarkMagenta, true);
                        Console.ReadKey();
                        string codec_8 = ChooseCodec();
                        if (codec_8 != string.Empty) File.Delete(codec_8);
                        result = 0;
                        break;
                    #endregion
                    default: result = 0; break;
                }
            } while (result == 0);
#if DEBUG
            Console.WriteLine("\n\n\n[DEBUG] Press a key to close the program.");
            Console.ReadKey();
#endif
        }
    }
}
