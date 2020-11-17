Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

<Table("iscrittiperpolizza")> _
Public MustInherit Class IscrittoPerPolizza
    Inherits DataObject(Of IscrittoPerPolizza)

    <PrimaryKey(), AutoIncrement()> _
    Public MustOverride ReadOnly Property IdIscrittiPolizza() As Integer
    <Column("IdPolizza", GetType(Long))> _
    Public MustOverride Property Polizza() As Polizza
    <Column("IdAnagrafica", GetType(String))> _
    Public MustOverride Property Anagrafica() As Anagrafica
    <Column("CodStatoIscritto", GetType(Int16))> _
    Public MustOverride Property StatoIscritto() As StatoIscritto
    Public MustOverride Property CodGradoParentela() As Integer
    Public MustOverride Property DataIscrizione() As Date
    Public MustOverride Property DataCessazione() As Date
    Public MustOverride Property CodMotivoCessazione() As Integer
    Public MustOverride Property MesiCarenza() As Integer
    <Column("Confluente", GetType(String))> _
    Public MustOverride Property Confluente() As Int16
    <Column("FiscCarico", GetType(String))> _
    Public MustOverride Property FiscCarico() As Int16
    Public MustOverride Property DataInserimento() As Date

End Class
