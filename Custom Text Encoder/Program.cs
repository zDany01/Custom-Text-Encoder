using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleJSON;
using System.Diagnostics;
using static System.Threading.Thread;

namespace Custom_Text_Encoder
{
    internal class Program
    {
        static string codecPath = "";
        static bool isCodecLoaded;
        static ConsoleColor EDLMED = Convert.ToBoolean(ChooseCodec(true)) ? Console.ForegroundColor : ConsoleColor.DarkGray;


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
            foreach (char Char in Path.GetInvalidFileNameChars())
            {
                text = text.Replace(Char.ToString(), null);
            }
            return text;
        }

        static bool CheckCodec(string filePath)
        {
            EDLMED = Convert.ToBoolean(ChooseCodec(true)) ? Console.ForegroundColor : ConsoleColor.DarkGray;
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

        static void SetupLists(out List<string> key, out List<string> value)
        {
            JSONNode json = JSONNode.Parse(File.ReadAllText(codecPath));
            key = new List<string>();
            value = new List<string>();
            foreach(KeyValuePair<string, JSONNode> keyValuePair in json)
            {
                key.Add(keyValuePair.Key);
                value.Add(keyValuePair.Value);
            }
        }

        static void CreateJSONCodec(string filePath, List<string> chars, List<string> encodedChars)
        {
            JSONNode file = new JSONObject();
            for (int i = 0; i < chars.Count; i++)
            {
                file.Add(chars[i], encodedChars[i]);
            }
            File.WriteAllText(filePath, file.AsObject.ToString(0)); //Questo ToString() non è quello di Microsoft, è una funzione stessa della libreria SimpleJSON, il numero indica se usare la formattazione(mantenere gli spazi)

        }

        static bool ExportCodec(string codec)
        {
            string reply, newExportedFilePath, exportedMessage = newExportedFilePath = string.Empty;
            do
            {
                Console.Clear();
                Console.WriteLine("Where do you want to export this file?\n1. Documents folder\n2. Desktop\n3. User folder\n4. Custom path\n");
                Console.Write("Choose or type ");
                WriteWithColor("ABORT", ConsoleColor.DarkRed);
                Console.Write(": ");
                reply = Console.ReadLine();
                switch (reply)
                {
                    case "ABORT": return false;
                    case "1":
                        newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        exportedMessage = "Codec successfully exported in Documents";
                        break;
                    case "2":
                        newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        exportedMessage = "Codec successfully exported on Desktop";
                        break;
                    case "3":
                        newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        exportedMessage = "Codec successfully exported in User folder";
                        break;
                    case "4":
                        Console.Write("Type here the custom directory path: ");
                        newExportedFilePath = Console.ReadLine().Replace('/', '\\');
                        if (!Directory.Exists(newExportedFilePath)) Directory.CreateDirectory(newExportedFilePath);
                        if (newExportedFilePath.EndsWith("\\")) newExportedFilePath = newExportedFilePath.Remove(newExportedFilePath.Length - 1, 1);
                        exportedMessage = $"Codec successfully exported in {newExportedFilePath.Remove(0, newExportedFilePath.LastIndexOf("\\") + 1)}";
                        break;
                    default: reply = "Why do you want to restart this?"; break;
                }
            } while (reply == "Why do you want to restart this?");

            newExportedFilePath +=  "\\" + codec.Replace(Environment.CurrentDirectory, null);
            if (File.Exists(newExportedFilePath))
            {
                Console.Write("Seems like you've already exported this. Do you want to overwrite the file? (Y/n): ");
                if (Console.ReadLine().ToUpper() == "N") return false;
            }
            try
            {
                File.Copy(codec, newExportedFilePath, true);
            }
            catch (UnauthorizedAccessException)
            {
                Console.Clear();
                WriteWithColor("I can't save that file there, try restart me as an administrator", ConsoleColor.DarkRed, true);
                Sleep(2000);
                return false;
            }

            Console.Clear();
            WriteWithColor(exportedMessage, ConsoleColor.Green, true);
            Sleep(1000);
            return true;
        }

        static bool ExportFile(string fileContents)
        {
            string reply, newExportedFilePath, exportedMessage = newExportedFilePath = string.Empty;
            do
            {
                Console.Clear();
                Console.WriteLine("Where do you want to export this file?\n1. Documents folder\n2. Desktop\n3. User folder\n4. Custom path\n");
                Console.Write("Choose or type ");
                WriteWithColor("ABORT", ConsoleColor.DarkRed);
                Console.Write(": ");
                reply = Console.ReadLine();
                switch (reply)
                {
                    case "ABORT": return false;
                    case "1":
                        newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        exportedMessage = "File successfully exported in Documents";
                        break;
                    case "2":
                        newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        exportedMessage = "File successfully exported on Desktop";
                        break;
                    case "3":
                        newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        exportedMessage = "File successfully exported in User folder";
                        break;
                    case "4":
                        Console.Write("Type here the custom directory path: ");
                        newExportedFilePath = Console.ReadLine().Replace('/', '\\');
                        if (!Directory.Exists(newExportedFilePath)) Directory.CreateDirectory(newExportedFilePath);
                        if (newExportedFilePath.EndsWith("\\")) newExportedFilePath = newExportedFilePath.Remove(newExportedFilePath.Length - 1, 1);
                        exportedMessage = $"File successfully exported in {newExportedFilePath.Remove(0, newExportedFilePath.LastIndexOf("\\") + 1)}";
                        break;
                    default: reply = "Why do you want to restart this?"; break;
                }
            } while (reply == "Why do you want to restart this?");
            Console.Write("Insert filename: "); 
            newExportedFilePath += "\\" + RemoveIllegalChar(Console.ReadLine() + ".txt");
            if (File.Exists(newExportedFilePath))
            {
                Console.Write("Seems like you've already exported this. Do you want to overwrite the file? (Y/n): ");
                if (Console.ReadLine().ToUpper() == "N") return false;
            }
            try
            {
                File.WriteAllText(newExportedFilePath, fileContents);
            }
            catch (UnauthorizedAccessException)
            {
                Console.Clear();
                WriteWithColor("I can't save that file there, try restart me as an administrator", ConsoleColor.DarkRed, true);
                Sleep(2000);
                return false;
            }

            Console.Clear();
            WriteWithColor(exportedMessage, ConsoleColor.Green, true);
            Sleep(1000);
            return true;
        }

        static string ChooseCodec(bool onlyCheck = false)
        {
            List<string> codecs = new List<string>();
            foreach (string filePath in Directory.GetFiles(Environment.CurrentDirectory))
            {
                string fileName = filePath.Replace(Environment.CurrentDirectory, null).Remove(0, 1);
                if (fileName.EndsWith(".json"))
                {
                    if (IsValidCodec(fileName)) { codecs.Add(fileName); }
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
                    if (reply == "ABORT" || reply == string.Empty) goto Exit;
                    int.TryParse(reply, out codecNumber);
                } while (codecNumber <= 0 || codecNumber > codecs.Count);
                return codecs[codecNumber - 1];
            }
            if (onlyCheck) return bool.FalseString;
Exit:
            return string.Empty;
        }

        static bool IsValidCodec(string filePath, bool writeInvalid = false)
        {
            if (!File.Exists(filePath))
            {
                if (writeInvalid) WriteWithColor("The specified file doesn't exists.\n", ConsoleColor.DarkYellow, true);
                return false;
            }

            string fileContents = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(fileContents) || fileContents.Count(_ => (_ == '{')) > 1)
            {
                if (writeInvalid) WriteWithColor($"This isn't a valid codec file.\n", ConsoleColor.DarkYellow, true);
                return false;
            }

            JSONNode jsonFile = JSONNode.Parse(fileContents);
            int jsonFileLine = 0;
            foreach (KeyValuePair<string, JSONNode> keyValuePair in jsonFile)
            {
                if (keyValuePair.Key.Length != 1)
                {
#if DEBUG
                    Debug.WriteLine($"\u2717{keyValuePair.Key} - line {jsonFileLine}");
#endif
                    if (writeInvalid) WriteWithColor($"This isn't a valid codec file.\n", ConsoleColor.DarkYellow, true);
                    return false;
                }
                jsonFileLine++;
            }
#if DEBUG
            Debug.WriteLine($"\u2713{Path.GetFileNameWithoutExtension(filePath)}");
#endif
            return true;
        }

        static void Main()
        {
            int result;
            do
            {
                Console.Clear();
                isCodecLoaded = CheckCodec(codecPath);
                Console.WriteLine($"Select Mode");
                WriteWithColor("1. Encode\n2. Decode\n3. Load Encoding Format", EDLMED, true);
                Console.WriteLine("4. Create Encoding Format");
                WriteWithColor("5. Modify Encoding Format\n6. Export Codec", EDLMED, true);
                Console.WriteLine("7. Import Codec");
                WriteWithColor("8. Delete Codec", EDLMED, true);
                Console.Write("\nChoose an option: ");
                int.TryParse(Console.ReadLine(), out result);
                Console.Clear();
                switch (result)
                {
                    #region "Encode"
                    case 1:
                        if (EDLMED != ConsoleColor.DarkGray)
                        {
                            if (isCodecLoaded)
                            {
                                /*JSONNode codec = JSONNode.Parse(File.ReadAllText(codecPath));
                                List<string> normalChar = new List<string>();
                                List<string> encodedValue = new List<string>();
                                foreach (KeyValuePair<string, JSONNode> keyValuePairs in codec)
                                {
                                    normalChar.Add(keyValuePairs.Key);
                                    encodedValue.Add(keyValuePairs.Value);
                                }*/ //Backup
                                SetupLists(out List<string> normalChar, out List<string> encodedValue);
                                Console.Write("Type here the text that you want to encode, the program will accept user input until you write ");
                                WriteWithColor("ENCODE\n", ConsoleColor.Green, true);
                                string textToEncode = string.Empty;
                                string encoderLine;
                                do
                                {
                                    encoderLine = Console.ReadLine();
                                    textToEncode += encoderLine + Environment.NewLine;
                                } while (encoderLine != "ENCODE");
                                textToEncode = textToEncode.Remove(textToEncode.Length - 8, 8); //remove ENCODE and NewLine from string
                                if (textToEncode.Length != 0)
                                {

                                    List<string> stringChars = new List<string>();
                                    foreach (char c in textToEncode)
                                    {
                                        stringChars.Add(c.ToString());
                                    }

                                    for (int i = 0; i < stringChars.Count; i++)
                                    {
                                        int charIndex = normalChar.IndexOf(stringChars[i]);
                                        if (charIndex != -1)
                                        {
                                            stringChars[i] = encodedValue[charIndex];
                                        }
                                    }
                                    string encodedText = string.Empty;
                                    foreach (string stringChar in stringChars)
                                    {
                                        encodedText += stringChar;
                                    }
                                    Console.Clear();
                                    Console.WriteLine($"{textToEncode}has been encoded to:\n{encodedText}\n");
                                    Console.Write("Do you want to export it? (y/N): ");
                                    if (Console.ReadLine().ToUpper() == "Y")
                                    {
                                        if (!ExportFile(encodedText)) goto default;
                                        /*File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\exported.txt", encodedText); //TODO: It's going to change when starting optimization
                                        WriteWithColor("File successfully exported on Desktop", ConsoleColor.Green, true);
                                        Sleep(1500);*/
                                    }
                                }
                                else
                                {
                                    WriteWithColor("You cannot encode the \"nothing\"", ConsoleColor.DarkYellow, true);
                                    Sleep(1500);
                                }
                            }
                            else
                            {
                                WriteWithColor("Select a codec first!", ConsoleColor.Yellow, true);
                                Sleep(1500);
                            }
                        }
                        result = 0;
                        break;
                    #endregion
                    #region "Decode"
                    case 2:
                        if (EDLMED != ConsoleColor.DarkGray)
                        {
                            if (isCodecLoaded)
                            {
                                /*JSONNode codec = JSONNode.Parse(File.ReadAllText(codecPath));
                                List<string> normalChar = new List<string>();
                                List<string> encodedValue = new List<string>();
                                foreach(KeyValuePair<string, JSONNode> keyValuePair in codec)
                                {
                                    normalChar.Add(keyValuePair.Key);
                                    encodedValue.Add(keyValuePair.Value);
                                }*/ //Backup
                                SetupLists(out List<string> normalChar, out List<string> encodedValue);
                                Console.Write("Type here the text that you want to decode, the program will accept user input until you write ");
                                WriteWithColor("DECODE\n", ConsoleColor.Green, true);
                                string textToDecode = string.Empty;
                                string decoderLine;
                                do
                                {
                                    decoderLine = Console.ReadLine();
                                    textToDecode += decoderLine + Environment.NewLine;
                                } while (decoderLine != "DECODE");

                                string decodedText = textToDecode = textToDecode.Remove(textToDecode.Length - 8, 8); //remove DECODE and NewLine from string
                                if (textToDecode.Length != 0)
                                {
                                    for (int i = 0; i < encodedValue.Count; i++)
                                    {
                                        if (textToDecode.Contains(encodedValue[i]))
                                        {
                                            decodedText = decodedText.Replace(encodedValue[i], normalChar[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    WriteWithColor("You cannot encode the \"nothing\"", ConsoleColor.DarkYellow, true);
                                    Sleep(1500);
                                }
                                Console.Clear();
                                Console.WriteLine($"{textToDecode}has been encoded to:\n{decodedText}\n");
                                Console.Write("Do you want to export it? (y/N): ");
                                if (Console.ReadLine().ToUpper() == "Y")
                                {
                                    if (!ExportFile(decodedText)) goto default;
                                    /*File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\exported.txt", DecodedText); //TODO: It's going to change when starting optimization
                                    WriteWithColor("File successfully exported on Desktop", ConsoleColor.Green, true);
                                    Sleep(1500);*/
                                }
                            }
                            else
                            {
                                WriteWithColor("Select a codec first!", ConsoleColor.Yellow, true);
                                Sleep(1500);
                            }
                        }
                        result = 0;
                        break;
                    #endregion
                    #region "Load Codec"
                    case 3:
                        if (EDLMED != ConsoleColor.DarkGray) codecPath = ChooseCodec();
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
                        WriteWithColor("ABORT", ConsoleColor.Red);
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
                            if (line.Contains("ABORT") || line.Contains("SAVE")) { break; }
                            line = line.Replace(" ", null);
                            if (line.Contains("-") && !line.EndsWith("-") && !line.StartsWith("-"))
                            {
                                foreach (string subString in line.Split(','))
                                {
                                    string[] splittedString = subString.Split('-');
                                    if (splittedString[0] == splittedString[1]) { WriteWithColor($"The char({splittedString[0]}) and its encoded version({splittedString[1]}) are the same, skipping...", ConsoleColor.DarkYellow, true); continue; }
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
                                            Debug.WriteLine($"\u2795{subString}");
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
                                WriteWithColor(chars[i], ConsoleColor.Blue);
                                Console.Write(" => ");
                                WriteWithColor(encodedChars[i], ConsoleColor.Cyan, true);
                            }
                            Console.Write("Insert filename: ");
                            string filePath = RemoveIllegalChar(Console.ReadLine());

                            if (!filePath.EndsWith(".json")) filePath += ".json";
                            if (File.Exists(filePath))
                            {
                                Console.Write($"File {filePath.Replace(Environment.CurrentDirectory, null).Replace(".json", null)} already exists. Do you want to overwrite it? (Y/n): ");
                                if (Console.ReadLine().ToUpper() == "N") goto default;
                            }
                            CreateJSONCodec(filePath, chars, encodedChars);
                            Console.Write("\nFile saved successfully, do you want to load it? (y/N): ");
                            if (Console.ReadLine().ToUpper() == "Y") codecPath = filePath;
                        }
                        result = 0;
                        break;
                    #endregion
                    #region "Modify Codec"
                    case 5:
                        if (EDLMED != ConsoleColor.DarkGray)
                        {
                            if (isCodecLoaded)
                            {
                                string modifyLine;
                                /*List<string> codecChars = new List<string>();
                                List<string> modifiedChars = new List<string>();
                                JSONNode jsonCodec = JSONNode.Parse(File.ReadAllText(codecPath));
                                foreach (KeyValuePair<string, JSONNode> keyValuePairs in jsonCodec)
                                {
                                    codecChars.Add(keyValuePairs.Key);
                                    modifiedChars.Add(keyValuePairs.Value);
                                } */ //Backup
                                SetupLists(out List<string> codecChars, out List<string> modifiedChars);
                                bool restartModify;
                                do
                                {
                                    restartModify = false;
                                    Console.Clear();
                                    Console.Write("Hi, to modify an encoding format you need to follow these rules:\nThe text format must be the following: ");
                                    WriteWithColor('X', ConsoleColor.Yellow);
                                    Console.Write('-');
                                    WriteWithColor("YYY", ConsoleColor.DarkGreen);
                                    Console.Write(" where\n - ");
                                    WriteWithColor('X', ConsoleColor.Yellow);
                                    Console.Write(" is the character that you want to modify or add\n - ");
                                    WriteWithColor('Y', ConsoleColor.Green);
                                    Console.Write(" is the encoded or modified version of that character\nFor example if i want to modify all \"z\" to become \"A\", I'll write z-A\nFor deleting an existing character, you will need to type ");
                                    WriteWithColor("DELETE ", ConsoleColor.DarkRed);
                                    WriteWithColor('X', ConsoleColor.Yellow);
                                    Console.Write(" where ");
                                    WriteWithColor('X', ConsoleColor.Yellow);
                                    Console.Write(" is the character\nTo see which characters are already present type ");
                                    WriteWithColor("LIST", ConsoleColor.Magenta, true);
                                    Console.Write("The program will accept input until you write ");
                                    WriteWithColor("ABORT", ConsoleColor.Red);
                                    Console.Write(" to abort or ");
                                    WriteWithColor("SAVE", ConsoleColor.Green);
                                    Console.WriteLine(" to save");
                                    Console.Write("You can write multiple entries in two ways:\n 1. Separated by a comma(Ex: z-A, Q-L, x-3, 2-z)\n 2. By writing one at time, Ex:\nz-A\nQ-L\nx-3\n2-z");
                                    Console.WriteLine("\nIllegal characters are \',-");
                                    Console.Write("Write Here, press enter for a new line: ");
                                    while (true)
                                    {
                                        modifyLine = Console.ReadLine();
                                        if (modifyLine.Contains("ABORT") || modifyLine.Contains("SAVE")) break;
                                        else if (modifyLine == "LIST")
                                        {
                                            for (int i = 0; i < codecChars.Count; i++)
                                            {
                                                WriteWithColor(codecChars[i], ConsoleColor.Blue);
                                                Console.Write(" => ");
                                                WriteWithColor(modifiedChars[i], ConsoleColor.Cyan, true);
                                            }
                                            continue;
                                        }
                                        modifyLine = modifyLine.Replace(" ", null);
                                        if (modifyLine.Contains("DELETE"))
                                        {
                                            modifyLine = modifyLine.Replace("DELETE", null);
                                            if (modifyLine.Length == 1)
                                            {
                                                int charPosition = codecChars.IndexOf(modifyLine);
                                                if (charPosition != -1)
                                                {
#if DEBUG
                                                    Debug.WriteLine($"\u2326{codecChars.ElementAt(charPosition)}");
#endif
                                                    codecChars.RemoveAt(charPosition);
                                                    modifiedChars.RemoveAt(charPosition);
                                                }
                                                else
                                                {
                                                    WriteWithColor($"Can't delete {modifyLine} from the list because it isn't present", ConsoleColor.DarkYellow, true);
                                                }
                                            }
                                            else
                                            {
                                                WriteWithColor("Ignored line, incorrect text formatting", ConsoleColor.DarkYellow, true);
                                            }
                                            continue;
                                        }
                                        else
                                        {

                                            if (modifyLine.Contains("-") && !modifyLine.EndsWith("-") && !modifyLine.StartsWith("-"))
                                            {
                                                foreach (string subString in modifyLine.Split(','))
                                                {
                                                    string[] splittedString = subString.Split('-');
                                                    if (splittedString[0] == splittedString[1]) { WriteWithColor($"The char({splittedString[0]}) and its encoded version({splittedString[1]}) are the same, skipping...", ConsoleColor.DarkYellow, true); continue; }
                                                    if (splittedString.Length == 2 && splittedString[0].Length == 1 && !subString.Contains(',') && !subString.Contains('\'')) //splittedString.Length ottiene il numero di sotto-stringhe in cui è stato divisa la variabile, se questo numero non è 2 vuol dire che è stato messo più di un trattino(tipo a-b-z) perché con un solo trattino si ottengono solamente 2 sotto-stringhe
                                                    {
                                                        int charPosition = codecChars.IndexOf(splittedString[0]);
                                                        if (charPosition != -1) //check if char is already in the list
                                                        {
#if DEBUG
                                                            Debug.WriteLine($"\u26A0{splittedString[0]}({modifiedChars[charPosition]} => {splittedString[1]})");
#endif
                                                            WriteWithColor($"{splittedString[0]} has been modified ({modifiedChars[charPosition]} => {splittedString[1]})", ConsoleColor.Cyan, true);
                                                            modifiedChars[charPosition] = splittedString[1];
                                                        }
                                                        else
                                                        {
                                                            codecChars.Add(splittedString[0]);
                                                            modifiedChars.Add(splittedString[1]);
#if DEBUG
                                                            Debug.WriteLine($"\u2795{subString}");
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
                                    }


                                    if (modifyLine.Contains("SAVE") && codecChars.Count > 0)
                                    {
                                        bool redo;
                                        do
                                        {
                                            redo = false;
                                            Console.Clear();
                                            Console.WriteLine("Codec overview: ");
                                            for (int i = 0; i < codecChars.Count; i++)
                                            {
                                                WriteWithColor(codecChars[i], ConsoleColor.Blue);
                                                Console.Write(" => ");
                                                WriteWithColor(modifiedChars[i], ConsoleColor.Cyan, true);
                                            }
                                            Console.Write("What do you want to do?:\n1. Save\n2. Continue Modify \n3. Abort\nChoose: ");
                                            int.TryParse(Console.ReadLine(), out int reply);
                                            switch (reply)
                                            {
                                                case 1:
                                                    CreateJSONCodec(codecPath, codecChars, modifiedChars);
                                                    Console.Write("\nFile modified successfully, do you want to load it? (Y/n): ");
                                                    if (Console.ReadLine().ToUpper() == "N") codecPath = "";
                                                    break;
                                                case 2: restartModify = true; break;
                                                case 3: goto default;
                                                default: redo = true; break;
                                            }
                                        } while (redo);
                                    }
                                } while (restartModify);
                            }
                            else
                            {
                                WriteWithColor("Select a codec first!", ConsoleColor.Yellow, true);
                                Sleep(1500);
                            }

                        }
                        result = 0;
                        break;
                    #endregion
                    #region "Export Codec"
                    case 6:
                        if (EDLMED != ConsoleColor.DarkGray)
                        {
                            string codec_6 = ChooseCodec();
                            if (codec_6 != string.Empty)
                            {
                                if (!ExportCodec(codec_6)) { goto default; }
                                /*string reply_6;
                                string newExportedFilePath = "PGRARPT";
                                string exportedMessage = string.Empty;
                                do
                                {
                                    Console.Clear();
                                    Console.WriteLine("Where do you want to export this file?\n1. Documents folder\n2. Desktop\n3. User folder\n4. Custom path\n");
                                    Console.Write("Choose or type ");
                                    WriteWithColor("ABORT", ConsoleColor.DarkRed);
                                    Console.Write(": ");
                                    reply_6 = Console.ReadLine();
                                    switch (reply_6)
                                    {
                                        case "ABORT": goto default;
                                        case "1":
                                            newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                            exportedMessage = "Codec successfully exported in Documents";
                                            break;
                                        case "2":
                                            newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                                            exportedMessage = "Codec successfully exported on Desktop";
                                            break;
                                        case "3":
                                            newExportedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                                            exportedMessage = "Codec successfully exported in User folder";
                                            break;
                                        case "4":
                                            Console.Write("Type here the custom directory path: ");
                                            newExportedFilePath = Console.ReadLine().Replace('/', '\\');
                                            if (!Directory.Exists(newExportedFilePath)) Directory.CreateDirectory(newExportedFilePath);
                                            if (newExportedFilePath.EndsWith("\\")) newExportedFilePath = newExportedFilePath.Remove(newExportedFilePath.Length - 1, 1);
                                            exportedMessage = $"Codec successfully exported in {newExportedFilePath.Remove(0, newExportedFilePath.LastIndexOf("\\") + 1)}";
                                            break;
                                        default: reply_6 = "PGRARPT"; break;
                                    }
                                } while (reply_6 == "PGRARPT");

                                newExportedFilePath += "\\" + codec_6.Replace(Environment.CurrentDirectory, null);
                                if (File.Exists(newExportedFilePath))
                                {
                                    Console.Write("Seems like you've already exported this. Do you want to overwrite the file? (Y/n): ");
                                    if (Console.ReadLine().ToUpper() == "N") goto default;
                                }
                                try
                                {
                                    File.Copy(codec_6, newExportedFilePath, true);
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    Console.Clear();
                                    WriteWithColor("I can't save that file there, try restart me as an administrator", ConsoleColor.DarkRed, true);
                                    Sleep(2000);
                                    goto default;
                                }

                                Console.Clear();
                                WriteWithColor(exportedMessage, ConsoleColor.Green, true);
                                Sleep(1000);*/
                            }
                        }
                        result = 0;
                        break;
                    #endregion
                    #region "Import Codec"
                    case 7:
                        bool isValid = false;
                        string codecName = "If you see me, there is a bug the program :/\n If you are an user, please open a report on GitHub https://github.com/zDany01/Custom-Text-Encoder/issues"; //:)
                        string importFilePath;
                        do
                        {
                            Console.WriteLine("To import a file:\n- Write the file path\n- Drag the file in the console window\n");
                            Console.Write("Write file path here or type ");
                            WriteWithColor("ABORT", ConsoleColor.DarkRed);
                            Console.Write(": ");
                            importFilePath = Console.ReadLine().Replace("\"", null);
                            if (importFilePath == string.Empty || importFilePath == "ABORT") goto default;
                            Console.Clear();
                            if (importFilePath.Contains("/") || importFilePath.Contains("\\")) isValid = IsValidCodec(importFilePath, true);
                            else WriteWithColor("Insert a valid file path\n", ConsoleColor.DarkYellow, true);
                            if (isValid) codecName = Path.GetFileNameWithoutExtension(importFilePath);
                        } while (!isValid);
                        Console.Clear();
                        string newFilePath = $"{Environment.CurrentDirectory}\\{codecName}.json";
                        if (File.Exists(newFilePath))
                        {
                            Console.Write($"The codec ");
                            WriteWithColor(codecName, ConsoleColor.DarkCyan);
                            Console.Write(" already exists, do you want to ");
                            WriteWithColor("overwrite", ConsoleColor.Red);
                            Console.Write(" it? (Y/n): ");
                            if (Console.ReadLine().ToUpper() == "N") goto default;
                        }
                        File.Copy(importFilePath, newFilePath, true);
                        WriteWithColor($"\nSuccessfully imported {codecName}", ConsoleColor.Green, true);
                        Console.Write("Do you want to load it? (y/N): ");
                        if (Console.ReadLine().ToUpper() == "Y") codecPath = codecName + ".json";
                        result = 0;
                        break;
                    #endregion
                    #region "Delete Codec"
                    case 8:
                        if (EDLMED != ConsoleColor.DarkGray)
                        {
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
                        }
                        result = 0;
                        break;
                    #endregion:
                    default: result = 0; break;
                }
            } while (result == 0);
        }
    }
}
