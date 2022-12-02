Imports System.Runtime.InteropServices
Imports Microsoft.Win32

Namespace Core.WebKit

    <ComVisible(False)>
    Public Class ExtendedWebBrowser
        Inherits WebBrowser

        Public Sub New()

            Me.ScriptErrorsSuppressed = True

            Try

                Dim key As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", True)
                Dim AppDomainName As String = AppDomain.CurrentDomain.FriendlyName

                If CObj(key.GetValue(AppDomainName)) Is Nothing Then
                    key.SetValue(AppDomainName, 11001, RegistryValueKind.DWord)
                End If

                Dim RegKeyWrite As RegistryKey = Registry.CurrentUser
                RegKeyWrite = RegKeyWrite.CreateSubKey("Software\Microsoft\Internet Explorer\Styles")
                RegKeyWrite.SetValue("MaxScriptStatements", 0) '1000000000
                RegKeyWrite.Close()

                Dim RegKeyWrit As RegistryKey = Registry.CurrentUser
                RegKeyWrit = RegKeyWrit.CreateSubKey("Software\Microsoft\Internet Explorer\Styles")
                RegKeyWrit.SetValue("Use My Stylesheet", 0)
                RegKeyWrit.Close()

            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try

        End Sub

        Public Sub WebBrowser_NewWindow(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.NewWindow
            e.Cancel = True
        End Sub

        Private Sub ExtendedWebBrowser_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles Me.DocumentCompleted

        End Sub

    End Class

End Namespace
