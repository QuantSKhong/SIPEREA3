Imports System.Net
Imports System.IO


Public Class WebClientAsync

    Structure Type_ClientInfo
        Dim IsDownloading As Boolean
        Dim SourceURL As String
        Dim TagStr As String
        Dim TimeRequested As Date
        Dim TimeCompleted As Date
        Dim TimeLimitSec As Integer
    End Structure


    Public MyClientsCount As Integer = 50
    Public MyClients(MyClientsCount) As WebClient
    Public MyClientsInfo(MyClientsCount) As Type_ClientInfo
    Public WithEvents Timer_DownloadChecker As New Timer

    Public Event OneTaskDone(ByVal SourceURL As String,
                             ByVal SourceURLNum As Integer,
                             ByVal TagStr As String,
                             ByVal pBitmapBytes As Byte(),
                             ByVal TimeRequested As Date,
                             ByVal TimeCompleted As Date,
                             ByVal ErrorMsg As String)

    Public Event ProgressChanged(ByVal Percent As Single)



    Public Sub New()
        For q As Integer = 0 To MyClientsCount
            MyClients(q) = New WebClient
            AddHandler MyClients(q).DownloadDataCompleted, AddressOf OnDownloadComplete
            AddHandler MyClients(q).DownloadProgressChanged, AddressOf DownloadProgressChanged
            MyClientsInfo(q).IsDownloading = False
        Next

        Timer_DownloadChecker.Interval = 250
        Timer_DownloadChecker.Enabled = False
    End Sub


    'If avilable client not found, return false
    Public Function Download_Image(ByVal ImageURL As String,
                                   ByVal ImageURLNum As Integer,
                                   ByVal TagStr As String,
                                   ByVal TimeLimitSec As Integer) As Boolean

        If MyClientsInfo(ImageURLNum).IsDownloading OrElse MyClients(ImageURLNum).IsBusy Then
            Return False
        End If

        With MyClientsInfo(ImageURLNum)
            .IsDownloading = True
            .TagStr = TagStr
            .SourceURL = ImageURL
            .TimeRequested = Now
            .TimeLimitSec = TimeLimitSec
        End With
        MyClients(ImageURLNum).DownloadDataAsync(New Uri(ImageURL))


        Timer_DownloadChecker.Enabled = True

        Return True
    End Function

    Private Sub DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
        RaiseEvent ProgressChanged(e.ProgressPercentage)
    End Sub

    Private Sub OnDownloadComplete(sender As Object, e As DownloadDataCompletedEventArgs)
        Dim cIndex As Integer = Get_MyClientIndex(sender)
        If cIndex = -1 Then
            MsgBox("Critical error!")
            Exit Sub
        End If

        If Not (e.Cancelled) AndAlso (e.Error Is Nothing) Then

            With MyClientsInfo(cIndex)
                .TimeCompleted = Now

                RaiseEvent OneTaskDone(.SourceURL,
                                       cIndex,
                                       .TagStr,
                                       e.Result,
                                       .TimeRequested,
                                       .TimeCompleted,
                                       "")
                .IsDownloading = False
                .SourceURL = ""
            End With

        Else
            With MyClientsInfo(cIndex)
                .TimeCompleted = Now

                RaiseEvent OneTaskDone(.SourceURL,
                                       cIndex,
                                       .TagStr,
                                       Nothing,
                                       .TimeRequested,
                                       .TimeCompleted,
                                       "Aborted or error")
                .IsDownloading = False
                .SourceURL = ""
            End With
        End If


        If IsAllClientIdle() Then
            Timer_DownloadChecker.Enabled = False
        End If
    End Sub



    Public Function IsAllClientIdle() As Integer
        For q As Integer = 0 To MyClientsCount
            If MyClientsInfo(q).IsDownloading Then
                Return False
            End If
        Next

        Return True
    End Function


    'if not found, return -1
    Function Get_MyClientIndex(sender As Object) As Integer
        For q As Integer = 0 To MyClientsCount
            If sender Is MyClients(q) Then
                Return q
            End If
        Next

        Return -1
    End Function



    Private Sub Timer_DownloadChecker_Tick(sender As Object, e As EventArgs) Handles Timer_DownloadChecker.Tick
        For q As Integer = 0 To MyClientsCount
            With MyClientsInfo(q)
                If .IsDownloading Then
                    If Math.Abs(DateDiff(DateInterval.Second, .TimeRequested, Now)) > .TimeLimitSec Then
                        MyClients(q).CancelAsync()
                    End If
                End If
            End With
        Next
    End Sub

End Class
