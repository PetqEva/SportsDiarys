namespace SportsDiarys.ViewModels.Admin
{
    public class UserAdminVm
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
