Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes

Public MustInherit Class AziendaBiz
    Inherits BusinessObject(Of Azienda)

    Public Prova As Decimal = 10.5D

    Public Pippo As New Lazy(Of Provincia)(Function() Me.Slot.LoadObjByPK(Of Provincia)(Me.DataObj.CodProvincia))

    Public Sub New(ByVal az As Azienda)
        MyBase.New(az)
    End Sub

End Class

Public Class AziendaBiz2
    Inherits AziendaBiz

    Public Prova As Decimal = 10.5D

    Public Sub New(ByVal az As Azienda)
        MyBase.New(az)
    End Sub

End Class

Public Class AziendaBiz3
    Inherits AziendaBiz

    Public Prova As Decimal = 10.5D

    Public Sub New(ByVal az As Azienda)
        MyBase.New(az)
    End Sub

End Class
