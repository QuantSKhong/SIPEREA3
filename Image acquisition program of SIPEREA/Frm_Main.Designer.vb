<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Frm_Main
    Inherits System.Windows.Forms.Form

    'Form은 Dispose를 재정의하여 구성 요소 목록을 정리합니다.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows Form 디자이너에 필요합니다.
    Private components As System.ComponentModel.IContainer

    '참고: 다음 프로시저는 Windows Form 디자이너에 필요합니다.
    '수정하려면 Windows Form 디자이너를 사용하십시오.  
    '코드 편집기에서는 수정하지 마세요.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Frm_Main))
        Me.Text_ImageURL = New System.Windows.Forms.TextBox()
        Me.Combo_AbortAfter = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Cmd_Start = New System.Windows.Forms.Button()
        Me.Cmd_SaveNow = New System.Windows.Forms.Button()
        Me.Cmd_Stop = New System.Windows.Forms.Button()
        Me.Label_DurationMin = New System.Windows.Forms.Label()
        Me.Text_Schedule = New System.Windows.Forms.TextBox()
        Me.Text_DurationMin = New System.Windows.Forms.TextBox()
        Me.Radio_Schedule = New System.Windows.Forms.RadioButton()
        Me.Check_Continuous = New System.Windows.Forms.CheckBox()
        Me.Combo_Hour = New System.Windows.Forms.ComboBox()
        Me.Combo_Min = New System.Windows.Forms.ComboBox()
        Me.Combo_Sec = New System.Windows.Forms.ComboBox()
        Me.Radio_Hour = New System.Windows.Forms.RadioButton()
        Me.Radio_Min = New System.Windows.Forms.RadioButton()
        Me.Radio_Sec = New System.Windows.Forms.RadioButton()
        Me.Group_Save = New System.Windows.Forms.GroupBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Text_TargetFolder = New System.Windows.Forms.TextBox()
        Me.Cmd_SetNewFolder = New System.Windows.Forms.Button()
        Me.Check_Save = New System.Windows.Forms.CheckBox()
        Me.Cmd_Copy = New System.Windows.Forms.Button()
        Me.Pic_Received = New System.Windows.Forms.PictureBox()
        Me.List_Status = New System.Windows.Forms.ListBox()
        Me.Timer_CheckTime = New System.Windows.Forms.Timer(Me.components)
        Me.FolderDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.Timer_Refresh = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Text_SequentialDelaySec = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Group_Illumination = New System.Windows.Forms.GroupBox()
        Me.Cmd_IlluminationOff = New System.Windows.Forms.Button()
        Me.Cmd_IlluminationOn = New System.Windows.Forms.Button()
        Me.Text_TargetDeviceID = New System.Windows.Forms.TextBox()
        Me.Label_TimeNow = New System.Windows.Forms.Label()
        Me.Check_Illumination = New System.Windows.Forms.CheckBox()
        Me.WebB = New System.Windows.Forms.WebBrowser()
        Me.List_DownloadFail = New System.Windows.Forms.ListBox()
        Me.Label_Status = New System.Windows.Forms.Label()
        Me.Label_DownloadFail = New System.Windows.Forms.Label()
        Me.Cmd_ClearDownloadFail = New System.Windows.Forms.Button()
        Me.Cmd_ClearStatus = New System.Windows.Forms.Button()
        Me.Cmd_About = New System.Windows.Forms.Button()
        Me.Group_Save.SuspendLayout()
        CType(Me.Pic_Received, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.Group_Illumination.SuspendLayout()
        Me.SuspendLayout()
        '
        'Text_ImageURL
        '
        Me.Text_ImageURL.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Text_ImageURL.Location = New System.Drawing.Point(14, 23)
        Me.Text_ImageURL.Multiline = True
        Me.Text_ImageURL.Name = "Text_ImageURL"
        Me.Text_ImageURL.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.Text_ImageURL.Size = New System.Drawing.Size(190, 210)
        Me.Text_ImageURL.TabIndex = 0
        Me.Text_ImageURL.Text = "http://192.168.0.100" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "http://192.168.0.110" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "http://192.168.0.111"
        '
        'Combo_AbortAfter
        '
        Me.Combo_AbortAfter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Combo_AbortAfter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Combo_AbortAfter.FormattingEnabled = True
        Me.Combo_AbortAfter.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15"})
        Me.Combo_AbortAfter.Location = New System.Drawing.Point(352, 80)
        Me.Combo_AbortAfter.Name = "Combo_AbortAfter"
        Me.Combo_AbortAfter.Size = New System.Drawing.Size(64, 26)
        Me.Combo_AbortAfter.TabIndex = 41
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label7.Location = New System.Drawing.Point(227, 82)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(114, 18)
        Me.Label7.TabIndex = 40
        Me.Label7.Text = "Abort after (sec)"
        '
        'Cmd_Start
        '
        Me.Cmd_Start.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Cmd_Start.Image = CType(resources.GetObject("Cmd_Start.Image"), System.Drawing.Image)
        Me.Cmd_Start.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Cmd_Start.Location = New System.Drawing.Point(18, 24)
        Me.Cmd_Start.Name = "Cmd_Start"
        Me.Cmd_Start.Size = New System.Drawing.Size(110, 42)
        Me.Cmd_Start.TabIndex = 39
        Me.Cmd_Start.Text = "Start"
        Me.Cmd_Start.UseVisualStyleBackColor = True
        '
        'Cmd_SaveNow
        '
        Me.Cmd_SaveNow.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Cmd_SaveNow.Image = CType(resources.GetObject("Cmd_SaveNow.Image"), System.Drawing.Image)
        Me.Cmd_SaveNow.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Cmd_SaveNow.Location = New System.Drawing.Point(252, 24)
        Me.Cmd_SaveNow.Name = "Cmd_SaveNow"
        Me.Cmd_SaveNow.Size = New System.Drawing.Size(164, 42)
        Me.Cmd_SaveNow.TabIndex = 38
        Me.Cmd_SaveNow.Text = "   Download now"
        Me.Cmd_SaveNow.UseVisualStyleBackColor = True
        '
        'Cmd_Stop
        '
        Me.Cmd_Stop.Enabled = False
        Me.Cmd_Stop.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Cmd_Stop.Image = CType(resources.GetObject("Cmd_Stop.Image"), System.Drawing.Image)
        Me.Cmd_Stop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Cmd_Stop.Location = New System.Drawing.Point(134, 24)
        Me.Cmd_Stop.Name = "Cmd_Stop"
        Me.Cmd_Stop.Size = New System.Drawing.Size(112, 42)
        Me.Cmd_Stop.TabIndex = 37
        Me.Cmd_Stop.Text = "Stop"
        Me.Cmd_Stop.UseVisualStyleBackColor = True
        '
        'Label_DurationMin
        '
        Me.Label_DurationMin.AutoSize = True
        Me.Label_DurationMin.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label_DurationMin.Location = New System.Drawing.Point(351, 148)
        Me.Label_DurationMin.Name = "Label_DurationMin"
        Me.Label_DurationMin.Size = New System.Drawing.Size(32, 18)
        Me.Label_DurationMin.TabIndex = 30
        Me.Label_DurationMin.Text = "min"
        Me.Label_DurationMin.Visible = False
        '
        'Text_Schedule
        '
        Me.Text_Schedule.Enabled = False
        Me.Text_Schedule.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Text_Schedule.Location = New System.Drawing.Point(136, 191)
        Me.Text_Schedule.Multiline = True
        Me.Text_Schedule.Name = "Text_Schedule"
        Me.Text_Schedule.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal
        Me.Text_Schedule.Size = New System.Drawing.Size(280, 66)
        Me.Text_Schedule.TabIndex = 36
        Me.Text_Schedule.Text = "00:00:00-24:00:00(01:00:00)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "07:00:00-20:00:00(00:15:00)"
        '
        'Text_DurationMin
        '
        Me.Text_DurationMin.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Text_DurationMin.Location = New System.Drawing.Point(288, 145)
        Me.Text_DurationMin.Name = "Text_DurationMin"
        Me.Text_DurationMin.Size = New System.Drawing.Size(53, 24)
        Me.Text_DurationMin.TabIndex = 28
        Me.Text_DurationMin.Text = "20"
        Me.Text_DurationMin.Visible = False
        '
        'Radio_Schedule
        '
        Me.Radio_Schedule.AutoSize = True
        Me.Radio_Schedule.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Radio_Schedule.Location = New System.Drawing.Point(18, 192)
        Me.Radio_Schedule.Name = "Radio_Schedule"
        Me.Radio_Schedule.Size = New System.Drawing.Size(112, 22)
        Me.Radio_Schedule.TabIndex = 35
        Me.Radio_Schedule.Text = "User defined"
        Me.Radio_Schedule.UseVisualStyleBackColor = True
        '
        'Check_Continuous
        '
        Me.Check_Continuous.AutoSize = True
        Me.Check_Continuous.Checked = True
        Me.Check_Continuous.CheckState = System.Windows.Forms.CheckState.Checked
        Me.Check_Continuous.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Check_Continuous.Location = New System.Drawing.Point(230, 120)
        Me.Check_Continuous.Name = "Check_Continuous"
        Me.Check_Continuous.Size = New System.Drawing.Size(159, 22)
        Me.Check_Continuous.TabIndex = 26
        Me.Check_Continuous.Text = "Continuous capture"
        Me.Check_Continuous.UseVisualStyleBackColor = True
        '
        'Combo_Hour
        '
        Me.Combo_Hour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Combo_Hour.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Combo_Hour.FormattingEnabled = True
        Me.Combo_Hour.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"})
        Me.Combo_Hour.Location = New System.Drawing.Point(136, 152)
        Me.Combo_Hour.Name = "Combo_Hour"
        Me.Combo_Hour.Size = New System.Drawing.Size(65, 26)
        Me.Combo_Hour.TabIndex = 33
        '
        'Combo_Min
        '
        Me.Combo_Min.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Combo_Min.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Combo_Min.FormattingEnabled = True
        Me.Combo_Min.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30"})
        Me.Combo_Min.Location = New System.Drawing.Point(136, 116)
        Me.Combo_Min.Name = "Combo_Min"
        Me.Combo_Min.Size = New System.Drawing.Size(65, 26)
        Me.Combo_Min.TabIndex = 34
        '
        'Combo_Sec
        '
        Me.Combo_Sec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Combo_Sec.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Combo_Sec.FormattingEnabled = True
        Me.Combo_Sec.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30"})
        Me.Combo_Sec.Location = New System.Drawing.Point(136, 83)
        Me.Combo_Sec.Name = "Combo_Sec"
        Me.Combo_Sec.Size = New System.Drawing.Size(65, 26)
        Me.Combo_Sec.TabIndex = 32
        '
        'Radio_Hour
        '
        Me.Radio_Hour.AutoSize = True
        Me.Radio_Hour.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Radio_Hour.Location = New System.Drawing.Point(19, 152)
        Me.Radio_Hour.Name = "Radio_Hour"
        Me.Radio_Hour.Size = New System.Drawing.Size(100, 22)
        Me.Radio_Hour.TabIndex = 31
        Me.Radio_Hour.Text = "Every hour"
        Me.Radio_Hour.UseVisualStyleBackColor = True
        '
        'Radio_Min
        '
        Me.Radio_Min.AutoSize = True
        Me.Radio_Min.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Radio_Min.Location = New System.Drawing.Point(19, 116)
        Me.Radio_Min.Name = "Radio_Min"
        Me.Radio_Min.Size = New System.Drawing.Size(114, 22)
        Me.Radio_Min.TabIndex = 29
        Me.Radio_Min.Text = "Every minute"
        Me.Radio_Min.UseVisualStyleBackColor = True
        '
        'Radio_Sec
        '
        Me.Radio_Sec.AutoSize = True
        Me.Radio_Sec.Checked = True
        Me.Radio_Sec.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Radio_Sec.Location = New System.Drawing.Point(19, 82)
        Me.Radio_Sec.Name = "Radio_Sec"
        Me.Radio_Sec.Size = New System.Drawing.Size(119, 22)
        Me.Radio_Sec.TabIndex = 27
        Me.Radio_Sec.TabStop = True
        Me.Radio_Sec.Text = "Every second"
        Me.Radio_Sec.UseVisualStyleBackColor = True
        '
        'Group_Save
        '
        Me.Group_Save.Controls.Add(Me.Label5)
        Me.Group_Save.Controls.Add(Me.Text_TargetFolder)
        Me.Group_Save.Controls.Add(Me.Cmd_SetNewFolder)
        Me.Group_Save.Location = New System.Drawing.Point(10, 335)
        Me.Group_Save.Name = "Group_Save"
        Me.Group_Save.Size = New System.Drawing.Size(278, 128)
        Me.Group_Save.TabIndex = 31
        Me.Group_Save.TabStop = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(20, 33)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(123, 18)
        Me.Label5.TabIndex = 25
        Me.Label5.Text = "Destination folder"
        '
        'Text_TargetFolder
        '
        Me.Text_TargetFolder.Location = New System.Drawing.Point(22, 60)
        Me.Text_TargetFolder.Multiline = True
        Me.Text_TargetFolder.Name = "Text_TargetFolder"
        Me.Text_TargetFolder.ReadOnly = True
        Me.Text_TargetFolder.Size = New System.Drawing.Size(242, 53)
        Me.Text_TargetFolder.TabIndex = 2
        Me.Text_TargetFolder.Text = "D:\My Temp\Test"
        '
        'Cmd_SetNewFolder
        '
        Me.Cmd_SetNewFolder.Image = CType(resources.GetObject("Cmd_SetNewFolder.Image"), System.Drawing.Image)
        Me.Cmd_SetNewFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Cmd_SetNewFolder.Location = New System.Drawing.Point(192, 24)
        Me.Cmd_SetNewFolder.Name = "Cmd_SetNewFolder"
        Me.Cmd_SetNewFolder.Size = New System.Drawing.Size(74, 27)
        Me.Cmd_SetNewFolder.TabIndex = 1
        Me.Cmd_SetNewFolder.Text = "    Set"
        Me.Cmd_SetNewFolder.UseVisualStyleBackColor = True
        '
        'Check_Save
        '
        Me.Check_Save.AutoSize = True
        Me.Check_Save.Checked = True
        Me.Check_Save.CheckState = System.Windows.Forms.CheckState.Checked
        Me.Check_Save.Location = New System.Drawing.Point(10, 308)
        Me.Check_Save.Name = "Check_Save"
        Me.Check_Save.Size = New System.Drawing.Size(165, 22)
        Me.Check_Save.TabIndex = 30
        Me.Check_Save.Text = "Save image to folder"
        Me.Check_Save.UseVisualStyleBackColor = True
        '
        'Cmd_Copy
        '
        Me.Cmd_Copy.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Cmd_Copy.Location = New System.Drawing.Point(911, 370)
        Me.Cmd_Copy.Name = "Cmd_Copy"
        Me.Cmd_Copy.Size = New System.Drawing.Size(75, 30)
        Me.Cmd_Copy.TabIndex = 9
        Me.Cmd_Copy.Text = "Copy"
        Me.Cmd_Copy.UseVisualStyleBackColor = True
        '
        'Pic_Received
        '
        Me.Pic_Received.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Pic_Received.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Pic_Received.Location = New System.Drawing.Point(11, 23)
        Me.Pic_Received.Name = "Pic_Received"
        Me.Pic_Received.Size = New System.Drawing.Size(357, 324)
        Me.Pic_Received.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.Pic_Received.TabIndex = 0
        Me.Pic_Received.TabStop = False
        '
        'List_Status
        '
        Me.List_Status.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.List_Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.List_Status.FormattingEnabled = True
        Me.List_Status.ItemHeight = 18
        Me.List_Status.Location = New System.Drawing.Point(12, 514)
        Me.List_Status.Name = "List_Status"
        Me.List_Status.Size = New System.Drawing.Size(668, 148)
        Me.List_Status.TabIndex = 0
        '
        'Timer_CheckTime
        '
        Me.Timer_CheckTime.Interval = 333
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Cmd_Start)
        Me.GroupBox1.Controls.Add(Me.Check_Continuous)
        Me.GroupBox1.Controls.Add(Me.Combo_Hour)
        Me.GroupBox1.Controls.Add(Me.Radio_Schedule)
        Me.GroupBox1.Controls.Add(Me.Combo_Min)
        Me.GroupBox1.Controls.Add(Me.Text_DurationMin)
        Me.GroupBox1.Controls.Add(Me.Combo_Sec)
        Me.GroupBox1.Controls.Add(Me.Text_Schedule)
        Me.GroupBox1.Controls.Add(Me.Radio_Hour)
        Me.GroupBox1.Controls.Add(Me.Label_DurationMin)
        Me.GroupBox1.Controls.Add(Me.Combo_AbortAfter)
        Me.GroupBox1.Controls.Add(Me.Radio_Min)
        Me.GroupBox1.Controls.Add(Me.Cmd_Stop)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Radio_Sec)
        Me.GroupBox1.Controls.Add(Me.Cmd_SaveNow)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold)
        Me.GroupBox1.Location = New System.Drawing.Point(242, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(438, 277)
        Me.GroupBox1.TabIndex = 45
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Image capture"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Text_SequentialDelaySec)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.Text_ImageURL)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(219, 274)
        Me.GroupBox2.TabIndex = 47
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Source cameras"
        '
        'Text_SequentialDelaySec
        '
        Me.Text_SequentialDelaySec.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Text_SequentialDelaySec.Location = New System.Drawing.Point(161, 239)
        Me.Text_SequentialDelaySec.Name = "Text_SequentialDelaySec"
        Me.Text_SequentialDelaySec.Size = New System.Drawing.Size(43, 24)
        Me.Text_SequentialDelaySec.TabIndex = 49
        Me.Text_SequentialDelaySec.Text = "1"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(19, 242)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(136, 18)
        Me.Label1.TabIndex = 49
        Me.Label1.Text = "Sequential delay (s)"
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.Controls.Add(Me.Pic_Received)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(696, 8)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(376, 357)
        Me.GroupBox3.TabIndex = 48
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Received image"
        '
        'Group_Illumination
        '
        Me.Group_Illumination.Controls.Add(Me.Cmd_IlluminationOff)
        Me.Group_Illumination.Controls.Add(Me.Cmd_IlluminationOn)
        Me.Group_Illumination.Controls.Add(Me.Text_TargetDeviceID)
        Me.Group_Illumination.Controls.Add(Me.Label_TimeNow)
        Me.Group_Illumination.Location = New System.Drawing.Point(304, 336)
        Me.Group_Illumination.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Group_Illumination.Name = "Group_Illumination"
        Me.Group_Illumination.Padding = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Group_Illumination.Size = New System.Drawing.Size(376, 128)
        Me.Group_Illumination.TabIndex = 49
        Me.Group_Illumination.TabStop = False
        '
        'Cmd_IlluminationOff
        '
        Me.Cmd_IlluminationOff.Location = New System.Drawing.Point(256, 73)
        Me.Cmd_IlluminationOff.Name = "Cmd_IlluminationOff"
        Me.Cmd_IlluminationOff.Size = New System.Drawing.Size(96, 30)
        Me.Cmd_IlluminationOff.TabIndex = 29
        Me.Cmd_IlluminationOff.Text = "Off"
        Me.Cmd_IlluminationOff.UseVisualStyleBackColor = True
        '
        'Cmd_IlluminationOn
        '
        Me.Cmd_IlluminationOn.Location = New System.Drawing.Point(256, 32)
        Me.Cmd_IlluminationOn.Name = "Cmd_IlluminationOn"
        Me.Cmd_IlluminationOn.Size = New System.Drawing.Size(96, 30)
        Me.Cmd_IlluminationOn.TabIndex = 28
        Me.Cmd_IlluminationOn.Text = "On"
        Me.Cmd_IlluminationOn.UseVisualStyleBackColor = True
        '
        'Text_TargetDeviceID
        '
        Me.Text_TargetDeviceID.Location = New System.Drawing.Point(16, 24)
        Me.Text_TargetDeviceID.Multiline = True
        Me.Text_TargetDeviceID.Name = "Text_TargetDeviceID"
        Me.Text_TargetDeviceID.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.Text_TargetDeviceID.Size = New System.Drawing.Size(190, 89)
        Me.Text_TargetDeviceID.TabIndex = 25
        Me.Text_TargetDeviceID.Text = "http://192.168.0.104"
        '
        'Label_TimeNow
        '
        Me.Label_TimeNow.AutoSize = True
        Me.Label_TimeNow.Location = New System.Drawing.Point(759, 35)
        Me.Label_TimeNow.Name = "Label_TimeNow"
        Me.Label_TimeNow.Size = New System.Drawing.Size(24, 18)
        Me.Label_TimeNow.TabIndex = 24
        Me.Label_TimeNow.Text = "    "
        '
        'Check_Illumination
        '
        Me.Check_Illumination.AutoSize = True
        Me.Check_Illumination.Checked = True
        Me.Check_Illumination.CheckState = System.Windows.Forms.CheckState.Checked
        Me.Check_Illumination.Location = New System.Drawing.Point(304, 308)
        Me.Check_Illumination.Name = "Check_Illumination"
        Me.Check_Illumination.Size = New System.Drawing.Size(103, 22)
        Me.Check_Illumination.TabIndex = 50
        Me.Check_Illumination.Text = "Illumination"
        Me.Check_Illumination.UseVisualStyleBackColor = True
        '
        'WebB
        '
        Me.WebB.Location = New System.Drawing.Point(136, 272)
        Me.WebB.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebB.Name = "WebB"
        Me.WebB.Size = New System.Drawing.Size(174, 60)
        Me.WebB.TabIndex = 51
        Me.WebB.Visible = False
        '
        'List_DownloadFail
        '
        Me.List_DownloadFail.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.List_DownloadFail.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.List_DownloadFail.FormattingEnabled = True
        Me.List_DownloadFail.ItemHeight = 18
        Me.List_DownloadFail.Location = New System.Drawing.Point(697, 405)
        Me.List_DownloadFail.Name = "List_DownloadFail"
        Me.List_DownloadFail.Size = New System.Drawing.Size(375, 256)
        Me.List_DownloadFail.TabIndex = 52
        '
        'Label_Status
        '
        Me.Label_Status.AutoSize = True
        Me.Label_Status.Location = New System.Drawing.Point(12, 493)
        Me.Label_Status.Name = "Label_Status"
        Me.Label_Status.Size = New System.Drawing.Size(50, 18)
        Me.Label_Status.TabIndex = 53
        Me.Label_Status.Text = "Status"
        '
        'Label_DownloadFail
        '
        Me.Label_DownloadFail.AutoSize = True
        Me.Label_DownloadFail.Location = New System.Drawing.Point(694, 384)
        Me.Label_DownloadFail.Name = "Label_DownloadFail"
        Me.Label_DownloadFail.Size = New System.Drawing.Size(167, 18)
        Me.Label_DownloadFail.TabIndex = 54
        Me.Label_DownloadFail.Text = "Download failed (total 0)"
        '
        'Cmd_ClearDownloadFail
        '
        Me.Cmd_ClearDownloadFail.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Cmd_ClearDownloadFail.Location = New System.Drawing.Point(992, 370)
        Me.Cmd_ClearDownloadFail.Name = "Cmd_ClearDownloadFail"
        Me.Cmd_ClearDownloadFail.Size = New System.Drawing.Size(75, 30)
        Me.Cmd_ClearDownloadFail.TabIndex = 55
        Me.Cmd_ClearDownloadFail.Text = "Clear"
        Me.Cmd_ClearDownloadFail.UseVisualStyleBackColor = True
        '
        'Cmd_ClearStatus
        '
        Me.Cmd_ClearStatus.Location = New System.Drawing.Point(584, 480)
        Me.Cmd_ClearStatus.Name = "Cmd_ClearStatus"
        Me.Cmd_ClearStatus.Size = New System.Drawing.Size(98, 30)
        Me.Cmd_ClearStatus.TabIndex = 56
        Me.Cmd_ClearStatus.Text = "Clear"
        Me.Cmd_ClearStatus.UseVisualStyleBackColor = True
        '
        'Cmd_About
        '
        Me.Cmd_About.Location = New System.Drawing.Point(560, 304)
        Me.Cmd_About.Name = "Cmd_About"
        Me.Cmd_About.Size = New System.Drawing.Size(98, 30)
        Me.Cmd_About.TabIndex = 57
        Me.Cmd_About.Text = "About"
        Me.Cmd_About.UseVisualStyleBackColor = True
        '
        'Frm_Main
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(1083, 674)
        Me.Controls.Add(Me.Cmd_About)
        Me.Controls.Add(Me.Cmd_Copy)
        Me.Controls.Add(Me.Cmd_ClearStatus)
        Me.Controls.Add(Me.Cmd_ClearDownloadFail)
        Me.Controls.Add(Me.Label_DownloadFail)
        Me.Controls.Add(Me.Label_Status)
        Me.Controls.Add(Me.List_DownloadFail)
        Me.Controls.Add(Me.WebB)
        Me.Controls.Add(Me.Check_Illumination)
        Me.Controls.Add(Me.Group_Illumination)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.List_Status)
        Me.Controls.Add(Me.Group_Save)
        Me.Controls.Add(Me.Check_Save)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Frm_Main"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Image Acquisition Program of SIPEREA 1.0"
        Me.Group_Save.ResumeLayout(False)
        Me.Group_Save.PerformLayout()
        CType(Me.Pic_Received, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.Group_Illumination.ResumeLayout(False)
        Me.Group_Illumination.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Text_ImageURL As TextBox
    Friend WithEvents Group_Save As GroupBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Text_TargetFolder As TextBox
    Friend WithEvents Cmd_SetNewFolder As Button
    Friend WithEvents Check_Save As CheckBox
    Friend WithEvents Label_DurationMin As Label
    Friend WithEvents Text_Schedule As TextBox
    Friend WithEvents Text_DurationMin As TextBox
    Friend WithEvents Radio_Schedule As RadioButton
    Friend WithEvents Check_Continuous As CheckBox
    Friend WithEvents Combo_Hour As ComboBox
    Friend WithEvents Combo_Min As ComboBox
    Friend WithEvents Combo_Sec As ComboBox
    Friend WithEvents Radio_Hour As RadioButton
    Friend WithEvents Radio_Min As RadioButton
    Friend WithEvents Radio_Sec As RadioButton
    Friend WithEvents Timer_CheckTime As Timer
    Friend WithEvents FolderDialog As FolderBrowserDialog
    Friend WithEvents Cmd_Start As Button
    Friend WithEvents Cmd_SaveNow As Button
    Friend WithEvents Cmd_Stop As Button
    Friend WithEvents Combo_AbortAfter As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents List_Status As ListBox
    Friend WithEvents Cmd_Copy As Button
    Friend WithEvents Pic_Received As PictureBox
    Friend WithEvents Timer_Refresh As Timer
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Text_SequentialDelaySec As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Group_Illumination As GroupBox
    Friend WithEvents Label_TimeNow As Label
    Friend WithEvents Text_TargetDeviceID As TextBox
    Friend WithEvents Cmd_IlluminationOn As Button
    Friend WithEvents Cmd_IlluminationOff As Button
    Friend WithEvents Check_Illumination As CheckBox
    Friend WithEvents WebB As WebBrowser
    Friend WithEvents List_DownloadFail As ListBox
    Friend WithEvents Label_Status As Label
    Friend WithEvents Label_DownloadFail As Label
    Friend WithEvents Cmd_ClearDownloadFail As Button
    Friend WithEvents Cmd_ClearStatus As Button
    Friend WithEvents Cmd_About As Button
End Class
