using System.Collections.Generic;

namespace Superbrands.Selection.WebApi.Models
{
    public class Role
    {
        public bool IsBelongsToUser { get; set; }
        public Module Module { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } 

        public virtual ICollection<ExternalApplicationRole> ExternalApplicationRoles { get; set; }

        public virtual ICollection<SectionPermission> SectionPermissions { get; set; }

        public virtual ICollection<ResourcePermission> ResourcePermissions { get; set; }

    }
    
    public enum Module
    {
        PIM = 1,
        Administration = 2,
        B2B = 3,
        Calculator = 4,
        Seasons = 5,
        Deals = 6
    }

}