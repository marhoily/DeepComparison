namespace DeepComparison
{
    internal sealed class Formatting
    {
        public string Format(object obj)
        {
            if (obj == null) return "<null>";
            return obj.ToString();
        }
    }
}