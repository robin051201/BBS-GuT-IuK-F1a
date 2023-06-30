using Microsoft.AspNetCore.Identity;

namespace PizzaWelt.Models
{
    public class ApplicationUserRoles : IdentityRole
    {
        public ICollection<UserAccounts> UserAccounts { get; } = new List<UserAccounts>();
    }
}