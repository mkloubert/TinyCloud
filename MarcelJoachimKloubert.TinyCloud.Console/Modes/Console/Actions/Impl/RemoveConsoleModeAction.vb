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

Imports System.ComponentModel.Composition
Imports MarcelJoachimKloubert.TinyCloud.SDK
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports SysConsole = System.Console
Imports System.Linq
Imports System.Net
Imports System.IO
Imports System.Text

''' <summary>
''' Action for clearing the console screen.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class RemoveConsoleModeAction
    Inherits ConsoleModeActionBase

#Region "Constructors (1)"

    ''' <inheriteddoc />
    <ImportingConstructor>
    Public Sub New(mode As ConsoleMode)
        MyBase.New(mode)
    End Sub

#End Region

#Region "Properties (2)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Names" />
    ''' </summary>
    Public Overrides ReadOnly Property Names As IEnumerable(Of String)
        Get
            Return New String() {"rm", "remove"}
        End Get
    End Property

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.ShortDescription" />
    ''' </summary>
    Public Overrides ReadOnly Property ShortDescription As String
        Get
            Return "Removes one or more directory / file."
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        For Each a In args.Where(Function(x) Not String.IsNullOrWhiteSpace(x)) _
                          .Select(Function(x) Me.GetFullPath(x))

            Try
                Dim enc As Encoding = AppServices.Charset

                Dim request As WebRequest = conn.CreateApiRequest("remove")
                request.Method = "POST"

                request.ContentType = "text/plain; charset=" + enc.WebName
                Using reqStream As Stream = request.GetRequestStream()
                    Using writer As StreamWriter = New StreamWriter(reqStream, enc)
                        writer.Write(a)

                        reqStream.Flush()
                    End Using
                End Using

                Dim response As IDictionary(Of String, Object) = request.GetResponse().GetJson()
                If response Is Nothing Then
                    ConsoleHelper.InvokeForColor(Sub()
                                                     SysConsole.WriteLine("Got NO result!")
                                                 End Sub, ConsoleColor.Yellow)

                    Return
                End If
            Catch ex As Exception
                ShowException(ex)
            End Try
        Next
    End Sub

#End Region

End Class