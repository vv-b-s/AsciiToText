using System;
using System.Text;
/*using System.Collections.Generic;
using System.Linq;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;*/

namespace Translate
{
    public static class Translator
    {
        public static StringBuilder stBuild;

        public enum Type { None, Ascii, Text };
        public static string ConvertTo(Type type, string text, int _base)
        {
            switch (type)
            {
                case Type.Ascii:
                    return TextToAscii(text, _base);

                case Type.Text:
                    return (_base == 0) ? AsciiToText(text) : AsciiToText(text, _base);

            }
            return null;
        }

        static string AsciiToText(string input)  // base 2
        {
            string output = Ascii___Text.MainActivity.Text;
            input = input.Replace(" ", "");   
            
            if (input.Length % 8 == 0)                                           //Checks if the input can be translated
            {
                stBuild = new StringBuilder();
                string[] inputs = new string[input.Length / 8];                   // splits the string to bits
                int j = 0;                                                       // counts the index of inputs[]

                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == ' ')
                        continue;

                    stBuild.Append(input[i]);

                    if (stBuild.Length == 8)
                    {
                        inputs[j] = stBuild.ToString();
                        stBuild = new StringBuilder();
                        j++;
                    }
                }

                stBuild = new StringBuilder();
                for (int i = 0; i < inputs.Length; i++)             // Transforming inputs into char
                    stBuild.Append((char)BinToInt(inputs[i]));
                output = stBuild.ToString();

                return output;
            }
            else
                return output;
        }

        static string AsciiToText(string input, int _base)   //Other bases 
        {
            switch (_base)
            {
                case 1:                                   // Base 10
                    return IntToText(input);
                case 2:                                 // Base 16
                    return HexToText(input);
                case 3:                               // Base 8
                    return OctToString(input);        
                default:
                    return "";
            }
        }

        static string TextToAscii(string input, int _base)
        {
            char[] letters = new char[input.Length];                             // Splits all the letters
            for (int i = 0; i < input.Length; i++)
                letters[i] = input[i];

            stBuild = new StringBuilder();
            string output;

            switch (_base)
            {
                case 0:                                                                              //Binary conversion
                    for (int i = 0; i < letters.Length; i++)
                    {
                        stBuild.Append(ToBin(letters[i], 8));
                        stBuild.Append(" ");      // Adds intervals if spacing is not enabled
                    }
                    output = stBuild.ToString();
                    stBuild = new StringBuilder();

                    return output;

                case 1:                                                                     // Decimal Conversion
                    for (int i = 0; i < letters.Length; i++)
                    {
                        stBuild.Append((long)letters[i]);
                        stBuild.Append(' ');
                    }
                    output = stBuild.ToString();
                    stBuild = new StringBuilder();

                    return output;


                case 2:                                                             // Hexadecimal conversion
                    for (int i = 0; i < letters.Length; i++)
                    {
                        stBuild.Append(ToHex(letters[i]));
                        stBuild.Append(' ');
                    }
                    output = stBuild.ToString();
                    stBuild = new StringBuilder();

                    return output;

                case 3:                                                     //Octadecimal
                    for (int i = 0; i < letters.Length; i++)
                    {
                        stBuild.Append(ToOct(letters[i]));
                        stBuild.Append(' ');
                    }
                    output = stBuild.ToString();
                    stBuild = new StringBuilder();

                    return output;
                default:
                    return "";
            }
        }

        #region Conversion Logics
        static string ToBin(int value, int len)      // Converts integer to bits. len is the length | http://stackoverflow.com/questions/1838963/easy-and-fast-way-to-convert-an-int-to-binary
        {
            return (len > 1 ? ToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }

        static int BinToInt(string input)             // takes a string of bits and converts it into integer
        {
            int output = 0;
            for (int i = input.Length-1, j=0; i >=0; i--,j++)
            {
                if (input[j] == '1')
                    output += (int)Math.Pow(2, i);
            }
            return output;
        }

        static string IntToText(string input)
        {
            stBuild = new StringBuilder();
            if (input.Contains(" "))
            {
                string[] numbers = input.Split();
                long integer = 0;
                for (int i = 0; i < numbers.Length; i++)
                {
                    long.TryParse(numbers[i], out integer);
                    stBuild.Append((char)integer);
                }
                return stBuild.ToString();
            }
            else
            {
                long integer;
                long.TryParse(input,out integer);
                return ((char)integer).ToString();                
            }
        }

        static string HexToText(string input)                                               // http://stackoverflow.com/questions/724862/converting-from-hex-to-string
        {
            stBuild = new StringBuilder();
            try
            {
                string[] insplit = new string[0];               // used when input has spaces
                if (input.Contains(" "))
                    insplit = input.Split();
                int[] raw = new int[(insplit.Length != 0) ? insplit.Length : 1];       // if insplit's length is > 0, raw will take its value. else it will take input's value
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] = int.Parse(((insplit.Length != 0) ? insplit[i] : input), System.Globalization.NumberStyles.HexNumber);  // if insplit contains something, insplit will be translated instead.
                    stBuild.Append((char)raw[i]);
                }
                return stBuild.ToString();
            }
            catch (FormatException)
            {
                return stBuild.ToString();
            }
            catch(OverflowException)
            {
                return stBuild.ToString();
            }
        }
        static string ToHex(int input) => Convert.ToInt64(input).ToString("x2");           // http://stackoverflow.com/questions/12527694/c-sharp-convert-char-to-byte-hex-representation

        static string OctToString(string input)
        {
            stBuild = new StringBuilder();

            if (input.Contains(" "))
            {
                try
                {
                    string[] temp = input.Split();
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].Length > 10)
                            return stBuild.ToString();
                        stBuild.Append((char)Convert.ToInt32(temp[i], 8));
                    }
                    return stBuild.ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return stBuild.ToString();
                }
            }
            else
            {
                try
                {
                    if (input.Length > 10)
                        return "";
                    return ((char)Convert.ToInt32(input, 8)).ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return "";
                }
            }
        }
        static string ToOct(int input) => Convert.ToString(input, 8);           //http://stackoverflow.com/questions/4123613/formatting-an-integer-as-octal

        #endregion
    }
}