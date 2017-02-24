Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.CatalogUI
Imports System.Runtime.InteropServices


''' <summary>
''' MasterController implements the IMetadataEditor interface and acts as the glue between ArcCatalog and EME.
''' </summary>
''' <remarks></remarks>
<ComClass(MasterController.ClassId, MasterController.InterfaceId, MasterController.EventsId), _
 ProgId("EPAMetadataEditor.MasterController")> _
Public Class MasterController
    Implements IMetadataEditor


#Region "COM Registration Function(s)"
    ''' <summary>
    ''' Register EME with COM
    ''' </summary>
    ''' <param name="registerType"></param>
    ''' <remarks></remarks>
    <ComRegisterFunction(), ComVisibleAttribute(False)> _
    Public Shared Sub RegisterFunction(ByVal registerType As Type)
        ' Required for ArcGIS Component Category Registrar support
        ArcGISCategoryRegistration(registerType)

        'Add any COM registration code after the ArcGISCategoryRegistration() call

    End Sub

    ''' <summary>
    ''' Unregister EME with COM
    ''' </summary>
    ''' <param name="registerType"></param>
    ''' <remarks></remarks>
    <ComUnregisterFunction(), ComVisibleAttribute(False)> _
    Public Shared Sub UnregisterFunction(ByVal registerType As Type)
        ' Required for ArcGIS Component Category Registrar support
        ArcGISCategoryUnregistration(registerType)

        'Add any COM unregistration code after the ArcGISCategoryUnregistration() call

    End Sub

#Region "ArcGIS Component Category Registrar generated code"
    ''' <summary>
    ''' Required method for ArcGIS Component Category registration -
    ''' Do not modify the contents of this method with the code editor.
    ''' </summary>
    Private Shared Sub ArcGISCategoryRegistration(ByVal registerType As Type)
        Dim regKey As String = String.Format("HKEY_CLASSES_ROOT\CLSID\{{{0}}}", registerType.GUID)
        GxCommands.Register(regKey)
        GxExtensions.Register(regKey)
        MetadataEditor.Register(regKey)

    End Sub
    ''' <summary>
    ''' Required method for ArcGIS Component Category unregistration -
    ''' Do not modify the contents of this method with the code editor.
    ''' </summary>
    Private Shared Sub ArcGISCategoryUnregistration(ByVal registerType As Type)
        Dim regKey As String = String.Format("HKEY_CLASSES_ROOT\CLSID\{{{0}}}", registerType.GUID)
        GxCommands.Unregister(regKey)
        GxExtensions.Unregister(regKey)
        MetadataEditor.Unregister(regKey)

    End Sub

#End Region
#End Region

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


    ''' <summary>
    ''' Main entry point into metadata editor when initiated by ArcCatalog
    ''' </summary>
    ''' <param name="metadata">Metadata record copy to be edited.</param>
    ''' <param name="hWnd">Handle to the metadata editor window.</param>
    ''' <returns>
    ''' Boolean return value indicates if the metadata record was modified by the editor
    ''' </returns>
    ''' <remarks>With the advent of standalone editor, this is now a wrapper around it.</remarks>
    Public Function Edit(ByVal metadata As IPropertySet, ByVal hWnd As Integer) As Boolean Implements IMetadataEditor.Edit
        Try
            ' We immediately convert the metadata record from IPropertySet to XmlMetadata 
            ' since the latter has a richer interface to manipulate the record. 
            iXPS = New XmlMetadata
            Dim AoMdHolder As IXmlPropertySet2 = CType(metadata, IXmlPropertySet2)
            iXPS.SetXml(AoMdHolder.GetXml(""))

            Dim editorPath As String
            If Utils.iAmInstalled Then
                ' app installed 
                editorPath = Utils.getAppFolder + "\MetadataEditor.exe"
            Else
                ' app in dev env - AE: Change this if projects are reorganized...
                editorPath = Utils.getAppFolder.Replace("metadataeditorv32", "metadataeditorv32\MetadataEditor") + "\MetadataEditor.exe"
            End If

            Dim editorProc As New Process
            editorProc.StartInfo.FileName = editorPath
            Dim tmpMdFile As String = Utils.textToTempFile(iXPS.GetXml(""))
            Dim oldMd5 As String = Utils.stringMd5(iXPS.GetXml(""))
            editorProc.StartInfo.Arguments = """" + tmpMdFile + """"

            editorProc.Start()

            ' We loop around and check until the editor exits instead of doing a waitForExit to avoid background painting problems encountered on some machines.
            Do
                ' Sleep to avoid a busy spin
                Threading.Thread.Sleep(100)
                ' Give events a chance (including paint events)
                Application.DoEvents()
            Loop While Not editorProc.HasExited

            'editorProc.WaitForExit()
            editorProc.Close()

            iXPS.SetXml(IO.File.ReadAllText(tmpMdFile))
            Dim newMd5 As String = Utils.stringMd5(iXPS.GetXml(""))
            ' Save if md5 hash changed after editing
            Dim saveHint As Boolean = (oldMd5 <> newMd5)

            AoMdHolder.SetXml(iXPS.GetXml(""))
            ' Explicitly releasing the metadata object.
            AoMdHolder = Nothing

            ' Explicitly save settings as we are a DLL - not EXE.
            My.Settings.Save()

            ' When the form is closed, we return a boolean value indicating
            ' whether or not the metadata was modified. That information is
            ' passed back to the ArcCatalog -- if edited, the metadata at the source 
            ' will be updated and metadata view is refreshed so you can see the changes.
            Return saveHint
        Catch ex As Exception
            ' No well-defined mechanism in place for reporting errors.
            Utils.ErrorHandler(ex)
        End Try
    End Function

    ''' <summary>
    ''' Sets the name for this editor - this name will appear in the
    ''' list of metadata editors in ArcCatalog's Options dialog box
    ''' </summary>
    ''' <returns>String containing the name of this metadata editor.</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Name1() As String Implements IMetadataEditor.Name
        Get
            Return "EPA Metadata Editor Build: " + Utils.GetVersion()
        End Get
    End Property

End Class


