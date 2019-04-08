Imports BDO.Objects
Imports BDO.Attributes

<Table("anagrafica")> _
Public MustInherit Class Anagrafica
    Inherits DataObject(Of Anagrafica)
    Public Const KEY_CF As String = "KEY_CF"
    <PrimaryKey()> _
    Public MustOverride ReadOnly Property IdAnagrafica() As String

    <Column("CodAnagrafica", GetType(String)), AcceptNull()> _
    Public MustOverride ReadOnly Property AnagraficaConiuge() As Anagrafica


    Public MustOverride Property CodStatoSalute() As Integer


    Public MustOverride Property codProfessione() As Integer


    Public MustOverride Property Cognome() As String
       

    Public MustOverride Property Nome() As String


    Public MustOverride Property sesso() As String
       

    <AcceptNull()> _
    Public MustOverride Property dataNascita() As Date


    <AcceptNull()> _
    Public MustOverride Property luogoNascita() As String

    <AcceptNull(), SearchKey(KEY_CF)>
    Public MustOverride Property codiceFiscale() As String


    Public MustOverride Property dataInserimento() As DateTime
       

    <AcceptNull()> _
    Public MustOverride Property dataDecesso() As DateTime
      

    <AcceptNull()> _
    Public MustOverride Property telefonocasa() As String


    <AcceptNull()> _
    Public MustOverride Property telefonoUfficio() As String


    <AcceptNull()> _
    Public MustOverride Property telefonomobile() As String
      

    <AcceptNull()> _
    Public MustOverride Property fax() As String


    <AcceptNull()> _
    Public MustOverride Property eMail() As String
    

    <AcceptNull()> _
    Public MustOverride Property indirizzo() As String


    <AcceptNull()> _
    Public MustOverride Property cap() As String


    <AcceptNull()> _
    Public MustOverride Property localita() As String


    <Column("Provincia", GetType(String)), AcceptNull()> _
    Public MustOverride Property Provincia() As Provincia


    <AcceptNull()> _
    Public MustOverride Property note() As String


    <AcceptNull()> _
    Public MustOverride Property ProvinciaNascita() As String


    <AcceptNull()> _
    Public MustOverride Property CodInnalzamentoLimite() As Integer


    Public Property ConsensoDati() As String


    <AcceptNull()> _
    Public MustOverride Property dataScadenzaPassword() As Date


    <AcceptNull()> _
    Public MustOverride Property primoAccesso() As String


    <AcceptNull()> _
    Public MustOverride Property EncPassword() As String


    <AcceptNull()> _
    Public MustOverride Property identificativoVP() As String


    <AcceptNull()> _
    Public MustOverride Property dataInizioRapporto() As Date


    <AcceptNull()> _
    Public MustOverride Property dataFineRapporto() As Date


End Class
