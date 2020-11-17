Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

<Table("MF_BACKOFFICE_Preview.dbo.AnagraficaConvenzioni")>
Public MustInherit Class SAR_BKO_AnagConvenzione
    Inherits DataObject(Of SAR_BKO_AnagConvenzione)


    <PrimaryKey()>
    Public MustOverride Property CodConvenzione() As String

    <PrimaryKey()>
    Public MustOverride Property DataInizioValidita As DateTime

    <PrimaryKey()>
    Public MustOverride Property DataFineValidita As DateTime

    <MaxLength(100)>
    Public MustOverride Property RagioneSociale As String


End Class
