Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

<Table("Province"), GlobalCache()> _
Public MustInherit Class Provincia
    Inherits DataObject(Of Provincia)

    Public Const KEY_REG As String = "KEY_REG"

    <PrimaryKey()> _
    Public MustOverride ReadOnly Property IdProvincia() As String

    <Column("DescrizioneProvincia")> _
    Public MustOverride Property Descrizione() As String

    <Column("CodRegione", GetType(Integer)), SearchKey(KEY_REG)>
    Public MustOverride Property Regione() As Regione

End Class
