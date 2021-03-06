﻿''  TinyCloud Console (https://github.com/mkloubert/TinyCloud)
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
