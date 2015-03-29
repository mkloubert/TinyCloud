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

Imports SysConsole = System.Console

Module ConsoleHelper

#Region "Methods (1)"

    ''' <summary>
    ''' Invokes an action for specific colors. The old colors are restored after invokation.
    ''' </summary>
    ''' <param name="action">The action to invoke.</param>
    ''' <param name="foreColor">The foreground color (if defined).</param>
    ''' <param name="bgColor">The background color (if defined)</param>
    Public Sub InvokeForColor(action As Action,
                              Optional foreColor As ConsoleColor? = Nothing, Optional bgColor As ConsoleColor? = Nothing)

        Dim oldFG As ConsoleColor = SysConsole.ForegroundColor
        Dim oldBG As ConsoleColor = SysConsole.BackgroundColor

        Try
            If foreColor.HasValue Then
                SysConsole.ForegroundColor = foreColor.Value
            End If

            If bgColor.HasValue Then
                SysConsole.BackgroundColor = bgColor.Value
            End If

            action()
        Finally
            SysConsole.ForegroundColor = oldFG
            SysConsole.BackgroundColor = oldBG
        End Try
    End Sub

#End Region

End Module
