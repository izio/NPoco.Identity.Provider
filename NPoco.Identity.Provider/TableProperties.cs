using System;
using System.Collections.Generic;
using System.Text;

namespace NPoco.Identity.Provider
{
    /// <summary>
    /// 
    /// </summary>
    public class TableProperties
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Columns { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alias"></param>
        public TableProperties(string id, string alias) : this(id, alias, new List<string>())
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alias"></param>
        /// <param name="columns"></param>
        public TableProperties(string id, string alias, List<string> columns)
        {
            if (string.IsNullOrEmpty(Id))
            {
                throw new ArgumentException("id");
            }

            if (string.IsNullOrEmpty(Alias))
            {
                throw new ArgumentException("alias");
            }

            Columns = columns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetColumns()
        {
            var builder = new StringBuilder();

            for (int i = 0; i < Columns.Count; i++)
            {
                if (string.IsNullOrEmpty(Alias))
                {
                    builder.AppendFormat(i == Columns.Count - 1 ? "{1} " : "{1}, ", Alias, Columns[i]);
                }
                else
                {
                    builder.AppendFormat(i == Columns.Count - 1 ? "{1} " : "{1}, ", Alias, Columns[i]);
                }
            }

            return builder.ToString();
        }
    }
}