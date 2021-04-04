using Blog.API.Application.Models;

namespace Blog.API.Application.Commands
{

    /// <summary>
    /// post search model options
    /// </summary>
    public class PostPageOptionsCommand : PageOptionsCommand<PostPageResultModel>
    {
        public PostPageOptionsCommand(string title, string body = "", int page = 1, int count = 100) : base(page, count)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (Count < 1)
            {
                Count = 100;
            }

            Title = title;
            Body = body;
            Page = page;
            Count = count;
        }

        /// <summary>
        /// post title
        /// </summary>
        public string Title { get;}

        /// <summary>
        /// post body
        /// </summary>
        public string Body { get;}

        /// <summary>
        /// Data Count min:1 max:100
        /// </summary>
        public override int Count { get; protected set; }
    }
}
