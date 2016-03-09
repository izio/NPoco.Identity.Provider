using NPoco.Identity.Provider.Interfaces;

namespace NPoco.Identity.Provider.Tests.Modesl
{
    [TableName("AspNetUserClaims")]
    public class IdentityClaim : IIdentityClaim<int>
    {
        public string ClaimType
        {
            get; set;
        }

        public string ClaimValue
        {
            get; set;
        }

        public int Id
        {
            get; set;
        }

        public int UserId
        {
            get; set;
        }
    }
}
