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
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Columns { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetColumns()
        {
            var builder = new StringBuilder();

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i == Columns.Count - 1)
                {
                    builder.AppendFormat("{0}.{1} ", Alias, Columns[i]);
                }
                else
                {
                    builder.AppendFormat("{0}.{1}, ", Alias, Columns[i]);
                }
            }

            return builder.ToString();
        }
    }
}
