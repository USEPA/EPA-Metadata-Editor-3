Public Class ContactForm

    ' Row object that the form is working with
    Dim srcRow As DataGridViewRow

    ' Create new contact form and prep for use
    Sub New(fromRow As DataGridViewRow)
        MyBase.New()
        InitializeComponent()

        srcRow = fromRow
        populate()
    End Sub


    ''' <summary>
    ''' Close the form, saving its contents.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCloseSave_Click(sender As System.Object, e As System.EventArgs) Handles btnCloseSave.Click
        'saved = True

        Dim dr As DataRowView = Me.cntinfo.SelectedItem

        For Each dgvCell As DataGridViewCell In srcRow.Cells
            If Not dgvCell.Visible AndAlso TypeOf dgvCell Is DataGridViewTextBoxCell Then
                ' Clear cell first, then figure out what to put - if anything
                dgvCell.Value = ""
                Dim colName As String = dgvCell.OwningColumn.Name
                Debug.Print(colName)
                If colName.StartsWith("cnt") Then
                    colName = colName.Substring(colName.IndexOf("_") + 1)
                Else
                    colName = stripNonAlphanumeric(colName, "")
                End If
                If dr.DataView.Table.Columns.Contains(colName) Then
                    ' and set the column value to tag value.
                    Dim val = nv(dr(colName))
                    If dgvCell.OwningColumn.Name.StartsWith("cntorgp") Then
                        If Me.cntinfo_cntorgp.Checked Then
                            dgvCell.Value = val
                        End If
                    ElseIf dgvCell.OwningColumn.Name.StartsWith("cntperp") Then
                        If Me.cntinfo_cntperp.Checked Then
                            dgvCell.Value = val
                        End If
                    Else
                        dgvCell.Value = val
                    End If
                Else
                    Dim a = 1
                End If
            End If
        Next

        Me.Close()

    End Sub

    ''' <summary>
    ''' Close the form, discarding its contents.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCloseDiscard_Click(sender As System.Object, e As System.EventArgs) Handles btnCloseDiscard.Click
        If MessageBox.Show("Do you really want to cancel?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            'saved = False
            Me.Close()
        End If

    End Sub


    ''' <summary>
    ''' Populate form fields using information from database.
    ''' </summary>
    ''' <remarks></remarks>
    Sub populate()
        Dim ctrl As ComboBox = Me.cntinfo
        ctrl.DropDownStyle = ComboBoxStyle.DropDownList

        Dim SQLStr As String = ""
        SQLStr = "SELECT * FROM [" & "Contact_Information" & "_cluster]"
        SQLStr &= " ORDER BY orderedId"

        'Debug.Print(SQLStr)
        Dim dt As DataTable = Utils.datatableFromSQL(SQLStr)


        Dim dr As DataRow = dt.NewRow()
        Dim tagValue As String

        For Each dgvCell As DataGridViewCell In srcRow.Cells
            If Not dgvCell.Visible AndAlso TypeOf dgvCell Is DataGridViewTextBoxCell Then
                Dim colName As String = dgvCell.OwningColumn.Name
                Debug.Print(colName)
                If colName.EndsWith("_") Then
                    colName = colName.Substring(0, colName.Length - 1)
                ElseIf colName.Contains("_") Then
                    colName = colName.Substring(colName.IndexOf("_") + 1)
                End If
                If colName = "OrderedId" AndAlso (dgvCell.Value Is Nothing OrElse dgvCell.Value.ToString.Trim = "" OrElse dgvCell.Value = -9999) Then
                    dgvCell.Value = -9999
                    dr("default") = False
                    dr("clusterInfo") = dr("cntorg") & " | " & dr("cntper") & " (from metadata record)"
                End If
                If dr.Table.Columns.Contains(colName) Then
                    ' and set the column value to tag value.
                    tagValue = Utils.nv(dgvCell.Value, "")
                    If tagValue IsNot Nothing AndAlso tagValue.Trim() > "" Then
                        dr(colName) = tagValue

                        Dim rb As RadioButton
                        If dgvCell.OwningColumn.Name.StartsWith("cntorgp") Then
                            rb = Me.cntinfo_cntorgp
                            rb.Checked = True
                        ElseIf dgvCell.OwningColumn.Name.StartsWith("cntperp") Then
                            rb = Me.cntinfo_cntperp
                            rb.Checked = True
                        End If

                    End If
                End If
            End If
        Next

        ctrl.DisplayMember = "clusterInfo"
        ctrl.ValueMember = "OrderedId"
        If dr("OrderedId") < 0 Then
            dt.Rows.Add(dr)
        End If
        ctrl.DataSource = dt
        ctrl.SelectedValue = dr("OrderedId")
        'ctrl.SelectedIndex = ctrl.Items.Count - 1

    End Sub


    ''' <summary>
    ''' Hook into a win32 function
    ''' </summary>
    ''' <param name="hwnd"></param>
    ''' <param name="wMsg"></param>
    ''' <param name="wParam"></param>
    ''' <param name="lParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Declare Auto Function SendMessage Lib "user32" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr

    ''' <summary>
    ''' Override the Pressed Key Processing Routine of the MDI-Parent primarily to be able to pass down ctrl-x/c/v keypresses
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <param name="keyData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        If Me.ActiveMdiChild IsNot Nothing Then SendMessage(Me.ActiveMdiChild.Handle, msg.Msg, msg.WParam, msg.LParam)
    End Function

    ''' <summary>
    ''' Event handler to set a field to its default value
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cntinfo_____default_Click(sender As System.Object, e As System.EventArgs) Handles cntinfo_____default.Click
        Dim ctrl As ComboBox = Me.cntinfo
        ctrl.SelectedText = ""
        For j As Integer = 0 To ctrl.Items.Count - 1
            If ctrl.Items(j)("default") Then
                ctrl.SelectedIndex = j
            End If
        Next
    End Sub
End Class