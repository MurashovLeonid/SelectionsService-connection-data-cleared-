namespace Superbrands.Selection.WebApi.Models
{
    public class ExternalApplicationRole
    {
        public int ExternalApplicationId { get; set; }

        public virtual ExternalApplication ExternalApplication { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }

    }
}