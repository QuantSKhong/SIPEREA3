Imports System.ComponentModel
Imports System.IO

Public Class ImageFileWriterAsync
    Structure Type_LastBackgroundWorkerMessage
        Dim PathName As String
        Dim FileName As String
        Dim TimeRequested As Date
        Dim ErrorMsg As String
    End Structure

    Public WriteQueueSize As Integer = 20
    Public WriteQueue As New Queue


    Public WithEvents QueueCheckTimer As New Timer
    Public IsSavingReady As Boolean = True

    Public CurPathName As String
    Public CurFileName As String

    Dim WithEvents MyBackgroundWriter As New BackgroundWorker
    Dim WriterSyn As New ImageFileWriter


    Public LastBackgroundWorkerMessage As New Type_LastBackgroundWorkerMessage
    Public Event OneTaskDone(PathName As String,
                             FileName As String,
                             TimeRequested As Date,
                             ErrorMsg As String)



    Public Sub New()
        With QueueCheckTimer
            .Interval = 50
            .Enabled = False
        End With
    End Sub


    Public Function Write_Image(PathName As String,
                                FileName As String,
                                ByVal pImage As Image,
                                Optional ByRef pImageCompRate As Integer = 95) As String

        Dim TempObject As Object = New Object() {
                                       PathName,
                                       FileName,
                                       pImage,
                                       Now}


        CurPathName = PathName
        CurFileName = FileName

        If WriteQueue.Count >= WriteQueueSize Then
            Return "More than Queue limit"
        End If



        WriteQueue.Enqueue(TempObject)
        QueueCheckTimer.Enabled = True

        Return ""
    End Function

    Public Function Write_ImageStream(PathName As String,
                                FileName As String,
                                ByVal pImageStream As MemoryStream,
                                Optional ByRef pImageCompRate As Integer = 95) As String

        Dim TempObject As Object = New Object() {
                                       PathName,
                                       FileName,
                                       pImageStream,
                                       Now}


        CurPathName = PathName
        CurFileName = FileName

        If WriteQueue.Count >= WriteQueueSize Then
            Return "More than Queue limit"
        End If



        WriteQueue.Enqueue(TempObject)
        QueueCheckTimer.Enabled = True

        Return ""
    End Function

    Public Function Write_ImageBytes(PathName As String,
                                FileName As String,
                                ByVal pImageBytes As Byte(),
                                Optional ByRef pImageCompRate As Integer = 95) As String

        Dim TempObject As Object = New Object() {
                                       PathName,
                                       FileName,
                                       pImageBytes,
                                       Now}


        CurPathName = PathName
        CurFileName = FileName

        If WriteQueue.Count >= WriteQueueSize Then
            Return "More than Queue limit"
        End If



        WriteQueue.Enqueue(TempObject)
        QueueCheckTimer.Enabled = True

        Return ""
    End Function


    Private Sub MyBackgroundWriter_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles MyBackgroundWriter.RunWorkerCompleted
        Dim args As Object() = DirectCast(e.Result, Object())

        With LastBackgroundWorkerMessage
            .PathName = DirectCast(args(0), String)
            .FileName = DirectCast(args(1), String)
            .TimeRequested = DirectCast(args(2), Date)
            .ErrorMsg = DirectCast(args(3), String)

            RaiseEvent OneTaskDone(.PathName,
                                   .FileName,
                                   .TimeRequested,
                                   .ErrorMsg)
        End With



        QueueCheckTimer.Enabled = True
        IsSavingReady = True
    End Sub

    Private Sub BackgroundWriter_DoWork(sender As Object, e As DoWorkEventArgs) Handles MyBackgroundWriter.DoWork
        IsSavingReady = False


        Dim args As Object() = DirectCast(e.Argument, Object())

        Dim PathName As String = DirectCast(args(0), String)
        Dim FileName As String = DirectCast(args(1), String)
        Dim pImageBytes As Byte() = DirectCast(args(2), Byte())
        Dim TimeRequested As Date = DirectCast(args(3), Date)


        Dim RetStr As String =
                WriterSyn.WriteBytes(pImageBytes, PathName + PathDelimit + FileName)
        If RetStr = "" Then
            Dim TempObject As Object = New Object() {
                                   PathName,
                                   FileName,
                                   TimeRequested,
                                   ""}
            e.Result = TempObject

        Else
            Dim TempObject As Object = New Object() {
                                   PathName,
                                   FileName,
                                   TimeRequested,
                                   RetStr}
            e.Result = TempObject
        End If

    End Sub

    Private Sub UploadQueueCheckTimer_Tick(sender As Object, e As EventArgs) Handles QueueCheckTimer.Tick
        If WriteQueue.Count = 0 Then
            QueueCheckTimer.Enabled = False
            Exit Sub
        End If


        If IsSavingReady Then
            If MyBackgroundWriter.IsBusy = False Then
                QueueCheckTimer.Enabled = False
                Call MyBackgroundWriter.RunWorkerAsync(WriteQueue.Dequeue())
            End If
        End If
    End Sub
End Class
