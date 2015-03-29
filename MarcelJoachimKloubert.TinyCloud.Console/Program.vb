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
Imports System.Reflection

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
        Global.System.Console.Clear()
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
            Dim innerEx As Exception = ex.GetBaseException()
            If innerEx Is Nothing Then
                innerEx = ex
            End If

            ConsoleHelper.InvokeForColor(Sub()
                                             Global.System.Console.WriteLine()

                                             Global.System.Console.WriteLine(innerEx)

                                             Global.System.Console.WriteLine()
                                         End Sub, foreColor:=ConsoleColor.Yellow _
                                                , bgColor:=ConsoleColor.Red)
        End Try
    End Sub

    Private Sub PrintHeader()
        Dim title As String = String.Format("TinyCloud Console {0}", _
                                            Assembly.GetExecutingAssembly().GetName().Version)

        Global.System.Console.WriteLine(title)
        Global.System.Console.WriteLine(String.Concat(Enumerable.Repeat("=", _
                                                          title.Length + 5)))
        Global.System.Console.WriteLine()
    End Sub

#End Region

End Module