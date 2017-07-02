using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Translate
{
    public static class Translator
    {
        private static string _InputText = "";                       // Stores the Input text, to display it in case of exception for example
        public static string InputText
        {
            set { _InputText = value; }
            get { return _InputText; }
        }

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
            input = Regex.Replace(input, @"\s+","");

            if (input.Length % 8 == 0)                                           // Each character must contain 8 bits or the code won't be translatable
            {
                stBuild.Clear();
                var inputs = new List<string>();

                foreach(var character in input)
                {
                    stBuild.Append(character);

                    if (stBuild.Length == 8)
                    {
                        inputs.Add(stBuild.ToString());
                        stBuild.Clear();
                    }
                }

                stBuild.Clear();
                foreach(var number in inputs)            // Transforming inputs into char
                    stBuild.Append((char)ConvertToBase10(number, 2));
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
                    return OctToText(input);        
                default:
                    return "";
            }
        }

        static string TextToAscii(string input, Base _base)
        {
            switch (_base)
            {
                case Base.Binary:
                    return string.Join(" ", input.ToCharArray().Select(i => ToBin(i,8)).ToList());
                case Base.Decimal:
                    return string.Join(" ", input.ToCharArray().Select(i => (int)i).ToList());
                case Base.Octadecimal:
                    return string.Join(" ", input.ToCharArray().Select(i => ConvertToBaseN(i, 8)).ToList());
                case Base.Hexadecimal:
                    return string.Join(" ", input.ToCharArray().Select(i => ConvertToBaseN(i, 16)).ToList());

                default:
                    return "";
            }
        }

        #region Conversion Logics
        static string ToBin(int value, int len)      // Converts integer to bits. len is the length | http://stackoverflow.com/questions/1838963/easy-and-fast-way-to-convert-an-int-to-binary
        {
            return (len > 1 ? ToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }

        static string IntToText(string input)
        {
            stBuild.Clear();

            try
            {
                var numbers = Regex.Split(input, @"\s+").Where(n => n != "").Select(long.Parse).ToList();
                foreach (var number in numbers)
                    stBuild.Append((char)number);
            }
            catch (OverflowException) { }

            return stBuild.ToString();
        }

        static string OctToText(string input)
        {
            var output = Regex.Split(input, @"\s+").Where(i=>i!="").Select(i => (char)ConvertToBase10(i, 8)).ToList();
            return string.Join("", output);
        }

        static string HexToText(string input)
        {
            var output = Regex.Split(input, @"\s+").Select(n=>(char)ConvertToBase10(n,16)).ToList();
            return string.Join("", output);
        }


        #region Converters
        private static int ConvertToBase10(string number, int numberBase)
        {
            string digits = "0123456789ABCDEF";
            var numbers = number.ToCharArray().ToList();

            var remstack = new Stack<int>();

            while (numbers.Count > 0)
            {
                int index = digits.IndexOf(numbers[0]);
                remstack.Push(index);
                numbers.RemoveAt(0);
            }

            var num = 0;
            var remStackCount = remstack.Count;
            for (int i = 0; i < remStackCount; i++)
                num += (remstack.Pop() * (int)Math.Pow(numberBase, i));

            return num;
        }

        private static string ConvertToBaseN(int decNumber, int nBase)
        {
            string digits = "0123456789ABCDEF";

            var remstack = new Stack<int>();

            while (decNumber > 0)
            {
                int rem = (int)(decNumber % (int)nBase);
                remstack.Push(rem);
                decNumber /= (int)nBase;
            }

            var newString = new StringBuilder();
            while (remstack.Count != 0)
                newString.Append(digits[remstack.Pop()]);

            return newString.ToString();
        }
        #endregion

        #endregion
    }
}