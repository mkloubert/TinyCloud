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
Imports SysConsole = System.Console
Imports System.IO

''' <summary>
''' Console mode.
''' </summary>
Public NotInheritable Class ConsoleMode
    Inherits ModeBase

#Region "Fields (4)"

    Private ReadOnly _CONN As CloudConnection
    Private ReadOnly _CONTAINER As CompositionContainer
    Private _currentDirectory As String
    Private _currentLocalDirectory As DirectoryInfo

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

        Me.CurrentDirectory = Nothing
        Me.CurrentLocalDirectory = Nothing
    End Sub

#End Region

#Region "Properties (2)"

    ''' <summary>
    ''' Gets or sets the current directory.
    ''' </summary>
    Public Property CurrentDirectory As String
        Get
            Return Me._currentDirectory
        End Get

        Set(value As String)
            If String.IsNullOrWhiteSpace(value) Then
                value = "/"
            Else
                value = value.Trim()
            End If

            Me._currentDirectory = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the current local directory.
    ''' </summary>
    Public Property CurrentLocalDirectory As DirectoryInfo
        Get
            Return Me._currentLocalDirectory
        End Get

        Set(value As DirectoryInfo)
            If value Is Nothing Then
                value = New DirectoryInfo(Environment.CurrentDirectory)
            End If

            value.Refresh()

            Me._currentLocalDirectory = value
        End Set
    End Property

#End Region

#Region "Methods (6)"

    Private Shared Sub ExtractCommandLineArguments(input As String, ByRef cmd As String, ByRef args As IList(Of String))
        cmd = String.Empty
        args = New List(Of String)()

        If String.IsNullOrWhiteSpace(input) Then
            Return
        End If

        input = input.TrimStart()

        Const ENVELOP_CHAR As Char = Chr(34)
        Const ESCAPE_CHAR As Char = "\"
        Const TRIM_CHAR As Char = " "

        Dim isCmd As Boolean = True
        Dim isTrimming As Boolean = True
        Dim endChar As Char = " "
        Dim arg As String = String.Empty
        Dim charsToIgnore As Integer = 0

        Dim nonRefArgs As IList(Of String) = args
        Dim addArg As Action = Sub()
                                   If Not String.IsNullOrEmpty(arg) Then
                                       nonRefArgs.Add(arg)
                                   End If
                               End Sub

        For i = 1 To input.Length
            If charsToIgnore > 0 Then
                charsToIgnore = charsToIgnore - 1
                Continue For
            End If

            Dim c As Char = input(i - 1)

            Dim handleEscape As Func(Of String, String) = Function(initialResult)
                                                              Dim nextChar As Char = input(i)
                                                              Dim resultValue As String = initialResult

                                                              If nextChar = endChar Or nextChar = ESCAPE_CHAR Then
                                                                  charsToIgnore = 1

                                                                  resultValue = nextChar
                                                              Else
                                                                  Dim simpleEscapeChars = New Dictionary(Of Char, String) From {{"n", vbLf}, _
                                                                                                                                {"r", vbCr}, _
                                                                                                                                {"t", vbTab}, _
                                                                                                                                {"0", vbNullChar}, _
                                                                                                                                {"b", vbBack}}

                                                                  If simpleEscapeChars.ContainsKey(nextChar) Then
                                                                      '' escape char

                                                                      charsToIgnore = 1
                                                                      resultValue = simpleEscapeChars(nextChar)
                                                                  ElseIf nextChar = "u" Or nextChar = "x" Then
                                                                      '' unicode char

                                                                      charsToIgnore = 1

                                                                      '' extract hex part
                                                                      Dim hexStr As String = String.Empty
                                                                      For ii As Integer = 1 To 4
                                                                          Dim hexCharPos As Integer = i + ii
                                                                          If hexCharPos >= input.Length Then
                                                                              Exit For
                                                                          End If

                                                                          Dim hexChar = Char.ToLower(input(hexCharPos))
                                                                          If (hexChar >= "0" AndAlso hexChar <= "9") Or _
                                                                             (hexChar >= "a" AndAlso hexChar <= "f") Then

                                                                              hexStr = hexStr & hexChar
                                                                              charsToIgnore = charsToIgnore + 1
                                                                          Else
                                                                              '' no valid hex character
                                                                              Exit For
                                                                          End If
                                                                      Next

                                                                      resultValue = "\" & nextChar & hexStr

                                                                      If nextChar = "x" Then
                                                                          '' normalize
                                                                          hexStr = hexStr.PadLeft(4, "0")
                                                                      End If

                                                                      If hexStr.Length = 4 Then
                                                                          Dim charValue As UShort = UShort.Parse(hexStr, Global.System.Globalization.NumberStyles.HexNumber, AppServices.DataCulture)

                                                                          resultValue = Convert.ChangeType(charValue, GetType(Char)) _
                                                                                               .ToString()
                                                                      End If
                                                                  End If
                                                              End If

                                                              Return resultValue
                                                          End Function

            If Not isCmd Then
                '' argument

                If Not isTrimming Then
                    Dim valueToAppend As String = String.Empty

                    If c = ESCAPE_CHAR Then
                        '' escape

                        If i < input.Length Then
                            valueToAppend = handleEscape(c)
                        Else
                            valueToAppend = c
                        End If
                    ElseIf c = endChar Then
                        '' end char reached => add argument

                        isTrimming = True

                        addArg()
                        arg = String.Empty
                    Else
                        valueToAppend = c
                    End If

                    arg = arg & valueToAppend
                Else
                    '' leading whitespaces

                    If c = TRIM_CHAR Then
                        '' leading whitespace
                        Continue For
                    End If

                    isTrimming = False

                    If c = ESCAPE_CHAR Then
                        '' escape
                        endChar = TRIM_CHAR

                        Dim initialArgValue As String = c

                        If i < input.Length Then
                            initialArgValue = handleEscape(c)
                        End If

                        arg = initialArgValue
                    ElseIf c = ENVELOP_CHAR Then
                        '' argument starts with "

                        endChar = c
                        arg = String.Empty
                    Else
                        '' argument starts

                        endChar = TRIM_CHAR
                        arg = c
                    End If
                End If
            Else
                '' command

                If c <> TRIM_CHAR Then
                    cmd = cmd & c
                Else
                    isTrimming = True
                    isCmd = False
                End If
            End If
        Next

        addArg()
    End Sub

    Private Function GetActions() As IEnumerable(Of IConsoleModeAction)
        Return Me._CONTAINER _
                 .GetExportedValues(Of Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction)()
    End Function

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
                SysConsole.Write("{0}@{1}:{2}> ", AppSettings.Username _
                                                , AppSettings.Host, AppSettings.Port)

                Dim input As String = SysConsole.ReadLine()
                If String.IsNullOrWhiteSpace(input) Then
                    Continue While
                End If

                Dim cmd As String = Nothing
                Dim args As IList(Of String) = Nothing
                ExtractCommandLineArguments(input, cmd, args)

                '' find action
                Dim action As IConsoleModeAction = Me.TryFindAction(cmd)

                If action IsNot Nothing Then
                    SysConsole.WriteLine()

                    action.Execute(Me._CONN, cmd, args)

                    SysConsole.WriteLine()
                Else
                    Select Case cmd
                        Case "exit", "close"
                            '' exit application
                            Exit While

                        Case "help", "?"
                            Dim actionsToDisplay = Me.GetActions() _
                                                     .Where(Function(x) x.ShowInHelp) _
                                                     .ToList()

                            Dim normalizedNames As IList(Of String) = args.Where(Function(x) Not String.IsNullOrWhiteSpace(x)) _
                                                                          .Select(Function(x) x.ToLower().Trim()) _
                                                                          .Distinct() _
                                                                          .ToList()

                            '' show help
                            If (normalizedNames.Count < 1) Then
                                SysConsole.WriteLine()
                                SysConsole.WriteLine("Avaiable commands:")

                                '' list available commands / actions
                                For Each a As IConsoleModeAction In actionsToDisplay.Where(Function(x) If(x.Names, Enumerable.Empty(Of String)).OfType(Of String) _
                                                                                                                                               .FirstOrDefault() IsNot Nothing) _
                                                                                    .OrderBy(Function(x) x.Names.FirstOrDefault(), _
                                                                                             StringComparer.InvariantCultureIgnoreCase)

                                    SysConsole.WriteLine()

                                    ConsoleHelper.InvokeForColor(Sub()
                                                                     SysConsole.Write("  * {0}", a.Names.First())
                                                                 End Sub, foreColor:=ConsoleColor.White)

                                    SysConsole.WriteLine()

                                    ConsoleHelper.InvokeForColor(Sub()
                                                                     SysConsole.Write("    {0}", a.ShortDescription)
                                                                 End Sub, foreColor:=ConsoleColor.Gray)

                                    SysConsole.WriteLine()
                                Next

                                SysConsole.WriteLine()
                            Else
                                '' show help of commands / actions 

                                For Each name As String In normalizedNames
                                    Dim a = Me.TryFindAction(name)
                                    If a Is Nothing Then
                                        ''TODO: show warning

                                        Continue For
                                    End If

                                    SysConsole.WriteLine()

                                    a.ShowHelp()

                                    SysConsole.WriteLine()
                                Next
                            End If
                            Exit Select

                        Case Else
                            '' unknown command
                            ConsoleHelper.InvokeForColor(Sub()
                                                             SysConsole.WriteLine()

                                                             SysConsole.Write("Unknown command """)
                                                         End Sub, foreColor:=ConsoleColor.Yellow)

                            ConsoleHelper.InvokeForColor(Sub()
                                                             SysConsole.Write(cmd)
                                                         End Sub, foreColor:=ConsoleColor.White)

                            ConsoleHelper.InvokeForColor(Sub()
                                                             SysConsole.WriteLine("""!")

                                                             SysConsole.WriteLine()
                                                         End Sub, foreColor:=ConsoleColor.Yellow)
                            Exit Select
                    End Select
                End If
            Catch ex As Exception
                ShowException(ex)
            End Try
        End While
    End Sub

    ''' <summary>
    ''' Outputs an exception.
    ''' </summary>
    ''' <param name="ex">The exception to output.</param>
    Public Shared Sub ShowException(ex As Exception)
        If ex Is Nothing Then
            Return
        End If

        Dim innerEx As Exception = Nothing
        Try
            innerEx = If(ex.GetBaseException(), ex)
        Catch ex2 As Exception
            innerEx = ex
        End Try

        ConsoleHelper.InvokeForColor(Sub()
                                         SysConsole.WriteLine()

                                         SysConsole.WriteLine(innerEx)

                                         SysConsole.WriteLine()
                                     End Sub, foreColor:=ConsoleColor.Red)
    End Sub

    Private Function TryFindAction(cmd As String) As IConsoleModeAction
        If cmd IsNot Nothing Then
            cmd = cmd.ToLower().Trim()
        End If

        Return Me.GetActions() _
                 .SingleOrDefault(Function(x)
                                      Dim names As IEnumerable(Of String) = If(x.Names, Enumerable.Empty(Of String)())

                                      '' normalize
                                      names = names.Where(Function(y) Not String.IsNullOrWhiteSpace(y)) _
                                                   .Select(Function(y) y.ToLower().Trim())

                                      Return names.Any(Function(y) y = cmd)
                                  End Function)
    End Function

#End Region

End Class