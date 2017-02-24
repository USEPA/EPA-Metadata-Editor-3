Imports System.Reflection
Imports System.xml
Imports System.xml.Serialization
'Imports System.xml.XPath
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports Microsoft.Win32



''' <summary>
''' Miscellenaous utility functions utilized by EME.
''' </summary>
''' <remarks></remarks>
Module Utils

    ''' <summary>
    ''' Returns the folder path where EME is installed in.
    ''' </summary>
    ''' <returns></returns> 
    ''' <remarks></remarks>
    Public Function getAppFolder() As String
        Return Path.GetDirectoryName([Assembly].GetCallingAssembly().Location)
    End Function

    ''' <summary>
    ''' Returns the folder path where the template files are stored in.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Template files are copied to application data folder at start up if necessary.</remarks>
    Public Function getTemplateFolder() As String
        Return getAppFolder() + "\template"
    End Function

    ''' <summary>
    ''' Returns the folder path where application data is stored.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getAppDataFolder() As String
        Return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\" + My.Application.Info.CompanyName + "\" + My.Application.Info.ProductName
    End Function

    ''' <summary>
    ''' Returns the folder path where files for EPA validation are stored.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getEpaFolder() As String
        Return getAppDataFolder() + "\epa_interface\"
    End Function

    ''' <summary>
    ''' Returns the folder path where files for EPA validation are stored.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMpFolder() As String
        Return getAppDataFolder() + "\mp_interface\"
    End Function

    ''' <summary>
    ''' Returns the folder path where temporary files are stored in.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getWorkingFolder() As String
        Return getAppDataFolder() + "\tmp"
    End Function

    ''' <summary>
    ''' Returns the folder path where EPA stylesheet files are stored in at install time.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getEpaStylesheetsSourceFolder() As String
        Return getAppFolder() + "\EPA_Stylesheets"
    End Function

    ''' <summary>
    ''' Returns the folder path where Arcgis metadata stylesheet files are stored in.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getArcgisMetadataFolder() As String
        Return getArcgisInstallDir() + "\Metadata"
    End Function

    ''' <summary>
    ''' Returns the application version when called by the main assembly.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVersion() As String
        Return Assembly.GetExecutingAssembly.GetName.Version.ToString()
    End Function

    ''' <summary>
    ''' Returns  <paramref name="origStr"/> with non-alphanumeric characters stripped out.
    ''' Each stripped out character is replaced by the contents of <paramref name="replaceWith"/> if specified.
    ''' </summary>
    Function stripNonAlphanumeric(ByRef origStr As String, Optional ByVal replaceWith As String = "") As String
        ' If no string, return as is.
        If origStr Is Nothing Then
            Return origStr
        End If

        Dim c As Char
        Dim r As String = ""
        For lCtr As Int16 = 1 To origStr.Length
            c = Mid(origStr, lCtr, 1)
            If c Like "[0-9A-Za-z]" Then
                r = r & c
            ElseIf replaceWith <> "" Then
                r = r & replaceWith
            End If
        Next
        Return r
    End Function

    ''' <summary>
    ''' Prints debugging information to a dialog window based on exception information 
    ''' passed in <paramref name="ex"/>.
    ''' </summary>
    Public Sub ErrorHandler(ByVal ex As Exception)
        'Debug.Print(whereAmI)
        Debug.Print(ex.Message)
        Debug.Print(ex.TargetSite.ToString())
        Debug.Print(ex.StackTrace)
        'MessageBox.Show(ex.ToString() & vbCrLf & ex.StackTrace(), ex.Message())
    End Sub

    ''' <summary>
    ''' Returns the path to the application associated with the specified file extension.
    ''' Returns empty string, if none found in Windows registry.
    ''' </summary>
    Public Function GetAssociatedProgram(ByVal FileExtension As String) As String
        ' Returns the application associated with the specified FileExtension
        Dim objExtReg As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot
        Dim objAppReg As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot
        Dim strExtValue As String
        Try
            ' Add leading period if doesn't exist
            If FileExtension.Substring(0, 1) <> "." Then _
                FileExtension = "." & FileExtension
            ' Open registry areas containing launching app details
            objExtReg = objExtReg.OpenSubKey(FileExtension.Trim)
            strExtValue = objExtReg.GetValue("")
            objAppReg = objAppReg.OpenSubKey(strExtValue & "\shell\open\command")
            ' Parse out, tidy up and return result
            Dim SplitArray() As String
            SplitArray = Split(objAppReg.GetValue(Nothing), """")
            If SplitArray(0).Trim.Length > 0 Then
                Return SplitArray(0).Replace("%1", "")
            Else
                Return SplitArray(1).Replace("%1", "")
            End If
        Catch
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Attemps to open the specified URL using Internet Explorer.
    ''' </summary>
    Public Sub OpenInIE(ByVal url As String)
        Try
            Dim proc As New Process()
            proc.StartInfo.FileName = "iexplore.exe"
            proc.StartInfo.Arguments = """" & url & """"
            proc.Start()
        Finally
        End Try
    End Sub


    ''' <summary>
    ''' Attemps to open the specified URL using default file handler.
    ''' </summary>
    Public Sub OpenWitDefaultFileHandler(ByVal filename As String)
        Try
            System.Diagnostics.Process.Start("file:///" & filename.Replace("\", "/"))
        Finally
        End Try
    End Sub

    ''' <summary>
    ''' Runs MSAccess application. 
    ''' If the name of a macro is specified, the macro is run by MSAccess upon startup.
    ''' </summary>
    Public Sub openMSAccess(Optional ByVal macroToRun As String = "")
        Dim proc As New Process()
        proc.StartInfo.FileName = Utils.GetAssociatedProgram(".mdb")
        proc.StartInfo.Arguments = """" & GlobalVars.mdbPath & """"
        If macroToRun = "" Then
            ' We run a dummy macro that opens and closes a table to prevent
            ' MSAccess from crashing. Weird!
            macroToRun = "noop"
        End If
        proc.StartInfo.Arguments += " /X " & macroToRun
        proc.Start()
    End Sub

    ''' <summary>
    ''' Returns a data reader object for the provided SQL string.
    ''' The caller is responsible for proper cleanup after reader is no longer needed.
    ''' </summary>
    Public Function readerForSQL(ByVal SQLStr As String, Optional ByRef con As OleDb.OleDbConnection = Nothing) As OleDb.OleDbDataReader
        Try
            'Debug.Print(SQLStr)
            ' The Visual Basic 2005 'Using' keyword simplifies the use of Dispose methods by
            '  automatically calling dispose for an object.
            ' The keyword is used here to create a SqlConnection object.
            Dim theSqlConnection As New OleDb.OleDbConnection(connStr)
            con = theSqlConnection

            ' Open theSqlConnection.
            theSqlConnection.Open()

            ' Declare variable named theSqlCommand of type SqlCommand
            '  calling theSqlConnection object's CreateCommand method
            '  and assigning the resulting SqlCommand object to theSqlCommand variable.
            Dim theSqlCommand As OleDb.OleDbCommand = theSqlConnection.CreateCommand()
            ' Assign theQueryString to theSqlCommand's CommandText property.
            theSqlCommand.CommandText = SQLStr
            theSqlCommand.Connection = theSqlConnection

            Return theSqlCommand.ExecuteReader()

        Catch ex As Exception
            Utils.ErrorHandler(ex)
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' COnstruct a DataTable object using the provided SQL query text.
    ''' </summary>
    ''' <param name="SQLStr">SQL query text</param>
    ''' <returns>A DataTable object obtained by executing the provided SQL against source database.</returns>
    ''' <remarks>The resulting record structure needs to contain an "OrderedId" field with unique values for correct operation.</remarks>
    Public Function datatableFromSQL(ByVal SQLStr As String) As DataTable
        'Debug.Print(SQLStr)
        Try
            Dim da As New OleDb.OleDbDataAdapter(SQLStr, connStr)
            Dim dt As New DataTable
            da.Fill(dt)
            dt.PrimaryKey = New DataColumn() {dt.Columns("OrderedID")}

            Return dt
        Catch ex As Exception
            Utils.ErrorHandler(ex)
        End Try
        Return Nothing
    End Function


    ''' <summary>
    ''' Function to return the provided object's value if not null or nothing. Otherwise return the provided default value.
    ''' </summary>
    ''' <param name="o">Object</param>
    ''' <param name="defaultValue">Default value if the object has no value</param>
    ''' <returns>return the provided object's value if not null or nothing. Otherwise return the provided default value.</returns>
    ''' <remarks></remarks>
    Public Function nv(ByVal o As Object, Optional ByVal defaultValue As Object = "") As Object
        If IsDBNull(o) OrElse IsNothing(o) Then
            Return defaultValue
        Else
            Return o
        End If
    End Function

    ''' <summary>
    ''' Compares two values to determine if they are equal. Also compares DBNULL.Value.
    ''' </summary>
    ''' <param name="A">First value to compare</param>
    ''' <param name="B">Second value to compare</param>
    ''' <returns>Returns true if two values are equal or both are null. Otherwise, returns false.</returns>
    ''' <remarks>Based on http://support.microsoft.com/default.aspx?scid=kb;EN-US;325684 </remarks>
    Private Function ColumnEqual(ByVal A As Object, ByVal B As Object) As Boolean
        If A Is DBNull.Value And B Is DBNull.Value Then Return True ' Both are DBNull.Value.
        If A Is DBNull.Value Or B Is DBNull.Value Then Return False ' Only one is DBNull.Value.
        Return A = B                                                ' Value type standard comparison
    End Function

    ''' <summary>
    ''' Construct a copy of a DataView by applying a filter and removing duplicate rows. 
    ''' </summary>
    ''' <param name="TableName">Name of DataTable object to be created</param>
    ''' <param name="SourceView">DataView object that has the rows to be filtered</param>
    ''' <param name="FieldName">Name of field in SourceView that needs to be distinct</param>
    ''' <param name="filter">Filter string to apply</param>
    ''' <returns>A DataView object suitable for use as data source to a combo box. The rows in SourceView are filtered and copied to a
    ''' a new DataTable only if the distinct field value has not been seen before.</returns>
    ''' <remarks>Based on http://support.microsoft.com/default.aspx?scid=kb;EN-US;325684 </remarks>
    Public Function SelectDistinct(ByVal TableName As String, _
                               ByVal SourceView As DataView, _
                               ByVal FieldName As String, _
                               ByVal filter As String) As DataView
        Dim dt As New DataTable(TableName)
        dt.Columns.Add("clusterInfo", SourceView.Table.Columns("clusterInfo").DataType)
        dt.Columns.Add(FieldName, SourceView.Table.Columns(FieldName).DataType)
        dt.Columns.Add("default", SourceView.Table.Columns("default").DataType)
        Dim LastValue As Object = Nothing
        For Each dr As DataRow In SourceView.Table.Select(filter, FieldName)
            If LastValue Is Nothing OrElse Not ColumnEqual(LastValue, dr(FieldName)) Then
                LastValue = dr(FieldName)
                dt.Rows.Add(New Object() {LastValue, LastValue, dr("default")})
            End If
        Next
        Return dt.DefaultView
    End Function


    ''' <summary>
    ''' Write out the provided text to to a new temporary file.
    ''' </summary>
    ''' <param name="txt">Text to write to a file</param>
    ''' <returns>Full pathname of the temporary file that was created.</returns>
    ''' <remarks>File is created with .xml extension and the provided content must be in XML format.</remarks>
    Public Function textToTempFile(ByVal txt As String, Optional ByVal fileExt As String = ".xml") As String
        Dim filename As String = IO.Path.GetTempFileName() & fileExt
        My.Computer.FileSystem.WriteAllText(filename, txt, False)
        Return filename
    End Function

    ''' <summary>
    ''' Decorate XML snippet returned from the validation service turning it into a proper XML file
    ''' and inserting an XSL stylesheet.
    ''' </summary>
    ''' <param name="xmlStr">XML snippet</param>
    ''' <returns>Valid XML text</returns>
    ''' <remarks>The right way to do this is to do DOM manipulation but this works.</remarks>
    Public Function decorateXSL(ByVal xmlStr As String, Optional ByVal addStylesheet As Boolean = True) As String
        If xmlStr.IndexOf("<errs />") >= 0 Then
            Return _
                "<?xml version=""1.0""?>" & vbCrLf & _
                IIf(addStylesheet, "<?xml-stylesheet type=""text/xsl"" href=""" & Utils.getAppFolder() & "\eme.xsl" & """?>" & vbCrLf, "") & _
                "<errs></errs>"
        Else
            Return _
                "<?xml version=""1.0""?>" & vbCrLf & _
                IIf(addStylesheet, "<?xml-stylesheet type=""text/xsl"" href=""" & Utils.getAppFolder() & "\eme.xsl" & """?>" & vbCrLf, "") & _
                xmlStr.Substring(xmlStr.IndexOf("<errs>"))
        End If
    End Function

    ''' <summary>
    ''' Utility function to merge EPA schematron validation results with MP validation results.
    ''' </summary>
    ''' <param name="epaResults">EPA schematron validation results as XML snippet</param>
    ''' <param name="mpResults">MP validation results as XML snippet</param>
    ''' <returns></returns>
    ''' <remarks>The right way to do this is to do DOM manipulation but this works.</remarks>
    Public Function mergeEPAandMP(ByVal epaResults As String, ByVal mpResults As String) As String
        If Not epaResults.Contains("<errs") Then
            epaResults = "<errs>" + epaResults + "</errs>"
        End If

        If Not mpResults.Contains("<errs") Then
            mpResults = "<errs>" + mpResults + "</errs>"
        End If

        If epaResults.IndexOf("<errs />") >= 0 Then
            Return "<?xml version=""1.0"" ?>" + mpResults
        ElseIf mpResults.IndexOf("<errs />") >= 0 Then
            Return epaResults
        Else
            Return _
                epaResults.Substring(0, epaResults.IndexOf("</errs>")) & _
                mpResults.Substring(mpResults.IndexOf("<errs>") + 6)
        End If
    End Function

    ''' <summary>
    ''' Get the fully qualified filename for the file that contains a previously saved session.
    ''' </summary>
    ''' <returns>String containing the path.</returns>
    ''' <remarks></remarks>
    Public Function getLastSessionXMLPath() As String
        Dim path As String = getAppDataFolder()     ' Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\EPA Metadata Editor\"
        If Not IO.Directory.Exists(path) Then
            IO.Directory.CreateDirectory(path)
        End If
        Return path & "last_session.xml"
    End Function

    ''' <summary>
    ''' Check if a previously saved session exists.
    ''' </summary>
    ''' <returns>True if a previously saved session is found on the filesystem. False otherwise.</returns>
    ''' <remarks>The saved session must be from a previous EME session, not the one currently active.</remarks>
    Public Function checkForPreviouslySavedSession() As Boolean
        Return Not GlobalVars.savedSession AndAlso Not GlobalVars.recoveredSession AndAlso File.Exists(getLastSessionXMLPath)
    End Function

    ''' <summary>
    ''' Prompt the user for recovering a previously saved session.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub promptAndRecoverSavedSession()
        If checkForPreviouslySavedSession() Then
            If MsgBox( _
                "EME found a saved metadata file from your last session. Would you like to load it?" & vbCrLf & _
                "Warning: If you select 'Yes', metadata from saved session will overwrite the metadata of the object selected in ArcCatalog!", _
                MsgBoxStyle.YesNo, "Recover saved session") = MsgBoxResult.Yes Then

                iXPS.SetXml(My.Computer.FileSystem.ReadAllText(Utils.getLastSessionXMLPath))
                GlobalVars.recoveredSession = True
                ' Note: We don't delete the recovered session until the user saves out of this one.
            Else
                If MsgBox("Do you want to delete the saved metadata file from last session?" & vbCrLf & _
                    "If you select 'Yes', the file will be deleted permanently and you will not be prompted again.", _
                    MsgBoxStyle.YesNo, "Recover saved session") = MsgBoxResult.Yes Then

                    deleteSavedSession()
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Delete the file containing a previously saved session.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub deleteSavedSession()
        File.Delete(getLastSessionXMLPath)
    End Sub

    ''' <summary>
    ''' Get the value of the first field of an SQL expression that returns a single record.
    ''' </summary>
    ''' <param name="SQLStr">SQL expression to evaluate</param>
    ''' <returns>Field value as Object</returns>
    ''' <remarks></remarks>
    Public Function getFromSingleValuedSQL(ByVal SQLStr As String) As Object
        Dim con As OleDb.OleDbConnection = Nothing
        Try
            Dim dr As OleDb.OleDbDataReader = Nothing
            Try
                dr = readerForSQL(SQLStr, con)
                If dr.Read() Then
                    Return dr(0)
                End If
            Catch
                ' Error while reading from single-valued SQL
            Finally
                If dr IsNot Nothing Then dr.Close()
            End Try
        Catch
            ' We don't get here. We just want the "finally" part.
        Finally
            If con IsNot Nothing Then con.Close()
        End Try
            Return Nothing
    End Function

    ''' <summary>
    ''' Get the parent tag of a given XSL pattern.
    ''' </summary>
    ''' <param name="name">XSL pattern whose parent is sought</param>
    ''' <returns></returns>
    ''' <remarks>Warning: This will not work with some names that have qualifiers involving a slash character.</remarks>
    Public Function getTagParent(ByVal name As String)
        Dim idx As Integer = name.LastIndexOf("/")
        If idx >= 0 Then
            Return name.Substring(0, idx)
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' Apply an XSL transform to an XML file.
    ''' </summary>
    ''' <param name="xslfile">Full filepathname of the XSLT file</param>
    ''' <param name="fin">Full filepathname of the input file</param>
    ''' <param name="fout">Full filepathname of the output file</param>
    ''' <remarks></remarks>
    Private Sub xslTransform(ByVal xslfile As String, ByVal fin As String, ByVal fout As String)
        Dim xslt As New Xml.Xsl.XslCompiledTransform
        Dim xSettings As New Xml.Xsl.XsltSettings

        xSettings.EnableDocumentFunction = True
        xslt.Load(xslfile, xSettings, Nothing)
        xslt.Transform(fin, fout)
    End Sub



    'AE: Testing some ideas for batch validation. Requires msxml?.dll ref

    ''' <summary>
    ''' Perform XSL transformation via the COM library MSXML2.
    ''' </summary>
    ''' <param name="xslFile">Full path to the XSL transformation file</param>
    ''' <param name="fin">Full path to the XML file to be transformed</param>
    ''' <param name="fout">Full path to the file that will receive the output of the XSL transformation</param>
    ''' <remarks>This is just a different wrapper around the actual function that performs the transform.</remarks>
    Public Sub xslTransformViaCOM(ByVal xslFile As String, ByVal fin As String, ByVal fout As String)
        My.Computer.FileSystem.WriteAllText(fout, xslTransformViaCOM(xslFile, My.Computer.FileSystem.ReadAllText(fin)), False)
    End Sub

    ''' <summary>
    ''' Perform XSL transformation via the COM library MSXML2.
    ''' </summary>
    ''' <param name="xslFile"></param>
    ''' <param name="xmlStr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function xslTransformViaCOM(ByVal xslFile As String, ByVal xmlStr As String) As String
        Dim xformDom As New MSXML2.DOMDocument
        Dim outDom As New MSXML2.DOMDocument

        If outDom.loadXML(xmlStr) Then
            If xformDom.load(xslFile) Then
                Return outDom.transformNode(xformDom)
            End If
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Remove ESRI tags in metadata.
    ''' </summary>
    ''' <param name="iXPS">IXmlPropertySet that contains the metadata being edited.</param>
    ''' <returns>True if metadata was modified (i.e. at least one ESRI tag removed). False otherwise.</returns>
    ''' <remarks></remarks>
    Public Function wipeEsriTags(ByVal iXPS As XmlMetadata) As Boolean
        Dim modified As Boolean

        ' Retain PublishedDocID depending on user settings
        Dim publishedDocID As String = Nothing
        If My.Settings.WipeEsriTagsOnSync AndAlso My.Settings.RetainPublishedDocID Then
            publishedDocID = iXPS.SimpleGetProperty("Esri/PublishedDocID")
        End If

        Dim xslt As New Xml.Xsl.XslCompiledTransform
        Dim xSettings As New Xml.Xsl.XsltSettings

        xSettings.EnableDocumentFunction = True
        Try
            ' First, we will remove ESRI tags using xsl transformation in memory.
            xslt.Load(Utils.getAppFolder & "\remove_ESRI_tags.xsl", xSettings, Nothing)

            Dim ins As String = iXPS.GetXml("")
            Dim insr As New System.IO.StringReader(ins)
            Dim inr As New System.Xml.XmlTextReader(insr)
            Dim outsb As New System.Text.StringBuilder()
            Dim outw As System.Xml.XmlWriter = XmlTextWriter.Create(outsb, xslt.OutputSettings)

            xslt.Transform(inr, outw)
            ' Since .NET strings are UTF-16, xform output ends up with that encoding.
            ' We suspect, however, that AO APIs trim it down to a single-byte encoding
            ' without actually modifying xml declaration. This ends up causing issues
            ' when the file gets written with mismatching declared vs. actual encoding.
            ' We simply remove UTF-16 encoding declaration and let nature take its course.
            ' There is probably a smarter way to handle this.
            Dim outs As String = outsb.ToString
            cleanseUtf16(outs)
            modified = ins.Length <> outs.Length
            If modified Then iXPS.SetXml(outs)

            ' Retain PublishedDocID
            If publishedDocID IsNot Nothing AndAlso publishedDocID.Trim.Length > 0 Then iXPS.SetPropertyX("Esri/PublishedDocID", publishedDocID)

            Return modified
        Catch ex As Exception
            ErrorHandler(ex)
        End Try
    End Function

    ''' <summary>
    ''' XML file deserializer.
    ''' </summary>
    ''' <param name="filename">String containing the filename for an XML file that will be deserialized.</param>
    ''' <param name="targetType">The type that the XML file will be deserialized into.</param>
    ''' <returns>An instance of the requested type deserialized from the XML file provided.</returns>
    ''' <remarks></remarks>
    Public Function DeserializeFromXmlFile(ByVal filename As String, ByVal targetType As Type) As Object
        Dim retVal As Object
        Dim serializer As XmlSerializer = New XmlSerializer(targetType)
        Dim objectXml As String = My.Computer.FileSystem.ReadAllText(filename)
        Dim stringReader As StringReader = New StringReader(objectXml)

        Dim xmlReader As XmlTextReader = New XmlTextReader(stringReader)
        retVal = serializer.Deserialize(xmlReader)
        Return retVal
    End Function

    ''' <summary>
    ''' Function that will substitute instances of a string with another string in some text 
    ''' and report back the no of substitutions made.
    ''' </summary>
    ''' <param name="txt">String containing the text to operate on.</param>
    ''' <param name="oldValue">String to be replaced.</param>
    ''' <param name="newValue">String to substitute.</param>
    ''' <returns>Integer indicating the no of substitutions made.</returns>
    ''' <remarks></remarks>
    Public Function findAndReplaceWithCount(ByRef txt As String, ByVal oldValue As String, ByVal newValue As String) As Integer
        If oldValue > "" AndAlso txt > "" Then
            ' First do a fake sunstitution to determinde count.
            Dim tmp As String = txt.Replace(oldValue, "")
            ' Figure out how many times the string to be replaced occurred in target control's text
            Dim cnt As Integer = (txt.Length - tmp.Length) / oldValue.Length
            If cnt > 0 Then
                ' Actually replace only if needed.
                txt = txt.Replace(oldValue, newValue)
            End If
            Return cnt
        End If
        ' By default, return 0
        Return 0
    End Function

    ''' <summary>
    ''' Determine if the given string consist only of alphanumeric characters
    ''' </summary>
    ''' <param name="str">String to be tested</param>
    ''' <returns>True if str contains only alphanumeric characters, false otherwise.</returns>
    ''' <remarks>At least one character is required</remarks>
    Public Function IsAlphaNumeric(ByVal str As String) As Boolean
        Return System.Text.RegularExpressions.Regex.IsMatch(str, "^[\w]+$")
    End Function

    ''' <summary>
    ''' Determine if the given string consist only of numeric characters
    ''' </summary>
    ''' <param name="str">String to be tested</param>
    ''' <returns>True if str contains only numeric characters, false otherwise.</returns>
    ''' <remarks>At least one character is required</remarks>
    Public Function IsNumeric(ByVal str As String) As Boolean
        Return System.Text.RegularExpressions.Regex.IsMatch(str, "^[0-9]+$")
    End Function

    ''' <summary>
    ''' Copy source directory with given path to given target directory path.
    ''' </summary>
    ''' <param name="sourceDir">Full path to source directory</param>
    ''' <param name="targetDir">Full path to target directory</param>
    ''' <param name="searchOption">Whether only top level files are included from source directory or all files and directories under it. Defaults to all.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function copyDir(ByVal sourceDir As String, ByVal targetDir As String, Optional ByVal searchOption As IO.SearchOption = SearchOption.AllDirectories) As Boolean
        Try
            For Each f As String In System.IO.Directory.GetFiles(sourceDir, "*", searchOption)
                Dim target As String = targetDir + f.Replace(sourceDir, "")
                If Not System.IO.File.Exists(target) Then My.Computer.FileSystem.CopyFile(f, target)
            Next
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Undo of copy source directory with given path to given target directory path. 
    ''' </summary>
    ''' <param name="sourceDir">Full path to source directory</param>
    ''' <param name="targetDir">Full path to target directory</param>
    ''' <param name="searchOption">Whether only top level files are included from source directory or all files and directories under it. Defaults to all.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function uncopyDir(ByVal sourceDir As String, ByVal targetDir As String, Optional ByVal searchOption As IO.SearchOption = SearchOption.AllDirectories) As Boolean
        Try
            For Each f As String In System.IO.Directory.GetFiles(sourceDir, "*", searchOption)
                Dim target As String = targetDir + f.Replace(sourceDir, "")
                If System.IO.File.Exists(target) Then My.Computer.FileSystem.DeleteFile(target)
            Next
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Check if copy of source directory with given path to given target directory path has been performed previously.
    ''' </summary>
    ''' <param name="sourceDir">Full path to source directory</param>
    ''' <param name="targetDir">Full path to target directory</param>
    ''' <param name="searchOption">Whether only top level files are included from source directory or all files and directories under it. Defaults to all.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function copyDirPerformed(ByVal sourceDir As String, ByVal targetDir As String, Optional ByVal searchOption As IO.SearchOption = SearchOption.AllDirectories) As Boolean
        Try
            For Each f As String In System.IO.Directory.GetFiles(sourceDir, "*", searchOption)
                Dim target As String = targetDir + f.Replace(sourceDir, "")
                If Not System.IO.File.Exists(target) Then Return False
            Next
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Copy the contents of the template folder to the user's AppData system/special folder.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>We need this because each user has to work in folders reserved for them due to increasing security/privacy restrictions in Windows operating systems.</remarks>
    Public Function checkAndCopyTemplate() As Boolean
        Try
            Return copyDir(getTemplateFolder(), getAppDataFolder())
        Catch ex As Exception
            Return False
        End Try
    End Function


    ''' <summary>
    ''' Apply an XSL transform to an XML file.
    ''' </summary>
    ''' <param name="xslfile">Full filepathname of the XSLT file</param>
    ''' <param name="fin">Full filepathname of the input file</param>
    ''' <param name="fout">Full filepathname of the output file</param>
    ''' <remarks>.NET way to do XSL transform does not work so well for some stylesheets from the "XSL Patterns" era. Currently not used.</remarks>
    Public Sub transform(ByVal xslfile As String, ByVal fin As String, ByVal fout As String)
        Dim xslt As New Xml.Xsl.XslCompiledTransform
        Dim xSettings As New Xml.Xsl.XsltSettings
        xSettings.EnableDocumentFunction = True
        Try
            xslt.Load(xslfile, xSettings, Nothing)
            xslt.Transform(fin, fout)
        Catch ex As Exception
            Throw New ApplicationException("Transform problem with " & xslfile & ", " & fin & ", " & fout & ": " & ex.ToString)
        End Try
    End Sub


    ''' <summary>
    ''' The workhorse subroutine to do actual work of displaying help from a .chm file.
    ''' </summary>
    ''' <param name="helpPage">Default help page to display if no help for requiested name can be found.</param>
    ''' <remarks>We let only one help page open at a time. This is by design.</remarks>
    Public Sub HelpSeeker(ByVal helpPage As String, ByRef proc As Process)
        Try
            proc.CloseMainWindow()
            proc.Refresh()
        Catch ex As Exception
        End Try
        Try
            'Read path to hh.exe from application settings
            Dim HHPath As String = My.Settings.HHPath.Trim()
            'If nothing in application settings or the specified file does not exist...
            If HHPath = "" OrElse Not System.IO.File.Exists(HHPath) Then
                'then look for it in windows directory
                HHPath = Environ$("windir") & "\hh.exe"
                If Not System.IO.File.Exists(HHPath) Then
                    MessageBox.Show("Unable to find hh.exe required for displaying help. Please enter full pathname for it in '" & Utils.getAppDataFolder() & "\app.config', e.g. 'C:\Windows\hh.exe'", "EME", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Sub
                End If
            End If
            proc.StartInfo.FileName = HHPath

            Dim helpStr As String = """" & Utils.getAppFolder() & "\" & GlobalVars.nameStr & " Help.chm::"
            helpStr &= helpPage
            helpStr &= """"
            'Debug.Print(helpStr)
            proc.StartInfo.Arguments = helpStr
            proc.Start()
        Catch ex As Exception
            'Gracefully fail if we can't get help.
            MessageBox.Show("Unable to show help for this element!", "EME", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try

    End Sub


    ''' <summary>
    ''' Compute the MD5 digest of the given text.
    ''' </summary>
    ''' <param name="SourceText">The text for which MD5 digest is to be computed.</param>
    ''' <returns>MD5 digest of the given text.</returns>
    ''' <remarks></remarks>
    Public Function stringMd5(ByVal SourceText As String) As String
        'Create an encoding object to ensure the encoding standard for the source text
        Dim Ue As New UnicodeEncoding()
        'Retrieve a byte array based on the source text
        Dim ByteSourceText() As Byte = Ue.GetBytes(SourceText)
        'Instantiate an MD5 Provider object
        Dim Md5 As New MD5CryptoServiceProvider()
        'Compute the hash value from the source
        Dim ByteHash() As Byte = Md5.ComputeHash(ByteSourceText)
        'And convert it to String format for return
        Return Convert.ToBase64String(ByteHash)
    End Function

    ''' <summary>
    ''' Compute the MD5 digest of the text in the given file.
    ''' </summary>
    ''' <param name="srcFile">Full path name of the file which MD5 digest is to be computed for.</param>
    ''' <returns>MD5 digest of the given file.</returns>
    ''' <remarks></remarks>
    Public Function fileMd5(ByVal srcFile As String) As String
        Dim rdr As New IO.FileStream(srcFile, FileMode.Open)
        'Instantiate an MD5 Provider object
        Dim Md5 As New MD5CryptoServiceProvider()
        'Compute the hash value from the source
        Dim ByteHash() As Byte = Md5.ComputeHash(rdr)
        rdr.Close()
        rdr.Dispose()
        'And convert it to String format for return
        Return Convert.ToBase64String(ByteHash)
    End Function

    ''' <summary>
    ''' Determine if we are running as an installed app or from within VS IDE.
    ''' </summary>
    ''' <returns>True if application/extension is running in the field as an installed app, False otherwise.</returns>
    ''' <remarks></remarks>
    Function iAmInstalled() As Boolean
        Return getAppFolder.StartsWith(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) OrElse Not Debugger.IsAttached
    End Function


    ''' <summary>
    ''' Perform metadata validation using MP and schematron rules (for EPA compliance)
    ''' using webservice or local validation.
    ''' </summary>
    ''' <param name="args">Hashtable containing arguments to be used during validation.</param>
    ''' <returns>Boolean value indication successful completion.</returns>
    ''' <remarks></remarks>
    Public Function ValidateActionDo(ByVal args As Hashtable) As Boolean
        cleanseUtf16(args("Metadata"))
        Dim dom As New XmlDocument()
        dom.LoadXml(args("Metadata"))
        Dim filename As String = IO.Path.GetTempFileName() & ".xml"
        dom.Save(filename)
        Dim txt As String = My.Computer.FileSystem.ReadAllText(filename)
        txt = withoutDoctype(txt)

        Dim epaResults As String
        Dim mpResults As String
        Try
            'We do this every so often to make sure we handle cancellation requests promptly...
            'If ValidationWorker.CancellationPending Then Exit Function

            If args("ValidationMode") = ValidationMode.Webservice Then
                'epaResults = Utils.decorateXSL((New Validator).validate_epa(txt), True)
                epaResults = (New Validator).validate_epa(txt)
                'If ValidationWorker.CancellationPending Then Exit Function
                mpResults = (New Validator).validate_mp(txt)
            ElseIf args("ValidationMode") = ValidationMode.Local Then
                'epaResults = Utils.decorateXSL(LocalValidator.validate_epa(Utils.getAppDataFolder, txt), True)
                epaResults = LocalValidator.validate_epa(Utils.getAppDataFolder, txt)
                'If ValidationWorker.CancellationPending Then Exit Function
                mpResults = (LocalValidator.validate_mp(Utils.getAppDataFolder, txt))
            Else
                Exit Function
            End If
        Catch ex As Exception
            ErrorHandler(ex)
            Exit Function
        End Try

        'If ValidationWorker.CancellationPending Then Exit Function


        'Dim xslt As New Xml.Xsl.XslCompiledTransform
        'Dim xSettings As New Xml.Xsl.XsltSettings

        'xSettings.EnableDocumentFunction = True

        'xslt.Load(Utils.getAppFolder & "\eme.xsl", xSettings, Nothing)
        'Dim insr As New System.IO.StringReader(Utils.mergeEPAandMP(epaResults, mpResults))
        'Dim inr As New System.Xml.XmlTextReader(insr)
        'Dim outsb As New System.IO.StringWriter
        'Dim outw As New System.Xml.XmlTextWriter(outsb)

        'xslt.Transform(inr, outw)
        'filename = Utils.textToTempFile(outsb.ToString)

        'filename = Utils.textToTempFile(Utils.xslTransformViaCOM(Utils.getAppFolder & "\eme.xsl", Utils.mergeEPAandMP(epaResults, mpResults)))
        filename = Utils.textToTempFile(Utils.mergeEPAandMP(epaResults, mpResults))



        Dim aNode As XmlNode

        dom.Load(filename)

        aNode = dom.CreateElement("src")
        aNode.InnerText = txt
        dom.LastChild.AppendChild(aNode)

        aNode = dom.CreateElement("ObjectName")
        aNode.InnerText = args("ObjectName")
        dom.LastChild.AppendChild(aNode)

        dom.Save(filename)

        'AE: for bundled validation. Gotta review eme.xsl, probably separate css, other improvements...
        'filename = Utils.textToTempFile(Utils.xslTransformViaCOM(Utils.getAppFolder & "\eme.xsl", dom.InnerXml), ".html")

        Dim filename_html As String = filename.Replace(".xml", ".html")
        Utils.xslTransformViaCOM(Utils.filenameAsUrl(Utils.getAppFolder + "\eme.xsl"), filename, filename_html)

        args("Filename") = filename_html
        args("DOM") = dom
        Return True
    End Function

    ''' <summary>
    ''' Remove the DOCTYPE declaration in the metadata record - if it exists.
    ''' </summary>
    ''' <param name="mdXml">XML metadata record.</param>
    ''' <returns>The metadata record after removal of DOCTYPE declaration.</returns>
    ''' <remarks></remarks>
    Function withoutDoctype(ByVal mdXml As String) As String
        Dim startIdx As Integer = mdXml.IndexOf("<!DOCTYPE ")
        If startIdx > -1 Then
            Dim endIdx As Integer = mdXml.IndexOf("<metadata", startIdx + 1)
            If endIdx > -1 Then
                Return mdXml.Substring(0, startIdx) + mdXml.Substring(endIdx, mdXml.Length() - endIdx)
            Else
                ' We should never get here unless the xml is not a valid metadata xml
                Return mdXml
            End If
        Else
            Return mdXml
        End If
    End Function

    ''' <summary>
    ''' Determine if the given file contains well-formed XML.
    ''' </summary>
    ''' <param name="filename">Full path to the file to be tested.</param>
    ''' <returns>True if file contains well-formed XML, False otherwise.</returns>
    ''' <remarks></remarks>
    Function isWellFormedXmlFile(ByVal filename As String) As Boolean
        Try
            Dim txt As String = My.Computer.FileSystem.ReadAllText(filename)
            Return isWellFormedXmlString(txt)
        Catch ex As Exception
        End Try
        Return False
    End Function

    ''' <summary>
    ''' Determine if the given text contains well-formed XML.
    ''' </summary>
    ''' <param name="txt">Text to be tested.</param>
    ''' <returns>True if the text contains well-formed XML, False otherwise.</returns>
    ''' <remarks></remarks>
    Function isWellFormedXmlString(ByVal txt As String) As Boolean
        Dim r As New XmlTextReader(New StringReader(txt))
        Try
            While r.Read()
            End While
        Catch ex As Exception
            r.Close()
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' Turn the given filename into a valid Windows filename.
    ''' </summary>
    ''' <param name="filename">Filename to operate on</param>
    ''' <returns>Returns the filename modified by replacing any invalid characters with underscore character.</returns>
    ''' <remarks></remarks>
    Function asSafeFilename(filepath As String, ByVal filename As String, Optional commandLineFriendly As Boolean = False) As String
        For Each c As Char In System.IO.Path.GetInvalidFileNameChars()
            filename = filename.Replace(c, "_")
        Next
        asSafeFilename = IO.Path.Combine(filepath, filename)
        If commandLineFriendly AndAlso Not filename.Contains("""") Then
            asSafeFilename = """" + asSafeFilename + """"
        End If
    End Function

    ''' <summary>
    ''' Turn file reference to a URL.
    ''' </summary>
    ''' <param name="filename">Full path to the file</param>
    ''' <returns>A String containing the URL reference to the given file.</returns>
    ''' <remarks></remarks>
    Function filenameAsUrl(ByVal filename As String) As String
        Return "file:///" & filename.Replace("\", "/")
    End Function


    ''' <summary>
    ''' Get a string value from the registry
    ''' </summary>
    ''' <param name="regkey">Registry hive to look under</param>
    ''' <param name="subkey">Registry subkey to look under</param>
    ''' <param name="keyname">Name of requested registry key</param>
    ''' <param name="defaultValue">Default value to return if key not found</param>
    ''' <returns>The full path of the directory where ArcGIS Desktop 10.0 has been installed.</returns>
    ''' <remarks></remarks>
    Public Function getRegistryString(regkey As RegistryKey, subkey As String, keyname As String, Optional defaultValue As String = "") As String
        getRegistryString = Nothing
        Dim keyObj As RegistryKey = regkey.OpenSubKey(subkey, False)
        If (Not keyObj Is Nothing) Then
            getRegistryString = keyObj.GetValue(keyname, defaultValue).ToString
            keyObj.Close()
        End If
    End Function


    ''' <summary>
    ''' Get the location of ArcGIS installation
    ''' </summary>
    ''' <returns>The full path of the directory where ArcGIS Desktop 10.0 has been installed.</returns>
    ''' <remarks></remarks>
    Public Function getArcgisInstallDir() As String
        Dim versionStr As String = getRegistryString(Registry.LocalMachine, "SOFTWARE\ESRI\ArcGIS", "RealVersion")
        If versionStr Is Nothing Then
            Return Nothing  ' ArcGIS not installed
        Else
            Dim idx As Integer = versionStr.IndexOf(".")
            idx = versionStr.IndexOf(".", idx + 1)
            versionStr = versionStr.Substring(0, idx)
            Return getRegistryString(Registry.LocalMachine, "SOFTWARE\ESRI\Desktop" + versionStr, "InstallDir") ' "SOFTWARE\ESRI\Desktop10.0" for ArcGIS Desktop 10.0
        End If
    End Function

    ''' <summary>
    ''' Remove the utf-16 encoding attribute from xml header as it is typically incorrect
    ''' </summary>
    ''' <param name="md">The string containing metadata to cleanse. Passed by reference, hence changed in place if needed.</param>
    ''' <returns>String containing cleansed metadata</returns>
    ''' <remarks>Will we ever stumble on metadata actually in utf-16?</remarks>
    Public Function cleanseUtf16(ByRef md As String) As String
        Dim enc As String = "encoding = ""utf-16"""
        md = RegularExpressions.Regex.Replace(md, enc, "", RegularExpressions.RegexOptions.IgnoreCase Or RegularExpressions.RegexOptions.IgnorePatternWhitespace)
        Return md
    End Function


    ''' <summary>
    ''' Prevent LinkLabel objects from becoming tab stops.
    ''' </summary>
    ''' <param name="ctrl">Control to check</param>
    ''' <remarks></remarks>
    Public Sub disableLinkLabelTabStops(ctrl As Control)
        If TypeOf ctrl Is LinkLabel Then
            ctrl.TabStop = False
        Else
            For Each ctrl2 As Control In ctrl.Controls
                disableLinkLabelTabStops(ctrl2)
            Next
        End If
    End Sub

    ''' <summary>
    ''' Prettier expression of a count
    ''' </summary>
    ''' <param name="txt">Name of stuff to be counted</param>
    ''' <param name="count">The count to use</param>
    ''' <returns>Decorated expression</returns>
    ''' <remarks></remarks>
    Public Function decorateCountPhrase(txt As String, count As Integer) As String
        txt = " " + txt

        If count = 0 Then
            txt = "No" + txt + "s"
        ElseIf count = 1 Then
            txt = "1" + txt
        Else
            txt = Str(count).Trim + txt + "s"
        End If
        Return txt + " found."
    End Function

    ''' <summary>
    ''' Return the same filename with a new extension
    ''' </summary>
    ''' <param name="filename">Name of the file</param>
    ''' <param name="newExtension">New extension for file</param>
    ''' <returns></returns>
    ''' <remarks>If the file does not have an extension (no .), then we simply append the new extension.</remarks>
    Public Function changeExtensionTo(filename As String, newExtension As String) As String
        If Not newExtension.StartsWith(".") Then newExtension = "." + newExtension
        Dim idx As Integer = filename.LastIndexOf(".")
        If idx > -1 Then
            filename = Left(filename, idx) + newExtension
        Else
            filename = filename + newExtension
        End If
        Return filename
    End Function


    ''' <summary>
    ''' Use MP to save metadata in formats other than XML 
    ''' </summary>
    ''' <param name="filepath">The filesystem path where files will be stored.</param>
    ''' <param name="xmlFilename">Name of the XML file</param>
    ''' <param name="selectedFormats">Formats to generate</param>
    ''' <remarks></remarks>
    Public Sub exportMetadataWithMp(filepath As String, xmlFilename As String, selectedFormats As List(Of String))
        Dim args = asSafeFilename(filepath, xmlFilename, True)
        'Text export
        If selectedFormats.Contains("t") Then
            args += " -t " + asSafeFilename(filepath, changeExtensionTo(xmlFilename, ".txt"), True)
        End If
        'HTML export
        If selectedFormats.Contains("h") Then
            args += " -h " + asSafeFilename(filepath, changeExtensionTo(xmlFilename, ".html"), True)
        End If
        'Error log
        args += " -e " + asSafeFilename(filepath, changeExtensionTo(xmlFilename, ".log"), True)


        Try
            'Create the Process
            Dim startInfo As System.Diagnostics.ProcessStartInfo
            Dim pStart As New System.Diagnostics.Process

            'Dim processArguments As String = """" & xmlFile & """" & " -e " & """" & errorFileName & """"

            'Feed the executable file path and arguments
            startInfo = New System.Diagnostics.ProcessStartInfo(asSafeFilename(getMpFolder(), "mp_win.exe", True), args)
            pStart.StartInfo = startInfo

            'Start the process
            pStart.Start()

            'Code will halt until the exe file has executed
            pStart.WaitForExit()
        Catch ex As Exception
            Dim a = 1
        End Try

    End Sub


End Module
