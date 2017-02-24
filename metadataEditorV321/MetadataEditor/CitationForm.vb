Public Class CitationForm
    Private xm As XmlMetadata

    Private elementsDropdown As New List(Of String) From {"geoform", "pubinfo/publish", "pubinfo/pubplace"}
    Private elementsNonRepeated As New List(Of String) From {"origin", "title", "edition", "geoform", "othercit", "pubdate", "pubtime", "pubinfo/publish", "pubinfo/pubplace", "serinfo/sername", "serinfo/issue"}

    Private ReadOnly Property elementsRepeated As List(Of String)
        Get
            elementsRepeated = New List(Of String)
            For i As Integer = 1 To 10
                elementsRepeated.Add(injectIndex("onlink[?]", i))
            Next
        End Get
    End Property

    Private ReadOnly Property elements As List(Of String)
        Get
            elements = (New List(Of String))
            elements.AddRange(elementsNonRepeated)
            elements.AddRange(elementsRepeated)
        End Get
    End Property

    Private ReadOnly Property elementsHelp As List(Of String)
        Get
            elementsHelp = (New List(Of String))
            elementsHelp.AddRange(elements)
            elementsHelp.Add("lworkcit")
        End Get
    End Property


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


    Sub New(ByRef xmlHolder As Object, resourceName As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Text = "Source citation"

        If xmlHolder Is Nothing Then xmlHolder = New XmlMetadata("<citeinfo/>")
        xm = xmlHolder

        resourceName = resourceName.Trim
        If resourceName <> "" Then
            Me.Text += " for " + resourceName
        End If

        populateDropdowns()
        load_citeinfo()
        InitEventHandlers()
        disableLinkLabelTabStops(Me)
    End Sub


    Private Sub load_citeinfo()
        For Each elt As String In elements
            Try
                Dim ctrl As Control = getControlNamed(Utils.stripNonAlphanumeric(elt, "_"))
                ctrl.Text = xm.SimpleGetProperty("/citeinfo/" + elt)
                If TypeOf ctrl Is ComboBox Then
                    Dim cb As ComboBox = ctrl
                    If ctrl.Text.Trim > "" AndAlso cb.FindString(ctrl.Text) < 0 Then
                        cb.Items.Insert(0, ctrl.Text)
                    End If
                End If
            Catch ex As Exception
                Dim a = 1
            End Try
        Next

        lworkcit_____help.Tag = New XmlMetadata(xm.GetXml("/citeinfo/lworkcit/citeinfo"), "citeinfo")
    End Sub


    Private Sub save_citeinfo()
        xm.SetXml("<citeinfo/>")

        For Each elt As String In elementsNonRepeated
            Try
                Dim val As String = getControlNamed(Utils.stripNonAlphanumeric(elt, "_")).Text.Trim
                If val <> "" Then xm.SetPropertyX("/citeinfo/" + elt, val)
            Catch ex As Exception
                Dim a = 1
            End Try
        Next

        Dim j As Integer = 1
        For i As Integer = 1 To 10
            Try
                Dim elt As String = injectIndex("onlink[?]", i)
                Dim val As String = getControlNamed(Utils.stripNonAlphanumeric(elt, "_")).Text.Trim
                If val = "" Then
                    Continue For
                End If
                elt = injectIndex("onlink[?]", j)
                xm.SetPropertyX("/citeinfo/" + elt, val)
                j += 1
            Catch ex As Exception
                Dim a = 1
            End Try
        Next
        xm.copyFrom(lworkcit_____help.Tag, "/citeinfo", "/citeinfo/lworkcit/citeinfo")
    End Sub


    Private Sub btnEdit_lworkcit_Click(sender As System.Object, e As System.EventArgs) Handles btnEdit_lworkcit.Click
        Dim frm As New CitationForm(lworkcit_____help.Tag, title.Text)
        frm.ShowDialog()
    End Sub

    Private Sub btnCloseSave_Click(sender As System.Object, e As System.EventArgs) Handles btnCloseSave.Click
        save_citeinfo()
        Me.Close()
    End Sub

    Private Sub btnCloseDiscard_Click(sender As System.Object, e As System.EventArgs) Handles btnCloseDiscard.Click
        If MessageBox.Show("Do you really want to cancel?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            'saved = False
            Me.Close()
        End If
    End Sub


    Function getControlNamed(ctrlName As String) As Control
        Try
            Return getControlNamedInner(Me, ctrlName)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    Private Function getControlNamedInner(ctrl As Control, ctrlName As String) As Control
        If ctrl.Name = ctrlName Then
            Return ctrl
        Else
            For Each ctrl2 As Control In ctrl.Controls
                getControlNamedInner = getControlNamedInner(ctrl2, ctrlName)
                If getControlNamedInner IsNot Nothing Then Return getControlNamedInner
            Next
        End If
        Return Nothing
    End Function


    Private Sub pubdate_____today_Click(sender As System.Object, e As System.EventArgs) Handles pubdate_____today.Click
        pubdate.Text = Format(Now(), "yyyyMMdd")
    End Sub

    Private Sub pubtime_____now_Click(sender As System.Object, e As System.EventArgs) Handles pubtime_____now.Click
        pubtime.Text = Format(Now(), "hhmm")
    End Sub


    Private Sub InitEventHandlers()

        For Each elt As String In elementsHelp
            Dim ctrl As Control
            Try
                ctrl = getControlNamed(Utils.stripNonAlphanumeric(elt, "_") + idSep + "help")
                AddHandler ctrl.Click, AddressOf HelpSeeker
            Catch ex As Exception
                Dim a = 1
            End Try
        Next

        For Each elt As String In elementsRepeated
            Dim ctrl As Control
            Try
                ctrl = getControlNamed(Utils.stripNonAlphanumeric(elt, "_") + idSep + "check")
                AddHandler ctrl.Click, AddressOf check_onlink_Click
            Catch ex As Exception
                Dim a = 1
            End Try
        Next
    End Sub

    ''' <summary>
    ''' Check the online linkage URL by attempting to open it using a browser window. 
    ''' </summary>
    ''' <param name="sender">Event sender. The Button object that raised the event.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>Can only check URL types that are registered with the user's browser.</remarks>
    Sub check_onlink_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim senderName As String = DirectCast(sender, Button).Name
        Dim ctrl As Control = getControlNamed(senderName.Substring(0, senderName.Length - (idSep & "check").Length))
        Dim url As String = ctrl.Text.Trim
        If url <> "" Then
            Utils.OpenInIE(url)
        End If
    End Sub


    ''' <summary>
    ''' Open the help window with appropriate help screen.
    ''' </summary>
    ''' <param name="sender">Event sender. A Button object whose name determines the help screen to open</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>
    ''' The Button object sending the event must have the same name as the object for which help will be displayed, postfixed by idsep and the word "help".
    ''' </remarks>
    Private Sub HelpSeeker(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim senderName As String = DirectCast(sender, Control).Name
        Dim elementName As String = senderName.Substring(0, Math.Max(0, senderName.Length - (idSep & "help").Length))
        If elementName.StartsWith("onlink") Then elementName = "onlink"
        Utils.HelpSeeker("t2_srccite_" & elementName & ".html", proc)
    End Sub


    ''' <summary>
    ''' Populate dropdown controls from database table
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateDropdowns()
        For Each elt As String In elementsDropdown
            Dim ctrl As ComboBox
            Try
                ctrl = getControlNamed(Utils.stripNonAlphanumeric(elt, "_"))

                Dim con As OleDb.OleDbConnection = Nothing
                Dim SQLStr As String = "SELECT DISTINCT [" & ctrl.Name & "] as fldVal FROM srccite WHERE " & ctrl.Name & " > ''"
                Dim dr As OleDb.OleDbDataReader = readerForSQL(SQLStr, con)
                Do While dr.Read()
                    ctrl.Items.Add(dr("fldVal").ToString())
                Loop
                dr.Close()
                con.Close()
            Catch ex As Exception
                Dim a = 1
            End Try
        Next

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs)
        MsgBox(origin_____help.TabStop.ToString + vbTab + origin_____help.TabIndex.ToString)
    End Sub
End Class