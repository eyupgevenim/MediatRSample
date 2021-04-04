using AutoMapper;
using Blog.API.Application.Models;
using Blog.API.Data;
using Blog.API.Domain.Posts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.API.Application.Commands
{
    public class GetPostCommandHandler : IRequestHandler<GetPostCommand, PostModel>
    {
        private readonly IRepository<Post> _repositoryPost;
        public readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetPostCommandHandler(IRepository<Post> repositoryPost, IMapper mapper, IMediator mediator)
        {
            _repositoryPost = repositoryPost ?? throw new ArgumentNullException(nameof(repositoryPost));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<PostModel> Handle(GetPostCommand request, CancellationToken cancellationToken)
        {
            var post = _repositoryPost.GetByIdAsync(request.Id).Result;

            return Task.FromResult(_mapper.Map<PostModel>(post));
        }

    }
}
