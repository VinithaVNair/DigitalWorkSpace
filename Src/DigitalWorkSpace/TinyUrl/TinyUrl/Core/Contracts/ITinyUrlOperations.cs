using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using UrlManaging.Core.Model;

namespace UrlManaging.Core.Contracts
{
    public interface ITinyUrlOperations
    {
        TinyUrl GetUrl(string shortName);
        IList<TinyUrl> GetAllUrl();
        TinyUrl CreateTinyUrl(UrlInfo urlInfo);
        void LinkUrl(string url);
        void UnLinkUrl(string url);
        
        IList<string> UrlsExpired(DateTime date);
        void DeleteUrls(string shortUrl);

        void DeleteUrls(IList<string> shortUrls);
    }
}
