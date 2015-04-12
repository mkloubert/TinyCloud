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
Imports ICSharpCode.SharpZipLib.Zip

''' <summary>
''' Upload files from a ZIP container.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class UploadZipConsoleModeAction
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
            Return New String() {"upload-zip", "ul-zip"}
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

                Dim zipFile As FileInfo = New FileInfo(a)

                Using fsStream As FileStream = zipFile.OpenRead()
                    Using zipStream As ZipInputStream = New ZipInputStream(fsStream)
                        Dim zipEntry As ZipEntry = zipStream.GetNextEntry()

                        While zipEntry IsNot Nothing
                            Dim name As String = zipEntry.Name
                            If String.IsNullOrWhiteSpace(name) Then
                                name = String.Empty
                            Else
                                name = name.Trim()
                            End If

                            name = name.Replace("\", "/")

                            While name.StartsWith("/")
                                name = name.Substring(1).Trim()
                            End While

                            While name.EndsWith("/")
                                name = name.Substring(0, name.Length - 1).Trim()
                            End While

                            If name = String.Empty Then
                                Continue While
                            End If

                            Dim fullPath As String = Me.GetFullPath(name)
                            fullPath = fullPath.Replace("\", "/")

                            Dim parts = fullPath.Split("/") _
                                                .Where(Function(x) Not String.IsNullOrWhiteSpace(x)) _
                                                .Select(Function(x) x.Trim()) _
                                                .ToArray()

                            For i = 1 To parts.Length - 1
                                Dim dirToCreate = String.Join("/", parts.Take(i))
                                If String.IsNullOrWhiteSpace(dirToCreate) Then
                                    Continue For
                                End If

                                CloudHelper.CreateDirectory(conn, "/" & dirToCreate)
                            Next

                            If zipEntry.IsDirectory Then
                                '' directory
                                CloudHelper.CreateDirectory(conn, fullPath)
                            ElseIf zipEntry.IsFile Then
                                '' file

                                Try
                                    SysConsole.WriteLine()
                                    SysConsole.Write("Uploading file '")

                                    ConsoleHelper.InvokeForColor(Sub()
                                                                     SysConsole.Write("{0}", name)
                                                                 End Sub, ConsoleColor.White)

                                    SysConsole.Write("'...")
                                    SysConsole.WriteLine()

                                    CloudHelper.UploadFile(conn, zipStream, fullPath)
                                Catch ex As Exception

                                End Try
                            End If

                            zipEntry = zipStream.GetNextEntry()
                        End While
                    End Using
                End Using
            Catch ex As Exception
                ShowException(ex)
            End Try
        Next
    End Sub

    Private Sub UploadDirectory(conn As CloudConnection, path As String)

    End Sub

    Private Sub UploadFile(conn As CloudConnection, path As String)
        Dim f As FileInfo = New FileInfo(path)

        Using s As FileStream = f.OpenRead()
            Dim request As WebRequest = conn.CreateApiRequest("upload-file")
            request.Method = "POST"

            request.ContentLength = s.Length
            request.Headers("X-TinyCloud-Filename") = Me.Mode.CurrentDirectory & "/" & f.Name

            Using rs As Stream = request.GetRequestStream()
                s.CopyTo(rs)

                rs.Flush()
            End Using

            Dim response As IDictionary(Of String, Object) = request.GetResponse().GetJson()
            If response Is Nothing Then
                Return
            End If
        End Using
    End Sub

#End Region

End Class
