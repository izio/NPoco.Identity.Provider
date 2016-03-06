namespace NPoco.Identity.Provider.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IIdentityRoles<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        TKey UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        TKey RoleId { get; set; }
    }
}
