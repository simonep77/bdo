Imports BDO.Objects
Imports BDO.Attributes

<Table("abicab")>
Public MustInherit Class AbiCab
    Inherits DataObject(Of AbiCab)

    <PrimaryKey()>
    Public MustOverride Property Abi() As String
    <PrimaryKey()>
    Public MustOverride Property CabBancario() As String
    <AcceptNull()>
    Public MustOverride Property DenominazioneIstituto() As String
    <AcceptNull()>
    Public MustOverride Property DescrizioneAgenzia() As String
    <AcceptNull()>
    Public MustOverride Property IndirizzoSportello() As String
    <AcceptNull()>
    Public MustOverride Property LocalitaSportello() As String
    <AcceptNull()>
    Public MustOverride Property ComuneSportello() As String
    <AcceptNull()>
    Public MustOverride Property Cap() As String
    <AcceptNull()>
    Public MustOverride Property Provincia() As String

    <AutoInsertTimestamp()>
    Public MustOverride Property DataInserimento() As Date

    <AcceptNull(), MaxLength(1)>
    Public MustOverride Property Manuale() As String


End Class
