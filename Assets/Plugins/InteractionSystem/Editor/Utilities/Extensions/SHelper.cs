using System;
using System.Text;
using System.Text.RegularExpressions;

namespace NodeEngine
{
    internal static class SHelper
    {
        internal static string GetVarType(Type type) => ConvertTypeToString(type);
        internal static string GetVarType<T>() => GetVarType(typeof(T));

        public static string ConvertTypeToString(Type type)
        {
            string typeName = type.ToString();
            int backtickIndex = typeName.IndexOf('`');
            if (backtickIndex >= 0)
            {
                typeName = typeName.Remove(backtickIndex, 2);
                typeName += "<";

                Type[] genericArguments = type.GetGenericArguments();
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    typeName += i > 0 ? ", " : "";
                    typeName += ConvertTypeToString(genericArguments[i]);
                }

                typeName += ">";
            }
            if (typeName.IndexOf("[") != -1)
            {
                var startIndex = typeName.IndexOf("[");
                var finishIndex = typeName.IndexOf("]") + 1;
                typeName = typeName.Remove(startIndex, finishIndex - startIndex);
            }
            typeName = typeName.Replace("+", ".");

            return typeName;
        }
        public static string RemoveSpacesAndContentBetweenParentheses(string input)
        {
            // Проверка на null или пустую строку
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            StringBuilder result = new StringBuilder();
            bool insideParentheses = false;
            foreach (char c in input)
            {
                if (c == '(')
                {
                    insideParentheses = true;
                }
                else if (c == ')')
                {
                    insideParentheses = false;
                }
                else if (!insideParentheses)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        result.Append(c);
                    }
                }
            }

            return result.ToString();
        }
        internal static string GetShortVarType(Type type)
        {
            string fullName = GetVarType(type);
            string pattern = @"(?<type>[^,<>]+)(?:(?<gen><[^<>]+>)+)?";
            string result = Regex.Replace(fullName, pattern, match => GetShortType(match.Groups["type"].Value) + match.Groups["gen"].Value);

            return result;
        }
        private static string GetShortType(string type)
        {
            string[] parts = type.Split('.');

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "System") { continue; }
                if (parts[i].StartsWith("System."))
                    parts[i] = parts[i].Substring("System.".Length);
            }

            return string.Join(".", parts);
        }
    }
}
