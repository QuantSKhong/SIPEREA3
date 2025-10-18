Imports System.Drawing.Imaging
Imports System.IO

Module JpegCompression


    'Do not use memory_stream.dispose   It prevents image.save
    Dim memory_stream As MemoryStream


    ' Get a codec's information.
    Private Function GetEncoderInfo(ByVal mimeType As String) As ImageCodecInfo
        Dim encoders As ImageCodecInfo()
        encoders = ImageCodecInfo.GetImageEncoders()
        For i As Integer = 0 To encoders.Length
            If encoders(i).MimeType = mimeType Then
                Return encoders(i)
            End If
        Next i
        Return Nothing
    End Function

    ' Save a JPEG in compressed form. 
    ' The compression_level value
    ' should be between 10 and 100.
    Public Sub SaveCompressedJpeg(ByVal image As Image, ByVal file_name As String,
                                  ByVal compression_level As Long)

        If compression_level < 10 Then
            Throw New ArgumentException("Compression level must be between 10 and 100")
        End If

        ' Get an encoder parameter array and set the compression level.
        Dim encoder_params As EncoderParameters = New EncoderParameters(1)
        encoder_params.Param(0) = New EncoderParameter(Encoder.Quality, compression_level)
        ' Prepare the codec to encode.
        Dim image_codec_info As ImageCodecInfo = GetEncoderInfo("image/jpeg")

        ' Save the file.
        image.Save(file_name, image_codec_info, encoder_params)
    End Sub


    Public Function SaveJpegIntoStream(ByVal image As Image,
                                       ByVal compression_level As Long) As MemoryStream

        If compression_level < 10 Then
            Throw New ArgumentException("Compression level must be between 10 and 100")
        End If

        ' Get an encoder parameter array and set the compression level.
        Dim encoder_params As EncoderParameters = New EncoderParameters(1)
        encoder_params.Param(0) = New EncoderParameter(Encoder.Quality, compression_level)

        ' Prepare the codec to encode.
        Dim image_codec_info As ImageCodecInfo = GetEncoderInfo("image/jpeg")

        ' Save the file.
        Dim memory_stream As New MemoryStream()
        image.Save(memory_stream, image_codec_info, encoder_params)

        Return memory_stream

    End Function



    Public Sub LoadJpegFromStream(ByRef DesinationImage As Image,
                                   ByVal RawData As Byte())
        Try
            memory_stream = New MemoryStream(RawData)
            DesinationImage = Image.FromStream(memory_stream)
        Catch
        End Try
    End Sub

End Module
