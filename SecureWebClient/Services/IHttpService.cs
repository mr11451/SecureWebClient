using System;
using System.Threading.Tasks;

/// <summary>
/// HTTP通信のためのサービスインターフェース。
/// </summary>
public interface IHttpService : IDisposable
{
    /// <summary>
    /// 指定したURLにGETリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエスト先のURL。</param>
    /// <param name="user">ベーシック認証のユーザー名（省略可）。</param>
    /// <param name="password">ベーシック認証のパスワード（省略可）。</param>
    /// <returns>レスポンス文字列。</returns>
    Task<string?> GetAsync(string url, string? user = null, string? password = null);

    /// <summary>
    /// 指定したURLにPOSTリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエスト先のURL。</param>
    /// <param name="content">送信するコンテンツ。</param>
    /// <param name="mediaType">Content-Type（例: "application/json"）。</param>
    /// <param name="user">ベーシック認証のユーザー名（省略可）。</param>
    /// <param name="password">ベーシック認証のパスワード（省略可）。</param>
    /// <returns>レスポンス文字列。</returns>
    Task<string?> PostAsync(string url, string content, string mediaType = "text/plain", string? user = null, string? password = null);

    /// <summary>
    /// 指定したURLにPUTリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエスト先のURL。</param>
    /// <param name="content">送信するコンテンツ。</param>
    /// <param name="mediaType">Content-Type（例: "application/json"）。</param>
    /// <param name="user">ベーシック認証のユーザー名（省略可）。</param>
    /// <param name="password">ベーシック認証のパスワード（省略可）。</param>
    /// <returns>レスポンス文字列。</returns>
    Task<string?> PutAsync(string url, string content, string mediaType = "text/plain", string? user = null, string? password = null);

    /// <summary>
    /// 指定したURLにDELETEリクエストを送信します。
    /// </summary>
    /// <param name="url">リクエスト先のURL。</param>
    /// <param name="user">ベーシック認証のユーザー名（省略可）。</param>
    /// <param name="password">ベーシック認証のパスワード（省略可）。</param>
    /// <returns>レスポンス文字列。</returns>
    Task<string?> DeleteAsync(string url, string? user = null, string? password = null);
}