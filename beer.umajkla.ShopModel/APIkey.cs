using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace beer.umajkla
{
    public class APIkey
    {
        public static Random Random = new Random();

        public string[] Parts { get; private set; }
        public char Separator { get; private set; }

        public APIkey()
        {
            Parts = new string[5];
            Separator = '-';
            GenerateKey(5, 5, false);
        }

        public APIkey(int partLength, int count, char separator = '-', bool withAmbiguousCharacters = false)
        {
            Parts = new string[count];
            Separator = separator;
            GenerateKey(partLength, count, withAmbiguousCharacters);
        }

        private APIkey(string[] parts, char separator)
        {
            Parts = parts;
            Separator = separator;
        }

        private void GenerateKey(int partLength, int count, bool withAmbiguousCharacters)
        {
            string chars = "BCDFGHJKMPQSTVWXY2346789";
            chars = (withAmbiguousCharacters) ? chars + "015AEILNOSUZ" : chars;
            for (int i = 0; i < count; i++)
            {
                Parts[i] = new string(Enumerable.Repeat(chars, partLength).Select(s => s[Random.Next(s.Length)]).ToArray());
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < Parts.Length; i++)
            {
                stringBuilder.AppendFormat("{0}{1}", Parts[i], Separator);
            }
            return stringBuilder.ToString();
        }

        public override int GetHashCode()
        {
            return (Parts.GetHashCode() + Separator.GetHashCode()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            APIkey other;
            try
            {
                other = (APIkey)obj;
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException("Supplied object is not an APIkey instance", "obj", ex);
            }
            if (Parts.Length != other.Parts.Length) return false;
            if (Separator != other.Separator) return false;
            for (int i = 0; i < Parts.Length; i++)
            {
                if (Parts[i] != other.Parts[i]) return false;
            }
            return true;
        }

        public static APIkey Parse(string key, char separator = '-')
        {
            string[] parts = key.Split(separator);
            if (parts.Length == 1) throw new FormatException("Wrong separator or not an API key");
            return new APIkey(parts, separator);
        }
    }
}
