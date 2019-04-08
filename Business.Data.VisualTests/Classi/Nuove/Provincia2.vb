Imports BDO.Objects
Imports BDO.Attributes

<Table("Province"), GlobalCache()>
Public MustInherit Class Provincia2
    Inherits DataObject(Of Provincia2)

    <PrimaryKey()> _
    Public MustOverride ReadOnly Property IdProvincia() As String

    <Column("DescrizioneProvincia")> _
    Public MustOverride Property Descrizione() As String

    <Column("CodRegione", GetType(Integer))> _
    Public MustOverride Property Regione() As Regione

End Class
