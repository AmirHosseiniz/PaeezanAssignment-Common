using Common.Entity;

namespace Common.DTO;

public class UserDto : IDBEntity
{
    public int Id { get; set; }
    public string user_id { get; set; }
    public int wins { get; set; }
    public string uuid { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}