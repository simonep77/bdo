Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

Public MustInherit Class ListaAziende
    Inherits DataList(Of ListaAziende, Azienda)

    Public Function CercaTutti() As ListaAziende
        Me.Slot.DB.SQL = "SELECT IdAzienda FROM Aziende " & _
                       " WHERE Disabilitata <> '1'"

        Return Me.DoSearch()

    End Function
End Class
