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

Imports System.IO
Imports ColdTurkey.IniFile

Public Class Add_custom

    Dim list_cus3 As New ListBox
    Dim iniFile As IniFile

    Private Sub Add_custom_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        iniFile = New IniFile
        Try
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        Catch ex As Exception
            MsgBox("Error reading mah configuration file. Please re-install me.")
            End
        End Try

        Try
            Dim reader As String()
            reader = iniFile.GetKeyValue("User", "CustomSites").Split(";")
            For Each entry As String In reader
                If StrComp(entry, "") <> 0 And StrComp(entry, "null") <> 0 Then
                    list_cus3.Items.Add(entry)
                    list_cus2.Items.Add(entry)
                End If
            Next
        Catch ex As Exception
            MsgBox("Error reading mah configuration file. Please re-install me.")
        End Try

    End Sub

    Private Sub addbox_add_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles addbox_add.Click

        Dim addAddress As String = txt_add2.Text.ToString

        If StrComp(addAddress, "") = 0 Then
        ElseIf InStr(addAddress, ".") = 0 Then
            MsgBox("The domain should have an extention (like .com or .net).")
        Else
            If InStr(addAddress, "http://") = 1 Then
                addAddress = Microsoft.VisualBasic.Right(addAddress, addAddress.Length - 7)
            End If
            If InStr(addAddress, "www.") = 1 Then
                addAddress = Microsoft.VisualBasic.Right(addAddress, addAddress.Length - 4)
            End If
            If InStr(addAddress, "/") <> 0 Then
                Dim loc As Integer
                loc = InStr(addAddress, "/")
                addAddress = Microsoft.VisualBasic.Left(addAddress, loc - 1)
            End If
            If list_cus3.Items.Contains(addAddress) Then
                MsgBox("This address is already in the list.")
            Else
                list_cus2.Items.Add(addAddress)
                txt_add2.Text = ""
            End If

        End If
    End Sub


    Private Sub txt_add2_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt_add2.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) Then
            addbox_add_Click(vbNull, System.EventArgs.Empty)
            e.Handled = True
        End If
    End Sub

    Private Sub addbox_remove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles addbox_remove.Click

        Dim selectedIndex As Integer

        selectedIndex = list_cus2.SelectedIndex
        If list_cus3.Items.Contains(list_cus2.SelectedItem) Then
            MsgBox("Haha nice try, this item is currently blocked.")
        Else
            list_cus2.Items.Remove(list_cus2.SelectedItem)
            If selectedIndex <= list_cus2.Items.Count And selectedIndex > 0 Then
                list_cus2.SetSelected(selectedIndex - 1, True)
            ElseIf selectedIndex = 0 And list_cus2.Items.Count > 0 Then
                list_cus2.SetSelected(0, True)
            End If

        End If
    End Sub

    Private Sub addbox_done_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles addbox_done.Click

        Dim anythingAdded As Boolean = False
        Dim newKey As String = ""
        iniFile = New IniFile
        iniFile.Load(Application.StartupPath + "\ct_settings.ini")

        For Each itm As String In list_cus2.Items
            If list_cus3.Items.Contains(itm) Then
            Else
                anythingAdded = True
            End If
        Next
        If anythingAdded = True Then
            For Each entry As String In list_cus2.Items
                newKey = newKey + entry + ";"
            Next
            iniFile.SetKeyValue("User", "CustomSites", newKey)
            iniFile.Save(Application.StartupPath + "\ct_settings.ini")
            Using sw As New IO.StreamWriter(Environ("WinDir") & "\system32\drivers\etc\add_to_hosts", True)
                For Each entry As String In list_cus2.Items
                    If list_cus3.Items.Contains(entry) Then
                    Else
                        sw.WriteLine("0.0.0.0 " + entry)
                        sw.WriteLine("0.0.0.0 www." + entry)
                    End If
                Next
                sw.Close()
            End Using
            Me.Hide()
            MsgBox("Good Job! New sites were added." + vbNewLine + "You might need to restart your browser windows to see the block take effect", MsgBoxStyle.OkOnly, "Cold Turkey")
        Else
            Me.Hide()
            MsgBox("No new items added... Wuss.", MsgBoxStyle.OkOnly, "Cold Turkey")
        End If

        Me.DialogResult = DialogResult.OK

    End Sub

    Private Sub import_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles import.Click

        Dim filepath As String
        Dim newKey As String = ""
        iniFile = New IniFile
        iniFile.Load(Application.StartupPath + "\ct_settings.ini")

        If importfile.ShowDialog() = Windows.Forms.DialogResult.OK Then
            filepath = importfile.FileName

            If File.Exists(filepath) Then

                Try
                    Dim reader As String
                    reader = My.Computer.FileSystem.ReadAllText(filepath)
                    Dim strs() As String
                    strs = Split(reader, Environment.NewLine)
                    For Each line As String In strs
                        If StrComp(line, "") = 0 Then
                        ElseIf InStr(line, ".") = 0 Then
                            MsgBox("This domain should have an extention... like .com or .net:" + vbNewLine + line)
                        Else
                            If StrComp(line, "") <> 0 Then

                                If InStr(line, "http://") = 1 Then
                                    line = Microsoft.VisualBasic.Right(line, line.Length - 7)
                                End If
                                If InStr(line, "www.") = 1 Then
                                    line = Microsoft.VisualBasic.Right(line, line.Length - 4)
                                End If
                                If InStr(line, "/") <> 0 Then
                                    Dim loc As Integer
                                    loc = InStr(line, "/")
                                    line = Microsoft.VisualBasic.Left(line, loc - 1)
                                End If
                                If list_cus2.Items.Contains(line) Then
                                    MsgBox("This address is already in the list." + vbNewLine + line)
                                Else
                                    list_cus2.Items.Add(line)
                                End If
                            End If
                        End If
                    Next
                Catch
                    MsgBox("Error reading file.")
                End Try

            End If
        End If

    End Sub

    Private Sub addbox_cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles addbox_cancel.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

End Class