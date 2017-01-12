using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Data.SqlTypes;
using CBC.Framework.AuditTrail.Enums;
using System.Runtime.Serialization;
using CBC.Framework.AuditTrail.Attributes;
using System.Linq;

namespace CBC.Framework.Utility
{
    public class BinarySerializer
    {

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Byte[] SerializeObject(Object obj)
        {
            Byte[] serializedObject = null;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
            bf.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full;
            bf.Serialize(ms, obj);
            serializedObject = ms.ToArray();
            ms.Close();
            return serializedObject;
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <param name="serializedObject">The serialized object.</param>
        /// <returns></returns>
        public static Object DeSerializeObject(Byte[] serializedObject)
        {
            Object deSerializedObject = null;
            if (serializedObject != null)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream(serializedObject);
                try
                {
                    deSerializedObject = new BinaryFormatter().Deserialize(ms);
                }
                catch
                {
                }
                finally
                {
                    ms.Close();
                }
            }
            return deSerializedObject;
        }

        [Obsolete("Use DeserializeObject method instead")]
        public static List<TrailItem> Deserialize(Byte[] databefore, Byte[] dataafter, string dataType, AuditAction action)
        {
            List<TrailItem> ofItems = new List<TrailItem>();
            Object dafter = dataafter == null ? new object() : (DeSerializeObject(dataafter));
            Object dbefore = databefore == null ? new object() : (DeSerializeObject(databefore));

            Object when = dbefore;
            if (dataafter != null && databefore == null)      //create
                when = dafter;
            if (databefore != null && dataafter == null)      //delete
                when = dbefore;

            if (when == null) return ofItems;
            foreach (PropertyInfo prop in when.GetType().GetProperties())
            {
                Type ptype = prop.PropertyType;
                object ater = null;
                object bfore = null;
                //if ((ptype.IsPrimitive || ptype.Equals(typeof(String)) || ptype.Equals(typeof(DateTime)) || ptype.IsEnum) && prop.CanWrite)
               // if (prop.GetCustomAttributes(typeof(IsLoggedAttribute), false).Length > 0)
                {
                    string before = "";
                    string after = "";
                    string name = string.Empty;//((IsLoggedAttribute)prop.GetCustomAttributes(typeof(IsLoggedAttribute), false)[0]).LoggedName;
                    if (name == string.Empty) name = prop.Name;
                    if (action != AuditAction.CREATE)
                    {
                        bfore = prop.GetValue(dbefore, null);
                        if (ptype.Equals(typeof(DateTime)))
                        {
                            DateTime noDate = (DateTime)SqlDateTime.Null;
                            before = (((DateTime)bfore).Equals(noDate) ? "None" : ((DateTime)bfore).ToString("dd-MMM-yyyy hh:mm tt"));
                        }
                        else if (ptype.Equals(typeof(Decimal)))
                        {
                            before = ((Decimal)bfore).ToString("N");
                        }
                        else if (ptype.IsClass && !ptype.Equals(typeof(String)))
                        {
                            try
                            {
                                before = Convert.ToString(ptype.GetProperty("Name").GetValue(bfore, null));
                            }
                            catch
                            {
                                before = bfore == null ? "" : Convert.ToString(bfore);
                            }
                        }
                        else
                        {
                            before = bfore == null ? "" : Convert.ToString(bfore);
                        }
                    }

                    if (action != AuditAction.DELETE)
                    {
                        ater = prop.GetValue(dafter, null);
                        if (ptype.Equals(typeof(DateTime)))
                        {
                            DateTime noDate = (DateTime)SqlDateTime.Null;
                            after = (((DateTime)ater).Equals(noDate) ? "None" : ((DateTime)ater).ToString("dd-MMM-yyyy hh:mm tt"));
                        }
                        else if (ptype.Equals(typeof(Decimal)))
                        {
                            after = ((Decimal)ater).ToString("N");
                        }
                        else if (ptype.IsClass && !ptype.Equals(typeof(String)))
                        {
                            try
                            {
                                after = Convert.ToString(ptype.GetProperty("Name").GetValue(ater, null));
                            }
                            catch
                            {
                                after = ater == null ? "" : Convert.ToString(ater);
                            }
                        }
                        else
                        {
                            after = ater == null ? "" : Convert.ToString(ater);
                        }
                    }
                    //Don't show "Deleted" or "ID" status.
                    //if (prop.Name != "ID" && prop.Name != "Deleted")
                    ofItems.Add(new TrailItem(name, before, after));
                }
            }
            return ofItems;

        }

        public static byte[] SerializeData(object objectBefore, object objectAfter)
        {
            return SerializeData(objectBefore, objectAfter, 4);
        }

        public static byte[] SerializeData(object objectBefore, object objectAfter, int maxDepth)
        {
            return SerializeData(objectBefore, objectAfter, 0, maxDepth);
        }

        public static byte[] SerializeData(object objectBefore, object objectAfter, int currentDepth, int maxDepth)
        {
            return CBC.Framework.Utility.BinarySerializer.SerializeObject(ConvertToList(objectBefore, objectAfter, currentDepth, maxDepth));
        }

        public static List<TrailItem> DeSerializeObject(byte[] dataBefore, byte[] dataAfter)
        {
            return DeSerializeObject(dataBefore, dataAfter, 4);
        }

        public static List<TrailItem> DeSerializeObject(byte[] dataBefore, byte[] dataAfter, int maxDepth)
        {
            return DeSerializeObject(dataBefore, dataAfter, 0,maxDepth);
        }

        public static List<TrailItem> DeSerializeObject(byte[] dataBefore, byte[] dataAfter, int currentDepth, int maxDepth)
        {
            return ConvertToList(DeSerializeObject(dataBefore), DeSerializeObject(dataAfter), currentDepth, maxDepth);
        }

        public static List<TrailItem> ConvertToList(object objectBefore, object objectAfter, int currentDepth, int maxDepth)
        {
            if (currentDepth > maxDepth)
            {
                return null;
            }
            List<TrailItem> trailItems = new List<TrailItem>();
            if (objectBefore == null && objectAfter == null)
            {
                return trailItems;
            }
            if (objectBefore != null && objectAfter != null)
            {
                if (objectBefore.GetType() != objectAfter.GetType())
                {
                    throw new InvalidOperationException("The two objects must be of the same type");
                }
            }
            //Use one of them to invoke reflection.
            object dataTypeCheck = objectAfter != null ? objectAfter : objectBefore;

            //Iterate thru all the properties and make sure that the property can be logged.
            //foreach (PropertyInfo propertyInfo in dataTypeCheck.GetType().GetProperties().OrderBy(p => p.Name))
            foreach (PropertyInfo propertyInfo in dataTypeCheck.GetType().GetProperties())
            {
                //Specifies whether we should ignore the attribute.
                TrailableIgnoreAttribute[] ignoreAttribute = (TrailableIgnoreAttribute[])propertyInfo.GetCustomAttributes(typeof(TrailableIgnoreAttribute), false);
                if (ignoreAttribute != null && ignoreAttribute.Length > 0 && !ignoreAttribute[0].IsSpecial)
                {
                    continue;
                }

                Type propertyType = propertyInfo.PropertyType;
                object propertyAfter = null;
                object propertyBefore = null;

                string strBefore = "";
                string strAfter = "";
                bool ignoreBeforeText = false; //Indicates whether you should ignore setting the 'Before Text'
                bool ignoreAfterText = false;  //Indicates whether you should ignore setting the 'After Text'

                if (objectBefore != null)
                {
                    propertyBefore = propertyInfo.GetValue(Convert.ChangeType(objectBefore, dataTypeCheck.GetType()), null);
                }
                if (objectAfter != null)
                {
                    propertyAfter = propertyInfo.GetValue(Convert.ChangeType(objectAfter, dataTypeCheck.GetType()), null);
                }


                TrailableDependencyPropertyAttribute[] dependencyAttribute = (TrailableDependencyPropertyAttribute[])propertyInfo.GetCustomAttributes(typeof(TrailableDependencyPropertyAttribute), false);
                if (dependencyAttribute != null && dependencyAttribute.Length > 0)
                {
                    if (objectBefore != null && Convert.ToBoolean(objectBefore.GetType().GetProperty(dependencyAttribute[0].PropertyName).GetValue(objectBefore, null)))
                    {
                        ignoreBeforeText = true;
                        strBefore = dependencyAttribute[0].IfTrueText;
                    }
                    if (objectAfter != null && Convert.ToBoolean(objectAfter.GetType().GetProperty(dependencyAttribute[0].PropertyName).GetValue(objectAfter, null)))
                    {
                        ignoreAfterText = true;
                        strAfter = dependencyAttribute[0].IfTrueText;
                    }
                }
                
                
                if (propertyType.Equals(typeof(DateTime)) || propertyType.Equals(typeof(DateTime?)))
                {
                    DateTime noDate = (DateTime)SqlDateTime.Null;
                    if (!ignoreBeforeText && propertyBefore != null)
                    {
                        strBefore = (((DateTime)propertyBefore).Equals(noDate) ? "None" : ((DateTime)propertyBefore).ToString("dd-MMM-yyyy hh:mm tt"));
                    }
                    if (!ignoreAfterText && propertyAfter != null)
                    {
                        strAfter = (((DateTime)propertyAfter).Equals(noDate) ? "None" : ((DateTime)propertyAfter).ToString("dd-MMM-yyyy hh:mm tt"));
                    }
                }
                else if (propertyType.Equals(typeof(Decimal)) || propertyType.Equals(typeof(Decimal?)))
                {
                    if (!ignoreBeforeText && propertyBefore != null)
                    {
                        strBefore = ((Decimal)propertyBefore).ToString("N");
                    }
                    if (!ignoreAfterText && propertyAfter != null)
                    {
                        strAfter = ((Decimal)propertyAfter).ToString("N");
                    }
                }
                else if (propertyType.IsEnum)
                {
                    if (!ignoreBeforeText && propertyBefore != null)
                    {
                        strBefore = CBC.Framework.Utility.EnumBinder.SplitAtCapitalLetters(Convert.ToString(propertyBefore));
                    }
                    if (!ignoreAfterText && propertyAfter != null)
                    {
                        strAfter = CBC.Framework.Utility.EnumBinder.SplitAtCapitalLetters(Convert.ToString(propertyAfter));
                    }
                }
                else if (propertyType.IsGenericType || propertyType.Equals(typeof(IList)) || propertyType.Equals(typeof(ICollection)))
                {
                    TrailableInnerClassAttribute[] innerClassAttribute = (TrailableInnerClassAttribute[])propertyInfo.GetCustomAttributes(typeof(TrailableInnerClassAttribute), false);
                    //Do you need only the inner class's name property or all the data???
                    //if (innerClassAttribute.Length > 0 && innerClassAttribute[0].NeedOnlyNameProperty)
                    if (false)
                    {
                        if (!ignoreBeforeText && propertyBefore != null)
                        {
                            try
                            {
                                string beforeString = "";
                                IList arrayBefore = propertyBefore as IList;
                                if (arrayBefore != null)
                                {
                                    foreach (object item in arrayBefore)
                                    {
                                        beforeString += Convert.ToString(propertyType.GetProperty(innerClassAttribute[0].ClassNameIdentifier).GetValue(propertyBefore, null)) + " ,"; ;
                                    }

                                    beforeString = beforeString.Substring(0, beforeString.Length - 2);
                                }

                                strBefore = beforeString;
                            }
                            catch
                            {
                                strBefore = propertyBefore == null ? "" : Convert.ToString(propertyBefore);
                            }
                        }
                        if (!ignoreAfterText && propertyAfter != null)
                        {
                            try
                            {
                                string afterString = "";
                                IList arrayAfter = propertyAfter as IList;
                                if (arrayAfter != null)
                                {
                                    foreach (object item in arrayAfter)
                                    {
                                        afterString += Convert.ToString(propertyType.GetProperty(innerClassAttribute[0].ClassNameIdentifier).GetValue(propertyAfter, null)) + " ,"; ;
                                    }

                                    afterString = afterString.Substring(0, afterString.Length - 2);
                                }

                                strBefore = afterString;
                            }
                            catch
                            {
                                strAfter = propertyAfter == null ? "" : Convert.ToString(propertyAfter);
                            }
                        }
                    }
                    else
                    {
                        if (!ignoreBeforeText && propertyBefore != null)
                        {
                            try
                            {
                                StringBuilder beforeString = new StringBuilder();
                                IList arrayBefore = propertyBefore as IList;
                                if (arrayBefore != null && arrayBefore.Count > 0)
                                {
                                    foreach (object item in arrayBefore)
                                    {
                                        beforeString.Append("[");

                                        foreach (PropertyInfo subPropInfo in item.GetType().GetProperties().OrderBy(p => p.Name))
                                        {
                                            TrailableIgnoreAttribute[] ignoreAttribute2 = (TrailableIgnoreAttribute[])subPropInfo.GetCustomAttributes(typeof(TrailableIgnoreAttribute), false);
                                            if (ignoreAttribute2 != null && ignoreAttribute2.Length > 0)
                                            {
                                                continue;
                                            }

                                            //Get the property name...
                                            TrailableNameAttribute[] subNameAttribute = (TrailableNameAttribute[])subPropInfo.GetCustomAttributes(typeof(TrailableNameAttribute), false);
                                            string subPropertyName = subNameAttribute != null && subNameAttribute.Length > 0 ? subNameAttribute[0].LoggedName :
                                                EnumBinder.SplitAtCapitalLetters(subPropInfo.Name);
                                            beforeString.Append("{");
                                            beforeString.AppendFormat("{0}:", subPropertyName);
                                            try
                                            {
                                                beforeString.Append(GetValue(subPropInfo.PropertyType, subPropInfo.GetValue(item, null)));
                                            }
                                            catch
                                            {
                                            }
                                            //Closing bracket
                                            beforeString.Append("}, ");

                                        }
                                        beforeString.AppendLine("], ");
                                    }
                                    if (beforeString != null)
                                    {
                                        strBefore = beforeString.ToString().Trim().TrimEnd(',').Trim().TrimEnd(']').Trim().TrimEnd(',') + "]";
                                    }
                                    else
                                    {
                                        strBefore = "";
                                    }
                                }
                            }
                            catch
                            {
                                strBefore = "";
                            }
                        }
                        if (!ignoreAfterText && propertyAfter != null)
                        {
                            try
                            {
                                StringBuilder afterString = new StringBuilder();
                                IList arrayAfter = propertyAfter as IList;
                                if (arrayAfter != null && arrayAfter.Count > 0)
                                {
                                    foreach (object item in arrayAfter)
                                    {
                                        afterString.Append("[");

                                        foreach (PropertyInfo subPropInfo in item.GetType().GetProperties().OrderBy(p => p.Name))
                                        {
                                            //Check if the item has TrailableIgnore attribute on it.
                                            // if(!Attribute.IsDefined(, typeof(TrailableIgnoreAttribute))
                                            TrailableIgnoreAttribute[] ignoreAttribute2 = (TrailableIgnoreAttribute[])subPropInfo.GetCustomAttributes(typeof(TrailableIgnoreAttribute), false);
                                            if (ignoreAttribute2 != null && ignoreAttribute2.Length > 0)
                                            {
                                                continue;
                                            }

                                            //Get the property name...
                                            TrailableNameAttribute[] subNameAttribute = (TrailableNameAttribute[])subPropInfo.GetCustomAttributes(typeof(TrailableNameAttribute), false);
                                            string subPropertyName = subNameAttribute != null && subNameAttribute.Length > 0 ? subNameAttribute[0].LoggedName :
                                                EnumBinder.SplitAtCapitalLetters(subPropInfo.Name);
                                            afterString.Append("{");
                                            afterString.AppendFormat("{0}:", subPropertyName);
                                            try
                                            {
                                                afterString.Append(GetValue(subPropInfo.PropertyType, subPropInfo.GetValue(item, null)));
                                            }
                                            catch
                                            {
                                            }
                                            //Closing bracket
                                            afterString.Append("}, ");
                                        }

                                        afterString.AppendLine("], ");
                                    }
                                    if (afterString != null)
                                    {
                                        strAfter = afterString.ToString().Trim().TrimEnd(',').Trim().TrimEnd(']').Trim().TrimEnd(',') + "]";
                                    }
                                }
                            }
                            catch
                            {
                                strAfter = "";
                            }
                        }
                    }
                }
                //Treat specially if the property is a class and it is not a string.
                else if ((propertyType.IsInterface || propertyType.IsClass) && !propertyType.Equals(typeof(String)))
                {
                    TrailableInnerClassAttribute[] innerClassAttribute = (TrailableInnerClassAttribute[])propertyInfo.GetCustomAttributes(typeof(TrailableInnerClassAttribute), false);
                    
                    //Do you need only the inner class's name property or all the data???
                    if (innerClassAttribute.Length > 0 && innerClassAttribute[0].NeedOnlyNameProperty)
                    {
                        if (!ignoreBeforeText && propertyBefore != null)
                        {
                            try
                            {
                                strBefore = Convert.ToString(propertyType.GetProperty(innerClassAttribute[0].ClassNameIdentifier).GetValue(propertyBefore, null));
                            }
                            catch
                            {
                                strBefore = propertyBefore == null ? "" : Convert.ToString(propertyBefore);
                            }
                        }
                        if (!ignoreAfterText && propertyAfter != null)
                        {
                            try
                            {
                                strAfter = Convert.ToString(propertyType.GetProperty(innerClassAttribute[0].ClassNameIdentifier).GetValue(propertyAfter, null));
                            }
                            catch
                            {
                                strAfter = propertyAfter == null ? "" : Convert.ToString(propertyAfter);
                            }
                        }
                    }
                    else
                    {
                        //Remember to also check the properties of the subclass for Loggable attribute.
                        List<TrailItem> innerClassProperties = GetInnerClassProperties(propertyBefore, propertyAfter, currentDepth + 1, maxDepth);
                        if (innerClassProperties != null && innerClassProperties.Count > 0)
                        {
                            if (innerClassAttribute != null && innerClassAttribute.Length > 0 && innerClassAttribute[0].AppendPropertiesToClassName)
                            {
                                if (innerClassProperties != null)
                                {
                                    //Get the Class name...
                                    TrailableNameAttribute[] classNameAttribute = (TrailableNameAttribute[])propertyInfo.GetCustomAttributes(typeof(TrailableNameAttribute), false);
                                    string className = classNameAttribute != null && classNameAttribute.Length > 0 ? classNameAttribute[0].LoggedName :
                                        EnumBinder.SplitAtCapitalLetters(propertyInfo.Name);

                                    foreach (TrailItem innerClassProperty in innerClassProperties)
                                    {
                                        innerClassProperty.Name = String.Format("{0} {1}{2}", className,
                                            innerClassAttribute[0].UseSeperator ? innerClassAttribute[0].Seperator + " " : "", innerClassProperty.Name);
                                    }
                                }
                            }

                            trailItems.AddRange(innerClassProperties);
                        }
                        continue;
                    }
                }
                else
                {
                    if (!ignoreBeforeText)
                    {
                        strBefore = propertyBefore == null ? "" : Convert.ToString(propertyBefore);
                    }
                    if (!ignoreAfterText)
                    {
                        strAfter = propertyAfter == null ? "" : Convert.ToString(propertyAfter);
                    }
                }

                //Get the property name...
                TrailableNameAttribute[] nameAttribute = (TrailableNameAttribute[])propertyInfo.GetCustomAttributes(typeof(TrailableNameAttribute), false);
                
                string propertyName = nameAttribute != null && nameAttribute.Length > 0 ? nameAttribute[0].LoggedName :
                    EnumBinder.SplitAtCapitalLetters(propertyInfo.Name);

                if (nameAttribute != null && nameAttribute.Length > 0)
                {
                    if (nameAttribute[0].UseEmptyStringText && objectBefore != null)
                    {
                        if (strBefore.GetType() == typeof(string))
                        {
                            string sBefore = Convert.ToString(strBefore);
                            if (String.IsNullOrEmpty(sBefore)) strBefore = nameAttribute[0].EmptyStringText;
                        }
                        if (strAfter.GetType() == typeof(string))
                        {
                            string sAfter = Convert.ToString(strAfter);
                            if (String.IsNullOrEmpty(sAfter)) strAfter = nameAttribute[0].EmptyStringText;
                        }
                    }
                }


                //This section basically does a formatting on the property.
                TrailableFormatterAttribute[] formatAttribute = (TrailableFormatterAttribute[])propertyInfo.GetCustomAttributes(typeof(TrailableFormatterAttribute), false);
                if (formatAttribute != null && formatAttribute.Length > 0)
                {
                    if (formatAttribute[0].DoDivision)
                    {
                        decimal temp= 0;
                        if (Decimal.TryParse(strBefore, out temp))
                        {
                            strBefore = (temp / 100).ToString();
                        }
                        if (Decimal.TryParse(strAfter, out temp))
                        {
                            strAfter = (temp / 100).ToString();
                        }
                    }

                    strBefore = String.Format(formatAttribute[0].Format, strBefore);
                    strAfter = String.Format(formatAttribute[0].Format, strAfter);
                }

                trailItems.Add(new TrailItem(propertyName,strBefore, strAfter));
            }

            //return trailItems.OrderBy(t => t.Name).ToList();
            return trailItems.ToList();
        }

        private static List<TrailItem> GetInnerClassProperties(object objectBefore, object objectAfter, int currentDepth, int maxDepth)
        {
            return ConvertToList(objectBefore, objectAfter, currentDepth, maxDepth);
        }


        private static string GetValue(Type propertyType, object property)
        {
            if (propertyType.Equals(typeof(DateTime)))
            {
                DateTime noDate = (DateTime)SqlDateTime.Null;
                if (property != null)
                {
                    return (((DateTime)property).Equals(noDate) ? "None" : ((DateTime)property).ToString("dd-MMM-yyyy hh:mm tt"));
                }
            }
            else if (propertyType.Equals(typeof(Decimal)))
            {
                if (property != null)
                {
                    return ((Decimal)property).ToString("N");
                }
            }
            else if (propertyType.IsEnum)
            {
                if (property != null)
                {
                    return CBC.Framework.Utility.EnumBinder.SplitAtCapitalLetters(Convert.ToString(property));
                }
            }
            else
            {
                return property == null ? "" : Convert.ToString(property);
            }

            return "";
        }

    }
    
    [DataContract]
    [Serializable]
    public class TrailItem
    {
        public TrailItem(string name, string valueBefore, string valueAfter)
        {
            this.Name = name;
            this.ValueBefore = valueBefore;
            this.ValueAfter = valueAfter;
        }
        public TrailItem()
            : this("", "", "")
        {
        }
        public TrailItem(string name)
            : this(name, "", "")
        {
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ValueBefore { get; set; }

        [DataMember]
        public string ValueAfter { get; set; }
    }
 
}
