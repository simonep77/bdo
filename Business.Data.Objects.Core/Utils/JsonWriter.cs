﻿using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.Schema.Definition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Business.Data.Objects.Core.Utils
{

    /// <summary>
    /// Really simple JSON writer
    ///- Outputs JSON structures from an object
    ///- Really simple API (new List<int> { 1, 2, 3 }).ToJson() == "[1,2,3]"
    ///- Will only output public fields and property getters on objects
    /// </summary>
    public static class JSONWriter
    {
        public static string ToJson(object item)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendValue(stringBuilder, item);
            return stringBuilder.ToString();
        }

        internal static void AppendValue(StringBuilder stringBuilder, object item)
        {
            if (item == null)
            {
                stringBuilder.Append("null");
                return;
            }

            Type type = item.GetType();
            if (type.IsString())
            {
                stringBuilder.Append('\"');
                stringBuilder.Append(((string)item).Replace("\\", "\\\\"));
                stringBuilder.Append('\"');
            }
            else if (type.IsIntegerType())
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type.IsDecimalType())
            {
                stringBuilder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0:F}", item);
            }
            else if (type.IsBool())
            {
                stringBuilder.Append(((bool)item) ? "true" : "false");
            }
            else if (type.IsDate())
            {
                DateTime dt = (DateTime)item;
                stringBuilder.Append('\"');

                if (dt != DateTime.MinValue)
                {
                    //Se non presente la parte oraria
                    stringBuilder.Append(dt.ToString("O"));
                }
                //"2019-09-26T07:58:30.996+0200"

                stringBuilder.Append('\"');
            }
            else if (type.IsGuid())
            {
                stringBuilder.Append('\"');
                stringBuilder.Append(((Guid)item).ToString());
                stringBuilder.Append('\"');
            }
            else if (item is IList)
            {
                stringBuilder.Append('[');
                bool isFirst = true;
                IList list = item as IList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    AppendValue(stringBuilder, list[i]);
                }
                stringBuilder.Append(']');
            }
            else if (item is DataObjectBase)
            {
                stringBuilder.Append('{');
                bool isFirst = true;
                var obj = item as DataObjectBase;
                foreach (var oProp in obj.mClassSchema.Properties)
                {
                    //Skip complex property
                    if (!(oProp is PropertySimple))
                        continue;

                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');

                    stringBuilder.Append('\"');
                    stringBuilder.Append(oProp.Name);
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, oProp.GetValue(obj));

                }
                stringBuilder.Append('}');
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType = type.GetGenericArguments()[0];

                //Refuse to output dictionary keys that aren't of type string
                if (keyType != typeof(string))
                {
                    stringBuilder.Append("{}");
                    return;
                }

                stringBuilder.Append('{');
                IDictionary dict = item as IDictionary;
                bool isFirst = true;
                foreach (object key in dict.Keys)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append('\"');
                    stringBuilder.Append((string)key);
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, dict[key]);
                }
                stringBuilder.Append('}');
            }
            else
            {
                stringBuilder.Append('{');

                bool isFirst = true;
                FieldInfo[] fieldInfos = type.GetFields();
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    if (!fieldInfos[i].IsPublic)
                        continue;

                    object value = fieldInfos[i].GetValue(item);
                    if (value != null)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(fieldInfos[i].Name);
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }
                }
                PropertyInfo[] propertyInfo = type.GetProperties();
                for (int i = 0; i < propertyInfo.Length; i++)
                {
                    if (propertyInfo[i].CanRead)
                    {
                        object value = propertyInfo[i].GetValue(item, null);
                        if (value != null)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                stringBuilder.Append(',');
                            stringBuilder.Append('\"');
                            stringBuilder.Append(propertyInfo[i].Name);
                            stringBuilder.Append("\":");
                            AppendValue(stringBuilder, value);
                        }
                    }
                }

                stringBuilder.Append('}');
            }
        }
    }
}