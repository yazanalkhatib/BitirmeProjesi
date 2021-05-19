namespace BitirmeProjesiWeb.Utilities
{
    public interface ISecurity
    {
        public string UserFullName { get; }
        bool Authenticate();
        bool AuthorizeAdmin();
    }
}