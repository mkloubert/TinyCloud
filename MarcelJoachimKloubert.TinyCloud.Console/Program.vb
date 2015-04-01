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

Imports System.Net
Imports System.Reflection
Imports SysConsole = System.Console

''' <summary>
''' The program class.
''' </summary>
Module Program

#Region "Methods (2)"

    ''' <summary>
    ''' The entry point.
    ''' </summary>
    ''' <param name="args">The submitted arguments.</param>
    Sub Main(args As String())
        SysConsole.Clear()
        PrintHeader()

        Try
            Using mode As IMode = AppSettings.CreateMode()
                If mode IsNot Nothing Then
                    mode.Run()
                Else
                    ''TODO: show warning
                End If
            End Using
        Catch ex As Exception
            Dim innerEx As Exception = If(ex.GetBaseException(), ex)

            ConsoleHelper.InvokeForColor(Sub()
                                             SysConsole.WriteLine()

                                             SysConsole.WriteLine(innerEx)

                                             SysConsole.WriteLine()
                                         End Sub, foreColor:=ConsoleColor.Yellow _
                                                , bgColor:=ConsoleColor.Red)
        End Try

#If DEBUG Then
        Global.System.Console.WriteLine()
        Global.System.Console.WriteLine()

        Global.System.Console.WriteLine("===== ENTER =====")
        Global.System.Console.ReadLine()
#End If
    End Sub

    Private Sub PrintHeader()
        Dim title As String = String.Format("TinyCloud Console {0}", _
                                            Assembly.GetExecutingAssembly().GetName().Version)

        SysConsole.WriteLine(title)
        SysConsole.WriteLine(String.Concat(Enumerable.Repeat("=", _
                                                             title.Length + 5)))
        SysConsole.WriteLine()
    End Sub

#End Region

End Module