using System.IO;
using System.Threading.Tasks;

/// <summary>
/// HTTPリクエストのコマンド組み立てと送信を行うクラス。
/// </summary>
public class CommandComposer
{
    private readonly IHttpService _httpService;

    /// <summary>HTTPメソッド（GET/POST/PUT/DELETE）</summary>
    public string Method { get; private set; } = "GET";
    /// <summary>リクエストパス</summary>
    public string Path { get; private set; } = string.Empty;
    /// <summary>Content-Type（例: "application/json"）</summary>
    public string MediaType { get; private set; } = "text/plain";
    /// <summary>リクエストボディ</summary>
    public string BodyText { get; private set; } = string.Empty;
    /// <summary>組み立て済みリクエストURL</summary>
    public string RequestUrl { get; private set; } = string.Empty;
    /// <summary>認証ユーザー名</summary>
    public string? User { get; private set; }
    /// <summary>認証パスワード</summary>
    public string? Password { get; private set; }

    /// <summary>
    /// CommandComposerの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="httpService">HTTPサービスインスタンス</param>
    public CommandComposer(IHttpService httpService)
    {
        _httpService = httpService;
    }

    /// <summary>
    /// detailを";"で区切り、Method, Path, MediaType, BodyTextに分解してメンバー変数に設定します。
    /// </summary>
    /// <param name="detail">例: "POST;api/values;application/json;{&quot;id&quot;:1}"</param>
    public void ParseDetail(string? detail)
    {
        Method = "GET";
        Path = string.Empty;
        MediaType = "text/plain";
        BodyText = string.Empty;

        if (!string.IsNullOrEmpty(detail))
        {
            var parts = detail.Split(';');
            if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0]))
                Method = parts[0].Trim().ToUpper();
            if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
                Path = parts[1].Trim();
            if (parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2]))
                MediaType = parts[2].Trim();
            if (parts.Length > 3)
            {
                var body = parts[3].Trim();
                if (body.StartsWith("@"))
                {
                    var fileName = body.Substring(1);
                    if (File.Exists(fileName))
                    {
                        BodyText = File.ReadAllText(fileName);
                    }
                    else
                    {
                        BodyText = string.Empty;
                    }
                }
                else
                {
                    BodyText = body;
                }
            }
        }
    }

    /// <summary>
    /// URL文字列からユーザー名とパスワードを切り出し、メンバー変数に設定します。
    /// </summary>
    /// <param name="url">"http://example.com;user1;pass1" のような形式</param>
    /// <returns>ベースURL</returns>
    public string ParseUserAndPassword(string url)
    {
        User = null;
        Password = null;

        if (string.IsNullOrEmpty(url))
            return string.Empty;

        var urlParts = url.Split(';');
        var baseUrl = urlParts[0].Trim();

        if (urlParts.Length > 1)
            User = urlParts[1].Trim();
        if (urlParts.Length > 2)
            Password = urlParts[2].Trim();

        return baseUrl;
    }

    /// <summary>
    /// URLとdetailからリクエストURLを生成し、ユーザー名・パスワードも切り出してメンバー変数に設定します。
    /// </summary>
    /// <param name="url">ベースURL（例: "http://example.com;user1;pass1"）</param>
    /// <param name="detail">リクエスト詳細（例: "POST;api/values;application/json;{&quot;id&quot;:1}"）</param>
    /// <returns>組み立て済みリクエストURL</returns>
    public string ComposeRequestUrl(string url, string? detail)
    {
        ParseDetail(detail);

        var baseUrl = ParseUserAndPassword(url);

        if (string.IsNullOrEmpty(baseUrl))
        {
            RequestUrl = string.Empty;
            return RequestUrl;
        }

        RequestUrl = baseUrl;
        if (!string.IsNullOrEmpty(Path))
        {
            if (!RequestUrl.EndsWith("/")) RequestUrl += "/";
            RequestUrl += Path;
        }
        return RequestUrl;
    }

    /// <summary>
    /// メンバー変数のMethod・RequestUrl・User・Password・MediaType・BodyTextでHttpServiceを呼び出します。
    /// </summary>
    /// <returns>HTTPレスポンス文字列</returns>
    public async Task<string?> CallHttpServiceAsync()
    {
        switch (Method)
        {
            case "GET":
                return await _httpService.GetAsync(RequestUrl, User, Password);
            case "POST":
                return await _httpService.PostAsync(RequestUrl, BodyText, MediaType, User, Password);
            case "PUT":
                return await _httpService.PutAsync(RequestUrl, BodyText, MediaType, User, Password);
            case "DELETE":
                return await _httpService.DeleteAsync(RequestUrl, User, Password);
            default:
                return $"未対応のメソッド: {Method}";
        }
    }
}