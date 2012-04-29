Imports System.ServiceProcess
Imports Microsoft.Win32

Public Class Service1
    Inherits System.ServiceProcess.ServiceBase

#Region " Component Designer generated code "

    Public Sub New()
        MyBase.New()

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
    Friend WithEvents Timer1 As System.Timers.Timer
    Friend WithEvents FileSystemWatcher1 As System.IO.FileSystemWatcher

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Timer1 = New System.Timers.Timer()
        Me.FileSystemWatcher1 = New System.IO.FileSystemWatcher()
        CType(Me.Timer1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.FileSystemWatcher1, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 10000.0R
        '
        'FileSystemWatcher1
        '
        Me.FileSystemWatcher1.EnableRaisingEvents = True
        Me.FileSystemWatcher1.Path = "C:\Windows\System32\drivers\etc"
        '
        'Service1
        '
        Me.ServiceName = "KASRP"
        CType(Me.Timer1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FileSystemWatcher1, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

#End Region
    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things 
        ' in motion so your service can do its work. 
        Timer1.Enabled = True
    End Sub

    Private Sub Timer1_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs)

    End Sub

    Private Sub FileSystemWatcher1_Changed(ByVal sender As System.Object, ByVal e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Changed
        WriteRegistry(Registry.CurrentUser, "Software\KASRP", "hosts_changed", "1")
    End Sub
End Class


Sub WriteRegistry(ByVal ParentKey As RegistryKey, ByVal SubKey As String, _
    ByVal ValueName As String, ByVal Value As Object)

    Dim Key As RegistryKey

    Try
        'Open the registry key.
        Key = ParentKey.OpenSubKey(SubKey, True)
        If Key Is Nothing Then 'if the key doesn't exist.
            Key = ParentKey.CreateSubKey(SubKey)
        End If

        'Set the value.
        Key.SetValue(ValueName, Value)

        Console.WriteLine("Value:{0} for {1} is successfully written.", Value, ValueName)
    Catch e As Exception
        Console.WriteLine("Error occurs in WriteRegistry" & e.Message)
    End Try
End Sub