Namespace VecchieClassi


    Public Class BusinessSlot
        Private mDbConnection As DataBase
        Private mElencoEccezioni As ElencoEccezioni
        Public Operatore As String = String.Empty
        Public CodOperatore As Integer
        Public CodiceUfficio As Integer
        Public TipoSoggetto As String = String.Empty
        Public IndrizzoIP As String = String.Empty
        Public SessionNumber As String = String.Empty

        Public ReadOnly Property dbConnection() As DataBase
            Get
                If mDbConnection Is Nothing Then
                    mDbConnection = New DataBase
                End If
                Return mDbConnection
            End Get
        End Property

        Public ReadOnly Property ElencoEccezioni() As ElencoEccezioni
            Get
                Return mElencoEccezioni
            End Get
        End Property



        Public Sub New()

            'mDbConnection = CType(newByReflection(currSessionInfo.clientInstance.dbClassName), DataBaseInterface)
            mDbConnection = Nothing

            mElencoEccezioni = New ElencoEccezioni

        End Sub

        Public Sub New(ByVal dbIn As DataBase)
            Me.New()

            mDbConnection = dbIn
        End Sub

        Public Function errorOccurr() As Boolean

            Return (mElencoEccezioni.getCount > 0)

        End Function

    End Class

End Namespace

