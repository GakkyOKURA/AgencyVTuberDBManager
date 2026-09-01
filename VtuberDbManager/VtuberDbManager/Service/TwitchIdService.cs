using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using VtuberDbManager.Config;

namespace VtuberDbManager.Service;

public class TwitchIdService : ITwitchIdService
{
    private readonly HttpClient _httpClient;
    private readonly TwitchApiSettings _settings;
    private static string? _cachedToken;

    public TwitchIdService(HttpClient httpClient, IOptions<TwitchApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    // アプリ用トークンを取得（Client Credentials Flow）
    private async Task<string> GetAccessTokenAsync()
    {
        if (_cachedToken is not null)
        {
            return _cachedToken;
        }

        var url =
            "https://id.twitch.tv/oauth2/token" +
            $"?client_id={_settings.ClientId}" +
            $"&client_secret={_settings.ClientSecret}" +
            "&grant_type=client_credentials";

        var res = await _httpClient.PostAsync(url, null);
        var json = await res.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(json).RootElement;

        _cachedToken = doc.GetProperty("access_token").GetString();

        return _cachedToken!;
    }

    public async Task<string?> GetUserIdFromLoginIdAsync(string loginId)
    {
        var token = await GetAccessTokenAsync();

        var url = $"https://api.twitch.tv/helix/users?login={loginId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Client-ID", _settings.ClientId);
        request.Headers.Add("Authorization", $"Bearer {token}");

        using var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var dataArray = doc.RootElement.GetProperty("data");
        if(dataArray.GetArrayLength() <= 0)
        {
            return null;
        }

        return dataArray[0].GetProperty("id").GetString();
    }
}
