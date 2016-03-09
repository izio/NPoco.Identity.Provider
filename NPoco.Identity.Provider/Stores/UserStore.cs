using NPoco.Identity.Provider.Interfaces;
using NPoco.Identity.Provider.Extensions;
using NPoco.Identity.Provider.Enums;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NPoco.Identity.Provider.Stores
{
    public class UserStore<TUser, TRoles, TClaims, TLogins, TRole, TKey> : IUserStore<TUser, TKey>,
                                                IUserLoginStore<TUser, TKey>,
                                                IUserClaimStore<TUser, TKey>,
                                                IUserRoleStore<TUser, TKey>,
                                                IUserPasswordStore<TUser, TKey>,
                                                IUserSecurityStampStore<TUser, TKey>,
                                                IUserEmailStore<TUser, TKey>,
                                                IUserPhoneNumberStore<TUser, TKey>,
                                                IUserTwoFactorStore<TUser, TKey>,
                                                IUserLockoutStore<TUser, TKey>
                                                where TUser : class, IIdentityUser<TKey> where TRoles : class, IIdentityRoles<TKey>, new() where TClaims : class, IIdentityClaim<TKey>, new() where TLogins : class, IIdentityLogin<TKey>, new() where TRole : class, IIdentityRole<TKey>, new()
    {
        private Database _database;

        public UserStore(string connectionName)
        {
            _database = new Database(connectionName);
        }

        public UserStore(string connectionString, DatabaseType type)
        {
            _database = new Database(connectionString, type);
        }

        #region IUserStore

        public Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            _database.Insert(user);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                _database.Delete(user);
            }

            return Task.FromResult<Object>(null);
        }

        public Task<TUser> FindByIdAsync(TKey userId)
        {
            if (userId == null)
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            var user = _database.FirstOrDefault<TUser>("WHERE Id = @0", userId);

            if (user != null)
            {
                return Task.FromResult(user);
            }

            return Task.FromResult<TUser>(null);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            var user = _database.FirstOrDefault<TUser>("WHERE Username = @0", userName);

            return Task.FromResult(user);
        }

        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            _database.Update(user);

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            if (_database != null)
            {
                _database.Dispose();
                _database = null;
            }
        }

        #endregion

        #region IUserLoginStore

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("Null or empty argument: login");
            }

            _database.Insert(new TLogins() { UserId = user.Id, LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey });

            return Task.FromResult<object>(null);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("Null or empty argument: login");
            }

            var query = _database.BuildRelatedSelectQuery<TUser, TLogins>(new TableProperties("Id", "[User]", new List<string> { "*" } ), new TableProperties("UserId", "[Login]"), Join.LeftJoin, "WHERE [Login].LoginProvider = @0 AND [Login].ProviderKey = @1");

            var user = _database.FirstOrDefault<TUser>(query, login.LoginProvider, login.ProviderKey);

            if (user != null)
            {
                return Task.FromResult(user);
            }

            return Task.FromResult<TUser>(null);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            IEnumerable<TLogins> logins = _database.Query<TLogins>("WHERE UserId = @0", user.Id);

            if (logins != null)
            {
                return Task.FromResult<IList<UserLoginInfo>>(logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList());
            }

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("Null or empty argument: login");
            }

            _database.Delete<TLogins>("WHERE UserId = @0 AND LoginProvider = @1 AND ProviderKey = @2", user.Id, login.LoginProvider, login.ProviderKey);

            return Task.FromResult<Object>(null);
        }

        #endregion

        #region IUserClaimStore

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            _database.Insert(new TClaims() { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });

            return Task.FromResult<object>(null);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            var claims = _database.Query<TClaims>("WHERE UserId = @0", user.Id);

            return Task.FromResult<IList<Claim>>(claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList());
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("Null or empty argument: claim");
            }

            _database.Delete<TClaims>("WHERE UserId = @0 AND ClaimType = @1 AND ClaimValue = @2", user.Id, claim.Type, claim.Value);

            return Task.FromResult<object>(null);
        }

        #endregion

        #region IUserRoleStore

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Null or empty argument: roleName.");
            }

            var role = _database.SingleOrDefault<TRole>("WHERE Name = @0", roleName);

            if (role != null)
            {
                _database.Insert(new TRoles() { UserId = user.Id, RoleId = role.Id });
            }

            return Task.FromResult<object>(null);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException("Null or empty argument: roleName");
            }

            var query = _database.BuildRelatedDeleteQuery<TRoles, TRole>(new TableProperties("RoleId", "[Roles]"), new TableProperties("Id", "[Role]"), Join.InnerJoin, "WHERE [Roles].UserId = @0 AND [Role].Name = @1");

            _database.Execute(query, user.Id, roleName);

            return Task.FromResult<object>(null);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            var query = _database.BuildRelatedSelectQuery<TRole, TRoles>(new TableProperties("Id", "[Role]", new List<string> { "Name" }), new TableProperties("RoleId", "[Roles]"), Join.LeftJoin, "WHERE [Roles].UserId = @0");
            var roles = _database.Fetch<string>(query, user.Id);

            if (roles != null)
            {
                return Task.FromResult<IList<string>>(roles);
            }

            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("Null or empty argument: user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("Null or empty argument: roleName");
            }

            var query = _database.BuildRelatedSelectQuery<TRole, TRoles>(new TableProperties("Id", "[Role]", new List<string> { "Name" }), new TableProperties("RoleId", "[Roles]"), Join.LeftJoin, "WHERE [Roles].UserId = @0 AND [Role].Name = @1");
            var role = _database.FirstOrDefault<string>(query, user.Id, roleName);

            if (string.IsNullOrEmpty(role))
            {
                return Task.FromResult(false); 
            }

            return Task.FromResult(true);
        }

        #endregion

        #region IUserPasswordStore

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentException("Null or empty argument: passwordHash");
            }

            user.PasswordHash = passwordHash;

            _database.Update(user);

            return Task.FromResult<Object>(null);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            var identity = _database.SingleOrDefaultById<TUser>(user.Id);

            return Task.FromResult(identity.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            var passwordHash = _database.SingleOrDefaultById<TUser>(user.Id).PasswordHash;

            return Task.FromResult(!string.IsNullOrEmpty(passwordHash));
        }

        #endregion

        #region IUserSecurityStampStore

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            if (string.IsNullOrEmpty(stamp))
            {
                throw new ArgumentException("Null or empty argument: stamp");
            }

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region IUserEmailStore

        public Task SetEmailAsync(TUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Null or empty argument: email");
            }

            user.Email = email;

            _database.Update(user);

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            user.EmailConfirmed = confirmed;

            _database.Update(user);

            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Null or empty argument: email");
            }

            TUser user = _database.FirstOrDefault<TUser>("WHERE Email = @0", email);

            return Task.FromResult(user);
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentException("Null or empty argument: phoneNumber");
            }

            user.PhoneNumber = phoneNumber;

            _database.Update(user);

            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            user.PhoneNumberConfirmed = confirmed;

            _database.Update(user);

            return Task.FromResult(0);
        }

        #endregion

        #region IUserTwoFactorStore

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            user.TwoFactorEnabled = enabled;

            _database.Update(user);

            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: ");
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }


        #endregion

        #region IUserLockoutStore

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            return Task.FromResult(user.LockoutEndDateUtc.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) : new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;

            _database.Update(user);

            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            user.AccessFailedCount++;

            _database.Update(user);

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            user.AccessFailedCount = 0;

            _database.Update(user);

            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentException("Null or empty argument: user");
            }

            user.LockoutEnabled = enabled;

            _database.Update(user);

            return Task.FromResult(0);
        }

        #endregion
    }
}