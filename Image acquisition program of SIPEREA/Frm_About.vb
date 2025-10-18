Public Class Frm_About

    Private Sub Me_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub Frm_About_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Hide()
    End Sub

    Public Sub Frm_About_Click(sender As Object, e As EventArgs) Handles MyBase.Click
        Me.Hide()
    End Sub

End Class