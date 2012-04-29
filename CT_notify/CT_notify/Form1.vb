Imports Microsoft.Win32
Imports System.Security.Cryptography
Imports System.Diagnostics
Imports System.IO
Imports ct_notify.IniFile

Public Class Form1

    Dim done As String
    Dim pop As String
    Dim iniProcessList As String = ""
    Dim iniFile As IniFile
    Dim encryptionW As New Simple3Des("ct_textbox")
    Dim iniDateOld As Date
    Dim iniHourOld, iniMinuteOld As Integer

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Hide()
        iniFile = New IniFile
        AddHandler SystemEvents.TimeChanged, AddressOf SystemEvents_TimeChanged
        AddHandler SystemEvents.PowerModeChanged, AddressOf SystemEvents_PowerModeChanged

        Try
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        Catch ex As Exception
            MsgBox("Error reading mah configuration file. Please re-install me.")
            End
        End Try

        Try
            done = iniFile.GetKeyValue("User", "Done")
            iniProcessList = iniFile.GetKeyValue("Process", "List")
            If StrComp(iniProcessList, "null") <> 0 Then
                iniProcessList = encryptionW.DecryptData(iniProcessList)
            End If
        Catch ex As Exception
            End
        End Try

        If StrComp("yes", done) = 0 Then
            If StrComp(iniFile.GetKeyValue("User", "NeedsAlerted"), "no") = 0 Then
                End
            Else
                iniFile.SetKeyValue("User", "NeedsAlerted", "no")
                iniFile.Save(Application.StartupPath + "\ct_settings.ini")
                Process.Start(Application.StartupPath + "\ct_popup.exe")
                End
            End If
        End If
        timer.Start()

    End Sub

    Private Sub timer_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles timer.Elapsed
        iniFile = New IniFile
        Try
            iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        Catch ex As Exception
            Threading.Thread.Sleep(500)
            Try
                iniFile.Load(Application.StartupPath + "\ct_settings.ini")
            Catch ex2 As Exception
            End Try
        End Try

        done = iniFile.GetKeyValue("User", "Done")

        If StrComp("yes", done) = 0 Then
            If StrComp(iniFile.GetKeyValue("User", "NeedsAlerted"), "no") = 0 Then
                End
            Else
                iniFile.SetKeyValue("User", "NeedsAlerted", "no")
                iniFile.Save(Application.StartupPath + "\ct_settings.ini")
                Dim killHelper As Process() = Process.GetProcessesByName("ct_notify2")
                Dim proc As Process
                Try
                    For Each proc In killHelper
                        proc.Kill()
                    Next
                Catch ex As Exception
                End Try
                Process.Start(Application.StartupPath + "\ct_popup.exe")
                End
            End If
        End If



    End Sub

    Private Sub SystemEvents_TimeChanged(ByVal sender As Object, ByVal e As EventArgs)


        Dim iniDateNew, iniDateUntil As Date
        Dim iniYearDiff, iniMonthDiff, iniDayDiff, iniHourDiff, iniMinuteDiff, iniHourNew, iniMinuteNew, iniHourUntil, iniMinuteUntil As Integer

        RemoveHandler SystemEvents.TimeChanged, AddressOf SystemEvents_TimeChanged
        RemoveHandler SystemEvents.PowerModeChanged, AddressOf SystemEvents_PowerModeChanged

        timer.Stop()
        iniFile = New IniFile
        iniFile.Load(Application.StartupPath + "\ct_settings.ini")
        iniFile.SetKeyValue("Time", "TimeChanging", "yes")
        iniFile.Save(Application.StartupPath + "\ct_settings.ini")

        Threading.Thread.Sleep(8500)
        iniFile.Load(Application.StartupPath + "\ct_settings.ini")

        iniDateOld = Date.Parse(encryptionW.DecryptData(iniFile.GetKeyValue("CurrentTime", "Date")))
        iniHourOld = Val(encryptionW.DecryptData(iniFile.GetKeyValue("CurrentTime", "Hour")))
        iniMinuteOld = Val(encryptionW.DecryptData(iniFile.GetKeyValue("CurrentTime", "Minute")))
        iniDateUntil = Date.Parse(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Date")))
        iniHourUntil = Val(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Hour")))
        iniMinuteUntil = Val(encryptionW.DecryptData(iniFile.GetKeyValue("Time", "Minute")))

        iniYearDiff = Date.Now.Year - iniDateOld.Year
        iniMonthDiff = Date.Now.Month - iniDateOld.Month
        iniDayDiff = Date.Now.Day - iniDateOld.Day
        iniHourDiff = Date.Now.Hour - iniHourOld
        iniMinuteDiff = Date.Now.Minute - iniMinuteOld

        iniDateNew = New Date(iniYearDiff + iniDateUntil.Year, iniMonthDiff + iniDateUntil.Month, iniDayDiff + iniDateUntil.Day)
        iniHourNew = iniHourUntil + iniHourDiff
        iniMinuteNew = iniMinuteUntil + iniMinuteDiff

        iniFile.SetKeyValue("Time", "Date", encryptionW.EncryptData(iniDateNew.ToString))
        iniFile.SetKeyValue("Time", "Hour", encryptionW.EncryptData(iniHourNew.ToString))
        iniFile.SetKeyValue("Time", "Minute", encryptionW.EncryptData(iniMinuteNew.ToString))
        iniFile.SetKeyValue("Time", "TimeChanging", "no")
        iniFile.Save(Application.StartupPath + "\ct_settings.ini")

        Application.Restart()

    End Sub

    Private Sub timerHelper_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles timerHelper.Elapsed
        Dim helperProcessList = System.Diagnostics.Process.GetProcessesByName("ct_notify2")
        Try
            If helperProcessList.Length <= 0 Then
                Process.Start(Application.StartupPath + "\ct_notify2.exe")
            End If
        Catch ex As Exception
        End Try

        Dim processList = System.Diagnostics.Process.GetProcesses()
        For Each Proc In processList
            If Proc.SessionId <> 0 Then
                Try
                    If iniProcessList.Contains(Proc.ProcessName + ".exe") Then
                        Proc.Kill()
                        MsgBox("Cold Turkey just closed a blocked program from running." + vbNewLine + "Get back to work!", MsgBoxStyle.Exclamation, "Haha, nice try!")
                    End If
                Catch ex As Exception
                End Try
            End If
        Next

    End Sub

    Private Sub SystemEvents_PowerModeChanged(ByVal sender As Object, ByVal e As PowerModeChangedEventArgs)
        RemoveHandler SystemEvents.PowerModeChanged, AddressOf SystemEvents_PowerModeChanged
        RemoveHandler SystemEvents.TimeChanged, AddressOf SystemEvents_TimeChanged
        Threading.Thread.Sleep(10000)
        Application.Restart()
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
