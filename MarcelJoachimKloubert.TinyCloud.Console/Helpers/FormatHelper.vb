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

''' <summary>
''' Helper clas for formatting operations.
''' </summary>
Module FormatHelper

#Region "Fields (1)"

    Private ReadOnly _FILESIZE_UNITS As String() = {"kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"}

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' Converts a value that represents a file size to a human readable string.
    ''' </summary>
    ''' <param name="size">The file size value.</param>
    ''' <returns>The value as string.</returns>
    Public Function ToHumanReadableFileSize(size As Long) As String
        Dim exp As Long = CType(Math.Floor(Math.Log(size, 1000.0R)), Long)

        If exp < 1 Then
            Return size.ToString()
        End If

        If exp >= _FILESIZE_UNITS.Length Then
            exp = _FILESIZE_UNITS.Length
        End If

        Return String.Format("{0:0.##} {1}", _
                             CType(size, Double) / Math.Pow(1000.0R, CType(exp, Double)), _
                             _FILESIZE_UNITS(exp - 1))
    End Function

#End Region

End Module