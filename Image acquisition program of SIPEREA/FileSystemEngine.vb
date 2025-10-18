Imports System.IO
Imports System.Text

Public Class FileSystemEngine

    Public Function Get_TheLastFolder_From_FullPath(ByVal FullPath As String) As String
        Dim FoundTheLastFolder As String
        Dim FoundTheFolders() As String
        If FullPath = "" Then Return Nothing


        If Strings.Right(FullPath, 1) = System.IO.Path.DirectorySeparatorChar Then
            FullPath = Mid(FullPath, 1, FullPath.Length - 1)
        End If

        FoundTheFolders = FullPath.Split(Path.DirectorySeparatorChar)
        FoundTheLastFolder = FoundTheFolders(FoundTheFolders.GetUpperBound(0))

        Return FoundTheLastFolder
    End Function

End Class
