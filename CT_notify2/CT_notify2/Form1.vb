'    Copyright (c) 2011, 2012 Felix Belzile
'    Official software website: http://getcoldturkey.com
'    Contact: felixbelzile@rogers.com  Web: http://felixbelzile.com

'    This file is part of Cold Turkey
'
'    Cold Turkey is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    Cold Turkey is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with Cold Turkey.  If not, see <http://www.gnu.org/licenses/>.

Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim done As String
        Dim iniFile = New IniFile

        Me.Hide()

        Try
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
            done = iniFile.GetKeyValue("User", "Done")
        Catch ex As Exception
            MsgBox("Error reading my configuration file. Please re-install me.")
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
