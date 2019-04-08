Imports BDO.Objects
Imports BDO.Attributes

<Table("polizze")> _
Public MustInherit Class Polizza
    Inherits DataObject(Of Polizza)

    <PrimaryKey(), AutoIncrement()> _
    Public MustOverride ReadOnly Property IdPolizza() As Integer

    Public MustOverride Property codCombinazioneCopertureAssicurative() As Integer

    <DefaultValue("10")> _
    Public MustOverride Property codDurataPolizza() As Integer

    Public MustOverride Property CodAzienda() As String

    Public MustOverride Property codConvenzioneCollettiva() As Integer

    Public MustOverride Property codModalitaPagamento() As Integer

    Public MustOverride Property codFrazionamentoPagamento() As Integer

    <DefaultValue("10/03/2014 13:03:45")> _
    Public MustOverride Property dataDecorrenza() As Date

    <AcceptNull()> _
    Public MustOverride Property dataScadenza() As Date

    <AcceptNull()> _
    Public MustOverride Property codPaese() As String

    <AcceptNull()> _
    Public MustOverride Property checkDigit() As String


    <AcceptNull()> _
    Public MustOverride Property CIN() As String


    <AcceptNull()> _
    Public MustOverride Property ABI() As String


    <AcceptNull()> _
    Public MustOverride Property CAB() As String


    <AcceptNull()> _
    Public MustOverride Property numeroCC() As String


    <AcceptNull()> _
    Public MustOverride Property dataModificaIBAN() As Date

    <AcceptNull()> _
    Public MustOverride Property indirizzoCorrispondenza() As String


    <AcceptNull()> _
    Public MustOverride Property capCorrispondenza() As String


    <AcceptNull()> _
    Public MustOverride Property localitaCorrispondenza() As String


    <AcceptNull()> _
    Public MustOverride Property ProvinciaCorrispondenza() As String


    Public MustOverride Property mesiCarenza() As Integer


    <AcceptNull()> _
    Public MustOverride Property note() As String


    <AcceptNull()> _
    Public MustOverride Property dataDaFondo() As Date


    <AcceptNull()> _
    Public MustOverride Property codPaeseRichiesta() As String


    <AcceptNull()> _
    Public MustOverride Property checkDigitRichiesta() As String


    <AcceptNull()> _
    Public MustOverride Property CINRichiesta() As String


    <AcceptNull()> _
    Public MustOverride Property ABIRichiesta() As String


    <AcceptNull()> _
    Public MustOverride Property CABRichiesta() As String


    <AcceptNull()> _
    Public MustOverride Property CCRichiesta() As String


    <AcceptNull()> _
    Public MustOverride Property codSito() As Integer



End Class
