Public Class UpdateFacebookForm

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Visible = False
    End Sub


    Private Sub WebBrowser1_DocumentTitleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles WebBrowser1.DocumentTitleChanged
        If WebBrowser1.DocumentTitle = "Done" Then
            Me.Dispose()
        End If
    End Sub
End Class