Imports System.Runtime.InteropServices
Imports ESRI.ArcGIS.ADF.BaseClasses
Imports ESRI.ArcGIS.ADF.CATIDs
Imports ESRI.ArcGIS.CatalogUI
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.Catalog


<ComClass(ImportMetadataCommand.ClassId, ImportMetadataCommand.InterfaceId, ImportMetadataCommand.EventsId), _
 ProgId("EPAMetadataEditor.ImportMetadataCommand")> _
Public NotInheritable Class ImportMetadataCommand
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
        MyBase.m_caption = "EME Import Metadata"   'localizable text 
        MyBase.m_message = "Import metadata into selected object(s)"   'localizable text 
        MyBase.m_toolTip = "Import metadata into selected object(s)" 'localizable text 
        MyBase.m_name = "EPAMetadataEditor_ImportMetadataCommand"  'unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")

        Try
            MyBase.m_bitmap = My.Resources.ImportMetadata
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
    ''' Event handler to carry out batch metadata import when button is clicked.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub OnClick()
        Try
            If Not GlobalVars.enabled Then Return

            Dim objects As List(Of IGxObject) = getSelectedObjects(gxApp, metability.CanWriteMetadata)

            If objects.Count = 0 Then
                MsgBox("Your selection does not include any valid objects that can have metadata!", , MyBase.m_caption)
            Else

                Dim o As IGxObject = gxbrowse_for_geodatabase(gxApp.View.hWnd)

                If o Is Nothing Then Exit Sub

                If Not canHaveMetadata(o) Then
                    MsgBox("Selected object is not a metadata source.", , MyBase.m_caption)
                    Exit Sub
                End If

                If o.Category = "XML Document" AndAlso Not isWellFormedXmlFile(o.FullName) Then
                    MsgBox("Selected file does not contain well-formed XML.", , MyBase.m_caption)
                    Exit Sub
                Else
                    If Not hasMetadata(o) Then
                        MsgBox("Selected object does not have metadata.", , MyBase.m_caption)
                        Exit Sub
                    End If
                End If

                Dim srcXml As String = CType(CType(o, IMetadata).Metadata, IXmlPropertySet2).GetXml("")

                If MessageBox.Show("You are about to import metadata into " + objects.Count.ToString + " object(s). Proceed?", MyBase.m_caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                    For Each md As IMetadata In objects
                        Dim iXPSv As IXmlPropertySet2 = CType(md.Metadata, IXmlPropertySet2)
                        iXPSv.SetXml(srcXml)
                        md.Metadata = CType(iXPSv, ESRI.ArcGIS.esriSystem.IPropertySet)
                    Next
                    MessageBox.Show("Metadata has been imported into " + objects.Count.ToString + " object(s).", MyBase.m_caption, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            ErrorHandler(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Prompt user for selecting an ArcCatalog object
    ''' </summary>
    ''' <param name="hParentHwnd">Parent hwnd</param>
    ''' <returns>IGxObject representing the user selected ArcCatalog object. Returns Nothing if no object was selected.</returns>
    ''' <remarks></remarks>
    Public Shared Function gxbrowse_for_geodatabase(ByVal hParentHwnd As Integer) As ESRI.ArcGIS.Catalog.IGxObject
        gxbrowse_for_geodatabase = Nothing

        Dim pGxDialog As ESRI.ArcGIS.CatalogUI.IGxDialog
        Dim pGxObject As ESRI.ArcGIS.Catalog.IGxObject
        Dim pEnumGxObj As ESRI.ArcGIS.Catalog.IEnumGxObject = Nothing

        'Set variables
        pGxDialog = New ESRI.ArcGIS.CatalogUI.GxDialog

        'Set dialog properties
        pGxDialog.Title = "Select the object that you want to import metadata from"
        pGxDialog.ButtonCaption = "Select"

        Try
            If Not pGxDialog.DoModalOpen(hParentHwnd, pEnumGxObj) = True Then
                Return Nothing
            End If
            Try
                pEnumGxObj.Reset()
                pGxObject = pEnumGxObj.Next
            Catch ex As Exception
                ErrorHandler(ex)
                Return Nothing
            End Try
        Catch ex As Exception
            ErrorHandler(ex)
            Return Nothing
        End Try

        Return pGxObject
    End Function


End Class



