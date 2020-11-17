Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

<Table("Aziende", "FASCHIMDM")> _
Public MustInherit Class Azienda2
    Inherits DataObject(Of Azienda2)

    <PrimaryKey()> _
    Public MustOverride Property IdAzienda() As String
    Public MustOverride Property CodAzienda() As String
    <XmlFormatString("D5")> _
    Public MustOverride Property CodSettore() As Integer
    <UpperCase(), Trim()> _
    Public MustOverride Property RagioneSociale() As String
    Public MustOverride Property PICF() As String
    Public MustOverride Property Indirizzo() As String

    <Column("CodProvincia", GetType(String))> _
    Public MustOverride Property Provincia() As Provincia2

    <AcceptNull()> _
    Public MustOverride Property CodProvinciaLeg() As String

    <PropertyMap("CodProvinciaLeg")> _
    Public MustOverride ReadOnly Property ProvinciaLeg() As Provincia



End Class
