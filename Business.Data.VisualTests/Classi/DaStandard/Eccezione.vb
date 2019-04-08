Namespace VecchieClassi

    Public Class Eccezione
        Private mMessaggio As String
        Private mCodice As Integer
        Private mAttributo As String
        Private mBloccante As Boolean

        Public Property Messaggio() As String
            Get
                Return Me.mMessaggio
            End Get
            Set(ByVal Value As String)
                Me.mMessaggio = Value
            End Set
        End Property

        Public Property Codice() As Integer
            Get
                Return Me.mCodice
            End Get
            Set(ByVal Value As Integer)
                Me.mCodice = Value
            End Set
        End Property

        Public Property Attributo() As String
            Get
                Return Me.mAttributo
            End Get
            Set(ByVal Value As String)
                Me.mAttributo = Value
            End Set
        End Property

        Public ReadOnly Property Bloccante() As Boolean
            Get
                Return Me.mBloccante
            End Get

        End Property

        Public Sub New()
            Me.mMessaggio = String.Empty
            Me.mCodice = 0
            Me.mAttributo = String.Empty
            Me.mBloccante = True
        End Sub

        Public Sub New(ByVal codiceIn As Integer, ByVal messaggioIn As String, ByVal attributoIn As String)
            Me.mCodice = codiceIn
            Me.mMessaggio = messaggioIn
            Me.mAttributo = attributoIn
            Me.mBloccante = True
        End Sub

        Public Sub New(ByVal codiceIn As Integer, ByVal messaggioIn As String, ByVal attributoIn As String, ByVal bloccante As Boolean)
            Me.mCodice = codiceIn
            Me.mMessaggio = messaggioIn
            Me.mAttributo = attributoIn
            Me.mBloccante = bloccante
        End Sub
    End Class
End Namespace