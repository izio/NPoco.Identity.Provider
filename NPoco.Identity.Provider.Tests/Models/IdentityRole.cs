using NPoco.Identity.Provider.Interfaces;

namespace NPoco.Identity.Provider.Tests.Modesl
{
    [TableName("AspNetRoles")]
    public class IdentityRole : IIdentityRole<int>
    {
        public int Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }
    }
}