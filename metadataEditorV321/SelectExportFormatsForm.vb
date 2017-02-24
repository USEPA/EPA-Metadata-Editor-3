Public Class SelectExportFormatsForm

    Public Function getSelectedFormats() As List(Of String)
        getSelectedFormats = New List(Of String)
        'Add MP output options based on selection
        If cbExportTxt.Checked Then getSelectedFormats.Add("t")
        If cbExportTxt.Checked Then getSelectedFormats.Add("h")
    End Function

    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub
End Class