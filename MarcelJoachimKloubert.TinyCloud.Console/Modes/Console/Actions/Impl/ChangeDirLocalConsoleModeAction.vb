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
Public NotInheritable Class ChangeDirLocalConsoleModeAction
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
            Return New String() {"cd-local", "chdir-local"}
        End Get
    End Property

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.ShortDescription" />
    ''' </summary>
    Public Overrides ReadOnly Property ShortDescription As String
        Get
            Return "Changes the local directory."
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
            SysConsole.Write("Current local directory is: ")

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.WriteLine(Me.Mode.CurrentLocalDirectory.FullName)
                                         End Sub, ConsoleColor.White)

            Return
        End If

        Dim currentDir As DirectoryInfo = Me.Mode.CurrentLocalDirectory

        newDir = newDir.Trim()

        If newDir = "." Then
            '' nothing to do
            Return
        End If

        If newDir = ".." Then
            '' go up

            Dim parent As DirectoryInfo = currentDir.Parent
            If parent IsNot Nothing Then
                Me.Mode.CurrentLocalDirectory = parent
            End If

            Return
        End If

        If Not Path.IsPathRooted(newDir) Then
            newDir = Path.Combine(currentDir.FullName, newDir)
        End If

        newDir = Path.GetFullPath(newDir)

        If Directory.Exists(newDir) Then
            Me.Mode.CurrentLocalDirectory = New DirectoryInfo(newDir)
        Else
            '' directory not found
            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.Write("The local directory ")
                                         End Sub, ConsoleColor.Yellow)

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.Write(newDir)
                                         End Sub, ConsoleColor.White)

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.Write(" was not found!")

                                             SysConsole.WriteLine()
                                         End Sub, ConsoleColor.Yellow)
        End If
    End Sub

#End Region

End Class
