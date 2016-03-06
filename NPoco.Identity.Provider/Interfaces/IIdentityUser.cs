using Microsoft.AspNet.Identity;
using System;

namespace NPoco.Identity.Provider.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IIdentityUser<TKey> : IUser<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool EmailConfirmed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string PasswordHash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string SecurityStamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool LockoutEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int AccessFailedCount { get; set; }
    }
}