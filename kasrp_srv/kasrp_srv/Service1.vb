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

Imports System.ServiceProcess
Imports System.IO
Imports System.Security.Cryptography
Imports Microsoft.Win32
Imports Microsoft.VisualBasic
Imports System.Net.Sockets
Imports System.Net
Imports System.Runtime.InteropServices
Imports kctrp.IniFile
Imports System.Text
Imports System.Threading
Imports System.Globalization

Public Class Service1
    Inherits System.ServiceProcess.ServiceBase

    Dim ctMutex As Threading.Mutex
    Private m_previousExecutionState As UInteger
    Friend WithEvents timer As System.Timers.Timer
    Friend WithEvents adder As System.IO.FileSystemWatcher
    Dim install As String
    Public sWinDir As String = Environ("WinDir")
    Public hostDirS As String = sWinDir + "\system32\drivers\etc\hosts"
    Dim iniDateUntil As DateTime
    Dim iniTimeChanging As String
    Public fs As FileStream
    Public sw As StreamWriter
    Dim encryptionW As New Simple3Des("ct_textbox")
    Dim swStarted As Boolean = False
    Dim culture As CultureInfo = New CultureInfo("en-CA")

#Region " Component Designer generated code "

    Public Sub New()
        MyBase.New()
        MyBase.CanHandleSessionChangeEvent = True

        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-CA")
        Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-CA")
        ' This call is required by the Component Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call

    End Sub

    'UserService overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    Protected Overloads Sub OnStop(ByVal e As System.EventArgs)
        MyBase.OnStop()

        ' Restore previous state
        ' No way to recover; already exiting

    End Sub

    ' The main entry point for the process
    <MTAThread()> _
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase

        ' More than one NT Service may run within the same process. To add
        ' another service to this process, change the following line to
        ' create a second service object. For example,
        '
        '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
        '
        ServicesToRun = New System.ServiceProcess.ServiceBase() {New Service1}

        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.timer = New System.Timers.Timer()
        Me.adder = New System.IO.FileSystemWatcher()
        CType(Me.timer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.adder, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'timer
        '
        Me.timer.Enabled = True
        Me.timer.Interval = 10000.0R
        '
        'adder
        '
        Me.adder.EnableRaisingEvents = True
        Me.adder.Filter = "add_to_hosts"
        '
        'Service1
        '
        Me.CanStop = False
        Me.ServiceName = "KCTRP"
        CType(Me.timer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.adder, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

#End Region

    Protected Overrides Sub OnStart(ByVal args() As String)

        Dim timeLeft As Long
        Dim timeLeftDate As DateTime

        ctMutex = New Threading.Mutex(False, "KeepmealivepleaseKCTRP")
        adder.Path = sWinDir & "\system32\drivers\etc"
        Try
            Dim iniFile As IniFile = New IniFile
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
            If iniFile.Sections.Count < 2 Then
                stopMe()
            Else
                DateTime.TryParse(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Until")), culture, DateTimeStyles.None, timeLeftDate)
                timeLeft = DateDiff(DateInterval.Second, DateTime.Now, timeLeftDate)
                If timeLeft <= 0 Then
                    stopMe()
                End If
            End If

        Catch ex As Exception
            My.Computer.FileSystem.WriteAllText(Application.StartupPath + "\ct_settings.ini", "", False)
            Dim iniFile = New IniFile
            iniFile.AddSection("User")
            iniFile.SetKeyValue("User", "CustomChecked", "abcdefghijk")
            iniFile.SetKeyValue("User", "CustomSites", "null")
            iniFile.SetKeyValue("User", "Done", "no")
            iniFile.SetKeyValue("User", "NeedsAlerted", "yes")
            iniFile.AddSection("Time")
            iniFile.SetKeyValue("Time", "Until", encryptionW.EncryptData(DateAdd("d", 7, DateTime.Now)))
            iniFile.SetKeyValue("Time", "TimeChanging", "no")
            iniFile.AddSection("CurrentTime")
            iniFile.SetKeyValue("CurrentTime", "Now", encryptionW.EncryptData(DateTime.Now))
            iniFile.AddSection("Process")
            iniFile.SetKeyValue("Process", "List", "null")
            iniFile.Save(Application.StartupPath + "\ct_settings.ini")
        End Try

        If My.Computer.FileSystem.FileExists(hostDirS) Then
            SetAttr(hostDirS, vbNormal)
        Else
            System.IO.File.AppendAllText(hostDirS, "")
            SetAttr(hostDirS, vbNormal)
        End If
        Try
            fs = New FileStream(hostDirS, FileMode.Append, FileAccess.Write, FileShare.Read)
            sw = New StreamWriter(fs)
            swStarted = True
            SetAttr(hostDirS, vbReadOnly)
        Catch ex As Exception
            stopMe()
        End Try

    End Sub

    Private Sub timer_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles timer.Elapsed

        Dim processList As System.Diagnostics.Process() = Nothing
        Dim Proc As System.Diagnostics.Process
        Dim notifyFound As Boolean = False
        Dim iniProcessList As String = ""
        Dim timeLeft As Long
        Dim timeLeftDate As DateTime

        Try
            Dim iniFile = New IniFile
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
            DateTime.TryParse(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Until")), culture, DateTimeStyles.None, timeLeftDate)
            iniTimeChanging = iniFile.GetKeyValue("Time", "TimeChanging")
            iniProcessList = iniFile.GetKeyValue("Process", "List")
            If StrComp("null", iniProcessList) <> 0 Then
                iniProcessList = encryptionW.DecryptData(iniProcessList)
            End If
        Catch ex As Exception
            Dim iniFile = New IniFile
            iniFile.AddSection("User")
            iniFile.SetKeyValue("User", "CustomChecked", "abcdefghijk")
            iniFile.SetKeyValue("User", "CustomSites", "null")
            iniFile.SetKeyValue("User", "Done", "no")
            iniFile.SetKeyValue("User", "NeedsAlerted", "yes")
            iniFile.AddSection("Time")
            iniFile.SetKeyValue("Time", "Until", encryptionW.EncryptData(DateAdd("d", 7, DateTime.Now)))
            iniFile.SetKeyValue("Time", "TimeChanging", "no")
            iniFile.AddSection("CurrentTime")
            iniFile.SetKeyValue("CurrentTime", "Now", encryptionW.EncryptData(DateTime.Now))
            iniFile.AddSection("Process")
            iniFile.SetKeyValue("Process", "List", "null")
            iniFile.Save(Application.StartupPath + "\ct_settings.ini")
        End Try

        processList = System.Diagnostics.Process.GetProcesses()
        For Each Proc In processList
            If Proc.SessionId = 0 Then
                Try
                    If iniProcessList.Contains(Proc.ProcessName + ".exe") Then
                        Proc.Kill()
                    End If
                Catch ex As Exception
                End Try
            End If
        Next

        timeLeft = DateDiff(DateInterval.Second, DateTime.Now, timeLeftDate)

        If StrComp("no", iniTimeChanging) = 0 Then
            If timeLeft <= 5 Then
                stopMe()
            Else
                Dim iniFile = New IniFile
                iniFile.Load(Application.StartupPath + "\ct_settings.ini")
                iniFile.SetKeyValue("CurrentTime", "Now", encryptionW.EncryptData(DateTime.Now))
                iniFile.Save(Application.StartupPath + "\ct_settings.ini")
            End If
        End If
    End Sub

    Private Sub stopMe()

        Dim fileReader As String = ""
        Dim original As String = ""
        Dim startpos As Integer = 0
        Dim hostsFileNeedsRemoval As Boolean = False

        If My.Computer.FileSystem.FileExists(hostDirS) Then
            fileReader = My.Computer.FileSystem.ReadAllText(hostDirS)
            If fileReader.Contains("#### Cold Turkey Entries ####") Then
                hostsFileNeedsRemoval = True
            End If
        End If

        If hostsFileNeedsRemoval Then

            If swStarted Then
                sw.Close()
            End If

            SetAttr(hostDirS, vbNormal)

            startpos = InStr(1, fileReader, "#### Cold Turkey Entries ####", 1)
            If startpos <> 0 And startpos <= 2 Then
                original = ""
            ElseIf startpos = 0 Then
                original = fileReader
            Else
                original = Microsoft.VisualBasic.Left(fileReader, startpos - 3)
            End If

            Dim fs2 As New FileStream(hostDirS, FileMode.Create, FileAccess.Write, FileShare.Read)
            Dim sw2 As New StreamWriter(fs2)
            sw2.Write(original)
            sw2.Close()
            SetAttr(hostDirS, vbReadOnly)

            Dim iniFile = New IniFile
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
            iniFile.SetKeyValue("User", "Done", "yes")
            iniFile.Save(Application.StartupPath + "\ct_settings.ini")
        End If


        Me.Stop()
        End

    End Sub

    Private Sub adder_Changed(ByVal sender As System.Object, ByVal e As System.IO.FileSystemEventArgs) Handles adder.Changed

        If My.Computer.FileSystem.FileExists(sWinDir & "\system32\drivers\etc\add_to_hosts") Then
            Dim toAdd As String
            toAdd = System.IO.File.ReadAllText(sWinDir & "\system32\drivers\etc\add_to_hosts")
            SetAttr(hostDirS, vbNormal)
            sw.Write(toAdd)
            sw.Flush()
            SetAttr(hostDirS, vbReadOnly)
            Try
                System.IO.File.Delete(sWinDir & "\system32\drivers\etc\add_to_hosts")
            Catch ex As Exception
            End Try
        End If

    End Sub

    'Private Sub SystemEvents_PowerModeChanged(ByVal sender As Object, ByVal e As PowerModeChangedEventArgs)
    '    If e.Mode = PowerModes.Suspend Then
    '        Dim processList = System.Diagnostics.Process.GetProcessesByName("ct_notify")
    '        For Each Proc In processList
    '            Try
    '                Proc.Kill()
    '            Catch ex As Exception
    '            End Try
    '        Next
    '        Dim processList2 = System.Diagnostics.Process.GetProcessesByName("ct_notify2")
    '        For Each Proc In processList2
    '            Try
    '                Proc.Kill()
    '            Catch ex As Exception
    '            End Try
    '        Next
    '    End If
    '    If e.Mode = PowerModes.Resume Then
    '    End If

    'End Sub

End Class

Public NotInheritable Class Simple3Des
    Private TripleDes As New TripleDESCryptoServiceProvider
    Private Function TruncateHash(
        ByVal key As String,
        ByVal length As Integer) As Byte()

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
    Public Function EncryptData(
        ByVal plaintext As String) As String

        ' Convert the plaintext string to a byte array.
        Dim plaintextBytes() As Byte =
            System.Text.Encoding.Unicode.GetBytes(plaintext)

        ' Create the stream.
        Dim ms As New System.IO.MemoryStream
        ' Create the encoder to write to the stream.
        Dim encStream As New CryptoStream(ms,
            TripleDes.CreateEncryptor(),
            System.Security.Cryptography.CryptoStreamMode.Write)

        ' Use the crypto stream to write the byte array to the stream.
        encStream.Write(plaintextBytes, 0, plaintextBytes.Length)
        encStream.FlushFinalBlock()

        ' Convert the encrypted stream to a printable string.
        Return Convert.ToBase64String(ms.ToArray)
    End Function
    Public Function DecryptData(
    ByVal encryptedtext As String) As String
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
        Dim decStream As New CryptoStream(ms,
            TripleDes.CreateDecryptor(),
            System.Security.Cryptography.CryptoStreamMode.Write)

        ' Use the crypto stream to write the byte array to the stream.
        decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
        decStream.FlushFinalBlock()

        ' Convert the plaintext stream to a string.
        Return System.Text.Encoding.Unicode.GetString(ms.ToArray)
    End Function

End Class