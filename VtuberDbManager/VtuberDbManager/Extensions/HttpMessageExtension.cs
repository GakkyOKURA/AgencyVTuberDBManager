using System.Net.Http;
using System.Net.Http.Json;
using VtuberDbManager.Model;

namespace VtuberDbManager.Extensions;

internal static class HttpMessageExtension
{
    internal static async Task<InsertResultDTO> ToInsertResult(this HttpResponseMessage response)
    {
        if(response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<InsertResultDTO>();
            return new InsertResultDTO
            {
                Message = result?.Message ?? "",
                Id = result?.Id ?? 0,
                IsSuccess = true
            };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new InsertResultDTO
            {
                Message = errorMessage,
                IsSuccess = false
            };
        }
    }

    internal static async Task<UpdateResultDTO> ToUpdateResult(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UpdateResultDTO>();
            return new UpdateResultDTO
            {
                Message = result?.Message ?? "",
                IsSuccess = true
            };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new UpdateResultDTO
            {
                Message = errorMessage,
                IsSuccess = false
            };
        }
    }

    internal static async Task<DeleteResultDTO> ToDeleteResult(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<DeleteResultDTO>();
            return new DeleteResultDTO
            {
                Message = result?.Message ?? "",
                IsSuccess = true
            };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new DeleteResultDTO
            {
                Message = errorMessage,
                IsSuccess = false
            };
        }
    }
}
