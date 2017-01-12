using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace CBC.Framework.Utility
{
    /// <summary>
    /// When used, it specifies that the property should be be ignored.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        private string _name;
        public EnumDescriptionAttribute(string name)
        {
            this._name = name;
        }


        public string Name { get { return this._name; } }
    }
    
    public class EnumBinder
    {
        public class NV
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public static List<NV> GetEnumNames(Type enumType)
        {
            List<NV> nameValueList = new List<NV>();

            foreach (int enumValue in Enum.GetValues(enumType).Cast<int>())
            {
                string enumName = Enum.GetName(enumType, enumValue);
                EnumDescriptionAttribute[] nameAttribute = (EnumDescriptionAttribute[])enumType.GetField(Enum.GetName(enumType, enumValue)).GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
                nameValueList.Add(new NV() { Name = SplitAtCapitalLetters((nameAttribute == null || nameAttribute.Length == 0) ? enumName : nameAttribute[0].Name), Value = enumValue });
            }

            //foreach (NV nvl in nameValueList.OrderBy(n => n.Name))
            //{
            //    result.Add(new { Name = nvl.Name, Value = nvl.Value });
            //}
            return nameValueList.OrderBy(n => n.Name).ToList();
        }

        public static List<NV> GetEnumNames(string enumStringType)
        {
            List<NV> nameValueList = new List<NV>();

            //List<object> result = new List<object>();
            Type enumType = Type.GetType(enumStringType);
            return GetEnumNames(enumType);
        }

        //Old
        /*
        public static List<object> GetEnumNames(string enumStringType)
        {
            List<object> result = new List<object>();
            Type enumType = Type.GetType(enumStringType);
            foreach (int enumValue in Enum.GetValues(enumType).Cast<int>())
            {
                var data = new { Name = SplitAtCapitalLetters(Enum.GetName(enumType, enumValue)), Value = enumValue };
                result.Add(data);
            }
            return result;
        }
        */

       public static string SplitAtCapitalLetters(string stringToSplit)
        {
            string finalString = Regex.Replace(stringToSplit, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
            
            if (finalString.Length == 0)
                return finalString;

            finalString = String.Format("{0}{1}", finalString.Substring(0, 1).ToUpper(), finalString.Substring(1, finalString.Length - 1));

            StringBuilder result = new StringBuilder();
            //This part is responsible for joining the ONE-LETTER strings.
            string[] moreCheck = finalString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (moreCheck.Length == 1)
            {
                return finalString;
            }
            result.Append(moreCheck[0].Trim());
            bool addSpace = moreCheck[0].Trim().Length > 1;

            for (int i = 1; i < moreCheck.Length; i++)
            {
                if (moreCheck[i].Trim().Length == 1)
                {
                    result.AppendFormat("{0}{1}", addSpace == true && i == 1 ? " " : "", moreCheck[i].Trim());
                }
                else
                {
                    result.AppendFormat(" {0}", moreCheck[i].Trim());
                }
            }
            return result.ToString();
        }

        public static string[] CapitalLetters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    }
}
