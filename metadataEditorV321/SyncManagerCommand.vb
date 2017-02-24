Imports System.Runtime.InteropServices
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.CatalogUI


''' <summary>
''' Class that implements a user interface for controlling (turning on/off) synchronizers
''' and also controlling the EPA Synchronizer options.
''' </summary>
''' <remarks></remarks>
<ComClass(SyncManagerCommand.ClassId, SyncManagerCommand.InterfaceId, SyncManagerCommand.EventsId), _
 ProgId("EPAMetadataEditor.SyncManagerCommand")> _
Public NotInheritable Class SyncManagerCommand
    Inherits BaseCommand

    ''' <summary>
    ''' Reference to ArcCatalog instance
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared gxApp As IGxApplication

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

    ''' <summary>
    ''' Create and initialize.
    ''' </summary>
    ''' <remarks>A creatable COM class must have a Public Sub New() 
    ''' with no parameters, otherwise, the class will not be 
    ''' registered in the COM registry and cannot be created 
    ''' via CreateObject.
    ''' </remarks>
    Public Sub New()
        MyBase.New()

        MyBase.m_category = "EPA Metadata Tools"  'localizable text 
        MyBase.m_caption = "EME Synchronizer Manager"   'localizable text 
        MyBase.m_message = "Manage metadata synchronizers including EPA Synchronizer"   'localizable text 
        MyBase.m_toolTip = "Manage metadata synchronizers including EPA Synchronizer" 'localizable text 
        MyBase.m_name = "EPAMetadataEditor_SynchronizerManagerCommand"  'unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")

        Try
            MyBase.m_bitmap = My.Resources.SyncManagerBitmap
        Catch ex As Exception
            System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap")
            ErrorHandler(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Event handler that gets called when an instance is created. Used to get a hold of ArcCatalog.
    ''' </summary>
    ''' <param name="hook"></param>
    ''' <remarks></remarks>
    Public Overrides Sub OnCreate(ByVal hook As Object)
        If Not hook Is Nothing Then
            'Disable if it is not ArcCatalog
            If TypeOf hook Is IGxApplication Then
                gxApp = hook
                MyBase.m_enabled = True
            Else
                MyBase.m_enabled = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Event handler that opens the form for managing syncronizers.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub OnClick()
        Try
            Dim frm As New SyncManagerForm
            frm.ShowDialog()
        Catch ex As Exception
            ErrorHandler(ex)
        End Try

        ' Explicitly, save settings as we are a DLL - not EXE.
        My.Settings.Save()
    End Sub


End Class


