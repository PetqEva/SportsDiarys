namespace SportsDiarys.ViewModels.Admin
{
    public class UserAdminVm
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public bool IsAdministrator { get; set; }
    }
}