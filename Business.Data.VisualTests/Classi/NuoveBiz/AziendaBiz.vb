Imports Bdo.Objects

Public MustInherit Class AziendaBiz
    Inherits BusinessObject(Of Azienda)

    Public Prova As Decimal = 10.5D

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
