Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        End
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        Try
            Process.Start(Application.StartupPath & "\ColdTurkey.exe")
            End
        Catch ex As Exception
            MsgBox("Could not locate ColdTurkey's main executable.")
            MsgBox(ex.Message)
        End Try
        End
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class
