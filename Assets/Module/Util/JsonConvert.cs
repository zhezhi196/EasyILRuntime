using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LitJson;
using UnityEngine;

namespace Module
{
    public static class JsonConvert
    {
        public static JsonData ToJson(object value,Dictionary<string,bool> switchStation)
        {
            Type type = value.GetType();
            FieldInfo[] field = type.GetFields();
            JsonData data = new JsonData();
            for (int i = 0; i < field.Length; i++)
            {
                if (switchStation[field[i].Name])
                {
                    GetTypeJson(data,field[i].Name,field[i].GetValue(value));
                }
            }

            return data;
        }

        private static string GetJsonString(object fieldValue)
        {
            if (fieldValue == null) return null;
            if (fieldValue is Color)
            {
                Color c = (Color) fieldValue;
                return string.Join("_", c.r, c.g, c.b, c.a);
            }
            else if (fieldValue is Vector4)
            {
                Vector4 c = (Vector4) fieldValue;
                return string.Join("_", c.x, c.y, c.z, c.w);
            }
            else if (fieldValue is Vector3 || fieldValue is Vector3Int)
            {
                Vector3 c = (Vector3) fieldValue;
                return string.Join("_", c.x, c.y, c.z);
            }
            else if (fieldValue is Vector2 || fieldValue is Vector2Int)
            {
                Vector2 c = (Vector2) fieldValue;
                return string.Join("_", c.x, c.y);
            }
            else if (fieldValue is Transform)
            {
                Transform c = (Transform) fieldValue;
                return string.Join("#", string.Join("_", c.position.x, c.position.y, c.position.z),
                    string.Join("_", c.eulerAngles.x, c.eulerAngles.y, c.eulerAngles.z));
            }
            else
            {
                return fieldValue.ToString();
            }
        }

        private static void GetTypeJson(JsonData data,string fieldName,object fieldValue)
        {
            if (fieldValue == null) return;

            if (fieldValue is string|| fieldValue is Enum)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.String);
            }
            else if (fieldValue is int)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.Int);
            }
            else if (fieldValue is long)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.Long);
            }
            else if (fieldValue is float || fieldValue is double)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.Double);
            }
            else if (fieldValue is bool)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.Boolean);
            }
            else if (fieldValue is Array || fieldValue is IList)
            {
                IEnumerable target = (IEnumerable) fieldValue;
                data[fieldName] = new JsonData();
                data[fieldName].SetJsonType(JsonType.Array);
                foreach (object VARIABLE in target)
                {
                    data[fieldName].Add(GetJsonString(VARIABLE));
                }
            }
            else if (fieldValue is Color)
            {
                Color c = (Color) fieldValue;
                data[fieldName] = string.Join("_", c.r, c.g, c.b, c.a);
            }
            else if (fieldValue is Vector4)
            {
                Vector4 c = (Vector4) fieldValue;
                data[fieldName] = string.Join("_", c.x, c.y, c.z, c.w);
            }
            else if (fieldValue is Vector3 || fieldValue is Vector3Int)
            {
                Vector3 c = (Vector3) fieldValue;
                data[fieldName] = string.Join("_", c.x, c.y, c.z);
            }
            else if (fieldValue is Vector2 || fieldValue is Vector2Int)
            {
                Vector2 c = (Vector2) fieldValue;
                data[fieldName] = string.Join("_", c.x, c.y);
            }
            else if (fieldValue is Transform)
            {
                Transform c = (Transform) fieldValue;
                data[fieldName] = string.Join("#", string.Join("_", c.position.x, c.position.y, c.position.z),
                    string.Join("_", c.eulerAngles.x, c.eulerAngles.y, c.eulerAngles.z));
            }
            else
            {
                Type type = fieldValue.GetType();
                FieldInfo[] fildInfo = type.GetFields();
                for (int i = 0; i < fildInfo.Length; i++)
                {
                    GetTypeJson(data[fieldName], fildInfo[i].Name, fildInfo[i].GetValue(fieldValue));
                }
            }
            
        }
    }
}