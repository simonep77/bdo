Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

Public MustInherit Class ListaIscrittiPerPolizza
    Inherits DataList(Of ListaIscrittiPerPolizza, IscrittoPerPolizza)

    Public Function CercaIntestatariAttivi() As ListaIscrittiPerPolizza
        Me.Slot.DB.SQL = "SELECT IdIscrittiPolizza FROM iscrittiperpolizza where codgradoparentela=1 and codstatoiscritto=1 "

        Return Me.DoSearch()

    End Function
End Class
