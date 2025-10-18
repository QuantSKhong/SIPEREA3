Imports System.Math


Public Class MathLibrary

    Public Function Convert_Formatted_HMS_To_TimeSec(ByVal h As Integer, ByVal m As Integer, ByVal s As Integer) As Integer
        Return h * 3600 + m * 60 + s
    End Function
End Class
