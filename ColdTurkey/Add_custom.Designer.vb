<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Add_custom
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Add_custom))
        Me.list_cus2 = New System.Windows.Forms.ListBox()
        Me.addbox_remove = New System.Windows.Forms.Button()
        Me.txt_add2 = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.addbox_add = New System.Windows.Forms.Button()
        Me.addbox_done = New System.Windows.Forms.Button()
        Me.addbox_cancel = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.importfile = New System.Windows.Forms.OpenFileDialog()
        Me.import = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'list_cus2
        '
        Me.list_cus2.BackColor = System.Drawing.Color.White
        Me.list_cus2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.list_cus2.ForeColor = System.Drawing.Color.Black
        Me.list_cus2.FormattingEnabled = True
        Me.list_cus2.Location = New System.Drawing.Point(131, 80)
        Me.list_cus2.Name = "list_cus2"
        Me.list_cus2.Size = New System.Drawing.Size(192, 132)
        Me.list_cus2.TabIndex = 0
        '
        'addbox_remove
        '
        Me.addbox_remove.BackColor = System.Drawing.SystemColors.Control
        Me.addbox_remove.ForeColor = System.Drawing.Color.Black
        Me.addbox_remove.Location = New System.Drawing.Point(329, 80)
        Me.addbox_remove.Name = "addbox_remove"
        Me.addbox_remove.Size = New System.Drawing.Size(79, 21)
        Me.addbox_remove.TabIndex = 27
        Me.addbox_remove.Text = "- Remove"
        Me.addbox_remove.UseVisualStyleBackColor = False
        '
        'txt_add2
        '
        Me.txt_add2.BackColor = System.Drawing.Color.White
        Me.txt_add2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txt_add2.ForeColor = System.Drawing.Color.Black
        Me.txt_add2.Location = New System.Drawing.Point(131, 54)
        Me.txt_add2.Name = "txt_add2"
        Me.txt_add2.Size = New System.Drawing.Size(192, 20)
        Me.txt_add2.TabIndex = 28
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(88, 56)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(37, 15)
        Me.Label2.TabIndex = 29
        Me.Label2.Text = "www."
        '
        'addbox_add
        '
        Me.addbox_add.BackColor = System.Drawing.SystemColors.Control
        Me.addbox_add.ForeColor = System.Drawing.Color.Black
        Me.addbox_add.Location = New System.Drawing.Point(329, 52)
        Me.addbox_add.Name = "addbox_add"
        Me.addbox_add.Size = New System.Drawing.Size(79, 21)
        Me.addbox_add.TabIndex = 30
        Me.addbox_add.Text = "+ Add"
        Me.addbox_add.UseVisualStyleBackColor = False
        '
        'addbox_done
        '
        Me.addbox_done.BackColor = System.Drawing.SystemColors.Control
        Me.addbox_done.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.addbox_done.ForeColor = System.Drawing.Color.Black
        Me.addbox_done.Location = New System.Drawing.Point(241, 240)
        Me.addbox_done.Name = "addbox_done"
        Me.addbox_done.Size = New System.Drawing.Size(112, 27)
        Me.addbox_done.TabIndex = 32
        Me.addbox_done.Text = "Done"
        Me.addbox_done.UseVisualStyleBackColor = False
        '
        'addbox_cancel
        '
        Me.addbox_cancel.BackColor = System.Drawing.SystemColors.Control
        Me.addbox_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.addbox_cancel.ForeColor = System.Drawing.Color.Black
        Me.addbox_cancel.Location = New System.Drawing.Point(109, 240)
        Me.addbox_cancel.Name = "addbox_cancel"
        Me.addbox_cancel.Size = New System.Drawing.Size(112, 27)
        Me.addbox_cancel.TabIndex = 33
        Me.addbox_cancel.Text = "Cancel"
        Me.addbox_cancel.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(88, 80)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(37, 15)
        Me.Label1.TabIndex = 34
        Me.Label1.Text = "www."
        '
        'importfile
        '
        Me.importfile.Filter = "Text files (*.txt)|*.txt"
        '
        'import
        '
        Me.import.BackColor = System.Drawing.SystemColors.Control
        Me.import.Location = New System.Drawing.Point(329, 191)
        Me.import.Name = "import"
        Me.import.Size = New System.Drawing.Size(79, 21)
        Me.import.TabIndex = 35
        Me.import.Text = "Import File..."
        Me.import.UseVisualStyleBackColor = False
        '
        'Add_custom
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.ClientSize = New System.Drawing.Size(452, 300)
        Me.Controls.Add(Me.import)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.addbox_cancel)
        Me.Controls.Add(Me.addbox_done)
        Me.Controls.Add(Me.addbox_add)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txt_add2)
        Me.Controls.Add(Me.addbox_remove)
        Me.Controls.Add(Me.list_cus2)
        Me.ForeColor = System.Drawing.Color.Black
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Add_custom"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add to Current Block"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents list_cus2 As System.Windows.Forms.ListBox
    Friend WithEvents addbox_remove As System.Windows.Forms.Button
    Friend WithEvents txt_add2 As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents addbox_add As System.Windows.Forms.Button
    Friend WithEvents addbox_done As System.Windows.Forms.Button
    Friend WithEvents addbox_cancel As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents importfile As System.Windows.Forms.OpenFileDialog
    Friend WithEvents import As System.Windows.Forms.Button
End Class
