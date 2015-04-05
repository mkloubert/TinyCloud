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
Imports SysIoPath = System.IO.Path

''' <summary>
''' Download file.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class DownloadFileConsoleAction
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
            Return New String() {"download", "dl"}
        End Get
    End Property

#End Region

#Region "Methods (3)"

    Private Sub DownloadDirectory(conn As CloudConnection, path As String)

    End Sub

    Private Sub DownloadFile(conn As CloudConnection, path As String)
        Dim filename As String = SysIoPath.GetFileName(path)
        If String.IsNullOrWhiteSpace(filename) Then
            Return
        End If

        '' find unique file
        Dim i As ULong? = Nothing
        Dim f As FileInfo = New FileInfo(SysIoPath.Combine(Me.Mode.CurrentLocalDirectory.FullName, _
                                                           filename))
        While f.Exists
            If i Is Nothing Then
                i = 0
            Else
                i = i + 1
            End If

            Dim baseName As String = SysIoPath.GetFileNameWithoutExtension(filename)
            Dim fileExt As String = SysIoPath.GetExtension(filename)

            f = New FileInfo(SysIoPath.Combine(Me.Mode.CurrentLocalDirectory.FullName, _
                                               String.Format("{0}-{1}{2}", _
                                                             baseName, i, fileExt)))
        End While

        Dim request As WebRequest = conn.CreateApiRequest("download-file")
        request.Method = "POST"

        Using rs As Stream = request.GetRequestStream()
            Using writer As StreamWriter = New StreamWriter(rs, AppServices.Charset)
                writer.Write(path)

                rs.Flush()
            End Using
        End Using

        Dim response As HttpWebResponse = request.GetResponse()
        Using respStream As Stream = response.GetResponseStream()
            Try
                Using targetStream As FileStream = f.Open(FileMode.CreateNew, FileAccess.ReadWrite)
                    respStream.CopyTo(targetStream)
                End Using
            Catch ex As Exception
                f.Refresh()
                If f.Exists Then
                    f.Delete()
                    f.Refresh()
                End If

                Throw
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        For Each a As String In args
            Try
                If String.IsNullOrWhiteSpace(a) Then
                    Continue For
                End If

                a = Me.GetFullPath(a)

                Dim actionToInvoke As Action(Of CloudConnection, String) = AddressOf Me.DownloadFile

                If actionToInvoke IsNot Nothing Then
                    actionToInvoke(conn, a)
                End If
            Catch ex As Exception
                ShowException(ex)
            End Try
        Next
    End Sub

#End Region

End Class
