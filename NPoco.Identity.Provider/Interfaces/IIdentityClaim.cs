namespace NPoco.Identity.Provider.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IIdentityClaim<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        TKey Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        TKey UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string ClaimType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string ClaimValue { get; set; }
    }
}