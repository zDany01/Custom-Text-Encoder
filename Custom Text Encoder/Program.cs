using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Custom_Text_Encoder
{
    internal class Program
    {
        static void Main()
        {

            string codecStatus = File.Exists("codec.json") ? "Loaded" : "Not Loaded";
            Console.Write($@"Select Mode:
1. Encode.
2. Decode.
3. Load Encoding Codec(Current status: {codecStatus}).
4. Create Encoding Codec.
5. Modify Encoding Codec.
Choose an option: ");
            switch (Convert.ToInt32(Console.ReadLine()))
            {
                case 0: //debug

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

        static string Chiper(string text, char[] normalChar, char[] encodedChar)
        {
            for (int i = 0; i < normalChar.Length; i++)
            {
                text = text.Replace(normalChar[i], encodedChar[i]);
            }
            return text;
        }
    }
}
