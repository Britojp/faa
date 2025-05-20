namespace FhirArtifactAnalyzer.Domain.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static string WithWildcards(this string? value)
        {
            if (value is null) 
                return string.Empty;

            return value.Contains('*') ? value : $"*{value}*";
        }
    }
}
