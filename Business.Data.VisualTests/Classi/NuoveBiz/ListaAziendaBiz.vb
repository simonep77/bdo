Imports Bdo.Objects

Public Class ListaAziendaBiz
    Inherits BusinessList(Of ListaAziende, Azienda, AziendaBiz)

    Public Sub New(lst As ListaAziende)
        MyBase.New(lst)
    End Sub

End Class
