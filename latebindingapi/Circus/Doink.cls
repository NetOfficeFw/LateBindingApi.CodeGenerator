VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Doink"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit
Dim propStringRef As String
Dim propStringVal As String

Public Property Get ReadOnlyDoinkProperty(Optional i As Integer) As Doink
    Set ReadOnlyDoinkProperty = Me
End Property

Public Property Get ReadOnlyStringProperty() As String
    ReadOnlyStringProperty = "XReadOnlyStringProperty"
End Property

Public Property Get ReadOnlyIntProperty() As Integer
    ReadOnlyIntProperty = 1
End Property

Public Property Get ReadOnlyLongProperty() As Long
    ReadOnlyLongProperty = 1
End Property

Public Property Get ReadOnlyByteProperty() As Byte
    ReadOnlyByteProperty = 1
End Property

Public Property Get ReadOnlyBooleanProperty() As Boolean
    ReadOnlyBooleanProperty = True
End Property

Public Property Get ReadOnlyCurrencyProperty() As Currency
    ReadOnlyCurrencyProperty = 1
End Property

Public Property Get ReadOnlyDateProperty() As Date
    ReadOnlyDateProperty = 1
End Property

Public Property Get ReadOnlyDoubleProperty() As Double
    ReadOnlyDoubleProperty = 1
End Property

Public Property Get ReadOnlyObjectProperty() As Object
    ReadOnlyObjectProperty = 1
End Property

Public Property Get ReadOnlySingleProperty() As Single
    ReadOnlySingleProperty = 1
End Property

Public Property Get ReadOnlyVariantProperty() As Variant
    ReadOnlyVariantProperty = 1
End Property

Public Property Get TestProperty() As String
    RefStringProperty = propStringRef
End Property

Public Property Let TestProperty(val As String)
   propStringRef = val
End Property

Public Property Get RefStringProperty() As String
    RefStringProperty = propStringRef
End Property

Public Property Let RefStringProperty(ByRef val As String)
   propStringRef = val
End Property

Public Property Get ValStringProperty() As String
    ValStringProperty = propStringVal
End Property

Public Property Let ValStringProperty(ByVal val As String)
   propStringVal = val
End Property


Public Property Get ArrayStringProperty() As String()
   Dim x(0 To 4) As String
    x(0) = "Element0"
    x(1) = "Element1"
    x(2) = "Element2"
    x(3) = "Element3"
    ArrayStringProperty = x
End Property

Public Property Get ArrayIntProperty() As Integer()
   Dim x(0 To 4) As Integer
    x(0) = "0"
    x(1) = "1"
    x(2) = "2"
    x(3) = "3"
    ArrayIntProperty = x
End Property

Public Property Get ArrayLongProperty() As Long()
   Dim x(0 To 4) As Long
    x(0) = "0"
    x(1) = "1"
    x(2) = "2"
    x(3) = "3"
    ArrayLongProperty = x
End Property

Public Property Get ArrayDoubleProperty() As Double()
   Dim x(0 To 4) As Double
    x(0) = "0"
    x(1) = "1"
    x(2) = "2"
    x(3) = "3"
    ArrayDoubleProperty = x
End Property

Public Property Get ArraySingleProperty() As Single()
   Dim x(0 To 4) As Single
    x(0) = "0"
    x(1) = "1"
    x(2) = "2"
    x(3) = "3"
    ArraySingleProperty = x
End Property

Public Property Get ArrayCurrencyProperty() As Currency()
   Dim x(0 To 4) As Currency
    x(0) = "0"
    x(1) = "1"
    x(2) = "2"
    x(3) = "3"
    ArrayCurrencyProperty = x
End Property

Public Property Get ArrayBooleanProperty() As Boolean()
   Dim x(0 To 4) As Boolean
    x(0) = False
    x(1) = False
    x(2) = False
    x(3) = False
    ArrayBooleanProperty = x
End Property

Public Property Get ArrayVariantProperty() As Variant()
   Dim x(0 To 4) As Variant
    x(0) = False
    x(1) = False
    x(2) = False
    x(3) = False
    ArrayVariantProperty = x
End Property

Public Property Get ArrayObjectProperty() As Object()
   Dim x(0 To 4) As Object
    x(0) = False
    x(1) = False
    x(2) = False
    x(3) = False
    ArrayObjectProperty = x
End Property

Public Property Get ArrayByteProperty() As Byte()
   Dim x(0 To 4) As Byte
    x(0) = 1
    x(1) = 1
    x(2) = 1
    x(3) = 1
    ArrayByteProperty = x
End Property

Public Property Get ArrayDateProperty() As Date()
   Dim x(0 To 4) As Date
    x(0) = 1
    x(1) = 1
    x(2) = 1
    x(3) = 1
    ArrayDateProperty = x
End Property

Public Sub TestMethodIntegerParamArray(ArrayParam() As Integer)

End Sub

Public Sub TestMethodIntegerParam(param As Integer)

End Sub

Public Sub TestMethodStringParamArray(ArrayParam() As String)
 Dim x&, y&
 x = LBound(ArrayParam)
 y = UBound(ArrayParam)
 MsgBox x & " " & y & " " & ArrayParam(1)
End Sub

Public Sub TestMethodStringParam(param As String)

End Sub

Public Sub TestMethodDoinkParamArray(ArrayParam() As Doink)

End Sub

Public Function TestArrayArray(ByRef ArrayParam() As Doink) As Doink()
     TestArrayArray = ArrayParam
End Function

Public Function TestFunctionDoinkVal(ByVal param As Doink) As String
    param.ValStringProperty = "TestFunctionDoinkVal"
End Function

Public Sub TestMethodDoinkParam(param As Doink)
    param.ValStringProperty = "TestMethodDoinkParam"
End Sub

Public Function TestFunctionDoinkRef(ByRef param As Doink) As String
    param.ValStringProperty = "TestFunctionDoinkRef"
End Function

Public Function FunctionArrayStringProperty() As String()
   Dim x(0 To 4) As String
    x(0) = "Element0"
    x(1) = "Element1"
    x(2) = "Element2"
    x(3) = "Element3"
    x(4) = "Element4"
    FunctionArrayStringProperty = x
End Function

Public Function FunctionArrayDoinkProperty() As Doink()
     Dim x(0 To 4) As Doink
     Set x(0) = New Doink
     Set x(1) = New Doink
     Set x(2) = New Doink
     Set x(3) = New Doink
     Set x(4) = New Doink
    FunctionArrayDoinkProperty = x
End Function

Public Function FunctionObjectArray() As Object()
     Dim x(0 To 4) As Object
     Set x(0) = New Doink
     Set x(1) = New Doink
     Set x(2) = New Doink
     Set x(3) = New Doink
     Set x(4) = New Doink
    FunctionObjectArray = x
End Function

Public Function FunctionObject() As Doink
    Set FunctionObject = New Doink
End Function

Public Function FunctionVariantDoink() As Variant
    Set FunctionVariantDoink = New Doink
End Function

Public Function FunctionVariantBool() As Variant
      FunctionVariantBool = True
End Function

Public Function FunctionVariantArray() As Variant()
     Dim x(0 To 4) As Variant
     Set x(0) = New Doink
     x(1) = True
     Set x(2) = New Doink
     x(3) = "Hello"
     Set x(4) = New Doink
     FunctionVariantArray = x
End Function


Public Function TestFunctionBoolRef(ByRef boolParam As Boolean) As String
boolParam = Not boolParam
TestFunctionBoolRef = "Thanks"
End Function

Public Function TestFunctionStringRef(ByRef param As Integer) As String

End Function

Public Function TestFunctionStringVal(ByVal param As Integer) As String

End Function

Public Function TestFunctionOptionals(Optional param As Integer, Optional myOptional As Long = 199) As String

End Function
