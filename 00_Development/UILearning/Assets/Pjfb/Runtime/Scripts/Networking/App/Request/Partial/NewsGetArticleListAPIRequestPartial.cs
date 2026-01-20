using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class NewsGetArticleListAPIRequest : AppAPIRequestBase<NewsGetArticleListAPIPost, NewsGetArticleListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( NewsGetArticleListAPIResponse response ) {
        }
    }
}
