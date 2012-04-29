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

Public Class Service1
    Inherits System.ServiceProcess.ServiceBase

    Dim ctMutex As Threading.Mutex
    Private m_previousExecutionState As UInteger
    Friend WithEvents timer As System.Timers.Timer
    Friend WithEvents adder As System.IO.FileSystemWatcher
    Dim install As String
    Dim iniFile As IniFile
    Public sWinDir As String = Environ("WinDir")
    Public hostDirS As String = sWinDir + "\system32\drivers\etc\hosts"
    Dim iniDateUntil As Date
    Dim iniHourUntil, iniMinuteUntil As Integer
    Dim iniTimeChanging As String
    Public fs As FileStream
    Public sw As StreamWriter
    Dim encryptionW As New Simple3Des("ct_textbox")

#Region " Component Designer generated code "

    Public Sub New()
        MyBase.New()
        MyBase.CanHandleSessionChangeEvent = True
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

        ctMutex = New Threading.Mutex(False, "KeepmealivepleaseKCTRP")

        'AddHandler SystemEvents.PowerModeChanged, AddressOf SystemEvents_PowerModeChanged

        iniFile = New IniFile
        Try
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        Catch ex As Exception
            iniFile.AddSection("User")
            iniFile.SetKeyValue("User", "CustomChecked", "abcdefghijk")
            iniFile.SetKeyValue("User", "CustomSites", "null")
            iniFile.SetKeyValue("User", "Done", "no")
            iniFile.SetKeyValue("User", "NeedsAlerted", "yes")
            iniFile.AddSection("Time")
            iniFile.SetKeyValue("Time", "Date", encryptionW.EncryptData(DateAdd("d", 7, CDate(Date.Today))))
            iniFile.SetKeyValue("Time", "Hour", encryptionW.EncryptData(Date.Today.Hour))
            iniFile.SetKeyValue("Time", "Minute", encryptionW.EncryptData(Date.Today.Minute))
            iniFile.SetKeyValue("Time", "TimeChanging", "no")
            iniFile.AddSection("CurrentTime")
            iniFile.SetKeyValue("CurrentTime", "Date", encryptionW.EncryptData(Date.Today.Date))
            iniFile.SetKeyValue("CurrentTime", "Hour", encryptionW.EncryptData(Date.Today.Hour))
            iniFile.SetKeyValue("CurrentTime", "Minute", encryptionW.EncryptData(Date.Today.Minute))
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

        fs = New FileStream(hostDirS, FileMode.Append, FileAccess.Write, FileShare.Read)
        sw = New StreamWriter(fs)
        SetAttr(hostDirS, vbReadOnly)

        adder.Path = sWinDir & "\system32\drivers\etc"

    End Sub

    Private Sub timer_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles timer.Elapsed

        Dim processList As System.Diagnostics.Process() = Nothing
        Dim Proc As System.Diagnostics.Process
        Dim notifyFound As Boolean = False
        Dim iniProcessList As String = ""

        Threading.Thread.Sleep(1000)

        iniFile = New IniFile
        Try
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
            iniDateUntil = Date.Parse(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Date")))
            iniHourUntil = Integer.Parse(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Hour")))
            iniMinuteUntil = Integer.Parse(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Minute")))
            iniTimeChanging = iniFile.GetKeyValue("Time", "TimeChanging")
            iniProcessList = iniFile.GetKeyValue("Process", "List")
            If StrComp("null", iniProcessList) <> 0 Then
                iniProcessList = encryptionW.DecryptData(iniProcessList)
            End If
        Catch ex As Exception
            iniFile.AddSection("User")
            iniFile.SetKeyValue("User", "CustomChecked", "abcdefghijk")
            iniFile.SetKeyValue("User", "CustomSites", "null")
            iniFile.SetKeyValue("User", "Done", "no")
            iniFile.SetKeyValue("User", "NeedsAlerted", "yes")
            iniFile.AddSection("Time")
            iniFile.SetKeyValue("Time", "Date", encryptionW.EncryptData(DateAdd("d", 7, CDate(Date.Today))))
            iniFile.SetKeyValue("Time", "Hour", encryptionW.EncryptData(Date.Today.Hour))
            iniFile.SetKeyValue("Time", "Minute", encryptionW.EncryptData(Date.Today.Minute))
            iniFile.SetKeyValue("Time", "TimeChanging", "no")
            iniFile.AddSection("CurrentTime")
            iniFile.SetKeyValue("CurrentTime", "Date", encryptionW.EncryptData(Date.Today.Date))
            iniFile.SetKeyValue("CurrentTime", "Hour", encryptionW.EncryptData(Date.Today.Hour))
            iniFile.SetKeyValue("CurrentTime", "Minute", encryptionW.EncryptData(Date.Today.Minute))
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


        Dim secondsStillBlocked As Integer
        secondsStillBlocked = DateAndTime.DateDiff(DateInterval.Second, DateAndTime.Today.Date, iniDateUntil.Date)
        If TimeOfDay.Hour < iniHourUntil Then
            secondsStillBlocked = secondsStillBlocked + ((iniHourUntil - TimeOfDay.Hour) * 60 * 60)
            If TimeOfDay.Minute < iniMinuteUntil Then
                secondsStillBlocked = secondsStillBlocked + ((iniMinuteUntil - TimeOfDay.Minute) * 60)
            ElseIf TimeOfDay.Minute > iniMinuteUntil Then
                secondsStillBlocked = secondsStillBlocked - ((TimeOfDay.Minute - (iniMinuteUntil)) * 60)
            End If
        ElseIf TimeOfDay.Hour = iniHourUntil Then
            If TimeOfDay.Minute < iniMinuteUntil Then
                secondsStillBlocked = secondsStillBlocked + ((iniMinuteUntil - TimeOfDay.Minute) * 60)
            ElseIf TimeOfDay.Minute > iniMinuteUntil Then
                secondsStillBlocked = secondsStillBlocked - ((TimeOfDay.Minute - iniMinuteUntil) * 60)
            End If
        ElseIf TimeOfDay.Hour > iniHourUntil Then
            secondsStillBlocked = secondsStillBlocked - ((TimeOfDay.Hour - iniHourUntil) * 60 * 60)
            If TimeOfDay.Minute < iniMinuteUntil Then
                secondsStillBlocked = secondsStillBlocked + ((iniMinuteUntil - TimeOfDay.Minute) * 60)
            ElseIf TimeOfDay.Minute > iniMinuteUntil Then
                secondsStillBlocked = secondsStillBlocked - ((TimeOfDay.Minute - (iniMinuteUntil)) * 60)
            End If
        End If
        secondsStillBlocked = secondsStillBlocked - (TimeOfDay.Second)

        If StrComp("no", iniTimeChanging) = 0 Then
            If secondsStillBlocked <= 5 Then
                stopMe()
            Else
                iniFile.SetKeyValue("CurrentTime", "Date", encryptionW.EncryptData(DateAndTime.Now))
                iniFile.SetKeyValue("CurrentTime", "Hour", encryptionW.EncryptData(DateAndTime.Now.Hour))
                iniFile.SetKeyValue("CurrentTime", "Minute", encryptionW.EncryptData(DateAndTime.Now.Minute))
                iniFile.Save(Application.StartupPath + "\ct_settings.ini")
            End If
        End If
    End Sub

    Private Sub stopMe()

        Dim fileReader, original As String
        Dim startpos As Integer
        iniFile = New IniFile

        sw.Close()

        fileReader = My.Computer.FileSystem.ReadAllText(hostDirS)
        startpos = InStr(1, fileReader, "#### Cold Turkey Entries ####", 1)
        If startpos <> 0 And startpos <= 2 Then
            original = ""
        ElseIf startpos = 0 Then
            original = fileReader
        Else
            original = Microsoft.VisualBasic.Left(fileReader, startpos - 3)
        End If
        If My.Computer.FileSystem.FileExists(hostDirS) Then
            SetAttr(hostDirS, vbNormal)
        End If

        Dim fs2 As New FileStream(hostDirS, FileMode.Create, FileAccess.Write, FileShare.Read)
        Dim sw2 As New StreamWriter(fs2)
        sw2.Write(original)
        sw2.Close()
        SetAttr(hostDirS, vbReadOnly)

        iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        iniFile.SetKeyValue("User", "Done", "yes")
        iniFile.Save(Application.StartupPath + "\ct_settings.ini")
        Me.Stop()

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