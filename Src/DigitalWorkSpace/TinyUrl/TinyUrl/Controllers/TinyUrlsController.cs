using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrlManaging.Core.Contracts;
using UrlManaging.Core.Model;

namespace UrlManaging.Controllers
{
    [Route("tinyurls")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class TinyUrlsController : ControllerBase
    {
        private readonly ITinyUrlOperations _tinyUrlOperations;
        private readonly ILogger<TinyUrlsController> _logger;

        public TinyUrlsController(ITinyUrlOperations tinyUrlRepository, ILogger<TinyUrlsController> logger)
        {
            _tinyUrlOperations = tinyUrlRepository;
            _logger = logger;
        }

        /// <summary>
        /// Redirects to original url based on the input tinyUrl
        /// </summary>
        /// <param name="url">tinyUrl</param>
        /// <returns></returns>
        [HttpGet("{url}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetUrl(string url)
        {
            var tinyUrl = _tinyUrlOperations.GetUrl(url);
            if(tinyUrl==null)
            {
                return NotFound();
            }
            return Redirect(tinyUrl.OriginalUrl);
        }

        /// <summary>
        /// Gets all the tiny urlCreated
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpHead]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<TinyUrl>> GetUrls()
        {
            var urls = _tinyUrlOperations.GetAllUrl();
            return Ok(urls);
        }

        /// <summary>
        /// Create short url 
        /// </summary>
        /// <param name="originalUrl">Original url for which the tiny url needs to be created</param>
        /// <returns>An action result of type TinyUrl</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<TinyUrl> CreateTinyUrl([FromBody] UrlInfo originalUrl)
        {
            _logger.LogInformation("Creation of tiny url initiated ");
            var tinyUrl=_tinyUrlOperations.CreateTinyUrl(originalUrl);
            return Ok(tinyUrl);
        }
    }
}
