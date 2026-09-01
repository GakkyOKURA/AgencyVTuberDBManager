namespace VtuberDbManager.Service;

public interface ITwitchIdService
{
    /// <summary>
    /// twitch で user_login から user_id を取得する。
    /// </summary>
    /// <param name="loginId"></param>
    /// <returns></returns>
    Task<string?> GetUserIdFromLoginIdAsync(string loginId);
}
