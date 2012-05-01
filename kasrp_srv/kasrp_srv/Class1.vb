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

Imports System
Imports System.Collections
Imports System.Configuration.Install
Imports System.ServiceProcess
Imports System.ComponentModel

<RunInstallerAttribute(True)> _
Public Class MyProjectInstaller
    Inherits Installer
    Private serviceInstaller1 As ServiceInstaller
    Private processInstaller As ServiceProcessInstaller

    Public Sub New()
        ' Instantiate installers for process and services.
        processInstaller = New ServiceProcessInstaller()
        serviceInstaller1 = New ServiceInstaller()

        ' The services will run under the system account.
        processInstaller.Account = ServiceAccount.LocalSystem

        ' The services will be started manually.
        serviceInstaller1.StartType = ServiceStartMode.Automatic

        ' ServiceName must equal those on ServiceBase derived classes.
        serviceInstaller1.ServiceName = "KCTRP"

        ' Add installers to collection. Order is not important.
        Installers.Add(serviceInstaller1)
        Installers.Add(processInstaller)
    End Sub
End Class