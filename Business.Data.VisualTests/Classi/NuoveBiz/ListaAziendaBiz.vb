Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

Public Class ListaAziendaBiz
    Inherits BusinessList(Of ListaAziende, Azienda, AziendaBiz)

    Public Sub New(lst As ListaAziende)
        MyBase.New(lst)
    End Sub

End Class
