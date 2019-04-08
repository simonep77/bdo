Namespace VecchieClassi
    Public Class IscrittiPerPolizza

        Inherits BaseClass

        Private mIdIscrittoPolizza As Integer
        Private mIdPolizza As Integer
        Private mPolizza As Polizza
        Private mIdAnagrafica As String
        Private mCodStatoIscritto As Integer
        Private mCodGradoParentela As Integer
        Private mDataIscrizione As Date
        Private mDataIscrizionecalcolata As Date = Nothing
        Private mDataCessazione As Date
        Private mCodMotivoCessazione As Integer
        Private mMesiCarenza As Integer
        Private mConfluente As Boolean
        Private mFiscCarico As Boolean
        Private mDataInserimento As Date
        Private mAnagrafica As AnagraficaNew
        Private mStatoIscritto As StatoIscritto




#Region "PROPERTY"


        Public Property IdIscrittoPolizza() As Integer
            Get
                Return mIdIscrittoPolizza
            End Get
            Set(ByVal Value As Integer)
                mIdIscrittoPolizza = Value
            End Set
        End Property

        Public ReadOnly Property Polizza() As Polizza
            Get
                If Me.mPolizza Is Nothing AndAlso Me.mIdPolizza > 0 Then
                    Me.mPolizza = New Polizza(Me.currentSlot, Me.mIdPolizza)
                End If
                Return mPolizza
            End Get

        End Property



        Public ReadOnly Property CodStatoIscritto() As Integer
            Get
                Return Me.mCodStatoIscritto
            End Get
        End Property

        Public ReadOnly Property idPolizza() As Integer
            Get
                Return Me.mIdPolizza
            End Get
        End Property

        Public ReadOnly Property CodMotivoCessazione() As Integer
            Get
                Return Me.mCodMotivoCessazione
            End Get
        End Property


   

        Public ReadOnly Property CodGradoParentela() As Integer
            Get
                Return Me.mCodGradoParentela
            End Get
        End Property

        Public Property DataIscrizione() As DateTime
            Get
                Return mDataIscrizione
            End Get

            Set(ByVal Value As DateTime)
                mDataIscrizione = Value
            End Set
        End Property

        Public Property DataCessazione() As Date
            Get
                Return mDataCessazione
            End Get

            Set(ByVal Value As Date)
                mDataCessazione = Value
            End Set
        End Property




        Public Property MesiCarenza() As Integer
            Get
                Return mMesiCarenza
            End Get
            Set(ByVal Value As Integer)
                mMesiCarenza = Value
            End Set
        End Property

        Public Property Confluente() As Boolean
            Get
                Return mConfluente
            End Get

            Set(ByVal Value As Boolean)
                mConfluente = Value
            End Set
        End Property

        Public Property FiscCarico() As Boolean
            Get
                Return mFiscCarico
            End Get

            Set(ByVal Value As Boolean)
                mFiscCarico = Value
            End Set
        End Property

        Public Property DataInserimento() As DateTime
            Get
                Return mDataInserimento
            End Get

            Set(ByVal Value As DateTime)
                mDataInserimento = Value
            End Set
        End Property


        Public ReadOnly Property Anagrafica() As AnagraficaNew
            Get
                If Me.mAnagrafica Is Nothing AndAlso Not String.IsNullOrEmpty(Me.mIdAnagrafica) Then
                    Me.mAnagrafica = New AnagraficaNew(Me.currentSlot, Me.mIdAnagrafica)
                End If
                Return mAnagrafica
            End Get

        End Property

        Public ReadOnly Property StatoIscritto() As StatoIscritto
            Get
                If Me.mStatoIscritto Is Nothing AndAlso Me.mCodStatoIscritto > 0 Then
                    Me.mStatoIscritto = New StatoIscritto(Me.mCodStatoIscritto)
                End If
                Return mStatoIscritto
            End Get

        End Property

#End Region

#Region "PUBBLICO"

        Public Sub New(ByRef objSlot As BusinessSlot)
            Me.IscrittiPerPolizza(objSlot)
        End Sub

        Public Sub New(ByRef objSlotIn As BusinessSlot, ByVal idIscrittoPolizzaIn As Long)
            Me.IscrittiPerPolizza(objSlotIn, idIscrittoPolizzaIn)
        End Sub

        Public Sub New(ByRef objSlotIn As BusinessSlot, ByVal idPolizzaIn As Long, ByVal override As Boolean)
            Me.IscrittiPerPolizza(objSlotIn, idPolizzaIn, override)
        End Sub

        Public Sub New(ByRef objSlotIn As BusinessSlot, ByVal idPolizzaIn As Long, ByVal codAnagraficaIn As String)
            Me.IscrittiPerPolizza(objSlotIn, idPolizzaIn, codAnagraficaIn)
        End Sub

  

#End Region

#Region "PRIVATO"

        Private Sub IscrittiPerPolizza(ByRef objSlot As BusinessSlot)

            Me.currentSlot = objSlot

            Me.mIdIscrittoPolizza = 0
            Me.mIdPolizza = 0
            Me.mIdAnagrafica = ""
            Me.mCodStatoIscritto = 0
            Me.mCodGradoParentela = 0
            Me.mDataIscrizione = Date.MinValue
            Me.mDataCessazione = Date.MinValue
            Me.mCodMotivoCessazione = 0
            Me.mMesiCarenza = 0
            Me.mConfluente = False
            Me.mFiscCarico = False
            Me.mDataInserimento = Date.MinValue

        End Sub

        Private Sub IscrittiPerPolizza(ByRef objSlotIn As BusinessSlot, ByVal idIscrittoPolizzaIn As Long)

            Dim qRet As Integer

            Try
                Me.IscrittiPerPolizza(objSlotIn)

                'Controlla codice
                If idIscrittoPolizzaIn <= 0 Then
                    Throw New FaschimHandledException("Id Iscritto per Polizza non valorizzato")
                End If

                'Esegue query
                Me.currentSlot.dbConnection.SQL = String.Format("select * from iscrittiperpolizza where IdIscrittiPolizza = {0}", idIscrittoPolizzaIn)
                qRet = Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet)

                If qRet > 0 Then
                    Me.ImpostaAttributi()
                ElseIf qRet = 0 Then
                    Throw New FaschimHandledException(String.Format("Nessun record trovato per Id {0}", idIscrittoPolizzaIn))
                Else
                    Throw New FaschimHandledException(String.Format("Errore SQL: {0}", Me.currentSlot.dbConnection.StrErr))
                End If

            Catch ex As Exception
                Me.currentSlot.ElencoEccezioni.AddEccezione(1, ex.Message, Me.GetType.Name)
            End Try

        End Sub

        ''' <summary>
        ''' Cerca intestatario
        ''' </summary>
        ''' <param name="objSlotIn"></param>
        ''' <param name="idPolizzaIn"></param>
        ''' <param name="override"></param>
        ''' <remarks></remarks>
        Private Sub IscrittiPerPolizza(ByRef objSlotIn As BusinessSlot, ByVal idPolizzaIn As Long, ByVal override As Boolean)

            Dim qRet As Integer

            Try
                Me.IscrittiPerPolizza(objSlotIn)

                'Controlla codice
                If idPolizzaIn <= 0 Then
                    Throw New FaschimHandledException("Id  Polizza non valorizzato")
                End If

                'Esegue query
                Me.currentSlot.dbConnection.SQL = String.Format("SELECT * FROM iscrittiperpolizza WHERE IdPolizza = {0} AND CodGradoParentela = {1}", idPolizzaIn, CODICE_INTESTATARIO)
                qRet = Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet)

                If qRet > 0 Then
                    Me.ImpostaAttributi()
                ElseIf qRet = 0 Then
                    Throw New FaschimHandledException(String.Format("Nessun intestatario trovato per Polizza {0}", idPolizzaIn))
                Else
                    Throw New FaschimHandledException(String.Format("Errore SQL: {0}", Me.currentSlot.dbConnection.StrErr))
                End If

            Catch ex As Exception
                Me.currentSlot.ElencoEccezioni.AddEccezione(1, ex.Message, Me.GetType.Name)
            End Try

        End Sub


        Private Sub ImpostaAttributi()

            Dim aRow As DataRow = Me.localDataSet.Tables(0).Rows(0)

            Me.mIdIscrittoPolizza = aRow("IdIscrittiPolizza")
            Me.mIdPolizza = aRow("IdPolizza")
            Me.mIdAnagrafica = aRow("IdAnagrafica")
            Me.mCodStatoIscritto = aRow("CodStatoIscritto")
            Me.mCodGradoParentela = aRow("CodGradoParentela")
            Me.mDataIscrizione = aRow("DataIscrizione")
            Me.mDataCessazione = DBGet(aRow("DataCessazione"), Date.MinValue)
            Me.mCodMotivoCessazione = DBGet(aRow("CodMotivoCessazione"), 0)
            Me.mMesiCarenza = DBGet(aRow("MesiCarenza"), 0)
            Me.mConfluente = Convert.ToInt16(DBGet(aRow("Confluente"), "0"))
            Me.mFiscCarico = Convert.ToInt16(aRow("FiscCarico"))
            Me.mDataInserimento = DBGet(aRow("DataInserimento"), Date.MinValue)

        End Sub

#End Region

    End Class
End Namespace
