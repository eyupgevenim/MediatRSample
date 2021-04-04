using AutoMapper;
using Blog.API.Application.Models;
using Blog.API.Data;
using Blog.API.Domain;
using Blog.API.Domain.Posts;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.API.Application.Commands
{
    // Regular CommandHandler
    public class PostPageOptionsCommandHandler : IRequestHandler<PostPageOptionsCommand, PostPageResultModel>
    {
        private readonly IRepository<Post> _repositoryPost;
        public readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public PostPageOptionsCommandHandler(IRepository<Post> repositoryPost, IMapper mapper, IMediator mediator)
        {
            _repositoryPost = repositoryPost ?? throw new ArgumentNullException(nameof(repositoryPost));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<PostPageResultModel> Handle(PostPageOptionsCommand request, CancellationToken cancellationToken)
        {
            var query = _repositoryPost.Table;

            if (!string.IsNullOrEmpty(request.Title))
                query = query.Where(x => x.Title.Contains(request.Title));

            if (!string.IsNullOrEmpty(request.Body))
                query = query.Where(x => x.Body.Contains(request.Body));

            var queryPagingResult = new PagingResult<Post>(query, pageIndex:(request.Page - 1), pageSize: request.Count);
            var postPageResultModel = _mapper.Map<PostPageResultModel>(queryPagingResult);
            postPageResultModel.Page = request.Page;

            return Task.FromResult(postPageResultModel);
        }

    }
}
