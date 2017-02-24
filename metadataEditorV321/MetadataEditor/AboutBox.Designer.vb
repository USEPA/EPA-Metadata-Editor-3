<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AboutBox
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AboutBox))
        Me.pbEPA = New System.Windows.Forms.PictureBox()
        Me.pbOEI = New System.Windows.Forms.PictureBox()
        Me.pbCDA = New System.Windows.Forms.PictureBox()
        Me.LabelProductName = New System.Windows.Forms.Label()
        Me.LabelCopyright = New System.Windows.Forms.Label()
        Me.LabelVersion = New System.Windows.Forms.Label()
        Me.btnOK = New System.Windows.Forms.Button()
        CType(Me.pbEPA, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbOEI, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbCDA, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbEPA
        '
        Me.pbEPA.Image = CType(resources.GetObject("pbEPA.Image"), System.Drawing.Image)
        Me.pbEPA.Location = New System.Drawing.Point(22, 37)
        Me.pbEPA.Name = "pbEPA"
        Me.pbEPA.Size = New System.Drawing.Size(63, 54)
        Me.pbEPA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbEPA.TabIndex = 2
        Me.pbEPA.TabStop = False
        '
        'pbOEI
        '
        Me.pbOEI.Image = CType(resources.GetObject("pbOEI.Image"), System.Drawing.Image)
        Me.pbOEI.Location = New System.Drawing.Point(111, 48)
        Me.pbOEI.Name = "pbOEI"
        Me.pbOEI.Size = New System.Drawing.Size(124, 43)
        Me.pbOEI.TabIndex = 1
        Me.pbOEI.TabStop = False
        '
        'pbCDA
        '
        Me.pbCDA.Image = CType(resources.GetObject("pbCDA.Image"), System.Drawing.Image)
        Me.pbCDA.Location = New System.Drawing.Point(261, 37)
        Me.pbCDA.Name = "pbCDA"
        Me.pbCDA.Size = New System.Drawing.Size(156, 54)
        Me.pbCDA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbCDA.TabIndex = 0
        Me.pbCDA.TabStop = False
        '
        'LabelProductName
        '
        Me.LabelProductName.AutoSize = True
        Me.LabelProductName.Location = New System.Drawing.Point(19, 9)
        Me.LabelProductName.Name = "LabelProductName"
        Me.LabelProductName.Size = New System.Drawing.Size(39, 13)
        Me.LabelProductName.TabIndex = 3
        Me.LabelProductName.Text = "Label1"
        '
        'LabelCopyright
        '
        Me.LabelCopyright.AutoSize = True
        Me.LabelCopyright.Location = New System.Drawing.Point(19, 114)
        Me.LabelCopyright.Name = "LabelCopyright"
        Me.LabelCopyright.Size = New System.Drawing.Size(39, 13)
        Me.LabelCopyright.TabIndex = 4
        Me.LabelCopyright.Text = "Label1"
        '
        'LabelVersion
        '
        Me.LabelVersion.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.LabelVersion.AutoSize = True
        Me.LabelVersion.Location = New System.Drawing.Point(19, 161)
        Me.LabelVersion.Name = "LabelVersion"
        Me.LabelVersion.Size = New System.Drawing.Size(39, 13)
        Me.LabelVersion.TabIndex = 5
        Me.LabelVersion.Text = "Label1"
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(353, 156)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 6
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'AboutBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(440, 191)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.LabelVersion)
        Me.Controls.Add(Me.LabelCopyright)
        Me.Controls.Add(Me.LabelProductName)
        Me.Controls.Add(Me.pbCDA)
        Me.Controls.Add(Me.pbOEI)
        Me.Controls.Add(Me.pbEPA)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AboutBox"
        Me.Padding = New System.Windows.Forms.Padding(9)
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "AboutBox"
        CType(Me.pbEPA, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbOEI, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbCDA, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pbEPA As System.Windows.Forms.PictureBox
    Friend WithEvents pbOEI As System.Windows.Forms.PictureBox
    Friend WithEvents pbCDA As System.Windows.Forms.PictureBox
    Friend WithEvents LabelProductName As System.Windows.Forms.Label
    Friend WithEvents LabelCopyright As System.Windows.Forms.Label
    Friend WithEvents LabelVersion As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button

End Class
