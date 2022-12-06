Imports System.Runtime.CompilerServices

Module Extension
    <Extension()>
    Friend Function SelectedIndex(ByVal listView As ListView) As Integer
        If listView.SelectedIndices.Count > 0 Then
            Return listView.SelectedIndices(0)
        Else
            Return 0
        End If
    End Function
End Module