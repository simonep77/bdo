
Namespace VecchieClassi

    ''' <summary>
    ''' Eccezione per gestire errori
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EFaschimError
        Inherits FaschimBaseException

        Public Sub New(ByVal messageFmt As String, ByVal ParamArray args() As Object)
            MyBase.New(messageFmt, args)
        End Sub
    End Class

End Namespace