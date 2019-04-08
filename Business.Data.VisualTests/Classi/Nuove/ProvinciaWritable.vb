Imports BDO.Objects
Imports BDO.Attributes

<Table("Province")> _
Public MustInherit Class ProvinciaWritable
    Inherits DataObject(Of Provincia)

    Public Const KEY_SIGLA As String = "KEY_SIGLA"
    <PrimaryKey()>
    Public MustOverride Property Id() As Long

    <SearchKey(KEY_SIGLA)>
    Public MustOverride Property IdProvincia() As String

    Public MustOverride Property DescrizioneProvincia() As String

    Public MustOverride Property CodRegione() As Integer

End Class
