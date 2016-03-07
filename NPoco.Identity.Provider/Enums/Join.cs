using System.ComponentModel;

namespace NPoco.Identity.Provider.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum Join
    {
        [Description("INNER JOIN")]
        InnerJoin,
        [Description("LEFT JOIN")]
        LeftJoin,
        [Description("RIGHT JOIN")]
        RightJoin
    }
}
