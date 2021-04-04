using Blog.API.Application.Models;
using MediatR;
using System;

namespace Blog.API.Application.Commands
{
    public class EditPostCommand : PostModel, IRequest<bool>
    {
        public int Id { get; private set; }
        public EditPostCommand(int id,string title, string body, string bodyOverview, string tags,
            bool allowComments, string metaKeywords, string metaDescription, string metaTitle, 
            DateTime createdOnUtc, DateTime? startDateUtc, DateTime? endDateUtc)
        {
            Id = id;
            Title = title;
            Body = body;
            BodyOverview = bodyOverview;
            Tags = tags;
            AllowComments = allowComments;
            MetaKeywords = metaKeywords;
            MetaDescription = metaDescription;
            MetaTitle = metaTitle;
            CreatedOnUtc = createdOnUtc;
            StartDateUtc = startDateUtc;
            EndDateUtc = endDateUtc;
        }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
