<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ContactForm
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
        Me.pnlContact = New System.Windows.Forms.Panel()
        Me.cntinfo = New System.Windows.Forms.ComboBox()
        Me.cntinfo_cntorgp = New System.Windows.Forms.RadioButton()
        Me.cntinfo_____default = New System.Windows.Forms.Button()
        Me.cntinfo_cntperp = New System.Windows.Forms.RadioButton()
        Me.btnCloseDiscard = New System.Windows.Forms.Button()
        Me.btnCloseSave = New System.Windows.Forms.Button()
        Me.pnlContact.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlContact
        '
        Me.pnlContact.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlContact.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlContact.Controls.Add(Me.cntinfo)
        Me.pnlContact.Controls.Add(Me.cntinfo_cntorgp)
        Me.pnlContact.Controls.Add(Me.cntinfo_____default)
        Me.pnlContact.Controls.Add(Me.cntinfo_cntperp)
        Me.pnlContact.Location = New System.Drawing.Point(0, 0)
        Me.pnlContact.Name = "pnlContact"
        Me.pnlContact.Size = New System.Drawing.Size(595, 68)
        Me.pnlContact.TabIndex = 0
        '
        'cntinfo
        '
        Me.cntinfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cntinfo.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cntinfo.FormattingEnabled = True
        Me.cntinfo.Location = New System.Drawing.Point(8, 30)
        Me.cntinfo.Name = "cntinfo"
        Me.cntinfo.Size = New System.Drawing.Size(521, 21)
        Me.cntinfo.TabIndex = 2
        '
        'cntinfo_cntorgp
        '
        Me.cntinfo_cntorgp.AutoSize = True
        Me.cntinfo_cntorgp.Checked = True
        Me.cntinfo_cntorgp.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cntinfo_cntorgp.Location = New System.Drawing.Point(152, 8)
        Me.cntinfo_cntorgp.Name = "cntinfo_cntorgp"
        Me.cntinfo_cntorgp.Size = New System.Drawing.Size(125, 17)
        Me.cntinfo_cntorgp.TabIndex = 1
        Me.cntinfo_cntorgp.TabStop = True
        Me.cntinfo_cntorgp.Text = "Primary Organization"
        Me.cntinfo_cntorgp.UseVisualStyleBackColor = True
        '
        'cntinfo_____default
        '
        Me.cntinfo_____default.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cntinfo_____default.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.cntinfo_____default.Font = New System.Drawing.Font("Tahoma", 8.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cntinfo_____default.Location = New System.Drawing.Point(545, 28)
        Me.cntinfo_____default.Name = "cntinfo_____default"
        Me.cntinfo_____default.Size = New System.Drawing.Size(33, 23)
        Me.cntinfo_____default.TabIndex = 3
        Me.cntinfo_____default.Text = "D"
        Me.cntinfo_____default.UseVisualStyleBackColor = False
        '
        'cntinfo_cntperp
        '
        Me.cntinfo_cntperp.AutoSize = True
        Me.cntinfo_cntperp.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cntinfo_cntperp.Location = New System.Drawing.Point(8, 8)
        Me.cntinfo_cntperp.Name = "cntinfo_cntperp"
        Me.cntinfo_cntperp.Size = New System.Drawing.Size(97, 17)
        Me.cntinfo_cntperp.TabIndex = 0
        Me.cntinfo_cntperp.Text = "Primary Person"
        Me.cntinfo_cntperp.UseVisualStyleBackColor = True
        '
        'btnCloseDiscard
        '
        Me.btnCloseDiscard.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCloseDiscard.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCloseDiscard.Location = New System.Drawing.Point(500, 86)
        Me.btnCloseDiscard.Name = "btnCloseDiscard"
        Me.btnCloseDiscard.Size = New System.Drawing.Size(78, 25)
        Me.btnCloseDiscard.TabIndex = 2
        Me.btnCloseDiscard.Text = "Cancel"
        Me.btnCloseDiscard.UseVisualStyleBackColor = True
        '
        'btnCloseSave
        '
        Me.btnCloseSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCloseSave.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCloseSave.Location = New System.Drawing.Point(402, 86)
        Me.btnCloseSave.Name = "btnCloseSave"
        Me.btnCloseSave.Size = New System.Drawing.Size(78, 25)
        Me.btnCloseSave.TabIndex = 1
        Me.btnCloseSave.Text = "Save && Close"
        Me.btnCloseSave.UseVisualStyleBackColor = True
        '
        'ContactForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(592, 123)
        Me.Controls.Add(Me.btnCloseDiscard)
        Me.Controls.Add(Me.btnCloseSave)
        Me.Controls.Add(Me.pnlContact)
        Me.MinimumSize = New System.Drawing.Size(500, 150)
        Me.Name = "ContactForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Contact Form"
        Me.pnlContact.ResumeLayout(False)
        Me.pnlContact.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlContact As System.Windows.Forms.Panel
    Friend WithEvents cntinfo As System.Windows.Forms.ComboBox
    Friend WithEvents cntinfo_cntorgp As System.Windows.Forms.RadioButton
    Friend WithEvents cntinfo_____default As System.Windows.Forms.Button
    Friend WithEvents cntinfo_cntperp As System.Windows.Forms.RadioButton
    Friend WithEvents btnCloseDiscard As System.Windows.Forms.Button
    Friend WithEvents btnCloseSave As System.Windows.Forms.Button
End Class
