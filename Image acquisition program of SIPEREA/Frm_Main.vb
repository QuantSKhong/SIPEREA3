Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.Windows.Forms.VisualStyles

Public Class Frm_Main

    Public WithEvents MyImageFileWriterAsync As New ImageFileWriterAsync
    Public WithEvents MyWebClientAsync As New WebClientAsync
    Public WithEvents MyMath As New MathLibrary
    Public WithEvents MyFileSys As New FileSystemEngine


    Dim ScheduledTimeArrayInSec(24 * 60 * 60) As Boolean
    Dim FinalTimeDigit As Integer
    Dim IsContinuous As Boolean
    Dim DurationMin As Integer
    Dim StartTime As New DateTime

    Private Sub Frm_Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Threading.Thread.CurrentThread.CurrentCulture =
                        New Globalization.CultureInfo("en-US", True)

        Combo_Sec.Text = "1"
        Combo_Min.Text = "1"
        Combo_Hour.Text = "1"

        Combo_AbortAfter.Text = "10"

    End Sub

    Sub ProcessCommandLineArugments()
        Dim arguments As String() = Environment.GetCommandLineArgs()

        If arguments.Length > 1 Then
            If arguments(1).ToString.ToLower = "START".ToLower Then
                Cmd_Start_Click(Nothing, Nothing)
            End If
        End If
    End Sub


    Public Sub SaveSetting()
        With My.Settings
            .ImageURLs = Text_ImageURL.Text
            .IsEverySecond = Radio_Sec.Checked
            .IsEveryMinute = Radio_Min.Checked
            .IsEveryHour = Radio_Hour.Checked
            .IsUserDefined = Radio_Schedule.Checked
            .IsEverySecondNumb = Combo_Sec.Text
            .IsEveryMinuteNumb = Combo_Min.Text
            .IsEveryHourNumb = Combo_Hour.Text
            .IsUserDefinedText = Text_Schedule.Text
            .AbortAfter = Combo_AbortAfter.Text
            .IsContinuousCapture = Check_Continuous.Checked
            .IsContinuousCaptureNumb = Text_DurationMin.Text
            .IsSaveImage = Check_Save.Checked
            .DestinationFolder = Text_TargetFolder.Text
            .TargetDevice = Text_TargetDeviceID.Text
            .Illumination = Check_Illumination.Checked
        End With
        My.Settings.Save()
    End Sub


    Public Sub LoadSetting()
        With My.Settings
            Text_ImageURL.Text = .ImageURLs
            Radio_Sec.Checked = .IsEverySecond
            Radio_Min.Checked = .IsEveryMinute
            Radio_Hour.Checked = .IsEveryHour
            Radio_Schedule.Checked = .IsUserDefined
            Combo_Sec.Text = .IsEverySecondNumb
            Combo_Min.Text = .IsEveryMinuteNumb
            Combo_Hour.Text = .IsEveryHourNumb
            Text_Schedule.Text = .IsUserDefinedText
            Combo_AbortAfter.Text = .AbortAfter
            Check_Continuous.Checked = .IsContinuousCapture
            Text_DurationMin.Text = .IsContinuousCaptureNumb
            Check_Save.Checked = .IsSaveImage
            Text_TargetFolder.Text = .DestinationFolder
            Text_TargetDeviceID.Text = .TargetDevice
            Check_Illumination.Checked = .Illumination
        End With
    End Sub


    Private Sub Radio_Schedule_CheckedChanged_1(sender As Object, e As EventArgs) Handles Radio_Schedule.CheckedChanged
        Text_Schedule.Enabled = Radio_Schedule.Checked

        Check_RadioButtons()
    End Sub

    Private Sub Check_Continuous_CheckedChanged_1(sender As Object, e As EventArgs) Handles Check_Continuous.CheckedChanged
        Text_DurationMin.Visible = Not (Check_Continuous.Checked)
        Label_DurationMin.Visible = Not (Check_Continuous.Checked)
    End Sub


    Private Sub Radio_Sec_CheckedChanged_1(sender As Object, e As EventArgs) Handles Radio_Sec.CheckedChanged
        Check_RadioButtons()
    End Sub

    Private Sub Radio_Min_CheckedChanged_1(sender As Object, e As EventArgs) Handles Radio_Min.CheckedChanged
        Check_RadioButtons()
    End Sub

    Private Sub Radio_Hour_CheckedChanged_1(sender As Object, e As EventArgs) Handles Radio_Hour.CheckedChanged
        Check_RadioButtons()
    End Sub

    Private Sub Check_RadioButtons()
        If Radio_Sec.Checked Then
            Combo_Sec.Enabled = True
            Combo_Min.Enabled = False
            Combo_Hour.Enabled = False
            FinalTimeDigit = -1
        Else
            If Radio_Min.Checked Then
                Combo_Sec.Enabled = False
                Combo_Min.Enabled = True
                Combo_Hour.Enabled = False
                FinalTimeDigit = -1
            Else
                If Radio_Hour.Checked Then
                    Combo_Sec.Enabled = False
                    Combo_Min.Enabled = False
                    Combo_Hour.Enabled = True
                    FinalTimeDigit = -1
                Else
                    Combo_Sec.Enabled = False
                    Combo_Min.Enabled = False
                    Combo_Hour.Enabled = False
                    FinalTimeDigit = -1
                End If
            End If
        End If


    End Sub

    Private Sub Cmd_Start_Click(sender As Object, e As EventArgs) Handles Cmd_Start.Click
        If Radio_Schedule.Checked Then
            If CreateScheduledTimeList(Text_Schedule.Text) = False Then
                MsgBox("Something wrong in the User Defined Schedule!", MsgBoxStyle.Information, "Error")
                Exit Sub
            End If
        End If


        FinalTimeDigit = -1

        If Check_Continuous.Checked Then
            IsContinuous = True
        Else
            IsContinuous = False
            StartTime = DateTime.Now
            Try
                DurationMin = CInt(Val(Text_DurationMin.Text))
            Catch
                IsContinuous = False
                Check_Continuous.Checked = True
            End Try
        End If

        List_Status.Items.Clear()
        List_DownloadFail.Items.Clear()

        Cmd_Start.Enabled = False
        Cmd_Stop.Enabled = True

        Timer_CheckTime.Enabled = True
    End Sub

    'if any error found, return false
    Private Function CreateScheduledTimeList(InputTimeSchedule As String) As Boolean

        If InputTimeSchedule = "" Then Return False


        ReDim ScheduledTimeArrayInSec(24 * 60 * 60)



        Dim IndividualTime() As String = Split(InputTimeSchedule, vbCrLf)

        For q As Integer = 0 To IndividualTime.GetUpperBound(0)

            'Check grammar error
            Dim w As String = IndividualTime(q)


            If w = "" Then
                Continue For
            Else
                If w.Length <> 27 Then Return False

                If w.Substring(2, 1) <> ":" OrElse w.Substring(5, 1) <> ":" OrElse w.Substring(8, 1) <> "-" OrElse
                   w.Substring(11, 1) <> ":" OrElse w.Substring(14, 1) <> ":" OrElse w.Substring(17, 1) <> "(" OrElse
                   w.Substring(20, 1) <> ":" OrElse w.Substring(23, 1) <> ":" OrElse w.Substring(26, 1) <> ")" Then
                    Return False
                End If

            End If

            Dim h1, m1, s1, h2, m2, s2, h3, m3, s3, sec1, sec2, sec3 As Integer
            Try
                h1 = CInt(Val(w.Substring(0, 2)))
                m1 = CInt(Val(w.Substring(3, 2)))
                s1 = CInt(Val(w.Substring(6, 2)))
                h2 = CInt(Val(w.Substring(9, 2)))
                m2 = CInt(Val(w.Substring(12, 2)))
                s2 = CInt(Val(w.Substring(15, 2)))
                h3 = CInt(Val(w.Substring(18, 2)))
                m3 = CInt(Val(w.Substring(21, 2)))
                s3 = CInt(Val(w.Substring(24, 2)))
            Catch
                Return False
            End Try


            sec1 = MyMath.Convert_Formatted_HMS_To_TimeSec(h1, m1, s1)
            sec2 = MyMath.Convert_Formatted_HMS_To_TimeSec(h2, m2, s2)
            sec3 = MyMath.Convert_Formatted_HMS_To_TimeSec(h3, m3, s3)

            If sec1 > 24 * 60 * 60 OrElse sec2 > 24 * 60 * 60 OrElse sec3 > 24 * 60 * 60 OrElse
                    sec1 < 0 OrElse sec2 < 0 OrElse sec3 < 0 Then
                Return False
            End If

            'Reset previous scedule if time is overlap
            If q > 0 Then
                For scanT As Integer = sec1 To sec2
                    ScheduledTimeArrayInSec(scanT) = False
                Next
            End If


            'Mark new time
            For scanT As Integer = sec1 To sec2 Step sec3
                ScheduledTimeArrayInSec(scanT) = True
            Next

        Next

        Return True
    End Function

    Private Sub Cmd_Stop_Click(sender As Object, e As EventArgs) Handles Cmd_Stop.Click
        Cmd_Start.Enabled = True
        Cmd_Stop.Enabled = False

        Timer_CheckTime.Enabled = False

        Group_Save.Enabled = True
    End Sub


    Public Sub DownloadNow(ImageURL As String, ImageURLNum As Integer, TagStr As String, TimeLimitSec As Integer)
        MyWebClientAsync.Download_Image(ImageURL, ImageURLNum, TagStr, TimeLimitSec)
    End Sub

    Private Sub Cmd_SaveNow_Click(sender As Object, e As EventArgs) Handles Cmd_SaveNow.Click
        DownloadNow()
    End Sub


    Public Sub DownloadNow()
        Cmd_SaveNow.Enabled = False

        Dim SequentialDelaySec As Integer
        Try
            SequentialDelaySec = CInt(Text_SequentialDelaySec.Text)
        Catch ex As Exception
            Text_SequentialDelaySec.Text = "1"
            SequentialDelaySec = CInt(Text_SequentialDelaySec.Text)
        End Try

        If Check_Illumination.Checked Then
            controlLight(True)
            IdleInSec(1)
        End If

        Dim ImageURLs() As String = Split(Text_ImageURL.Text.Trim, vbCrLf)

        For q As Integer = 0 To ImageURLs.GetUpperBound(0)
            Dim IPaddressStr As String =
                        ImageURLs(q).Replace("https://", "").Replace("http://", "")
            Dim IPaddressTheLastDigitStr As String
            Try
                IPaddressTheLastDigitStr = IPAddress.Parse(IPaddressStr).GetAddressBytes(3).ToString
            Catch ee As Exception
                MsgBox(ee.ToString, MsgBoxStyle.Critical, "Error")
                Exit Sub
            End Try

            Dim IPadressFull As String
            If Strings.Right(ImageURLs(q), 1) = "/" Then
                IPadressFull = ImageURLs(q).Trim + "capture"
            Else
                IPadressFull = ImageURLs(q).Trim + "/capture"
            End If


            If ImageURLs(q) <> "" Then
                Add_NewStatus("[" + Now.ToString + "] " + "Downloading requested to " + "cam" + IPaddressTheLastDigitStr)


                MyWebClientAsync.Download_Image(IPadressFull,
                                                q,
                                                "cam" + IPaddressTheLastDigitStr,
                                                CInt(Combo_AbortAfter.Text))
                IdleInSec(SequentialDelaySec)
            End If
        Next



        If Check_Illumination.Checked Then
            TurnOffLight()
            IdleInSec(2)
        End If

        Cmd_SaveNow.Enabled = True
    End Sub



    Sub Add_NewFail(NewFail As String)
        If List_DownloadFail.Items.Count > 10000 Then
            List_DownloadFail.Items.Clear()
        End If

        List_DownloadFail.Items.Add(NewFail)
        List_DownloadFail.SelectedIndex = List_DownloadFail.Items.Count - 1
        Label_DownloadFail.Text = "Download failure (total " +
                                    List_DownloadFail.Items.Count.ToString + ")"
    End Sub

    Sub Add_NewStatus(NewStatus As String)
        If List_Status.Items.Count > 10000 Then
            List_Status.Items.Clear()
        End If

        List_Status.Items.Add(NewStatus)
        List_Status.SelectedIndex = List_Status.Items.Count - 1
    End Sub


    Private Sub MyWebClientAsync_OneTaskDone(SourceURL As String, SourceURLNum As Integer, TagStr As String,
                                             ByVal pBitmapBytes As Byte(), TimeRequested As Date,
                                             TimeCompleted As Date, ErrorMsg As String) Handles MyWebClientAsync.OneTaskDone

        If ErrorMsg <> "" OrElse pBitmapBytes Is Nothing Then
            Add_NewStatus("[" + Now.ToString + "] " + "Download failed from " + TagStr)
            Add_NewFail("[" + Now.ToString + "] " + "Failed from " + TagStr)
        Else
            Pic_Received.Image = New Bitmap(New MemoryStream(pBitmapBytes))

            Add_NewStatus("[" + Now.ToString + "] " + "Download success from " + TagStr)


            If Check_Save.Checked Then
                SaveCurrentImageBytes(TagStr, SourceURLNum, pBitmapBytes, TimeRequested, TimeCompleted)
            End If

        End If
    End Sub


    Public Function Get_JpgFileNameOfTwoTimes(TimeRequested As Date, TimeCompleted As Date) As String
        Dim FileName As String = Format(TimeRequested.Year, "0000") & "-" &
                                  Format(TimeRequested.Month, "00") & "-" &
                                  Format(TimeRequested.Day, "00") & " (" &
                                  Format(TimeRequested.Hour, "00") & "-" &
                                  Format(TimeRequested.Minute, "00") & "-" &
                                  Format(TimeRequested.Second, "00") & "-" &
                                  Format(TimeRequested.Millisecond, "000") & ")-(" &
                                  Format(TimeCompleted.Hour, "00") & "-" &
                                  Format(TimeCompleted.Minute, "00") & "-" &
                                  Format(TimeCompleted.Second, "00") & "-" &
                                  Format(TimeCompleted.Millisecond, "000") & ")" &
                                  ".jpg"

        Return FileName
    End Function


    Public Sub SaveCurrentImage(SubFolderName As String, pBitmap As Bitmap,
                                TimeRequested As Date, TimeCompleted As Date)

        Dim TargetFullFolder As String =
                                Text_TargetFolder.Text & PathDelimit & SubFolderName

        If FolderExists(TargetFullFolder) = False Then
            MkDir(TargetFullFolder)
        End If

        Dim FileName As String = Get_JpgFileNameOfTwoTimes(TimeRequested, TimeCompleted)

        MyImageFileWriterAsync.Write_Image(TargetFullFolder, FileName, pBitmap)
    End Sub

    Public Sub SaveCurrentImageStream(SubFolderName As String, pBitmapStream As MemoryStream,
                                      TimeRequested As Date, TimeCompleted As Date)

        Dim TargetFullFolder As String =
                                Text_TargetFolder.Text & PathDelimit & SubFolderName

        If FolderExists(TargetFullFolder) = False Then
            MkDir(TargetFullFolder)
        End If


        Dim FileName As String = Get_JpgFileNameOfTwoTimes(TimeRequested, TimeCompleted)

        MyImageFileWriterAsync.Write_ImageStream(TargetFullFolder, FileName, pBitmapStream)
    End Sub

    Public Sub SaveCurrentImageBytes(SubFolderName As String, URLNum As Integer, pBitmapBytes As Byte(),
                                     TimeRequested As Date, TimeCompleted As Date)

        Dim TargetFullFolder As String =
                                Text_TargetFolder.Text & PathDelimit & SubFolderName

        If FolderExists(TargetFullFolder) = False Then
            MkDir(TargetFullFolder)
        End If

        Dim FileName As String = Get_JpgFileNameOfTwoTimes(TimeRequested, TimeCompleted)

        MyImageFileWriterAsync.Write_ImageBytes(TargetFullFolder, FileName, pBitmapBytes)
    End Sub

    Public Function FolderExists(ByVal FolderPath As String) As Boolean
        If FolderPath = "" Then Return False

        Dim f As New IO.DirectoryInfo(FolderPath)
        Return f.Exists

    End Function



    Public Function Draw_Text_Outlined_Image(ByVal SourceImage As Image,
                           ByVal x As Integer,
                           ByVal y As Integer,
                           ByVal TextString As String,
                           ByVal TextFont As Font,
                           ByVal InnerColor As Color,
                           ByVal OuterColor As Color) As Image
        Using bm As New Bitmap(SourceImage),
                 GraphBox As Graphics = Graphics.FromImage(bm)

            Call Draw_Text_Outlined_Graphics(
                    GraphBox, x, y, TextString, TextFont, InnerColor, OuterColor)

            Draw_Text_Outlined_Image = CType(bm.Clone, Bitmap)
        End Using
    End Function



    Public Sub Draw_Text_Outlined_Graphics(SourceGraphics As Graphics,
                               ByVal x As Integer,
                               ByVal y As Integer,
                               ByVal TextString As String,
                               ByVal TextFont As Font,
                               ByVal InnerColor As Color,
                               ByVal OuterColor As Color)

        With SourceGraphics
            Dim DrawingTextBrush As Brush = New SolidBrush(OuterColor)
            .DrawString(TextString, TextFont, DrawingTextBrush, x - 1, y - 1)
            .DrawString(TextString, TextFont, DrawingTextBrush, x - 1, y + 1)
            .DrawString(TextString, TextFont, DrawingTextBrush, x + 1, y + 1)
            .DrawString(TextString, TextFont, DrawingTextBrush, x + 1, y - 1)

            DrawingTextBrush = New SolidBrush(InnerColor)
            .DrawString(TextString, TextFont, DrawingTextBrush, x, y)

            DrawingTextBrush.Dispose()
            DrawingTextBrush = Nothing
        End With
    End Sub

    Private Sub Cmd_SetNewFolder_Click(sender As Object, e As EventArgs) Handles Cmd_SetNewFolder.Click
        If FolderDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Text_TargetFolder.Text = FolderDialog.SelectedPath
        End If
    End Sub

    Private Sub MyImageFileWriterAsync_OneTaskDone(PathName As String, FileName As String, TimeRequested As Date, ErrorMsg As String) Handles MyImageFileWriterAsync.OneTaskDone
        Dim MsgStr As String = "[" + Now.ToString + "] " + "Saved " +
                                FileName + " at /" +
                                MyFileSys.Get_TheLastFolder_From_FullPath(PathName)

        If ErrorMsg = "" Then
            MsgStr += "    [OK]"
        Else
            MsgStr += "    [failed: " + ErrorMsg + "]"
        End If

        Add_NewStatus(MsgStr)
    End Sub

    Private Sub Timer_CheckTime_Tick(sender As Object, e As EventArgs) Handles Timer_CheckTime.Tick
        Timer_CheckTime.Enabled = False
        If Check_CaptureTime() Then
            DownloadNow()
        End If
        Timer_CheckTime.Enabled = True
    End Sub



    Private Function Check_CaptureTime() As Boolean
        'If capture time, Return True
        'If not capture time, Return False

        Dim CurrentTimeDigit As Integer



        If IsContinuous = False Then
            Dim curTimeSpan As TimeSpan = Now.Subtract(StartTime)
            If curTimeSpan.TotalSeconds > DurationMin * 60 Then
                Cmd_Stop_Click(Nothing, Nothing)
                Return False
            End If
        End If


        If Radio_Sec.Checked Then
            CurrentTimeDigit = Now.Second
            If (CurrentTimeDigit Mod Val(Combo_Sec.Text) = 0) AndAlso
                    FinalTimeDigit <> CurrentTimeDigit Then
                FinalTimeDigit = CurrentTimeDigit
                Return True
            Else
                Return False
            End If
        End If


        If Radio_Min.Checked Then
            CurrentTimeDigit = Now.Minute
            If (CurrentTimeDigit Mod Val(Combo_Min.Text) = 0) AndAlso
                    Now.Second = 0 AndAlso
                    FinalTimeDigit <> CurrentTimeDigit Then
                FinalTimeDigit = CurrentTimeDigit
                Return True
            Else
                Return False
            End If
        End If


        If Radio_Hour.Checked Then
            CurrentTimeDigit = Now.Hour
            If (CurrentTimeDigit Mod Val(Combo_Hour.Text) = 0) AndAlso
                    Now.Minute = 0 AndAlso Now.Second = 0 AndAlso
                FinalTimeDigit <> CurrentTimeDigit Then
                FinalTimeDigit = CurrentTimeDigit
                Return True
            Else
                Return False
            End If
        End If


        If Radio_Schedule.Checked Then
            CurrentTimeDigit = MyMath.Convert_Formatted_HMS_To_TimeSec(Now.Hour, Now.Minute, Now.Second)
            If ScheduledTimeArrayInSec(CurrentTimeDigit) AndAlso
                    FinalTimeDigit <> CurrentTimeDigit Then
                FinalTimeDigit = CurrentTimeDigit
                Return True
            Else
                Return False
            End If
        End If

        Return False
    End Function

    Private Sub Frm_Main_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        LoadSetting()

        Call ProcessCommandLineArugments()
    End Sub

    Private Sub Frm_Main_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        SaveSetting()
    End Sub

    Private Sub Cmd_Copy_Click(sender As Object, e As EventArgs) Handles Cmd_Copy.Click
        If Pic_Received.Image Is Nothing Then
            MsgBox("The image has not been captured yet", MsgBoxStyle.Critical, "Copy")
            Exit Sub
        End If

        Try
            Clipboard.SetImage(Pic_Received.Image)
        Catch ee As Exception
            MsgBox(ee.Message, MsgBoxStyle.Critical, "Copy")
        End Try
    End Sub


    Private Sub Cmd_IlluminationOn_Click(sender As Object, e As EventArgs) Handles Cmd_IlluminationOn.Click
        Group_Illumination.Enabled = False
        controlLight(True)
        Group_Illumination.Enabled = True
    End Sub

    Public Sub controlLight(IsOn As Boolean)
        Dim ImageURLs() As String = Split(Text_TargetDeviceID.Text.Trim, vbCrLf)

        For q As Integer = 0 To ImageURLs.GetUpperBound(0)
            Dim IPaddressStr As String =
                    ImageURLs(q).Replace("https://", "").Replace("http://", "")

            Dim IPadressFull As String
            If Strings.Right(ImageURLs(q), 1) = "/" Then
                If IsOn Then
                    IPadressFull = ImageURLs(q).Trim + "on"
                Else
                    IPadressFull = ImageURLs(q).Trim + "off"
                End If
            Else
                If IsOn Then
                    IPadressFull = ImageURLs(q).Trim + "/on"
                Else
                    IPadressFull = ImageURLs(q).Trim + "/off"
                End If
            End If

            IdleExit = False
            WebB.Navigate(IPadressFull)
            IdleInSec(2)
            IdleExit = False
            WebB.Stop()
            IdleInMS(200)
        Next
    End Sub


    Public Sub TurnOffLight()
        controlLight(False)
    End Sub

    Private Sub Cmd_IlluminationOff_Click(sender As Object, e As EventArgs) Handles Cmd_IlluminationOff.Click
        Group_Illumination.Enabled = False
        TurnOffLight()
        Group_Illumination.Enabled = True
    End Sub

    Private Sub Check_Illumination_CheckedChanged(sender As Object, e As EventArgs) Handles Check_Illumination.CheckedChanged
        Group_Illumination.Enabled = Check_Illumination.Checked
    End Sub

    Private Sub WebB_Navigated(sender As Object, e As WebBrowserNavigatedEventArgs) Handles WebB.Navigated
        IdleExit = True
    End Sub

    Private Sub WebB_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebB.DocumentCompleted
        Add_NewStatus("[" + Now.ToString + "] " + WebB.DocumentText)
    End Sub

    Private Sub Cmd_ClearDownloadFail_Click(sender As Object, e As EventArgs) Handles Cmd_ClearDownloadFail.Click
        List_DownloadFail.Items.Clear()
        Label_DownloadFail.Text = "Download failure (total " +
                                    List_DownloadFail.Items.Count.ToString + ")"
    End Sub

    Private Sub Cmd_ClearStatus_Click(sender As Object, e As EventArgs) Handles Cmd_ClearStatus.Click
        List_Status.Items.Clear()
    End Sub

    Private Sub Cmd_About_Click(sender As Object, e As EventArgs) Handles Cmd_About.Click
        With Frm_About
            .Show()
            .BringToFront()
        End With
    End Sub


End Class
