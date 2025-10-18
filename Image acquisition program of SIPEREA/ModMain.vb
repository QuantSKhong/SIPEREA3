Imports System.IO

Module ModMain
    Public PathDelimit As String = Path.DirectorySeparatorChar
    Public IdleExit As Boolean = False

    Public Sub IdleInSec(DurationSec As Integer)
        For d As Integer = 1 To DurationSec
            For s As Integer = 1 To 100
                If IdleExit Then
                    Exit Sub
                End If
                Threading.Thread.Sleep(10)
                Application.DoEvents()
            Next
        Next
    End Sub

    Public Sub IdleInMS(DurationMSec As Integer)
        Threading.Thread.Sleep(DurationMSec)
        Application.DoEvents()
    End Sub
End Module
