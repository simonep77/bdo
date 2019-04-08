Namespace VecchieClassi

    Public Class BaseElenco
        Inherits Paginazione

        Private mElenco As Collection = New Collection()

        Public Property localElenco() As Collection
            Get
                Return mElenco
            End Get
            Set(ByVal Value As Collection)
                If Value Is Nothing Then
                    Me.mElenco = New Collection()
                Else
                    Me.mElenco = Value
                End If
            End Set
        End Property

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Svuota l'elenco corrente
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub svuotaElenco()
            Me.mElenco.Clear()

            Me.localDataSet.Reset()
        End Sub

        ''' <summary>
        ''' Aggiunge Oggetto ad elenco
        ''' </summary>
        ''' <param name="objIn"></param>
        ''' <remarks></remarks>
        Protected Sub AddToElenco(ByVal objIn As Object)
            Me.mElenco.Add(objIn)
        End Sub

        ''' <summary>
        ''' Esegue la query impostata e decide se paginata o meno in base alle impostazioni iniziali
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub EseguiRicerca(Optional ByVal svuotaElenco As Boolean = True)

            If svuotaElenco Then
                Me.svuotaElenco()
            End If

            Me.localDataSet.Reset()

            If Me.Pagina = 0 Then
                If Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet) < 0 Then
                    Throw New EFaschimError("{0} - Errore query: {1}", Me.GetType().Name, Me.currentSlot.dbConnection.StrErr)
                End If
            Else
                If Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet, Me.Posizione, Me.Offset, Me.TotRecord) < 0 Then
                    Throw New EFaschimError("{0} - Errore query: {1}", Me.GetType().Name, Me.currentSlot.dbConnection.StrErr)
                End If
            End If
        End Sub

    End Class

End Namespace