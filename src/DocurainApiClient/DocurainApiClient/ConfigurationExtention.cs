using System;
using DocurainApiClient.Services;
using DocurainApiClient.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DocurainApiClient
{
    public static class ConfigurationExtention
    {
        /// <summary>
        /// 構成に<see cref="DocurainManagementApiClient"/>を追加します。
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="docurainSettings"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddDocurainManagementApiClient(this IServiceCollection serviceCollection, DocurainSettings docurainSettings)
        {
            serviceCollection.Configure<DocurainSettings>(s =>
            {
                s.ApiUrl = docurainSettings.ApiUrl;
                s.SecretKey = docurainSettings.SecretKey;
            });
            serviceCollection.AddTransient<IDocurainManagementApiClient, DocurainManagementApiClient>();
            return serviceCollection.AddHttpClient(DocurainManagementApiClient.HttpClientName, c =>
            {
                c.BaseAddress = new Uri(docurainSettings.ApiUrl);
                c.DefaultRequestHeaders.Add("Authorization", $"token {docurainSettings.SecretKey}");
            });
        }
    }
}
