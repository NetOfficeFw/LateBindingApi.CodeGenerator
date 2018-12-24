using System;
using System.Xml.Linq;
using LateBindingApi.CodeGenerator.CSharp;
using NUnit.Framework;

namespace CodeGenerator.UnitTests
{
    [TestFixture]
    public class CoClassApiTests
    {
        [Test]
        public void GetDelegateSignature_ZeroParameters_ReturnsCorrectSignature()
        {
            // Arrange
            var expected = @"public delegate void Application_StartupEventHandler();";
            var node = XElement.Parse(@"<Method Name=""Startup"" AnalyzeReturn=""true"" Hidden=""true"">
                                          <Parameters>
                                            <ReturnValue Type=""void"" VarType=""VT_VOID"" MarshalAs="""" TypeKind="""" IsComProxy=""false"" IsExternal=""false"" IsEnum=""false"" IsArray=""false"" IsNative=""false"" TypeKey="""" ProjectKey="""" LibraryKey="""" />
                                          </Parameters>
                                        </Method>");

            // Act
            var actual = CoClassApi.GetDelegateSignature("Application", node);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetDelegateSignature_SpecialParameterName_ConvertsNameToLowerCase()
        {
            // Arrange
            var expected = @"public delegate void Application_XMLValidationErrorEventHandler(object xmlNode);";
            var node = XElement.Parse(@"<Method Name=""XMLValidationError"" AnalyzeReturn=""true"" Hidden=""false"">
                                          <Parameters>
                                            <Parameter Name=""XMLNode"" Type=""XMLNode"" TypeKind=""TKIND_DISPATCH"" VarType=""VT_VARIANT"" MarshalAs=""UnmanagedType.Struct"" IsExternal=""false"" IsComProxy=""true"" IsOptional=""false"" HasDefaultValue=""false"" DefaultValue="""" IsEnum=""false"" IsRef=""false"" IsOut=""false"" ParamFlags=""paramflag_fin"" IsArray=""false"" IsNative=""false"" TypeKey=""_x0030_5aa2c2a-e030-4811-bd0d-b93ae10ffb91"" ProjectKey=""_x0035_c457066-d488-4bc1-b7f8-1e90f2a80648"" LibraryKey=""a7768db8-79a7-4f3a-8488-aec582aaf790"" />
                                          </Parameters>
                                        </Method>");

            // Act
            var actual = CoClassApi.GetDelegateSignature("Application", node);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetDelegateSignature_OneParameter_ReturnsCorrectSignature()
        {
            // Arrange
            var expected = @"public delegate void QueryTable_BeforeRefreshEventHandler(ref bool cancel);";
            var node = XElement.Parse(@"<Method Name=""BeforeRefresh"" AnalyzeReturn=""true"" Hidden=""false"">
                                          <Parameters>
                                            <ReturnValue Type=""void"" VarType=""VT_VOID"" MarshalAs="""" TypeKind="""" IsComProxy=""false"" IsExternal=""false"" IsEnum=""false"" IsArray=""false"" IsNative=""false"" TypeKey="""" ProjectKey="""" LibraryKey="""" />
                                            <Parameter Name=""Cancel"" Type=""bool"" TypeKind="""" VarType=""VT_BOOL"" MarshalAs="""" IsExternal=""false"" IsComProxy=""false"" IsOptional=""false"" HasDefaultValue=""false"" DefaultValue="""" IsEnum=""false"" IsRef=""true"" IsOut=""false"" ParamFlags=""paramflag_fin"" IsArray=""false"" IsNative=""true"" TypeKey="""" ProjectKey="""" LibraryKey="""" />
                                          </Parameters>
                                        </Method>");

            // Act
            var actual = CoClassApi.GetDelegateSignature("QueryTable", node);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
