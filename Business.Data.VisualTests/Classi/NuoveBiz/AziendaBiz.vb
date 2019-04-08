Imports Bdo.Objects

Public Class AziendaBiz
    Inherits BusinessObject(Of Azienda)

    Public Prova As Decimal = 10.5D

    Public Sub New(ByVal az As Azienda)
        MyBase.New(az)
    End Sub

End Class
