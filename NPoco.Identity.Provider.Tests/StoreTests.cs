using Microsoft.AspNet.Identity;
using NPoco.Identity.Provider.Tests.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NPoco.Identity.Provider.Tests
{
    [Collection("Database collection")]
    public class StoreTests : IDisposable
    {
        private DatabaseFixture _fixture;
        private ITestOutputHelper _output;

        public StoreTests(DatabaseFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;

            Setup();
        }

        private void Setup()
        {

            _fixture.Database.Execute(@"DELETE FROM AspNetUsers
                                            DELETE FROM AspNetUserLogins
                                            DELETE FROM AspNetUserRoles
                                            DELETE FROM AspNetUserClaims
                                            DELETE FROM AspNetRoles");
        }

        [Fact]
        public async Task IUserStoreTests()
        {
            // create test user
            var user = new IdentityUser() { UserName = "test user" };
            await _fixture.UserStore.CreateAsync(user);

            // get user by id
            var userById = await _fixture.UserStore.FindByIdAsync(user.Id);

            Assert.NotNull(userById);
            Assert.Equal(user.Id, userById.Id);

            // get user by name
            var userByName = await _fixture.UserStore.FindByNameAsync(user.UserName);

            Assert.NotNull(userByName);
            Assert.Equal(user.Id, userByName.Id);

            // update user
            user.UserName = "test user updated";
            await _fixture.UserStore.UpdateAsync(user);

            //get updated user
            var updatedUser = await _fixture.UserStore.FindByNameAsync(user.UserName);

            Assert.NotNull(updatedUser);
            Assert.Equal(user.Id, updatedUser.Id);

            //delete user
            await _fixture.UserStore.DeleteAsync(user);
            var deletedUser = await _fixture.UserStore.FindByIdAsync(user.Id);

            Assert.Equal(deletedUser, null);
        }

        [Fact]
        public async Task IUserLoginStoreTests()
        {
            //create user
            var user = new IdentityUser() { UserName = "test user" };
            await _fixture.UserStore.CreateAsync(user);

            //add login
            var userLogin = new UserLoginInfo("MyOauthProvider", Guid.NewGuid().ToString());
            await _fixture.UserStore.AddLoginAsync(user, userLogin);

            //get user by logininfro
            var userByLogin = await _fixture.UserStore.FindAsync(userLogin);

            Assert.Equal(user.Id, userByLogin.Id);

            // add an additional login
            var userAdditionalLogin = new UserLoginInfo("MyOtherOauthProvider", Guid.NewGuid().ToString());
            await _fixture.UserStore.AddLoginAsync(user, userAdditionalLogin);

            // get all user logins
            var allUserLogins = _fixture.UserStore.GetLoginsAsync(user);

            Assert.Equal(allUserLogins.Result.Count, 2);
            Assert.Contains(userLogin, allUserLogins.Result, new UserLoginInfoComparer());
            Assert.Contains(userAdditionalLogin, allUserLogins.Result, new UserLoginInfoComparer());

            // remove all user logins
            await _fixture.UserStore.RemoveLoginAsync(user, userLogin);
            await _fixture.UserStore.RemoveLoginAsync(user, userAdditionalLogin);

            // confirm logins have been deleted
            var noLogins = await _fixture.UserStore.GetLoginsAsync(user);

            Assert.Empty(noLogins);

            // delete user           
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IUserClaimStoreTests()
        {
            //create user
            var user = new IdentityUser() { UserName = "test user" };
            await _fixture.UserStore.CreateAsync(user);

            //create and add claims to user
            var claims = new List<Claim> { new Claim("First Claim", "First Claim"), new Claim("Second Claim", "Second Claim") };
            await _fixture.UserStore.AddClaimAsync(user, claims[0]);
            await _fixture.UserStore.AddClaimAsync(user, claims[1]);

            //get user claims
            var myClaims = await _fixture.UserStore.GetClaimsAsync(user);

            Assert.Equal(myClaims.Count, claims.Count);

            // delete the claims
            await _fixture.UserStore.RemoveClaimAsync(user, claims[0]);
            await _fixture.UserStore.RemoveClaimAsync(user, claims[1]);

            //verify claims have been deleted
            var noClaims = await _fixture.UserStore.GetClaimsAsync(user);

            Assert.Empty(noClaims);

            // delete user           
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IUserRoleStoreTests()
        {
            //create user
            var user = new IdentityUser() { UserName = "test user" };
            await _fixture.UserStore.CreateAsync(user);

            //create roles
            var roles = new List<IdentityRole> { new IdentityRole { Name = "first role" }, new IdentityRole { Name = "second role" } };
            await _fixture.RoleStore.CreateAsync(roles[0]);
            await _fixture.RoleStore.CreateAsync(roles[1]);

            //add user to roles
            await _fixture.UserStore.AddToRoleAsync(user, roles[0].Name);
            await _fixture.UserStore.AddToRoleAsync(user, roles[1].Name);
            
            var userRoles = await _fixture.UserStore.GetRolesAsync(user);

            Assert.Equal(userRoles.Count, roles.Count);

            var userIsInFirstRole = _fixture.UserStore.IsInRoleAsync(user, roles[0].Name);

            Assert.Equal(userIsInFirstRole.Result, true);

            var userIsInSecondRole = _fixture.UserStore.IsInRoleAsync(user, roles[1].Name);

            Assert.Equal(userIsInSecondRole.Result, true);
            
            //remove roles
            await _fixture.UserStore.RemoveFromRoleAsync(user, roles[0].Name);
            await _fixture.UserStore.RemoveFromRoleAsync(user, roles[1].Name);

            //verify user is not in the first role
            var userInFirstRole = await _fixture.UserStore.IsInRoleAsync(user, roles[0].Name);

            Assert.Equal(userInFirstRole, false);

            //verify user is not in the second role
            var userInSecondRole = await _fixture.UserStore.IsInRoleAsync(user, roles[1].Name);

            Assert.Equal(userInSecondRole, false);

            // delete the first role
            await _fixture.RoleStore.DeleteAsync(roles[0]);

            //verify role deleted
            var deletedFirstRole = _fixture.RoleStore.FindByNameAsync(roles[0].Name);

            Assert.Equal(deletedFirstRole.Result, null);

            //delete second role
            await _fixture.RoleStore.DeleteAsync(roles[1]);

            //verify second role deleted
            var deletedSecondRole = _fixture.RoleStore.FindByNameAsync(roles[1].Name);

            Assert.Equal(deletedSecondRole.Result, null);

            // delete user
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IUserPasswordStoreTests()
        {
            var user = new IdentityUser() { UserName = "test user" };

            await _fixture.UserStore.CreateAsync(user);

            PasswordHasher hasher = new PasswordHasher();

            //set password
            await _fixture.UserStore.SetPasswordHashAsync(user, hasher.HashPassword("Secret Squirrel!"));

            //verify user has password
            var hasPassword = await _fixture.UserStore.HasPasswordAsync(user);

            Assert.Equal(hasPassword, true);

            //verify password
            var userPassword = await _fixture.UserStore.GetPasswordHashAsync(user);

            Assert.Equal(hasher.VerifyHashedPassword(userPassword, "Secret Squirrel!"), PasswordVerificationResult.Success);

            //delete user
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IUserSecurityStampStoreTests()
        {
            //create user
            var user = new IdentityUser() { UserName = "test user" };

            await _fixture.UserStore.CreateAsync(user);

            //set security stamp
            var stamp = Guid.NewGuid().ToString();
            await _fixture.UserStore.SetSecurityStampAsync(user, stamp);

            //verify security stamp has been set
            var userStamp = await _fixture.UserStore.GetSecurityStampAsync(user);

            Assert.Equal(userStamp, stamp);

            //delete user
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IUserEmailStoreTests()
        {
            //create user
            var user = new IdentityUser() { UserName = "test user" };

            await _fixture.UserStore.CreateAsync(user);

            //set email address
            await _fixture.UserStore.SetEmailAsync(user, "test@user.com");

            //verify email address has been set
            var emailAddress = await _fixture.UserStore.GetEmailAsync(user);

            Assert.Equal(emailAddress, "test@user.com");

            //verify email address has not been confirmed
            var notConfirmed = await _fixture.UserStore.GetEmailConfirmedAsync(user);

            Assert.Equal(notConfirmed, false);

            //confirm email address
            await _fixture.UserStore.SetEmailConfirmedAsync(user, true);

            //verify email address has been confirmed
            var confirmed = await _fixture.UserStore.GetEmailConfirmedAsync(user);

            Assert.Equal(confirmed, true);

            //delete user
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IUserPhoneNumberStoreTests()
        {
            //create user
            var user = new IdentityUser() { UserName = "test user" };

            await _fixture.UserStore.CreateAsync(user);

            //set telephone number
            await _fixture.UserStore.SetPhoneNumberAsync(user, "123456");

            //verify phone number has been set
            var phoneNumber = await _fixture.UserStore.GetPhoneNumberAsync(user);

            Assert.Equal(phoneNumber, "123456");

            //verify phone number has not been confirmed
            var notConfirmed = await _fixture.UserStore.GetPhoneNumberConfirmedAsync(user);

            Assert.Equal(notConfirmed, false);

            //confirm phone number
            await _fixture.UserStore.SetPhoneNumberConfirmedAsync(user, true);

            //verify phone number confirmed
            var confirmed = await _fixture.UserStore.GetPhoneNumberConfirmedAsync(user);

            Assert.Equal(confirmed, true);

            //delete user
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IUserTwoFactorStoreTests()
        {
            //create user
            var user = new IdentityUser() { UserName = "test user" };

            await _fixture.UserStore.CreateAsync(user);

            //verify two factor not enabled
            var notEnabled = await _fixture.UserStore.GetTwoFactorEnabledAsync(user);

            Assert.Equal(notEnabled, false);

            //enable two factor
            await _fixture.UserStore.SetTwoFactorEnabledAsync(user, true);

            //verify two factor enabled
            var confirmed = await _fixture.UserStore.GetTwoFactorEnabledAsync(user);

            Assert.Equal(confirmed, true);

            //delete user
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IUserLockoutStoreTests()
        {
            //create user
            var user = new IdentityUser() { UserName = "test user" };
            var count = 0;
            var confirm = false;

            await _fixture.UserStore.CreateAsync(user);

            //verify failure count
            count = await _fixture.UserStore.GetAccessFailedCountAsync(user);

            Assert.Equal(count, 0);

            //increment failure count
            await _fixture.UserStore.IncrementAccessFailedCountAsync(user);

            //verify failure
            count = await _fixture.UserStore.GetAccessFailedCountAsync(user);

            Assert.Equal(count, 1);

            //reset failure count
            await _fixture.UserStore.ResetAccessFailedCountAsync(user);

            //verify failure count reset
            count = await _fixture.UserStore.GetAccessFailedCountAsync(user);

            Assert.Equal(count, 0);

            //verify lockout is enabled
            confirm = await _fixture.UserStore.GetLockoutEnabledAsync(user);

            Assert.Equal(confirm, false);

            //set lockout
            await _fixture.UserStore.SetLockoutEnabledAsync(user, true);

            //confirm lockout enabled
            confirm = await _fixture.UserStore.GetLockoutEnabledAsync(user);

            Assert.Equal(confirm, true);

            //set lockoutDate
            var lockoutDate = DateTimeOffset.UtcNow.AddMinutes(90);

            await _fixture.UserStore.SetLockoutEndDateAsync(user, lockoutDate);

            //verify lockout date has been set
            var verifyDate = await _fixture.UserStore.GetLockoutEndDateAsync(user);

            Assert.Equal(lockoutDate, verifyDate);

            //delete user
            await _fixture.UserStore.DeleteAsync(user);
        }

        [Fact]
        public async Task IQueryableRoleStoreTests()
        {
            //create role
            var role = new IdentityRole() { Name = "test role" };
            await _fixture.RoleStore.CreateAsync(role);

            // get role by id
            var roleById = await _fixture.RoleStore.FindByIdAsync(role.Id);

            Assert.NotNull(roleById);
            Assert.Equal(role.Id, roleById.Id);

            // get role by name
            var roleByName = await _fixture.RoleStore.FindByNameAsync(role.Name);

            Assert.NotNull(roleByName);
            Assert.Equal(role.Id, roleByName.Id);

            //update role
            role.Name = "test role updated";
            await _fixture.RoleStore.UpdateAsync(role);

            //get updated role
            var updatedRole = await _fixture.RoleStore.FindByNameAsync(role.Name);

            Assert.NotNull(updatedRole);
            Assert.Equal(role.Id, updatedRole.Id);

            //delete role
            await _fixture.RoleStore.DeleteAsync(role);
            var deletedRole = await _fixture.RoleStore.FindByIdAsync(role.Id);

            Assert.Equal(deletedRole, null);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class UserLoginInfoComparer : IEqualityComparer<UserLoginInfo>
    {
        public bool Equals(UserLoginInfo x, UserLoginInfo y)
        {
            return x.LoginProvider == y.LoginProvider && x.ProviderKey == y.ProviderKey;
        }

        public int GetHashCode(UserLoginInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}