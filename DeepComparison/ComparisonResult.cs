namespace DeepComparison
{
    /// <summary>Combines result of comparison with the description of what did not compare </summary>
    public sealed class ComparisonResult
    {
        /// <summary>True if objects are equal</summary>
        public bool AreEqual { get; }
        /// <summary>Path to the property that does not compare</summary>
        public string Path { get; }
        /// <summary>ctor</summary>
        public ComparisonResult(bool areEqual) { AreEqual = areEqual; }
        /// <summary>ctor</summary>
        public ComparisonResult(string path) { Path = path; }
    }
}