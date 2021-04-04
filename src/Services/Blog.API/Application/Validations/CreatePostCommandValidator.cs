using Blog.API.Application.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Blog.API.Application.Validations
{
    public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator(ILogger<CreatePostCommandValidator> logger)
        {
            RuleFor(command => command.Title).NotEmpty();
            RuleFor(command => command.Body).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
