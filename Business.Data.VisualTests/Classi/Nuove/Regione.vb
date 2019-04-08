Imports BDO.Objects
Imports BDO.Attributes

<Table("regioni"), GlobalCache(), [ReadOnly]()> _
Public MustInherit Class Regione
    Inherits DataObject(Of Regione)

    <PrimaryKey(), IntRange(0, 1000)> _
    Public MustOverride Property IdRegione() As Integer

    <Column("DescrizioneRegione"), Trim(), UpperCase(), MaxLength(50)> _
    Public MustOverride Property Descrizione() As String

    <Trim(), UpperCase()> _
    Public MustOverride Property AreaGeografica() As String

End Class
