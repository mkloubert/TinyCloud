Imports MarcelJoachimKloubert.TinyCloud.SDK
Imports System.Net
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions

''' <summary>
''' The program class.
''' </summary>
Module Program

#Region "Methods (1)"

    ''' <summary>
    ''' The entry point.
    ''' </summary>
    ''' <param name="args">The submitted arguments.</param>
    Sub Main(args As String())
        Try
            Using mode As IMode = AppSettings.CreateMode()
                If mode IsNot Nothing Then
                    mode.Run()
                Else
                    ''TODO: show warning
                End If
            End Using
        Catch ex As Exception

        End Try
    End Sub

#End Region

End Module