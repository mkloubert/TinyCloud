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
Imports System.ComponentModel.Composition
Imports System.Net
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports SysConsole = System.Console
Imports System.Linq
Imports System.IO

''' <summary>
''' List local directory action.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class ListLocalConsoleModeAction
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
            Return New String() {"list-local", "ls-local", "dir-local"}
        End Get
    End Property

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.ShortDescription" />
    ''' </summary>
    Public Overrides ReadOnly Property ShortDescription As String
        Get
            Return "Lists a local directory."
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        args = args.Where(Function(x) Not String.IsNullOrWhiteSpace(x)) _
                   .ToList()

        If args.Count < 1 Then
            args.Add(Me.Mode.CurrentLocalDirectory.FullName)
        End If

        For Each a As String In args
            Try
                If Not Path.IsPathRooted(a) Then
                    a = Path.Combine(Me.Mode.CurrentLocalDirectory.FullName, a)
                End If

                a = Path.GetFullPath(a)

                If Directory.Exists(a) Then
                    Dim dir As DirectoryInfo = New DirectoryInfo(a)

                    SysConsole.Write("Directory: ")

                    ConsoleHelper.InvokeForColor(Sub()
                                                     SysConsole.WriteLine(dir.FullName)
                                                 End Sub, ConsoleColor.White)

                    SysConsole.WriteLine()

                    For Each subDir As DirectoryInfo In dir.EnumerateDirectories()
                        Dim name As String = subDir.Name
                        Dim lastWriteTime As Date = subDir.LastWriteTime

                        SysConsole.Write("{0:yyyy-MM-dd}  {0:HH:mm:ss}", lastWriteTime)
                        SysConsole.Write("    ")
                        SysConsole.Write("<DIR>".PadRight(14, " "))
                        SysConsole.Write("  {0}", name)

                        SysConsole.WriteLine()
                    Next

                    For Each file As FileInfo In dir.EnumerateFiles()
                        Dim name As String = file.Name
                        Dim lastWriteTime As Date = file.LastWriteTime
                        Dim size As Long = file.Length

                        SysConsole.Write("{0:yyyy-MM-dd}  {0:HH:mm:ss}", lastWriteTime)
                        SysConsole.Write("    {0}", size.ToString("n0", AppServices.DataCulture) _
                                                        .PadLeft(14))
                        SysConsole.Write("  {0}", name)

                        SysConsole.WriteLine()
                    Next

                    SysConsole.WriteLine()
                Else
                    '' directory not found
                    ConsoleHelper.InvokeForColor(Sub()
                                                     SysConsole.Write("The local directory ")
                                                 End Sub, ConsoleColor.Yellow)

                    ConsoleHelper.InvokeForColor(Sub()
                                                     SysConsole.Write(a)
                                                 End Sub, ConsoleColor.White)

                    ConsoleHelper.InvokeForColor(Sub()
                                                     SysConsole.Write(" does not exist!")

                                                     SysConsole.WriteLine()
                                                 End Sub, ConsoleColor.Yellow)
                End If
            Catch ex As Exception
                ShowException(ex)
            End Try
        Next
    End Sub

#End Region

End Class