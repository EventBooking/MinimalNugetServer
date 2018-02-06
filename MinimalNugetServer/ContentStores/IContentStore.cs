namespace MinimalNugetServer.ContentStores
{
    public interface IContentStore
    {
        void Add(string contentId, string filePath);
        bool TryGetValue(string contentId, out byte[] content);
        void Clear();
    }
}
