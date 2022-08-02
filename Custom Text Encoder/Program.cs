using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Threading.Thread;

namespace Custom_Text_Encoder
{
    internal class Program
    {
        private static string codecPath = "";
        private static bool isCodecLoaded;
        private static ConsoleColor EDLMEDO;
        private static string CodecName => IsValidCodec(codecPath) ? codecPath.Replace(Environment.CurrentDirectory, null).Remove(codecPath.Length - 5, 5) : throw new InvalidOperationException("Unable to get the codec name, check if a codec is loaded before calling the codecName");

        private static void WriteWithColor<T>(T value, ConsoleColor color, bool endLine = false)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (endLine) Console.WriteLine(value);
            else Console.Write(value);
            Console.ForegroundColor = oldColor;
        }

        private static string RemoveIllegalChar(string text)
        {
            foreach (char Char in Path.GetInvalidFileNameChars()) text = text.Replace(Char.ToString(), null);
            return text;
        }

        private static bool CheckCodec(string filePath)
        {
            EDLMEDO = Convert.ToBoolean(ChooseCodec(true)) ? Console.ForegroundColor : ConsoleColor.DarkGray;
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

        private static void SetupLists(out List<string> key, out List<string> value)
        {
            JSONNode json = JSONNode.Parse(File.ReadAllText(codecPath));
            key = new List<string>();
            value = new List<string>();
            foreach (KeyValuePair<string, JSONNode> keyValuePair in json)
            {
                key.Add(keyValuePair.Key);
                value.Add(keyValuePair.Value);
            }
        }

        private static void CreateJSONCodec(string filePath, List<string> chars, List<string> encodedChars)
        {
            JSONNode file = new JSONObject();
            for (int i = 0; i < chars.Count; i++)
            {
                file.Add(chars[i], encodedChars[i]);
            }
            File.WriteAllText(filePath, file.AsObject.ToString(0)); //Questo ToString() non è quello di Microsoft, è una funzione stessa della libreria SimpleJSON, il numero indica se usare la formattazione(mantenere gli spazi)
        }

        private static void ExportCodec(string codec)
        {
            string reply, newExportedFilePath, exportedMessage;
            exportedMessage = newExportedFilePath = string.Empty;
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
                    case "ABORT": return;
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

            newExportedFilePath += "\\" + codec.Replace(Environment.CurrentDirectory, null);
            if (File.Exists(newExportedFilePath))
            {
                Console.Write("Seems like you've already exported this. Do you want to overwrite the file? (Y/n): ");
                if (Console.ReadLine().ToUpper() == "N") return;
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
                return;
            }

            Console.Clear();
            WriteWithColor(exportedMessage, ConsoleColor.Green, true);
            Sleep(1000);
        }

        private static void ExportFile(string fileContents)
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
                    case "ABORT": return;
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
            } while (reply == "Why do you want to restart this?"); // :)
            Console.Write("Insert filename: ");
            newExportedFilePath += "\\" + RemoveIllegalChar(Console.ReadLine());
            if (!newExportedFilePath.EndsWith(".txt")) newExportedFilePath += ".txt";
            if (File.Exists(newExportedFilePath))
            {
                Console.Write("Seems like you've already exported this. Do you want to overwrite the file? (Y/n): ");
                if (Console.ReadLine().ToUpper() == "N") return;
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
                return;
            }

            Console.Clear();
            WriteWithColor(exportedMessage, ConsoleColor.Green, true);
            Sleep(1000);
        }

        private static bool IsValidCodec(string filePath, bool writeInvalid = false)
        {
            if (!File.Exists(filePath))
            {
                if (writeInvalid) WriteWithColor("The specified file doesn't exists.\n", ConsoleColor.DarkYellow, true);
                return false;
            }

            string fileContents = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(fileContents) || fileContents.Count(_ => _ == '{') != 1)
            {
                if (writeInvalid) WriteWithColor($"This isn't a valid codec file.\n", ConsoleColor.DarkYellow, true);
                return false;
            }
            JSONNode jsonFile = JSONNode.Parse(fileContents);
#if DEBUG
            int jsonFileLine = 0;
#endif
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
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
                else
                {
                    keys.Add(keyValuePair.Key);
                    values.Add(keyValuePair.Value);
                }
#if DEBUG
                jsonFileLine++;
#endif
            }
            bool reoder = false;
            int maxlenght = values[0].Length;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Length > maxlenght) reoder = true;
            }

            if (reoder)
            {
                bool repeat = false;
                do
                {
                    Console.Clear();
                    WriteWithColor(Path.GetFileNameWithoutExtension(filePath), ConsoleColor.DarkCyan);
                    Console.WriteLine(" isn't ordered in the right way.\n1. Fix\n2. Delete\n\nChoose: ");
                    switch (Console.ReadLine())
                    {
                        case "1": repeat = false; break;
                        case "2":
                            File.Delete(filePath);
                            return false;
                        default: repeat = true; break;
                    }
                } while (repeat);
                OrderLists(ref keys, ref values);
                CreateJSONCodec(filePath, keys, values);
                Console.Clear();
            }
#if DEBUG
            Debug.WriteLine($"\u2713{Path.GetFileNameWithoutExtension(filePath)}");
#endif
            return true;
        }

        private static string ChooseCodec(bool onlyCheck = false)
        {
            List<string> codecs = new List<string>();
            foreach (string filePath in Directory.GetFiles(Environment.CurrentDirectory))
            {
                string fileName = Path.GetFileName(filePath);
                if (fileName.EndsWith(".json") && IsValidCodec(fileName)) codecs.Add(fileName);
            }

            if (codecs.Count > 0)
            {
                if (onlyCheck) return bool.TrueString;
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

        private static string RequestFile()
        {
            while (true)
            {
                Console.WriteLine("To import a file:\n- Write the file path\n- Drag the file in the console window\n");
                Console.Write("Write file path here or type ");
                WriteWithColor("ABORT", ConsoleColor.DarkRed);
                Console.Write(": ");
                string importFilePath = Console.ReadLine().Replace("\"", null);
                Console.Clear();
                if (importFilePath == string.Empty || importFilePath == "ABORT") return bool.FalseString;
                if (importFilePath.Contains("/") ^ importFilePath.Contains("\\")) return importFilePath;
                else
                {
                    WriteWithColor("Insert a valid file path\n", ConsoleColor.DarkYellow, true);
                    Sleep(1500);
                }
            }
        }

        private static bool GetInput(string keywordToStop, out string outputText)
        {
            Console.Clear();
            Console.Write("Type here the text that you want to encode, the program will accept user input until you write ");
            WriteWithColor($"{keywordToStop}", ConsoleColor.Green);
            Console.Write(" or ");
            WriteWithColor("ABORT", ConsoleColor.Red, true);
            Console.Write("You can also import text from files by typing ");
            WriteWithColor("IMPORT\n", ConsoleColor.DarkBlue, true);
            string line;
            outputText = string.Empty;
            do
            {
                line = Console.ReadLine();
                outputText += line + Environment.NewLine;
            } while (line != keywordToStop && line != "ABORT" && line != "IMPORT");

            if (line == "IMPORT")
            {
                Console.Clear();
                string filePath = RequestFile();
                if (filePath == bool.FalseString) return false;
                outputText = File.ReadAllText(filePath);
            }
            else if (line == keywordToStop) outputText = outputText.Remove(outputText.Length - 8, 8); //remove ENCODE and NewLine from string
            else return false;


            if (outputText.Length == 0)
            {
                WriteWithColor("You cannot encode the \"nothing\"", ConsoleColor.DarkYellow, true);
                Sleep(1500);
                return false;
            }
            else return true;
        }

        private static void OrderLists(ref List<string> key, ref List<string> value)
        {
            List<string> orderedValue = value.OrderBy(element => element.Length).ToList();
            orderedValue.Reverse();
            List<string> orderedKey = new List<string>();
            foreach (string item in orderedValue) orderedKey.Add(key[value.IndexOf(item)]);
            key = orderedKey;
            value = orderedValue;
        }

        private static void CodecOverview(List<string> key = null, List<string> value = null)
        {
            if (key is null) SetupLists(out key, out value);
            if (key.Count > 0)
            {
                for (int i = 0; i < key.Count; i++)
                {
                    WriteWithColor(key[i], ConsoleColor.Blue);
                    Console.Write(" => ");
                    WriteWithColor(value[i], ConsoleColor.Cyan, true);
                }
            }
            else WriteWithColor("The codec is empty, please add something first!", ConsoleColor.DarkYellow, true);
        }

        private static void CodecEditor(bool modifyCodec)
        {
            string line;
            List<string> codecChars, encodedChars;
            if (modifyCodec) SetupLists(out codecChars, out encodedChars);
            else
            {
                codecChars = new List<string>();
                encodedChars = new List<string>();
            }
            bool restartModify;
            do
            {
                restartModify = false;
                Console.Clear();
                Console.Write("Hi, to create or modify an encoding format you need to follow these rules:\nThe text format must be the following: ");
                WriteWithColor('X', ConsoleColor.Blue);
                Console.Write('-');
                WriteWithColor("YYY", ConsoleColor.Cyan);
                Console.Write(" where\n - ");
                WriteWithColor('X', ConsoleColor.Blue);
                Console.Write(" is the character that you want to modify or add\n - ");
                WriteWithColor('Y', ConsoleColor.Cyan);
                Console.Write(" is the encoded or modified version of that character\nFor example if i want to encode all \"z\" to become \"A\", I'll write z-A\nFor deleting an existing character, you will need to type ");
                WriteWithColor("DELETE ", ConsoleColor.DarkRed);
                WriteWithColor('X', ConsoleColor.Blue);
                Console.Write(" where ");
                WriteWithColor('X', ConsoleColor.Blue);
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
                    line = Console.ReadLine().Replace(" ", null);
                    if (line.Contains("ABORT") || line.Contains("SAVE")) break;
                    else if (line == "LIST")
                    {
                        CodecOverview(codecChars, encodedChars);
                        continue;
                    }
                    else if (line.Contains("DELETE"))
                    {
                        line = line.Replace("DELETE", null);
                        if (line.Length == 1)
                        {
                            int charPosition = codecChars.IndexOf(line);
                            if (charPosition != -1)
                            {
#if DEBUG
                                Debug.WriteLine($"\u2326{codecChars[charPosition]}");
#endif
                                codecChars.RemoveAt(charPosition);
                                encodedChars.RemoveAt(charPosition);
                            }
                            else
                            {
                                WriteWithColor($"Can't delete {line} from the list because it isn't present", ConsoleColor.DarkYellow, true);
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

                        if (line.Contains("-") && !(line.EndsWith("-") || line.StartsWith("-")))
                        {
                            foreach (string subString in line.Split(','))
                            {
                                string[] splittedString = subString.Split('-');
                                if (splittedString.Length == 2 && splittedString[0].Length == 1 && !subString.Contains('\'')) //splittedString.Length ottiene il numero di sotto-stringhe in cui è stato divisa la variabile, se questo numero non è 2 vuol dire che è stato messo più di un trattino(tipo a-b-z) perché con un solo trattino si ottengono solamente 2 sotto-stringhe
                                {
                                    if (splittedString[0] == splittedString[1])
                                    {
                                        WriteWithColor($"Ignored {splittedString[0]} => {splittedString[1]}, the char and its encoded version are the same", ConsoleColor.DarkYellow, true);
                                        continue;
                                    }

                                    int encodedPosition = encodedChars.IndexOf(splittedString[1]);
                                    if (encodedPosition != -1)
                                    {
                                        if (splittedString[0] == codecChars[encodedPosition])
                                        {
                                            WriteWithColor($"Nothing changed.. ", ConsoleColor.DarkYellow);
                                            WriteWithColor($"\"{codecChars[encodedPosition]}\"", ConsoleColor.Blue);
                                            WriteWithColor(" was already ", ConsoleColor.DarkYellow);
                                            WriteWithColor($"\"{splittedString[1]}\"", ConsoleColor.Cyan, true);
                                        }
                                        else
                                        {
                                            WriteWithColor($"Ignored \"{splittedString[0]}\", ", ConsoleColor.DarkYellow);
                                            WriteWithColor($"\"{splittedString[1]}\"", ConsoleColor.Cyan);
                                            WriteWithColor(" is already the encoding of ", ConsoleColor.DarkYellow);
                                            WriteWithColor($"\"{codecChars[encodedPosition]}\"", ConsoleColor.Blue, true);
                                        }
                                    }
                                    else
                                    {

                                        for (int i = 0; i < encodedChars.Count; i++)
                                        {
                                            for (int j = 0; j < encodedChars.Count; j++)
                                            {
                                                if (encodedChars[i] + encodedChars[j] == splittedString[1])
                                                {
                                                    WriteWithColor($"Cannot accept \"{splittedString[1]}\" as an encoding for ", ConsoleColor.DarkYellow);
                                                    WriteWithColor($"\"{splittedString[0]}\"", ConsoleColor.Blue);
                                                    WriteWithColor(" because it's already used for encoding ", ConsoleColor.DarkYellow);
                                                    WriteWithColor($"\"{codecChars[i]}{codecChars[j]}\"", ConsoleColor.Blue, true);
                                                    goto continueForEach;
                                                }
                                                else if (splittedString[1] + encodedChars[j] == encodedChars[i])
                                                {
                                                    WriteWithColor($"Cannot accept \"{splittedString[1]}\" as an encoding for ", ConsoleColor.DarkYellow);
                                                    WriteWithColor($"\"{splittedString[0]}\"", ConsoleColor.Blue);
                                                    WriteWithColor(" because there would be an ambiguity between ", ConsoleColor.DarkYellow);
                                                    WriteWithColor($"\"{codecChars[i]}\"", ConsoleColor.Blue);
                                                    WriteWithColor(" and ", ConsoleColor.DarkYellow);
                                                    WriteWithColor($"\"{splittedString[0]}{codecChars[j]}\"", ConsoleColor.Blue, true);
                                                    goto continueForEach;
                                                }
                                                else if (encodedChars[j] + splittedString[1] == encodedChars[i])
                                                {
                                                    WriteWithColor($"Cannot accept \"{splittedString[1]}\" as an encoding for ", ConsoleColor.DarkYellow);
                                                    WriteWithColor($"\"{splittedString[0]}\"", ConsoleColor.Blue);
                                                    WriteWithColor(" because there would be an ambiguity between ", ConsoleColor.DarkYellow);
                                                    WriteWithColor($"\"{codecChars[i]}\"", ConsoleColor.Blue);
                                                    WriteWithColor(" and ", ConsoleColor.DarkYellow);
                                                    WriteWithColor($"\"{codecChars[j]}{splittedString[0]}\"", ConsoleColor.Blue, true);
                                                    goto continueForEach;
                                                }
                                            }
                                        }

                                        int charPosition = codecChars.IndexOf(splittedString[0]);
                                        if (charPosition != -1) //check if char is already in the list
                                        {
#if DEBUG
                                            Debug.WriteLine($"\u26A0{splittedString[0]}({encodedChars[charPosition]} => {splittedString[1]})");
#endif
                                            WriteWithColor($"{splittedString[0]} has been modified ({encodedChars[charPosition]} => {splittedString[1]})", ConsoleColor.DarkYellow, true);
                                            encodedChars[charPosition] = splittedString[1];
                                        }
                                        else
                                        {
                                            codecChars.Add(splittedString[0]);
                                            encodedChars.Add(splittedString[1]);
#if DEBUG
                                            Debug.WriteLine($"\uFF0B {subString}");
#endif
                                        }
                                    }
                                }
                                else WriteWithColor($"Ignored {subString}, incorrect text formatting", ConsoleColor.DarkYellow, true);
                                continueForEach:
                                continue;
                            }
                        }
                        else WriteWithColor("Ignored line, incorrect text formatting", ConsoleColor.DarkYellow, true);
                    }
                }


                if (line.Contains("SAVE") && codecChars.Count > 0)
                {
                    bool redo;
                    do
                    {
                        redo = false;
                        Console.Clear();
                        Console.WriteLine("Codec overview: ");
                        CodecOverview(codecChars, encodedChars);
                        Console.Write("What do you want to do?:\n1. Save\n2. Continue Modify \n3. Abort\nChoose: ");
                        int.TryParse(Console.ReadLine(), out int reply);
                        switch (reply)
                        {
                            case 1:
                                string filePath;
                                if (modifyCodec) filePath = codecPath;
                                else
                                {
                                    Console.Write("Insert filename: ");
                                    filePath = RemoveIllegalChar(Console.ReadLine());

                                    if (!filePath.EndsWith(".json")) filePath += ".json";
                                    if (File.Exists(filePath))
                                    {
                                        Console.Write($"File {filePath.Replace(Environment.CurrentDirectory, null).Replace(".json", null)} already exists. Do you want to overwrite it? (Y/n): ");
                                        if (Console.ReadLine().ToUpper() == "N") goto default;
                                    }
                                    codecPath = filePath;
                                }
                                OrderLists(ref codecChars, ref encodedChars);
                                CreateJSONCodec(filePath, codecChars, encodedChars);

                                Console.Write($"\nFile {(modifyCodec ? "modified" : "created")} successfully, do you want to load it? (Y/n): ");
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

        private static void Main()
        {
            int result;
            do
            {
                Console.Clear();
                isCodecLoaded = CheckCodec(codecPath);
                Console.WriteLine($"Select Mode");
                WriteWithColor("1. Encode\n2. Decode\n3. Load Encoding Format", EDLMEDO, true);
                Console.WriteLine("4. Create Encoding Format");
                WriteWithColor("5. Modify Encoding Format\n6. Export Codec", EDLMEDO, true);
                Console.WriteLine("7. Import Codec");
                WriteWithColor("8. Delete Codec\n9. Codec Overview", EDLMEDO, true);
                Console.Write("\nChoose an option or Type 0 to Exit: ");
                if (!int.TryParse(Console.ReadLine(), out result)) result = -1;
                Console.Clear();
                switch (result)
                {
                    #region "Exit"
                    case 0:
                        Console.Clear();
                        WriteWithColor("Goodbye :)", ConsoleColor.Green);
                        Sleep(500);
                        WriteWithColor("\rGoodbye :)", ConsoleColor.Blue);
                        Sleep(500);
                        WriteWithColor("\rGoodbye :)", ConsoleColor.Red);
                        Sleep(300);
                        Environment.Exit(0);
                        break;
                    #endregion
                    #region "Encode"
                    case 1:
                        if (EDLMEDO != ConsoleColor.DarkGray)
                        {
                            if (isCodecLoaded)
                            {
                                if (!GetInput("ENCODE", out string textToEncode)) goto default;

                                SetupLists(out List<string> normalChar, out List<string> encodedValue);
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
                                stringChars.ForEach(_char => encodedText += _char);
                                Console.Clear();
                                if (!textToEncode.EndsWith("\n")) textToEncode += '\n';
                                Console.WriteLine($"{textToEncode}has been encoded to:\n{encodedText}\n");
                                Console.Write("Do you want to export it? (y/N): ");
                                if (Console.ReadLine().ToUpper() == "Y") ExportFile(encodedText);
                            }
                            else
                            {
                                WriteWithColor("Select a codec first!", ConsoleColor.Yellow, true);
                                Sleep(1500);
                            }
                        }
                        result = -1;
                        break;
                    #endregion
                    #region "Decode"
                    case 2:
                        if (EDLMEDO != ConsoleColor.DarkGray)
                        {
                            if (isCodecLoaded)
                            {
                                if (!GetInput("DECODE", out string textToDecode)) goto default;

                                SetupLists(out List<string> normalChar, out List<string> encodedValue);
                                char[] charsToDecode = textToDecode.ToCharArray();
                                int maxEncodedValueLenght = encodedValue[0].Length;
                                List<string> decodedChars = new List<string>();

                                int skipForXTimes = 0;
                                for (int i = 0; i < charsToDecode.Length; i++)
                                {
                                    if (skipForXTimes > 0)
                                    {
                                        skipForXTimes--;
                                        continue;
                                    }

                                    bool repeat = true;

                                    int remainingSpace = charsToDecode.Length - i;
                                    int maxCheck = (remainingSpace < maxEncodedValueLenght) ? remainingSpace : maxEncodedValueLenght; //to avoid adding non-existing chars to $charsToCheck

                                    while (repeat)
                                    {
                                        if (maxCheck == 0) //decode for value doesn't exist
                                        {
                                            decodedChars.Add(charsToDecode[i].ToString());
                                            break;
                                        }

                                        string charsToCheck = "";
                                        for (int c = 0; c < maxCheck; c++)
                                        {
                                            charsToCheck += charsToDecode[i + c];
                                        }

                                        int decodedIndex = encodedValue.IndexOf(charsToCheck);
                                        if (decodedIndex != -1)
                                        {
                                            decodedChars.Add(normalChar[decodedIndex]);
                                            repeat = false;
                                            skipForXTimes = maxCheck - 1;
                                        }
                                        else
                                        {
                                            if (maxCheck > 0) maxCheck--;
                                        }
                                    }
                                }
                                string decodedText = string.Empty;
                                decodedChars.ForEach(decodedchar => decodedText += decodedchar);
                                Console.Clear();
                                Console.WriteLine($"{textToDecode}has been encoded to:\n{decodedText}\n");
                                Console.Write("Do you want to export it? (y/N): ");
                                if (Console.ReadLine().ToUpper() == "Y") ExportFile(decodedText);
                            }
                            else
                            {
                                WriteWithColor("Select a codec first!", ConsoleColor.Yellow, true);
                                Sleep(1500);
                            }
                        }
                        result = -1;
                        break;
                    #endregion
                    #region "Load Codec"
                    case 3:
                        if (EDLMEDO != ConsoleColor.DarkGray) codecPath = ChooseCodec();
                        result = -1;
                        break;
                    #endregion
                    #region "Create Codec"
                    case 4:
                        CodecEditor(false);
                        result = -1;
                        break;
                    #endregion
                    #region "Modify Codec"
                    case 5:
                        if (EDLMEDO != ConsoleColor.DarkGray)
                        {
                            if (isCodecLoaded)
                            {
                                CodecEditor(true);
                            }
                            else
                            {
                                WriteWithColor("Select a codec first!", ConsoleColor.Yellow, true);
                                Sleep(1500);
                            }

                        }
                        result = -1;
                        break;
                    #endregion
                    #region "Export Codec"
                    case 6:
                        if (EDLMEDO != ConsoleColor.DarkGray)
                        {
                            string codec = ChooseCodec();
                            if (codec != string.Empty) ExportCodec(codec);
                        }
                        result = -1;
                        break;
                    #endregion
                    #region "Import Codec"
                    case 7:
                        string importFilePath;
                        do
                        {
                            importFilePath = RequestFile();
                            if (importFilePath == bool.FalseString) goto default;
                        } while (!IsValidCodec(importFilePath, true));
                        string codecName = Path.GetFileNameWithoutExtension(importFilePath);
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
                        WriteWithColor($"Successfully imported {codecName}", ConsoleColor.Green, true);
                        Console.Write("Do you want to load it? (y/N): ");
                        if (Console.ReadLine().ToUpper() == "Y") codecPath = codecName + ".json";
                        result = -1;
                        break;
                    #endregion
                    #region "Delete Codec"
                    case 8:
                        if (EDLMEDO != ConsoleColor.DarkGray)
                        {
                            for (int i = 0; i < (Console.WindowWidth / 2) - 4; i++) Console.Write(' ');
                            WriteWithColor("WARNING!", ConsoleColor.DarkRed, true);
                            WriteWithColor(string.Format($"{{0,{(Console.WindowWidth / 2) + 44}}}", "If you choose to delete a Codec, unless you have a backup, you will permanently lost it!"), ConsoleColor.Red, true);
                            WriteWithColor(string.Format($"{{0,{(Console.WindowWidth / 2) + 43}}}", "There is no confirmation for deleting files, so choose wisely. if you understood, then"), ConsoleColor.Red, true);
                            for (int i = 0; i < (Console.WindowWidth / 2) - 12; i++) Console.Write(' ');
                            WriteWithColor("Press a key to continue...", ConsoleColor.DarkMagenta, true);
                            Console.ReadKey();
                            string codec_8 = ChooseCodec();
                            if (codec_8 != string.Empty) File.Delete(codec_8);
                        }
                        result = -1;
                        break;
                    #endregion:
                    #region "Codec Overview"
                    case 9:
                        if (EDLMEDO != ConsoleColor.DarkGray)
                        {
                            if (isCodecLoaded)
                            {
                                WriteWithColor(CodecName, ConsoleColor.DarkCyan);
                                Console.WriteLine($" Codec Overview:\n");
                                CodecOverview();
                                Console.WriteLine("\nPress a key to close...");
                                Console.ReadKey();
                            }
                            else
                            {
                                WriteWithColor("Select a codec first!", ConsoleColor.Yellow, true);
                                Sleep(1500);
                            }
                        }
                        result = -1;
                        break;
                    #endregion
                    default: result = -1; break;
                }
            } while (result == -1);
        }
    }
}
