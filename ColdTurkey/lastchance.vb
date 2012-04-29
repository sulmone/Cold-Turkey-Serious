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