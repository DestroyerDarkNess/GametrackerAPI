Imports System.IO
Imports System.Runtime.CompilerServices

Friend Module Extensions
    <Extension()>
    Friend Function getElementsByTagAndClassName(ByVal doc As HtmlDocument, ByVal Optional tag As String = "", ByVal Optional className As String = "") As List(Of HtmlElement)
        Dim lst As List(Of HtmlElement) = New List(Of HtmlElement)()
        Dim empty_tag As Boolean = String.IsNullOrEmpty(tag)
        Dim empty_cn As Boolean = String.IsNullOrEmpty(className)
        If empty_tag AndAlso empty_cn Then Return lst
        Dim elmts As HtmlElementCollection = If(empty_tag, doc.All, doc.GetElementsByTagName(tag))

        If empty_cn Then
            lst.AddRange(elmts.Cast(Of HtmlElement)())
            Return lst
        End If

        For i As Integer = 0 To elmts.Count - 1

            If elmts(i).GetAttribute("className") = className Then
                lst.Add(elmts(i))
            End If
        Next

        Return lst
    End Function

    <Extension()>
    Friend Function ToStream(ByVal str As String) As Stream
        Dim stream As MemoryStream = New MemoryStream()
        Dim writer As StreamWriter = New StreamWriter(stream)
        writer.Write(str)
        writer.Flush()
        stream.Position = 0
        Return stream
    End Function

    <Extension()>
    Friend Async Function DownloadImageAsync(ByVal UrlImg As String) As Task(Of Image)
        Try
            Dim WebpImageData() As Byte = New System.Net.WebClient().DownloadData(UrlImg)
            Dim Webpstream As IO.MemoryStream = New IO.MemoryStream(WebpImageData)
            Dim ToBitmap As Bitmap = New Bitmap(Webpstream)
            Return ToBitmap
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Module
