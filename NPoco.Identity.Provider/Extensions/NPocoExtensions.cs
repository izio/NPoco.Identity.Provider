using NPoco.Identity.Provider.Enums;
using System;
using System.Linq;

namespace NPoco.Identity.Provider.Extensions
{
    /// <summary>
    /// NPoco Database extensions class
    /// </summary>
    public static class NPocoExtensions
    {
        /// <summary>
        /// Builds a select query across the related tables
        /// </summary>
        /// <typeparam name="T1">the primary table class</typeparam>
        /// <typeparam name="T2">the foreign table class</typeparam>
        /// <param name="database">the database the method extends</param>
        /// <param name="primary">the primary table properties</param>
        /// <param name="foreign">the foreign table properties</param>
        /// <param name="join">the type of join</param>
        /// <param name="where">the where clause</param>
        /// <param name="order">an optional order by clause</param>
        /// <returns>A formatted SQL query to select data from the specified tables</returns>
        public static string BuildRelatedSelectQuery<T1, T2>(this Database database, TableProperties primary, TableProperties foreign, Join join, string where, string order = "")
        {
            if (string.IsNullOrEmpty(where))
            {
                throw new ArgumentException("WHERE clause not specified");
            }

            if (where.ToLowerInvariant().StartsWith("where ") == false)
            {
                throw new ArgumentException("WHERE clause not well formed");
            }

            if (string.IsNullOrEmpty(order) == false && order.ToLowerInvariant().StartsWith("order by ") == false)
            {
                throw new ArgumentException("ORDER BY clause not well formed");
            }

            var primaryFactory = database.PocoDataFactory.ForType(typeof(T1));
            var primaryTable = primaryFactory.TableInfo.TableName;
            var primaryKey = primaryFactory.Columns.FirstOrDefault(c => c.Value.MemberInfo.Name == primary.Id).Key;

            var foreignFactory = database.PocoDataFactory.ForType(typeof(T2));
            var foreignTable = foreignFactory.TableInfo.TableName;
            var foreignKey = foreignFactory.Columns.FirstOrDefault(c => c.Value.MemberInfo.Name == foreign.Id).Key;

            return string.Format("SELECT {0}{1} FROM {2} AS {3} {4} {5} AS {6} ON {3}.{7} = {6}.{8} {9}", primary.GetColumns(), foreign.GetColumns(), primaryTable, primary.Alias, join.GetDescription(), foreignTable, foreign.Alias, primaryKey, foreignKey, where);
        }

        /// <summary>
        /// Builds a delete query across the related tables
        /// </summary>
        /// <typeparam name="T1">the primary table class</typeparam>
        /// <typeparam name="T2">the foreign table class</typeparam>
        /// <param name="database">the database the method extends</param>
        /// <param name="primary">the primary table properties</param>
        /// <param name="foreign">the foreign table properties</param>
        /// <param name="join">the type of join</param>
        /// <param name="where">the where clause</param>
        /// <returns>A formatted SQL query to delete data from the specified tables</returns>
        public static string BuildRelatedDeleteQuery<T1, T2>(this Database database, TableProperties primary, TableProperties foreign, Join join, string where)
        {
            if (string.IsNullOrEmpty(where))
            {
                throw new ArgumentException("WHERE clause not specified");
            }

            if (where.ToLowerInvariant().StartsWith("where ") == false)
            {
                throw new ArgumentException("WHERE clause not well formed");
            }

            var primaryFactory = database.PocoDataFactory.ForType(typeof(T1));
            var primaryTable = primaryFactory.TableInfo.TableName;
            var primaryKey = primaryFactory.Columns.FirstOrDefault(c => c.Value.MemberInfo.Name == primary.Id).Key;

            var foreignFactory = database.PocoDataFactory.ForType(typeof(T2));
            var foreignTable = foreignFactory.TableInfo.TableName;
            var foreignKey = foreignFactory.Columns.FirstOrDefault(c => c.Value.MemberInfo.Name == foreign.Id).Key;

            return string.Format("DELETE {0} FROM {1} AS {0} {2} {3} AS {4} ON {0}.{5} = {4}.{6} {7}", primary.Alias, primaryTable, join.GetDescription(), foreignTable, foreign.Alias, primaryKey, foreignKey, where);
        }
    }
}