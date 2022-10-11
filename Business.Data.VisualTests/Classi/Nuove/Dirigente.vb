Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

<Table("Dirigenti")> _
Public MustInherit Class Dirigente
    Inherits DataObject(Of Dirigente)

    Public Const KEY_CODICEFISCALE As String = "CodiceFiscale"

    <PrimaryKey(), AutoIncrement()> _
    Public MustOverride ReadOnly Property IdDirigente() As Integer

    Public MustOverride Property Cognome() As String

    Public MustOverride Property Nome() As String

    Public MustOverride Property CodiceFiscale() As String

    <Column("DataNascita"), LoadOnAccess()> _
    Public MustOverride Property DataNascita() As Date

    <AcceptNull()> _
    Public MustOverride Property LetteraAttivazioneInviata() As Boolean

    <AutoInsertTimestamp()> _
    Public MustOverride ReadOnly Property DataInserimento() As Date

    <AutoUpdateTimestamp()> _
    Public MustOverride ReadOnly Property DataAggiornamento() As Date




End Class
