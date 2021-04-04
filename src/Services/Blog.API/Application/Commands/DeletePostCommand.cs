using MediatR;

namespace Blog.API.Application.Commands
{
    public class DeletePostCommand : IRequest<bool>
    {
        public int Id { get; }
        public DeletePostCommand(int id)
        {
            Id = id;
        }
    }
}
