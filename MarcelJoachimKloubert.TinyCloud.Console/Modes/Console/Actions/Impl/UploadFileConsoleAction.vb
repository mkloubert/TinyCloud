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
Imports SysConsole = System.Console
Imports System.Net
Imports MarcelJoachimKloubert.TinyCloud.SDK
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports System.IO

''' <summary>
''' Upload file.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class UploadFileConsoleAction
    Inherits ConsoleModeActionBase

#Region "Constructors (1)"

    ''' <inheriteddoc />
    <ImportingConstructor>
    Public Sub New(mode As ConsoleMode)
        MyBase.New(mode)
    End Sub

#End Region

#Region "Properties (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Names" />
    ''' </summary>
    Public Overrides ReadOnly Property Names As IEnumerable(Of String)
        Get
            Return New String() {"upload", "ul"}
        End Get
    End Property

#End Region

#Region "Methods (3)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        For Each a As String In args
            Try
                If String.IsNullOrWhiteSpace(a) Then
                    Continue For
                End If

                If Not Path.IsPathRooted(a) Then
                    a = Path.Combine(Me.Mode.CurrentLocalDirectory.FullName, a)
                End If

                Dim actionToInvoke As Action(Of CloudConnection, String, String) = Nothing

                If File.Exists(a) Then
                    actionToInvoke = AddressOf Me.UploadFile
                ElseIf Directory.Exists(a) Then
                    actionToInvoke = AddressOf Me.UploadDirectory
                Else
                    '' show warning
                End If

                If actionToInvoke IsNot Nothing Then
                    actionToInvoke(conn, a, Me.Mode.CurrentDirectory)
                End If
            Catch ex As Exception
                ShowException(ex)
            End Try
        Next
    End Sub

    Private Sub UploadDirectory(conn As CloudConnection, localPath As String, remoteDir As String)
        Try
            Dim d As DirectoryInfo = New DirectoryInfo(localPath)
            If Not d.Exists Then
                Return
            End If

            Dim subRemoteDir As String = remoteDir & "/" & d.Name

            CloudHelper.CreateDirectory(conn, subRemoteDir)

            For Each f As FileInfo In d.EnumerateFiles()
                Me.UploadFile(conn, f.FullName, subRemoteDir)
            Next

            For Each sd As DirectoryInfo In d.EnumerateDirectories()
                Me.UploadDirectory(conn, sd.FullName, subRemoteDir)
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub UploadFile(conn As CloudConnection, localPath As String, remoteDir As String)
        Try
            Dim f As FileInfo = New FileInfo(localPath)

            SysConsole.WriteLine()
            SysConsole.Write("Uploading file '")

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.Write("{0}", f.FullName)
                                         End Sub, ConsoleColor.White)

            SysConsole.Write("'...")
            SysConsole.WriteLine()

            Using s As FileStream = f.Open(FileMode.Open, FileAccess.Read, FileShare.Read)
                CloudHelper.UploadFile(conn, s, remoteDir & "/" & f.Name)
            End Using
        Catch ex As Exception

        End Try
    End Sub

#End Region

End Class
