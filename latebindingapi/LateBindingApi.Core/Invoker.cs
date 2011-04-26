using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace LateBindingApi.Core
{
    /// <summary>
    /// invoke helper functions
    /// </summary>
    public static class Invoker
    { 
        #region Method

        public static void Method(COMObject comObject, string name)
        {
            ValidateParam(comObject);
            Method(comObject, name, null);
        }

        public static void Method(object comObject, string name)
        {
            Method(comObject, name, null);
        }

        public static void Method(COMObject comObject, string name, object[] paramsArray)
        {
            comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramsArray, Settings.ThreadCulture);
        }

        public static void Method(object comObject, string name, object[] paramsArray)
        {
            comObject.GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, comObject, paramsArray, Settings.ThreadCulture);
        }

        public static void Method(COMObject comObject, string name, object[] paramsArray, ParameterModifier[] paramModifiers)
        {
            comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramsArray, paramModifiers, Settings.ThreadCulture, null);
        }

        public static object MethodReturn(COMObject comObject, string name)
        {
            return MethodReturn(comObject, name);
        }

        public static object MethodReturn(COMObject comObject, string name, object[] paramsArray)
        {
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramsArray, Settings.ThreadCulture);
            return returnValue;
        }

        public static object MethodReturn(COMObject comObject, string name, object[] paramsArray, ParameterModifier[] paramModifiers)
        {
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramsArray, paramModifiers, Settings.ThreadCulture, null);
            return returnValue;
        }

        #endregion

        #region Property

        public static object PropertyGet(object comObject, string name)
        {
            object returnValue = comObject.GetType().InvokeMember(name, BindingFlags.GetProperty, null, comObject, null, Settings.ThreadCulture);
            return returnValue;
        }

        public static object PropertyGet(COMObject comObject, string name)
        {
            ValidateObject(comObject);
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.GetProperty, null, comObject.UnderlyingObject, null, Settings.ThreadCulture);
            return returnValue;
        }

        public static object PropertyGet(object comObject, string name, object[] paramsArray)
        {
            object returnValue = comObject.GetType().InvokeMember(name, BindingFlags.GetProperty, null, comObject, paramsArray, Settings.ThreadCulture);
            return returnValue;
        }

        public static object PropertyGet(COMObject comObject, string name, object[] paramsArray)
        {
            ValidateObject(comObject);
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.GetProperty, null, comObject.UnderlyingObject, paramsArray, Settings.ThreadCulture);
            return returnValue;
        }

        public static object PropertyGet(COMObject comObject, string name, object[] paramsArray, ParameterModifier[] paramModifiers)
        {
            ValidateObject(comObject);
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.GetProperty, null, comObject.UnderlyingObject, paramsArray, paramModifiers, Settings.ThreadCulture, null);
            return returnValue;
        }

        public static void PropertySet(COMObject comObject, string name, object[] paramsArray, object value)
        {
            ValidateObject(comObject);
            object[] newParamsArray = new object[paramsArray.Length + 1];
            for (int i = 0; i < paramsArray.Length; i++)
                newParamsArray[i] = paramsArray[i];
            newParamsArray[newParamsArray.Length - 1] = value;

            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, newParamsArray , Settings.ThreadCulture);
        }

        public static void PropertySet(COMObject comObject, string name, object[] paramsArray, object value, ParameterModifier[] paramModifiers)
        {
            ValidateObject(comObject);
            object[] newParamsArray = new object[paramsArray.Length + 1];
            for (int i = 0; i < paramsArray.Length; i++)
                newParamsArray[i] = paramsArray[i];
            newParamsArray[newParamsArray.Length - 1] = value;

            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, newParamsArray, paramModifiers, Settings.ThreadCulture, null);
        }

        public static void PropertySet(COMObject comObject, string name, object value)
        {
            ValidateObject(comObject);
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, new object[] { value }, Settings.ThreadCulture);
        }

        public static void PropertySet(COMObject comObject, string name, object value, ParameterModifier[] paramModifiers)
        {
            ValidateObject(comObject);
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, new object[] { value }, paramModifiers, Settings.ThreadCulture, null);
        }

        public static void PropertySet(COMObject comObject, string name, object[] value, ParameterModifier[] paramModifiers)
        {
            ValidateObject(comObject);
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, value, paramModifiers, Settings.ThreadCulture, null);
        }

        public static void PropertySet(COMObject comObject, string name, object[] value)
        {
            ValidateObject(comObject);
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, value, Settings.ThreadCulture);
        }

        #endregion

        #region Parameters

        public static ParameterModifier[] CreateParamModifiers(params bool[] isRef)
        {
            if (null != isRef)
            {
                int parramArrayCount = isRef.Length;
                ParameterModifier newModifiers = new ParameterModifier(parramArrayCount);
                
                for (int i = 0; i < parramArrayCount; i++)
                    newModifiers[i] = isRef[i];

                ParameterModifier[] returnModifiers = { newModifiers };
                return returnModifiers;
            }
            else
                return null;
        }

        public static object ValidateParam(object param)
        {
            if (null != param)
            {
                IObject comObject = param as IObject;
                if (null != comObject)
                        param = comObject.UnderlyingObject;

                return param;
            }
            else
                return null;
        }

        public static object[] ValidateParamsArray(params object[] paramsArray)
        {
            if (null != paramsArray)
            {
                int parramArrayCount = paramsArray.Length;
                for (int i = 0; i<parramArrayCount; i++)
                    paramsArray[i] = ValidateParam(paramsArray[i]);
                return paramsArray;
            }
            else
                return null;
        }
     
        public static void ReleaseParam(object param)
        {
            if (null != param)
            {
                if (param is COMObject)
                {
                    COMObject comObject = param as COMObject;
                    comObject.Dispose();
                }
                else if (param is COMVariant)
                {
                    COMVariant comVariant = param as COMVariant;
                    comVariant.Dispose();
                }
                else
                {
                    Type paramType = param.GetType();
                    if (true == paramType.IsCOMObject)
                        Marshal.ReleaseComObject(param);
                }
            }
        }

        public static void ReleaseParamsArray(params object[] paramsArray)
        {
            if (null != paramsArray)
            {
                int parramArrayCount = paramsArray.Length;
                for (int i = 0; i < parramArrayCount; i++)
                    ReleaseParam(paramsArray[i]);
            }
        }

        public static object[] CreateEventParamsArray(params object[] paramsArray)
        {
            object[] returnArray = null;
            if (null != paramsArray)
            {
                int parramArrayCount = paramsArray.Length;
                for (int i = 0; i < parramArrayCount; i++)
                    returnArray[i] = paramsArray[i];
                return returnArray;
            }
            else
                return null;
        }

        public static object[] CreateEventParamsArray(bool[] paramsModifier, params object[] paramsArray)
        {
            object[] returnArray = null;
            if (null != paramsArray)
            {
                int parramArrayCount = paramsArray.Length;
                for (int i = 0; i < parramArrayCount; i++)
                {
                    if (true == paramsModifier[i])
                        returnArray[i] = paramsArray[i];
                    else
                        returnArray.SetValue(paramsArray[i], i);
                }
                return returnArray;
            }
            else
                return null;
        }

        #endregion

        #region Private Methods

        private static void ValidateObject(COMVariant variant)
        {
            if (null == variant.UnderlyingObject)
                throw (new LateBindingApiException("UnderlyingObject is not initalized. This indicates you try to use a Interface in a wrong way."));
        }

        #endregion
    }
}
