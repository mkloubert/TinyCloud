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

''' <summary>
''' Describes an action for a <see cref="ConsoleMode" /> instance.
''' </summary>
Public Interface IConsoleModeAction : Inherits ICloudObject

#Region "Properties (4)"

    ''' <summary>
    ''' Gets the underlying mode instance.
    ''' </summary>
    ReadOnly Property Mode As ConsoleMode

    ''' <summary>
    ''' Gets the names of the action.
    ''' </summary>
    ReadOnly Property Names As IEnumerable(Of String)

    ''' <summary>
    ''' Gets the short description of the command.
    ''' </summary>
    ReadOnly Property ShortDescription As String

    ''' <summary>
    ''' Gets if the action should be shown by help command or not.
    ''' </summary>
    ReadOnly Property ShowInHelp As Boolean

#End Region

#Region "Methods (2)"

    ''' <summary>
    ''' Executes the action.
    ''' </summary>
    ''' <param name="conn">The underlying connection.</param>
    ''' <param name="cmd">The lower-case normalized command.</param>
    ''' <param name="args">The arguments for the action.</param>
    Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))

    ''' <summary>
    ''' Shows the help screen of that action.
    ''' </summary>
    Sub ShowHelp()

#End Region

End Interface
