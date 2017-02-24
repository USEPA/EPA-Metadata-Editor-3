Imports System.Runtime.InteropServices
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.CatalogUI
Imports ESRI.ArcGIS.Catalog
Imports ESRI.ArcGIS.Geodatabase


<ComClass(ClearAllMetadataCommand.ClassId, ClearAllMetadataCommand.InterfaceId, ClearAllMetadataCommand.EventsId), _
 ProgId("EPAMetadataEditor.ClearAllMetadataCommand")> _
Public NotInheritable Class ClearAllMetadataCommand
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
        MyBase.m_caption = "EME Batch Clear Metadata"   'localizable text 
        MyBase.m_message = "Clear metadata for selected object(s)"   'localizable text 
        MyBase.m_toolTip = "Clear metadata for selected object(s)" 'localizable text 
        MyBase.m_name = "EPAMetadataEditor_ClearMetadataCommand"  'unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")

        Try
            MyBase.m_bitmap = My.Resources.ClearMetadata
        Catch ex As Exception
            System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap")
        End Try


    End Sub

    ''' <summary>
    ''' Event handler to initialize the command when created.
    ''' </summary>
    ''' <param name="hook"></param>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' Event handler to carry out batch metadata clearing when button is clicked.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub OnClick()
        Try
            If Not GlobalVars.enabled Then Return

            Dim objects As List(Of IGxObject) = getSelectedObjects(gxApp, metability.CanWriteMetadata)

            If objects.Count = 0 Then
                MsgBox("Your selection does not include any valid objects that you can write metadata to!", , MyBase.m_caption)
            Else
                If MessageBox.Show("You are about to clear metadata for " + objects.Count.ToString + " object(s). Proceed?", MyBase.m_caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                    For Each md As IMetadata In objects
                        Dim srcXml As String = "<?xml version=""1.0""?>" + vbCrLf + "<metadata></metadata>"
                        Dim iXPSv As IXmlPropertySet2 = CType(md.Metadata, IXmlPropertySet2)
                        iXPSv.SetXml(srcXml)
                        md.Metadata = CType(iXPSv, ESRI.ArcGIS.esriSystem.IPropertySet)
                    Next
                    MessageBox.Show("Metadata for " + objects.Count.ToString + " object(s) have been cleared.", MyBase.m_caption, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            ErrorHandler(ex)
        End Try
    End Sub

End Class



