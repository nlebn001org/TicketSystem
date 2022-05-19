namespace MinimalWebAPI.Data
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ChangedTime { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public List<Note>? Notes { get; set; }
    }
}
