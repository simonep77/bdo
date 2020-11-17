Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

<Table("aziende")>
Public MustInherit Class Azienda
    Inherits DataObject(Of Azienda)


    <PrimaryKey()> _
    Public MustOverride Property IdAzienda() As String
    <AcceptNull()> _
    Public MustOverride Property CodAzienda() As String
    <XmlFormatString("D5")> _
    Public MustOverride Property CodSettore() As Integer
    <UpperCase(), Trim(), MaxLength(15), MinLength(5), ValidateRegex("^S*$", "pippo non valido")> _
    Public MustOverride Property RagioneSociale() As String
    Public MustOverride Property PICF() As String
    Public MustOverride Property Indirizzo() As String
    Public MustOverride Property CAP() As String
    Public MustOverride Property Localita() As String
    Public MustOverride Property CodProvincia() As String
    <PropertyMap("CodProvincia")> _
    Public MustOverride ReadOnly Property Provincia() As Provincia
    <AcceptNull()> _
    Public MustOverride Property Telefono() As String
    Public MustOverride Property Fax() As String
    Public MustOverride Property Email() As String
    Public MustOverride Property Riferimento() As String
    Public MustOverride Property IndirizzoLeg() As String
    Public MustOverride Property CAPLeg() As String
    Public MustOverride Property LocalitaLeg() As String
    <Column("CodProvinciaLeg", GetType(String)), AcceptNull()> _
    Public MustOverride Property ProvinciaLeg() As Provincia
    <AcceptNull()> _
    Public MustOverride Property TelefonoLeg() As String
    <AcceptNull()> _
    Public MustOverride Property FaxLeg() As String
    Public MustOverride Property CodModalitaPagamento() As Integer
    <AcceptNull()> _
    Public MustOverride Property CodPaese() As String
    <AcceptNull()> _
    Public MustOverride Property CheckDigit() As String
    <AcceptNull()> _
    Public MustOverride Property CIN() As String
    <AcceptNull()> _
    Public MustOverride Property ABI() As String
    <AcceptNull()> _
    Public MustOverride Property CAB() As String
    <AcceptNull()> _
    Public MustOverride Property NumeroCC() As String
    <AcceptNull()> _
    Public MustOverride Property DifferenzaPremioAzienda() As Double
    <AutoUpdateTimestamp()> _
    Public MustOverride Property DataScadenzaPassword() As Date
    Public MustOverride Property ProgFileAnagrafica() As Integer
    Public MustOverride Property Stato() As Integer
    Public MustOverride Property EncPassWord() As String
    <SearchKey("VP")> _
    Public MustOverride Property IdentificativoVP() As String
    <XmlFormatString("dd/MM/yyyy")> _
    Public MustOverride Property DataInserimento() As DateTime
    Public MustOverride Property Disabilitata() As String
    Public MustOverride Property BloccoKit() As String
    ' xxxxxdxs

    <ListMap("CodAzienda")>
    Public MustOverride ReadOnly Property Polizze As ListaPolizza


End Class
