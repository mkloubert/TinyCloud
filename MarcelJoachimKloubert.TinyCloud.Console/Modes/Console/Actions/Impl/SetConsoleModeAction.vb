''  TinyCloud Console (https://github.com/mkloubert/TinyCloud)
''  Copyright (C) 2015  Marcel Joachim Kloubert <marcel.kloubert@gmx.net>
''
''  This program is free software: you can redistribute it and/or modify
''  it under the terms of the GNU Affero General Public License as
''  published by the Free Software Foundation, either version 3 of the
''  License, or (at your option) any later version.
''
''  This program is distributed in the hope that it will be useful,
''  but WITHOUT ANY WARRANTY; without even the implied warranty of
''  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
''  GNU Affero General Public License for more details.
''
''  You should have received a copy of the GNU Affero General Public License
''  along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports System.ComponentModel.Composition
Imports MarcelJoachimKloubert.TinyCloud.SDK
Imports SysConsole = System.Console

''' <summary>
''' Action for clearing the console screen.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public Class SetConsoleModeAction
    Inherits ConsoleModeActionBase

#Region "Constructors (1)"

    ''' <inheriteddoc />
    <ImportingConstructor>
    Public Sub New(mode As ConsoleMode)
        MyBase.New(mode)
    End Sub

#End Region

#Region "Properties (2)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Names" />
    ''' </summary>
    Public Overrides ReadOnly Property Names As IEnumerable(Of String)
        Get
            Return New String() {"set"}
        End Get
    End Property

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.ShortDescription" />
    ''' </summary>
    Public Overrides ReadOnly Property ShortDescription As String
        Get
            Return "Defines settings."
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        If args.Count < 1 Then
            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.WriteLine("And what?")
                                         End Sub, foreColor:=ConsoleColor.Yellow)

            Return
        End If

        Dim setArgs As IEnumerable(Of String) = args.Skip(1)
        Dim setArgsNoEmpty As IEnumerable(Of String) = setArgs.Where(Function(x) Not String.IsNullOrWhiteSpace(x))

        Select Case args(0).ToLower().Trim()
            Case "host"
                Dim host As String = AppSettings.Host
                For Each a As String In setArgsNoEmpty
                    host = a
                Next

                If String.IsNullOrWhiteSpace(host) Then
                    host = AppSettings.DEFAULT_HOST
                End If

                AppSettings.Host = host.ToLower().Trim()
                Exit Select

            Case "port"
                Dim port As Integer = AppSettings.Port
                For Each a As String In setArgsNoEmpty
                    If a.ToLower().Trim() = "default" Then
                        port = AppSettings.DEFAULT_PORT
                    Else
                        port = Integer.Parse(a.Trim(), AppServices.DataCulture)
                    End If
                Next

                AppSettings.Port = port
                Exit Select

            Case "user"
                Dim user As String = AppSettings.Username
                For Each a As String In setArgsNoEmpty
                    user = a.ToLower().Trim()
                Next

                AppSettings.Username = user
                Exit Select

            Case "password", "pwd"
                Dim pwd As String = Nothing
                For Each a As String In setArgs
                    pwd = a
                    If String.IsNullOrEmpty(pwd) Then
                        pwd = Nothing
                    End If
                Next

                AppSettings.Password = pwd
                Exit Select
        End Select
    End Sub

#End Region

End Class