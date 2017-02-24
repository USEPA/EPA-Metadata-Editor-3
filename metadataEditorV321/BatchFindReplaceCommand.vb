Imports System.Runtime.InteropServices
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.Catalog
Imports ESRI.ArcGIS.CatalogUI
Imports ESRI.ArcGIS.Geodatabase

Imports System.Xml

<ComClass(BatchFindReplaceCommand.ClassId, BatchFindReplaceCommand.InterfaceId, BatchFindReplaceCommand.EventsId), _
 ProgId("EPAMetadataEditor.BatchFindReplaceCommand")> _
Public NotInheritable Class BatchFindReplaceCommand
    Inherits BaseCommand

#Region "COM GUIDs"
    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.

    ''' <summary>
    ''' Class GUID.
    ''' </summary>
    ''' <remarks>
    ''' Be sure to change this so your editor does not conflict with EPA Metadata Editor
    ''' and can run side by side with it.
    ''' </remarks>
    Public Const ClassId As String = "GENERATE-ANDI-NSER-TYOU-ROWNGUIDHERE"

    ''' <summary>
    ''' Interface GUID.
    ''' </summary>
    ''' <remarks>
    ''' Be sure to change this so your editor does not conflict with EPA Metadata Editor
    ''' and can run side by side with it.
    ''' </remarks>
    Public Const InterfaceId As String = "GENERATE-ANDI-NSER-TYOU-ROWNGUIDHERE"

    ''' <summary>
    ''' Events GUID.
    ''' </summary>
    ''' <remarks>
    ''' Be sure to change this so your editor does not conflict with EPA Metadata Editor
    ''' and can run side by side with it.
    ''' </remarks>
    Public Const EventsId As String = "GENERATE-ANDI-NSER-TYOU-ROWNGUIDHERE"
#End Region

#Region "COM Registration Function(s)"
    <ComRegisterFunction(), ComVisibleAttribute(False)> _
    Public Shared Sub RegisterFunction(ByVal registerType As Type)
        ' Required for ArcGIS Component Category Registrar support
        ArcGISCategoryRegistration(registerType)

        'Add any COM registration code after the ArcGISCategoryRegistration() call

    End Sub

    <ComUnregisterFunction(), ComVisibleAttribute(False)> _
    Public Shared Sub UnregisterFunction(ByVal registerType As Type)
        ' Required for ArcGIS Component Category Registrar support
        ArcGISCategoryUnregistration(registerType)

        'Add any COM unregistration code after the ArcGISCategoryUnregistration() call

    End Sub

#Region "ArcGIS Component Category Registrar generated code"
    Private Shared Sub ArcGISCategoryRegistration(ByVal registerType As Type)
        Dim regKey As String = String.Format("HKEY_CLASSES_ROOT\CLSID\{{{0}}}", registerType.GUID)
        GxCommands.Register(regKey)

    End Sub
    Private Shared Sub ArcGISCategoryUnregistration(ByVal registerType As Type)
        Dim regKey As String = String.Format("HKEY_CLASSES_ROOT\CLSID\{{{0}}}", registerType.GUID)
        GxCommands.Unregister(regKey)

    End Sub

#End Region
#End Region

    Private Shared gxApp As IGxApplication

    ' A creatable COM class must have a Public Sub New() 
    ' with no parameters, otherwise, the class will not be 
    ' registered in the COM registry and cannot be created 
    ' via CreateObject.
    Public Sub New()
        MyBase.New()

        ' TODO: Define values for the public properties
        MyBase.m_category = "EPA Metadata Tools"  'localizable text 
        MyBase.m_caption = "EME Batch Find and Replace"   'localizable text 
        MyBase.m_message = "Perform find and replace operation on selected objects' metadata"   'localizable text 
        MyBase.m_toolTip = "Perform find and replace operation on selected objects' metadata" 'localizable text 
        MyBase.m_name = "EPAMetadataEditor_BatchFindReplaceCommand"  'unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")

        MyBase.m_bitmap = My.Resources.FindReplace


    End Sub


    Public Overrides Sub OnCreate(ByVal hook As Object)
        If Not hook Is Nothing Then
            gxApp = CType(hook, IGxApplication)

            'Disable if it is not ArcCatalog
            If TypeOf hook Is IGxApplication Then
                MyBase.m_enabled = True
            Else
                MyBase.m_enabled = False
            End If
        End If

        ' TODO:  Add other initialization code
    End Sub

    Public Overrides Sub OnClick()
        Try
            If Not GlobalVars.enabled Then Return

            Dim objects As List(Of IGxObject) = getSelectedObjects(gxApp, metability.HasWriteableMetadata)

            If objects.Count = 0 Then
                MsgBox("Your selection does not include any valid objects that has metadata you can write to!", , MyBase.m_caption)
            Else
                Dim frm As New BatchFindReplaceForm
                frm.Text = MyBase.m_caption
                Dim dr As DialogResult = (New BatchFindReplaceForm).ShowDialog()
                My.Settings.Save()
                If dr <> DialogResult.OK Then Exit Sub
                If MessageBox.Show("You are about to potentially modify metadata for " + objects.Count.ToString + " object(s). Proceed?", MyBase.m_caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                    Dim modifiedMdCount As Integer = 0
                    Dim totalReplaceCount As Integer = 0
                    For Each md As IMetadata In objects
                        Dim iXPSv As IXmlPropertySet2 = CType(md.Metadata, IXmlPropertySet2)
                        Dim mdXml As String = iXPSv.GetXml("")
                        Dim replaceCount As Integer = xmlFindReplace(mdXml, My.Settings.FindString, My.Settings.ReplaceString)
                        totalReplaceCount += replaceCount
                        If replaceCount > 0 Then modifiedMdCount += 1
                        'MsgBox(mdXml)
                        iXPSv.SetXml(mdXml)
                        md.Metadata = CType(iXPSv, ESRI.ArcGIS.esriSystem.IPropertySet)
                    Next

                    Dim msg As String = "Metadata for " + objects.Count.ToString + " object(s) have been processed." + vbCrLf
                    If totalReplaceCount = 0 Then
                        msg += "No replacements have been made."
                    Else
                        msg += "A total of " + totalReplaceCount.ToString + " replacement(s) have been made in " + modifiedMdCount.ToString + " object(s)"
                    End If
                    MessageBox.Show(msg, MyBase.m_caption, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            ErrorHandler(ex)
        End Try
    End Sub


    Public Shared Function xmlFindReplace(ByRef xmlStr As String, txtFind As String, txtReplace As String) As Integer
        Dim replaceCount As Integer = 0

        Dim doc As New XmlDocument()
        doc.LoadXml(xmlStr)

        If Not My.Settings.caseSensitiveSearch Then txtFind = txtFind.ToLower

        Dim textNodes As XmlNodeList = doc.SelectNodes("//text()")
        For Each node As XmlNode In textNodes
            Dim startIdx As Integer = 0
            If node.Value IsNot Nothing Then
                Dim txt As String = node.Value
                Dim txtCompare As String
                If My.Settings.caseSensitiveSearch Then
                    txtCompare = txt
                Else
                    txtCompare = txt.ToLower
                End If
                While True
                    Dim idx As Integer = txtCompare.IndexOf(txtFind, startIdx)
                    If idx < startIdx Then Exit While 'No new matches
                    Debug.Print("##" + node.Value + "##")
                    Debug.Print(idx)
                    Debug.Print(idx + txtFind.Length)
                    startIdx = idx + txtFind.Length

                    If idx > 0 AndAlso Utils.IsAlphaNumeric(txtCompare(idx - 1)) Then Continue While 'Suffix to a word, don't replace
                    If idx + txtFind.Length < txtCompare.Length AndAlso Utils.IsAlphaNumeric(txtCompare(idx + txtFind.Length)) Then Continue While 'Prefix to a word, don't replace
                    'If we get here, it's OK to replace
                    txt = txt.Substring(0, idx) + txtReplace + txt.Substring(idx + txtFind.Length)
                    If My.Settings.caseSensitiveSearch Then
                        txtCompare = txt
                    Else
                        txtCompare = txt.ToLower
                    End If
                    replaceCount += 1
                    startIdx = idx + txtReplace.Length
                End While
                node.Value = txt
            End If
        Next

        Dim w As New System.IO.StringWriter()

        xmlStr = w.ToString
        cleanseUtf16(xmlStr)
        doc.Save(xmlStr)

        Return replaceCount
    End Function

End Class



