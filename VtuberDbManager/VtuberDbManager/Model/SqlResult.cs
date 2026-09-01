namespace VtuberDbManager.Model;

/// <summary>
/// データ更新の結果
/// </summary>
public class UpdateResultDTO
{
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; }
}

/// <summary>
/// データ追加の結果
/// </summary>
public class InsertResultDTO
{
    public string Message { get; set; } = "";
    public int Id { get; set; }
    public bool IsSuccess { get; set; }
}

/// <summary>
/// データ削除の結果
/// </summary>
public class DeleteResultDTO
{
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; }
}
