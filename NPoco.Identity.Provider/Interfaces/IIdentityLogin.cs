namespace NPoco.Identity.Provider.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IIdentityLogin<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        TKey UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string LoginProvider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string ProviderKey { get; set; }
    }
}
