Namespace VecchieClassi
    Public Class Paginazione
        Inherits BaseClass

#Region "Attributi"

        Private mPagina As Integer = 0
        Private mTotRecord As Long = 0
        Private mOffset As Integer = 20
        Private mTotPagine As Integer = 0
        Private mXML As String = String.Empty

        Public Property TotRecord() As Long
            Get
                Return mTotRecord
            End Get
            Set(ByVal Value As Long)
                mTotRecord = Value
            End Set
        End Property

        Public ReadOnly Property TotPagine() As Integer
            Get
                Return Convert.ToInt32(Math.Ceiling(Me.mTotRecord / Me.mOffset))
                'mTotPagine = Int32.Parse(mTotRecord) \ Int32.Parse(mOffset) + 1
                'If Fix(mTotRecord / mOffset) = mTotRecord / mOffset Then mTotPagine = mTotPagine - 1
                'Return mTotPagine
            End Get
        End Property

        Public Property Pagina() As Integer
            Get
                Return mPagina
            End Get
            Set(ByVal Value As Integer)
                If Value = 0 Then
                    mPagina = 1
                Else
                    mPagina = Value
                End If
            End Set
        End Property

        Public Property Offset() As Integer
            Get
                Return mOffset
            End Get
            Set(ByVal Value As Integer)
                mOffset = Value
            End Set
        End Property

        Public ReadOnly Property Posizione() As Integer
            Get
                Return (mPagina - 1) * mOffset
            End Get
        End Property

        Public ReadOnly Property XMLPaginazione() As String
            Get
                'mXML = "<PAGINAZIONE>"
                'mXML = mXML & "<TOT_RECORD>" & TotRecord & "</TOT_RECORD>"
                'mXML = mXML & "<TOT_PAGINE>" & TotPagine & "</TOT_PAGINE>"
                'mXML = mXML & "<PAGINA>" & Pagina & "</PAGINA>"
                'mXML = mXML & "<OFFSET>" & Offset & "</OFFSET>"
                'mXML = mXML & "</PAGINAZIONE>"
                'Return mXML

                Return ""
            End Get
        End Property

#End Region

#Region "Metodi Pubblici"

        Public Sub New()

        End Sub

#End Region

    End Class
End Namespace

