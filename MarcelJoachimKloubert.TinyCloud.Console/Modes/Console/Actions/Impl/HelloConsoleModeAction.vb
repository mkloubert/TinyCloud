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

''' <summary>
''' Hello action.
''' </summary>
<Export(GetType(Global.MarcelJoachimKloubert.TinyCloud.Console.IConsoleModeAction))>
<PartCreationPolicy(CreationPolicy.NonShared)>
Public NotInheritable Class HelloConsoleModeAction
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
            Return New String() {"hello"}
        End Get
    End Property

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' <see cref="ConsoleModeActionBase.Execute" />
    ''' </summary>
    Public Overrides Sub Execute(conn As CloudConnection, args As IList(Of String))
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

#End Region

End Class