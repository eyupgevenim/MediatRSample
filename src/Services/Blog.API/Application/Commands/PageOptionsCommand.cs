using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.API.Application.Commands
{
    /// <summary>
    /// Page Options
    /// </summary>
    public abstract class PageOptionsCommand<TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// Page Number min:1 max:int.MaxValue
        /// </summary>
        public virtual int Page { get; protected set; }

        /// <summary>
        /// Data Count min:1 max:20
        /// </summary>
        public virtual int Count { get; protected set; }

        protected PageOptionsCommand(int page = 1, int count = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (Count < 1)
            {
                Count = 10;
            }

            Page = page;
            Count = count;
        }
    }
}
