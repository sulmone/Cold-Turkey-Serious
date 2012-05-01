'    Copyright (c) 2011, 2012 Felix Belzile
'    Official software website: http://getcoldturkey.com
'    Contact: felixbelzile@rogers.com  Web: http://felixbelzile.com

'    This file is part of Cold Turkey
'
'    Cold Turkey is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    Cold Turkey is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with Cold Turkey.  If not, see <http://www.gnu.org/licenses/>.

Public Class listShowWarning

    Private Sub good_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles good.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub bad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bad.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub lastchance_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Visible = False
    End Sub
End Class