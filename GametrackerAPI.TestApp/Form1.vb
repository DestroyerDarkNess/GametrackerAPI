Imports EO.WebBrowser
Imports GametrackerAPI.Core.Controllers
Imports GametrackerAPI.Core.Manage

Public Class Form1

    Public WebEngineView As EO.WinForm.WebControl '= New EO.WinForm.WebControl With {.WebView = WebEngineHost, .Dock = DockStyle.Fill}
    Public WithEvents WebEngineHost As EO.WebBrowser.WebView '= New EO.WebBrowser.WebView With {.AllowDropLoad = False, .CustomUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36"}

    Private ScraperEngine As Scraper = New Scraper
    Private Servers As New List(Of Server)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EO.WebBrowser.Runtime.AddLicense("Kb114+30EO2s3OmxGeCm3MGz8M5nzunz7fGo7vf2HaF3s7P9FOKe5ff2EL112PD9GvZ3s+X1D5+t8PT26KF+xrLUE/Go5Omzy5+v3PYEFO6ntKbC461pmaTA6bto2PD9GvZ3s/MDD+SrwPL3Gp+d2Pj26KFpqbPC3a5rp7XIzZ+v3PYEFO6ntKbC46FotcAEFOan2PgGHeR36d7SGeWawbMKFOervtrI9eBysO3XErx2s7MEFOan2PgGHeR3s7P9FOKe5ff26XXj7fQQ7azcws0X6Jzc8gQQyJ21tMbbtnCttcbcs3Wm8PoO5Kfq6doP")
        EO.WebEngine.Engine.Default.Options.AllowProprietaryMediaFormats()

        WebEngineHost = New EO.WebBrowser.WebView With {.AllowDropLoad = False, .CustomUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36"}
        WebEngineView = New EO.WinForm.WebControl With {.WebView = WebEngineHost, .Dock = DockStyle.Fill}

        Panel1.Controls.Add(WebEngineView)
        WebEngineView.SendToBack()




        StartRuntimes()
    End Sub

    Private Async Sub StartRuntimes()
        ProgressBar1.Visible = True

        Dim Navigate As NavigationTask = WebEngineHost.LoadUrl(ScraperEngine.InfoUrl)

        Navigate.OnDone(Async Sub()

                            If Not Navigate.HttpStatusCode = 200 Then
                                MsgBox("Error: HttpStatusCode (" & Navigate.HttpStatusCode & ") - Error Code: " & Navigate.ErrorCode)
                                ProgressBar1.Visible = False
                                Exit Sub
                            End If

                            Dim HtmlPage As String = WebEngineHost.GetHtml

                            Dim LoadData As Boolean = Await ScraperEngine.GetServerData(HtmlPage)

                            If LoadData = True Then
                                ComboBox1.Items.AddRange(ScraperEngine.Games.Values.ToArray)
                                ComboBox2.Items.AddRange(ScraperEngine.Locations.Values.ToArray)
                                If Not ComboBox1.Items.Count = 0 Then ComboBox1.SelectedIndex = 0
                                If Not ComboBox2.Items.Count = 0 Then ComboBox2.SelectedIndex = 0
                                If Not ComboBox3.Items.Count = 0 Then ComboBox3.SelectedIndex = 0

                                Button1.Enabled = True

                            Else
                                MsgBox("No Internet Connection. :(")
                                Environment.Exit(0)
                            End If
                            ProgressBar1.Visible = False
                        End Sub)



    End Sub

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ProgressBar1.Visible = True
            Dim GameName As String = ScraperEngine.Games.Keys(ComboBox1.SelectedIndex)
            Dim LocationID As String = ScraperEngine.Locations.Keys(ComboBox2.SelectedIndex)
            If Not ComboBox3.Items.Count = 0 AndAlso ComboBox3.SelectedIndex = -1 Then ComboBox3.SelectedIndex = 0
            Dim PageInt As String = If(ComboBox3.Items.Count = 0, "1", Val(ComboBox3.Items(ComboBox3.SelectedIndex)))
            Dim UrlTarget As String = ScraperEngine.MakeSearch(GameName, LocationID, PageInt)

            Dim Navigate As NavigationTask = WebEngineHost.LoadUrl(UrlTarget)

            Navigate.OnDone(Async Sub()

                                If Not Navigate.HttpStatusCode = 200 Then
                                    MsgBox("Error: HttpStatusCode (" & Navigate.HttpStatusCode & ") - Error Code: " & Navigate.ErrorCode)
                                    ProgressBar1.Visible = False
                                    Exit Sub
                                End If

                                Dim HtmlPage As String = WebEngineHost.GetHtml

                                Servers.Clear()

                                ComboBox3.Items.Clear()

                                Dim MaxPages As Integer = Await ScraperEngine.GetMaximunPages(HtmlPage)
                                For i As Integer = 1 To MaxPages
                                    ComboBox3.Items.Add(i)
                                Next

                                Servers.AddRange(Await ScraperEngine.GetServers(HtmlPage))

                                ListServers()
                                ProgressBar1.Visible = False
                            End Sub)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ListServers()
        ListView1.Items.Clear()
        Dim ID As Integer = 0
        For Each Sv As Server In Servers

            Dim ListItem As New ListViewItem With {.Name = ID, .Text = Sv.Rank}
            ListItem.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Sv.Name})
            ListItem.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Sv.Players_Current & "/" & Sv.Players_Maximum, .BackColor = If(Sv.Players_Current = 0, Color.Red, Color.Lime)})
            ListItem.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Sv.IP & ":" & Sv.Port})
            ListItem.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Sv.Map})
            ListView1.Items.Add(ListItem)

            ID += 1
        Next
    End Sub

    Dim OldSelected As Server = Nothing

    Private Sub ListView1_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListView1.ItemSelectionChanged
        Dim ServerSelected As Server = Servers(e.ItemIndex)

        If OldSelected Is Nothing Or Not OldSelected Is ServerSelected Then
            ListPlayers(ServerSelected)
            OldSelected = ServerSelected
        End If

    End Sub

    Private Sub ListPlayers(ByVal ServerEx As Server)
        ProgressBar1.Visible = True
        Dim Navigate As NavigationTask = WebEngineHost.LoadUrl(ServerEx.InfoUrl)

        Navigate.OnDone(Async Sub()
                            If Not Navigate.HttpStatusCode = 200 Then
                                MsgBox("Error: HttpStatusCode (" & Navigate.HttpStatusCode & ") - Error Code: " & Navigate.ErrorCode)
                                ProgressBar1.Visible = False
                                Exit Sub
                            End If

                            Dim HtmlPage As String = WebEngineHost.GetHtml

                            Dim PlayerListTask As List(Of Player) = Await ServerEx.Get_Online_PLAYERS(HtmlPage)

                            ListView2.Items.Clear()
                            If PlayerListTask IsNot Nothing Then
                                Label4.Visible = False
                                For Each Py As Player In PlayerListTask

                                    Dim ListItem As New ListViewItem With {.Name = Py.Rank, .Text = Py.Name}
                                    ListItem.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Py.Score})
                                    ListView2.Items.Add(ListItem)

                                Next
                            Else
                                Label4.Visible = True
                            End If
                            ProgressBar1.Visible = False
                        End Sub)



    End Sub

    Private Sub CopyAdressToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyAdressToolStripMenuItem.Click
        Clipboard.SetText(ListView1.Items(ListView1.SelectedIndex).SubItems(3).Text.ToString)
    End Sub

    Private Sub WebView1_NewWindow(sender As Object, e As NewWindowEventArgs)
        e.Accepted = False
    End Sub

End Class
