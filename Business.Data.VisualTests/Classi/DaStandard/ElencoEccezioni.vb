Namespace VecchieClassi

    Public Class ElencoEccezioni
        Inherits BaseElenco

        Public Sub AddEccezione(ByVal objEccezione As Eccezione)
            localElenco.Add(objEccezione)
        End Sub

        Public Sub AddEccezione(ByVal codiceIn As Integer, ByVal messaggioIn As String)
            localElenco.Add(New Eccezione(codiceIn, messaggioIn, ""))
        End Sub

        Public Sub AddEccezione(ByVal codiceIn As Integer, ByVal messaggioIn As String, ByVal attributoIn As String)
            localElenco.Add(New Eccezione(codiceIn, messaggioIn, attributoIn))
        End Sub

        Public Sub AddEccezione(ByVal codiceIn As Integer, ByVal messaggioIn As String, ByVal attributoIn As String, ByVal bloccante As Boolean)
            localElenco.Add(New Eccezione(codiceIn, messaggioIn, attributoIn, bloccante))
        End Sub


        Public Function getCount() As Integer
            Return localElenco.Count
        End Function

      

        Public Overrides Function ToString() As String

            Dim sb As New System.Text.StringBuilder()

            For Each ecc As Eccezione In Me.localElenco
                sb.AppendLine(ecc.Messaggio)
            Next

            Return sb.ToString()

        End Function



    End Class

End Namespace