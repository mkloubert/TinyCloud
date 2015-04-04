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
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports SysConsole = System.Console
Imports System.Net
Imports System.IO

<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class ChangeDirConsoleModeAction
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
            Return New String() {"cd", "chdir"}
        End Get
    End Property

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.ShortDescription" />
    ''' </summary>
    Public Overrides ReadOnly Property ShortDescription As String
        Get
            Return "Changes the directory."
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        Dim newDir As String = args.FirstOrDefault(Function(x) Not String.IsNullOrWhiteSpace(x))

        If String.IsNullOrWhiteSpace(newDir) Then
            SysConsole.Write("Current directory is: ")

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.WriteLine(Me.Mode.CurrentDirectory)
                                         End Sub, ConsoleColor.White)

            Return
        End If

        Dim currentDir As String = Me.Mode.CurrentDirectory
        If String.IsNullOrWhiteSpace(currentDir) Then
            currentDir = "/"
        End If

        newDir = newDir.Trim()

        If newDir = "." Then
            '' nothing to do
            Return
        End If

        If newDir = ".." Then
            '' go up

            Dim parts As String() = currentDir.Split("/") _
                                              .Where(Function(x) Not String.IsNullOrWhiteSpace(x)) _
                                              .ToArray()

            Me.Execute(conn, cmd, _
                       New List(Of String) From {"/" & String.Join("/", _
                                                                   parts.Take(parts.Length - 1))})

            Return
        End If

        If Not Path.IsPathRooted(newDir) Then
            newDir = Path.Combine(currentDir, newDir)
        End If

        Dim request As WebRequest = conn.CreateApiRequest("list")
        request.Method = "POST"

        Dim requestData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
        requestData("path") = newDir
        requestData("test") = True

        request.SendJson(requestData)

        Dim response As IDictionary(Of String, Object) = request.GetResponse().GetJson()
        If response Is Nothing Then
            Return
        End If

        Dim code As Integer = Convert.ChangeType(response("code"), GetType(Integer), _
                                                 AppServices.DataCulture)

        Select Case code
            Case 0
                '' directory exists
                Me.Mode.CurrentDirectory = newDir
                Exit Select

            Case 404
                '' directory not found
                ConsoleHelper.InvokeForColor(Sub()
                                                 SysConsole.Write("The directory ")
                                             End Sub, ConsoleColor.Yellow)

                ConsoleHelper.InvokeForColor(Sub()
                                                 SysConsole.Write(newDir)
                                             End Sub, ConsoleColor.White)

                ConsoleHelper.InvokeForColor(Sub()
                                                 SysConsole.Write(" was not found!")

                                                 SysConsole.WriteLine()
                                             End Sub, ConsoleColor.Yellow)
                Exit Select
        End Select
    End Sub

#End Region

End Class
