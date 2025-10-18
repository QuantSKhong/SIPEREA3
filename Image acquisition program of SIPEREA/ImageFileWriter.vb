Imports System.IO

Public Class ImageFileWriter

    Public FileSaveDialog As New SaveFileDialog


    Public Function WriteBytes(ByVal SourceImageBytes As Byte(), ByVal DestFileName As String) As String
        Try
            IO.File.WriteAllBytes(DestFileName, SourceImageBytes)
        Catch
            Return "cannot save image bytes"
        End Try

        Return ""
    End Function
End Class
