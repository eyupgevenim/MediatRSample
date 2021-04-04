using MediatR;
using System.Collections.Generic;

namespace Blog.API.Domain
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual int Id { get; set; }


        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();
        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }
        public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);
        public void ClearDomainEvents() => _domainEvents?.Clear();
    }
}
