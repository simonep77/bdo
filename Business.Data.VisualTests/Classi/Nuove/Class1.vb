Public Class Class1
    Private mId As Integer = 0
    Private mNome As String = String.Empty
    Private mCognome As String = String.Empty

    Public Property Id()
        Get
            Return Me.mId
        End Get
        Set(ByVal value)
            Me.mId = value
        End Set
    End Property

    Public Property Nome()
        Get
            Return Me.mNome
        End Get
        Set(ByVal value)
            Me.mNome = value
        End Set
    End Property

    Public Property Cognome()
        Get
            Return Me.mCognome
        End Get
        Set(ByVal value)
            Me.mCognome = value
        End Set
    End Property
End Class
