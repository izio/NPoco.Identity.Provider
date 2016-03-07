using System;
using System.ComponentModel;
using System.Linq;

namespace NPoco.Identity.Provider.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            DescriptionAttribute attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
