using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DocurainApiClient.Services
{
    /// <summary>
    /// Docurainの管理APIクライアントです。
    /// </summary>
    public class DocurainManagementApiClient : IDocurainManagementApiClient
    {
        public const string HttpClientName = nameof(DocurainManagementApiClient);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// <see cref="DocurainManagementApiClient"/>を初期化します。
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public DocurainManagementApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
        }

        /// <summary>
        /// Docurain上に保存されたテンプレートの一覧を取得します。
        /// </summary>
        /// <returns></returns>
        public async Task<List<Template>> GetTemplatesAsync()
        {
            var response = await CreateHttpClient().GetAsync("/api/template/");
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
            var responseText = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Template>>(responseText, _jsonSerializerSettings);
        }

        /// <summary>
        /// テンプレートを取得します。
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public async Task<Stream> GetTemplateAsync(string templateName)
        {
            return await CreateHttpClient().GetStreamAsync($"/api/template/{templateName}");
        }

        /// <summary>
        /// テンプレートを保存します。
        /// </summary>
        /// <param name="localTemplatePath">ローカルのテンプレートExcelファイル</param>
        /// <param name="remoteTemplateName">Docurain上のテンプレート名</param>
        /// <param name="comment">コメント</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SaveTemplateAsync(string localTemplatePath, string remoteTemplateName, string comment)
        {
            if (string.IsNullOrEmpty(localTemplatePath))
                throw new ArgumentException(nameof(localTemplatePath));
            if (!File.Exists(localTemplatePath))
                throw new FileNotFoundException(localTemplatePath);
            if (string.IsNullOrEmpty(remoteTemplateName))
                throw new ArgumentNullException(nameof(remoteTemplateName));

            var contentType = (Path.GetExtension(localTemplatePath).ToLower() == ".xlsx")
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/vnd.ms-excel";

            await using var fileStream = File.OpenRead(localTemplatePath);
            using var multipartContent = BuildSaveTemplateRequest(remoteTemplateName, fileStream, contentType, comment);
            return await CreateHttpClient().PostAsync($"/api/template/{remoteTemplateName}", multipartContent);
        }

        /// <summary>
        /// テンプレートを削除します。
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteTemplate(string templateName)
        {
            return await CreateHttpClient().DeleteAsync($"/api/template/{templateName}");
        }

        private MultipartFormDataContent BuildSaveTemplateRequest(string fileName, Stream stream, string contentType, string comment)
        {
            var multipart = new MultipartFormDataContent();
            {
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = fileName
                };
                multipart.Add(fileContent);
            }
            {
                var commentContent = new StringContent(comment);
                commentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "comment"
                };
                multipart.Add(commentContent);
            }
            return multipart;
        }
        private HttpClient CreateHttpClient()
        {
            return _httpClientFactory.CreateClient(HttpClientName);
        }
    }
}