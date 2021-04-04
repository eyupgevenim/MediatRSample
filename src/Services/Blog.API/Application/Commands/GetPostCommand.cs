using Blog.API.Application.Models;
using MediatR;

namespace Blog.API.Application.Commands
{
    public class GetPostCommand : IRequest<PostModel>
    {
        public int Id { get; }
        public GetPostCommand(int id)
        {
            Id = id;
        }
    }
}
