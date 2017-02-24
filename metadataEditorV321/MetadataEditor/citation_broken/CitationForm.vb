Public Class CitationForm
    Private base As String

    Sub New(base As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.base = base
    End Sub

    Private Sub btnEdit_lworkcit_Click(sender As System.Object, e As System.EventArgs) Handles btnEdit_lworkcit.Click
        Dim frm As New CitationForm("")
        frm.ShowDialog()
    End Sub

    Private Sub btnCloseSave_Click(sender As System.Object, e As System.EventArgs) Handles btnCloseSave.Click
        Me.Close()
    End Sub

    Private Sub btnCloseDiscard_Click(sender As System.Object, e As System.EventArgs) Handles btnCloseDiscard.Click
        If MessageBox.Show("Do you really want to cancel?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            'saved = False
            Me.Close()
        End If
    End Sub
End Class