using Microsoft.AspNet.Identity;
using NPoco.Identity.Provider.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NPoco.Identity.Provider.Stores
{
    public class RoleStore<TRole, TKey> : IQueryableRoleStore<TRole, TKey> where TRole : IIdentityRole<TKey>
    {
        private Database _database;

        public RoleStore(string connectionName)
        {
            _database = new Database(connectionName);
        }

        public RoleStore(string connectionString, DatabaseType type)
        {
            _database = new Database(connectionString, type);
        }

        #region IQueryableRoleStore

        public IQueryable<TRole> Roles
        {
            get
            {
                return _database.Fetch<TRole>() as IQueryable<TRole>;
            }
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("Null or empty argument: role");
            }

            _database.Insert(role);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("Null or empty argument: role");
            }

            _database.Delete(role);

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public Task<TRole> FindByIdAsync(TKey roleId)
        {
            if (roleId == null)
            {
                throw new ArgumentException("Null or empty argument: roleId");
            }

            var role = _database.FirstOrDefault<TRole>("WHERE Id = @0", roleId);

            return Task.FromResult(role);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Null or empty argument: roleName");
            }

            var role = _database.FirstOrDefault<TRole>("WHERE Name = @0", roleName);

            return Task.FromResult(role);
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("Null or empty argument: role");
            }

            _database.Update(role);

            return Task.FromResult<object>(null);
        }

        #endregion
    }
}