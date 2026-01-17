using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http.Headers;

/// <summary>
/// HTTP通信のためのサービス実装クラス。
/// </summary>
public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// HttpServiceの新しいインスタンスを初期化します。
    /// </summary>
    public HttpService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// ベーシック認証ヘッダーを設定します。
    /// </summary>
    /// <param name="user">ユーザー名</param>
    /// <param name="password">パスワード</param>
    private void SetBasicAuthHeader(string? user, string? password)
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
        {
            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
        }
    }

    /// <summary>
    /// 指定したURLにGETリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエスト先のURL。</param>
    /// <param name="user">ベーシック認証のユーザー名（省略可）。</param>
    /// <param name="password">ベーシック認証のパスワード（省略可）。</param>
    /// <returns>レスポンス文字列。</returns>
    public async Task<string?> GetAsync(string url, string? user = null, string? password = null)
    {
        SetBasicAuthHeader(user, password);
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// 指定したURLにPOSTリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエスト先のURL。</param>
    /// <param name="content">送信するコンテンツ。</param>
    /// <param name="mediaType">Content-Type（例: "application/json"）。</param>
    /// <param name="user">ベーシック認証のユーザー名（省略可）。</param>
    /// <param name="password">ベーシック認証のパスワード（省略可）。</param>
    /// <returns>レスポンス文字列。</returns>
    public async Task<string?> PostAsync(string url, string content, string mediaType = "text/plain", string? user = null, string? password = null)
    {
        SetBasicAuthHeader(user, password);
        using var httpContent = new StringContent(content, Encoding.UTF8, mediaType);
        var response = await _httpClient.PostAsync(url, httpContent);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// 指定したURLにPUTリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエスト先のURL。</param>
    /// <param name="content">送信するコンテンツ。</param>
    /// <param name="mediaType">Content-Type（例: "application/json"）。</param>
    /// <param name="user">ベーシック認証のユーザー名（省略可）。</param>
    /// <param name="password">ベーシック認証のパスワード（省略可）。</param>
    /// <returns>レスポンス文字列。</returns>
    public async Task<string?> PutAsync(string url, string content, string mediaType = "text/plain", string? user = null, string? password = null)
    {
        SetBasicAuthHeader(user, password);
        using var httpContent = new StringContent(content, Encoding.UTF8, mediaType);
        var response = await _httpClient.PutAsync(url, httpContent);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// 指定したURLにDELETEリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエスト先のURL。</param>
    /// <param name="user">ベーシック認証のユーザー名（省略可）。</param>
    /// <param name="password">ベーシック認証のパスワード（省略可）。</param>
    /// <returns>レスポンス文字列。</returns>
    public async Task<string?> DeleteAsync(string url, string? user = null, string? password = null)
    {
        SetBasicAuthHeader(user, password);
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// 使用しているリソースを解放します。
    /// </summary>
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}