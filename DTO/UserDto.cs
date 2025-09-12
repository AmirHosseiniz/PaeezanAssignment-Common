using Common.Entity;

namespace Common.DTO;

public class UserDto : IDBEntity
{
    public int Id { get; set; }

    public string user_id { get; set; }
    public int? wins { get; set; }
}