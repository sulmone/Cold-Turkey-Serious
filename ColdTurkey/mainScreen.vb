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

Option Explicit On
Imports System.IO
Imports System.Security.Cryptography
Imports Microsoft.Win32
Imports Microsoft.VisualBasic
Imports ServiceTools
Imports ColdTurkey.IniFile

Public Class ColdTurkey
    Dim hostDirS As String
    Dim svcAlreadyExists As Boolean = False
    Dim firstTimeClickCustomBoxB As Boolean = True
    Dim hour24I As Integer
    Dim encryptionW As New Simple3Des("ct_textbox")

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim secondsStillBlocked As Integer
        Dim iniFile = New IniFile
        Dim iniFileBroken As Boolean = False
        hostDirS = Environ("WinDir") & "\system32\drivers\etc\hosts"

        If My.Computer.FileSystem.FileExists(Application.StartupPath + "\ct_settings.ini") = True Then
            Try
                iniFile.Load(Application.StartupPath + "\ct_settings.ini")
                If iniFile.Sections.Count < 2 Then
                    iniFileBroken = True
                End If
            Catch ex As Exception
                iniFileBroken = True
            End Try
        Else
            iniFileBroken = True
        End If
        If iniFileBroken = True Then
            Try
                My.Computer.FileSystem.WriteAllText(Application.StartupPath + "\ct_settings.ini", "", False)
                iniFile.AddSection("Process")
                iniFile.SetKeyValue("Process", "List", "null")
                iniFile.AddSection("User")
                iniFile.SetKeyValue("User", "CustomChecked", "abcdefghijk")
                iniFile.SetKeyValue("User", "CustomSites", "null")
                iniFile.SetKeyValue("User", "Done", "yes")
                iniFile.SetKeyValue("User", "NeedsAlerted", "no")
                iniFile.AddSection("Time")
                iniFile.SetKeyValue("Time", "Until", encryptionW.EncryptData(DateTime.Now))
                iniFile.SetKeyValue("Time", "TimeChanging", "no")
                iniFile.AddSection("CurrentTime")
                iniFile.SetKeyValue("CurrentTime", "Now", encryptionW.EncryptData(DateTime.Now))
                iniFile.Save(Application.StartupPath + "\ct_settings.ini")
            Catch ex As Exception
                MsgBox("Error creating configuration file.")
                End
            End Try
        End If

        Try
            If serviceController.Status = ServiceProcess.ServiceControllerStatus.Stopped Or ServiceProcess.ServiceControllerStatus.StopPending Then
                svcAlreadyExists = True
            End If
            If serviceController.Status = ServiceProcess.ServiceControllerStatus.Running Then

                Dim iniDateUntil As DateTime
                Dim answer As MsgBoxResult

                Try
                    DateTime.TryParse(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Until")), iniDateUntil)
                Catch ex As Exception
                    MsgBox("Error reading my configuration file. Please re-install me.")
                    End
                End Try

                secondsStillBlocked = DateDiff(DateInterval.Second, DateTime.Now, iniDateUntil)
                answer = MsgBox("You are still blocked for: " + secondsToText(secondsStillBlocked, False) + vbNewLine + "Do you want to block more sites?", MsgBoxStyle.YesNo, "Cold Turkey")

                If (answer = MsgBoxResult.Yes) Then
                    Me.Hide()
                    Add_custom.ShowDialog()
                    End
                Else
                End If

                End

            End If
        Catch ex As Exception
        End Try

        Try
            Dim reader As String()
            reader = iniFile.GetKeyValue("User", "CustomSites").Split(";")
            For Each entry As String In reader
                If StrComp(entry, "") <> 0 And StrComp(entry, "null") <> 0 Then
                    list_cus.Items.Add(entry)
                End If
            Next
        Catch
        End Try

        Try
            Dim reader As String()
            reader = encryptionW.DecryptData(iniFile.GetKeyValue("Process", "List")).Split(";")
            For Each entry As String In reader
                If StrComp(entry, "") <> 0 And StrComp(entry, "null") <> 0 Then
                    list_prog.Items.Add(entry)
                End If
            Next
        Catch
        End Try

        Try
            Dim reader As String
            reader = iniFile.GetKeyValue("User", "CustomChecked")
            If reader.Contains("a") Then
                chk_facebook.Checked = True
            End If
            If reader.Contains("b") Then
                chk_myspace.Checked = True
            End If
            If reader.Contains("c") Then
                chk_twitter.Checked = True
            End If
            If reader.Contains("d") Then
                chk_agames.Checked = True
            End If
            If reader.Contains("e") Then
                chk_stumble.Checked = True
            End If
            If reader.Contains("f") Then
                chk_collegehumor.Checked = True
            End If
            If reader.Contains("g") Then
                chk_ebay.Checked = True
            End If
            If reader.Contains("h") Then
                chk_failblog.Checked = True
            End If
            If reader.Contains("i") Then
                chk_reddit.Checked = True
            End If
            If reader.Contains("j") Then
                chk_shotmail.Checked = True
            End If
            If reader.Contains("k") Then
                chk_youtube.Checked = True
            End If
            If reader.Contains("l") Then
                chk_wikipedia.Checked = True
            End If
            If reader.Contains("z") Then
                chk_24.Checked = True
            End If
        Catch ex As Exception
        End Try

        importfile.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        addProgramDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.ProgramFiles
        DateTimePicker1.Value = Date.Now
        DateTimePicker1.MinDate = Date.Now
        DateTimePicker1.MaxDate = DateAdd("m", 1, Date.Now)

        If Windows.Forms.Screen.PrimaryScreen.Bounds.Height < 700 Then
            Me.ClientSize = New System.Drawing.Size(451, 530)
            Me.AutoScroll = True
        End If

    End Sub

    Function secondsToText(ByVal Seconds As Integer, ByVal shortVersion As Boolean) As String
        Dim bAddComma As Boolean
        Dim Result As String = ""
        Dim sTemp As String
        Dim days, hours, minutes As Integer

        If Seconds <= 0 Or Not IsNumeric(Seconds) Then
            secondsToText = "0 seconds"
            Exit Function
        End If

        Seconds = Fix(Seconds)

        If Seconds >= 86400 Then
            days = Fix(Seconds / 86400)
        Else
            days = 0
        End If

        If Seconds - (days * 86400) >= 3600 Then
            hours = Fix((Seconds - (days * 86400)) / 3600)
        Else
            hours = 0
        End If

        If Seconds - (hours * 3600) - (days * 86400) >= 60 Then
            minutes = Fix((Seconds - (hours * 3600) - (days * 86400)) / 60)
        Else
            minutes = 0
        End If

        Seconds = Seconds - (minutes * 60) - (hours * 3600) - _
           (days * 86400)

        If shortVersion = False Then
            If Seconds > 0 Then Result = Seconds & " second" & AutoS(Seconds)
        End If

        If minutes > 0 Then
            bAddComma = Result <> ""

            sTemp = minutes & " minute" & AutoS(minutes)
            If bAddComma Then sTemp = sTemp & ", "
            Result = sTemp & Result
        End If

        If hours > 0 Then
            bAddComma = Result <> ""

            sTemp = hours & " hour" & AutoS(hours)
            If bAddComma Then sTemp = sTemp & ", "
            Result = sTemp & Result
        End If

        If days > 0 Then
            bAddComma = Result <> ""
            sTemp = days & " day" & AutoS(days)
            If bAddComma Then sTemp = sTemp & ", "
            Result = sTemp & Result
        End If

        secondsToText = Result
    End Function

    Function AutoS(ByVal Number)
        If Number = 1 Then AutoS = "" Else AutoS = "s"
    End Function

    Private Sub facebookLabelClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If updateStatus.Enabled = True Then
            If updateStatus.Checked = False Then
                updateStatus.Checked = True
            Else
                updateStatus.Checked = False
            End If
        End If
    End Sub


    Private Sub goColdTurkey(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim checkedSettings As String = ""
        Dim entryListShowWarningHotmail, entryListShowWarningLive, entryListShowWarningEbay, _
            entryListShowWarningYoutube, entryListShowWarningWiki As New System.Windows.Forms.ListViewItem
        Dim responseShowWarning As DialogResult

        listShowWarning.full_list.Clear()
        entryListShowWarningHotmail.ForeColor = Color.Purple
        entryListShowWarningLive.ForeColor = Color.Purple
        entryListShowWarningEbay.ForeColor = Color.Purple
        entryListShowWarningYoutube.ForeColor = Color.Purple
        entryListShowWarningWiki.ForeColor = Color.Purple

        Try
            If serviceController.Status = ServiceProcess.ServiceControllerStatus.Running Then
                MsgBox("Please wait until your current ban is lifted.", MsgBoxStyle.Information, "Cold Turkey")
                End
            End If
        Catch f As Exception
        End Try

        If chk_facebook.Checked = True Then
            checkedSettings = checkedSettings + "a"
            listShowWarning.full_list.Items.Add("Facebook")
        End If
        If chk_myspace.Checked = True Then
            checkedSettings = checkedSettings + "b"
            listShowWarning.full_list.Items.Add("Myspace")
        End If
        If chk_twitter.Checked = True Then
            checkedSettings = checkedSettings + "c"
            listShowWarning.full_list.Items.Add("Twitter")
        End If
        If chk_agames.Checked = True Then
            checkedSettings = checkedSettings + "d"
            listShowWarning.full_list.Items.Add("AddictingGames")
        End If
        If chk_stumble.Checked = True Then
            checkedSettings = checkedSettings + "e"
            listShowWarning.full_list.Items.Add("StumbleUpon")
        End If
        If chk_collegehumor.Checked = True Then
            checkedSettings = checkedSettings + "f"
            listShowWarning.full_list.Items.Add("CollegeHumor")
        End If
        If chk_ebay.Checked = True Then
            checkedSettings = checkedSettings + "g"
            entryListShowWarningEbay.Text = "Ebay"
            listShowWarning.full_list.Items.Add(entryListShowWarningEbay)
        End If
        If chk_failblog.Checked = True Then
            checkedSettings = checkedSettings + "h"
            listShowWarning.full_list.Items.Add("FailBlog.org")
        End If
        If chk_reddit.Checked = True Then
            checkedSettings = checkedSettings + "i"
            listShowWarning.full_list.Items.Add("Reddit")
        End If
        If chk_shotmail.Checked = True Then
            checkedSettings = checkedSettings + "j"
            entryListShowWarningHotmail.Text = "Hotmail"
            listShowWarning.full_list.Items.Add(entryListShowWarningHotmail)
            entryListShowWarningLive.Text = "MSN / Windows Live"
            listShowWarning.full_list.Items.Add(entryListShowWarningLive)
        End If
        If chk_youtube.Checked = True Then
            checkedSettings = checkedSettings + "k"
            entryListShowWarningYoutube.Text = "YouTube"
            listShowWarning.full_list.Items.Add(entryListShowWarningYoutube)
        End If
        If chk_wikipedia.Checked = True Then
            checkedSettings = checkedSettings + "l"
            entryListShowWarningWiki.Text = "Wikipedia"
            listShowWarning.full_list.Items.Add(entryListShowWarningWiki)
        End If
        If chk_24.Checked = True Then
            checkedSettings = checkedSettings + "z"
        End If
        Dim iniFile = New IniFile
        iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        iniFile.SetKeyValue("User", "CustomChecked", checkedSettings)
        iniFile.Save(Application.StartupPath + "\ct_settings.ini")
        For Each entry As String In list_cus.Items
            listShowWarning.full_list.Items.Add(entry)
        Next

        If StrComp(checkedSettings, "") = 0 And list_prog.Items.Count = 0 And list_cus.Items.Count = 0 Then
            MsgBox("Common, you need the balls to block at least one thing!")
        Else
            Me.Hide()
            responseShowWarning = listShowWarning.ShowDialog
            If responseShowWarning = DialogResult.OK Then
                If updateStatus.Checked = True Then
                    UpdateFacebookForm.ShowDialog()
                    writeToHostsFile()
                Else
                    writeToHostsFile()
                End If
            Else
                Me.Show()
            End If
        End If

    End Sub

    Private Sub startBlock()

        Dim testWorked As Boolean = False

        testWorked = writeToHostsFileTest()
        If testWorked = True Then
            writeToHostsFile()
            startService()

            If chk_24.Checked = True Then
                MsgBox("You are now going Cold Turkey until " + hour24.Text + ":" + minute.Text + " on " + DateTimePicker1.Text + "." + vbNewLine + vbNewLine _
                    + "You might not see the block until you close and reopen your all browser windows. You can see how much time you have left and add more sites by running Cold Turkey again.", MsgBoxStyle.Information, "Cold Turkey")
            Else
                MsgBox("You are now going Cold Turkey until " + hour.Text + ":" + minute.Text + " " + amPmSelector.Text + " on " + DateTimePicker1.Text + "." + vbNewLine + vbNewLine _
                    + "You might not see the block until you close and reopen your all browser windows. You can see how much time you have left and add more sites by running Cold Turkey again.", MsgBoxStyle.Information, "Cold Turkey")
            End If
        Else
            MsgBox("Error writing to your hosts file. Your antivirus most likely doesn't trust what I will do with it. Please don't disable your antivirus though. You have not been blocked from anything.")
        End If

        End

    End Sub


    Private Function writeToHostsFileTest() As Boolean

        Dim fs As FileStream
        Dim sw As StreamWriter
        Dim iniFile = New IniFile

        If My.Computer.FileSystem.FileExists(hostDirS) Then
            Try
                SetAttr(hostDirS, vbNormal)
            Catch ex As Exception
                Return False
            End Try
            Try
                fs = New FileStream(hostDirS, FileMode.Append, FileAccess.Write, FileShare.Read)
                sw = New StreamWriter(fs)
                sw.WriteLine(vbNewLine + "#### Cold Turkey Entries ####")
                sw.Close()

                Dim fileReader, original As String
                Dim startpos As Integer
                fileReader = My.Computer.FileSystem.ReadAllText(hostDirS)
                startpos = InStr(1, fileReader, "#### Cold Turkey Entries ####")
                If startpos <> 0 And startpos <= 2 Then
                    original = ""
                ElseIf startpos = 0 Then
                    original = fileReader
                Else
                    original = Microsoft.VisualBasic.Left(fileReader, startpos - 1)
                End If
                Dim fs2 As New FileStream(hostDirS, FileMode.Create, FileAccess.Write, FileShare.Read)
                Dim sw2 As New StreamWriter(fs2)
                sw2.Write(original)
                sw2.Close()
                SetAttr(hostDirS, vbReadOnly)
            Catch ex As Exception
                Return False
            End Try
        Else
            Try
                My.Computer.FileSystem.WriteAllText(hostDirS, "", False)
                If My.Computer.FileSystem.FileExists(hostDirS) Then
                    Return True
                End If
            Catch ex As Exception
                Return False
            End Try
        End If

        Return True
    End Function

    Private Sub writeToHostsFile()

        Dim fs As FileStream
        Dim sw As StreamWriter
        Dim iniFile = New IniFile

        If My.Computer.FileSystem.FileExists(hostDirS) Then
            Try
                SetAttr(hostDirS, vbNormal)
            Catch ex As Exception
                MsgBox("Error removing hosts file read-only flag")
                erroredOut()
            End Try

        End If

        Try
            fs = New FileStream(hostDirS, FileMode.Append, FileAccess.Write, FileShare.Read)
            sw = New StreamWriter(fs)
            sw.WriteLine(vbNewLine + "#### Cold Turkey Entries ####")
            If chk_facebook.Checked = True Then
                sw.WriteLine("0.0.0.0 facebook.com" _
                + vbNewLine + "0.0.0.0 www.facebook.com" _
                + vbNewLine + "0.0.0.0 ah8.facebook.com" _
                + vbNewLine + "0.0.0.0 api.connect.facebook.com" _
                + vbNewLine + "0.0.0.0 apps.facebook.com" _
                + vbNewLine + "0.0.0.0 apps.new.facebook.com" _
                + vbNewLine + "0.0.0.0 badge.facebook.com" _
                + vbNewLine + "0.0.0.0 blog.facebook.com" _
                + vbNewLine + "0.0.0.0 ck.connect.facebook.com" _
                + vbNewLine + "0.0.0.0 connect.facebook.com" _
                + vbNewLine + "0.0.0.0 da-dk.facebook.com" _
                + vbNewLine + "0.0.0.0 de-de.facebook.com" _
                + vbNewLine + "0.0.0.0 de.facebook.com" _
                + vbNewLine + "0.0.0.0 depauw.facebook.com" _
                + vbNewLine + "0.0.0.0 developer.facebook.com" _
                + vbNewLine + "0.0.0.0 developers.facebook.com" _
                + vbNewLine + "0.0.0.0 el-gr.facebook.com" _
                + vbNewLine + "0.0.0.0 en-gb.facebook.com" _
                + vbNewLine + "0.0.0.0 en-us.facebook.com" _
                + vbNewLine + "0.0.0.0 es-es.facebook.com" _
                + vbNewLine + "0.0.0.0 es-la.facebook.com" _
                + vbNewLine + "0.0.0.0 fr-fr.facebook.com" _
                + vbNewLine + "0.0.0.0 fr.facebook.com" _
                + vbNewLine + "0.0.0.0 fsu.facebook.com" _
                + vbNewLine + "0.0.0.0 hs.facebook.com" _
                + vbNewLine + "0.0.0.0 hy-am.facebook.com" _
                + vbNewLine + "0.0.0.0 iphone.facebook.com" _
                + vbNewLine + "0.0.0.0 it-it.facebook.com" _
                + vbNewLine + "0.0.0.0 ja-jp.facebook.com" _
                + vbNewLine + "0.0.0.0 lite.facebook.com" _
                + vbNewLine + "0.0.0.0 login.facebook.com" _
                + vbNewLine + "0.0.0.0 m.facebook.com" _
                + vbNewLine + "0.0.0.0 new.facebook.com" _
                + vbNewLine + "0.0.0.0 nn-no.facebook.com" _
                + vbNewLine + "0.0.0.0 northland.facebook.com" _
                + vbNewLine + "0.0.0.0 nyu.facebook.com" _
                + vbNewLine + "0.0.0.0 photos-a.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 photos-b.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 photos-b.ll.facebook.com" _
                + vbNewLine + "0.0.0.0 photos-f.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 photos-g.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 presby.facebook.com" _
                + vbNewLine + "0.0.0.0 profile.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 pt-br.facebook.com" _
                + vbNewLine + "0.0.0.0 pt-pt.facebook.com" _
                + vbNewLine + "0.0.0.0 ru-ru.facebook.com" _
                + vbNewLine + "0.0.0.0 static.ak.connect.facebook.com" _
                + vbNewLine + "0.0.0.0 static.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 static.new.facebook.com" _
                + vbNewLine + "0.0.0.0 sv-se.facebook.com" _
                + vbNewLine + "0.0.0.0 te-in.facebook.com" _
                + vbNewLine + "0.0.0.0 th-th.facebook.com" _
                + vbNewLine + "0.0.0.0 touch.facebook.com" _
                + vbNewLine + "0.0.0.0 tr-tr.facebook.com" _
                + vbNewLine + "0.0.0.0 ufl.facebook.com" _
                + vbNewLine + "0.0.0.0 uillinois.facebook.com" _
                + vbNewLine + "0.0.0.0 uppsalauni.facebook.com" _
                + vbNewLine + "0.0.0.0 video.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 vthumb.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 wiki.developers.facebook.com" _
                + vbNewLine + "0.0.0.0 wm.facebook.com" _
                + vbNewLine + "0.0.0.0 ww.facebook.com" _
                + vbNewLine + "0.0.0.0 zh-cn.facebook.com" _
                + vbNewLine + "0.0.0.0 www.ah8.facebook.com" _
                + vbNewLine + "0.0.0.0 www.api.connect.facebook.com" _
                + vbNewLine + "0.0.0.0 www.apps.facebook.com" _
                + vbNewLine + "0.0.0.0 www.apps.new.facebook.com" _
                + vbNewLine + "0.0.0.0 www.badge.facebook.com" _
                + vbNewLine + "0.0.0.0 www.blog.facebook.com" _
                + vbNewLine + "0.0.0.0 www.ck.connect.facebook.com" _
                + vbNewLine + "0.0.0.0 www.connect.facebook.com" _
                + vbNewLine + "0.0.0.0 www.da-dk.facebook.com" _
                + vbNewLine + "0.0.0.0 www.de-de.facebook.com" _
                + vbNewLine + "0.0.0.0 www.de.facebook.com" _
                + vbNewLine + "0.0.0.0 www.depauw.facebook.com" _
                + vbNewLine + "0.0.0.0 www.developer.facebook.com" _
                + vbNewLine + "0.0.0.0 www.developers.facebook.com" _
                + vbNewLine + "0.0.0.0 www.el-gr.facebook.com" _
                + vbNewLine + "0.0.0.0 www.en-gb.facebook.com" _
                + vbNewLine + "0.0.0.0 www.en-us.facebook.com" _
                + vbNewLine + "0.0.0.0 www.es-es.facebook.com" _
                + vbNewLine + "0.0.0.0 www.es-la.facebook.com" _
                + vbNewLine + "0.0.0.0 www.fr-fr.facebook.com" _
                + vbNewLine + "0.0.0.0 www.fr.facebook.com" _
                + vbNewLine + "0.0.0.0 www.fsu.facebook.com" _
                + vbNewLine + "0.0.0.0 www.hs.facebook.com" _
                + vbNewLine + "0.0.0.0 www.hy-am.facebook.com" _
                + vbNewLine + "0.0.0.0 www.iphone.facebook.com" _
                + vbNewLine + "0.0.0.0 www.it-it.facebook.com" _
                + vbNewLine + "0.0.0.0 www.ja-jp.facebook.com" _
                + vbNewLine + "0.0.0.0 www.lite.facebook.com" _
                + vbNewLine + "0.0.0.0 www.login.facebook.com" _
                + vbNewLine + "0.0.0.0 www.m.facebook.com" _
                + vbNewLine + "0.0.0.0 www.new.facebook.com" _
                + vbNewLine + "0.0.0.0 www.nn-no.facebook.com" _
                + vbNewLine + "0.0.0.0 www.northland.facebook.com" _
                + vbNewLine + "0.0.0.0 www.nyu.facebook.com" _
                + vbNewLine + "0.0.0.0 www.photos-a.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 www.photos-b.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 www.photos-b.ll.facebook.com" _
                + vbNewLine + "0.0.0.0 www.photos-f.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 www.photos-g.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 www.presby.facebook.com" _
                + vbNewLine + "0.0.0.0 www.profile.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 www.pt-br.facebook.com" _
                + vbNewLine + "0.0.0.0 www.pt-pt.facebook.com" _
                + vbNewLine + "0.0.0.0 www.ru-ru.facebook.com" _
                + vbNewLine + "0.0.0.0 www.static.ak.connect.facebook.com" _
                + vbNewLine + "0.0.0.0 www.static.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 www.static.new.facebook.com" _
                + vbNewLine + "0.0.0.0 www.sv-se.facebook.com" _
                + vbNewLine + "0.0.0.0 www.te-in.facebook.com" _
                + vbNewLine + "0.0.0.0 www.th-th.facebook.com" _
                + vbNewLine + "0.0.0.0 www.touch.facebook.com" _
                + vbNewLine + "0.0.0.0 www.tr-tr.facebook.com" _
                + vbNewLine + "0.0.0.0 www.ufl.facebook.com" _
                + vbNewLine + "0.0.0.0 www.uillinois.facebook.com" _
                + vbNewLine + "0.0.0.0 www.uppsalauni.facebook.com" _
                + vbNewLine + "0.0.0.0 www.video.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 www.vthumb.ak.facebook.com" _
                + vbNewLine + "0.0.0.0 www.wiki.developers.facebook.com" _
                + vbNewLine + "0.0.0.0 www.wm.facebook.com" _
                + vbNewLine + "0.0.0.0 www.ww.facebook.com" _
                + vbNewLine + "0.0.0.0 www.zh-cn.facebook.com")

            End If
            If chk_myspace.Checked = True Then
                sw.WriteLine("0.0.0.0 myspace.com" _
                + vbNewLine + "0.0.0.0 www.myspace.com" _
                + vbNewLine + "0.0.0.0 api.myspace.com" _
                + vbNewLine + "0.0.0.0 ar.myspace.com" _
                + vbNewLine + "0.0.0.0 at.myspace.com" _
                + vbNewLine + "0.0.0.0 au.myspace.com" _
                + vbNewLine + "0.0.0.0 b.myspace.com" _
                + vbNewLine + "0.0.0.0 belgie.myspace.com" _
                + vbNewLine + "0.0.0.0 belgique.myspace.com" _
                + vbNewLine + "0.0.0.0 blog.myspace.com" _
                + vbNewLine + "0.0.0.0 blogs.myspace.com" _
                + vbNewLine + "0.0.0.0 br.myspace.com" _
                + vbNewLine + "0.0.0.0 browseusers.myspace.com" _
                + vbNewLine + "0.0.0.0 bulletins.myspace.com" _
                + vbNewLine + "0.0.0.0 ca.myspace.com" _
                + vbNewLine + "0.0.0.0 cf.myspace.com" _
                + vbNewLine + "0.0.0.0 collect.myspace.com" _
                + vbNewLine + "0.0.0.0 comment.myspace.com" _
                + vbNewLine + "0.0.0.0 cp.myspace.com" _
                + vbNewLine + "0.0.0.0 creative-origin.myspace.com" _
                + vbNewLine + "0.0.0.0 de.myspace.com" _
                + vbNewLine + "0.0.0.0 dk.myspace.com" _
                + vbNewLine + "0.0.0.0 es.myspace.com" _
                + vbNewLine + "0.0.0.0 events.myspace.com" _
                + vbNewLine + "0.0.0.0 fi.myspace.com" _
                + vbNewLine + "0.0.0.0 forum.myspace.com" _
                + vbNewLine + "0.0.0.0 forums.myspace.com" _
                + vbNewLine + "0.0.0.0 fr.myspace.com" _
                + vbNewLine + "0.0.0.0 friends.myspace.com" _
                + vbNewLine + "0.0.0.0 groups.myspace.com" _
                + vbNewLine + "0.0.0.0 home.myspace.com" _
                + vbNewLine + "0.0.0.0 ie.myspace.com" _
                + vbNewLine + "0.0.0.0 in.myspace.com" _
                + vbNewLine + "0.0.0.0 invites.myspace.com" _
                + vbNewLine + "0.0.0.0 it.myspace.com" _
                + vbNewLine + "0.0.0.0 jobs.myspace.com" _
                + vbNewLine + "0.0.0.0 jp.myspace.com" _
                + vbNewLine + "0.0.0.0 ksolo.myspace.com" _
                + vbNewLine + "0.0.0.0 la.myspace.com" _
                + vbNewLine + "0.0.0.0 lads.myspace.com" _
                + vbNewLine + "0.0.0.0 latino.myspace.com" _
                + vbNewLine + "0.0.0.0 m.myspace.com" _
                + vbNewLine + "0.0.0.0 mediaservices.myspace.com" _
                + vbNewLine + "0.0.0.0 messaging.myspace.com" _
                + vbNewLine + "0.0.0.0 music.myspace.com" _
                + vbNewLine + "0.0.0.0 mx.myspace.com" _
                + vbNewLine + "0.0.0.0 nl.myspace.com" _
                + vbNewLine + "0.0.0.0 no.myspace.com" _
                + vbNewLine + "0.0.0.0 nz.myspace.com" _
                + vbNewLine + "0.0.0.0 pl.myspace.com" _
                + vbNewLine + "0.0.0.0 profile.myspace.com" _
                + vbNewLine + "0.0.0.0 profileedit.myspace.com" _
                + vbNewLine + "0.0.0.0 pt.myspace.com" _
                + vbNewLine + "0.0.0.0 ru.myspace.com" _
                + vbNewLine + "0.0.0.0 school.myspace.com" _
                + vbNewLine + "0.0.0.0 schweiz.myspace.com" _
                + vbNewLine + "0.0.0.0 se.myspace.com" _
                + vbNewLine + "0.0.0.0 searchservice.myspace.com" _
                + vbNewLine + "0.0.0.0 secure.myspace.com" _
                + vbNewLine + "0.0.0.0 signups.myspace.com" _
                + vbNewLine + "0.0.0.0 suisse.myspace.com" _
                + vbNewLine + "0.0.0.0 svizzera.myspace.com" _
                + vbNewLine + "0.0.0.0 tr.myspace.com" _
                + vbNewLine + "0.0.0.0 uk.myspace.com" _
                + vbNewLine + "0.0.0.0 us.myspace.com" _
                + vbNewLine + "0.0.0.0 vids.myspace.com" _
                + vbNewLine + "0.0.0.0 viewmorepics.myspace.com" _
                + vbNewLine + "0.0.0.0 zh.myspace.com" _
                + vbNewLine + "0.0.0.0 www.api.myspace.com" _
                + vbNewLine + "0.0.0.0 www.ar.myspace.com" _
                + vbNewLine + "0.0.0.0 www.at.myspace.com" _
                + vbNewLine + "0.0.0.0 www.au.myspace.com" _
                + vbNewLine + "0.0.0.0 www.b.myspace.com" _
                + vbNewLine + "0.0.0.0 www.belgie.myspace.com" _
                + vbNewLine + "0.0.0.0 www.belgique.myspace.com" _
                + vbNewLine + "0.0.0.0 www.blog.myspace.com" _
                + vbNewLine + "0.0.0.0 www.blogs.myspace.com" _
                + vbNewLine + "0.0.0.0 www.br.myspace.com" _
                + vbNewLine + "0.0.0.0 www.browseusers.myspace.com" _
                + vbNewLine + "0.0.0.0 www.bulletins.myspace.com" _
                + vbNewLine + "0.0.0.0 www.ca.myspace.com" _
                + vbNewLine + "0.0.0.0 www.cf.myspace.com" _
                + vbNewLine + "0.0.0.0 www.collect.myspace.com" _
                + vbNewLine + "0.0.0.0 www.comment.myspace.com" _
                + vbNewLine + "0.0.0.0 www.cp.myspace.com" _
                + vbNewLine + "0.0.0.0 www.creative-origin.myspace.com" _
                + vbNewLine + "0.0.0.0 www.de.myspace.com" _
                + vbNewLine + "0.0.0.0 www.dk.myspace.com" _
                + vbNewLine + "0.0.0.0 www.es.myspace.com" _
                + vbNewLine + "0.0.0.0 www.events.myspace.com" _
                + vbNewLine + "0.0.0.0 www.fi.myspace.com" _
                + vbNewLine + "0.0.0.0 www.forum.myspace.com" _
                + vbNewLine + "0.0.0.0 www.forums.myspace.com" _
                + vbNewLine + "0.0.0.0 www.fr.myspace.com" _
                + vbNewLine + "0.0.0.0 www.friends.myspace.com" _
                + vbNewLine + "0.0.0.0 www.groups.myspace.com" _
                + vbNewLine + "0.0.0.0 www.home.myspace.com" _
                + vbNewLine + "0.0.0.0 www.ie.myspace.com" _
                + vbNewLine + "0.0.0.0 www.in.myspace.com" _
                + vbNewLine + "0.0.0.0 www.invites.myspace.com" _
                + vbNewLine + "0.0.0.0 www.it.myspace.com" _
                + vbNewLine + "0.0.0.0 www.jobs.myspace.com" _
                + vbNewLine + "0.0.0.0 www.jp.myspace.com" _
                + vbNewLine + "0.0.0.0 www.ksolo.myspace.com" _
                + vbNewLine + "0.0.0.0 www.la.myspace.com" _
                + vbNewLine + "0.0.0.0 www.lads.myspace.com" _
                + vbNewLine + "0.0.0.0 www.latino.myspace.com" _
                + vbNewLine + "0.0.0.0 www.m.myspace.com" _
                + vbNewLine + "0.0.0.0 www.mediaservices.myspace.com" _
                + vbNewLine + "0.0.0.0 www.messaging.myspace.com" _
                + vbNewLine + "0.0.0.0 www.music.myspace.com" _
                + vbNewLine + "0.0.0.0 www.mx.myspace.com" _
                + vbNewLine + "0.0.0.0 www.nl.myspace.com" _
                + vbNewLine + "0.0.0.0 www.no.myspace.com" _
                + vbNewLine + "0.0.0.0 www.nz.myspace.com" _
                + vbNewLine + "0.0.0.0 www.pl.myspace.com" _
                + vbNewLine + "0.0.0.0 www.profile.myspace.com" _
                + vbNewLine + "0.0.0.0 www.profileedit.myspace.com" _
                + vbNewLine + "0.0.0.0 www.pt.myspace.com" _
                + vbNewLine + "0.0.0.0 www.ru.myspace.com" _
                + vbNewLine + "0.0.0.0 www.school.myspace.com" _
                + vbNewLine + "0.0.0.0 www.schweiz.myspace.com" _
                + vbNewLine + "0.0.0.0 www.se.myspace.com" _
                + vbNewLine + "0.0.0.0 www.searchservice.myspace.com" _
                + vbNewLine + "0.0.0.0 www.secure.myspace.com" _
                + vbNewLine + "0.0.0.0 www.signups.myspace.com" _
                + vbNewLine + "0.0.0.0 www.suisse.myspace.com" _
                + vbNewLine + "0.0.0.0 www.svizzera.myspace.com" _
                + vbNewLine + "0.0.0.0 www.tr.myspace.com" _
                + vbNewLine + "0.0.0.0 www.uk.myspace.com" _
                + vbNewLine + "0.0.0.0 www.us.myspace.com" _
                + vbNewLine + "0.0.0.0 www.vids.myspace.com" _
                + vbNewLine + "0.0.0.0 www.viewmorepics.myspace.com" _
                + vbNewLine + "0.0.0.0 www.zh.myspace.com")
            End If
            If chk_twitter.Checked = True Then
                sw.WriteLine("0.0.0.0 twitter.com" _
                + vbNewLine + "0.0.0.0 www.twitter.com" _
                + vbNewLine + "0.0.0.0 apiwiki.twitter.com" _
                + vbNewLine + "0.0.0.0 assets0.twitter.com" _
                + vbNewLine + "0.0.0.0 assets3.twitter.com" _
                + vbNewLine + "0.0.0.0 blog.fr.twitter.com" _
                + vbNewLine + "0.0.0.0 blog.twitter.com" _
                + vbNewLine + "0.0.0.0 business.twitter.com" _
                + vbNewLine + "0.0.0.0 chirp.twitter.com" _
                + vbNewLine + "0.0.0.0 dev.twitter.com" _
                + vbNewLine + "0.0.0.0 explore.twitter.com" _
                + vbNewLine + "0.0.0.0 help.twitter.com" _
                + vbNewLine + "0.0.0.0 integratedsearch.twitter.com" _
                + vbNewLine + "0.0.0.0 m.twitter.com" _
                + vbNewLine + "0.0.0.0 mobile.twitter.com" _
                + vbNewLine + "0.0.0.0 mobile.blog.twitter.com" _
                + vbNewLine + "0.0.0.0 platform.twitter.com" _
                + vbNewLine + "0.0.0.0 search.twitter.com" _
                + vbNewLine + "0.0.0.0 static.twitter.com" _
                + vbNewLine + "0.0.0.0 status.twitter.com" _
                + vbNewLine + "0.0.0.0 www.apiwiki.twitter.com" _
                + vbNewLine + "0.0.0.0 www.assets0.twitter.com" _
                + vbNewLine + "0.0.0.0 www.assets3.twitter.com" _
                + vbNewLine + "0.0.0.0 www.blog.fr.twitter.com" _
                + vbNewLine + "0.0.0.0 www.blog.twitter.com" _
                + vbNewLine + "0.0.0.0 www.business.twitter.com" _
                + vbNewLine + "0.0.0.0 www.chirp.twitter.com" _
                + vbNewLine + "0.0.0.0 www.dev.twitter.com" _
                + vbNewLine + "0.0.0.0 www.explore.twitter.com" _
                + vbNewLine + "0.0.0.0 www.help.twitter.com" _
                + vbNewLine + "0.0.0.0 www.integratedsearch.twitter.com" _
                + vbNewLine + "0.0.0.0 www.m.twitter.com" _
                + vbNewLine + "0.0.0.0 www.mobile.twitter.com" _
                + vbNewLine + "0.0.0.0 www.mobile.blog.twitter.com" _
                + vbNewLine + "0.0.0.0 www.platform.twitter.com" _
                + vbNewLine + "0.0.0.0 www.search.twitter.com" _
                + vbNewLine + "0.0.0.0 www.static.twitter.com" _
                + vbNewLine + "0.0.0.0 www.status.twitter.com")
            End If
            If chk_youtube.Checked = True Then
                sw.WriteLine("0.0.0.0 youtube.com" _
                + vbNewLine + "0.0.0.0 www.youtube.com" _
                + vbNewLine + "0.0.0.0 apiblog.youtube.com" _
                + vbNewLine + "0.0.0.0 au.youtube.com" _
                + vbNewLine + "0.0.0.0 ca.youtube.com" _
                + vbNewLine + "0.0.0.0 de.youtube.com" _
                + vbNewLine + "0.0.0.0 es.youtube.com" _
                + vbNewLine + "0.0.0.0 fr.youtube.com" _
                + vbNewLine + "0.0.0.0 gdata.youtube.com" _
                + vbNewLine + "0.0.0.0 help.youtube.com" _
                + vbNewLine + "0.0.0.0 hk.youtube.com" _
                + vbNewLine + "0.0.0.0 img.youtube.com" _
                + vbNewLine + "0.0.0.0 in.youtube.com" _
                + vbNewLine + "0.0.0.0 it.youtube.com" _
                + vbNewLine + "0.0.0.0 jp.youtube.com" _
                + vbNewLine + "0.0.0.0 m.youtube.com" _
                + vbNewLine + "0.0.0.0 nl.youtube.com" _
                + vbNewLine + "0.0.0.0 nz.youtube.com" _
                + vbNewLine + "0.0.0.0 sjc-static7.sjc.youtube.com" _
                + vbNewLine + "0.0.0.0 sjl-static16.sjl.youtube.com" _
                + vbNewLine + "0.0.0.0 uk.youtube.com" _
                + vbNewLine + "0.0.0.0 upload.youtube.com" _
                + vbNewLine + "0.0.0.0 web1.nyc.youtube.com" _
                + vbNewLine + "0.0.0.0 www.apiblog.youtube.com" _
                + vbNewLine + "0.0.0.0 www.au.youtube.com" _
                + vbNewLine + "0.0.0.0 www.ca.youtube.com" _
                + vbNewLine + "0.0.0.0 www.de.youtube.com" _
                + vbNewLine + "0.0.0.0 www.es.youtube.com" _
                + vbNewLine + "0.0.0.0 www.fr.youtube.com" _
                + vbNewLine + "0.0.0.0 www.gdata.youtube.com" _
                + vbNewLine + "0.0.0.0 www.help.youtube.com" _
                + vbNewLine + "0.0.0.0 www.hk.youtube.com" _
                + vbNewLine + "0.0.0.0 www.img.youtube.com" _
                + vbNewLine + "0.0.0.0 www.in.youtube.com" _
                + vbNewLine + "0.0.0.0 www.it.youtube.com" _
                + vbNewLine + "0.0.0.0 www.jp.youtube.com" _
                + vbNewLine + "0.0.0.0 www.m.youtube.com" _
                + vbNewLine + "0.0.0.0 www.nl.youtube.com" _
                + vbNewLine + "0.0.0.0 www.nz.youtube.com" _
                + vbNewLine + "0.0.0.0 www.sjc-static7.sjc.youtube.com" _
                + vbNewLine + "0.0.0.0 www.sjl-static16.sjl.youtube.com" _
                + vbNewLine + "0.0.0.0 www.uk.youtube.com" _
                + vbNewLine + "0.0.0.0 www.upload.youtube.com" _
                + vbNewLine + "0.0.0.0 www.web1.nyc.youtube.com")

            End If
            If chk_shotmail.Checked = True Then
                sw.WriteLine("0.0.0.0 live.com" _
                + vbNewLine + "0.0.0.0 www.live.com" _
                + vbNewLine + "0.0.0.0 webim.live.sg" _
                + vbNewLine + "0.0.0.0 www.webim.live.sg" _
                + vbNewLine + "0.0.0.0 e-messenger.net" _
                + vbNewLine + "0.0.0.0 www.e-messenger.net" _
                + vbNewLine + "0.0.0.0 iloveim.com" _
                + vbNewLine + "0.0.0.0 www.iloveim.com" _
                + vbNewLine + "0.0.0.0 meebo.com" _
                + vbNewLine + "0.0.0.0 www.meebo.com" _
                + vbNewLine + "0.0.0.0 messengerfx.com" _
                + vbNewLine + "0.0.0.0 www.messengerfx.com" _
                + vbNewLine + "0.0.0.0 ebuddy.com" _
                + vbNewLine + "0.0.0.0 www.ebuddy.com" _
                + vbNewLine + "0.0.0.0 web-messenger.eu" _
                + vbNewLine + "0.0.0.0 www.web-messenger.eu" _
                + vbNewLine + "0.0.0.0 msn2go.com" _
                + vbNewLine + "0.0.0.0 www.msn2go.com" _
                + vbNewLine + "0.0.0.0 f1messenger.com" _
                + vbNewLine + "0.0.0.0 www.f1messenger.com" _
                + vbNewLine + "0.0.0.0 favorites.live.com" _
                + vbNewLine + "0.0.0.0 g.live.com" _
                + vbNewLine + "0.0.0.0 gallery.live.com" _
                + vbNewLine + "0.0.0.0 get.live.com" _
                + vbNewLine + "0.0.0.0 gfx2.mail.live.com" _
                + vbNewLine + "0.0.0.0 groups.live.com" _
                + vbNewLine + "0.0.0.0 home.live.com" _
                + vbNewLine + "0.0.0.0 home.spaces.live.com" _
                + vbNewLine + "0.0.0.0 hotmail.live.com" _
                + vbNewLine + "0.0.0.0 ideas.live.com" _
                + vbNewLine + "0.0.0.0 im.live.com" _
                + vbNewLine + "0.0.0.0 images.domains.live.com" _
                + vbNewLine + "0.0.0.0 intl.local.live.com" _
                + vbNewLine + "0.0.0.0 local.live.com" _
                + vbNewLine + "0.0.0.0 localsearch.live.com" _
                + vbNewLine + "0.0.0.0 login.live.com" _
                + vbNewLine + "0.0.0.0 lsrvsc.spaces.live.com" _
                + vbNewLine + "0.0.0.0 mail.live.com" _
                + vbNewLine + "0.0.0.0 messenger.live.com" _
                + vbNewLine + "0.0.0.0 messenger.services.live.com" _
                + vbNewLine + "0.0.0.0 mobile.live.com" _
                + vbNewLine + "0.0.0.0 my.live.com" _
                + vbNewLine + "0.0.0.0 people.live.com" _
                + vbNewLine + "0.0.0.0 photo.live.com" _
                + vbNewLine + "0.0.0.0 profile.live.com" _
                + vbNewLine + "0.0.0.0 qna.live.com" _
                + vbNewLine + "0.0.0.0 settings.messenger.live.com" _
                + vbNewLine + "0.0.0.0 shared.live.com" _
                + vbNewLine + "0.0.0.0 sn103w.snt103.mail.live.com" _
                + vbNewLine + "0.0.0.0 sn105w.snt105.mail.live.com" _
                + vbNewLine + "0.0.0.0 sn110w.snt110.mail.live.com" _
                + vbNewLine + "0.0.0.0 spaces.live.com" _
                + vbNewLine + "0.0.0.0 tou.live.com" _
                + vbNewLine + "0.0.0.0 www.favorites.live.com" _
                + vbNewLine + "0.0.0.0 www.g.live.com" _
                + vbNewLine + "0.0.0.0 www.gallery.live.com" _
                + vbNewLine + "0.0.0.0 www.get.live.com" _
                + vbNewLine + "0.0.0.0 www.gfx2.mail.live.com" _
                + vbNewLine + "0.0.0.0 www.groups.live.com" _
                + vbNewLine + "0.0.0.0 www.home.live.com" _
                + vbNewLine + "0.0.0.0 www.home.spaces.live.com" _
                + vbNewLine + "0.0.0.0 www.hotmail.live.com" _
                + vbNewLine + "0.0.0.0 www.ideas.live.com" _
                + vbNewLine + "0.0.0.0 www.im.live.com" _
                + vbNewLine + "0.0.0.0 www.images.domains.live.com" _
                + vbNewLine + "0.0.0.0 www.intl.local.live.com" _
                + vbNewLine + "0.0.0.0 www.local.live.com" _
                + vbNewLine + "0.0.0.0 www.localsearch.live.com" _
                + vbNewLine + "0.0.0.0 www.login.live.com" _
                + vbNewLine + "0.0.0.0 www.lsrvsc.spaces.live.com" _
                + vbNewLine + "0.0.0.0 www.mail.live.com" _
                + vbNewLine + "0.0.0.0 www.messenger.live.com" _
                + vbNewLine + "0.0.0.0 www.messenger.services.live.com" _
                + vbNewLine + "0.0.0.0 www.mobile.live.com" _
                + vbNewLine + "0.0.0.0 www.my.live.com" _
                + vbNewLine + "0.0.0.0 www.people.live.com" _
                + vbNewLine + "0.0.0.0 www.photo.live.com" _
                + vbNewLine + "0.0.0.0 www.profile.live.com" _
                + vbNewLine + "0.0.0.0 www.qna.live.com" _
                + vbNewLine + "0.0.0.0 www.settings.messenger.live.com" _
                + vbNewLine + "0.0.0.0 www.shared.live.com" _
                + vbNewLine + "0.0.0.0 www.sn103w.snt103.mail.live.com" _
                + vbNewLine + "0.0.0.0 www.sn105w.snt105.mail.live.com" _
                + vbNewLine + "0.0.0.0 www.sn110w.snt110.mail.live.com" _
                + vbNewLine + "0.0.0.0 www.spaces.live.com" _
                + vbNewLine + "0.0.0.0 www.tou.live.com")
            End If
            If chk_agames.Checked = True Then
                sw.WriteLine("0.0.0.0 addictinggames.com" _
                + vbNewLine + "0.0.0.0 www.addictinggames.com")
            End If
            If chk_collegehumor.Checked = True Then
                sw.WriteLine("0.0.0.0 collegehumor.com" _
                + vbNewLine + "0.0.0.0 www.collegehumor.com")
            End If
            If chk_stumble.Checked = True Then
                sw.WriteLine("0.0.0.0 stumbleupon.com" _
                + vbNewLine + "0.0.0.0 www.stumbleupon.com")
            End If
            If chk_ebay.Checked = True Then
                sw.WriteLine("0.0.0.0 ebay.com" _
                + vbNewLine + "0.0.0.0 ebay.co.uk" _
                + vbNewLine + "0.0.0.0 benl.ebay.be" _
                + vbNewLine + "0.0.0.0 ebay.ca" _
                + vbNewLine + "0.0.0.0 ebay.cn" _
                + vbNewLine + "0.0.0.0 ebay.com.au" _
                + vbNewLine + "0.0.0.0 ebay.com.cn" _
                + vbNewLine + "0.0.0.0 ebay.de" _
                + vbNewLine + "0.0.0.0 ebay.es" _
                + vbNewLine + "0.0.0.0 ebay.fr" _
                + vbNewLine + "0.0.0.0 ebay.ie" _
                + vbNewLine + "0.0.0.0 ebay.in" _
                + vbNewLine + "0.0.0.0 ebay.it" _
                + vbNewLine + "0.0.0.0 ebay.nl" _
                + vbNewLine + "0.0.0.0 ebay.pl" _
                + vbNewLine + "0.0.0.0 ebay.ph" _
                + vbNewLine + "0.0.0.0 ebay.jobs" _
                + vbNewLine + "0.0.0.0 cgi.ebay.com" _
                + vbNewLine + "0.0.0.0 cgi.ebay.co.uk" _
                + vbNewLine + "0.0.0.0 cgi.benl.ebay.be" _
                + vbNewLine + "0.0.0.0 cgi.ebay.ca" _
                + vbNewLine + "0.0.0.0 cgi.ebay.cn" _
                + vbNewLine + "0.0.0.0 cgi.ebay.com.au" _
                + vbNewLine + "0.0.0.0 cgi.ebay.com.cn" _
                + vbNewLine + "0.0.0.0 cgi.ebay.de" _
                + vbNewLine + "0.0.0.0 cgi.ebay.es" _
                + vbNewLine + "0.0.0.0 cgi.ebay.fr" _
                + vbNewLine + "0.0.0.0 cgi.ebay.ie" _
                + vbNewLine + "0.0.0.0 cgi.ebay.in" _
                + vbNewLine + "0.0.0.0 cgi.ebay.it" _
                + vbNewLine + "0.0.0.0 cgi.ebay.nl" _
                + vbNewLine + "0.0.0.0 cgi.ebay.pl" _
                + vbNewLine + "0.0.0.0 cgi.ebay.ph" _
                + vbNewLine + "0.0.0.0 cgi.ebay.jobs" _
                + vbNewLine + "0.0.0.0 www.ebay.com" _
                + vbNewLine + "0.0.0.0 www.ebay.co.uk" _
                + vbNewLine + "0.0.0.0 www.benl.ebay.be" _
                + vbNewLine + "0.0.0.0 www.ebay.ca" _
                + vbNewLine + "0.0.0.0 www.ebay.cn" _
                + vbNewLine + "0.0.0.0 www.ebay.com.au" _
                + vbNewLine + "0.0.0.0 www.ebay.com.cn" _
                + vbNewLine + "0.0.0.0 www.ebay.de" _
                + vbNewLine + "0.0.0.0 www.ebay.es" _
                + vbNewLine + "0.0.0.0 www.ebay.fr" _
                + vbNewLine + "0.0.0.0 www.ebay.ie" _
                + vbNewLine + "0.0.0.0 www.ebay.in" _
                + vbNewLine + "0.0.0.0 www.ebay.it" _
                + vbNewLine + "0.0.0.0 www.ebay.nl" _
                + vbNewLine + "0.0.0.0 www.ebay.pl" _
                + vbNewLine + "0.0.0.0 www.ebay.ph" _
                + vbNewLine + "0.0.0.0 www.ebay.jobs" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.com" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.co.uk" _
                + vbNewLine + "0.0.0.0 www.cgi.benl.ebay.be" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.ca" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.cn" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.com.au" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.com.cn" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.de" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.es" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.fr" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.ie" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.in" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.it" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.nl" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.pl" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.ph" _
                + vbNewLine + "0.0.0.0 www.cgi.ebay.jobs")
            End If
            If chk_failblog.Checked = True Then
                sw.WriteLine("0.0.0.0 failblog.org" _
                + vbNewLine + "0.0.0.0 www.failblog.org")
            End If
            If chk_reddit.Checked = True Then
                sw.WriteLine("0.0.0.0 reddit.com" _
                + vbNewLine + "0.0.0.0 www.reddit.com")
            End If
            If chk_wikipedia.Checked = True Then
                sw.WriteLine("0.0.0.0 wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ab.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ace.wikipedia.org" _
                + vbNewLine + "0.0.0.0 af.wikipedia.org" _
                + vbNewLine + "0.0.0.0 als.wikipedia.org" _
                + vbNewLine + "0.0.0.0 am.wikipedia.org" _
                + vbNewLine + "0.0.0.0 an.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ang.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ar.wikipedia.org" _
                + vbNewLine + "0.0.0.0 arc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 arz.wikipedia.org" _
                + vbNewLine + "0.0.0.0 as.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ast.wikipedia.org" _
                + vbNewLine + "0.0.0.0 av.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ay.wikipedia.org" _
                + vbNewLine + "0.0.0.0 az.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ba.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bar.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bat-smg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bcl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 be.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bpy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 br.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bs.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bug.wikipedia.org" _
                + vbNewLine + "0.0.0.0 bxr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ca.wikipedia.org" _
                + vbNewLine + "0.0.0.0 cbk-zam.wikipedia.org" _
                + vbNewLine + "0.0.0.0 cdo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ce.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ceb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ch.wikipedia.org" _
                + vbNewLine + "0.0.0.0 chr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ckb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 co.wikipedia.org" _
                + vbNewLine + "0.0.0.0 commons.wikipedia.org" _
                + vbNewLine + "0.0.0.0 cr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 crh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 cs.wikipedia.org" _
                + vbNewLine + "0.0.0.0 csb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 cu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 cv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 cy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 da.wikipedia.org" _
                + vbNewLine + "0.0.0.0 de.wikipedia.org" _
                + vbNewLine + "0.0.0.0 diq.wikipedia.org" _
                + vbNewLine + "0.0.0.0 dsb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 dv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 dz.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ee.wikipedia.org" _
                + vbNewLine + "0.0.0.0 el.wikipedia.org" _
                + vbNewLine + "0.0.0.0 eml.wikipedia.org" _
                + vbNewLine + "0.0.0.0 en.wikipedia.org" _
                + vbNewLine + "0.0.0.0 eo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 es.wikipedia.org" _
                + vbNewLine + "0.0.0.0 et.wikipedia.org" _
                + vbNewLine + "0.0.0.0 eu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ext.wikipedia.org" _
                + vbNewLine + "0.0.0.0 fa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 fi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 fiu-vro.wikipedia.org" _
                + vbNewLine + "0.0.0.0 fo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 foundation.wikipedia.org" _
                + vbNewLine + "0.0.0.0 fr.m.wikipedia.org" _
                + vbNewLine + "0.0.0.0 fr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 frp.wikipedia.org" _
                + vbNewLine + "0.0.0.0 fur.wikipedia.org" _
                + vbNewLine + "0.0.0.0 fy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ga.wikipedia.org" _
                + vbNewLine + "0.0.0.0 gan.wikipedia.org" _
                + vbNewLine + "0.0.0.0 gd.wikipedia.org" _
                + vbNewLine + "0.0.0.0 gl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 glk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 gn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 got.wikipedia.org" _
                + vbNewLine + "0.0.0.0 gu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 gv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ha.wikipedia.org" _
                + vbNewLine + "0.0.0.0 hak.wikipedia.org" _
                + vbNewLine + "0.0.0.0 haw.wikipedia.org" _
                + vbNewLine + "0.0.0.0 he.wikipedia.org" _
                + vbNewLine + "0.0.0.0 hi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 hif.wikipedia.org" _
                + vbNewLine + "0.0.0.0 hr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 hsb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ht.wikipedia.org" _
                + vbNewLine + "0.0.0.0 hu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 hy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ia.wikipedia.org" _
                + vbNewLine + "0.0.0.0 id.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ie.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ig.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ik.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ilo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 io.wikipedia.org" _
                + vbNewLine + "0.0.0.0 is.wikipedia.org" _
                + vbNewLine + "0.0.0.0 it.wikipedia.org" _
                + vbNewLine + "0.0.0.0 iu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ja.wikipedia.org" _
                + vbNewLine + "0.0.0.0 jbo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 jv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ka.wikipedia.org" _
                + vbNewLine + "0.0.0.0 kaa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 kab.wikipedia.org" _
                + vbNewLine + "0.0.0.0 kg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ki.wikipedia.org" _
                + vbNewLine + "0.0.0.0 kk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 kl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 km.wikipedia.org" _
                + vbNewLine + "0.0.0.0 kn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ko.wikipedia.org" _
                + vbNewLine + "0.0.0.0 krc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ks.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ksh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ku.wikipedia.org" _
                + vbNewLine + "0.0.0.0 kv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 kw.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ky.wikipedia.org" _
                + vbNewLine + "0.0.0.0 la.wikipedia.org" _
                + vbNewLine + "0.0.0.0 lad.wikipedia.org" _
                + vbNewLine + "0.0.0.0 lb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 li.wikipedia.org" _
                + vbNewLine + "0.0.0.0 lij.wikipedia.org" _
                + vbNewLine + "0.0.0.0 lmo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ln.wikipedia.org" _
                + vbNewLine + "0.0.0.0 lo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 lt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 lv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 map-bms.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mdf.wikipedia.org" _
                + vbNewLine + "0.0.0.0 meta.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mhr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ml.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ms.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mwl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 my.wikipedia.org" _
                + vbNewLine + "0.0.0.0 myv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 mzn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 na.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nah.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nap.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nds-nl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nds.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ne.wikipedia.org" _
                + vbNewLine + "0.0.0.0 new.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 no.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nov.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nrm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 nv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 oc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 om.wikipedia.org" _
                + vbNewLine + "0.0.0.0 or.wikipedia.org" _
                + vbNewLine + "0.0.0.0 os.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pag.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pam.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pap.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pcd.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pdc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pih.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pms.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pnb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pnt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ps.wikipedia.org" _
                + vbNewLine + "0.0.0.0 pt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 qu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 rm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 rmy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ro.wikipedia.org" _
                + vbNewLine + "0.0.0.0 roa-rup.wikipedia.org" _
                + vbNewLine + "0.0.0.0 roa-tara.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ru.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sah.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 scn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sco.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sd.wikipedia.org" _
                + vbNewLine + "0.0.0.0 se.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 si.wikipedia.org" _
                + vbNewLine + "0.0.0.0 simple.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 so.wikipedia.org" _
                + vbNewLine + "0.0.0.0 species.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sq.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 srn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ss.wikipedia.org" _
                + vbNewLine + "0.0.0.0 stq.wikipedia.org" _
                + vbNewLine + "0.0.0.0 su.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 sw.wikipedia.org" _
                + vbNewLine + "0.0.0.0 szl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ta.wikipedia.org" _
                + vbNewLine + "0.0.0.0 te.wikipedia.org" _
                + vbNewLine + "0.0.0.0 tet.wikipedia.org" _
                + vbNewLine + "0.0.0.0 tg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 th.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ti.wikipedia.org" _
                + vbNewLine + "0.0.0.0 tk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 tl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 tn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 to.wikipedia.org" _
                + vbNewLine + "0.0.0.0 tpi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 tr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ts.wikipedia.org" _
                + vbNewLine + "0.0.0.0 tt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ty.wikipedia.org" _
                + vbNewLine + "0.0.0.0 udm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ug.wikipedia.org" _
                + vbNewLine + "0.0.0.0 uk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ur.wikipedia.org" _
                + vbNewLine + "0.0.0.0 uz.wikipedia.org" _
                + vbNewLine + "0.0.0.0 ve.wikipedia.org" _
                + vbNewLine + "0.0.0.0 vec.wikipedia.org" _
                + vbNewLine + "0.0.0.0 vi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 vls.wikipedia.org" _
                + vbNewLine + "0.0.0.0 vo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 wa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 war.wikipedia.org" _
                + vbNewLine + "0.0.0.0 wo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 wuu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 xal.wikipedia.org" _
                + vbNewLine + "0.0.0.0 xh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 yi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 yo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 za.wikipedia.org" _
                + vbNewLine + "0.0.0.0 zea.wikipedia.org" _
                + vbNewLine + "0.0.0.0 zh-classical.wikipedia.org" _
                + vbNewLine + "0.0.0.0 zh-min-nan.wikipedia.org" _
                + vbNewLine + "0.0.0.0 zh-yue.wikipedia.org" _
                + vbNewLine + "0.0.0.0 zh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 zu.wikipedia.org")
                sw.WriteLine("0.0.0.0 www.ab.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ace.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.af.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.als.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.am.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.an.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ang.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ar.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.arc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.arz.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.as.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ast.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.av.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ay.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.az.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ba.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bar.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bat-smg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bcl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.be.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bpy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.br.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bs.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bug.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.bxr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ca.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.cbk-zam.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.cdo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ce.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ceb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ch.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.chr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ckb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.co.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.commons.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.cr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.crh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.cs.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.csb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.cu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.cv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.cy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.da.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.de.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.diq.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.dsb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.dv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.dz.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ee.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.el.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.eml.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.en.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.eo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.es.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.et.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.eu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ext.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.fa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.fi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.fiu-vro.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.fo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.foundation.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.fr.m.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.fr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.frp.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.fur.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.fy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ga.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.gan.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.gd.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.gl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.glk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.gn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.got.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.gu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.gv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ha.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.hak.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.haw.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.he.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.hi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.hif.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.hr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.hsb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ht.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.hu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.hy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ia.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.id.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ie.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ig.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ik.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ilo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.io.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.is.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.it.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.iu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ja.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.jbo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.jv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ka.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.kaa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.kab.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.kg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ki.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.kk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.kl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.km.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.kn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ko.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.krc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ks.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ksh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ku.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.kv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.kw.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ky.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.la.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.lad.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.lb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.li.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.lij.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.lmo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ln.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.lo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.lt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.lv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.map-bms.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mdf.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.meta.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mhr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ml.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ms.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mwl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.my.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.myv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.mzn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.na.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nah.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nap.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nds-nl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nds.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ne.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.new.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.no.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nov.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nrm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.nv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.oc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.om.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.or.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.os.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pag.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pam.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pap.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pcd.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pdc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pih.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pms.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pnb.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pnt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ps.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.pt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.qu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.rm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.rmy.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ro.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.roa-rup.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.roa-tara.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ru.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sah.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sc.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.scn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sco.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sd.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.se.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.si.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.simple.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.so.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.species.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sq.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.srn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ss.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.stq.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.su.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sv.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.sw.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.szl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ta.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.te.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.tet.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.tg.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.th.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ti.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.tk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.tl.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.tn.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.to.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.tpi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.tr.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ts.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.tt.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ty.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.udm.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ug.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.uk.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ur.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.uz.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.ve.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.vec.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.vi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.vls.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.vo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.wa.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.war.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.wo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.wuu.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.xal.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.xh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.yi.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.yo.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.za.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.zea.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.zh-classical.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.zh-min-nan.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.zh-yue.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.zh.wikipedia.org" _
                + vbNewLine + "0.0.0.0 www.zu.wikipedia.org")
            End If
            For Each itm As String In list_cus.Items
                sw.WriteLine("0.0.0.0 " + itm _
                + vbNewLine + "0.0.0.0 www." + itm)
            Next
            sw.Close()

        Catch ex As Exception
            MsgBox("Error writing to your hosts file. Your antivirus most likely doesn't trust what I will do with it. Please don't disable your antivirus though. You have not been blocked from anything.")
            erroredOut()
        End Try
    End Sub

    Private Sub startService()

        Dim dateToBlockToString As String = DateTimePicker1.Value.Month & "/" & DateTimePicker1.Value.Day & "/" & DateTimePicker1.Value.Year & " " & hour24I & ":" & minute.Text & ":00"

        Try
            Dim iniFile As New IniFile
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
            iniFile.SetKeyValue("Time", "Until", encryptionW.EncryptData(dateToBlockToString))
            iniFile.SetKeyValue("User", "NeedsAlerted", "yes")
            iniFile.SetKeyValue("User", "Done", "no")
            iniFile.Save(Application.StartupPath + "\ct_settings.ini")
        Catch ex As Exception
            MsgBox("Error writing to my configuration file. Please re-install me.")
            erroredOut()
        End Try

        If svcAlreadyExists = False Then
            ServiceInstaller.InstallAndStart("KCTRP", "KCTRP", Application.StartupPath + "\KCTRP_srv.exe")
        Else
            ServiceInstaller.StartService("KCTRP")
        End If

        Try
            Dim serviceController As System.ServiceProcess.ServiceController
            Dim timeSpan As New TimeSpan(0, 0, 31)

            serviceController = New System.ServiceProcess.ServiceController("KCTRP")
            serviceController.WaitForStatus(ServiceProcess.ServiceControllerStatus.Running, timeSpan)

            Try
                Dim runMe As RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
                runMe.SetValue("ColdTurkey_notify", Application.StartupPath + "\ct_notify.exe")
            Catch ex As Exception
                MsgBox("There was an error writing the ct_notify.exe startup entry")
            End Try
            
            Try
                Threading.Thread.Sleep(500) 'To offset the periodical reading times of ct_settings.ini between KCTRP and ct_notify
                Process.Start(Application.StartupPath + "\ct_notify.exe")
                Process.Start(Application.StartupPath + "\ct_notify2.exe")
            Catch ex As Exception
                MsgBox("There was an error starting the notification program. Cold Turkey will still work, but you won't be notified when your block is over.")
            End Try

        Catch ex As Exception
            MsgBox("The service could not be started. This problem is likely caused by your antivirus software, but please don't disable your antivirus to run this program." + vbNewLine + vbNewLine + _
                   "You can try to add an antivirus exception to allow access to the hosts file by both 'ColdTurkey.exe' and 'kctrp.exe' in the ColdTurkey program folder, then try again.")
            erroredOut()
        End Try

    End Sub

    Private Sub erroredOut()

        Dim fileReader, original As String
        Dim startpos As Integer
        fileReader = My.Computer.FileSystem.ReadAllText(hostDirS)
        startpos = InStr(1, fileReader, "#### ColdTurkey Entries ####", 1)
        If startpos <> 0 And startpos <= 2 Then
            original = ""
        ElseIf startpos = 0 Then
            original = fileReader
        Else
            original = Microsoft.VisualBasic.Left(fileReader, startpos - 1)
        End If
        If My.Computer.FileSystem.FileExists(hostDirS) Then
            SetAttr(hostDirS, vbNormal)
        End If

        Dim fs2 As New FileStream(hostDirS, FileMode.Create, FileAccess.Write, FileShare.Read)
        Dim sw2 As New StreamWriter(fs2)
        sw2.Write(original)
        sw2.Close()
        SetAttr(hostDirS, vbReadOnly)
        Dim iniFile As New IniFile
        iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        iniFile.SetKeyValue("User", "NeedsAlerted", "no")
        iniFile.SetKeyValue("User", "Done", "yes")
        iniFile.Save(Application.StartupPath + "\ct_settings.ini")
        End

    End Sub

    Private Sub tab_sites_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tab_sites.Click
        pan_sites.Visible = True
        pan_custom.Visible = False
        pan_apps.Visible = False
        Dim tab_custom_u As New Font(tab_custom.Font.Name, tab_custom.Font.Size, FontStyle.Underline)
        tab_custom.Font = tab_custom_u
        Dim tab_programs_u As New Font(tab_programs.Font.Name, tab_programs.Font.Size, FontStyle.Underline)
        tab_programs.Font = tab_programs_u
        Dim tab_sites_u As New Font(tab_sites.Font.Name, tab_sites.Font.Size, FontStyle.Regular)
        tab_sites.Font = tab_sites_u

        tab_sites.Cursor = Cursors.Arrow
        tab_custom.Cursor = Cursors.Hand
        tab_programs.Cursor = Cursors.Hand

    End Sub

    Private Sub tab_programs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tab_programs.Click
        pan_apps.Visible = True
        pan_custom.Visible = False
        pan_sites.Visible = False
        Dim tab_custom_u As New Font(tab_custom.Font.Name, tab_custom.Font.Size, FontStyle.Underline)
        tab_custom.Font = tab_custom_u
        Dim tab_programs_u As New Font(tab_programs.Font.Name, tab_programs.Font.Size, FontStyle.Regular)
        tab_programs.Font = tab_programs_u
        Dim tab_sites_u As New Font(tab_sites.Font.Name, tab_sites.Font.Size, FontStyle.Underline)
        tab_sites.Font = tab_sites_u

        tab_sites.Cursor = Cursors.Hand
        tab_custom.Cursor = Cursors.Hand
        tab_programs.Cursor = Cursors.Arrow
    End Sub

    Private Sub tab_custom_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tab_custom.Click
        pan_custom.Visible = True
        pan_apps.Visible = False
        pan_sites.Visible = False
        Dim tab_custom_u As New Font(tab_custom.Font.Name, tab_custom.Font.Size, FontStyle.Regular)
        tab_custom.Font = tab_custom_u
        Dim tab_programs_u As New Font(tab_programs.Font.Name, tab_programs.Font.Size, FontStyle.Underline)
        tab_programs.Font = tab_programs_u
        Dim tab_sites_u As New Font(tab_sites.Font.Name, tab_sites.Font.Size, FontStyle.Underline)
        tab_sites.Font = tab_sites_u

        tab_sites.Cursor = Cursors.Hand
        tab_custom.Cursor = Cursors.Arrow
        tab_programs.Cursor = Cursors.Hand
    End Sub

    Private Sub lbl_info_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbl_info.Click
        Dim message As String
        message = "Cold Turkey (0.7 Beta) Serious Edition" + vbNewLine + "(c) Copyright Felix Belzile, 2012" + vbNewLine + vbNewLine + "Cold Turkey will temporarily block addicting sites and games" + vbNewLine + "so that you can get some work done!"
        MsgBox(message, vbOKOnly, "About Cold Turkey")
    End Sub

    Function GetHTML(ByVal strPage As String) As String
        Dim strReply As String = "NULL"
        'Dim objErr As ErrObject

        'Try
        Dim objHttpRequest As System.Net.HttpWebRequest
        Dim objHttpResponse As System.Net.HttpWebResponse
        objHttpRequest = System.Net.HttpWebRequest.Create(strPage)
        objHttpResponse = objHttpRequest.GetResponse
        Dim objStrmReader As New System.IO.StreamReader(objHttpResponse.GetResponseStream)

        strReply = objStrmReader.ReadToEnd()

        'Catch ex As Exception
        'strReply = "ERROR! " + ex.Message.ToString
        'End Try

        Return strReply

    End Function

    Private Sub add_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles add.Click

        Dim tempString As String = txt_add.Text.ToString
        Dim newKey As String = ""

        If StrComp(tempString, "") = 0 Then
        ElseIf InStr(tempString, ".") = 0 Then
            MsgBox("The domain should have an extention. (like .com or .net)")
        Else
            If InStr(tempString, "http://") = 1 Then
                tempString = Microsoft.VisualBasic.Right(tempString, tempString.Length - 7)
            End If
            If InStr(tempString, "www.") = 1 Then
                tempString = Microsoft.VisualBasic.Right(tempString, tempString.Length - 4)
            End If
            If InStr(tempString, "/") <> 0 Then
                Dim loc As Integer
                loc = InStr(tempString, "/")
                tempString = Microsoft.VisualBasic.Left(tempString, loc - 1)
            End If
            If list_cus.Items.Contains(tempString) Then
                MsgBox("This address is already in the list.")
            Else
                list_cus.Items.Add(tempString)
                txt_add.Text = ""
                For Each entry As String In list_cus.Items
                    newKey = newKey + entry + ";"
                Next
                Dim iniFile As New IniFile
                iniFile.Load(Application.StartupPath + "\ct_settings.ini")
                iniFile.SetKeyValue("User", "CustomSites", newKey)
                iniFile.Save(Application.StartupPath + "\ct_settings.ini")
            End If
        End If

    End Sub

    Private Sub txt_add_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt_add.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) Then
            add_Click(vbNull, System.EventArgs.Empty)
            e.Handled = True
        End If
    End Sub

    Private Sub remove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles remove.Click

        Dim indexOfItem As Integer
        Dim newKey As String = ""

        indexOfItem = list_cus.SelectedIndex
        list_cus.Items.Remove(list_cus.SelectedItem)
        For Each entry As String In list_cus.Items
            newKey = newKey + entry + ";"
        Next

        Dim iniFile As New IniFile
        iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        iniFile.SetKeyValue("User", "CustomSites", newKey)
        iniFile.Save(Application.StartupPath + "\ct_settings.ini")

        If indexOfItem < list_cus.Items.Count And indexOfItem > 0 Then
            list_cus.SetSelected(indexOfItem, True)
        ElseIf indexOfItem = list_cus.Items.Count And indexOfItem > 0 Then
            list_cus.SetSelected(indexOfItem - 1, True)
        ElseIf indexOfItem = 0 And list_cus.Items.Count > 0 Then
            list_cus.SetSelected(0, True)
        End If
        If firstTimeClickCustomBoxB = True Then
            firstTimeClickCustomBoxB = False
            txt_add.Text = ""
            Dim normfont As New Font(txt_add.Font.Name, txt_add.Font.Size, FontStyle.Regular)
            txt_add.Font = normfont
            txt_add.ForeColor = Color.Black
            add.Enabled = True
        End If
    End Sub

    Private Sub chk_24_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_24.CheckedChanged
        If chk_24.Checked = True Then
            amPmSelector.Visible = False
            hour.Visible = False
            If hour.SelectedIndex >= 0 Then
                If StrComp(amPmSelector.Text, "PM", vbTextCompare) = 0 And Val(hour.Text) >= 1 And Val(hour.Text) <= 11 Then
                    hour24.SelectedIndex = Val(hour.Text) + 12
                ElseIf Val(hour.Text) = 12 And StrComp(amPmSelector.Text, "AM", vbTextCompare) = 0 Then
                    hour24.SelectedIndex = 0
                Else
                    hour24.SelectedIndex = Val(hour.Text)
                End If
            End If
            hour24.Visible = True
            If (hour24.SelectedIndex >= 0 And minute.SelectedIndex >= 0) Or (hour.SelectedIndex >= 0 And minute.SelectedIndex >= 0 And amPmSelector.SelectedIndex >= 0) Then
                calculateTimeDiff()
            End If

        Else
            amPmSelector.Visible = True
            hour.Visible = True
            hour24.Visible = False
            If hour24.SelectedIndex >= 0 Then
                If Val(hour24.Text) > 0 And Val(hour24.Text) < 12 Then
                    hour.SelectedIndex = Val(hour24.Text) - 1
                    amPmSelector.SelectedIndex = 0
                ElseIf Val(hour24.Text) = 0 Then
                    hour.SelectedIndex = 11
                    amPmSelector.SelectedIndex = 0
                ElseIf Val(hour24.Text) = 12 Then
                    hour.SelectedIndex = Val(hour24.Text) - 1
                    amPmSelector.SelectedIndex = 1
                Else
                    hour.SelectedIndex = Val(hour24.Text) - 13
                    amPmSelector.SelectedIndex = 1
                End If
            End If
            If (hour24.SelectedIndex >= 0 And minute.SelectedIndex >= 0) Or (hour.SelectedIndex >= 0 And minute.SelectedIndex >= 0 And amPmSelector.SelectedIndex >= 0) Then
                calculateTimeDiff()
            End If
        End If
    End Sub

    Private Sub list_cus_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles list_cus.SelectedIndexChanged
        If list_cus.SelectedIndex >= 0 Then
            remove.Enabled = True
        Else
            remove.Enabled = False
        End If
    End Sub

    Private Sub list_prog_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles list_prog.SelectedIndexChanged
        If list_prog.SelectedIndex >= 0 Then
            removeProg.Enabled = True
        Else
            removeProg.Enabled = False
        End If
    End Sub

    Private Sub txt_add_TextClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_add.Click
        If firstTimeClickCustomBoxB = True Then
            firstTimeClickCustomBoxB = False
            txt_add.Text = ""
            Dim normfont As New Font(txt_add.Font.Name, txt_add.Font.Size, FontStyle.Regular)
            txt_add.Font = normfont
            txt_add.ForeColor = Color.Black
            add.Enabled = True
        End If
    End Sub


    Private Sub import_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles import.Click

        Dim filePathS, reader As String
        Dim newKey As String = ""
        If importfile.ShowDialog() = Windows.Forms.DialogResult.OK Then
            filePathS = importfile.FileName
            If File.Exists(filePathS) Then
                Try
                    reader = My.Computer.FileSystem.ReadAllText(filePathS)
                    Dim strs() As String
                    strs = Split(reader, Environment.NewLine)
                    For Each line As String In strs
                        If StrComp(line, "") = 0 Then
                        ElseIf InStr(line, ".") = 0 Then
                            MsgBox("This domain should have an extention... like .com or .net for example:" + vbNewLine + line)
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
                                If list_cus.Items.Contains(line) Then
                                    MsgBox("This address is already in the list:" + vbNewLine + line)
                                Else
                                    list_cus.Items.Add(line)
                                    For Each entry As String In list_cus.Items
                                        newKey = newKey + entry + ";"
                                    Next
                                    Dim iniFile = New IniFile
                                    iniFile.Load(Application.StartupPath + "\ct_settings.ini")
                                    iniFile.SetKeyValue("User", "CustomSites", newKey)
                                    iniFile.Save(Application.StartupPath + "\ct_settings.ini")
                                End If
                            End If
                        End If
                    Next
                Catch
                    MsgBox("Error importing file.")
                End Try

            End If
            If firstTimeClickCustomBoxB = True Then
                firstTimeClickCustomBoxB = False
                txt_add.Text = ""
                Dim normfont As New Font(txt_add.Font.Name, txt_add.Font.Size, FontStyle.Regular)
                txt_add.Font = normfont
                txt_add.ForeColor = Color.Black
                add.Enabled = True
            End If
        End If

    End Sub

    Private Sub hour24_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles hour24.SelectedIndexChanged
        If hour24.SelectedIndex >= 0 And minute.SelectedIndex >= 0 Then
            calculateTimeDiff()
        End If

    End Sub

    Private Sub hour_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles hour.SelectedIndexChanged
        If hour.SelectedIndex >= 0 And minute.SelectedIndex >= 0 And amPmSelector.SelectedIndex >= 0 Then
            calculateTimeDiff()
        End If
    End Sub

    Private Sub minute_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles minute.SelectedIndexChanged
        If (hour24.SelectedIndex >= 0 And minute.SelectedIndex >= 0) Or (hour.SelectedIndex >= 0 And minute.SelectedIndex >= 0 And amPmSelector.SelectedIndex >= 0) Then
            calculateTimeDiff()
        End If
    End Sub

    Private Sub mornin_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles amPmSelector.SelectedIndexChanged
        If hour.SelectedIndex >= 0 And minute.SelectedIndex >= 0 And amPmSelector.SelectedIndex >= 0 Then
            calculateTimeDiff()
        End If
    End Sub

    Private Sub DateTimePicker1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateTimePicker1.ValueChanged
        If (hour24.SelectedIndex >= 0 And minute.SelectedIndex >= 0) Or (hour.SelectedIndex >= 0 And minute.SelectedIndex >= 0 And amPmSelector.SelectedIndex >= 0) Then
            calculateTimeDiff()
        End If
    End Sub

    Private Sub calculateTimeDiff()

        Dim secondsout As Long

        If chk_24.Checked = False Then
            If StrComp(amPmSelector.Text, "PM", vbTextCompare) = 0 And Val(hour.Text) >= 1 And Val(hour.Text) <= 11 Then
                hour24I = Val(hour.Text) + 12
            ElseIf Val(hour.Text) = 12 And StrComp(amPmSelector.Text, "PM", vbTextCompare) <> 0 Then
                hour24I = 0
            Else
                hour24I = Val(hour.Text)
            End If
        Else
            hour24I = Val(hour24.Text)
        End If


        Dim didItWork As Boolean
        Dim dateToBlockTo As DateTime
        Dim dateToBlockToString As String = DateTimePicker1.Value.Month & "/" & DateTimePicker1.Value.Day & "/" & DateTimePicker1.Value.Year & " " & hour24I & ":" & minute.Text & ":00"
        didItWork = DateTime.TryParse(dateToBlockToString, dateToBlockTo)
        If didItWork = True Then
            secondsout = DateDiff(DateInterval.Second, DateTime.Now, dateToBlockTo)

            If secondsout > 86400 Then
                timewarning.Text = "(Nice! Thats about " + secondsToText(secondsout, True) + ")"
                timewarning.ForeColor = Color.Purple
                Button1.Enabled = True
            ElseIf secondsout < 120 And secondsout > 0 Then
                timewarning.Text = "(under two minutes)"
                timewarning.ForeColor = Color.Purple
                Button1.Enabled = False
            ElseIf secondsout <= 0 Then
                timewarning.Text = "Please enter a time in the future."
                timewarning.ForeColor = Color.Gray
                Button1.Enabled = False
            Else
                timewarning.Text = "(about " + secondsToText(secondsout, True) + ")"
                timewarning.ForeColor = Color.Green
                Button1.Enabled = True
            End If
        Else
            MsgBox("Error calculating time.")
        End If
    End Sub

    Private Sub ColdTurkey_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        Dim serverVersion As Double
        Dim thisVersion As Double = 0.7
        Dim answer As MsgBoxResult
        Try
            serverVersion = GetHTML("http://getcoldturkey.com/version.html")
            If serverVersion > thisVersion Then
                answer = MsgBox("Looks like there's an update for Cold Turkey!" + vbNewLine + "Would you like to update now?", MsgBoxStyle.YesNo)
                If answer = MsgBoxResult.Yes Then
                    Process.Start("http://getcoldturkey.com/download.html")
                    End
                End If
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub addProgram(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        If addProgramDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then

            Dim filePathS As String = addProgramDialog.FileName
            Dim newKey As String = ""
            If File.Exists(filePathS) Then
                list_prog.Items.Add(filePathS)
                For Each entry As String In list_prog.Items
                    newKey = newKey + entry + ";"
                Next
                Dim iniFile = New IniFile
                iniFile.Load(Application.StartupPath + "\ct_settings.ini")
                iniFile.SetKeyValue("Process", "List", encryptionW.EncryptData(newKey))
                iniFile.Save(Application.StartupPath + "\ct_settings.ini")

            End If
        End If
    End Sub

    Private Sub removeProgram(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles removeProg.Click
        Dim indexOfItem As Integer
        Dim newKey As String = ""
        Dim iniFile As New IniFile
        iniFile.Load(Application.StartupPath + "\ct_settings.ini")

        indexOfItem = list_prog.SelectedIndex
        list_prog.Items.Remove(list_prog.SelectedItem)
        For Each entry As String In list_prog.Items
            newKey = newKey + entry + ";"
        Next
        If newKey = "" Then
            iniFile.SetKeyValue("Process", "List", "null")
        Else
            iniFile.SetKeyValue("Process", "List", newKey)
        End If

        iniFile.Save(Application.StartupPath + "\ct_settings.ini")
        If indexOfItem < list_prog.Items.Count And indexOfItem > 0 Then
            list_prog.SetSelected(indexOfItem, True)
        ElseIf indexOfItem = list_prog.Items.Count And indexOfItem > 0 Then
            list_prog.SetSelected(indexOfItem - 1, True)
        ElseIf indexOfItem = 0 And list_prog.Items.Count > 0 Then
            list_prog.SetSelected(0, True)
        End If
    End Sub
End Class

Public NotInheritable Class Simple3Des
    Private TripleDes As New TripleDESCryptoServiceProvider
    Private Function TruncateHash(ByVal key As String, ByVal length As Integer) As Byte()

        Dim sha1 As New SHA1CryptoServiceProvider

        ' Hash the key.
        Dim keyBytes() As Byte =
            System.Text.Encoding.Unicode.GetBytes(key)
        Dim hash() As Byte = sha1.ComputeHash(keyBytes)

        ' Truncate or pad the hash.
        ReDim Preserve hash(length - 1)
        Return hash
    End Function
    Sub New(ByVal key As String)
        ' Initialize the crypto provider.
        TripleDes.Key = TruncateHash(key, TripleDes.KeySize \ 8)
        TripleDes.IV = TruncateHash("", TripleDes.BlockSize \ 8)
    End Sub
    Public Function EncryptData(ByVal plaintext As String) As String

        ' Convert the plaintext string to a byte array.
        Dim plaintextBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(plaintext)

        ' Create the stream.
        Dim ms As New System.IO.MemoryStream
        ' Create the encoder to write to the stream.
        Dim encStream As New CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)

        ' Use the crypto stream to write the byte array to the stream.
        encStream.Write(plaintextBytes, 0, plaintextBytes.Length)
        encStream.FlushFinalBlock()

        ' Convert the encrypted stream to a printable string.
        Return Convert.ToBase64String(ms.ToArray)
    End Function
    Public Function DecryptData(ByVal encryptedtext As String) As String
        Dim encryptedBytes() As Byte
        ' Convert the encrypted text string to a byte array.
        Try
            encryptedBytes = Convert.FromBase64String(encryptedtext)
        Catch ef As System.FormatException
            'encryptedBytes = 
            End
        End Try
        ' Create the stream.
        Dim ms As New System.IO.MemoryStream
        ' Create the decoder to write to the stream.
        Dim decStream As New CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write)

        ' Use the crypto stream to write the byte array to the stream.
        decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
        decStream.FlushFinalBlock()

        ' Convert the plaintext stream to a string.
        Return System.Text.Encoding.Unicode.GetString(ms.ToArray)
    End Function

End Class
