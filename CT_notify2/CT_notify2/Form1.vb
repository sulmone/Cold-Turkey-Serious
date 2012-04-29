
Public Class Form1

    Dim iniFile As IniFile

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim done As String

        Me.Hide()
        iniFile = New IniFile

        Try
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        Catch ex As Exception
            MsgBox("Error reading mah configuration file. Please re-install me.")
            End
        End Try

        Try
            done = iniFile.GetKeyValue("User", "Done")
        Catch ex As Exception
            End
        End Try

        If StrComp("yes", done) = 0 Then
            End
        End If
        timerHelper.Start()
    End Sub

    Private Sub timerHelper_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles timerHelper.Elapsed
        Dim processList = System.Diagnostics.Process.GetProcessesByName("ct_notify")
        'Dim running As Boolean
        'For Each proc In processList
        ' Try
        ' If (StrComp(proc.Modules(0).FileName.ToString(), Application.StartupPath + "\ct_notify.exe")) = 0 Then
        ' running = True
        ' End If
        ' Catch ex As Exception
        ' End Try
        ' Next
        'If running = False Then
        Try
            If processList.Length <= 0 Then
                Process.Start(Application.StartupPath + "\ct_notify.exe")
            End If
        Catch ex As Exception
        End Try

    End Sub
End Class
