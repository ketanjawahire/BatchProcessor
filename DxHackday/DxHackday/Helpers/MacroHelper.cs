using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DxHackday.Helpers
{
    public static class MacroHelper
    {
        private static readonly Regex _macroCheckRegex = new Regex(@"{{.*?}}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const string MacroStringFormat = "{{{{{0}}}}}";

        public static string GetMacroName(string macroName)
        {
            return string.Format(MacroStringFormat, macroName);
        }

        public static string GetStringWithoutMacro(string inputStr)
        {
            var matches = _macroCheckRegex.Match(inputStr);

            return matches.Groups["macro"].ToString();
        }

        public static IEnumerable<string> GetAllMacros(string stringWithMacros)
        {
            var matches = _macroCheckRegex.Matches(stringWithMacros);

            return (from object match in matches select match.ToString());
        }

        public static string ReplaceMacros(string inputStr, IDictionary<string, string> macroValueCollection)
        {
            try
            {
                if (string.IsNullOrEmpty(inputStr))
                {
                    return inputStr;
                }

                var inputStrMacros = GetAllMacros(inputStr);
                var sb = new StringBuilder(inputStr);

                foreach (var macro in inputStrMacros.Where(macroValueCollection.ContainsKey))
                {
                    macroValueCollection[macro] = ReplaceMacros(macroValueCollection[macro], macroValueCollection);

                    sb.Replace(macro, macroValueCollection[macro]);
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Problem parsing macro : " + inputStr, ex);
            }
        }
    }
}
