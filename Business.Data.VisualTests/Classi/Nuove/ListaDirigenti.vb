Imports BDO.Objects

Public Class ListaDirigenti
    Inherits DataList(Of ListaDirigenti, Dirigente)

    Public Sub CercaTutti()
        Me.Slot.DB.SQL = "SELECT * FROM Dirigenti ORDER BY Cognome"
        Me.DoSearch()

    End Sub
End Class
