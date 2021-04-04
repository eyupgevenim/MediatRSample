namespace Blog.API
{
    public class BlogSettings
    {
        /// <summary>
        /// SqlServer
        /// InMemoryDatabase
        /// </summary>
        public string DbType { get; set; }
        public ConnectionStringsOptions ConnectionStrings { get; set; }
        public JwtOptions Jwt { get; set; }
    }

    public class ConnectionStringsOptions
    {
        public string DefaultConnection { get; set; }
        public string InMemoryConnection { get; set; }
    }

    public class JwtOptions
    {
        public string ApiKey { get; set; }
        public string IssuerSigningKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public int Expires { get; set; }
    }
}
