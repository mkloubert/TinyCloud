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
Imports SysConsole = System.Console

''' <summary>
''' A basic action for a <see cref="ConsoleMode" /> instance.
''' </summary>
Public MustInherit Class ConsoleModeActionBase
    Inherits CloudObjectBase : Implements IConsoleModeAction

#Region "Fields (1)"

    Private ReadOnly _MODE As ConsoleMode

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ConsoleModeActionBase" /> class.
    ''' </summary>
    ''' <param name="mode">The value for the <see cref="ConsoleModeActionBase.Mode" /> property.</param>
    ''' <exception cref="ArgumentNullException">
    ''' <paramref name="mode" /> is <see langword="Nothing" />.
    ''' </exception>
    Protected Sub New(mode As ConsoleMode)
        If mode Is Nothing Then
            Throw New ArgumentNullException("mode")
        End If

        Me._MODE = mode
    End Sub

#End Region

#Region "Properties (4)"

    ''' <summary>
    ''' <see cref="IConsoleModeAction.Mode" />
    ''' </summary>
    Public ReadOnly Property Mode As ConsoleMode Implements IConsoleModeAction.Mode
        Get
            Return Me._MODE
        End Get
    End Property

    ''' <summary>
    ''' <see cref="IConsoleModeAction.Names" />
    ''' </summary>
    Public MustOverride ReadOnly Property Names As IEnumerable(Of String) Implements IConsoleModeAction.Names

    ''' <summary>
    ''' <see cref="IConsoleModeAction.ShortDescription" />
    ''' </summary>
    Public Overridable ReadOnly Property ShortDescription As String Implements IConsoleModeAction.ShortDescription
        Get
            Return Nothing
        End Get
    End Property

    ''' <summary>
    ''' <see cref="IConsoleModeAction.ShowInHelp" />
    ''' </summary>
    Public Overridable ReadOnly Property ShowInHelp As Boolean Implements IConsoleModeAction.ShowInHelp
        Get
            Return True
        End Get
    End Property

#End Region

#Region "Methods (3)"

    ''' <summary>
    ''' <see cref="IConsoleModeAction.Execute" />
    ''' </summary>
    Public MustOverride Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String)) Implements IConsoleModeAction.Execute

    ''' <summary>
    ''' <see cref="ConsoleMode.ShowException" />
    ''' </summary>
    Protected Shared Sub ShowException(ex As Exception)
        ConsoleMode.ShowException(ex)
    End Sub

    ''' <summary>
    ''' <see cref="IConsoleModeAction.ShowHelp" />
    ''' </summary>
    Public Overridable Sub ShowHelp() Implements IConsoleModeAction.ShowHelp
        ''TODO
    End Sub

#End Region

End Class