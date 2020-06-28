using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DG.Tweening.Core;
using Module;
using UnityEngine;
using xasset;
using Object = UnityEngine.Object;

namespace HotFix
{
    public abstract class HotFixMonoBehaviour<T> : HotFixMonoBehaviour where T: ViewReference
    {
        #region target

        private T temp;

        public T target
        {
            get
            {
                if (temp == null)
                {
                    temp = (T) m_target;
                }

                return temp;
            }
        }

        #endregion
    }
    public abstract class HotFixMonoBehaviour : ViewBehaviour
    {
        #region 静态方法和静态函数

        protected static Dictionary<Type, FieldInfo[]> fieldCache = new Dictionary<Type, FieldInfo[]>();
        /// <summary>
        /// 根据信息生成MonoBehaviour
        /// </summary>
        /// <param name="ModuleMono"></param>
        public static HotFixMonoBehaviour CreateMonoBehaviour(ViewReference ModuleMono)
        {
            if (ModuleMono.target != null) return null;
            Type type = Type.GetType("HotFix." + ModuleMono.targetType);
            if (type == null)
            {
                Debug.LogError($"type={ModuleMono.targetType}无法找到");
                return null;
            }

            HotFixMonoBehaviour monoBehaviour = Activator.CreateInstance(type) as HotFixMonoBehaviour;
            FieldInfo[] fileds = null;

            if (!fieldCache.ContainsKey(type))
            {
                fileds = type.GetFields();
                fieldCache.Add(type, fileds);
            }
            else
            {
                fileds = fieldCache[type];
            }

            monoBehaviour.fileds = fileds;
            ModuleMono.target = monoBehaviour;
            monoBehaviour.m_target = ModuleMono;
            //monoBehaviour.RefreshField(fileds, ModuleMono);
            monoBehaviour.Awake();
            return monoBehaviour;
        }

        #endregion

        #region 属性

        public ViewReference target
        {
            get { return m_target; }
        }
        public Transform transform
        {
            get { return m_target.transform; }
        }

        public GameObject gameObject
        {
            get { return m_target.gameObject; }
        }

        public bool enable
        {
            get { return m_target.enabled; }
        }

        public string tag
        {
            get { return m_target.tag; }
            set { m_target.tag = value; }
        }

        public string name
        {
            get { return m_target.name; }
            set { m_target.name = value; }
        }

        public HideFlags hidFlags
        {
            get { return m_target.hideFlags; }
            set { m_target.hideFlags = value; }
        }
        

        #endregion
        
        #region 对字段进行赋值
        private FieldInfo[] fileds;
        /// <summary>
        /// 根据Modul的信息刷新字段信息
        /// </summary>
        /// <param name="fileds"></param>
        /// <param name="viewReference"></param>
        private void RefreshField(FieldInfo[] fileds, ViewReference viewReference)
        {
            for (int i = 0; i < fileds.Length; i++)
            {
                FieldInfo fieldInfo = fileds[i];
                Type fieldType = fieldInfo.FieldType;
                if (fieldType == typeof(string))
                {
                    for (int j = 0; j < viewReference.stringList.Count; j++)
                    {
                        if (viewReference.stringList[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, viewReference.stringList[j].value);
                            break;
                        }
                    }
                }
                else if (fieldType == typeof(int))
                {
                    for (int j = 0; j < viewReference.intList.Count; j++)
                    {
                        if (viewReference.intList[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, (int) viewReference.intList[j].value);
                            break;
                        }
                    }
                }
                else if (fieldType == typeof(long))
                {
                    for (int j = 0; j < viewReference.intList.Count; j++)
                    {
                        if (viewReference.intList[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, viewReference.intList[j].value);
                            break;
                        }
                    }
                }
                else if (fieldType == typeof(float))
                {
                    for (int j = 0; j < viewReference.floatList.Count; j++)
                    {
                        if (viewReference.floatList[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, (float) viewReference.floatList[j].value);
                            break;
                        }
                    }
                }
                else if (fieldType == typeof(double))
                {
                    for (int j = 0; j < viewReference.floatList.Count; j++)
                    {
                        if (viewReference.floatList[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, viewReference.floatList[j].value);
                            break;
                        }
                    }
                }
                else if (fieldType == typeof(bool))
                {
                    for (int j = 0; j < viewReference.boolList.Count; j++)
                    {
                        if (viewReference.boolList[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, viewReference.boolList[j].value);
                            break;
                        }
                    }
                }
                else if (fieldType == typeof(AnimationCurve))
                {
                    for (int j = 0; j < viewReference.animationList.Count; j++)
                    {
                        if (viewReference.animationList[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, viewReference.animationList[j].value);
                            break;
                        }
                    }
                }
                else if (fieldType == typeof(Vector3) || fieldType == typeof(Vector2))
                {
                    for (int j = 0; j < viewReference.vector3List.Count; j++)
                    {
                        if (viewReference.vector3List[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, viewReference.vector3List[j].value);
                            break;
                        }
                    }
                }
                else if (fieldType == typeof(Color))
                {
                    for (int j = 0; j < viewReference.colorList.Count; j++)
                    {
                        if (viewReference.colorList[j].key == fieldInfo.Name)
                        {
                            fieldInfo.SetValue(this, viewReference.colorList[j].value);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region 获取值的函数

        protected T GetObjectVaule<T>(string name)
        {
            for (int i = 0; i < m_target.objectList.Count; i++)
            {
                if (m_target.objectList[i].key == name)
                {
                    if (m_target.objectList[i].value != null)
                    {
                        return m_target.objectList[i].value.GetComponent<T>();
                    }
                }
            }

            return default;
        }

        protected string GetStringValue(string name)
        {
            for (int i = 0; i < m_target.stringList.Count; i++)
            {
                if (m_target.stringList[i].key == name)
                {
                    return m_target.stringList[i].value;
                }
            }

            return default;
        }

        protected int GetIntValue(string name)
        {
            for (int i = 0; i < m_target.intList.Count; i++)
            {
                if (m_target.intList[i].key == name)
                {
                    return (int) m_target.intList[i].value;
                }
            }

            return default;
        }

        protected long GetLongValue(string name)
        {
            for (int i = 0; i < m_target.intList.Count; i++)
            {
                if (m_target.intList[i].key == name)
                {
                    return m_target.intList[i].value;
                }
            }

            return default;
        }

        protected float GetFloatValue(string name)
        {
            for (int i = 0; i < m_target.floatList.Count; i++)
            {
                if (m_target.floatList[i].key == name)
                {
                    return (float) m_target.floatList[i].value;
                }
            }

            return default;
        }

        protected double GetDoubleValue(string name)
        {
            for (int i = 0; i < m_target.floatList.Count; i++)
            {
                if (m_target.floatList[i].key == name)
                {
                    return m_target.floatList[i].value;
                }
            }

            return default;
        }

        protected bool GetBoolValue(string name)
        {
            for (int i = 0; i < m_target.boolList.Count; i++)
            {
                if (m_target.boolList[i].key == name)
                {
                    return m_target.boolList[i].value;
                }
            }

            return default;
        }

        protected AnimationCurve GetCurveValue(string name)
        {
            for (int i = 0; i < m_target.animationList.Count; i++)
            {
                if (m_target.animationList[i].key == name)
                {
                    return m_target.animationList[i].value;
                }
            }

            return default;
        }

        protected Vector2 GetVector2Value(string name)
        {
            for (int i = 0; i < m_target.vector3List.Count; i++)
            {
                if (m_target.vector3List[i].key == name)
                {
                    return m_target.vector3List[i].value;
                }
            }

            return default;
        }

        protected Vector3 GetVector3Value(string name)
        {
            for (int i = 0; i < m_target.vector3List.Count; i++)
            {
                if (m_target.vector3List[i].key == name)
                {
                    return m_target.vector3List[i].value;
                }
            }

            return default;
        }

        protected Color GetColorValue(string name)
        {
            for (int i = 0; i < m_target.colorList.Count; i++)
            {
                if (m_target.colorList[i].key == name)
                {
                    return m_target.colorList[i].value;
                }
            }

            return default;
        }

        protected T GetScript<T>(string name) where T : HotFixMonoBehaviour
        {
            ViewReference view = GetObjectVaule<ViewReference>(name);
            if (view == null) return null;
            return (T) view.target;
        }

        #endregion

        #region 时钟函数

        public override void Awake()
        {
        }

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }

        public override void Start()
        {
            if (Configuration.isEditor)
            {
                EventCenter.Register(EventKey.Update, RefreshField);
            }
        }

        public override void OnDestroy()
        {
            if (Configuration.isEditor)
            {
                EventCenter.UnRegister(EventKey.Update, RefreshField);
            }
        }

        public override void OnBecameInvisible()
        {
        }

        public override void OnBecameVisible()
        {
        }

        private void RefreshField()
        {
            RefreshField(fileds, m_target);
        }

        #endregion

        #region 原生函数

        public bool CompareTag(string tag)
        {
            return m_target.CompareTag(tag);
        }

        public T GetComponent<T>() where T : Object
        {
            return m_target.GetComponent<T>();
        }

        public T[] GetComponents<T>() where T : Object
        {
            return m_target.GetComponents<T>();
        }

        public T GetComponentInChildren<T>() where T : Object
        {
            return m_target.GetComponentInChildren<T>();
        }

        public T[] GetComponentsInChildren<T>() where T : Object
        {
            return m_target.GetComponentsInChildren<T>();
        }

        #endregion
    }
}