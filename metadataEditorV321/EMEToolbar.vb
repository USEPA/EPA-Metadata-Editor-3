Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.ADF.BaseClasses
'Imports ESRI.ArcGIS.ArcCatalogUI
Imports System.Runtime.InteropServices

''' <summary>
''' An ArcCatalog toolbar for EME, EPA Synchronizer and related tools.
''' </summary>
''' <remarks></remarks>
<ComClass(EMEToolbar.ClassId, EMEToolbar.InterfaceId, EMEToolbar.EventsId), _
 ProgId("EPAMetadataEditor.EMEToolbar")> _
Public NotInheritable Class EMEToolbar
    Inherits BaseToolbar

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
    ''' <summary>
    ''' Required method for ArcGIS Component Category registration -
    ''' Do not modify the contents of this method with the code editor.
    ''' </summary>
    Private Shared Sub ArcGISCategoryRegistration(ByVal registerType As Type)
        Dim regKey As String = String.Format("HKEY_CLASSES_ROOT\CLSID\{{{0}}}", registerType.GUID)
        GxCommandBars.Register(regKey)

    End Sub
    ''' <summary>
    ''' Required method for ArcGIS Component Category unregistration -
    ''' Do not modify the contents of this method with the code editor.
    ''' </summary>
    Private Shared Sub ArcGISCategoryUnregistration(ByVal registerType As Type)
        Dim regKey As String = String.Format("HKEY_CLASSES_ROOT\CLSID\{{{0}}}", registerType.GUID)
        GxCommandBars.Unregister(regKey)

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
    ''' 
    ''' </summary>
    ''' <remarks>A creatable COM class must have a Public Sub New() 
    ''' with no parameters, otherwise, the class will not be 
    ''' registered in the COM registry and cannot be created 
    ''' via CreateObject.
    ''' </remarks>
    Public Sub New()
        ' We include some standard items from ArcCatalog
        ' Some are defunct in v10
        'AddItem("esriCatalogUI.EditCommand")
        'AddItem("esriCatalogUI.SyncCommand")
        'AddItem("esriCatalogUI.ImportCommand")
        ' Some don't seem all that useful
        'AddItem("esriCatalogUI.PropsCommand")
        'AddItem("esriCatalogUI.ValidateTool")
        'AddItem("esriCatalogUI.ExportCommand")
        'BeginGroup() 'Separator
        ' We include some commands of our own
        AddItem("EPAMetadataEditor.EditCommand")
        AddItem("EPAMetadataEditor.BatchValidatorCommand")
        AddItem("EPAMetadataEditor.SyncManagerCommand")
        AddItem("EPAMetadataEditor.SyncCommand")
        AddItem("EPAMetadataEditor.ClearAllMetadataCommand")
        AddItem("EPAMetadataEditor.ImportMetadataCommand")
        AddItem("EPAMetadataEditor.ExportMetadataCommand")
        AddItem("EPAMetadataEditor.BatchFindReplaceCommand")
        AddItem("EPAMetadataEditor.HelpCommand")

        ' See this list for other metadata related ArcCatalog commands you could add to the toolbar:
        ' http://edndoc.esri.com/arcobjects/9.2/CPP_VB6_VBA_VCPP_Doc/shared/desktop/reference/ArcCatalogIds.htm
        ' v10 version is at:
        ' http://help.arcgis.com/en/sdk/10.0/arcobjects_net/conceptualhelp/index.html#//000100000020000000
        'Toolbar Metadata Gx_MetadataTools none {64BF3C81-E501-11D1-AEE5-080009EC734B}
        'esriCatalogUI.GxDocumentationViewTools none none none 
        'Command Select stylesheet Metadata_Stylesheet Metadata {B8D50601-EC46-11D2-9FA9-00C04F8ED211}
        'esriCatalogUI.StylesheetControl none Gx_MetadataTools Choose the stylesheet you want to use to view metadata 
        'Command Edit metadata Metadata_Edit Metadata {D728B851-F1F4-11D2-9FBD-00C04F8ED211}
        'esriCatalogUI.EditCommand none Gx_MetadataTools Edit metadata for the selected item 
        'Command Metadata properties Metadata_Props Metadata {075EB5EB-2B22-11D3-A646-0008C7D3AE50}
        'esriCatalogUI.PropsCommand none Gx_MetadataTools Displays the metadata properties of the selected item 
        'Command Create/Update metadata Metadata_Create_Update Metadata {EB7A30D3-1237-11D3-A62B-0008C7D3AE50}
        'esriCatalogUI.SyncCommand none Gx_MetadataTools Automatically create or update metadata for the selected item 
        'Command Import metadata Metadata_Import Metadata {C3A6071B-6643-11D3-A68E-0008C7D3AE50}
        'esriCatalogUI.ImportCommand none Gx_MetadataTools Import metadata of the selected item 
        'Command Export metadata Metadata_Export Metadata {81186A0C-6639-11D3-A68E-0008C7D3AE50}
        'esriCatalogUI.ExportCommand none Gx_MetadataTools Export metadata of the selected item 
    End Sub

    ''' <summary>
    ''' Caption for the toolbar.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property Caption() As String
        Get
            Return "EPA Metadata Toolbar"
        End Get
    End Property

    ''' <summary>
    ''' Name of the toolbar.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property Name() As String
        Get
            Return "EMEToolbar"
        End Get
    End Property
End Class


