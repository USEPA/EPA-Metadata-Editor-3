Imports System.Runtime.InteropServices
Imports System.Drawing
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.CatalogUI
Imports ESRI.ArcGIS.Catalog
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem


<ComClass(EditCommand.ClassId, EditCommand.InterfaceId, EditCommand.EventsId), _
 ProgId("EPAMetadataEditor.EditCommand")> _
Public NotInheritable Class EditCommand
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

    'Private m_application As IApplication
    Private Shared gxApp As IGxApplication

    ' A creatable COM class must have a Public Sub New() 
    ' with no parameters, otherwise, the class will not be 
    ' registered in the COM registry and cannot be created 
    ' via CreateObject.
    Public Sub New()
        MyBase.New()

        ' TODO: Define values for the public properties
        MyBase.m_category = "EPA Metadata Tools"  'localizable text 
        MyBase.m_caption = "EME Edit FGDC Metadata"   'localizable text 
        MyBase.m_message = "Edit FGDC metadata"   'localizable text 
        MyBase.m_toolTip = "Edit FGDC metadata" 'localizable text 
        MyBase.m_name = "EPAMetadataEditor_EditCommand"  'unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")

        Try
            MyBase.m_bitmap = New Bitmap(My.Resources.MetadataEdit16)
        Catch ex As Exception
            System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap")
            ErrorHandler(ex)
        End Try


    End Sub

    ''' <summary>
    ''' Event handler to initialize the command when created.
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

        ' TODO:  Add other initialization code
    End Sub

    ''' <summary>
    ''' Indicate whether the command (and its button) should be enabled.
    ''' </summary>
    ''' <value></value>
    ''' <returns>True to enable. False otherwise.</returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property Enabled() As Boolean
        Get
            Return gxApp IsNot Nothing AndAlso gxApp.SelectedObject IsNot Nothing 'AndAlso selectionHasMetadata()
        End Get
    End Property

    ''' <summary>
    ''' Event handler to start editor when button is clicked.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub OnClick()
        Try
            If Not GlobalVars.enabled Then Return

            Dim objects As List(Of IGxObject) = getSelectedObjects(gxApp, metability.CanHaveMetadata)
            If objects.Count = 0 Then
                MsgBox("Your selection does not include any valid objects that can have metadata!", , MyBase.m_caption)
            ElseIf objects.Count > 1 Then
                MsgBox("You can edit only one metadata record at a time!", , MyBase.m_caption)
            Else
                Dim md As IMetadata = objects(0)
                Dim ips As IPropertySet = md.Metadata
                Dim mc As New MasterController()
                If mc.Edit(ips, 0) Then
                    'save
                    md.Metadata = ips
                End If
            End If
        Catch ex As Exception
            ErrorHandler(ex)
        End Try

    End Sub
End Class



