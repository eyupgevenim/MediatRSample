using System;
using System.Threading.Tasks;
using MediatR;
using Blog.API.Application.Commands;
using Blog.API.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Blog.API.Domain.Exceptions;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Blog.API.Controllers
{
    /// <summary>
    /// Posts api controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v1/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IMediator mediator, ILogger<PostsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// search posts PostPageOptionsCommand postPageOptionsCommand parameter
        /// </summary>
        /// <param name="pageOptions">search parameters</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "A collection of Posts for the specified page.", typeof(PostPageResultModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The page request parameters are invalid.", typeof(IEnumerable<ValidationFailure>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "A page with the specified page number was not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        //[ProducesResponseType(typeof(PostPageResultModel), (int)HttpStatusCode.OK)]
        public virtual async Task<IActionResult> Get(PostPageOptionsCommand postPageOptionsCommand)//[FromQuery] 
        {
            return await Runsafely(async () =>
            {
                var posts = await _mediator.Send(postPageOptionsCommand);
                if (posts.Count > 0)
                    return Ok(posts);

                return NotFound();
            });
        }

        /// <summary>
        /// Get post by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "A page with the specified page number was not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public virtual async Task<IActionResult> Get(int id)
        {
            return await Runsafely(async () =>
            {
                var getPostCommand = new GetPostCommand(id);
                var post = await _mediator.Send(getPostCommand);
                if (post != null)
                    return Ok(post);

                return NotFound();
            });
        }

        /// <summary>
        /// Add post as CreatePostCommand model
        /// </summary>
        /// <param name="model">CreatePostCommand createPostCommand</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Insert success")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The page request parameters are invalid.", typeof(IEnumerable<ValidationFailure>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public virtual async Task<IActionResult> Post([FromBody] CreatePostCommand createPostCommand)
        {
            return await Runsafely(async () =>
            {
                var commandResult = await _mediator.Send(createPostCommand);
                return commandResult ? Ok() : BadRequest();
            });
        }

        /// <summary>
        /// Update post by id
        /// </summary>
        /// <param name="id">post id</param>
        /// <param name="model">EditPostCommand editPostCommand</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "A page with the specified page number was not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public virtual async Task<IActionResult> Put(int id, [FromBody] EditPostCommand editPostCommand)
        {
            return await Runsafely(async () =>
            {
                editPostCommand.SetId(id);
                var commandResult = await _mediator.Send(editPostCommand);
                return commandResult ? Ok() : BadRequest();
            });
        }

        /// <summary>
        /// Delete post by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "A page with the specified page number was not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            return await Runsafely(async () =>
            {
                var deletePostCommand = new DeletePostCommand(id);
                var commandResult = await _mediator.Send(deletePostCommand);
                return commandResult ? Ok() : BadRequest();
            });
        }

        #region Helpers

        private async Task<IActionResult> Runsafely(Func<Task<IActionResult>> func)
        {
            try
            {
                return await func();
            }
            catch (BlogDomainException blogDomainException) when (blogDomainException.InnerException is FluentValidation.ValidationException validationException)
            {
                _logger.LogError(blogDomainException, $"ValidationException - Message: {blogDomainException.Message}");

                return BadRequest(validationException.Errors);
            }
            catch (BlogDomainException blogDomainException)
            {
                _logger.LogError(blogDomainException, $"blogDomainException - Message: {blogDomainException.Message}");

                //TODO:...
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Message: {exception.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return BadRequest();
        }

        #endregion
    }
}
