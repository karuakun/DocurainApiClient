namespace DocurainApiClient.Settings
{
    /// <summary>
    /// Docurainの接続設定を保持します。
    /// </summary>
    public class DocurainSettings
    {
        public const string SectionName = nameof(DocurainSettings);

        /// <summary>
        /// 環境名を取得または設定します。
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// シークレットキーを取得または設定します。
        /// </summary>
        public string SecretKey { get; set; }

    }
}
