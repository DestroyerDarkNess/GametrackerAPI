Imports GametrackerAPI.Core.Manage
Imports GametrackerAPI.Core.Controllers


Public Class Form1

    Private ScraperEngine As Scraper = New Scraper
    Private Servers As New List(Of Server)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StartRuntimes()
    End Sub

    Private Async Sub StartRuntimes()

        Dim LoadData As Boolean = Await ScraperEngine.GetServerData

        If LoadData = True Then
            ComboBox1.Items.AddRange(ScraperEngine.Games.Values.ToArray)
            ComboBox2.Items.AddRange(ScraperEngine.Locations.Values.ToArray)
            If Not ComboBox1.Items.Count = 0 Then ComboBox1.SelectedIndex = 0
            If Not ComboBox2.Items.Count = 0 Then ComboBox2.SelectedIndex = 0
            If Not ComboBox3.Items.Count = 0 Then ComboBox3.SelectedIndex = 0
        Else
            MsgBox("No Internet Connection. :(")
            Environment.Exit(0)
        End If

    End Sub

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Servers.Clear()

            Dim UrlTarget As String = ScraperEngine.MakeSearch(ScraperEngine.Games.Keys(ComboBox1.SelectedIndex), ScraperEngine.Locations.Keys(ComboBox2.SelectedIndex), Val(ComboBox3.Items(ComboBox3.SelectedIndex)))

            ComboBox3.Items.Clear()
            Dim MaxPages As Integer = Await ScraperEngine.GetMaximunPages(UrlTarget)
            For i As Integer = 1 To MaxPages
                ComboBox3.Items.Add(i)
            Next

            Servers.AddRange(Await ScraperEngine.GetServers(UrlTarget))

            ListServers()
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
        Dim PlayerListTask As Task(Of List(Of Player)) = ServerEx.Get_Online_PLAYERS

        PlayerListTask.GetAwaiter.OnCompleted(Sub()
                                                  ListView2.Items.Clear()
                                                  For Each Py As Player In PlayerListTask.Result

                                                      Dim ListItem As New ListViewItem With {.Name = Py.Rank, .Text = Py.Name}
                                                      ListItem.SubItems.Add(New ListViewItem.ListViewSubItem With {.Text = Py.Score})
                                                      ListView2.Items.Add(ListItem)

                                                  Next
                                              End Sub)
    End Sub

    Private Sub CopyAdressToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyAdressToolStripMenuItem.Click
        Clipboard.SetText(ListView1.Items(ListView1.SelectedIndex).SubItems(3).Text.ToString)
    End Sub

End Class
