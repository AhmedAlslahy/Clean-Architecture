using Domain.Common;

namespace Domain.Entities.Identity
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = string.Empty; //"permissions:roles:create"
        public string Group { get; set; } = string.Empty;//"Role"

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}