using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToUnderscoreCase(this string source)
        {
            var stringBuilder = new StringBuilder();
            var startOfWord = true;

            foreach (var ch in source)
            {
                if (char.IsUpper(ch) && !startOfWord)
                {
                    stringBuilder.Append("_");
                }

                stringBuilder.Append(ch.ToString().ToLower());

                startOfWord = char.IsWhiteSpace(ch);
            }

            return stringBuilder.ToString();
        }
    }
}
