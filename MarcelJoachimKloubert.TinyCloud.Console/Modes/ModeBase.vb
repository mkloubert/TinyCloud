Imports MarcelJoachimKloubert.TinyCloud.SDK

''' <summary>
''' A basic mode.
''' </summary>
Public MustInherit Class ModeBase : Inherits CloudDisposableBase : Implements IMode

#Region "Constructors (2)"

    ''' <inheriteddoc />
    Protected Sub New()
        MyBase.New()
    End Sub

    ''' <inheriteddoc />
    Protected Sub New(sync As Object)
        MyBase.New(sync)
    End Sub

#End Region

#Region "Methods (2)"

    ''' <summary>
    ''' <see cref="IMode.Run" />
    ''' </summary>
    Public MustOverride Sub Run() Implements IMode.Run

    ''' <summary>
    ''' Stores the logic for the <see cref="ModeBase.Dispose" /> method and the finalizer.
    ''' </summary>
    ''' <param name="disposing">
    ''' <see cref="ModeBase.Dispose" /> or <see cref="ModeBase.Finalize" /> method was called.
    ''' </param>
    Protected Overrides Sub OnDispose(disposing As Boolean)
        '' dummy
    End Sub

#End Region

End Class
