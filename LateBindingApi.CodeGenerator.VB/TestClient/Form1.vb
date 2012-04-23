Imports NetOffice
Imports xyz

Public Class Form1

    Dim _excelApp As Excel.Application
    Dim WithEvents _workBook As Excel.Workbook

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Factory.Initialize()

        _excelApp = New Excel.Application()
        _excelApp.Visible = True
        _workBook = _excelApp.Workbooks.Add()

        Dim worksheet As Excel.Worksheet = _workBook.Worksheets.Add()
         
    End Sub

    Private Sub _workBook_BeforeCloseEvent(ByRef Cancel As Boolean) Handles _workBook.BeforeCloseEvent
        Cancel = True
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        _excelApp.Quit()
        _excelApp.Dispose()
    End Sub
End Class
