
Imports System.IO
Imports System.Net

Namespace Core.WebKit
    Public Class IE_Hook
        Implements IDisposable

        Public WithEvents WebRender As ExtendedWebBrowser = New ExtendedWebBrowser
        Public Property ErrInfo As String = String.Empty

        Public Sub New()

        End Sub

        Public Async Function GetHtmlDocument(ByVal html As String) As Task(Of System.Windows.Forms.HtmlDocument)

            Dim HtmlEditor As WebBrowser = New WebBrowser()
            HtmlEditor.ScriptErrorsSuppressed = True
            HtmlEditor.DocumentText = html

            For i As Integer = 0 To 2
                If HtmlEditor.Document.Body IsNot Nothing Then Exit For
                Application.DoEvents()
                i -= 1
            Next

            Return HtmlEditor.Document
        End Function

        Public Async Function GetHTML(ByVal TargetUrl As String, Optional DelayMilliseconds As Integer = 10000, Optional ByVal UsertAgent As String = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)") As Task(Of String)
            Dim Result As String = String.Empty
            WebRender.Navigate(TargetUrl, Nothing, Nothing, "User-Agent: " & UsertAgent)

            AddHandler WebRender.DocumentCompleted, Sub()
                                                        Result = WebRender.Document.Body.InnerHtml
                                                    End Sub

            For i As Integer = 0 To DelayMilliseconds
                If Not Result = String.Empty Then
                    Exit For
                End If
                System.Threading.Thread.Sleep(1)
            Next

            Return Result
        End Function

        Public Function HTMLExternalDownload(ByVal TargetUrl As String, Optional ByVal UsertAgent As String = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)") As String
            Dim CodeHTML As String = String.Empty

            CodeHTML = BaseDownaloder(TargetUrl)

            If String.IsNullOrEmpty(CodeHTML) Then CodeHTML = HTMLDownloader(TargetUrl, UsertAgent)

            If CodeHTML = String.Empty Or Not ErrInfo = String.Empty Then
                Throw New Exception(ErrInfo)
            End If

            Return CodeHTML
        End Function

        ' For Windows 7 and olders.
        Private Function BaseDownaloder(ByVal TargetUrl As String)
            Try
                Dim Result As String = String.Empty
                Dim DownloadInfo As New WebClient
                Dim Info As String = DownloadInfo.DownloadString(TargetUrl)
                If Not Info = String.Empty Then Result = Info
                DownloadInfo.Dispose()
                Return Result
            Catch ex As Exception
                ' ErrInfo = ex.Message
                Return String.Empty
            End Try
        End Function

        'Dont Work on windows 7 for TLS Protocol v1.2
        Private Function HTMLDownloader(ByVal TargetUrl As String, Optional ByVal UsertAgent As String = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)")
            Try
                Dim Result As String = String.Empty
                Dim UrlHost As String = New Uri(TargetUrl).Host
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
                Dim cookieJar As CookieContainer = New CookieContainer()
                Dim request As HttpWebRequest = CType(WebRequest.Create(TargetUrl), HttpWebRequest)
                request.UseDefaultCredentials = True
                request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials
                request.CookieContainer = cookieJar
                request.Accept = "text/html, application/xhtml+xml, */*"
                request.Referer = "https://" + UrlHost + "/"
                request.Headers.Add("Accept-Language", "en-GB")
                request.UserAgent = UsertAgent
                request.Host = UrlHost

                Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)

                Using reader = New StreamReader(response.GetResponseStream())
                    Result = reader.ReadToEnd()
                End Using
                Return Result
            Catch ex As Exception
                ErrInfo = ex.Message
                Return String.Empty
            End Try
        End Function

#Region "IDisposable Support"

        Private disposedValue As Boolean = False ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    WebRender.Dispose()
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub

#End Region

    End Class
End Namespace

