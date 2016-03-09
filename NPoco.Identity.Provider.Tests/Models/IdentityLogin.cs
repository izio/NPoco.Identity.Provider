using NPoco.Identity.Provider.Interfaces;

namespace NPoco.Identity.Provider.Tests.Modesl
{
    [TableName("AspNetUserLogins")]
    public class IdentityLogin : IIdentityLogin<int>
    {
        public string LoginProvider
        {
            get; set;
        }

        public string ProviderKey
        {
            get; set;
        }

        public int UserId
        {
            get; set;
        }
    }
}
