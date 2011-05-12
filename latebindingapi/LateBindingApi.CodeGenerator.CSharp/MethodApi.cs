﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class MethodApi
    {
        /// <summary>
        /// convert all methods to code as string
        /// </summary>
        /// <param name="methodsNode"></param>
        /// <returns></returns>
        internal static string ConvertMethodsLateBindToString(Settings settings, XElement methodsNode)
        {
            ParameterApi.ValidateItems(methodsNode, "Method");

            string result = "\r\n\t\t#region Methods\r\n\r\n";
            foreach (XElement methodNode in methodsNode.Elements("Method"))
            {
                if("_NewEnum" == methodNode.Attribute("Name").Value)
                    continue;

                string method = ConvertMethodLateBindToString(settings, methodNode);
                result += method;
            }
            result += "\t\t#endregion\r\n";
            return result;
        }

        /// <summary>
        /// convert method to code as string
        /// </summary>
        /// <param name="methodNode"></param>
        /// <returns></returns>
        internal static string ConvertMethodLateBindToString(Settings settings, XElement methodNode)
        {
            string inParam = "(";
            string outParam = ")";

            string result = "";
            string name = methodNode.Attribute("Name").Value;
            if ("_Default" == name)
            { 
                name = "this";
                inParam = "[";
                outParam = "]";
            }
            
            foreach (XElement itemParams in methodNode.Elements("Parameters"))
            {
                XElement returnValue = itemParams.Element("ReturnValue");
                
                string method = "";
                if (true == settings.CreateXmlDocumentation)
                    method = DocumentationApi.CreateParameterDocumentation(2, itemParams);

                method += "\t\t" + CSharpGenerator.GetSupportByLibraryAttribute(itemParams) + "\r\n";
                if("this" == name)
                    method += "\t\t" + "[NetRuntimeSystem.Runtime.CompilerServices.IndexerName(\"IndexerItem\")]" + "\r\n";

                string valueReturn = CSharpGenerator.GetQualifiedType(returnValue);
                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "[]";

                method += "\t\tpublic " + valueReturn + " " + name + inParam + "%params%" + outParam + "\r\n\t\t{\r\n%methodbody%\t\t}\r\n";
                string parameters = ParameterApi.CreateParametersPrototypeString(settings, itemParams, true, true);
                method = method.Replace("%params%", parameters);

                string methodBody ="";
                if ("this" == name)
                    methodBody = "\t\t\tget\r\n\t\t\t{\r\n" + CreateLateBindMethodBody(settings, 4, itemParams) + "\t\t\t}\r\n";
                else
                    methodBody = CreateLateBindMethodBody(settings, 3, itemParams);
                
                method = method.Replace("%methodbody%", methodBody);
                result += method + "\r\n";
            }

            return result;
        }

        /// <summary>
        /// convert parametersNode to complete method body code
        /// </summary>
        /// <param name="numberOfRootTabs"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreateLateBindMethodBody(Settings settings, int numberOfRootTabs, XElement parametersNode)
        {
            string tabSpace      = CSharpGenerator.TabSpace(numberOfRootTabs);
            string methodBody    = ParameterApi.CreateParametersSetArrayString(settings, numberOfRootTabs, parametersNode, true);
            XElement returnValue = parametersNode.Element("ReturnValue");
            string methodName    = parametersNode.Parent.Attribute("Name").Value;
            string typeName      = returnValue.Attribute("Type").Value;
            string fullTypeName  = CSharpGenerator.GetQualifiedType(returnValue);

            string objectArrayField = "";
            string arrayField = "";
            string arrayName = "";
            if ("true" == returnValue.Attribute("IsArray").Value)
            {
                arrayField = "[]";
                arrayName = "Array";
                fullTypeName += "[]";
                objectArrayField = "(object[])";
            }

            string modifiers = "";
            if(true == ParameterApi.HasRefParams(parametersNode ,true))
                modifiers = ", modifiers";

            if (typeName != "void") 
            {
                if ("true" == returnValue.Attribute("IsComProxy").Value)
                {
                    methodBody += tabSpace + "object returnItem = Invoker.MethodReturn(this, \"" + methodName + "\", paramsArray" + modifiers + ");\r\n";
                    if (typeName == "COMObject")
                    {
                        methodBody += tabSpace + "COMObject" + arrayField + " newObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(this," + objectArrayField + "returnItem);\r\n";
                        methodBody += "%modifiers%";
                        methodBody += tabSpace + "return newObject;\r\n";
                    }
                    else if (typeName == "COMVariant")
                    {
                        methodBody += tabSpace + "Type returnItemType = Invoker.GetObjectType(returnItem);\r\n";
                        methodBody += tabSpace + "if ((null != returnItem) && (true == returnItemType.IsCOMObject))\r\n" + tabSpace + "{\r\n";
                        if("" == objectArrayField)
                            methodBody += tabSpace + "\tCOMObject" + arrayField + " newObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(this, " + objectArrayField + "returnItem, returnItemType);\r\n";
                        else
                            methodBody += tabSpace + "\tCOMObject" + arrayField + " newObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(this, " + objectArrayField + "returnItem);\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return newObject;\r\n";
                        methodBody += tabSpace + "}\r\n";
                        methodBody += tabSpace + "else" + "\r\n" + tabSpace +"{\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return " + objectArrayField + " returnItem;\r\n";
                        methodBody += tabSpace + "}\r\n";
                    }
                    else
                    {
                        // library type
                        if ("true" == returnValue.Attribute("IsArray").Value)
                        {
                            methodBody += tabSpace + "COMObject[] newObject = LateBindingApi.Core.Factory.CreateObjectArrayFromComProxy(this, " + objectArrayField + "returnItem);\r\n";
                            methodBody += tabSpace + fullTypeName + " returnArray = new " + CSharpGenerator.GetQualifiedType(returnValue) + "[newObject.Length];\r\n";
                            methodBody += tabSpace + "for (int i = 0; i < newObject.Length; i++)\r\n";
                            methodBody += tabSpace + "\treturnArray[i] = newObject[i] as " + CSharpGenerator.GetQualifiedType(returnValue) + ";\r\n";
                            methodBody += "%modifiers%";
                            methodBody += tabSpace + "return returnArray;\r\n";
                        }
                        else
                        {
                            methodBody += tabSpace + fullTypeName + " newObject = LateBindingApi.Core.Factory.CreateObjectFromComProxy(this, " + objectArrayField + "returnItem) as " + fullTypeName + ";\r\n";
                            methodBody += "%modifiers%";
                            methodBody += tabSpace + "return newObject;\r\n";
                        }
                    }
                }
                else
                {
                    // native type
                    string objectString = "";
                    if ("true" == returnValue.Attribute("IsArray").Value)
                        objectString = "(object)";

                    methodBody += tabSpace + "object" + " returnItem = " + objectString + "Invoker.MethodReturn" + "(this, \"" + methodName + "\", paramsArray);\r\n";
                    methodBody += "%modifiers%";
                    methodBody += tabSpace + "return (" + fullTypeName + ")returnItem;\r\n";
                }

                if (true == ParameterApi.HasRefParams(parametersNode, true))
                {
                    string modRefs = ParameterApi.CreateParametersToRefUpdateString(settings, numberOfRootTabs, parametersNode, true); 
                    methodBody = methodBody.Replace("%modifiers%", modRefs);
                }
                else                
                {
                    methodBody = methodBody.Replace("%modifiers%", "");
                }                
            }
            else
            {
                methodBody += tabSpace + "Invoker.Method(this, \"" + methodName + "\", paramsArray" + modifiers + ");\r\n";
                methodBody += "%modifiers%";

                if (true == ParameterApi.HasRefParams(parametersNode, true))
                {
                    string modRefs = ParameterApi.CreateParametersToRefUpdateString(settings, numberOfRootTabs, parametersNode, true);
                    methodBody = methodBody.Replace("%modifiers%", modRefs);
                }
                else
                {
                    methodBody = methodBody.Replace("%modifiers%", "");
                }
            }

            return methodBody;
        }

        internal static string ConvertMethodEarlyBindToString(Settings settings, XElement methodNode)
        {
            string result = "";
            string name = methodNode.Attribute("Name").Value;
            foreach (XElement itemParams in methodNode.Elements("Parameters"))
            {
                XElement returnValue = itemParams.Element("ReturnValue");

                string method = "";
                method += "\t\t" + CSharpGenerator.GetSupportByLibraryAttribute(itemParams) + "\r\n";

                string marshalReturnAs = returnValue.Attribute("MarshalAs").Value;
                if ("" != marshalReturnAs)
                    method += "\t\t[return: MarshalAs(" + marshalReturnAs + ")]\r\n";

                method += "\t\t" + "[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(" + itemParams.Parent.Element("DispIds").Element("DispId").Attribute("Id").Value + ")]\r\n";
                method += "\t\t" + GetEarlyBindMethodSignatur(itemParams) + ";\r\n\r\n";
                 
                result += method;
            }
          

            return result;
        }

        internal static string GetEarlyBindMethodSignatur(XElement paramsNode)
        {
            string result = paramsNode.Element("ReturnValue").Attribute("Type").Value + " " + paramsNode.Parent.Attribute("Name").Value + "(";
            foreach (XElement itemParam in paramsNode.Elements("Parameter"))
            {
                string isRef = "";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    isRef = "ref ";

                string par = "[In";
                string marshalAs = itemParam.Attribute("MarshalAs").Value;
                if ("" != marshalAs)
                    marshalAs = ", MarshalAs(" + marshalAs + ")]";

                if (("true" == itemParam.Attribute("IsComProxy").Value) || ("COMObject" == itemParam.Attribute("Type").Value) ||
                    ("COMObject" == itemParam.Attribute("Type").Value) || ("object" == itemParam.Attribute("Type").Value))
                {
                    par += marshalAs + " object " + itemParam.Attribute("Name").Value;
                }
                else
                {
                    par += marshalAs + isRef + itemParam.Attribute("Type").Value + " " + itemParam.Attribute("Name").Value;
                }

                result += par + ", ";
            }
            if (", " == result.Substring(result.Length - 2))
                result = result.Substring(0, result.Length - 2);

            return result + ")";
        }

        internal static string ConvertMethodsEarlyBindToString(Settings settings, XElement methodsNode)
        {
            string result = "\t\t#region Methods\r\n\r\n";
            foreach (XElement methodNode in methodsNode.Elements("Method"))
            {
                if ("_NewEnum" == methodNode.Attribute("Name").Value)
                    continue;

                string method = ConvertMethodEarlyBindToString(settings, methodNode);
                result += method;
            }
            result += "\t\t#endregion\r\n";
            return result;
        }
    }
}
