using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DocurainApiClient.Services
{
    /// <summary>
    /// Docurainの管理APIクライアントです。
    /// </summary>
    public interface IDocurainManagementApiClient
    {
        /// <summary>
        /// テンプレートの一覧を取得します。
        /// </summary>
        /// <returns></returns>
        Task<List<Template>> GetTemplatesAsync();
        /// <summary>
        /// テンプレートを取得します。
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        Task<Stream> GetTemplateAsync(string templateName);
        /// <summary>
        /// テンプレートを保存します。
        /// </summary>
        /// <param name="localTemplatePath">ローカルのテンプレートExcelファイル</param>
        /// <param name="remoteTemplateName">Docurain上のテンプレート名</param>
        /// <param name="comment">コメント</param>
        /// <returns></returns>
        Task<HttpResponseMessage> SaveTemplateAsync(string localTemplatePath, string remoteTemplateName, string comment);
        /// <summary>
        /// テンプレートを削除します。
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteTemplate(string templateName);
    }

    public class Template
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime LastModified { get; set; }
    }
}
