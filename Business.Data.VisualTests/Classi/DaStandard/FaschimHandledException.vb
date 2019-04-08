Namespace VecchieClassi
    ''' <summary>
    ''' Eccezione da utilizzare per interrompere un operazione che ha generato già dei messaggi altrove
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FaschimHandledException
        Inherits FaschimBaseException

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal messageFmt As String, ByVal ParamArray args() As Object)
            MyBase.New(messageFmt, args)
        End Sub
    End Class

End Namespace

