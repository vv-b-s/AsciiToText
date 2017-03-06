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
        public static string InputText { set; get; }                       // Stores the Input text, to display it in case of exception for example

        static StringBuilder stBuild = new StringBuilder();

        public enum Type { None, Ascii, Text };
        public enum Base { Binary, Octadecimal, Decimal, Hexadecimal};
        

        public static string ConvertTo(Type type, string text, Base _base)
        {
            switch (type)
            {
                case Type.Ascii:
                    return TextToAscii(text, _base);

                case Type.Text:
                    return (_base == Base.Binary) ? AsciiToText(text) : AsciiToText(text, _base);

            }
            return null;
        }

        static string AsciiToText(string input)  // base 2 does not support unicode so it has to work separately
        {
            string output = InputText;
            input = input.Replace(" ", "");   
            
            if (input.Length % 8 == 0)                                           // Each character must contain 8 bits or the code won't be translatable
            {
                stBuild.Clear();
                string[] inputs = new string[input.Length / 8];
                int j = 0;                                                       // counts the index of inputs[]

                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == ' ')
                        continue;

                    stBuild.Append(input[i]);

                    if (stBuild.Length == 8)
                    {
                        inputs[j] = stBuild.ToString();
                        stBuild.Clear();
                        j++;
                    }
                }

                stBuild.Clear();
                for (int i = 0; i < inputs.Length; i++)             // Transforming inputs into char
                    stBuild.Append((char)BinToInt(inputs[i]));
                output = stBuild.ToString();

                return output;
            }
            else
                return output;
        }

        static string AsciiToText(string input, Base _base)   // Other bases, supporting Unicode
        {
            switch (_base)
            {
                case Base.Decimal:                                   
                    return IntToText(input);
                case Base.Hexadecimal:                               
                    return HexToText(input);
                case Base.Octadecimal:                               
                    return OctToString(input);        
                default:
                    return "";
            }
        }

        static string TextToAscii(string input, Base _base)
        {
            stBuild.Clear();
            string output;

            switch (_base)
            {
                case Base.Binary:                                                                        
                    for (int i = 0; i < input.Length; i++)
                    {
                        stBuild.Append(ToBin(input[i], 8));
                        stBuild.Append(" ");     
                    }
                    output = stBuild.ToString();
                    stBuild.Clear();

                    return output;

                case Base.Decimal:                                                                   
                    for (int i = 0; i < input.Length; i++)
                    {
                        stBuild.Append((long)input[i]);
                        stBuild.Append(' ');
                    }
                    output = stBuild.ToString();
                    stBuild.Clear();

                    return output;


                case Base.Hexadecimal:                                                            
                    for (int i = 0; i < input.Length; i++)
                    {
                        stBuild.Append(ToHex(input[i]));
                        stBuild.Append(' ');
                    }
                    output = stBuild.ToString();
                    stBuild.Clear();

                    return output;

                case Base.Octadecimal:                                    
                    for (int i = 0; i < input.Length; i++)
                    {
                        stBuild.Append(ToOct(input[i]));
                        stBuild.Append(' ');
                    }
                    output = stBuild.ToString();
                    stBuild.Clear();

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
            stBuild.Clear();
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
                return (integer!=0)?((char)integer).ToString():InputText;                
            }
        }

        static string HexToText(string input)                                               // http://stackoverflow.com/questions/724862/converting-from-hex-to-string
        {
            stBuild.Clear();
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
                return (stBuild.Length>0)?stBuild.ToString(): InputText;
            }
            catch(OverflowException)
            {
                return (stBuild.Length > 0) ? stBuild.ToString() : InputText;
            }
        }
        static string ToHex(int input) => Convert.ToInt64(input).ToString("x2");           // http://stackoverflow.com/questions/12527694/c-sharp-convert-char-to-byte-hex-representation

        static string OctToString(string input)
        {
            stBuild.Clear();

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
                    return (stBuild.Length > 0) ? stBuild.ToString() : InputText;
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