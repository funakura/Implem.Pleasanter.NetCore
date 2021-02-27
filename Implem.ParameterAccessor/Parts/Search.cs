using System.Runtime.Serialization;
namespace Implem.ParameterAccessor.Parts
{
    public class Search
    {
        public bool SearchDocuments;
        public bool CreateIndexes;
        public int PageSize;
        public bool DisableCrossSearch;
        public string FullTextEngine;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            FullTextEngine = string.IsNullOrWhiteSpace(FullTextEngine) ? "" : FullTextEngine;
        }
    }

}