using System.Net.Http;
using System.Net.Http.Json;
using VtuberDbManager.Extensions;
using VtuberDbManager.Model;

namespace VtuberDbManager.Service;

public class VtuberDbService : IVtuberDbService
{
    private const string baseUrl = "api/vtuberData";
    private readonly HttpClient _httpClient;
    public VtuberDbService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // バックエンドにアクセスする際にヘッダを追加して認識させる
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("VtuberDbMgr");
    }
    public async Task<VtuberResponse> GetAllVtubersAsync()
    {
        return await _httpClient.GetFromJsonAsync<VtuberResponse>($"{baseUrl}/vtuber") ?? new();
    }

    public async Task<VtuberResponse> GetVtubersByNameAsync(string searchName)
    {
        return await _httpClient.GetFromJsonAsync<VtuberResponse>(
            $"{baseUrl}/vtuber/name?name={searchName}") ?? new();
    }

    public async Task<VtuberResponse> GetVtubersByGroupAsync(string groupName)
    {
        return await _httpClient.GetFromJsonAsync<VtuberResponse>(
            $"{baseUrl}/vtuber/groupName?groupName={groupName}") ?? new();
    }

    public async Task<VtuberResponse> GetVtubersByFilterAsync(string name, string group, string platform)
    {
        return await _httpClient.GetFromJsonAsync<VtuberResponse>(
            $"{baseUrl}/vtuber/filter?name={name}&group={group}&platform={platform}") ?? new();
    }

    public async Task<InsertResultDTO> AddVtuberAsync(VtuberDTO dto)
    {
        var content = JsonContent.Create(dto);
        var response = await _httpClient.PostAsync($"{baseUrl}/vtuber", content);

        return await response.ToInsertResult();
    }

    public async Task<UpdateResultDTO> UpdateVtuberAsync(VtuberDTO dto)
    {
        var content = JsonContent.Create(dto);
        var response = await _httpClient.PutAsync($"{baseUrl}/vtuber", content);

        return await response.ToUpdateResult();
    }

    public async Task<DeleteResultDTO> DeleteVtuberAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{baseUrl}/vtuber?id={id}");
        return await response.ToDeleteResult();
    }

    public async Task<GroupResponse> GetAllGroupsAsync()
    {
        return await _httpClient.GetFromJsonAsync<GroupResponse>($"{baseUrl}/group") ?? new();
    }

    public async Task<InsertResultDTO> AddGroupAsync(GroupTable dto)
    {
        var content = JsonContent.Create(dto);
        var response = await _httpClient.PostAsync($"{baseUrl}/group", content);

        return await response.ToInsertResult();
    }

    public async Task<UpdateResultDTO> UpdateGroupAsync(GroupTable dto)
    {
        var content = JsonContent.Create(dto);
        var response = await _httpClient.PutAsync($"{baseUrl}/group", content);

        return await response.ToUpdateResult();
    }

    public async Task<DeleteResultDTO> DeleteGroupAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{baseUrl}/group?id={id}");
        return await response.ToDeleteResult();
    }

    public async Task<PlatformResponse> GetAllPlatformsAsync()
    {
        return await _httpClient.GetFromJsonAsync<PlatformResponse>($"{baseUrl}/platform") ?? new();
    }

    public async Task<InsertResultDTO> AddPlatformAsync(PlatformTable dto)
    {
        var content = JsonContent.Create(dto);
        var response = await _httpClient.PostAsync($"{baseUrl}/platform", content);

        return await response.ToInsertResult();
    }

    public async Task<UpdateResultDTO> UpdatePlatformAsync(PlatformTable dto)
    {
        var content = JsonContent.Create(dto);
        var response = await _httpClient.PutAsync($"{baseUrl}/platform", content);

        return await response.ToUpdateResult();
    }

    public async Task<DeleteResultDTO> DeletePlatformAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{baseUrl}/platform?id={id}");
        return await response.ToDeleteResult();
    }

    public async Task<long> GetCountAsync()
    {
        return await _httpClient.GetFromJsonAsync<long>($"{baseUrl}/visitorCount");
    }
}
