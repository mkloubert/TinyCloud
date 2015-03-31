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
''' Action for tracing arguments.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class TestArgsConsoleModeAction
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
            Return New String() {"test_args", "testargs"}
        End Get
    End Property

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.ShowInHelp" />
    ''' </summary>
    Public Overrides ReadOnly Property ShowInHelp As Boolean
        Get
            Return False
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        For i = 1 To args.Count
            Dim a As String = args(i - 1)

            SysConsole.Write("[{0}] => ", i - 1)

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.Write(Chr(34))
                                         End Sub, foreColor:=ConsoleColor.Blue)

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.Write("{0}", a)
                                         End Sub, foreColor:=ConsoleColor.Green)

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.Write(Chr(34))
                                         End Sub, foreColor:=ConsoleColor.Blue)

            SysConsole.Write(" ({0})", a.Length)

            SysConsole.WriteLine()
        Next
    End Sub

#End Region

End Class