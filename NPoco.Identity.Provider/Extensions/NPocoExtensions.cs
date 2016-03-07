using NPoco.Identity.Provider.Enums;
using System.Linq;

namespace NPoco.Identity.Provider.Extensions
{
    /// <summary>
    /// NPoco Database extensions class
    /// </summary>
    public static class NPocoExtensions
    {
        /// <summary>
        /// Builds a query across the specified tables
        /// </summary>
        /// <typeparam name="T1">the primary table class</typeparam>
        /// <typeparam name="T2">the foreign table class</typeparam>
        /// <param name="database">the database the method extends</param>
        /// <param name="primary">the primary table properties</param>
        /// <param name="foreign">the foreign table properties</param>
        /// <param name="join">the type of join</param>
        /// <param name="where">an optional where clause</param>
        /// <returns></returns>
        public static string BuildRelatedQuery<T1, T2>(this Database database, TableProperties primary, TableProperties foreign, Join join, string where = "")
        {
            var primaryFactory = database.PocoDataFactory.ForType(typeof(T1));
            var primaryTable = primaryFactory.TableInfo.TableName;
            var primaryKey = primaryFactory.Columns.FirstOrDefault(c => c.Value.MemberInfo.Name == primary.Id).Key.ToString();

            var foreignFactory = database.PocoDataFactory.ForType(typeof(T2));
            var foreignTable = foreignFactory.TableInfo.TableName;
            var foreignKey = foreignFactory.Columns.FirstOrDefault(c => c.Value.MemberInfo.Name == foreign.Id).Key.ToString();

            return string.Format("SELECT {0}{1} FROM {2} AS {3} {4} {5} AS {6} ON {3}.{7} = {6}.{8} {9}", primary.GetColumns(), foreign.GetColumns(), primaryTable, primary.Alias, join.GetDescription(), foreignTable, foreign.Alias, primaryKey, foreignKey, where);
        }        
    }
}