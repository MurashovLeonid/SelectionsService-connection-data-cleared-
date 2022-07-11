using System.Collections.Generic;

namespace Superbrands.Selection.WebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProviderData { get; set; }

        public int? DepartmentId { get; set; }
        
        public bool IsLead { get; set; }
        
        public string DisplayName { get; set; }
        
        public virtual ICollection<UserRole> UserRoles { get; set; }
        
        public virtual Department Department { get; set; }
    }
}