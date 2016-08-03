namespace DeepComparison
{
    internal sealed class Formatting
    {
        public string Format(object obj)
        {
            if (obj == null) return "<null>";
            return obj.ToString();
        }

        public ComparisonResult Explain(object x, object y, string tag)
        {
            var xText = Format(x);
            var yText = Format(y);
            return $"{tag}({xText}, {yText})";
        }
    }
}