Namespace Core.Controllers
    Public Class Server

        Public Property InfoUrl As String = String.Empty
        Public Property Rank As Integer = 0
        Public Property Name As String = String.Empty
        Public Property Players_Current As Integer = 0
        Public Property Players_Maximum As Integer = 0
        Public Property IP As String = String.Empty
        Public Property Port As String = String.Empty
        Public Property Map As String = String.Empty

        Private WebEngine As Core.WebKit.IE_Hook = Nothing

        Public Sub New(ByVal EngineInstance As Core.WebKit.IE_Hook)
            WebEngine = EngineInstance
        End Sub

        Public Async Function Get_Online_PLAYERS(ByVal InfoUrl_HTML As String) As Task(Of List(Of Controllers.Player))

            '   Dim HtmlCode As String = WebEngine.HTMLExternalDownload(InfoUrl)
            Try
                If String.IsNullOrEmpty(InfoUrl_HTML) Then
                    Throw New Exception("Empty HTML Code.")
                End If

                Dim ParsedDocument As HtmlDocument = Await WebEngine.GetHtmlDocument(InfoUrl_HTML)

                If ParsedDocument Is Nothing Then
                    Debug.WriteLine("Vacio !")
                    Throw New Exception("Error to Parsing.")
                End If

                Dim FindTable As HtmlElement = ParsedDocument.getElementsByTagAndClassName("table", "table_lst table_lst_stp").FirstOrDefault

                Return Await GetPlayersFromTable(FindTable)
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                Return Nothing
            End Try
        End Function

        Public Async Function Get_TenTOP_PLAYERS(ByVal InfoUrl_HTML As String) As Task(Of List(Of Controllers.Player))

            '   Dim HtmlCode As String = WebEngine.HTMLExternalDownload(InfoUrl)
            Try
                If String.IsNullOrEmpty(InfoUrl_HTML) Then
                    Throw New Exception("Empty HTML Code.")
                End If

                Dim ParsedDocument As HtmlDocument = Await WebEngine.GetHtmlDocument(InfoUrl_HTML)

                If ParsedDocument Is Nothing Then
                    Throw New Exception("Error to Parsing.")
                End If

                Dim Tables As List(Of HtmlElement) = ParsedDocument.getElementsByTagAndClassName("table", "table_lst table_lst_stp")

                If Tables.Count = 1 Then
                    Throw New Exception("This server's player stats are not tracked.")
                End If

                Dim FindTable As HtmlElement = Tables.LastOrDefault

                Return Await GetPlayersFromTable(FindTable)
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                Return Nothing
            End Try
        End Function

        Public Async Function GetMapPreview_URL(ByVal InfoUrl_HTML As String) As Task(Of String)

            '  Dim HtmlCode As String = WebEngine.HTMLExternalDownload(InfoUrl)

            If String.IsNullOrEmpty(InfoUrl_HTML) Then
                Throw New Exception("Empty HTML Code.")
            End If

            Dim ParsedDocument As HtmlDocument = Await WebEngine.GetHtmlDocument(InfoUrl_HTML)

            If ParsedDocument Is Nothing Then
                Throw New Exception("Error to Parsing.")
            End If

            Dim Img_Container As HtmlElement = ParsedDocument.GetElementById("HTML_map_ss_img")

            Dim Img_Url As String = Img_Container.Children(0).GetAttribute("src").Replace("about:", "https:")

            Return Img_Url
        End Function

        Private Async Function GetPlayersFromTable(ByVal Table As HtmlElement) As Task(Of List(Of Core.Controllers.Player))
            Dim PagingElement As New List(Of HtmlElement)

            For Each Element As HtmlElement In Table.All
                If Element.TagName.ToLower = "tr" Then
                    PagingElement.Add(Element)
                End If
            Next

            Dim TableHeaderInfo As HtmlElement = Nothing
            Dim TableInfo As New List(Of HtmlElement)

            If Not PagingElement.Count = 0 Then

                Dim MaxItems As Integer = PagingElement.Count - 1

                For i As Integer = 0 To MaxItems

                    Select Case i
                        Case 0 : TableHeaderInfo = PagingElement(i)
                        Case Else
                            TableInfo.Add(PagingElement(i))
                    End Select

                Next

            End If

            Dim Result As New List(Of Core.Controllers.Player)

            For Each ElementParent As HtmlElement In TableInfo
                Dim Player As New Core.Controllers.Player
                Player.Rank = Val(ElementParent.Children(0).InnerText)
                Player.Name = ElementParent.Children(1).InnerText
                Player.Score = ElementParent.Children(2).InnerText
                Player.Time_Played = ElementParent.Children(3).InnerText

                Result.Add(Player)
            Next

            Return Result
        End Function

    End Class
End Namespace

