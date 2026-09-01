using VtuberDbManager.Model;

namespace VtuberDbManager.Service;

public interface IVtuberDbService
{
    Task<VtuberResponse> GetAllVtubersAsync();
    Task<VtuberResponse> GetVtubersByNameAsync(string searchName);
    Task<VtuberResponse> GetVtubersByGroupAsync(string groupName);
    Task<VtuberResponse> GetVtubersByFilterAsync(string name, string group, string platform);
    Task<InsertResultDTO> AddVtuberAsync(VtuberDTO dto);
    Task<UpdateResultDTO> UpdateVtuberAsync(VtuberDTO dto);
    Task<DeleteResultDTO> DeleteVtuberAsync(int id);
    Task<GroupResponse> GetAllGroupsAsync();
    Task<InsertResultDTO> AddGroupAsync(GroupTable dto);
    Task<UpdateResultDTO> UpdateGroupAsync(GroupTable dto);
    Task<DeleteResultDTO> DeleteGroupAsync(int id);
    Task<PlatformResponse> GetAllPlatformsAsync();
    Task<InsertResultDTO> AddPlatformAsync(PlatformTable dto);
    Task<UpdateResultDTO> UpdatePlatformAsync(PlatformTable dto);
    Task<DeleteResultDTO> DeletePlatformAsync(int id);
    Task<long> GetCountAsync();
}
