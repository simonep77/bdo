Namespace VecchieClassi

    ''' <summary>
    ''' Eccezione Base da cui ereditano tutte le eccezioni Faschim
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class FaschimBaseException
        Inherits ApplicationException

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal Message As String)
            MyBase.New(Message)
        End Sub

        Public Sub New(ByVal messageFmt As String, ByVal ParamArray args() As Object)
            MyBase.New(String.Format(messageFmt, args))
        End Sub
    End Class
End Namespace