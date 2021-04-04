using AutoMapper;
using Blog.API.Data;
using Blog.API.Domain.Posts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.API.Application.Commands
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, bool>
    {
        private readonly IRepository<Post> _repositoryPost;
        public readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreatePostCommandHandler(IRepository<Post> repositoryPost, IMapper mapper, IMediator mediator)
        {
            _repositoryPost = repositoryPost ?? throw new ArgumentNullException(nameof(repositoryPost));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var post = _mapper.Map<Post>(request);

            await _repositoryPost.InsertAsync(post);

            return await Task.FromResult(true);
        }

    }
}
