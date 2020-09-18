using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using UrlManaging.Core.Contracts;
using UrlManaging.Core.Model;

namespace UrlManaging.Core
{
    public class TinyUrlOperations : ITinyUrlOperations
    {
        private IUrlContext _urlContext;
        private readonly IUrlExpiredEventHandler _urlExpiredEventHandler;
        private readonly ILogger<TinyUrlOperations> _logger;

        public TinyUrlOperations(IUrlContext urlContext,IUrlExpiredEventHandler urlExpiredEventHandler, ILogger<TinyUrlOperations> logger)
        {
            _urlContext = urlContext;
            _urlExpiredEventHandler = urlExpiredEventHandler;
            _logger = logger;
        }
        public TinyUrl GetUrl(string shortName)
        {
            return _urlContext.TinyUrl.Where(v => v.ShortUrl.Equals(shortName)).FirstOrDefault();
        }
        public IList<TinyUrl> GetAllUrl()
        {
            return _urlContext.TinyUrl.ToList();
        }

        public TinyUrl CreateTinyUrl(UrlInfo urlInfo)
        {
            var shortUrl = ShortUrlGenerator.RandomString(8);
            while(_urlContext.TinyUrl.Where(v=>v.ShortUrl==shortUrl).Any())
            {
                shortUrl = ShortUrlGenerator.RandomString(8);
            }
            
            var tinyUrl = new TinyUrl { OriginalUrl = urlInfo.OriginalUrl, ShortUrl = shortUrl ,Expiry=urlInfo.Expiry};
            _urlContext.SaveUrl(tinyUrl);
            _logger.LogInformation("Tiny url generated");
            return tinyUrl;
        }
       

        public void LinkUrl(string url)
        {
            var tinyUrl = _urlContext.TinyUrl.Where(v => v.ShortUrl.Equals(url)).FirstOrDefault();
            tinyUrl.IsLinked = true;
            _logger.LogInformation("Tiny url is made unexpirable");
            _urlContext.SaveChanges();
        }

        public void UnLinkUrl(string url)
        {
            var tinyUrl = _urlContext.TinyUrl.Where(v => v.ShortUrl.Equals(url)).FirstOrDefault();
            tinyUrl.IsLinked = false;
            _logger.LogInformation("Tiny url is made expirable");
            _urlContext.SaveChanges();
        }

        public IList<string> UrlsExpired(DateTime date)
        {
            return _urlContext.TinyUrl.Where(d => d.Expiry <= date.Date&&!d.IsLinked).Select(v => v.ShortUrl).ToList();
        }

        public void DeleteUrls(string shortUrl)
        {
            var urlTobeDeleted=_urlContext.TinyUrl.Where(d => d.ShortUrl == shortUrl).FirstOrDefault();
            if(urlTobeDeleted==null)
            {
                _logger.LogInformation("No url found matching for {shortUrl} hence deletion canceled", shortUrl);
                return;
            }
            _urlContext.TinyUrl.Remove(urlTobeDeleted);
            _urlContext.SaveChanges();
            _urlExpiredEventHandler.Raise(shortUrl);
            _logger.LogInformation("Tiny url is deleted {shortUrl} and url deletion event raised", shortUrl);
        }

        public void DeleteUrls(IList<string> shortUrls)
        {
            var urlTobeDeleted = _urlContext.TinyUrl.Where(d => shortUrls.Contains(d.ShortUrl)).ToList();
            if (urlTobeDeleted == null || !urlTobeDeleted.Any())
            {
                _logger.LogInformation("No url found matching for {shortUrl} hence deletion canceled", string.Join(",", shortUrls));
                return;
            }
            _urlContext.TinyUrl.RemoveRange(urlTobeDeleted);
            _urlContext.SaveChanges();
            _urlExpiredEventHandler.Raise(shortUrls);
            _logger.LogInformation("Urls {0} deleted and url deletion event raised", string.Join(",", shortUrls));
        }
    }
}
