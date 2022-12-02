Imports GametrackerAPI.Core.Manage
Imports GametrackerAPI.Core.Controllers


Public Class Form1

    Dim ScraperEngine As Scraper = New Scraper


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
        ListBox1.Items.Clear()

        Dim UrlTarget As String = ScraperEngine.MakeSearch(ScraperEngine.Games.Keys(ComboBox1.SelectedIndex), ScraperEngine.Locations.Keys(ComboBox2.SelectedIndex), Val(ComboBox3.Items(ComboBox3.SelectedIndex)))

        ComboBox3.Items.Clear()
        Dim MaxPages As Integer = Await ScraperEngine.GetMaximunPages(UrlTarget)
        For i As Integer = 1 To MaxPages
            ComboBox3.Items.Add(i)
        Next

        Dim Servers As List(Of Server) = Await ScraperEngine.GetServers(UrlTarget)
        For Each Sv As Server In Servers
            Dim InfoSTR As String = "Rank: " & Sv.Rank & "  Name: " & Sv.Name & "   Players: " & Sv.Players_Current & "/" & Sv.Players_Maximum & "   IP: " & Sv.IP & ":" & Sv.Port & "   Map: " & Sv.Map
            ListBox1.Items.Add(InfoSTR)
        Next
    End Sub

End Class
