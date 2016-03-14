using NPoco.Identity.Provider.Stores;
using NPoco.Identity.Provider.Tests.Models;
using System;
using Xunit;

namespace NPoco.Identity.Provider.Tests
{
    public class DatabaseFixture : IDisposable
    {
        readonly string connString = "Data Source=DELL-XPS13\\SQLEXPRESS;Initial Catalog=Membership;Integrated Security=True;";

        public UserStore<IdentityUser, IdentityRoles, IdentityClaim, IdentityLogin, IdentityRole, int> UserStore { get; private set; }

        public RoleStore<IdentityRole, int> RoleStore { get; private set; }

        public Database Database { get; private set; }

        public DatabaseFixture()
        {
            UserStore = new UserStore<IdentityUser, IdentityRoles, IdentityClaim, IdentityLogin, IdentityRole, int>(connString, DatabaseType.SqlServer2012);
            RoleStore = new RoleStore<IdentityRole, int>(connString, DatabaseType.SqlServer2012);
            Database = new Database(connString, DatabaseType.SqlServer2012);
        }

        public void Dispose()
        {
            UserStore.Dispose();
            RoleStore.Dispose();
            Database.Dispose();
        }
    }

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
