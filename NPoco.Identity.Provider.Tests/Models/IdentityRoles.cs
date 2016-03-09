using NPoco.Identity.Provider.Interfaces;

namespace NPoco.Identity.Provider.Tests.Modesl
{
    [TableName("AspNetUserRoles")]
    public class IdentityRoles : IIdentityRoles<int>
    {
        public int RoleId
        {
            get; set;
        }

        public int UserId
        {
            get; set;
        }
    }
}