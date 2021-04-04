using Blog.API.Data;
using Blog.API.Domain.Posts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.API.Application.Commands
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IRepository<Post> _repositoryPost;
        private readonly IMediator _mediator;

        public DeletePostCommandHandler(IRepository<Post> repositoryPost, IMediator mediator)
        {
            _repositoryPost = repositoryPost ?? throw new ArgumentNullException(nameof(repositoryPost));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = _repositoryPost.GetById(request.Id);
            await _repositoryPost.DeleteAsync(post);

            return await Task.FromResult(true);
        }
    }
}
