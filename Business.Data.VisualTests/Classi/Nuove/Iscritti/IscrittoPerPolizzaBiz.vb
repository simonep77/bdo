Imports Bdo.Objects
Public Class IscrittoPerPolizzaBiz
    Inherits BusinessObject(Of IscrittoPerPolizza)

    Public Sub New(ByVal oIscritto As IscrittoPerPolizza)
        MyBase.New(oIscritto)
    End Sub


    ''' <summary>
    ''' Metodo pubblico di assegnazione Carenza
    ''' </summary>
    ''' <param name="mesiCarenzaIn"></param>
    ''' <remarks></remarks>
    Public Sub AssegnaCarenza(ByVal mesiCarenzaIn As Integer)
        If mesiCarenzaIn < 0 OrElse mesiCarenzaIn > 24 Then

        End If

        Me.DataObj.MesiCarenza = mesiCarenzaIn
        Me.Slot.SaveObject(Of IscrittoPerPolizza)(Me.DataObj)

    End Sub
End Class
