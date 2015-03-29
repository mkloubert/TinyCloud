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

Imports System.Text.RegularExpressions
Imports MarcelJoachimKloubert.TinyCloud.SDK
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports System.Net
Imports System.ComponentModel.Composition.Hosting
Imports System.Linq
Imports System.ComponentModel.Composition

''' <summary>
''' Console mode.
''' </summary>
Public NotInheritable Class ConsoleMode
    Inherits ModeBase

#Region "Fields (2)"

    Private ReadOnly _CONN As CloudConnection
    Private ReadOnly _CONTAINER As CompositionContainer

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ConsoleMode" /> class.
    ''' </summary>
    Public Sub New()
        Me._CONN = AppSettings.CreateConnection()

        Dim catalogs As AggregateCatalog = New AggregateCatalog()

        Dim asmCatalog As AssemblyCatalog = New AssemblyCatalog(Me.GetType().Assembly)
        catalogs.Catalogs.Add(asmCatalog)

        Me._CONTAINER = New CompositionContainer(catalogs, isThreadSafe:=True)
        Me._CONTAINER.ComposeExportedValue(Of ConsoleMode)(Me)
    End Sub

#End Region

#Region "Methods (5)"


    Private Shared Sub ExtractCommandLineArguments(input As String, ByRef cmd As String, ByRef args As IList(Of String))
        cmd = String.Empty
        args = New List(Of String)()

        If String.IsNullOrWhiteSpace(input) Then
            Return
        End If

        input = input.TrimStart()

        Dim isCmd As Boolean = True
        Dim isTrimming As Boolean = True
        Dim endChar As Char = " "
        Dim arg As String = String.Empty
        Dim charsToIgnore As Integer = 0
        For i = 1 To input.Length
            If charsToIgnore > 0 Then
                charsToIgnore = charsToIgnore - 1
                Continue For
            End If

            Dim c As Char = input(i - 1)

            If isCmd Then
                '' command

                If c <> " " Then
                    cmd = cmd & c
                Else
                    isTrimming = True
                    isCmd = False
                End If
            Else
                '' argument

                If Not isTrimming Then
                    Dim appendChar As Boolean = False

                    If c = "\" Then
                        '' escape

                        If i < input.Length Then
                            charsToIgnore = 1

                            arg = arg & input(i)
                        Else
                            appendChar = True
                        End If
                    ElseIf c = endChar Then
                        '' end char reached => add argument

                        isTrimming = True

                        args.Add(arg)
                        arg = String.Empty
                    Else
                        appendChar = True
                    End If

                    If appendChar Then
                        arg = arg & c
                    End If
                Else
                    '' leading whitespaces

                    If c = " " Then
                        '' leading whitespace
                        Continue For
                    End If

                    isTrimming = False

                    If c = "\" Then
                        '' escape

                        If i < input.Length Then
                            Continue For
                        Else
                            arg = c
                        End If
                    ElseIf c = Chr(34) Then
                        '' argument starts with "

                        endChar = c
                        arg = String.Empty
                    Else
                        '' argument starts

                        endChar = " "
                        arg = c
                    End If
                End If
            End If
        Next

        If Not String.IsNullOrEmpty(arg) Then
            '' add last argument
            args.Add(arg)
        End If
    End Sub

    ''' <summary>
    ''' <see cref="ModeBase.OnDispose" />
    ''' </summary>
    Protected Overrides Sub OnDispose(disposing As Boolean)
        If disposing Then
            Me._CONN.Dispose()
            Me._CONTAINER.Dispose()
        End If
    End Sub

    ''' <summary>
    ''' <see cref="ModeBase.Run" />
    ''' </summary>
    Public Overrides Sub Run()
        While True
            Try
                Global.System.Console.Write("{0}@{1}:{2}> ", AppSettings.Username _
                                                           , AppSettings.Host, AppSettings.Port)

                Dim input As String = Global.System.Console.ReadLine()
                If String.IsNullOrWhiteSpace(input) Then
                    Continue While
                End If

                Dim args As IList(Of String) = Nothing

                Dim cmd As String = Nothing
                ExtractCommandLineArguments(input, cmd, args)

                If cmd IsNot Nothing Then
                    cmd = cmd.ToLower().Trim()
                End If

                '' find action
                Dim action As IConsoleModeAction = Me._CONTAINER _
                                                     .GetExportedValues(Of Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction)() _
                                                     .SingleOrDefault(Function(x)
                                                                          Dim names As IEnumerable(Of String) = x.Names
                                                                          If names Is Nothing Then
                                                                              names = Enumerable.Empty(Of String)()
                                                                          End If

                                                                          '' normalize
                                                                          names = names.Where(Function(y) Not String.IsNullOrWhiteSpace(y)) _
                                                                                       .Select(Function(y) y.ToLower().Trim())

                                                                          Return names.Any(Function(y) y = cmd)
                                                                      End Function)

                If action IsNot Nothing Then
                    action.Execute(Me._CONN, args)
                Else
                    Select Case cmd
                        Case "exit", "close"
                            Exit While
                    End Select
                End If
            Catch ex As Exception
                Dim innerEx As Exception = ex.GetBaseException()
                If innerEx Is Nothing Then
                    innerEx = ex
                End If

                ConsoleHelper.InvokeForColor(Sub()
                                                 Global.System.Console.WriteLine()

                                                 Global.System.Console.WriteLine(innerEx)

                                                 Global.System.Console.WriteLine()
                                             End Sub, foreColor:=ConsoleColor.Red)
            End Try
        End While
    End Sub

#End Region

End Class