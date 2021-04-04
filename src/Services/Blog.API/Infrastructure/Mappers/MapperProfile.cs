using AutoMapper;
using Blog.API.Application.Commands;
using Blog.API.Application.Models;
using Blog.API.Domain;
using Blog.API.Domain.Posts;

namespace Blog.API.Infrastructure.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Post, PostModel>();
            CreateMap<PostModel, Post>();

            CreateMap<PagingResult<Post>, PostPageResultModel>()
                .ForMember(postPageResultModel => postPageResultModel.Count, pagingResult => pagingResult.MapFrom(x => x.Items.Count))
                ;

            CreateMap<EditPostCommand, Post>();
            CreateMap<CreatePostCommand, Post>();
        }
    }
}
