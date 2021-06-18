using System;
namespace ViewModels.Views
{
    public class UserView
    {
        public Guid Id { get; set; }
        
        public string Username { get; set; }
        
        public RoleView Role { get; set; }
    }
}