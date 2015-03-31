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
Imports System.ComponentModel.Composition
Imports System.Net
Imports MarcelJoachimKloubert.TinyCloud.SDK.Extensions
Imports SysConsole = System.Console
Imports System.Linq

''' <summary>
''' List directory action.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class ListConsoleModeAction
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
            Return New String() {"list", "ls", "dir"}
        End Get
    End Property

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.ShortDescription" />
    ''' </summary>
    Public Overrides ReadOnly Property ShortDescription As String
        Get
            Return "Lists a directory."
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, cmd As String, args As IList(Of String))
        Dim request As WebRequest = conn.CreateApiRequest("list")
        request.Method = "POST"

        Dim requestData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
        requestData("path") = ""

        request.SendJson(requestData)

        Dim response As IDictionary(Of String, Object) = request.GetResponse().GetJson()
        If response Is Nothing Then
            Return
        End If

        Dim code As Integer = Convert.ChangeType(response("code"), GetType(Integer), _
                                                 AppServices.DataCulture)

        Select Case code
            Case 0
                Dim data As IDictionary(Of String, Object) = response("data")
                If data Is Nothing Then
                    Return
                End If

                SysConsole.WriteLine("Directory: {0}", data("path"))

                SysConsole.WriteLine()

                For Each subDir In DirectCast(data("dirs"), IEnumerable).OfType(Of Object)() _
                                                                        .OrderBy(Function(x) CType(x.name, String), StringComparer.InvariantCultureIgnoreCase)

                    Dim name As String = subDir.name
                    Dim lastWriteTime As Date = subDir.lastWriteTime

                    SysConsole.Write("{0:yyyy-MM-dd}  {0:HH:mm:ss}", lastWriteTime)
                    SysConsole.Write("    ")
                    SysConsole.Write("<DIR>".PadRight(14, " "))
                    SysConsole.Write("  {0}", name)

                    SysConsole.WriteLine()
                Next

                For Each file In DirectCast(data("files"), IEnumerable).OfType(Of Object)() _
                                                                       .OrderBy(Function(x) CType(x.name, String), StringComparer.InvariantCultureIgnoreCase)

                    Dim name As String = file.name
                    Dim lastWriteTime As Date = file.lastWriteTime
                    Dim size As Long = file.size

                    SysConsole.Write("{0:yyyy-MM-dd}  {0:HH:mm:ss}", lastWriteTime)
                    SysConsole.Write("    {0}", size.ToString("n0", AppServices.DataCulture) _
                                                    .PadLeft(14))
                    SysConsole.Write("  {0}", name)

                    SysConsole.WriteLine()
                Next
                Exit Select

            Case 404
                Exit Select
        End Select
    End Sub

#End Region

End Class