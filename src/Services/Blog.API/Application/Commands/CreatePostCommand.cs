using Blog.API.Application.Models;
using MediatR;
using System;

namespace Blog.API.Application.Commands
{
    public class CreatePostCommand : PostModel, IRequest<bool>
    {
        public CreatePostCommand(string title, string body, string bodyOverview, string tags,
            bool allowComments, string metaKeywords, string metaDescription, string metaTitle, 
            DateTime createdOnUtc, DateTime? startDateUtc, DateTime? endDateUtc)
        {
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
    }
}
