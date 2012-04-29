<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class listShowWarning
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(listShowWarning))
        Me.good = New System.Windows.Forms.Button()
        Me.bad = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.full_list = New System.Windows.Forms.ListView()
        Me.SuspendLayout()
        '
        'good
        '
        Me.good.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.good.Location = New System.Drawing.Point(235, 239)
        Me.good.Name = "good"
        Me.good.Size = New System.Drawing.Size(130, 27)
        Me.good.TabIndex = 0
        Me.good.Text = "Go Cold Turkey!"
        Me.good.UseVisualStyleBackColor = True
        '
        'bad
        '
        Me.bad.Location = New System.Drawing.Point(90, 239)
        Me.bad.Name = "bad"
        Me.bad.Size = New System.Drawing.Size(130, 27)
        Me.bad.TabIndex = 1
        Me.bad.Text = "Opps! I made a boo boo"
        Me.bad.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Location = New System.Drawing.Point(59, 53)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(324, 26)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "You will go Cold Turkey from the following sites. Sites in             are " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "comm" & _
            "on accidental blocks that might be needed for work."
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.ForeColor = System.Drawing.Color.Purple
        Me.Label2.Location = New System.Drawing.Point(324, 53)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(36, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "purple"
        '
        'full_list
        '
        Me.full_list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.full_list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.full_list.Location = New System.Drawing.Point(62, 91)
        Me.full_list.MultiSelect = False
        Me.full_list.Name = "full_list"
        Me.full_list.ShowGroups = False
        Me.full_list.Size = New System.Drawing.Size(329, 117)
        Me.full_list.TabIndex = 5
        Me.full_list.UseCompatibleStateImageBehavior = False
        Me.full_list.View = System.Windows.Forms.View.List
        '
        'lastchance
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.ClientSize = New System.Drawing.Size(452, 299)
        Me.Controls.Add(Me.full_list)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.bad)
        Me.Controls.Add(Me.good)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "lastchance"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Last Chance!"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents good As System.Windows.Forms.Button
    Friend WithEvents bad As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents full_list As System.Windows.Forms.ListView
End Class
