using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToUnderscoreCase(this string source)
        {
            var sb = new StringBuilder();
            var startOfWord = true;

            foreach (var ch in source)
            {
                if (char.IsUpper(ch) && !startOfWord)
                {
                    sb.Append("_");
                }

                sb.Append(ch.ToString().ToLower());

                startOfWord = char.IsWhiteSpace(ch);
            }

            return sb.ToString();
        }
    }
}
