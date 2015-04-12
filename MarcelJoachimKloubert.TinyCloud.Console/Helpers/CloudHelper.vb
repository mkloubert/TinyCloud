Option Explicit Off

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

Imports MarcelJoachimKloubert.TinyCloud.SDK
Imports System.Net
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports SysConsole = System.Console
Imports System.IO

''' <summary>
''' Helper class for cloud operations.
''' </summary>
Module CloudHelper

#Region "Methods (3)"

    ''' <summary>
    ''' Creates a new directory.
    ''' </summary>
    ''' <param name="conn">The underlying connection.</param>
    ''' <param name="newDir">The full path of the new directory.</param>
    Public Sub CreateDirectory(conn As CloudConnection, newDir As String)
        Dim request As WebRequest = conn.CreateApiRequest("create-directory")
        request.Method = "POST"

        Dim requestData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
        requestData("path") = newDir

        request.SendJson(requestData)

        Dim response = request.GetResponse().GetJson()
        If response Is Nothing Then
            Return
        End If

        Select Case CType(response.code, Integer)
            Case 0
                SysConsole.Write("Directory ")

                ConsoleHelper.InvokeForColor(Sub()
                                                 SysConsole.Write(newDir)
                                             End Sub, ConsoleColor.White)

                SysConsole.Write(" was created successfully.")
                Exit Select

            Case 6
                ConsoleHelper.InvokeForColor(Sub()
                                                 SysConsole.Write("Directory ")
                                             End Sub, ConsoleColor.Yellow)

                ConsoleHelper.InvokeForColor(Sub()
                                                 SysConsole.Write(newDir)
                                             End Sub, ConsoleColor.White)

                ConsoleHelper.InvokeForColor(Sub()
                                                 SysConsole.Write(" already exists.")
                                             End Sub, ConsoleColor.Yellow)
                Exit Select
        End Select

        SysConsole.WriteLine()
    End Sub

    ''' <summary>
    ''' Uploads a file.
    ''' </summary>
    ''' <param name="conn">The underlying connection.</param>
    ''' <param name="srcStream">The stream with the source data.</param>
    ''' <param name="targetFile">The full path of the target file.</param>
    Public Sub UploadFile(conn As CloudConnection, srcStream As Stream, targetFile As String)
        Dim request As WebRequest = conn.CreateApiRequest("upload-file")
        request.Method = "POST"

        request.ContentLength = srcStream.Length
        request.Headers("X-TinyCloud-Filename") = targetFile

        Using rs As Stream = request.GetRequestStream()
            Dim bytesWritten As Long = 0

            Dim buffer(81919) As Byte
            Dim bytesRead As Integer
            While (True)
                rs.Flush()

                bytesRead = srcStream.Read(buffer, 0, buffer.Length)
                If bytesRead < 1 Then
                    Exit While
                End If

                rs.Write(buffer, 0, bytesRead)

                bytesWritten = bytesWritten + bytesRead
                UpdateUploadProgress(bytesWritten, srcStream.Length, _
                                     Nothing)
            End While
        End Using

        Dim response As IDictionary(Of String, Object) = request.GetResponse().GetJson()
        If response Is Nothing Then
            Return
        End If

        SysConsole.WriteLine()
    End Sub

    Private Sub UpdateUploadProgress(written As Long, total As Long, ex As Exception)
        SysConsole.Write(vbCr)

        Dim progress As Double = 0
        If total > 0 Then
            progress = CType(written, Double) / CType(total, Double)
        End If

        If progress < 0 Then
            progress = 0
        ElseIf progress > 1 Then
            progress = 1
        End If

        SysConsole.Write("|")

        Dim progressCharCount As Integer = CType(Math.Floor(progress * CType(30, Double)), Integer)
        SysConsole.Write(String.Concat(Enumerable.Repeat("=", progressCharCount)))
        SysConsole.Write(String.Concat(Enumerable.Repeat(" ", 30 - progressCharCount)))

        SysConsole.Write("|")

        SysConsole.Write(String.Concat(Enumerable.Repeat(" ", 4)))

        SysConsole.Write("{0} / {1}", FormatHelper.ToHumanReadableFileSize(written) _
                                    , FormatHelper.ToHumanReadableFileSize(total))

        SysConsole.Write(String.Concat(Enumerable.Repeat(" ", 6)))
    End Sub

#End Region

End Module
