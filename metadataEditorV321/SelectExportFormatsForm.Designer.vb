<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SelectExportFormatsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.gbExportFormats = New System.Windows.Forms.GroupBox()
        Me.cbExportHtml = New System.Windows.Forms.CheckBox()
        Me.cbExportTxt = New System.Windows.Forms.CheckBox()
        Me.cbExportXml = New System.Windows.Forms.CheckBox()
        Me.lblExportFormats = New System.Windows.Forms.Label()
        Me.gbExportMetadataFilename = New System.Windows.Forms.GroupBox()
        Me.tbExportMetadataFilename = New System.Windows.Forms.TextBox()
        Me.lblExportMetadataFilename = New System.Windows.Forms.Label()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.gbExportFormats.SuspendLayout()
        Me.gbExportMetadataFilename.SuspendLayout()
        Me.SuspendLayout()
        '
        'gbExportFormats
        '
        Me.gbExportFormats.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbExportFormats.Controls.Add(Me.cbExportHtml)
        Me.gbExportFormats.Controls.Add(Me.cbExportTxt)
        Me.gbExportFormats.Controls.Add(Me.cbExportXml)
        Me.gbExportFormats.Controls.Add(Me.lblExportFormats)
        Me.gbExportFormats.Location = New System.Drawing.Point(5, 9)
        Me.gbExportFormats.Name = "gbExportFormats"
        Me.gbExportFormats.Size = New System.Drawing.Size(521, 100)
        Me.gbExportFormats.TabIndex = 7
        Me.gbExportFormats.TabStop = False
        '
        'cbExportHtml
        '
        Me.cbExportHtml.AutoSize = True
        Me.cbExportHtml.Location = New System.Drawing.Point(8, 73)
        Me.cbExportHtml.Name = "cbExportHtml"
        Me.cbExportHtml.Size = New System.Drawing.Size(56, 17)
        Me.cbExportHtml.TabIndex = 11
        Me.cbExportHtml.Text = "HTML"
        Me.cbExportHtml.UseVisualStyleBackColor = True
        '
        'cbExportTxt
        '
        Me.cbExportTxt.AutoSize = True
        Me.cbExportTxt.Location = New System.Drawing.Point(8, 50)
        Me.cbExportTxt.Name = "cbExportTxt"
        Me.cbExportTxt.Size = New System.Drawing.Size(47, 17)
        Me.cbExportTxt.TabIndex = 10
        Me.cbExportTxt.Text = "Text"
        Me.cbExportTxt.UseVisualStyleBackColor = True
        '
        'cbExportXml
        '
        Me.cbExportXml.AutoSize = True
        Me.cbExportXml.Checked = True
        Me.cbExportXml.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbExportXml.Enabled = False
        Me.cbExportXml.Location = New System.Drawing.Point(8, 27)
        Me.cbExportXml.Name = "cbExportXml"
        Me.cbExportXml.Size = New System.Drawing.Size(48, 17)
        Me.cbExportXml.TabIndex = 9
        Me.cbExportXml.Text = "XML"
        Me.cbExportXml.UseVisualStyleBackColor = True
        '
        'lblExportFormats
        '
        Me.lblExportFormats.AutoSize = True
        Me.lblExportFormats.Location = New System.Drawing.Point(5, 11)
        Me.lblExportFormats.Name = "lblExportFormats"
        Me.lblExportFormats.Size = New System.Drawing.Size(460, 13)
        Me.lblExportFormats.TabIndex = 8
        Me.lblExportFormats.Text = "Your metadata will be exported as XML. Please select any additional formats to in" & _
    "clude in export:"
        '
        'gbExportMetadataFilename
        '
        Me.gbExportMetadataFilename.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbExportMetadataFilename.Controls.Add(Me.tbExportMetadataFilename)
        Me.gbExportMetadataFilename.Controls.Add(Me.lblExportMetadataFilename)
        Me.gbExportMetadataFilename.Location = New System.Drawing.Point(7, 298)
        Me.gbExportMetadataFilename.Name = "gbExportMetadataFilename"
        Me.gbExportMetadataFilename.Size = New System.Drawing.Size(510, 65)
        Me.gbExportMetadataFilename.TabIndex = 8
        Me.gbExportMetadataFilename.TabStop = False
        '
        'tbExportMetadataFilename
        '
        Me.tbExportMetadataFilename.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbExportMetadataFilename.Location = New System.Drawing.Point(7, 37)
        Me.tbExportMetadataFilename.Name = "tbExportMetadataFilename"
        Me.tbExportMetadataFilename.Size = New System.Drawing.Size(497, 20)
        Me.tbExportMetadataFilename.TabIndex = 7
        '
        'lblExportMetadataFilename
        '
        Me.lblExportMetadataFilename.AutoSize = True
        Me.lblExportMetadataFilename.Location = New System.Drawing.Point(7, 17)
        Me.lblExportMetadataFilename.Name = "lblExportMetadataFilename"
        Me.lblExportMetadataFilename.Size = New System.Drawing.Size(310, 13)
        Me.lblExportMetadataFilename.TabIndex = 6
        Me.lblExportMetadataFilename.Text = "Filename for exported metadata (for single metadata export only):"
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOK.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOK.Location = New System.Drawing.Point(475, 119)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(51, 25)
        Me.btnOK.TabIndex = 30
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'SelectExportFormatsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(529, 152)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.gbExportMetadataFilename)
        Me.Controls.Add(Me.gbExportFormats)
        Me.Name = "SelectExportFormatsForm"
        Me.Text = "Metadata Export Options"
        Me.gbExportFormats.ResumeLayout(False)
        Me.gbExportFormats.PerformLayout()
        Me.gbExportMetadataFilename.ResumeLayout(False)
        Me.gbExportMetadataFilename.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gbExportFormats As System.Windows.Forms.GroupBox
    Friend WithEvents cbExportHtml As System.Windows.Forms.CheckBox
    Friend WithEvents cbExportTxt As System.Windows.Forms.CheckBox
    Friend WithEvents cbExportXml As System.Windows.Forms.CheckBox
    Friend WithEvents lblExportFormats As System.Windows.Forms.Label
    Friend WithEvents gbExportMetadataFilename As System.Windows.Forms.GroupBox
    Friend WithEvents tbExportMetadataFilename As System.Windows.Forms.TextBox
    Friend WithEvents lblExportMetadataFilename As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
End Class
