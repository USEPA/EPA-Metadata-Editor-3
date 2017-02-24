Imports System.IO.File
Imports System.IO.Path
Imports NetSpell.SpellChecker
Imports System.ComponentModel
Imports System.Xml
Imports System.Text.RegularExpressions


''' <summary>
''' Enum representing the important phases of a form's lifetime.
''' Loading: Form has started but not yet finished loading.
''' Loaded: Form is loaded and ready for user interaction.
''' Closing: Form has started closing.
''' </summary>
''' <remarks></remarks>
Public Enum State
    Loading
    Loaded
    Closing
End Enum

''' <summary>
''' Enum representing a form's modification status.
''' Clean: All tracked form controls retain their initial state.
''' Dirty: At least one tracked form control has been modified from its initial state.
''' </summary>
''' <remarks></remarks>
Public Enum Modified
    Clean
    Dirty
End Enum

''' <summary>
''' Enum representing a form's event handling state.
''' Default: Events are handled as usual.
''' Busy: Form is busy handling an event.
''' Noise: Events are being ignored.
''' </summary>
''' <remarks></remarks>
Public Enum HandleEvents
    [Default]
    Busy
    Noise
End Enum

''' <summary>
''' EditorForm implements EME's user interface.
''' </summary>
''' <remarks></remarks>
Public Class EditorForm
    Private _FormState As State
    Private _FormChanged As Modified
    Private _FormEventHandling As HandleEvents
    Public saveHint As Boolean = False


    ''' <summary>
    ''' Getter/setter for form state.
    ''' </summary>
    ''' <value>New state of form.</value>
    ''' <returns>Current state of form.</returns>
    ''' <remarks>See "State" enumeration.</remarks>
    <Browsable(True), Description("Tells the load state of the form")> _
    Public Property FormState() As State
        Get
            Return _FormState
        End Get
        Set(ByVal Value As State)
            _FormState = Value
        End Set
    End Property

    ''' <summary>
    ''' Getter/setter for form change status.
    ''' </summary>
    ''' <value>New change status of form.</value>
    ''' <returns>Current change status of form.</returns>
    ''' <remarks>See "Modified" enumeration.</remarks>
    <Browsable(True), Description("Tells if control contents have been modified")> _
    Public Property FormChanged() As Modified
        Get
            Return _FormChanged
        End Get
        Set(ByVal Value As Modified)
            _FormChanged = Value
        End Set
    End Property

    ''' <summary>
    ''' Getter/setter for form event handling status.
    ''' </summary>
    ''' <value>New event handling status of form.</value>
    ''' <returns>Current event handling status of form.</returns>
    ''' <remarks>See "HandleEvents" enumeration.</remarks>
    <Browsable(True), Description("Tells whether to ignore or process events")> _
    Public Property FormEventHandling() As HandleEvents
        Get
            Return _FormEventHandling
        End Get
        Set(ByVal Value As HandleEvents)
            _FormEventHandling = Value
        End Set
    End Property

    ''' <summary>
    ''' Create and initialize an EditorForm instance.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        'Form starts out as clean
        FormChanged = Modified.Clean
        'Ignore all events until further notice
        FormEventHandling = HandleEvents.Noise
        'Initialize hover tips for controls
        HoverHelpInit()

        Me.Icon = Nothing

        ' Switch to the tabs selected in previous session
        Me.tcEME.SelectedIndex = My.Settings.SelectedTabIndex
        Me.tcKeywords.SelectedIndex = My.Settings.SelectedTabIndexKeywords
        Me.tcEntityAttr.SelectedIndex = My.Settings.SelectedTabIndexEntityAttr

        ' Set up menu item for EPA stylesheets depending on existence of ArcGIS Desktop and stylesheet install status
        setupEpaStylesheetsMenuItem()

        disableLinkLabelTabStops(Me)
    End Sub

    '''' <summary>
    '''' Used to disable close(x) button of the form
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks>Removed by user demand</remarks>
    'Protected Overrides ReadOnly Property CreateParams() As CreateParams
    '    Get
    '        Dim CS_NOCLOSE As Integer = Int32.Parse("200", Globalization.NumberStyles.HexNumber)
    '        Dim cp As CreateParams = MyBase.CreateParams
    '        cp.ClassStyle = CS_NOCLOSE
    '        Return cp
    '    End Get
    'End Property


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' Load event handler form EditorForm.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EditorForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Flag that the form is in the process of loading
        FormState = State.Loading
        'Flag that any events fired as a result of our changes should not be handled
        FormEventHandling = HandleEvents.Noise
        'Initialize setting controls
        Me.SettingsLoader()
        'Initialize PageControllers with information read from database
        PageController.readFromDb()
        'Allow PageControllers populate the metadata related controls on the EditorForm.
        PageController.ElementPopulator(Me)
        Me.initUserKeywordsTab()
        ' Initialize handlers for eainfo
        Me.eainfoInit()
        'Initialize the spelling checker component
        initSpellChecker()
        ' Initialize evemt handllers to process various EditorForm controls
        Me.InitEventHandlers()
        'Flag that events fired from this point on should be handled
        FormEventHandling = HandleEvents.Default
        If DummyForm IsNot Nothing Then DummyForm.Visible = False
        Me.Text = GlobalVars.nameStr
        updateWindowTitle()
        setupSpdoinfo(False)
        'setupForDonnie()
    End Sub

    Public Sub updateWindowTitle()
        Dim newTitle As String = GlobalVars.nameStr
        If MdUtils.currentlyEditing IsNot Nothing AndAlso Not MdUtils.currentlyEditing.StartsWith(IO.Path.GetTempPath) Then
            newTitle += " - " + MdUtils.currentlyEditing
        End If
        Me.Text = newTitle
    End Sub


    ''' <summary>
    ''' Initialize spelling checker component
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub initSpellChecker()
        ' Following line is probably ineffectual.
        Me.EMEWordDictionary.DictionaryFolder = Utils.getAppFolder()
        'Dictionary used by NetSpell is stored under the system's application data folder
        Dim userDictDstPath As String = Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NetSpell")
        Dim userDictDst As String = Combine(userDictDstPath, "user.dic")
        Dim userDictSrc As String = Combine(Utils.getAppFolder(), "EME_user.dic")
        'If no user dictionary exists...
        If Exists(userDictSrc) And Not Exists(userDictDst) Then
            Try
                'Attempt to create destination folder
                System.IO.Directory.CreateDirectory(userDictDstPath)
            Catch
            End Try
            'and copy default user dictionary
            Copy(userDictSrc, userDictDst, False)
        End If
        'We will need a smarter mechanism in future releases if we want to retain user's existing dictionary but still want to add new spellings...
    End Sub

    ''' <summary>
    ''' Read-only property returning all non-container controls on the form including those nested inside container controls.
    ''' </summary>
    ''' <value></value>
    ''' <returns>Returs a HashTable of controls keyed by the names of the controls</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property allControls() As Hashtable
        Get
            If allControlsColl Is Nothing Then
                allControlsColl = New Hashtable
                allControlsSub(Me)
            End If
            Return allControlsColl
        End Get
    End Property

    ''' <summary>
    ''' Master collection holding all non-container controls on the form.
    ''' </summary>
    ''' <remarks></remarks>
    Private allControlsColl As Hashtable

    ''' <summary>
    ''' Recursively called subroutine adding all controlls in the given container to a master collection.
    ''' </summary>
    ''' <param name="Container">Container whose controls will be added to master collection of controls.</param>
    ''' <remarks></remarks>
    Private Sub allControlsSub(ByVal Container As Object)
        Dim ctrl As Control
        If Container.haschildren Then
            For Each ctrl In Container.controls
                'add this control to the controls collection
                registerControl(ctrl)
                If ctrl.HasChildren Then
                    'This control has children, go visit each of them
                    allControlsSub(ctrl)
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Mark form as modified.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub setDirty(ByVal sender As Object, ByVal e As System.EventArgs)
        setDirtyGeneric()
    End Sub

    ''' <summary>
    ''' Event handler for cell events of a DataGridView control.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We simply set the form dirty.</remarks>
    Private Sub setDirty(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        setDirtyGeneric()
    End Sub

    ''' <summary>
    ''' Event handler for row removal event of a DataGridView control.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We simply set the form dirty.</remarks>
    Private Sub setDirty(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowsRemovedEventArgs)
        setDirtyGeneric()
    End Sub

    ''' <summary>
    ''' Mark the form as dirty.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setDirtyGeneric()
        'If the form is ignoring events then exit
        If FormEventHandling = HandleEvents.Noise Then Exit Sub
        FormChanged = Modified.Dirty
    End Sub

    ''' <summary>
    ''' Register a control by adding it to the master collection
    ''' </summary>
    ''' <param name="ctrl">Control to be registered</param>
    ''' <remarks>Operations that apply to all controls in the form can be registered here.</remarks>
    Public Sub registerControl(ByRef ctrl As Control)
        Dim register As Boolean = True

        If TypeOf ctrl Is TextBox OrElse TypeOf ctrl Is ComboBox Then
            'Register event handler that marks the form as modified when controls value is changed.
            AddHandler ctrl.TextChanged, AddressOf setDirty
            If TypeOf ctrl Is TextBox Then
                AddHandler ctrl.DoubleClick, AddressOf spaciousFormOnDoubleClick
            End If
        ElseIf TypeOf ctrl Is ListBox Then
            AddHandler DirectCast(ctrl, ListBox).SelectedIndexChanged, AddressOf setDirty
        ElseIf TypeOf ctrl Is DataGridView Then
            AddHandler DirectCast(ctrl, DataGridView).CellValueChanged, AddressOf setDirty
            AddHandler DirectCast(ctrl, DataGridView).RowsRemoved, AddressOf setDirty
        Else
            'register = False
            'Debug.Print(ctrl.GetType.ToString)
            If TypeOf ctrl Is PictureBox Then
                'MsgBox(ctrl.Name)
            End If
        End If

        If register Then
            'allControlsColl.Add(ctrl.Name, ctrl)
            allControlsColl(ctrl.Name) = ctrl
        End If
    End Sub

    ''' <summary>
    ''' Open Microsoft Access database holding metadata defaults and supporting settings.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnOpenDatabase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Utils.openMSAccess("")
    End Sub

    ' We no loger have a button on the GUI to open contacts table.
    '''' <summary>
    '''' Open the contacts table in Microsoft Access.
    '''' </summary>
    '''' <param name="sender">Event sender. Not used.</param>
    '''' <param name="e">Event arguments. Not used.</param>
    '''' <remarks></remarks>
    'Private Sub btnOpenDb_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button8.Click
    '    Utils.openMSAccess("openContacts")
    'End Sub

    ''' <summary>
    ''' Spellcheck text controls on a given tab.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub spellcheckTab(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.EMESpelling.SpellCheck(PageController.GetTextControls(Me, Me.tcEME.SelectedIndex + 1))
    End Sub


    Public Shared Sub dgvPostSpellcheckHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ctrl As TextBoxBase = sender
        If TypeOf ctrl.Tag Is DataGridViewTextBoxCell Then
            ctrl.Tag.value = ctrl.Text
        End If
    End Sub

    ''' <summary>
    ''' Set the default values for all controls in the current tab.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnSetDefault_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        PageController.DefaultSetter(Me, Me.tcEME.SelectedIndex + 1)
        'There is surely a better pattern to force update of CSI, but this works for now.
        If Me.tcEME.SelectedTab Is Me.TabPage2 Then
            updateCSI(Me, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Validate metadata record as represented by current state of the form.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnValidate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        PageController.ValidateAction(Me)
    End Sub

    ''' <summary>
    ''' Initialize mouse hovering tips using text stored in the database.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub HoverHelpInit()
        'Read hover help texts from database
        Dim dr As OleDb.OleDbDataReader = Utils.readerForSQL("SELECT * FROM EME_GUI WHERE HoverNote>''")

        ' Call Read method until the end of the cursor is reached.
        Do While dr.Read()
            'Debug.Print(dr("GUIElement").ToString().ToLower())
            For Each ctrl As Control In Me.allControls.Values()
                'Debug.Print(ctrl.Name)
                'Try to find the right control to attach hover text based on the text of bothe the control and its parent.
                If SpecMatchesControl(ctrl, dr("GUIElement"), dr("GUIElementParent")) Then
                    HoverToolTip.SetToolTip(ctrl, dr("HoverNote"))
                Else
                    'Debug.Print(vbTab & ctrl.Text.ToLower())
                End If
            Next
        Loop
        ' Close the reader
        dr.Close()
    End Sub

    ''' <summary>
    ''' Determine if the name specifications match the given control
    ''' </summary>
    ''' <param name="ctrl"></param>
    ''' <param name="ctrlSpec"></param>
    ''' <param name="ctrlParentSpec"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function SpecMatchesControl(ByRef ctrl As Control, ByVal ctrlSpec As String, ByVal ctrlParentSpec As String) As Boolean
        If (TypeOf ctrl Is Label Or TypeOf ctrl Is LinkLabel Or TypeOf ctrl Is GroupBox) _
        And (ctrl.Text.ToLower() = ctrlSpec.ToString().Trim().ToLower() & ":" Or ctrl.Text.ToLower() = ctrlSpec.ToString().Trim().ToLower()) Then
            Dim cParent As Control = ctrl.Parent
            While cParent IsNot Nothing
                If cParent.Text.Trim().ToLower() = ctrlParentSpec.ToString().Trim().ToLower() Then
                    Return True
                Else
                    cParent = cParent.Parent
                End If
            End While
        End If
        Return False
    End Function

    ''' <summary>
    ''' Update metadata future review date field to be four years from date provided for metadata review date.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnMetaFRDate4yrs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMetaFRDate4yrs.Click
        Try
            Dim part2 As String = Me.metainfo_metd.Text.Trim()
            part2 = part2.Substring(4, part2.Length - 4)
            metainfo_metfrd.Text = (Integer.Parse(Me.metainfo_metd.Text.Trim().Substring(0, 4)) + 4).ToString() & part2
        Catch ex As Exception
            MessageBox.Show("Unable to compute 4 years to the future of '" & metainfo_metd.Text & "'")
        End Try
    End Sub

    ''' <summary>
    ''' Perform form closing tasks. If any obstacles, cancel form closing event.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments.</param>
    ''' <remarks></remarks>
    Private Sub EditorForm_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            If FormChanged = Modified.Dirty OrElse GlobalVars.recoveredSession OrElse GlobalVars.savedSession Then
                If Me.saveHint Then
                    If MessageBox.Show("Do you really want to save and close?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) = DialogResult.Yes Then
                        If FormChanged = Modified.Dirty Then
                            PageController.PageSaver(Me)
                        End If
                    Else
                        e.Cancel = True
                    End If
                Else
                    If GlobalVars.savedSession Then
                        Me.saveHint = True
                    Else
                        Dim msg As String = ""
                        If GlobalVars.recoveredSession Then
                            msg = "You have recovered a previously saved session. Do you really want to discard it?"
                        ElseIf FormChanged = Modified.Dirty Then
                            msg = "You have made changes. Do you really want to discard them?"
                        End If
                        If MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) = DialogResult.Yes Then
                            '' Forget about all changes...
                            'FormChanged = Modified.Clean
                            'GlobalVars.recoveredSession = False
                        Else
                            e.Cancel = True
                        End If
                    End If
                End If
            End If
        Catch
        Finally
            If e.Cancel Then
                Me.saveHint = False
            Else
                ' Also remove saved file from this session
                If GlobalVars.savedSession OrElse GlobalVars.recoveredSession Then
                    Utils.deleteSavedSession()
                End If
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Attempt to close form by saving any changes. Triggered by user action.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnCloseSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCloseSave.Click
        Me.saveHint = True
        Me.Close()
        btnSave_Click(sender, e)
    End Sub

    ''' <summary>
    ''' Attempt to close form by discarding any changes. Triggered by user action.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnCloseDiscard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCloseDiscard.Click
        Me.saveHint = False
        Me.Close()
    End Sub

    ''' <summary>
    ''' Set a field to its default value.
    ''' </summary>
    ''' <param name="sender">Event sender. A Button object whose name determines the field to manipulate</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>
    ''' The Button object sending the event must have the same name as the object that will be manipulated, postfixed by idsep and the word "default".
    ''' E.g.: Field with name "idinfo_citation_citeinfo_origin" can be set to default only by a button with name "idinfo_citation_citeinfo_origin_____default" where idsep equals "_____".
    ''' </remarks>
    Private Sub default_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles idinfo_citation_citeinfo_origin_____default.Click, dataqual_complete_____default.Click, idinfo_spdom_bounding_____default.Click, metainfo_metc_____default.Click, distinfo_distrib_____default.Click, idinfo_ptcontac_____default.Click, idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey_____default.Click, idinfo_keywords_theme_themekt__EPA_GIS_Keyword_Thesaurus___themekey_____default.Click, idinfo_keywords_place_placekt__None___placekey_____default.Click, distinfo_distliab_____default.Click, idinfo_useconst_____default.Click, idinfo_secinfo_____default.Click, idinfo_citation_citeinfo_title_____default.Click, idinfo_accconst_____default.Click, distinfo_resdesc_____default.Click, dataqual_posacc_horizpa_horizpar_____default.Click, dataqual_logic_____default.Click, idinfo_keywords_theme_themekt__User___themekey_____default.Click, idinfo_citation_citeinfo_onlink_1______default.Click, idinfo_citation_citeinfo_onlink_2______default.Click, idinfo_citation_citeinfo_onlink_3______default.Click, idinfo_citation_citeinfo_onlink_4______default.Click, idinfo_citation_citeinfo_onlink_5______default.Click, idinfo_citation_citeinfo_onlink_6______default.Click, idinfo_citation_citeinfo_onlink_7______default.Click, idinfo_citation_citeinfo_onlink_8______default.Click, idinfo_citation_citeinfo_onlink_9______default.Click, idinfo_citation_citeinfo_onlink_10______default.Click
        Dim senderName As String = DirectCast(sender, Button).Name
        Dim pc As PageController = PageController.thatControls(senderName.Substring(0, senderName.Length - (idSep & "default").Length))
        pc.setDefault(Me)
    End Sub

    ''' <summary>
    ''' Set all controls in the same group as the default button itself to their default values.
    ''' </summary>
    ''' <param name="sender">Event sender. A Button object which will force all objects in its container to be set to their default values as applicable.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub group_default_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click, Button99.Click, eainfo_overview_eadetcit_____default.Click, Button1.Click, spdo_____default.Click
        'Button that was clicked
        Dim btn As Button = DirectCast(sender, Button)
        'Controls that we want to set to default value
        Dim ctrls As IList = Nothing

        'Try to locate controls, first among siblings...
        If TypeOf btn.Parent Is Panel Then
            ctrls = btn.Parent.Controls
            'then among children of siblings...
        ElseIf TypeOf btn.Parent Is GroupBox Then
            For Each ctrl As Control In btn.Parent.Controls
                If TypeOf ctrl Is Panel Then
                    ctrls = ctrl.Controls
                    Exit For
                End If
            Next
        End If

        'Exit if we can't locate the controls
        If ctrls Is Nothing Then
            Exit Sub
        End If

        Dim pc As PageController
        'Set each control to its default if it's a control assocated with a PageController
        For Each ctrl As Control In ctrls
            pc = PageController.thatControls(ctrl.Name)
            If pc IsNot Nothing Then
                pc.setDefault(Me)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Set coordinate system info controls to default value.
    ''' </summary>
    ''' <param name="sender">Event sender.</param>
    ''' <param name="e">Event arguments.</param>
    ''' <remarks></remarks>
    Private Sub CSI_default_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles spref_horizsys_____default.Click
        ' Remove event handlers to avoid chicken/egg problem.
        RemoveHandler Me.spref_horizsys_CoordinateSystem.SelectionChangeCommitted, AddressOf updateCSI
        RemoveHandler Me.spref_horizsys_Zone.SelectionChangeCommitted, AddressOf updateCSI
        RemoveHandler Me.spref_horizsys_Units.SelectionChangeCommitted, AddressOf updateCSI
        RemoveHandler Me.spref_horizsys_Datum.SelectionChangeCommitted, AddressOf updateCSI

        ' Clear filter
        DirectCast(Me.spref_horizsys.DataSource, DataView).RowFilter = ""
        ' Execute default operation.
        default_Click(sender, e)
        ' Note existing selections.
        Dim cs As String = Me.spref_horizsys.SelectedItem("CoordinateSystem")
        Dim zone As String = Me.spref_horizsys.SelectedItem("Zone").ToString
        Dim unit As String = IIf(Me.spref_horizsys.SelectedItem("geogunit") IsNot DBNull.Value, Me.spref_horizsys.SelectedItem("geogunit"), Me.spref_horizsys.SelectedItem("plandu"))
        Dim datum As String = Me.spref_horizsys.SelectedItem("horizdn")
        ' Attempt to set displayed selections.
        Me.spref_horizsys_CoordinateSystem.SelectedValue = cs
        updateCSI(sender, e)
        Me.spref_horizsys_Zone.SelectedValue = zone
        Me.spref_horizsys_Units.SelectedValue = unit
        Me.spref_horizsys_Datum.SelectedValue = datum
        ' Re-engage event handlers.
        AddHandler Me.spref_horizsys_CoordinateSystem.SelectionChangeCommitted, AddressOf updateCSI
        AddHandler Me.spref_horizsys_Zone.SelectionChangeCommitted, AddressOf updateCSI
        AddHandler Me.spref_horizsys_Units.SelectionChangeCommitted, AddressOf updateCSI
        AddHandler Me.spref_horizsys_Datum.SelectionChangeCommitted, AddressOf updateCSI
    End Sub

    ''' <summary>
    ''' Given the tag name, returns the Control object that controls the tag.
    ''' </summary>
    ''' <param name="tagName">Tag name. This is actually a uniquely identifying XSL pattern for an FGDC element where each non-alphanumeric character is replaced with an underscore character.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' If a control for the given tag does not already exist, a textbox control is automatically created but not added to form. 
    ''' This allows FGDC elements that are not directly represented on the user interface to be populated, manipulated and saved behind the scenes.
    ''' </remarks>
    Function getControlForTag(ByVal tagName As String) As Control
        ' Check to see if a control associated with tagName already exists
        If allControls.ContainsKey(tagName) Then
            Return allControls(tagName)
        End If

        ' Otherwise, create a new control and return it. 
        Dim ctrl As TextBox = New TextBox()
        ctrl.Visible = False
        ctrl.Name = tagName
        ctrl.Parent = Me
        registerControl(ctrl)
        'Debug.Print("Created: " & tagName)
        Return ctrl
    End Function

    ''' <summary>
    ''' Allow user to see metadata XML in a browser window. Triggered by user action.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnViewMetadataXML_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        PageController.PageSaver(Me)

        Dim FileName As String = IO.Path.GetTempFileName() & ".xml"

        Dim File_Stream As New IO.FileStream(FileName, IO.FileMode.Create, IO.FileAccess.Write)
        Dim FileWriter As New IO.StreamWriter(File_Stream)
        Dim metaXML As String = ""
        With FileWriter
            Try
                Dim iXPSv As New XmlMetadata
                iXPSv.SetXml(iXPS.GetXml(""))
                ' Delete keyword themes that don't have keywords
                iXPSv.checkDeleteKTTags()
                If pbCSISelection.Visible Then iXPSv.DeleteProperty("spref/horizsys")

                metaXML = iXPSv.GetXml("")
                .WriteLine(metaXML)
                '.WriteLine(Validator.validate_mp(metaXML))
            Catch ex As IO.IOException
                Utils.ErrorHandler(ex)
            Finally
                .Close()
            End Try
        End With

        Utils.OpenInIE(FileName)
    End Sub

    ''' <summary>
    ''' Load the initial values of user settings on Tab 3 using saved application settings.
    ''' </summary>
    ''' <remarks></remarks>
    Sub SettingsLoader()
        Me.tbValidationTimeout.Text = My.Settings.ValidationTimeout
        Me.tbValidationTimeout.Enabled = Me.ViaWebserviceToolStripMenuItem.Checked
        Me.ViaWebserviceToolStripMenuItem.Checked = My.Settings.ValidationEnabled
        Me.LocalToolStripMenuItem.Checked = Not My.Settings.ValidationEnabled
    End Sub

    ''' <summary>
    ''' Check to make sure the setting whose name is provided is valid .
    ''' </summary>
    ''' <param name="settingName">The name of the setting to check for. The VB.NET project must have a setting with this name.</param>
    ''' <returns>Returns true if setting is valid, false otherwise.</returns>
    ''' <remarks>Currently we only check for cluster update behavior setting.</remarks>
    Public Function SettingsChecker(ByVal settingName As String) As Boolean
        Return True
    End Function

    ''' <summary>
    ''' Check the online linkage URL by attempting to open it using a browser window. 
    ''' </summary>
    ''' <param name="sender">Event sender. The Button object that raised the event.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>Can only check URL types that are registered with the user's browser.</remarks>
    Private Sub check_onlink_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim senderName As String = DirectCast(sender, Button).Name
        Dim ctrl As Control = Me.allControls(senderName.Substring(0, senderName.Length - (idSep & "check").Length))
        Dim url As String = ctrl.Text.Trim
        If url <> "" Then
            'Dim myMatch As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(url, "^[a-zA-Z]+://")
            'If Not myMatch.Success Then
            '    url = "file://" & url
            'End If

            Utils.OpenInIE(url)
        End If
    End Sub

    ''' <summary>
    ''' Set the corresponding date field value to today's date.
    ''' </summary>
    ''' <param name="sender">Event sender. A Button object whose name determines the field to manipulate</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>
    ''' The Button object sending the event must have the same name as the object that will be manipulated, postfixed by idsep and the word "today".
    ''' E.g.: Field with name "dataqual_lineage_procstep_1__procdate" can be set to today's date only by a button with name "dataqual_lineage_procstep_1__procdate_____today" where idsep equals "_____".
    ''' </remarks>
    Private Sub today_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles metainfo_metd_____today.Click, idinfo_citation_citeinfo_pubdate_____today.Click, timeinfo_____today.Click
        Dim senderName As String = DirectCast(sender, Button).Name
        Dim ctrl As TextBox = Me.allControls(senderName.Substring(0, senderName.Length - (idSep & "today").Length))
        ctrl.Text = Format(Now(), "yyyyMMdd")
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
        HelpSeeker(senderName.Substring(0, Math.Max(0, senderName.Length - (idSep & "help").Length)))
    End Sub

    ''' <summary>
    ''' Handle custom help events associated with "help2" controls (as opposed to "help").
    ''' </summary>
    ''' <param name="sender">Event sender. Typically a LinkLabel object whose name determines the help screen to open</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Public Sub HelpSeekerCustom(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles enttyp_____help2.Click, detailed_____help2.Click, attr_____help2.Click, attrdomv_____help2.Click, timeinfo_____help2.Click, procstep_____help2.Click, srcinfo_____help2.Click
        Dim senderName As String = DirectCast(sender, Control).Name
        Dim helpPage As String = senderName.Substring(0, Math.Max(0, senderName.Length - (idSep & "help2").Length))
        helpPage = "t" & CStr(Me.tcEME.SelectedIndex + 1) & "_" & helpPage & ".html"
        HelpSeeker("dummy", helpPage)
    End Sub

    Public Shared Sub HelpSeeker(ByVal name As String, Optional ByVal defaultHelp As String = "/Help_Main.html")
        Utils.HelpSeeker(PageController.getHelpPageFor(name, defaultHelp), GlobalVars.proc)
    End Sub


    Public Shared Sub HelpSeekerLineage(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dataqual_lineage_____help3.Click, lineage_____help3.Click
        HelpSeeker("dummy", "t2_lineage.html")
    End Sub



    Private Sub InitEventHandlers()
        For Each ctrl As Control In Me.allControls.Values()
            If ctrl.Name.EndsWith(idSep & "help") Then
                If TypeOf ctrl Is LinkLabel Then
                    AddHandler ctrl.Click, AddressOf HelpSeeker
                End If
            ElseIf ctrl.Name.EndsWith(idSep & "check") Then
                If TypeOf ctrl Is Button Then
                    AddHandler ctrl.Click, AddressOf check_onlink_Click
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' Update coordinate system information ComboBoxes when any one of them is changed so that the correct options are displayed in the rest of them.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>Arguably, the most convoluted piece in EME. Prime for refactoring.</remarks>
    Private Sub updateCSI(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles spref_horizsys_Zone.SelectionChangeCommitted, spref_horizsys_Units.SelectionChangeCommitted, spref_horizsys_Datum.SelectionChangeCommitted, spref_horizsys_CoordinateSystem.SelectionChangeCommitted
        Debug.Print(Now)
        ' Return without action if the CSI ComboBoxes don't have their DataSources initialized.
        If Me.spref_horizsys_CoordinateSystem.DataSource Is Nothing _
            OrElse Me.spref_horizsys_Zone.DataSource Is Nothing _
            OrElse Me.spref_horizsys_Datum.DataSource Is Nothing _
            OrElse Me.spref_horizsys_Units.DataSource Is Nothing Then
            Return
        End If
        Dim ctrl As ComboBox = Me.spref_horizsys
        Dim dv As DataView = DirectCast(ctrl.DataSource, DataView)
        Dim filter As String = "1=1"
        Dim coordSys As String = ""
        Dim datum As String = ""
        Dim zone As String = ""
        Dim zoneId As String = ""
        Dim unit As String = ""

        If Me.spref_horizsys_CoordinateSystem.SelectedItem IsNot Nothing Then
            coordSys = Me.spref_horizsys_CoordinateSystem.SelectedItem("clusterInfo")
        End If

        If Me.spref_horizsys_Datum.SelectedItem IsNot Nothing Then
            datum = Me.spref_horizsys_Datum.SelectedItem("clusterInfo").ToString()
        End If

        If Me.spref_horizsys_Zone.SelectedItem IsNot Nothing Then
            zone = Me.spref_horizsys_Zone.SelectedItem("clusterInfo")
            zoneId = Me.spref_horizsys_Zone.SelectedItem("zone")
        End If

        If Me.spref_horizsys_Units.SelectedItem IsNot Nothing Then
            unit = Me.spref_horizsys_Units.SelectedItem("clusterInfo")
        End If


        If coordSys > "" Then
            filter &= " AND CoordinateSystem='" & coordSys & "'"
            Me.spref_horizsys_Zone.DataSource.rowfilter = "CoordinateSystem='" & coordSys & "'"
            Me.spref_horizsys_Zone.SelectedValue = zone
        End If

        Me.spref_horizsys_Datum.DataSource.rowfilter = ""

        If coordSys = "Geographic" Then
            Me.spref_horizsys_Units.DataSource.rowfilter = "appliesToGeographic"
        Else
            If coordSys = "Universal Transverse Mercator" Then
                Me.spref_horizsys_Units.DataSource.rowfilter = "appliesToUTM"
                Me.spref_horizsys_Datum.DataSource.rowfilter = "horizdn='North American Datum of 1983'"
                filter &= " AND horizdn='North American Datum of 1983'"
            ElseIf coordSys = "State Plane Coordinate System" Then
                Me.spref_horizsys_Units.DataSource.rowfilter = "appliesToSPCS"
            ElseIf coordSys = "Map Projection" Then
                Me.spref_horizsys_Units.DataSource.rowfilter = "appliesToMapProjection"
                If zone.Contains("Web Mercator") Then
                    Me.spref_horizsys_Units.DataSource.rowfilter = "unit='meters'"
                    unit = "meters"
                    Dim otherprj As String = iXPS.SimpleGetProperty("spref/horizsys/planar/mapproj/mapprojp/otherprj").Trim
                    'If otherprj <> "" Then
                    '    filter &= String.Format(" AND otherprj='{0}'", iXPS.SimpleGetProperty("spref/horizsys/planar/mapproj/mapprojp/otherprj"))
                    'End If
                ElseIf zone.Contains("Albers") Then
                    Me.spref_horizsys_Units.DataSource.rowfilter = "unit NOT LIKE 'decimal*'"
                    If unit = "decimal degrees" Then unit = ""
                End If
            End If
        End If

        If coordSys > "" AndAlso zone > "" Then
            filter &= " AND (spcszone='" & zoneId & "' or utmzone='" & zoneId & "' or [Zone]='" & zoneId & "')"
            If zone.Contains("Web Mercator") Then
                filter &= " AND clusterInfo = '" & zone & "'"
            End If
        End If

        If unit > "" Then
            filter &= " AND (plandu='" & unit & "' or geogunit='" & unit & "')"
        End If

        'RemoveHandler Me.spref_horizsys_Units.SelectionChangeCommitted, AddressOf updateCSI
        'RemoveHandler Me.spref_horizsys_Datum.SelectionChangeCommitted, AddressOf updateCSI

        Me.spref_horizsys_Datum.DataSource = Utils.SelectDistinct(Me.spref_horizsys_Datum.DataSource.table.tablename.ToString, dv, "horizdn", filter)

        If datum > "" AndAlso Me.spref_horizsys_Datum.DataSource.count > 0 Then
            Me.spref_horizsys_Datum.SelectedValue = datum
        Else
            Me.spref_horizsys_Datum.SelectedValue = ""
        End If

        If datum > "" Then
            filter &= " AND horizdn='" & datum & "'"
        End If

        If unit > "" AndAlso Me.spref_horizsys_Units.DataSource.count > 0 Then
            Me.spref_horizsys_Units.SelectedValue = unit
        Else
            Me.spref_horizsys_Units.SelectedValue = ""
        End If

        Dim isGeographic As Boolean = (coordSys = "Geographic")
        Me.spref_horizsys_Zone.Enabled = Not isGeographic
        Me.spref_horizsys_Zone_____help.Enabled = Not isGeographic
        Me.spref_horizsys_Zone_____warning.Enabled = Not isGeographic
        Me.lblZone.Enabled = Not isGeographic
        Me.spref_horizsys_planar_planci_coordrep_absres.Visible = Not isGeographic
        Me.spref_horizsys_planar_planci_coordrep_ordres.Visible = Not isGeographic
        Me.spref_horizsys_planar_planci_coordrep_absres_____help.Visible = Not isGeographic
        Me.spref_horizsys_planar_planci_coordrep_ordres_____help.Visible = Not isGeographic
        Me.btnCopy_absres2ordres.Visible = Not isGeographic

        Me.spref_horizsys_geograph_longres.Visible = isGeographic
        Me.spref_horizsys_geograph_latres.Visible = isGeographic
        Me.spref_horizsys_geograph_longres_____help.Visible = isGeographic
        Me.spref_horizsys_geograph_latres_____help.Visible = isGeographic
        Me.btnCopy_latres2longres.Visible = isGeographic

        If sender.Name = "spref_horizsys_CoordinateSystem" Then
            Me.spref_horizsys_planar_planci_coordrep_absres_____warning.Visible = Not isGeographic AndAlso (Me.HoverToolTip.GetToolTip(Me.spref_horizsys_planar_planci_coordrep_absres_____warning).Trim <> "")
            Me.spref_horizsys_planar_planci_coordrep_ordres_____warning.Visible = Not isGeographic AndAlso (Me.HoverToolTip.GetToolTip(Me.spref_horizsys_planar_planci_coordrep_ordres_____warning).Trim <> "")

            Me.spref_horizsys_geograph_longres_____warning.Visible = isGeographic AndAlso (Me.HoverToolTip.GetToolTip(Me.spref_horizsys_geograph_longres_____warning).Trim <> "")
            Me.spref_horizsys_geograph_latres_____warning.Visible = isGeographic AndAlso (Me.HoverToolTip.GetToolTip(Me.spref_horizsys_geograph_latres_____warning).Trim <> "")
        End If

        Me.spref_horizsys_geograph_longres.Location = spref_horizsys_planar_planci_coordrep_absres.Location
        Me.spref_horizsys_geograph_longres_____help.Location = spref_horizsys_planar_planci_coordrep_absres_____help.Location
        Me.spref_horizsys_geograph_longres_____warning.Location = spref_horizsys_planar_planci_coordrep_absres_____warning.Location
        Me.spref_horizsys_geograph_latres.Location = spref_horizsys_planar_planci_coordrep_ordres.Location
        Me.spref_horizsys_geograph_latres_____help.Location = spref_horizsys_planar_planci_coordrep_ordres_____help.Location
        Me.spref_horizsys_geograph_latres_____warning.Location = spref_horizsys_planar_planci_coordrep_ordres_____warning.Location
        Me.btnCopy_latres2longres.Location = btnCopy_absres2ordres.Location

        If isGeographic Then
            Me.spref_horizsys_planar_planci_coordrep_absres.Text = ""
            Me.spref_horizsys_planar_planci_coordrep_ordres.Text = ""
        Else
            Me.spref_horizsys_geograph_longres.Text = ""
            Me.spref_horizsys_geograph_latres.Text = ""
        End If

        If coordSys = "Map Projection" Then
            Me.spref_horizsys_Zone_____help.Text = "Projection Name:"
        ElseIf coordSys = "Geographic" Then
            Me.spref_horizsys_Zone_____help.Text = "Not applicable:"
        ElseIf coordSys = "Universal Transverse Mercator" Then
            Me.spref_horizsys_Zone_____help.Text = "UTM Zone:"
        ElseIf coordSys = "State Plane Coordinate System" Then
            Me.spref_horizsys_Zone_____help.Text = "SPCS Zone:"
        End If

        'AddHandler Me.spref_horizsys_Units.SelectionChangeCommitted, AddressOf updateCSI
        'AddHandler Me.spref_horizsys_Datum.SelectionChangeCommitted, AddressOf updateCSI

        dv.RowFilter = ""
        Dim fullCount As Integer = dv.Count
        dv.RowFilter = filter
        pbCSISelection.Visible = (dv.Count <> 1 AndAlso dv.Count < fullCount)
        If dv.Count > 0 Then
            If dv.Count = 1 Then ctrl.SelectedValue = dv(0)("clusterInfo")
            If dv.Count = 1 Then
                PageController.thatControls(ctrl.Name).propagateClusterSelectionChanged(ctrl, Nothing)
            ElseIf TypeOf sender Is EditorForm AndAlso Utils.nv(iXPS.GetXml("spref"), "").ToString.Trim > "" Then
                'ElseIf TypeOf sender Is EditorForm AndAlso (coordSys > "" OrElse zone > "" OrElse datum > "" OrElse unit > "") Then
                disableCSI()
            End If
        End If
        'Debug.Print(dv.RowFilter)
        'Debug.Print(dv.Count)

        For Each cbctrl As ComboBox In New List(Of ComboBox)() From {Me.spref_horizsys_Units, Me.spref_horizsys_Datum}
            If sender Is cbctrl OrElse e Is Nothing Then Continue For
            'if there is only one datum selection available, then auto-select it.
            If cbctrl.DataSource.count = 1 Then
                RemoveHandler cbctrl.SelectionChangeCommitted, AddressOf updateCSI
                cbctrl.SelectedIndex = 0
                updateCSI(cbctrl, Nothing)
                AddHandler cbctrl.SelectionChangeCommitted, AddressOf updateCSI
            End If
        Next

    End Sub

    ''' <summary>
    ''' Disable controls associated with coordinate system info (spref).
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub disableCSI()
        Panel14.Enabled = False
        pbCSISelection.Visible = False
        'Me.spref_horizsys.Enabled = False
        'Me.spref_horizsys_CoordinateSystem.Enabled = False
        'Me.spref_horizsys_Datum.Enabled = False
        'Me.spref_horizsys_Zone.Enabled = False
        'Me.spref_horizsys_Units.Enabled = False
        'Me.spref_horizsys_____default.Enabled = False
        PageController.disableCluster("spref/horizsys")
    End Sub

    ''' <summary>
    ''' Refresh those controls (on current tab) that get their value domain from the database.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnRefreshFromDB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            ProgressDialog.ShowMessage("Refreshing from database...")
            Dim pc As PageController
            For Each ctrl As Control In Me.allControls.Values
                pc = PageController.thatControls(ctrl.Name)
                Debug.Print(ctrl.Name)
                Application.DoEvents()
                If pc IsNot Nothing AndAlso pc.isOnTab(Me.tcEME.SelectedIndex + 1) Then
                    If TypeOf ctrl Is ComboBox Then
                        pc.comboBoxLoader(ctrl, True)
                    ElseIf TypeOf ctrl Is ListBox Then
                        pc.listBoxLoader(ctrl, True)
                    Else
                        pc.checkForAndLoadDataGridViewComboBoxColumn(Me)
                    End If
                End If
            Next

            Me.initUserKeywordsTab()
        Catch ex As Exception
            Dim a = 1
        Finally
            ProgressDialog.CancelMessage()
        End Try
    End Sub

    ''' <summary>
    ''' Adjust dropdown menu width for ComboBox objects dynamically so that maximum amount of information is displayed without going off-screen.
    ''' </summary>
    ''' <param name="sender">Event sender. The ComboBox object whose dropdown width will be adjusted.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub AdjustWidth_ComboBox_DropDown(ByVal sender As Object, ByVal e As System.EventArgs) Handles idinfo_accconst.DropDown, spref_horizsys_Zone.DropDown, spref_horizsys_Units.DropDown, spref_horizsys_Datum.DropDown, spref_horizsys_CoordinateSystem.DropDown, metainfo_metc.DropDown, idinfo_useconst.DropDown, idinfo_timeperd_current.DropDown, idinfo_status_update.DropDown, idinfo_status_progress.DropDown, idinfo_spdom_bounding.DropDown, idinfo_secinfo.DropDown, idinfo_ptcontac.DropDown, idinfo_citation_citeinfo_pubinfo_pubplace.DropDown, idinfo_citation_citeinfo_pubinfo_publish.DropDown, distinfo_resdesc.DropDown, distinfo_distrib.DropDown, dataqual_complete.DropDown, dataqual_posacc_horizpa_horizpar.DropDown
        Dim senderComboBox As ComboBox = DirectCast(sender, ComboBox)
        'Dim senderComboBox As DataGridViewComboBoxEditingControl = DirectCast(sender, DataGridViewComboBoxEditingControl)
        'If TypeOf sender Is ComboBox Then
        'Else
        '    Dim senderComboBox As ComboBox = DirectCast(sender, ComboBox)
        'End If

        'senderComboBox.DropDownStyl.DropDownWidth = 555
        'Exit Sub


        Dim cbWidth As Integer = senderComboBox.Width
        Dim cbFont As Font = senderComboBox.Font
        Dim g As Graphics = senderComboBox.CreateGraphics()
        Dim vertScrollBarWidth As Integer = 0
        'Account for scrollbar if exists
        If senderComboBox.Items.Count > senderComboBox.MaxDropDownItems Then
            vertScrollBarWidth = SystemInformation.VerticalScrollBarWidth
        End If

        Dim newWidth As Integer
        Dim s As String
        'Iterate over combobox contents to compute the width required to fit
        For i As Integer = 0 To senderComboBox.Items.Count - 1
            's = DirectCast(senderComboBox.Items.Item(i), DataRowView).Item(senderComboBox.ValueMember).ToString()
            If TypeOf senderComboBox.Items(i) Is XmlFragment Then
                s = senderComboBox.Items(i).displayname
            ElseIf TypeOf senderComboBox.Items(i) Is String Then
                s = senderComboBox.Items(i)
            Else
                s = senderComboBox.Items(i)(senderComboBox.DisplayMember).ToString()
            End If
            newWidth = g.MeasureString(s, cbFont).Width + vertScrollBarWidth
            If cbWidth < newWidth Then
                cbWidth = newWidth
            End If
        Next

        ' Make sure we don't go off screen
        Dim cbLeft As Integer = senderComboBox.PointToScreen(New Point(0, senderComboBox.Left)).X
        cbWidth = Math.Min(Screen.PrimaryScreen.WorkingArea.Width - cbLeft, cbWidth)
        senderComboBox.DropDownWidth = Math.Max(cbWidth - 2, senderComboBox.Width)

        g.Dispose()
    End Sub

    ''' <summary>
    ''' Adjust dropdown menu width for DataGridViewComboBoxEditingControl objects dynamically so that maximum amount of information is displayed without going off-screen.
    ''' </summary>
    ''' <param name="sender">Event sender. The DataGridViewComboBoxEditingControl object whose dropdown width will be adjusted.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>Not functional.</remarks>
    Private Sub AdjustWidth_ComboBox_DropDown2(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim senderComboBox As DataGridViewComboBoxEditingControl = sender

        Dim cbWidth As Integer = senderComboBox.Width
        Dim cbFont As Font = senderComboBox.Font
        Dim g As Graphics = senderComboBox.CreateGraphics()
        Dim vertScrollBarWidth As Integer = 0
        'Account for scrollbar if exists
        If senderComboBox.Items.Count > senderComboBox.MaxDropDownItems Then
            vertScrollBarWidth = SystemInformation.VerticalScrollBarWidth
        End If

        Dim newWidth As Integer
        Dim s As String
        'Iterate over combobox contents to compute the width required to fit
        For i As Integer = 0 To senderComboBox.Items.Count - 1
            's = DirectCast(senderComboBox.Items.Item(i), DataRowView).Item(senderComboBox.ValueMember).ToString()
            If TypeOf senderComboBox.Items(i) Is XmlFragment Then
                s = senderComboBox.Items(i).displayname
            ElseIf TypeOf senderComboBox.Items(i) Is String Then
                s = senderComboBox.Items(i)
            Else
                s = senderComboBox.Items(i)(senderComboBox.DisplayMember).ToString()
            End If
            newWidth = g.MeasureString(s, cbFont).Width + vertScrollBarWidth
            If cbWidth < newWidth Then
                cbWidth = newWidth
            End If
        Next

        ' Make sure we don't go off screen
        Dim cbLeft As Integer = senderComboBox.PointToScreen(New Point(0, senderComboBox.Left)).X
        cbWidth = Math.Min(Screen.PrimaryScreen.WorkingArea.Width - cbLeft, cbWidth)
        senderComboBox.DropDownWidth = Math.Max(cbWidth - 2, senderComboBox.Width)

        g.Dispose()
    End Sub

    ''' <summary>
    ''' Form activated event handler. Performs one-time tasks that need to run after the form is loaded and activated.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub EditorForm_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        If FormState = State.Loading Then
            'Ideally, we need a semaphore lock here
            FormState = State.Loaded
            ' this code will only execute once
            ' perform initialization code here
            updateCSI(Me, Nothing)
            load_timeinfo()
            If tcEME.SelectedIndex = 1 Then updateLineageCounts()

            'If we recovered a session...
            If GlobalVars.recoveredSession Then
                ' then we start out with a dirty form.
                FormChanged = Modified.Dirty
            Else
                ' otherwise, the form is clean.
                FormChanged = Modified.Clean
            End If
        End If
    End Sub


    ''' <summary>
    ''' Ensure that the validation timeout is set to a valid value.
    ''' </summary>
    ''' <param name="sender">Event sender. TextBox object holding the validation timeout value.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub tbValidationTimeout_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbValidationTimeout.TextChanged
        Dim t As Integer
        Integer.TryParse(sender.Text, t)
        If t = 0 Then
            If sender.Text.Trim <> "0" Then
                MessageBox.Show("Invalid value!")
            Else
                MessageBox.Show("Setting to default timeout of 30 seconds!")
            End If
            sender.text = 30
        Else
            My.Settings.ValidationTimeout = t
        End If
    End Sub

    ''' <summary>
    ''' Event handler to keep track of which tab the user is on.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub tcEME_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tcEME.SelectedIndexChanged
        ' Save last selected tab's index for later use
        My.Settings.SelectedTabIndex = Me.tcEME.SelectedIndex
    End Sub

    ''' <summary>
    ''' Event handler that opens a spacious entry form.
    ''' </summary>
    ''' <param name="sender">Event sender. The control that initiated the event.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub spaciousFormOnDoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim ctrl As Control = DirectCast(sender, Control)
        Dim frm As New SpaciousEntryForm
        frm.element = ctrl.Name
        frm.llHelp.Visible = False
        If Me.allControls.ContainsKey(ctrl.Name & idSep & "help") Then
            frm.elementDisplayName = Me.allControls(ctrl.Name & idSep & "help").Text
            frm.llHelp.Visible = True
        ElseIf Me.allControls.ContainsKey(ctrl.Name & idSep & "help2") Then
            frm.elementDisplayName = Me.allControls(ctrl.Name & idSep & "help2").Text
        End If
        frm.content = ctrl.Text
        frm.optionalityColor = ctrl.Parent.BackColor
        frm.ShowDialog()
        ctrl.Text = frm.content
    End Sub

    ''' <summary>
    ''' Event handler to remove ESRI added/specific stuff.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnRemoveESRITags_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ' We will remove tags that have ESRI inserted boilerplate entries:  "REQUIRED: ..." or ESRI-specific tags
        Dim modified_ As Boolean = PageController.wipeEsriRequired(Me) OrElse Utils.wipeEsriTags(iXPS)
        If modified_ Then
            ' Mark form as dirty.
            FormChanged = Modified.Dirty
            'We need to re-init XMLFragment holding eainfo, so that it does not overwrite what we did here.
            Me.eainfoInit()
            MsgBox("ESRI tags have been removed.")
        Else
            MsgBox("No ESRI tags found.")
        End If
    End Sub

    ''' <summary>
    ''' Event handler to open find and replace form.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnFindReplace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim frf As New FindReplaceForm
        frf.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Event handler to perform (temporary) save operation.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Me.SaveToolStripMenuItem_Click(Nothing, Nothing)

        ' AE: This is the old logic. Is there value in preserving last session xml functionality?
        'If Utils.checkForPreviouslySavedSession() Then
        '    If MsgBox("You are about to overwrite a metadata file saved during a previous session." & vbCrLf & "Do you really want to proceed?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
        '        Exit Sub
        '    End If
        'End If
        'If Me.FormChanged = Modified.Dirty Then
        '    ' Save form (this will update iXPS)
        '    PageController.PageSaver(Me)
        'End If

        '' Write out current metadata XML to filesystem.
        'My.Computer.FileSystem.WriteAllText(Utils.getLastSessionXMLPath, iXPS.GetXml(""), False)
        'GlobalVars.savedSession = True
        'Me.FormChanged = Modified.Clean
    End Sub

    ''' <summary>
    ''' Event handler to keep track of the last keyword tab selected by the user.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub tcKeywords_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tcKeywords.SelectedIndexChanged
        My.Settings.SelectedTabIndexKeywords = tcKeywords.SelectedIndex
    End Sub

    ''' <summary>
    ''' Event handler to write the correct metadata element depending on format of the timeinfo value entered.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>This went through a number of changes including the content allowed
    ''' and the interactively updated help link (which was replaced by a static one). 
    ''' Hence the abundance of commented lines.</remarks>
    Private Sub idinfo_timeperd_timeinfo_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ' Don't do anything if we're called when the form is loading
        If Me.FormState = State.Loading Then Return

        Dim txt As String = timeinfo.Text.Trim
        Dim YYYY As String = "[0-9][0-9][0-9][0-9]"
        Dim MM As String = "[0-1][0-9]"
        Dim DD As String = "[0-3][0-9]"
        Dim YYYYMM As String = YYYY & MM
        Dim YYYYMMDD As String = YYYYMM & DD
        Dim re_caldate As String = " *(" & YYYY & "|" & YYYYMM & "|" & YYYYMMDD & ") *"
        Dim re_mdattim As String = re_caldate & "(," & re_caldate & ")+"

        Dim timeinfo2 As String = "idinfo_timeperd_timeinfo"
        Dim sngdate As String = "idinfo_timeperd_timeinfo_sngdate_caldate"
        Dim mdattim As String = "idinfo_timeperd_timeinfo_mdattim_sngdate____caldate"
        Dim rngdates As String = "idinfo_timeperd_timeinfo_rngdates"
        Dim begdate As String = "idinfo_timeperd_timeinfo_rngdates_begdate"
        Dim enddate As String = "idinfo_timeperd_timeinfo_rngdates_enddate"

        'Dim helpMsg As String = ""
        Dim help As String = "/t1_timeinfo.html"

        ' Clear cluster controls
        Me.allControls(sngdate).text = ""
        Me.allControls(begdate).text = ""
        Me.allControls(enddate).text = ""
        'Me.idinfo_timeperd_timeinfo_mdattim_caldate.Items.Clear()
        Me.allControls(mdattim).Items.Clear()

        'If Regex.Match(txt, "^" & re_caldate & "$").Success OrElse txt.ToLower = "unknown" Then
        If Regex.Match(txt, "^" & re_mdattim & "$").Success Then
            'helpMsg = "Multiple dates"
            For Each dt As String In txt.Split(New Char() {","})
                Me.allControls(mdattim).Items.Add(dt.Trim)
                Me.allControls(mdattim).SelectedItems.Add(dt.Trim)
            Next
            help = PageController.thatControls(mdattim).help
        ElseIf Regex.Match(txt, "^" & re_caldate & "-" & re_caldate & "$").Success Then
            'helpMsg = "Date range"
            Dim dates As String() = txt.Split(New Char() {"-"})
            Me.allControls(begdate).text = dates(0).Trim
            Me.allControls(enddate).text = dates(1).Trim
            help = PageController.thatControls(begdate).help
        Else
            'helpMsg = "Invalid pattern"
            'helpMsg = "Single date"
            Me.allControls(sngdate).text = txt
            help = PageController.thatControls(sngdate).help
        End If

        'Me.idinfo_timeperd_timeinfo_____help.Text = helpMsg
        PageController.thatControls(timeinfo2).help = help
    End Sub

    ''' <summary>
    ''' Initialize timeinfo control by peeking to see which element has content.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub load_timeinfo()
        Dim txt As String = ""
        If Me.allControls("idinfo_timeperd_timeinfo_sngdate_caldate").text > "" Then
            txt = Me.allControls("idinfo_timeperd_timeinfo_sngdate_caldate").text
        ElseIf Me.idinfo_timeperd_timeinfo_mdattim_sngdate____caldate.Items.Count > 0 Then
            For Each dt As String In Me.idinfo_timeperd_timeinfo_mdattim_sngdate____caldate.Items
                txt += dt + ", "
            Next
            txt = txt.Substring(0, txt.Length - 2)
        ElseIf Me.allControls("idinfo_timeperd_timeinfo_rngdates_begdate").text > "" OrElse Me.allControls("idinfo_timeperd_timeinfo_rngdates_enddate").text > "" Then
            txt = Me.allControls("idinfo_timeperd_timeinfo_rngdates_begdate").text & "-" & Me.allControls("idinfo_timeperd_timeinfo_rngdates_enddate").text
        End If

        ' Wire event handler after we're done initializing the control, to avoid triggering it prematurely.
        AddHandler Me.timeinfo.TextChanged, AddressOf idinfo_timeperd_timeinfo_TextChanged
        Me.timeinfo.Text = txt
    End Sub

    ''' <summary>
    ''' Event handler to keep track of the last entity/attribute tab the user has selected.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub tcEntityAttr_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tcEntityAttr.SelectedIndexChanged
        ' Save last selected tab's index for later use
        My.Settings.SelectedTabIndexEntityAttr = Me.tcEntityAttr.SelectedIndex
    End Sub

    ''' <summary>
    ''' Reset all warning controls.
    ''' </summary>
    ''' <remarks>Call this when previously displayed validation results are no longer relevant.</remarks>
    Public Sub turnOffWarnings()
        For Each ctrl As Control In Me.allControls.Values
            If TypeOf ctrl Is PictureBox AndAlso ctrl.Name.EndsWith(GlobalVars.idSep & "warning") Then
                Me.HoverToolTip.SetToolTip(ctrl, "")
                ctrl.Visible = False
            End If
        Next
    End Sub

    ''' <summary>
    ''' Event handler to propagate coordinate system info changes upon TextUpdate event of related controls.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub CSI_TextUpdate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles spref_horizsys_CoordinateSystem.TextUpdate, spref_horizsys_Units.TextUpdate, spref_horizsys_Datum.TextUpdate
        Me.spref_horizsys.SelectedItem = Nothing
        PageController.thatControls(Me.spref_horizsys.Name).propagateClusterSelectionChanged(Me.spref_horizsys, Nothing)
    End Sub

    'Private Sub tcEME_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    'Panel33.Size = New System.Drawing.Size(tcEME.Size.Width * 490 / 814, tcEME.Size.Height)
    '    'Panel32.Size = New System.Drawing.Size(tcEME.Size.Width * 324 / 814, tcEME.Size.Height)
    'End Sub

    'Private Sub TempForm_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
    '    If Me.WindowState = FormWindowState.Minimized Then
    '        'Dim t As Type = Type.GetTypeFromProgID("esriFramework.AppRef")
    '        'Dim obj As System.Object = Activator.CreateInstance(t)
    '        'Dim pApp As ESRI.ArcGIS.Framework.IApplication = obj

    '        'Microsoft.VisualBasic.AppActivate(pApp.Caption)
    '        If Me.Opacity = 1 Then
    '            Me.Opacity = 0.11
    '        Else
    '            Me.Opacity = 1
    '        End If
    '        System.Windows.Forms.Application.DoEvents()
    '        System.Threading.Thread.Sleep(200)
    '        System.Windows.Forms.Application.DoEvents()
    '        'Me.WindowState = FormWindowState.Normal
    '    End If
    'End Sub

    ''' <summary>
    ''' Event handler to turn form transparent when user starts dragging it.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>.NET treats beginning of drag as a resize event.</remarks>
    Private Sub EditorForm_ResizeBegin(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.ResizeBegin
        Me.Opacity = My.Settings.EditorFormOpacityOnResize
    End Sub

    ''' <summary>
    ''' Event handler to turn form opaque when user stops dragging it.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>.NET treats ending of drag as a resize event.</remarks>
    Private Sub EditorForm_ResizeEnd(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.ResizeEnd
        Me.Opacity = 1
    End Sub

    ''' <summary>
    ''' Keeps track of the index of entity being operated on. Zero-based.
    ''' </summary>
    ''' <remarks></remarks>
    Private entIdx As Integer
    ''' <summary>
    ''' Keeps track of the index of attribute being operated on. Zero-based. Relative to current entity.
    ''' </summary>
    ''' <remarks></remarks>
    Private attIdx As Integer
    ''' <summary>
    ''' XMLFragment object representing the eainfo section of the metadata XML.
    ''' </summary>
    ''' <remarks></remarks>
    Public eainfoXF As XmlFragment

    ''' <summary>
    ''' Initialize eainfo.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub eainfoInit()
        entIdx = -1
        attIdx = -1
        ' Get eainfo from metadata.
        Dim xml As String = iXPS.GetXml("eainfo")
        ' If no eainfo, then make up one.
        If xml Is Nothing OrElse xml.Trim = "" Then
            xml = "<eainfo></eainfo>"
        End If
        ' Convert to an XMLFragment.
        eainfoXF = XmlFragment.fromXml(xml, True)

        Me.entityUpdate()
        ' Add event handlers to dynamically adjust combo box drop down width.
        AddHandler enttypl.DropDown, AddressOf AdjustWidth_ComboBox_DropDown
        AddHandler enttypds.DropDown, AddressOf AdjustWidth_ComboBox_DropDown
        AddHandler attrlabl.DropDown, AddressOf AdjustWidth_ComboBox_DropDown
        AddHandler attrdefs.DropDown, AddressOf AdjustWidth_ComboBox_DropDown
        ' Add other event handlers.
        eainfoHandlersAdd()
    End Sub

    ''' <summary>
    ''' Add handlers to detect selection of a new entity/attribute.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub eainfoHandlersAdd()
        AddHandler enttypl.SelectedIndexChanged, AddressOf enttypl_SelectedIndexChanged
        AddHandler attrlabl.SelectedIndexChanged, AddressOf attrlabl_SelectedIndexChanged
    End Sub

    ''' <summary>
    ''' Remove handlers that detect selection of a new entity/attribute.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub eainfoHandlersRemove()
        RemoveHandler enttypl.SelectedIndexChanged, AddressOf enttypl_SelectedIndexChanged
        RemoveHandler attrlabl.SelectedIndexChanged, AddressOf attrlabl_SelectedIndexChanged
    End Sub

    ''' <summary>
    ''' Event handler for handling selection of a different entity.
    ''' </summary>
    ''' <param name="sender">Event sender. enttypl combo box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub enttypl_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        eainfoHandlersRemove()
        Dim selIdx As Integer = DirectCast(sender, ComboBox).SelectedIndex
        If entIdx <> selIdx Then
            entIdx = selIdx
            entityUpdate(False)
        End If
        eainfoHandlersAdd()
    End Sub

    ''' <summary>
    ''' Update all entity selection dependent controls optionally resetting controls that are 
    ''' directly entity related.
    ''' </summary>
    ''' <param name="resetEntities">Boolean indicating whether entity stuff will be reset.</param>
    ''' <remarks>We want to reset entity controls in all cases except for navigation from one entity to another.
    ''' Resetting causes controls to re-read their potential values (i.e. combo box options).</remarks>
    Private Sub entityUpdate(Optional ByVal resetEntities As Boolean = True)
        eainfoHandlersRemove()
        If resetEntities Then
            ' Populate entity labels and definition sources
            Me.enttypl.Items.Clear()
            Me.enttypl.Items.AddRange(eainfoXF.collectFromEntities("enttypl"))
            Me.enttypds.Items.Clear()
            Me.enttypds.Items.AddRange(eainfoXF.collectFromEntities("enttypds"))
        End If

        'If no selection but there's sth to select, then point to the first 
        If entIdx < 0 AndAlso Me.enttypl.Items.Count > 0 Then entIdx = 0
        'If pointing beyond the last item, then point to last item
        If entIdx >= Me.enttypl.Items.Count Then entIdx = Me.enttypl.Items.Count - 1

        If entIdx < 0 Then Me.enttypl.Text = ""
        Me.enttypl.SelectedIndex = entIdx
        Me.enttypds.Text = eainfoXF.value(String.Format("detailed[{0}]/enttyp[0]/enttypds[0]", entIdx))
        Me.enttypd.Text = eainfoXF.value(String.Format("detailed[{0}]/enttyp[0]/enttypd[0]", entIdx))
        Me.attributeUpdate()
        eainfoHandlersAdd()
    End Sub

    ''' <summary>
    ''' Event handler for handling selection of a different attribute.
    ''' </summary>
    ''' <param name="sender">Event sender. attrlabl combo box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub attrlabl_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim selIdx As Integer = DirectCast(sender, ComboBox).SelectedIndex
        If attIdx = selIdx Then Exit Sub
        attIdx = selIdx
        attributeUpdate(False)
    End Sub

    ''' <summary>
    ''' Update all attribute selection dependent controls optionally resetting controls that are 
    ''' directly attribute related.
    ''' </summary>
    ''' <param name="resetAttributes">Boolean indicating whether attribute stuff will be reset.</param>
    ''' <remarks>We want to reset attribute controls in all cases except for navigation from one attribute to another.
    ''' Resetting causes controls to re-read their potential values (i.e. combo box options).</remarks>
    Private Sub attributeUpdate(Optional ByVal resetAttributes As Boolean = True)
        RemoveHandler attrlabl.SelectedIndexChanged, AddressOf attrlabl_SelectedIndexChanged
        If resetAttributes Then
            Me.attrlabl.Items.Clear()
            For Each xf As XmlFragment In eainfoXF.valueList(String.Format("detailed[{0}]/attr", entIdx))
                Me.attrlabl.Items.Add(xf.valueList("attrlabl[0]")(0))
            Next
            Me.attrdefs.Items.Clear()
            Me.attrdefs.Items.AddRange(eainfoXF.collectFromAttributes("attrdefs"))
        End If

        'If no selection but there's sth to select, then point to the first 
        If attIdx < 0 AndAlso Me.attrlabl.Items.Count > 0 Then attIdx = 0
        'If pointing beyond the last item, then point to last item
        If attIdx >= Me.attrlabl.Items.Count Then attIdx = Me.attrlabl.Items.Count - 1

        If attIdx < 0 Then Me.attrlabl.Text = ""
        Me.attrlabl.SelectedIndex = attIdx
        Me.attrdefs.Text = eainfoXF.value(String.Format("detailed[{0}]/attr[{1}]/attrdefs[0]", entIdx, attIdx))
        Me.attrdef.Text = eainfoXF.value(String.Format("detailed[{0}]/attr[{1}]/attrdef[0]", entIdx, attIdx))
        Me.domainUpdate()
        AddHandler attrlabl.SelectedIndexChanged, AddressOf attrlabl_SelectedIndexChanged
    End Sub

    ''' <summary>
    ''' Check to see if an XSL pattern exists in eainfo.
    ''' </summary>
    ''' <param name="name">String containing the XSL pattern sought.</param>
    ''' <returns>Boolean value indicating if the XSL patter exists in eainfo.</returns>
    ''' <remarks></remarks>
    Private Function exists(ByVal name As String) As Boolean
        Dim xfl As List(Of XmlFragment) = eainfoXF.valueList(name)
        Return xfl.Count > 0
    End Function

    ''' <summary>
    ''' Update domain info based on currently selected entity/attribute.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub domainUpdate()
        ' Clear existing values
        Me.rdommin.Text = ""
        Me.rdommax.Text = ""
        Me.codesetn.Text = ""
        Me.codesets.Text = ""
        Me.udom.Text = ""
        Me.dgv_edom.Rows.Clear()

        If entIdx < 0 OrElse attIdx < 0 Then Exit Sub

        Dim tp As TabPage = Me.tp_udom
        Dim curAttrXF As XmlFragment = eainfoXF.valueList(String.Format("detailed[{0}]/attr", entIdx))(attIdx)

        ' Init udom
        Me.udom.Items.Clear()
        Me.udom.Items.AddRange(eainfoXF.collectFromAttributeDomains("udom"))
        Me.udom.Text = curAttrXF.value("attrdomv[0]/udom[0]")
        If Me.udom.Text.Trim > "" Then
            tp = Me.tp_udom
        End If

        ' Init rdom
        Me.rdommin.Text = curAttrXF.value("attrdomv[0]/rdom[0]/rdommin[0]")
        Me.rdommax.Text = curAttrXF.value("attrdomv[0]/rdom[0]/rdommax[0]")
        If Me.rdommin.Text.Trim > "" OrElse Me.rdommax.Text.Trim > "" Then
            tp = Me.tp_rdom
        End If

        ' Init codesetd
        Me.codesetn.Text = curAttrXF.value("attrdomv[0]/codesetd[0]/codesetn[0]")
        Me.codesets.Items.Clear()
        Me.codesets.Items.AddRange(eainfoXF.collectFromAttributeDomains("codesetd[0]/codesets"))
        Me.codesets.Text = curAttrXF.value("attrdomv[0]/codesetd[0]/codesets[0]")
        If Me.codesetn.Text.Trim > "" OrElse Me.codesets.Text.Trim > "" Then
            tp = Me.tp_codesetd
        End If

        ' Init edom
        If Me.exists(String.Format("detailed[{0}]/attr[{1}]/attrdomv[0]/edom", entIdx, attIdx)) Then
            Dim attrdefsList As XmlFragment() = eainfoXF.collectFromAttributeDomains("edom[0]/edomvds")
            Dim attrdefsList2(attrdefsList.Length - 1) As String
            For i As Integer = 0 To attrdefsList.Length - 1
                attrdefsList2(i) = attrdefsList(i).displayName
            Next
            For Each xf As XmlFragment In curAttrXF.valueList("attrdomv")
                Dim row As New DataGridViewRow
                row.CreateCells(Me.dgv_edom)
                row.Cells(0).Value = xf.value("edom[0]/edomv[0]")
                row.Cells(1).Value = xf.value("edom[0]/edomvd[0]")
                'DirectCast(row.Cells(2), DataGridViewComboBoxCell).ValueMember = "displayName"
                DirectCast(row.Cells(2), DataGridViewComboBoxCell).Items.AddRange(attrdefsList2)
                DirectCast(row.Cells(2), DataGridViewComboBoxCell).DropDownWidth = 600                       'AE
                row.Cells(2).Value = xf.value("edom[0]/edomvds[0]")
                If row.Cells(0).Value > "" OrElse row.Cells(1).Value > "" OrElse row.Cells(2).Value > "" Then
                    Me.dgv_edom.Rows.Add(row)
                    tp = Me.tp_edom
                End If
            Next
        End If

        ' Select the last domain tab that has values from metadata or default to udom.
        Me.tcDomain.SelectedTab = tp
    End Sub

    ''' <summary>
    ''' Event handler that executes when an entity is deleted on the GUI.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnDeleteEntity_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteEntity.Click
        ' If no entity, nothing to do
        If entIdx < 0 Then Exit Sub

        eainfoXF.delete("detailed", entIdx)
        entityUpdate()
    End Sub

    ''' <summary>
    ''' Event handler that executes when an attribute is deleted on the GUI.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnDeleteAttribute_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteAttribute.Click
        ' If no attribute, nothing to do
        If attIdx < 0 Then Exit Sub

        eainfoXF.delete(String.Format("detailed[{0}]/attr", entIdx), attIdx)
        attributeUpdate()
    End Sub

    ''' <summary>
    ''' Event handler that executes when an entity is added on the GUI.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnAddEntity_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddEntity.Click
        ' Insert after current selection
        entIdx += 1
        ' Create XMLFragment from a nearly empty XML for the new entity.
        Dim newXF As XmlFragment = XmlFragment.fromXml("<detailed><enttyp><enttypl>new_entity</enttypl></enttyp></detailed>")
        ' Insert new XMLFragment into eainfo.
        eainfoXF.add("detailed", entIdx, newXF)
        ' Finally update all selections.
        attIdx = -1
        entityUpdate()
        Me.enttypl.SelectedIndex = entIdx
    End Sub

    ''' <summary>
    ''' Event handler that executes when an attribute is added on the GUI.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnAddAttribute_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddAttribute.Click
        ' If no entity, nothing to do
        If entIdx < 0 Then Exit Sub
        ' Insert after current selection
        attIdx += 1
        ' Create XMLFragment from a nearly empty XML for the new attribute.
        eainfoXF.valueList("detailed")(entIdx).add("attr", attIdx, XmlFragment.fromXml("<attr><attrlabl>new_attribute</attrlabl></attr>"))
        ' Finally update all selections.
        attributeUpdate()
        Me.attrlabl.SelectedIndex = attIdx
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves enttypl combo box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. enttypl combo box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the entity's name.</remarks>
    Private Sub enttypl_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles enttypl.Leave
        If sender.selectedItem IsNot Nothing Then Exit Sub
        eainfoXF.setValue(String.Format("detailed[{0}]/enttyp[0]/enttypl[0]", entIdx), sender.text)
        entityUpdate()
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves enttypds combo box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. enttypds combo box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the entity's definition source.</remarks>
    Private Sub enttypds_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles enttypds.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/enttyp[0]/enttypds[0]", entIdx), sender.text)
        entityUpdate()
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves enttypd text box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. enttypd text box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the entity's definition.</remarks>
    Private Sub enttypd_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles enttypd.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/enttyp[0]/enttypd[0]", entIdx), sender.text)
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves attrlabl combo box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. attrlabl combo box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the attribute's name.</remarks>
    Private Sub attrlabl_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles attrlabl.Leave
        If sender.selectedItem IsNot Nothing Then Exit Sub
        eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrlabl[0]", entIdx, attIdx), sender.text)
        attributeUpdate()
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves attrdefs combo box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. attrdefs combo box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the attribute's definition source.</remarks>
    Private Sub attrdefs_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles attrdefs.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdefs[0]", entIdx, attIdx), sender.text)
        attributeUpdate()
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves attrdef text box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. attrdef text box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the attribute's definition.</remarks>
    Private Sub attrdef_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles attrdef.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdef[0]", entIdx, attIdx), sender.text)
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves rdommin text box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. rdommin text box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the range domain's min value.</remarks>
    Private Sub rdommin_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdommin.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdomv[0]/rdom[0]/rdommin[0]", entIdx, attIdx), sender.text)
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves rdommax text box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. rdommax text box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the range domain's max value.</remarks>
    Private Sub rdommax_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdommax.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdomv[0]/rdom[0]/rdommax[0]", entIdx, attIdx), sender.text)
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves codesetn text box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. codesetn text box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the codeset domain's name.</remarks>
    Private Sub codesetn_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles codesetn.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdomv[0]/codesetd[0]/codesetn[0]", entIdx, attIdx), sender.text)
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves codesets text box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. codesets text box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the codeset domain's source.</remarks>
    Private Sub codesets_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles codesets.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdomv[0]/codesetd[0]/codesets[0]", entIdx, attIdx), sender.text)
        domainUpdate()
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves udom text box after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. udom text box.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the unrepresentable domain.</remarks>
    Private Sub udom_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles udom.Leave
        eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdomv[0]/udom[0]", entIdx, attIdx), sender.text)
        domainUpdate()
    End Sub

    ''' <summary>
    ''' Event handler that executes when focus leaves a DataGridViewCell after modifying its text.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to update the enumerated domain.</remarks>
    Private Sub dgv_edom_CellLeave(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles dgv_edom.CellLeave
        Dim cell As DataGridViewCell = dgv_edom.CurrentCell
        If cell.FormattedValue = cell.EditedFormattedValue Then Exit Sub

        Select Case cell.ColumnIndex
            Case 0
                eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdomv[{2}]/edom[0]/edomv[0]", entIdx, attIdx, cell.RowIndex), cell.EditedFormattedValue)
            Case 1
                eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdomv[{2}]/edom[0]/edomvd[0]", entIdx, attIdx, cell.RowIndex), cell.EditedFormattedValue)
            Case 2
                eainfoXF.setValue(String.Format("detailed[{0}]/attr[{1}]/attrdomv[{2}]/edom[0]/edomvds[0]", entIdx, attIdx, cell.RowIndex), cell.EditedFormattedValue)
                'domainUpdate()
        End Select
        'cell.Value = cell.EditedFormattedValue
    End Sub

    ' ''' <summary>
    ' ''' Event handler that executes when focus leaves a DataGridViewComboBoxCell after modifying its text.
    ' ''' </summary>
    ' ''' <param name="sender">Event sender. Not used.</param>
    ' ''' <param name="e">Event arguments. Used to get the newly entered value.</param>
    ' ''' <remarks>We use this event to insert the new value if not already available as an option for the combo box.</remarks>
    'Private Sub dgv_edom_CellValidating(ByVal sender As Object, ByVal e As DataGridViewCellValidatingEventArgs) Handles dgv_edom.CellValidating
    '    Dim dgv As DataGridView = sender
    '    If Not TypeOf dgv.CurrentCell Is DataGridViewComboBoxCell Then Exit Sub
    '    If dgv.Name <> "dgv_edom" AndAlso dgv.Columns(dgv.CurrentCell.ColumnIndex).Name <> "srccurr" Then Exit Sub
    '    Dim cell As DataGridViewComboBoxCell = dgv.CurrentCell
    '    If cell IsNot Nothing AndAlso Not cell.Items.Contains(e.FormattedValue) Then
    '        ' Insert the new value into position 0  in the item collection of the cell
    '        cell.Items.Insert(0, e.FormattedValue)
    '        ' When setting the Value of the cell, the string is not shown until it has been
    '        ' committed. The code below will make sure it is committed directly.
    '        If dgv.IsCurrentCellDirty Then
    '            ' Ensure the inserted value will be shown directly.
    '            ' First tell the DataGridView to commit itself using the Commit context...
    '            dgv.CommitEdit(DataGridViewDataErrorContexts.Commit)
    '        End If
    '        ' ...then set the Value that needs to be committed in order to be displayed directly.
    '        cell.Value = cell.Items(0)
    '    End If
    'End Sub

    ' ''' <summary>
    ' ''' Event handler that executes when a combo box in a data grid view is actually showing 
    ' ''' (they are in a way phantom controls that get created when they have the focus).
    ' ''' </summary>
    ' ''' <param name="sender">Event sender. Not used.</param>
    ' ''' <param name="e">Event arguments. Used to get the combo box control.</param>
    ' ''' <remarks>We use this event to set properties for the combo box that diverge from its defaults.</remarks>
    'Private Sub dgv_edom_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles dgv_edom.EditingControlShowing
    '    Dim dgv As DataGridView = sender
    '    If Not TypeOf dgv.CurrentCell Is DataGridViewComboBoxCell Then Exit Sub
    '    If dgv.Name <> "dgv_edom" AndAlso dgv.Columns(dgv.CurrentCell.ColumnIndex).Name <> "srccurr" Then Exit Sub

    '    Dim comboControl As ComboBox = TryCast(e.Control, ComboBox)
    '    If comboControl IsNot Nothing Then
    '        ' Set the DropDown style to get an editable ComboBox
    '        If comboControl.DropDownStyle <> ComboBoxStyle.DropDown Then
    '            comboControl.DropDownStyle = ComboBoxStyle.DropDown
    '        End If
    '        ' This part doesn't quite work. Workaround is satisfactory though.
    '        'AddHandler comboControl.DropDown, AddressOf AdjustWidth_ComboBox_DropDown
    '        'comboControl.DropDownWidth = 600
    '        'AdjustWidth_ComboBox_DropDown2(comboControl, Nothing)
    '    End If
    'End Sub

    ''' <summary>
    ''' Event handler that executes when user click the button to add a new procstep.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnAddDgvRow(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddProcstep.Click, btnAddSrcinfo.Click, btnAddSDTS.Click, btnAddVPF.Click
        Dim btn As Button = sender
        For Each ctrl As Control In btn.Parent.Controls
            If TypeOf ctrl Is DataGridView Then
                Dim dgv As DataGridView = ctrl
                dgv.Rows.Add()
            End If
        Next
    End Sub

    ''' <summary>
    ''' Event handler that executes when user click the button to delete the selected procstep(s).
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub btnDelDgvRow(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelProcstep.Click, dataqual_lineage_procstep.UserDeletingRow, btnDelSrcinfo.Click, dataqual_lineage_srcinfo.UserDeletingRow, btnDelSDTS.Click, btnDelVPF.Click, spdoinfo_ptvctinf_sdtsterm.UserDeletingRow, spdoinfo_ptvctinf_vpfterm_vpfinfo.UserDeletingRow
        If TypeOf sender Is Button Then
            For Each ctrl As Control In sender.Parent.Controls
                If TypeOf ctrl Is DataGridView Then
                    Dim dgv As DataGridView = ctrl
                    For Each r As DataGridViewRow In dgv.SelectedRows
                        If Not r.IsNewRow Then
                            dgv.Rows.Remove(r)
                            'Else
                            '    For Each c As DataGridViewCell In r.Cells
                            '        If nv(c.Value) > "" Then
                            '            c.Value = ""
                            '        End If
                            '    Next

                        End If
                    Next
                    Exit For
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Event handler that executes when procstep selection changes.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks>We use this event to enable/disable procstep deletion button. We enable it only if there are any selected rows.</remarks>
    Private Sub dgv_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dataqual_lineage_procstep.RowsAdded, dataqual_lineage_procstep.RowsRemoved, dataqual_lineage_procstep.SelectionChanged, dataqual_lineage_srcinfo.RowsRemoved, dataqual_lineage_srcinfo.SelectionChanged, dataqual_lineage_srcinfo.RowsAdded, spdoinfo_ptvctinf_sdtsterm.SelectionChanged, spdoinfo_ptvctinf_sdtsterm.RowsAdded, spdoinfo_ptvctinf_sdtsterm.RowsRemoved, spdoinfo_ptvctinf_vpfterm_vpfinfo.SelectionChanged, spdoinfo_ptvctinf_vpfterm_vpfinfo.RowsRemoved, spdoinfo_ptvctinf_vpfterm_vpfinfo.RowsAdded
        Dim dgv As DataGridView = sender
        For Each ctrl As Control In dgv.Parent.Controls
            If TypeOf ctrl Is Button AndAlso ctrl.Name.StartsWith("btnDel") Then
                Dim btn As Button = ctrl
                btn.Enabled = dgv.SelectedRows.Count > 0 AndAlso (dgv.SelectedRows.Count > 1 OrElse Not dgv.SelectedRows(0).IsNewRow)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Initialize the user keywords tab by retrieving its name from the database.
    ''' Defaults to "User".
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub initUserKeywordsTab()
        ' Hook to set user keywords tab
        tpUser.Text = Utils.nv(Utils.getFromSingleValuedSQL("SELECT DISTINCT themekt from 1l_KeywordsUser"), "User")
    End Sub

    ''' <summary>
    ''' Boolean that controls if all linkage input boxes should be shown or the first few.
    ''' </summary>
    ''' <remarks></remarks>
    Dim moreLinkages As Boolean = False

    ''' <summary>
    ''' Show/Hide more linkages upon user click
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnMoreLinkages_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMoreLinkages.Click
        If moreLinkages Then
            GroupBox1.Size = New Size(GroupBox1.Size.Width, GroupBox2.Location.Y - GroupBox1.Location.Y - 1)
            btnMoreLinkages.Image = My.Resources.plus
        Else
            GroupBox1.Size = New Size(GroupBox1.Size.Width, GroupBox3.Location.Y + GroupBox3.Size.Height - GroupBox1.Location.Y)
            btnMoreLinkages.Image = My.Resources.minus
        End If
        moreLinkages = Not moreLinkages
    End Sub

    ''' <summary>
    ''' Keep the contact person/org in sync independent of whether person or org is the primary contact.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cntp_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles idinfo_ptcontac_cntinfo_cntperp.CheckedChanged, idinfo_ptcontac_cntinfo_cntorgp.CheckedChanged, metainfo_metc_cntinfo_cntperp.CheckedChanged, metainfo_metc_cntinfo_cntorgp.CheckedChanged, distinfo_distrib_cntinfo_cntperp.CheckedChanged, distinfo_distrib_cntinfo_cntorgp.CheckedChanged
        If Me.FormState = State.Loading Then Return
        Dim senderName As String = DirectCast(sender, RadioButton).Name
        If senderName = "" Then Return
        Dim thisName As String = senderName.Substring(senderName.LastIndexOf("_") + 1)
        Dim baseName As String = senderName.Substring(0, senderName.LastIndexOf("_"))
        Dim thatName As String

        If thisName = "cntperp" Then
            thatName = "cntorgp"
        Else
            thatName = "cntperp"
        End If

        mirror(baseName + "_" + thisName + "_cntper", baseName + "_" + thatName + "_cntper")
        mirror(baseName + "_" + thisName + "_cntorg", baseName + "_" + thatName + "_cntorg")
    End Sub

    ''' <summary>
    ''' Helper sub to keep the contact person/org in sync independent of whether person or org is the primary contact.
    ''' </summary>
    ''' <param name="this"></param>
    ''' <param name="that"></param>
    ''' <remarks></remarks>
    Private Sub mirror(ByVal this As String, ByVal that As String)
        Dim thisCtrl As TextBox = Me.allControls(this)
        Dim thatCtrl As TextBox = Me.allControls(that)

        If thisCtrl.Text.Trim = "" AndAlso thatCtrl.Text.Trim <> "" Then
            thisCtrl.Text = thatCtrl.Text
        ElseIf thisCtrl.Text.Trim <> "" AndAlso thatCtrl.Text.Trim = "" Then
            thatCtrl.Text = thisCtrl.Text
        End If
    End Sub

    ''' <summary>
    ''' Open a file handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        If MdUtils.promptForMdFile() Then
            Me.Close()
            MdUtils.quit = False
        End If
    End Sub

    ''' <summary>
    ''' View metadata XML handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ViewMetadataXMLToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewMetadataXMLToolStripMenuItem.Click
        Try
            btnViewMetadataXML_Click(Nothing, Nothing)
        Catch ex As Exception
            Dim z = 1
        End Try
    End Sub

    ''' <summary>
    ''' Validate metadata handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ValidateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValidateToolStripMenuItem.Click
        PageController.ValidateAction(Me)
    End Sub

    ''' <summary>
    ''' Save the metadata currently being edited to file.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <remarks></remarks>
    Private Sub saveMdToFile(ByVal filename As String)
        ' Save form (this will update iXPS)
        PageController.PageSaver(Me)

        ' Write out current metadata XML to filesystem.
        My.Computer.FileSystem.WriteAllText(filename, iXPS.GetXml(""), False)
        Me.FormChanged = Modified.Clean
    End Sub

    ''' <summary>
    ''' Save metadata handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        If MdUtils.currentlyEditing Is Nothing Then
            ' If no file specified, then it's a new document. So do a "Save As"
            SaveAsToolStripMenuItem_Click(sender, e)
        Else
            saveMdToFile(MdUtils.currentlyEditing)
        End If
    End Sub

    ''' <summary>
    ''' Save metadata as handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveAsToolStripMenuItem.Click
        Dim dlg As SaveFileDialog = New SaveFileDialog()
        dlg.DefaultExt = ".xml"
        dlg.AddExtension = True
        dlg.Filter = "XML file (*.xml)|*.xml|All files (*.*)|*.*"
        Dim res As DialogResult = dlg.ShowDialog()
        If res = Windows.Forms.DialogResult.OK Then
            MdUtils.currentlyEditing = dlg.FileName
            saveMdToFile(MdUtils.currentlyEditing)
            'ValidateToolStripMenuItem_Click(sender, e)
            updateWindowTitle()
        End If
    End Sub

    ''' <summary>
    ''' New metadata record handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripMenuItem.Click
        Me.Close()
        MdUtils.quit = False
    End Sub

    ''' <summary>
    ''' Help contents handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ContentsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ContentsToolStripMenuItem.Click
        HelpSeeker("")
    End Sub

    ''' <summary>
    ''' View validation results in user interface setting handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ViewInUserInterfaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewInUserInterfaceToolStripMenuItem.Click
        My.Settings.ViewValidationResultsInEME = Me.ViewInUserInterfaceToolStripMenuItem.Checked
    End Sub

    ''' <summary>
    ''' View validation results in browser setting handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ViewInBrowserWindowToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewInBrowserWindowToolStripMenuItem.Click
        My.Settings.ViewValidationResultsInBrowser = Me.ViewInBrowserWindowToolStripMenuItem.Checked
    End Sub

    ''' <summary>
    ''' Open database handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OpenDatabaseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenDatabaseToolStripMenuItem.Click
        Try
            btnOpenDatabase_Click(Nothing, Nothing)
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Spell check handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SpellCheckToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SpellCheckToolStripMenuItem.Click
        Try
            If TypeOf Me.ActiveControl Is DataGridViewTextBoxEditingControl Then
                Dim ctrl As DataGridViewTextBoxEditingControl = Me.ActiveControl
                ctrl.EditingControlDataGridView.EndEdit()
                'ctrl.EditingControlDataGridView.Focus()
                'ctrl.EditingControlDataGridView.Refresh()
            End If
            spellcheckTab(Nothing, Nothing)
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Exit handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
        MdUtils.quit = True
    End Sub

    ''' <summary>
    ''' Set fields to default values handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SetDefaultToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SetDefaultToolStripMenuItem.Click
        Try
            btnSetDefault_Click(Nothing, Nothing)
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Refresh configuration from database handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RefreshFromDatabaseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshFromDatabaseToolStripMenuItem.Click
        btnRefreshFromDB_Click(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' Remove ESRI tags handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RemoveESRITagsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveESRITagsToolStripMenuItem.Click
        btnRemoveESRITags_Click(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' Find and replace handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FindAndReplaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindAndReplaceToolStripMenuItem.Click
        btnFindReplace_Click(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' Validation settings handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ValidationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LocalToolStripMenuItem.Click, ViaWebserviceToolStripMenuItem.Click
        My.Settings.ValidationEnabled = Not My.Settings.ValidationEnabled
        Me.ViaWebserviceToolStripMenuItem.Checked = My.Settings.ValidationEnabled
        Me.LocalToolStripMenuItem.Checked = Not My.Settings.ValidationEnabled
        Me.tbValidationTimeout.Enabled = Me.ViaWebserviceToolStripMenuItem.Checked
    End Sub

    ''' <summary>
    ''' Open About form handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        AboutBox.ShowDialog()
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
    ''' Event handler to install/uninstall EPA stylesheets
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub InstallEPAStylesheetsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles InstallEPAStylesheetsToolStripMenuItem.Click
        If InstallEPAStylesheetsToolStripMenuItem.Text.StartsWith("Install") Then
            If MessageBox.Show("If you proceed, several metadata stylesheets customized for EPA will be installed. Please note that these stylesheets based on ArcGIS Desktop 10.0 Service Pack 3 stylesheets and do not contain changes made in later ArcGIS releases.", "EPA Stylesheets", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = Windows.Forms.DialogResult.OK Then
                If copyDir(getEpaStylesheetsSourceFolder, getArcgisMetadataFolder) Then
                    MessageBox.Show("The stylesheets have been installed. Please remember to select the one you prefer to use in ArcCatalog (Customize -> ArcCatalog Options -> Metadata Tab).", "EPA Stylesheets")
                Else
                    MessageBox.Show("At least one file could not be copied. Please make sure you have enough permission to write to ArcGIS install directory.")
                End If
            End If
        Else
            If MessageBox.Show("If you proceed, metadata stylesheets customized for EPA will be uninstalled.", "EPA Stylesheets", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = Windows.Forms.DialogResult.OK Then
                If uncopyDir(getEpaStylesheetsSourceFolder, getArcgisMetadataFolder) Then
                    MessageBox.Show("The stylesheets have been uninstalled. Please remember to select a different one to use in ArcCatalog (Customize -> ArcCatalog Options -> Metadata Tab).", "EPA Stylesheets")
                Else
                    MessageBox.Show("At least one file could not be removed. Please make sure you have enough permission to write to ArcGIS install directory.")
                End If
            End If
        End If
        setupEpaStylesheetsMenuItem()
    End Sub

    ''' <summary>
    ''' Update menu item name to reflect correct action based on install status of EPA stylesheets
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupEpaStylesheetsMenuItem()
        ' Set up menu item for EPA stylesheets depending on existence of ArcGIS Desktop and stylesheet install status
        If getArcgisInstallDir() Is Nothing Then
            InstallEPAStylesheetsToolStripMenuItem.Enabled = False
        Else
            If copyDirPerformed(getEpaStylesheetsSourceFolder, getArcgisMetadataFolder) Then
                InstallEPAStylesheetsToolStripMenuItem.Text = "Uninstall EPA Stylesheets"
            Else
                InstallEPAStylesheetsToolStripMenuItem.Text = "Install EPA Stylesheets"
            End If
        End If

    End Sub

    ''' <summary>
    ''' Show contact form if user clicked on such button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub dataqual_lineage_procstep_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dataqual_lineage_procstep.CellContentClick, dataqual_lineage_procstep.CellEndEdit
        Dim dgv As DataGridView = sender
        If dgv.Columns(e.ColumnIndex).CellType.Name <> "DataGridViewButtonCell" Then Exit Sub

        If e.ColumnIndex = dgv.ColumnCount - 1 AndAlso -1 < e.RowIndex AndAlso e.RowIndex < dgv.RowCount Then
            Dim deleteIfEmpty As Boolean = copyButtonCellToNewRow(dgv, e.RowIndex, e.ColumnIndex)
            Dim frm As New ContactForm(dgv.Rows(e.RowIndex))
            frm.ShowDialog()
            PageController.updateContactButton(dgv.Rows(e.RowIndex), deleteIfEmpty)
        End If
    End Sub

    ''' <summary>
    ''' spdoinfo elements are displayed in two modes (minimized/maximized). Set things up
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnObjectInfo_Click(sender As System.Object, e As System.EventArgs) Handles btnObjectInfo.Click
        Dim spdoinfoMinimized As Boolean = (gbSpdoinfo.Location.Y > 20)

        setupSpdoinfo(spdoinfoMinimized)
    End Sub

    ''' <summary>
    ''' Maximize or minimize spdoinfo panel.
    ''' </summary>
    ''' <param name="maximize"></param>
    ''' <remarks>Numbers carefully hand-tuned to avoid incorrect display issue that have been reported by only one beta tester.</remarks>
    Private Sub setupSpdoinfo(maximize As Boolean)
        ' 518 is TabPage2.Size.Height but we hardwire it here to fix Donnie Williams' screen issue
        ' If TabPage2.Size.Height is ever adjusted, 518 below needs to be adjusted accordingly.
        Dim deltaYRatio As Double = 436 / 518
        Dim initialHeightRatio As Double = 77 / 518
        If maximize Then
            gbSpdoinfo.Location = New Point(gbSpdoinfo.Location.X, gbQuality.Location.Y)
            gbSpdoinfo.Size = New Size(gbSpdoinfo.Size.Width, Math.Round(initialHeightRatio * TabPage2.Size.Height + deltaYRatio * TabPage2.Size.Height) - 4)
            btnObjectInfo.Text = "Close Object Info"
        Else
            gbSpdoinfo.Location = New Point(gbSpdoinfo.Location.X, Math.Round(gbQuality.Location.Y + deltaYRatio * TabPage2.Size.Height))
            gbSpdoinfo.Size = New Size(gbSpdoinfo.Size.Width, Math.Round(initialHeightRatio * TabPage2.Size.Height) - 4)
            btnObjectInfo.Text = "Edit Object Info"
        End If
    End Sub

    ''' <summary>
    ''' Keeps track of the previously selected index
    ''' </summary>
    ''' <remarks></remarks>
    Private spdoinfo_direct_PreviousSelectedIndex As Integer = 0

    ''' <summary>
    ''' Event handler to gracefully handle the impacts of changing the spdoinfo/direct element.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub spdoinfo_direct_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles spdoinfo_direct.SelectedIndexChanged
        gbRastinfo.Enabled = (sender.selectedValue = "Raster")
        gbSdtsterm.Enabled = (sender.selectedValue = "Point" OrElse sender.selectedValue = "Vector")
        gbVpfterm.Enabled = gbSdtsterm.Enabled

        Dim spdoinfo_direct_PreviousSelectedTab As TabPage = tcSpdoinfo.SelectedTab
        If sender.selectedValue = "Raster" Then
            tcSpdoinfo.SelectedTab = tpRastinfo
            If spdoinfo_ptvctinf_sdtsterm.RowCount > 1 OrElse spdoinfo_ptvctinf_vpfterm_vpfinfo.RowCount > 1 OrElse spdoinfo_ptvctinf_vpfterm_vpflevel.SelectedValue <> "" Then
                If MsgBox("You have selected 'Raster', however, you have information for 'Point' or 'Vector'. EME will delete this information if you proceed.", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
                    While spdoinfo_ptvctinf_sdtsterm.RowCount > 1
                        spdoinfo_ptvctinf_sdtsterm.Rows.RemoveAt(0)
                    End While
                    While spdoinfo_ptvctinf_vpfterm_vpfinfo.RowCount > 1
                        spdoinfo_ptvctinf_vpfterm_vpfinfo.Rows.RemoveAt(0)
                    End While
                    spdoinfo_ptvctinf_vpfterm_vpflevel.SelectedValue = ""
                    spdoinfo_direct_PreviousSelectedIndex = sender.selectedindex
                Else
                    RemoveHandler spdoinfo_direct.SelectedIndexChanged, AddressOf spdoinfo_direct_SelectedIndexChanged
                    sender.selectedindex = spdoinfo_direct_PreviousSelectedIndex
                    tcSpdoinfo.SelectedTab = spdoinfo_direct_PreviousSelectedTab
                    AddHandler spdoinfo_direct.SelectedIndexChanged, AddressOf spdoinfo_direct_SelectedIndexChanged
                End If
            End If
        ElseIf (sender.selectedValue = "Point" OrElse sender.selectedValue = "Vector") AndAlso tcSpdoinfo.SelectedTab Is tpRastinfo Then
            tcSpdoinfo.SelectedTab = tpSdtsterm
            If spdoinfo_rastinfo_rasttype.SelectedValue <> "" OrElse spdoinfo_rastinfo_rowcount.Text.Trim > "" OrElse spdoinfo_rastinfo_colcount.Text.Trim > "" OrElse spdoinfo_rastinfo_vrtcount.Text.Trim > "" Then
                If MsgBox("You have selected '" + sender.selectedValue + "', however, you have information for 'Raster'. EME will delete this information if you proceed.", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
                    spdoinfo_rastinfo_rasttype.SelectedValue = ""
                    spdoinfo_rastinfo_rowcount.Text = ""
                    spdoinfo_rastinfo_colcount.Text = ""
                    spdoinfo_rastinfo_vrtcount.Text = ""
                    spdoinfo_direct_PreviousSelectedIndex = sender.selectedindex
                Else
                    RemoveHandler spdoinfo_direct.SelectedIndexChanged, AddressOf spdoinfo_direct_SelectedIndexChanged
                    sender.selectedindex = spdoinfo_direct_PreviousSelectedIndex
                    tcSpdoinfo.SelectedTab = spdoinfo_direct_PreviousSelectedTab
                    AddHandler spdoinfo_direct.SelectedIndexChanged, AddressOf spdoinfo_direct_SelectedIndexChanged
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Event handler to show lineage panel.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEditLineage_Click(sender As System.Object, e As System.EventArgs) Handles btnEditLineage.Click
        gbLineage.Location = New Point(gbQuality.Location.X, gbQuality.Location.X)
        gbLineage.Size = New Size(gbEainfo.Location.X + gbEainfo.Size.Width - gbQuality.Location.X, gbEainfo.Location.Y + gbEainfo.Size.Height - gbQuality.Location.Y)
        gbLineage.Visible = True
    End Sub

    ''' <summary>
    ''' Event handler to hide lineage panel.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCloseLineage_Click(sender As System.Object, e As System.EventArgs) Handles btnCloseLineage.Click
        updateLineageCounts()
        gbLineage.Visible = False
    End Sub

    ''' <summary>
    ''' Show citation form if user clicked on such button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub dataqual_lineage_srcinfo_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dataqual_lineage_srcinfo.CellEndEdit, dataqual_lineage_srcinfo.CellContentClick
        Dim dgv As DataGridView = sender
        If dgv.Columns(e.ColumnIndex).CellType.Name <> "DataGridViewButtonCell" Then Exit Sub

        If e.RowIndex > -1 AndAlso e.RowIndex < dgv.RowCount Then
            If e.ColumnIndex = srccite.Index Then
                Dim deleteIfEmpty As Boolean = copyButtonCellToNewRow(dgv, e.RowIndex, e.ColumnIndex)

                Dim frm As New CitationForm(dgv.Rows(e.RowIndex).Cells("srccite").Tag, nv(dgv.Rows(e.RowIndex).Cells("srccitea").Value)) 'AE
                frm.ShowDialog()
                PageController.updateCitationButton(dgv.Rows(e.RowIndex).Cells("srccite"), deleteIfEmpty)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Copies useful info from a button cell inside one row to a new row.
    ''' </summary>
    ''' <param name="dgv">Grid to operate within</param>
    ''' <param name="rowIndex">Row index of source row</param>
    ''' <param name="columnIndex">Column index of source cell</param>
    ''' <returns>True if copy was performed</returns>
    ''' <remarks>Helps tame  a grid object to behave consistently in creating new rows when clicked on a cell button vs a regular cell.</remarks>
    Private Function copyButtonCellToNewRow(dgv As DataGridView, rowIndex As Integer, columnIndex As Integer) As Boolean
        If dgv.Rows(rowIndex).IsNewRow Then
            dgv.Rows.AddCopy(rowIndex)
            dgv.Rows(rowIndex).Selected = True
            dgv.Rows(rowIndex + 1).Selected = False
            Dim fromCell As DataGridViewButtonCell = dgv.Rows(rowIndex + 1).Cells(columnIndex)
            fromCell.Value = ""
            fromCell.ToolTipText = ""
            fromCell.Tag = Nothing
            Return True
        Else
            Return False
        End If
    End Function

   
    ''' <summary>
    ''' Event handler to manage visible alerts in areas that have different display modes.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub warning_VisibleChanged(sender As System.Object, e As System.EventArgs) Handles dataqual_lineage_srcinfo_____warning.VisibleChanged, dataqual_lineage_____warning.VisibleChanged, dataqual_lineage_procstep_____warning.VisibleChanged
        Dim pb As PictureBox = sender
        Dim pb2 As PictureBox = Nothing
        If pb.Name.Contains("srcinfo") Then
            pb2 = dataqual_lineage_srcinfo_____warning2
        ElseIf pb.Name.Contains("procstep") Then
            pb2 = dataqual_lineage_procstep_____warning2
        ElseIf pb.Name.Contains("lineage") Then
            pb2 = dataqual_lineage_____warning2
        End If
        If pb2 Is Nothing Then Exit Sub
        pb2.Visible = pb.Visible
        Me.HoverToolTip.SetToolTip(pb2, HoverToolTip.GetToolTip(pb))
    End Sub

    ''' <summary>
    ''' Display help for appropriate element if user clicks on a grid header.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub dgv_ColumnHeaderMouseClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dataqual_lineage_srcinfo.ColumnHeaderMouseClick, dgv_edom.ColumnHeaderMouseClick, dataqual_lineage_procstep.ColumnHeaderMouseClick
        Dim dgv As DataGridView = sender
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso e.Clicks > 0 Then
            Utils.HelpSeeker("/t" + Str(tcEME.SelectedIndex + 1).Trim + "_" + dgv.Columns(e.ColumnIndex).Name + ".html", GlobalVars.proc)
        End If
    End Sub

    ''' <summary>
    ''' Event handler to copy absres value to ordres
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCopy_absres2ordres_Click(sender As System.Object, e As System.EventArgs) Handles btnCopy_absres2ordres.Click
        btnCopy_textBox2textBox(spref_horizsys_planar_planci_coordrep_absres, spref_horizsys_planar_planci_coordrep_ordres)
    End Sub

    ''' <summary>
    ''' Event handler to copy latres value to longres
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCopy_latres2longres_Click(sender As System.Object, e As System.EventArgs) Handles btnCopy_latres2longres.Click
        btnCopy_textBox2textBox(spref_horizsys_geograph_longres, spref_horizsys_geograph_latres)
    End Sub

    ''' <summary>
    ''' Copies value of one text box to another if one has value and the other is empty
    ''' </summary>
    ''' <param name="c1"></param>
    ''' <param name="c2"></param>
    ''' <remarks></remarks>
    Private Sub btnCopy_textBox2textBox(c1 As TextBox, c2 As TextBox)
        If c2.Text.Trim <> "" AndAlso c1.Text.Trim = "" Then
            c1.Text = c2.Text
        Else
            c2.Text = c1.Text
        End If
    End Sub

    ''' <summary>
    ''' Update counts of processing step and source information records
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub updateLineageCounts()
        lbl_dataqual_lineage_procstep.Text = decorateCountPhrase("processing step", dataqual_lineage_procstep.Rows.Count - 1)
        lbl_dataqual_lineage_srcinfo.Text = decorateCountPhrase("source", dataqual_lineage_srcinfo.Rows.Count - 1)
    End Sub



     ''' <summary>
     ''' Event handler that executes when focus leaves a DataGridViewComboBoxCell after modifying its text.
     ''' </summary>
     ''' <param name="sender">Event sender. Not used.</param>
     ''' <param name="e">Event arguments. Used to get the newly entered value.</param>
     ''' <remarks>We use this event to insert the new value if not already available as an option for the combo box.</remarks>
    Private Sub dgv_CellValidating(ByVal sender As Object, ByVal e As DataGridViewCellValidatingEventArgs) Handles dataqual_lineage_srcinfo.CellValidating, dataqual_lineage_procstep.CellValidating, dgv_edom.CellValidating ', spdoinfo_ptvctinf_sdtsterm.CellValidating
        Dim dgv As DataGridView = sender
        If Not TypeOf dgv.CurrentCell Is DataGridViewComboBoxCell Then Exit Sub

        Dim cb As DataGridViewComboBoxEditingControl = dgv.EditingControl
        If cb Is Nothing Then Return

        ' check if item is already in drop down, if not, add it to all
        If Not cb.Items.Contains(cb.Text) Then
            Dim comboColumn As DataGridViewComboBoxColumn = dgv.Columns(dgv.CurrentCell.ColumnIndex)
            cb.Items.Add(cb.Text)
            comboColumn.Items.Add(cb.Text)
            dgv.CurrentCell.Value = cb.Text
        End If
    End Sub

    ''' <summary>
    ''' Sets the behavior of the combo box object within a grid.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Need to do this when cell is showing as the combo box object is created on the fly.</remarks>
    Private Sub dgv_CellShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs) Handles dataqual_lineage_srcinfo.EditingControlShowing, dataqual_lineage_procstep.EditingControlShowing, dgv_edom.EditingControlShowing ', spdoinfo_ptvctinf_sdtsterm.EditingControlShowing
        Dim dgv As DataGridView = sender
        If Not TypeOf dgv.CurrentCell Is DataGridViewComboBoxCell Then Exit Sub

        Dim cb As ComboBox = e.Control
        If cb Is Nothing Then Return

        cb.DropDownStyle = ComboBoxStyle.DropDown
    End Sub




    'Private Sub dataqual_lineage_srcinfo_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dataqual_lineage_srcinfo.DataError, dataqual_lineage_procstep.DataError
    '    Dim dgv As DataGridView = sender
    '    If Not TypeOf dgv.CurrentCell Is DataGridViewComboBoxCell Then Exit Sub

    '    Dim combo As DataGridViewComboBoxCell = dgv.CurrentCell
    '    If combo Is Nothing Then Return

    '    'check if item is already in drop down, if not, add it to all
    '    If Not combo.Items.Contains(combo.Value) Then
    '        Dim comboColumn As DataGridViewComboBoxColumn = combo.DataGridView.Columns(combo.DataGridView.CurrentCell.ColumnIndex)
    '        combo.Items.Add(combo.Value)
    '        comboColumn.Items.Add(combo.Value)
    '        combo.DataGridView.CurrentCell.Value = combo.Value
    '    End If

    'End Sub

    'Private Sub dataqual_lineage_srcinfo_CellValueChanged(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dataqual_lineage_srcinfo.CellValueChanged, dataqual_lineage_procstep.CellValueChanged
    '    Dim dgv As DataGridView = sender
    '    If Not TypeOf dgv.CurrentCell Is DataGridViewComboBoxCell Then Exit Sub

    '    Dim combo As DataGridViewComboBoxEditingControl = dgv.EditingControl
    '    If combo Is Nothing Then Return

    '    'check if item is already in drop down, if not, add it to all
    '    If Not combo.Items.Contains(combo.Text) Then
    '        Dim comboColumn As DataGridViewComboBoxColumn = combo.EditingControlDataGridView.Columns(combo.EditingControlDataGridView.CurrentCell.ColumnIndex)
    '        combo.Items.Add(combo.Text)
    '        comboColumn.Items.Add(combo.Text)
    '        combo.EditingControlDataGridView.CurrentCell.Value = combo.Text
    '    End If

    'End Sub



    'Private Sub EditorForm_MouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
    '    Try
    '        Dim ctrl As Control = Me.GetChildAtPoint(New Drawing.Point(e.X, e.Y))

    '        While ctrl IsNot Nothing
    '            Me.Text = ctrl.Name
    '            ctrl = ctrl.GetChildAtPoint(New Drawing.Point(e.X, e.Y))
    '        End While

    '    Catch ex As Exception
    '        Dim a = 1
    '    End Try
    'End Sub

    ''' <summary>
    ''' Helper function keeping track of what control has the mouse.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Was needed to diagnose Donnie Williams issue.</remarks>
    Private Sub EditorForm_MouseEnter(sender As System.Object, e As System.EventArgs)
        Try
            Dim ctrl As Control = sender
            Me.Text = ctrl.Name
            Me.Text += " / " + ctrl.Parent.Name
            Me.Text += " / X: " + ctrl.Location.X.ToString + " - Y: " + ctrl.Location.Y.ToString
        Catch ex As Exception
            Dim a = 1
        End Try
    End Sub

    ''' <summary>
    ''' Set up event handlers to help diagnose Donnie Williams issue.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupForDonnie()
        For Each ctrl As Control In Me.allControls.Values
            AddHandler ctrl.MouseEnter, AddressOf EditorForm_MouseEnter
        Next
    End Sub
End Class
