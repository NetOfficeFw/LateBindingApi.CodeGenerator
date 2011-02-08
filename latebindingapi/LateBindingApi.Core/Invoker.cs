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
            Method(comObject, name, null);
        }

        public static void Method(COMObject comObject, string name, object[] paramArray)
        {
            paramArray = ValidateParamArray(paramArray);
            comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramArray, Settings.ThreadCulture);
        }

        public static object MethodReturn(COMObject comObject, string name)
        {
            return MethodReturn(comObject, name);
        }

        public static object MethodReturn(COMObject comObject, string name, object[] paramArray)
        {
            paramArray = ValidateParamArray(paramArray);
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramArray, Settings.ThreadCulture);
            return returnValue;
        }

        #endregion

        #region Property

        public static object PropertyGet(COMObject comObject, string name)
        { 
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.GetProperty, null, comObject.UnderlyingObject, null, Settings.ThreadCulture);
            return returnValue;
        }

        public static object PropertyGet(COMObject comObject, string name, object[] paramArray)
        {
            paramArray = ValidateParamArray(paramArray);
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.GetProperty, null, comObject.UnderlyingObject, paramArray, Settings.ThreadCulture);
            return returnValue;
        }

        public static void PropertySet(COMObject comObject, string name, object value)
        {
            value = ValidateParam(value);
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, new object[]{value}, Settings.ThreadCulture);
        }

        public static void PropertySet(COMObject comObject, string name, object[] paramArray, object value)
        {
            paramArray = ValidateParamArray(paramArray);
            value = ValidateParam(value);
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, new object[] { value }, Settings.ThreadCulture);
        }

        #endregion

        #region Methods

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

        public static object[] ValidateParamArray(object[] paramArray)
        {
            if (null != paramArray)
            {
                int parramArrayCount = paramArray.Length;
                for (int i = 0; i<parramArrayCount; i++)
                {
                    paramArray[i] = ValidateParam(paramArray[i]);
                }
                return paramArray;
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

        public static void ReleaseParamArray(object[] paramArray)
        {
            if (null != paramArray)
            {
                int parramArrayCount = paramArray.Length;
                for (int i = 0; i < parramArrayCount; i++)
                {
                    ReleaseParam(paramArray[i]);
                }
            }
        }

        #endregion
    }
}
