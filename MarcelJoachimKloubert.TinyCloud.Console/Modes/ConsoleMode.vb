Imports System.Text.RegularExpressions
Imports MarcelJoachimKloubert.TinyCloud.SDK
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports System.Net

''' <summary>
''' Console mode.
''' </summary>
Public NotInheritable Class ConsoleMode
    Inherits ModeBase

#Region "Fields (2)"

    Private ReadOnly _CONN As CloudConnection

    ''' <summary>
    ''' The regular expression to split command line arguments.
    ''' </summary>
    Public Shared REGEX_COMMAND_LINE_ARGUMENTS As Regex = New Regex("(?<quote>[" & Chr(34) & "]?)(?<param>(?:\k<quote>{2}|[^" & Chr(34) & "]+)*)\k<quote>[ ]+", _
                                                                    (RegexOptions.ECMAScript Or RegexOptions.IgnoreCase) Or RegexOptions.Compiled)

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ConsoleMode" /> class.
    ''' </summary>
    Public Sub New()
        Me._CONN = AppSettings.CreateConnection()
    End Sub

#End Region

#Region "Methods (5)"

    ''' <summary>
    ''' <see cref="ModeBase.OnDispose" />
    ''' </summary>
    Protected Overrides Sub OnDispose(disposing As Boolean)
        If disposing Then
            Me._CONN.Dispose()
        End If
    End Sub

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

                Dim action As Action(Of CloudConnection, IList(Of String)) = Nothing
                Dim args As IList(Of String)

                Dim cmd As String
                ExtractCommandLineArguments(input, cmd, args)

                Select Case cmd.ToLower().Trim()
                    Case "cls"
                        Global.System.Console.Clear()
                        Continue While

                    Case "exit"
                        Exit While

                    Case "hello"
                        action = AddressOf Me.cmd_hello
                        Exit Select

                    Case "test_args"
                        action = AddressOf Me.cmd_testArgs
                        Exit Select
                End Select

                If action IsNot Nothing Then
                    action(Me._CONN, args)
                End If
            Catch ex As Exception

            End Try
        End While
    End Sub

    Private Sub cmd_hello(conn As CloudConnection, args As IList(Of String))
        Dim request As WebRequest = conn.CreateApiRequest("hello")
        request.Method = "GET"

        Dim response As IDictionary(Of String, Object) = request.GetResponse().GetJson()
        If response Is Nothing Then
            Return
        End If

        Dim data As IDictionary(Of String, Object) = response("data")
        If data Is Nothing Then
            Return
        End If

        Global.System.Console.WriteLine()

        If data.ContainsKey("version") Then
            Global.System.Console.WriteLine("Version: {0}", data("version"))
        End If

        Global.System.Console.WriteLine()
    End Sub

    Private Sub cmd_testArgs(conn As CloudConnection, args As IList(Of String))
        Global.System.Console.WriteLine()

        For i = 1 To args.Count
            Dim a As String = args(i - 1)

            Global.System.Console.WriteLine("[{0}] => ""{1}"" ({2})", _
                                            i - 1, _
                                            a, a.Length)
        Next

        Global.System.Console.WriteLine()
    End Sub

#End Region

End Class