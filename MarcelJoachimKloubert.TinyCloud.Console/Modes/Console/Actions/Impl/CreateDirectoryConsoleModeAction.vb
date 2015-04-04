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

Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports MarcelJoachimKloubert.TinyCloud.SDK
Imports System.ComponentModel.Composition
Imports SysConsole = System.Console
Imports System.Net
Imports System.IO

<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class CreateDirectoryConsoleModeAction
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
            Return New String() {"mkdir", "md"}
        End Get
    End Property

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.ShortDescription" />
    ''' </summary>
    Public Overrides ReadOnly Property ShortDescription As String
        Get
            Return "Creates a new directory."
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        For Each a As String In args
            If String.IsNullOrWhiteSpace(a) Then
                Continue For
            End If

            Try
                Dim newDir As String = a.Trim()

                If Not Path.IsPathRooted(newDir) Then
                    newDir = Path.Combine(Me.Mode.CurrentDirectory, newDir)
                End If

                Dim request As WebRequest = conn.CreateApiRequest("create-directory")
                request.Method = "POST"

                Dim requestData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
                requestData("path") = newDir

                request.SendJson(requestData)

                Dim response As IDictionary(Of String, Object) = request.GetResponse().GetJson()
                If response Is Nothing Then
                    Return
                End If

                Dim code As Integer = Convert.ChangeType(response("code"), GetType(Integer), _
                                                         AppServices.DataCulture)

                Select Case code
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
            Catch ex As Exception
                ShowException(ex)
            End Try
        Next
    End Sub

#End Region

End Class
