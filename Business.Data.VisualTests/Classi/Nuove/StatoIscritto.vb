Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

<Table("statiiscritto"), GlobalCache()> _
Public MustInherit Class StatoIscritto
    Inherits DataObject(Of StatoIscritto)

    <PrimaryKey()> _
    Public MustOverride ReadOnly Property IdStatoIscritto() As Int16

    Public MustOverride Property DescrizioneStatoIscritto() As String

End Class
