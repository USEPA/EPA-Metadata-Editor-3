Imports System.Runtime.InteropServices
Imports System.Drawing
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.Framework
Imports ESRI.ArcGIS.CatalogUI
Imports ESRI.ArcGIS.Catalog
Imports ESRI.ArcGIS.Geodatabase


<ComClass(SyncCommand.ClassId, SyncCommand.InterfaceId, SyncCommand.EventsId), _
 ProgId("EPAMetadataEditor.SyncCommand")> _
Public NotInheritable Class SyncCommand
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

    ''' <summary>
    ''' Enum representing the various syncronization modes that EPA syncer can be in.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum syncMode
        none                        ' Not syncing
        mainSyncer                  ' Syncing: coordination
        helperSyncerScoutingRun     ' Syncing: look ahead to collect items being synced
        helperSyncerExecutionRun    ' Syncing: perform actual sync
    End Enum

    ''' <summary>
    ''' Current synchronization mode
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared currentSyncLevel As syncMode = syncMode.none

    ''' <summary>
    ''' Shared variable to tell syncer which user selected object syncing was initiated for
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared imd As IMetadata

    ''' <summary>
    ''' Reference to ArcCatalog instance
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared gxApp As IGxApplication

    ' A creatable COM class must have a Public Sub New() 
    ' with no parameters, otherwise, the class will not be 
    ' registered in the COM registry and cannot be created 
    ' via CreateObject.
    Public Sub New()
        MyBase.New()

        ' TODO: Define values for the public properties
        MyBase.m_category = "EPA Metadata Tools"  'localizable text 
        MyBase.m_caption = "EME Synchronize Metadata"   'localizable text 
        MyBase.m_message = "Synchronize metadata for selected objects"   'localizable text 
        MyBase.m_toolTip = "Synchronize metadata for selected objects" 'localizable text 
        MyBase.m_name = "EPAMetadataEditor_SyncCommand"  'unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")


        Try
            MyBase.m_bitmap = My.Resources.SyncNowBitmap
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
            gxApp = CType(hook, IApplication)

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
    ''' Event handler to carry out synchronization when button is clicked.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub OnClick()
        Try
            If Not GlobalVars.enabled Then Return

            Dim synced As Integer = 0

            Dim objects As List(Of IGxObject) = getSelectedObjects(gxApp, metability.CanWriteMetadata)

            If objects.Count = 0 Then
                MsgBox("Your selection does not include any valid objects that you can synchronize metadata for!", , MyBase.m_caption)
            Else
                If MessageBox.Show("You are about to synchronize metadata for " + objects.Count.ToString + " object(s). This may overwrite existing information. Proceed?", MyBase.m_caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                    For Each md As IMetadata In objects
                        Try
                            Try
                                currentSyncLevel = syncMode.mainSyncer
                                imd = md
                                imd.Synchronize(esriMetadataSyncAction.esriMSAOverwrite, 11)
                                imd = Nothing
                                synced += 1
                            Finally
                                currentSyncLevel = syncMode.none
                            End Try
                        Catch ex As Exception
                            Dim aa = 1
                        Finally
                            currentSyncLevel = syncMode.none
                        End Try
                    Next

                    MessageBox.Show("Synchronized metadata for " + Str(synced) + " of " + objects.Count.ToString + " objects selected.", MyBase.m_caption, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If

        Catch ex As Exception
            ErrorHandler(ex)
        End Try
    End Sub


End Class



