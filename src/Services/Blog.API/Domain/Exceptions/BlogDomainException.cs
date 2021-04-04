using System;

namespace Blog.API.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class BlogDomainException : Exception
    {
        public BlogDomainException() { }
        public BlogDomainException(string message) : base(message) { }
        public BlogDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
