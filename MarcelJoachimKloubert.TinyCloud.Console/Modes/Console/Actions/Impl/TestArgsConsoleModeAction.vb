Imports System.ComponentModel.Composition
Imports MarcelJoachimKloubert.TinyCloud.SDK

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

#Region "Properties (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Names" />
    ''' </summary>
    Public Overrides ReadOnly Property Names As IEnumerable(Of String)
        Get
            Return New String() {"test_args", "testargs"}
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, args As IList(Of String))
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
