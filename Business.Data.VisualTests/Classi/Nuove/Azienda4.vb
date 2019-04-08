Imports Bdo.Objects
Imports Bdo.Attributes

<Table("Aziende")> _
Public MustInherit Class Azienda4
    Inherits DataObject(Of Azienda4)

    <PrimaryKey()> _
    Public MustOverride Property IdAzienda() As String
    Public MustOverride Property DataScadenzaPassword() As Date



End Class
