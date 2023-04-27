Namespace Core.Manage
    Public Class Scraper

        Public ReadOnly BaseURL As String = "https://www.gametracker.com/"

        Public ReadOnly InfoUrl As String = BaseURL & "servers/"

        Public Property Games As Dictionary(Of String, String)

        Public Property Locations As Dictionary(Of String, String)

        Public Property WebEngine As Core.WebKit.IE_Hook = Nothing

        Public Sub New()
            Games = New Dictionary(Of String, String)
            Locations = New Dictionary(Of String, String)
            WebEngine = New Core.WebKit.IE_Hook
        End Sub

        Public Async Function GetServerData(ByVal InfoUrl_html As String) As Task(Of Boolean)
            Try
                '  Dim HtmlCode As String = WebEngine.HTMLExternalDownload(InfoUrl)

                If String.IsNullOrEmpty(InfoUrl_html) Then
                    Throw New Exception("Empty HTML Code.")
                End If

                Dim ParsedDocument As HtmlDocument = Await WebEngine.GetHtmlDocument(InfoUrl_html)

                If ParsedDocument Is Nothing Then
                    Throw New Exception("Error to Parsing.")
                End If

                Dim GameSelect As HtmlElement = Nothing
                Dim LocationSelect As HtmlElement = Nothing

                For Each Element As HtmlElement In ParsedDocument.GetElementsByTagName("select")
                    Select Case Element.Name
                        Case "game"
                            If GameSelect Is Nothing Then GameSelect = Element
                        Case "loc"
                            If LocationSelect Is Nothing Then LocationSelect = Element
                        Case Else
                            If GameSelect IsNot Nothing And LocationSelect IsNot Nothing Then Exit For
                    End Select
                Next

                For Each Game As HtmlElement In GameSelect.Children
                    If Game.TagName.ToLower = "option" Then Games.Add(Game.GetAttribute("value"), Game.InnerText)
                Next

                For Each Location As HtmlElement In LocationSelect.Children
                    Select Case Location.TagName.ToLower
                        Case "option"
                            Locations.Add(Location.GetAttribute("value"), Location.InnerText)
                        Case "optgroup"
                            For Each LocationEx As HtmlElement In Location.Children
                                ' Debug.WriteLine(LocationEx.TagName & "   =   " & LocationEx.GetAttribute("value") & "    =    " & LocationEx.InnerText)
                                Locations.Add(LocationEx.GetAttribute("value"), LocationEx.InnerText)
                            Next
                    End Select
                Next

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function MakeSearch(ByVal GameName As String, Optional ByVal LocationID As String = "", Optional ByVal PageNumber As Integer = 1) As String
            Dim Result As String = BaseURL & "search/" & GameName & "/"
            If Not LocationID = "" Then Result += LocationID & "/"
            If Result.EndsWith("/") Then Result += "?"
            Result += "searchpge=" & PageNumber
            Return Result
        End Function

        'Public Async Function Deprecated_GetMaximunPages(ByVal GameUrl As String) As Task(Of Integer)
        '    Dim HtmlCode As String = WebEngine.HTMLExternalDownload(GameUrl) ' "https://www.gametracker.com/search/halo/?"

        '    If String.IsNullOrEmpty(HtmlCode) Then
        '        Throw New Exception("Empty HTML Code.")
        '    End If

        '    Dim ParsedDocument As HtmlDocument = Await WebEngine.GetHtmlDocument(HtmlCode)

        '    If ParsedDocument Is Nothing Then
        '        Throw New Exception("Error to Parsing.")
        '    End If

        '    Dim PagingElement As HtmlElement = ParsedDocument.getElementsByTagAndClassName("div", "paging").FirstOrDefault
        '    Dim MaxPages As Integer = 0
        '    Dim Controller As Boolean = False

        '    For Each Element As HtmlElement In PagingElement.Children
        '        If Controller = True Then
        '            If Element.InnerText.All(AddressOf Char.IsDigit) Then
        '                MaxPages = Element.InnerText
        '            End If
        '            If MaxPages = 0 Then MaxPages = 1
        '            Exit For
        '        End If
        '        If Element.TagName.ToLower = "span" Then
        '            If Element.InnerText = "..." Then
        '                Controller = True
        '            End If
        '        End If
        '    Next
        '    Return MaxPages
        'End Function

        'Public Async Function Deprecated_GetServers(ByVal GameUrl As String) As Task(Of List(Of Controllers.Server))

        '    Dim HtmlCode As String = WebEngine.HTMLExternalDownload(GameUrl)

        '    If String.IsNullOrEmpty(HtmlCode) Then
        '        Throw New Exception("Empty HTML Code.")
        '    End If

        '    Dim ParsedDocument As HtmlDocument = Await WebEngine.GetHtmlDocument(HtmlCode)

        '    If ParsedDocument Is Nothing Then
        '        Throw New Exception("Error to Parsing.")
        '    End If

        '    Dim FindTable As HtmlElement = ParsedDocument.getElementsByTagAndClassName("table", "table_lst table_lst_srs").FirstOrDefault

        '    Dim TableString As String = FindTable.Children(0).InnerText

        '    Dim PagingElement As New List(Of HtmlElement)

        '    For Each Element As HtmlElement In FindTable.All
        '        If Element.TagName.ToLower = "tr" Then
        '            PagingElement.Add(Element)
        '        End If
        '    Next

        '    Dim TableHeaderInfo As HtmlElement = Nothing
        '    Dim TableInfo As New List(Of HtmlElement)

        '    If Not PagingElement.Count = 0 Then

        '        Dim MaxItems As Integer = PagingElement.Count - 1

        '        For i As Integer = 0 To MaxItems

        '            Select Case i
        '                Case 0 : TableHeaderInfo = PagingElement(i)
        '                Case MaxItems
        '                Case Else
        '                    TableInfo.Add(PagingElement(i))
        '            End Select

        '        Next

        '    End If

        '    Dim Result As New List(Of Controllers.Server)

        '    For Each ElementParent As HtmlElement In TableInfo

        '        Dim Server As New Core.Controllers.Server(WebEngine)

        '        Server.Rank = Val(ElementParent.Children(0).InnerText)
        '        Server.Name = ElementParent.Children(2).InnerText

        '        Dim Players As String() = ElementParent.Children(3).InnerText.Split("/")

        '        Server.Players_Current = Players.FirstOrDefault
        '        Server.Players_Maximum = Players.LastOrDefault

        '        Dim IPandPort As String() = ElementParent.Children(6).InnerText.Split(":")

        '        Server.IP = IPandPort.FirstOrDefault
        '        Server.Port = IPandPort.LastOrDefault

        '        Server.Map = ElementParent.Children(7).InnerText

        '        Dim DetailsUrl As String = ElementParent.Children(2).Children(0).GetAttribute("href").Replace("about:/", "")

        '        Server.InfoUrl = BaseURL & DetailsUrl

        '        Result.Add(Server)

        '    Next

        '    Return Result
        'End Function

        Public Async Function GetMaximunPages(ByVal HtmlCode As String) As Task(Of Integer)
            'Dim HtmlCode As String = WebEngine.HTMLExternalDownload(GameUrl) ' "https://www.gametracker.com/search/halo/?"

            If String.IsNullOrEmpty(HtmlCode) Then
                Throw New Exception("Empty HTML Code.")
            End If

            Dim ParsedDocument As HtmlDocument = Await WebEngine.GetHtmlDocument(HtmlCode)

            If ParsedDocument Is Nothing Then
                Throw New Exception("Error to Parsing.")
            End If

            Dim PagingElement As HtmlElement = ParsedDocument.getElementsByTagAndClassName("div", "paging").FirstOrDefault
            Dim MaxPages As Integer = 0
            Dim Controller As Boolean = False

            For Each Element As HtmlElement In PagingElement.Children
                If Controller = True Then
                    If Element.InnerText.All(AddressOf Char.IsDigit) Then
                        MaxPages = Element.InnerText
                    End If
                    If MaxPages = 0 Then MaxPages = 1
                    Exit For
                End If
                If Element.TagName.ToLower = "span" Then
                    If Element.InnerText = "..." Then
                        Controller = True
                    End If
                End If
            Next
            Return MaxPages
        End Function

        Public Async Function GetServers(ByVal HtmlCode As String) As Task(Of List(Of Controllers.Server))

            'Dim HtmlCode As String = WebEngine.HTMLExternalDownload(GameUrl)

            If String.IsNullOrEmpty(HtmlCode) Then
                Throw New Exception("Empty HTML Code.")
            End If

            Dim ParsedDocument As HtmlDocument = Await WebEngine.GetHtmlDocument(HtmlCode)

            If ParsedDocument Is Nothing Then
                Throw New Exception("Error to Parsing.")
            End If

            Dim FindTable As HtmlElement = ParsedDocument.getElementsByTagAndClassName("table", "table_lst table_lst_srs").FirstOrDefault

            Dim TableString As String = FindTable.Children(0).InnerText

            Dim PagingElement As New List(Of HtmlElement)

            For Each Element As HtmlElement In FindTable.All
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
                        Case MaxItems
                        Case Else
                            TableInfo.Add(PagingElement(i))
                    End Select

                Next

            End If

            Dim Result As New List(Of Controllers.Server)

            For Each ElementParent As HtmlElement In TableInfo

                Dim Server As New Core.Controllers.Server(WebEngine)

                Server.Rank = Val(ElementParent.Children(0).InnerText)
                Server.Name = ElementParent.Children(2).InnerText

                Dim Players As String() = ElementParent.Children(3).InnerText.Split("/")

                Server.Players_Current = Players.FirstOrDefault
                Server.Players_Maximum = Players.LastOrDefault

                Dim IPandPort As String() = ElementParent.Children(6).InnerText.Split(":")

                Server.IP = IPandPort.FirstOrDefault
                Server.Port = IPandPort.LastOrDefault

                Server.Map = ElementParent.Children(7).InnerText

                Dim DetailsUrl As String = ElementParent.Children(2).Children(0).GetAttribute("href").Replace("about:/", "")

                Server.InfoUrl = BaseURL & DetailsUrl

                Result.Add(Server)

            Next

            Return Result
        End Function

    End Class
End Namespace

