using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public static class ParameterApi
    {
        private static string[] _Keywords;
        
        /// <summary>
        /// call ValidateParameters for all method nodes
        /// </summary>
        /// <param name="methodsNode"></param>
        public static void ValidateItems(XElement enumeratorNode, string itemName, Settings settings)
        {
            foreach (XElement methodNode in enumeratorNode.Elements(itemName))
            {
                if ("_NewEnum" == methodNode.Attribute("Name").Value)
                    continue;

                if ("_Default" == methodNode.Attribute("Name").Value)
                {
                    bool hasParams = ParameterApi.HasParams(methodNode);
                    bool hasItem = EnumerableApi.HasItem(enumeratorNode.Parent);
                    if ((hasParams) && (!hasItem))
                    {
                        foreach (XElement itemParameters in methodNode.Elements("Parameters"))
                            RemoveRefAttributes(itemParameters);
                        
                        methodNode.Add(new XAttribute("Underlying", methodNode.Attribute("Name").Value));
                        methodNode.Attribute("Name").Value = "this";
                        continue;
                    }
                }
                else if ("Item" == methodNode.Attribute("Name").Value)
                {
                    bool hasParams = ParameterApi.HasParams(methodNode);
                    if (hasParams)
                    { 
                        foreach (XElement itemParameters in methodNode.Elements("Parameters"))
                            RemoveRefAttributes(itemParameters);

                        methodNode.Add(new XAttribute("Underlying", methodNode.Attribute("Name").Value));
                        methodNode.Attribute("Name").Value = "this";
                        continue;
                    }
                }

                if (itemName == "Property")
                    ParameterApi.xValidateParameters(methodNode);
            }
        }

        internal static string ValidateParamName(Settings settings, string name)
        {
            if (null == _Keywords)
            {
                string res = RessourceApi.ReadString("Keywords.txt");
                _Keywords =  res.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (true == settings.ConvertParamNamesToCamelCase)
                name = name.Substring(0, 1).ToLower() + name.Substring(1);
            
            bool isInList = false;
            foreach (string item in _Keywords)
            {
                if (item == name)
                {
                    isInList = true;
                    break;
                }
            }
            if (true == isInList)
                name = "_" + name;

            return name;
        }

        /// <summary>
        /// spilt parameters nodes to optional and non-optional version
        /// remove doublettes and copy version attributes reflibs etc. from delete doublette to origin
        /// </summary>
        /// <param name="methodsNode"></param>
        internal static void xValidateParameters(XElement methodNode)
        {
            // create overloads
            List<XElement> addList = new List<XElement>();
            foreach (var itemParameters in methodNode.Elements("Parameters"))
            {
                bool hasOptionals = ParameterApi.HasOptionalParams(itemParameters);
                if (true == hasOptionals)
                {
                    XElement mirrorNode = SplitParametersNode(itemParameters);
                    addList.Add(mirrorNode);
                }
            }

            foreach (XElement item in addList)
                methodNode.Add(item);

            // delete doublettes
            List<XElement> deleteList = new List<XElement>();
            foreach (var itemParameters in methodNode.Elements("Parameters"))
            {
                bool isInDeleteList = false;
                foreach (XElement item in deleteList)
                {
                    if (item == itemParameters)
                    {
                        isInDeleteList = true;
                        break;
                    }
                }

                if (false == isInDeleteList)
                {
                    List<XElement> mirrorNodes = GetEqualsParameterNode(itemParameters);
                    foreach (XElement itemMirror in mirrorNodes)
                    {
                        AddEntities(itemParameters, itemMirror);
                        deleteList.Add(itemMirror);
                    }
                }
            }

            foreach (XElement item in deleteList)
                item.Remove();
        }

        internal static void RemoveRefAttributes(XElement parametersNode)
        {
            foreach (XElement item in parametersNode.Elements("Parameter"))
                item.Attribute("IsRef").Value = "false"; 
        }

        /// <summary>
        /// Get all other parametersNodes in collection there have the same parameters, means doublettes
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        private static List<XElement> GetEqualsParameterNode(XElement parametersNode)
        {
            List<XElement> listToReturn = new List<XElement>();
            XElement methodNode = parametersNode.Parent;

            // special case zero parameters
            if (parametersNode.Elements("Parameter").Count() == 0)
            {
                var paramsNodes = (from a in methodNode.Elements("Parameters")
                                   where a.Elements("Parameter").Count() == 0
                                   select a);

                foreach (XElement item in paramsNodes)
                {
                    if (item != parametersNode)
                        listToReturn.Add(item);
                }
                return listToReturn;
            }

            foreach (XElement itemParameters in methodNode.Elements("Parameters"))
            {
                if (itemParameters != parametersNode)
                {
                    if (itemParameters.Elements("Parameter").Count() == parametersNode.Elements("Parameter").Count())
                    {
                        bool isSame = true;
                        for (int i = 0; i < itemParameters.Elements("Parameter").Count(); i++)
                        {
                            XElement parameter1 = itemParameters.Elements("Parameter").ElementAt(i);
                            XElement parameter2 = parametersNode.Elements("Parameter").ElementAt(i);

                            if (true != parameter1.Attribute("Type").Value.Equals(parameter2.Attribute("Type").Value))
                            {
                                isSame = false;
                                break;
                            }
                        }
                        if (true == isSame)
                            listToReturn.Add(itemParameters);
                    }
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// add reflibs from doubletteNode to parametersNode
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <param name="doubletteNode"></param>
        private static void AddEntities(XElement parametersNode, XElement doubletteNode)
        {
            foreach (XElement doubletteRef in doubletteNode.Element("RefLibraries").Elements("Ref"))
            {
                string key = doubletteRef.Attribute("Key").Value;

                XElement paramsRef = (from a in parametersNode.Element("RefLibraries").Elements("Ref")
                                      where a.Attribute("Key").Equals(key)
                                      select a).SingleOrDefault();

                if (null == paramsRef)
                {
                    XElement newRef = new XElement(doubletteRef);
                    parametersNode.Element("RefLibraries").Add(newRef);
                }
            }
        }

        /// <summary>
        /// spilt parameters nodes to optional and non-optional version
        /// returns a non optional version
        /// </summary>
        /// <param name="itemParameters"></param>
        /// <returns></returns>
        private static XElement SplitParametersNode(XElement itemParameters)
        {
            XElement newParameters = new XElement(itemParameters);
            IEnumerable<XElement> optionals = (from a in newParameters.Elements("Parameter")
                                               where a.Attribute("IsOptional").Value.Equals("true")
                                               select a);

            optionals.Remove();

            return newParameters;
        }

        /// <summary>
        /// get parameter nodes of parametersNode
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <param name="withOptionals"></param>
        /// <returns></returns>
        internal static IEnumerable<XElement> GetParameter(XElement parametersNode, bool withOptionals)
        {
            IEnumerable<XElement> returnParams = null;
            if (true == withOptionals)
            {
                returnParams = (from a in parametersNode.Elements("Parameter")
                           select a);
            }
            else
            {
                returnParams = (from a in parametersNode.Elements("Parameter")
                                 where a.Attribute("IsOptional").Value.Equals("false")
                           select a);
            }
            return returnParams;
        }

        /// <summary>
        /// parametersNode includes ref Parameter
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static bool HasRefOrOutParamsParams(XElement parametersNode, bool withOptionals)
        {
            IEnumerable<XElement> xParams =null;
            if (true == withOptionals)
            {

                xParams = (from a in parametersNode.Elements("Parameter")
                           where a.Attribute("IsRef").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase) || a.Attribute("IsOut").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                           select a);
            }
            else
            {
               xParams = (from a in parametersNode.Elements("Parameter")
                          where a.Attribute("IsRef").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase) || a.Attribute("IsOut").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase) &&
                                 a.Attribute("IsOptional").Value.Equals("false", StringComparison.InvariantCultureIgnoreCase)
                           select a);
            }
            return (xParams.Count() > 0);
        }

        internal static bool HasOutParamsParams(XElement parametersNode, bool withOptionals)
        {
            IEnumerable<XElement> xParams = null;
            if (true == withOptionals)
            {

                xParams = (from a in parametersNode.Elements("Parameter")
                           where  a.Attribute("IsOut").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                           select a);
            }
            else
            {
                xParams = (from a in parametersNode.Elements("Parameter")
                           where a.Attribute("IsOut").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase) &&
                                  a.Attribute("IsOptional").Value.Equals("false", StringComparison.InvariantCultureIgnoreCase)
                           select a);
            }
            return (xParams.Count() > 0);
        }

        /// <summary>
        /// returns parameters node has optional params
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static bool HasOptionalParams(XElement parametersNode)
        {
            XElement paramNode = (from a in parametersNode.Elements("Parameter")
                                  where a.Attribute("IsOptional").Value.Equals("true")
                                  select a).FirstOrDefault();

            return (paramNode != null);
        }

        /// <summary>
        /// returns entityNode node has params
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static bool HasParams(XElement entityNode)
        {
            foreach (var item in entityNode.Elements("Parameters"))
            {
                XElement paramNode = (from a in item.Elements("Parameter")
                                      select a).FirstOrDefault();

                if (paramNode != null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// count of parameter in parametersNode
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <param name="withOptionals"></param>
        /// <returns></returns>
        internal static int GetParamsCount(XElement parametersNode, bool withOptionals)
        {
            if (false == withOptionals)
            {
                var xParams = (from a in parametersNode.Elements("Parameter")
                               where a.Attribute("IsOptional").Value.Equals("false")
                               select a);
                return xParams.Count();
            }
            else
            {
                return parametersNode.Elements("Parameter").Count();
            }          
        }

        /// <summary>
        /// convert parameters node to function call parameter string
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <param name="withOptionals"></param>
        /// <returns></returns>
        internal static string CreateParametersCallString(Settings settings, XElement parametersNode, bool withOptionals, bool convertOptionalsInNet4)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(parametersNode.Parent.Parent.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(parametersNode.Parent.Parent.Parent);

            string parameters = "";
            int countOfParams = GetParamsCount(parametersNode, withOptionals);
            int i = 1;

            IEnumerable<XElement> xParams = GetParameter(parametersNode, withOptionals);
            foreach (XElement itemParam in xParams)
            {
                string parameter = "";
                string type = CSharpGenerator.GetQualifiedType(settings, itemParam);
                if ("true" == itemParam.Attribute("IsArray").Value)
                    type += "[]";

                string name = itemParam.Attribute("Name").Value;
                name = ValidateParamName(settings, name);
                
                if ("true" == itemParam.Attribute("IsOut").Value)
                    name = "out " + name;
                else if ("true" == itemParam.Attribute("IsRef").Value)
                    name = "ref " + name;

                parameter = name;
                if (i < countOfParams)
                    parameter += ", ";

                parameters += parameter;
                i++;
                if (i > countOfParams)
                    break;
            }
            return parameters;
        }

        /// <summary>
        /// convert parameters node to function prototype parameter string
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <param name="withOptionals"></param>
        /// <returns></returns>
        internal static string CreateParametersPrototypeString(Settings settings, XElement parametersNode, bool withOptionals, bool convertOptionalsInNet4)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(parametersNode.Parent.Parent.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(parametersNode.Parent.Parent.Parent);
    
            string parameters = "";
            int countOfParams = GetParamsCount(parametersNode, withOptionals);
            int i = 1;
            
            //string optionalValue = "";

           // if (("4.0" == settings.Framework) && ((true == convertOptionalsInNet4) || ("INVOKE_PROPERTYGET" == parametersNode.Parent.Attribute("InvokeKind").Value)))
            //    optionalValue = "=null";
 
            IEnumerable<XElement> xParams = GetParameter(parametersNode, withOptionals);
            foreach (XElement itemParam in xParams)
            {               
                string parameter = "";
                string type = CSharpGenerator.GetQualifiedType(settings, itemParam);
                if ("true" == itemParam.Attribute("IsArray").Value)
                    type += "[]";

                if ("true" == itemParam.Attribute("IsOut").Value)
                    type = "out " + type;
                else if ("true" == itemParam.Attribute("IsRef").Value)
                    type = "ref " + type;

                string name = itemParam.Attribute("Name").Value;
                name = ValidateParamName(settings, name);

                parameter = type + " " + name;
                if (i < countOfParams)
                    parameter += ", ";                

                parameters += parameter;
                i++;
                if (i > countOfParams)
                    break;
            }
            return parameters;
        }
        
        /// <summary>
        /// convert parameters node to set-parameters-to-array string 
        /// </summary>
        /// <param name="numberOfTabSpaces"></param>
        /// <param name="parametersNode"></param>
        /// <param name="withOptionals"></param>
        /// <returns></returns>
        internal static string CreateParametersSetArrayString(Settings settings, int numberOfTabSpaces, XElement parametersNode, bool withOptionals)
        {
            bool hasRefParams = HasRefOrOutParamsParams(parametersNode, withOptionals);
            if (true == hasRefParams)
                return ConvertParametersToSetArrayWithRef(settings, numberOfTabSpaces, parametersNode, withOptionals);
            else
                return ConvertParametersToSetArrayWithoutRef(settings, numberOfTabSpaces, parametersNode, withOptionals);
        }

        /// <summary>
        ///  convert parameters node to set-parameters-to-ParameterModifiers string
        /// </summary>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreateParametersModifiersString(XElement parametersNode, bool withOptionals)
        {
            string stringReturn = "";

            IEnumerable<XElement> xParams = GetParameter(parametersNode, withOptionals);
            foreach (XElement item in xParams)
            {
                if( ("true" == item.Attribute("IsRef").Value) || ("true" == item.Attribute("IsOut").Value))
                    stringReturn += "true";
                else
                    stringReturn += "false";
                
                stringReturn += ",";
            }

            if (("" != stringReturn) && ("," == stringReturn.Substring(stringReturn.Length - 1)))
                stringReturn = stringReturn.Substring(0, stringReturn.Length - 1);

            return stringReturn;
        }

        /// <summary>
        /// convert parameters node to update ref parameter string
        /// </summary>
        /// <param name="numberOfTabSpaces"></param>
        /// <param name="parametersNode"></param>
        /// <param name="withOptionals"></param>
        /// <returns></returns>
        internal static string CreateParametersToRefUpdateString(Settings settings, int numberOfTabSpaces, XElement parametersNode, bool withOptionals)
        {
            string result = "";

            string tabSpace = CSharpGenerator.TabSpace(numberOfTabSpaces);
            int countOfParams = GetParamsCount(parametersNode, withOptionals);

            if (0 == countOfParams)
                return "";

            int i = 0;
            IEnumerable<XElement> xParams = GetParameter(parametersNode, withOptionals);
            foreach (XElement itemParam in xParams)
            {
                string isArray = "";
                if ("true" == itemParam.Attribute("IsArray").Value)
                    isArray = "[]";
               
                string paramName = itemParam.Attribute("Name").Value;
                paramName = ValidateParamName(settings, paramName);

                if (("true" == itemParam.Attribute("IsRef").Value) || ("true" == itemParam.Attribute("IsOut").Value))
                    result += tabSpace + paramName + 
                        " = (" + CSharpGenerator.GetQualifiedType(itemParam) +  isArray + 
                        ")paramsArray[" + i.ToString() + "];\r\n";
                
                i++;
            }

            return result;
        }

        private static bool IsRecord(XElement itemParam)
        {
            XElement project = itemParam.Parent;
            while (project.Name != "Project")
                project = project.Parent;

            XElement recordNode = (from a in project.Element("Records").Elements("Record") select a).FirstOrDefault();
            return (null != recordNode);
        }

        private static string CreateParameterClearString(Settings settings, IEnumerable<XElement> xParams, string tabSpace)
        {
            string result = "";
            foreach (XElement itemParam in xParams)
            {
                if ("true" == itemParam.Attribute("IsOut").Value)
                {
                    string paramName = itemParam.Attribute("Name").Value;
                    paramName = ValidateParamName(settings, paramName);

                    if (itemParam.Attribute("IsArray").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                           result += tabSpace + paramName + " = null;" + Environment.NewLine;
                    else if(itemParam.Attribute("Type").Value.Equals("String",StringComparison.InvariantCultureIgnoreCase))
                          result += tabSpace + paramName + " = string.Empty;" + Environment.NewLine;
                    else if (itemParam.Attribute("Type").Value.Equals("Object", StringComparison.InvariantCultureIgnoreCase))
                        result += tabSpace + paramName + " = null;" + Environment.NewLine;
                    else if (itemParam.Attribute("Type").Value.Equals("UIntPtr", StringComparison.InvariantCultureIgnoreCase))
                        result += tabSpace + paramName + " = UIntPtr.Zero;" + Environment.NewLine;
                    else if (itemParam.Attribute("Type").Value.Equals("Bool", StringComparison.InvariantCultureIgnoreCase))
                        result += tabSpace + paramName + " = false;" + Environment.NewLine;
                    else if (itemParam.Attribute("Type").Value.Equals("Int32", StringComparison.InvariantCultureIgnoreCase))
                        result += tabSpace + paramName + " = 0;" + Environment.NewLine;
                    else if (itemParam.Attribute("Type").Value.Equals("Int16", StringComparison.InvariantCultureIgnoreCase))
                        result += tabSpace + paramName + " = 0;" + Environment.NewLine;
                    else if (itemParam.Attribute("Type").Value.Equals("Guid", StringComparison.InvariantCultureIgnoreCase))
                        result += tabSpace + paramName + " = Guid.Empty;" + Environment.NewLine;                    
                    else if (itemParam.Attribute("IsEnum").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                        result += tabSpace + paramName + " = 0;" + Environment.NewLine;
                    else if (itemParam.Attribute("IsComProxy").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                            result += tabSpace + paramName + " = null;" + Environment.NewLine;
                    else if (IsRecord(itemParam))
                    {
                        XElement project = itemParam.Parent;
                        while (project.Name != "Project")
                            project = project.Parent;
                        result += tabSpace + paramName + " = new " + project.Attribute("Namespace").Value + "." + itemParam.Attribute("Type").Value + "();" + Environment.NewLine;
                    }
                    else
                        result += tabSpace + paramName + " = 0;" + Environment.NewLine;             
                }
            }
            return result;
        }

        private static string ConvertParametersToSetArrayWithRef(Settings settings, int numberOfTabSpaces, XElement parametersNode, bool withOptionals)
        {
            IEnumerable<XElement> xParams = GetParameter(parametersNode, withOptionals);

            string tabSpace = CSharpGenerator.TabSpace(numberOfTabSpaces);

            if (0 == parametersNode.Elements("Parameter").Count())
                return tabSpace + "object[] paramsArray = null;\r\n";

            string result = tabSpace + "ParameterModifier[] modifiers = Invoker.CreateParamModifiers(" + CreateParametersModifiersString(parametersNode, withOptionals) + ");\r\n";
            result += CreateParameterClearString(settings, xParams, tabSpace);
            
            result += tabSpace + "object[] paramsArray = Invoker.ValidateParamsArray(";
            int countOfParams = GetParamsCount(parametersNode, withOptionals);
            int i = 1;

           
            foreach (XElement itemParam in xParams)
            {
                if ("true" == itemParam.Attribute("IsArray").Value)
                    result += "(object)";

                string paramName = itemParam.Attribute("Name").Value;
                paramName = ValidateParamName(settings, paramName);
 
                result += paramName;

                if (i < countOfParams)
                    result += ", ";
                i++;
                if (i > countOfParams)
                    break;
            }
            result += ");\r\n";
            return result;
        }

        private static string ConvertParametersToSetArrayWithoutRef(Settings settings, int numberOfTabSpaces, XElement parametersNode, bool withOptionals)
        {
            string tabSpace = CSharpGenerator.TabSpace(numberOfTabSpaces);

            if (0 == parametersNode.Elements("Parameter").Count())
                return tabSpace + "object[] paramsArray = null;\r\n";

            string result = tabSpace + "object[] paramsArray = Invoker.ValidateParamsArray(";
            int countOfParams = GetParamsCount(parametersNode, withOptionals);
            int i = 1;

            IEnumerable<XElement> xParams = GetParameter(parametersNode, withOptionals);
            foreach (XElement itemParam in xParams)
            {
                if ("true" == itemParam.Attribute("IsArray").Value)
                    result += "(object)";

                string paramName = itemParam.Attribute("Name").Value;
                paramName = ValidateParamName(settings, paramName);

                result += paramName;

                if (i < countOfParams)
                    result += ", ";
                i++;
                if (i > countOfParams)
                    break;
            }
            result += ");\r\n";
            return result;
        }
    }
}
