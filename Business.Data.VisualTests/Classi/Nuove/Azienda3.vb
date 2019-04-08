Imports Bdo.Objects
Imports Bdo.Attributes

<Table("Aziende")>
Public MustInherit Class Azienda3
    Inherits DataObject(Of Azienda3)

    <PrimaryKey()>
    Public MustOverride Property IdAzienda() As String
    Public MustOverride Property CodAzienda() As String
    <XmlFormatString("D5")>
    Public MustOverride Property CodSettore() As Integer
    <UpperCase(), Trim()>
    Public MustOverride Property RagioneSociale() As String
    Public MustOverride Property PICF() As String
    <Encrypted("KEYRSA", 40)>
    Public MustOverride Property Indirizzo() As String

    <Column("CodProvincia", GetType(String))>
    Public MustOverride Property Provincia() As Provincia2

    <AcceptNull()>
    Public MustOverride Property CodProvinciaLeg() As String

    <PropertyMap("CodProvinciaLeg")>
    Public MustOverride ReadOnly Property ProvinciaLeg() As Provincia



End Class
