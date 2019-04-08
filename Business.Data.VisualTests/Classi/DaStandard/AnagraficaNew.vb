Namespace VecchieClassi



    Public Class AnagraficaNew

        Inherits BaseClass

        Private mIdAnagrafica As String
        Private mCodAnagraficaConiuge As String
        Private mAnagraficaConiuge As AnagraficaNew
        Private mCodStatoSalute As Integer
        Private mCodProfessione As Integer
        Private mCognome As String
        Private mNome As String
        Private mSesso As String
        Private mDataNascita As Date
        Private mLuogoNascita As String
        Private mCodiceFiscale As String
        Private mDataInserimento As DateTime
        Private mDataDecesso As DateTime
        Private mTelefonoAbitazione As String
        Private mTelefonoUfficio As String
        Private mTelefonoCellulare As String
        Private mFax As String
        Private mEMail As String
        Private mIndirizzo As String
        Private mCap As String
        Private mLocalita As String
        Private mCodProvincia As String
        Private mProvincia As Provincia
        Private mNote As String
        Private mCodProvNascita As String
        Private mProvNascita As Provincia
        Private mCodInnalzamentoLimite As Integer
        Private mConsensoDati As Boolean
        Private mDataScadenzaPassword As Date
        Private mPrimoAccesso As Boolean
        Private mPassword As String
        Private mIdentificativoVP As String
        Private mDataInizioRapporto As Date
        Private mDataFineRapporto As Date

#Region "PROPERTY"

        Public Property IdAnagrafica() As String
            Get
                Return mIdAnagrafica
            End Get
            Set(ByVal Value As String)
                mIdAnagrafica = Value
            End Set
        End Property

        Public ReadOnly Property AnagraficaConiuge() As AnagraficaNew
            Get
                If Me.mAnagraficaConiuge Is Nothing AndAlso Not String.IsNullOrEmpty(Me.mCodAnagraficaConiuge) Then
                    Me.mAnagraficaConiuge = New AnagraficaNew(Me.currentSlot, Me.mCodAnagraficaConiuge)
                End If
                Return Me.mAnagraficaConiuge
            End Get
        End Property

        Public Property codStatoSalute() As Integer
            Get
                Return mCodStatoSalute
            End Get
            Set(ByVal Value As Integer)
                mCodStatoSalute = Value
            End Set
        End Property

        Public Property codProfessione() As Integer
            Get
                Return mCodProfessione
            End Get
            Set(ByVal Value As Integer)
                mCodProfessione = Value
            End Set
        End Property

        Public Property Cognome() As String
            Get
                Return mCognome
            End Get
            Set(ByVal Value As String)
                mCognome = Value.ToUpper
            End Set
        End Property

        Public Property Nome() As String
            Get
                Return mNome
            End Get

            Set(ByVal Value As String)
                mNome = Value.ToUpper
            End Set
        End Property

        Public Property sesso() As String
            Get
                Return mSesso
            End Get

            Set(ByVal Value As String)
                mSesso = Value.ToUpper
            End Set
        End Property

        Public Property dataNascita() As Date
            Get
                Return mDataNascita
            End Get

            Set(ByVal Value As Date)
                mDataNascita = Value
            End Set
        End Property


        Public Property luogoNascita() As String
            Get
                Return mLuogoNascita
            End Get

            Set(ByVal Value As String)
                mLuogoNascita = Value.ToUpper
            End Set
        End Property

        Public Property codiceFiscale() As String
            Get
                Return mCodiceFiscale
            End Get

            Set(ByVal Value As String)
                mCodiceFiscale = Value.ToUpper
            End Set
        End Property
        Public Property dataInserimento() As DateTime
            Get
                Return mDataInserimento
            End Get

            Set(ByVal Value As DateTime)
                mDataInserimento = Value
            End Set
        End Property

        Public Property dataDecesso() As DateTime
            Get
                Return mDataDecesso
            End Get

            Set(ByVal Value As DateTime)
                mDataDecesso = Value
            End Set
        End Property

        Public Property telefonoAbitazione() As String
            Get
                Return mTelefonoAbitazione
            End Get

            Set(ByVal Value As String)
                mTelefonoAbitazione = Value.ToUpper
            End Set
        End Property

        Public Property telefonoUfficio() As String
            Get
                Return mTelefonoUfficio
            End Get

            Set(ByVal Value As String)
                mTelefonoUfficio = Value.ToUpper
            End Set
        End Property

        Public Property telefonoCellulare() As String
            Get
                Return mTelefonoCellulare
            End Get

            Set(ByVal Value As String)
                mTelefonoCellulare = Value.ToUpper
            End Set
        End Property

        Public Property fax() As String
            Get
                Return mFax
            End Get

            Set(ByVal Value As String)
                mFax = Value.ToUpper
            End Set
        End Property

        Public Property eMail() As String
            Get
                Return mEMail
            End Get

            Set(ByVal Value As String)
                mEMail = Value.ToUpper
            End Set
        End Property

        Public Property indirizzo() As String
            Get
                Return mIndirizzo
            End Get

            Set(ByVal Value As String)
                mIndirizzo = Value.ToUpper
            End Set
        End Property

        Public Property cap() As String
            Get
                Return mCap
            End Get

            Set(ByVal Value As String)
                mCap = Value.ToUpper
            End Set
        End Property

        Public Property localita() As String
            Get
                Return mLocalita
            End Get

            Set(ByVal Value As String)
                mLocalita = Value.ToUpper
            End Set
        End Property

        Public Property Provincia() As Provincia
            Get
                If Me.mProvincia Is Nothing AndAlso Me.mCodProvincia <> "" Then
                    Me.mProvincia = New Provincia(Me.mCodProvincia)
                End If
                Return mProvincia
            End Get

            Set(ByVal Value As Provincia)
                mCodProvincia = Value.codice
                mProvincia = Value
            End Set
        End Property



        Public Property note() As String
            Get
                Return mNote
            End Get
            Set(ByVal Value As String)
                mNote = Value
            End Set
        End Property

        Public Property codProvNascita() As String
            Get
                Return mCodProvNascita
            End Get

            Set(ByVal Value As String)
                mCodProvNascita = Value.ToUpper
            End Set
        End Property



        Public Property codInnalzamentoLiminte() As Integer
            Get
                Return mCodInnalzamentoLimite
            End Get
            Set(ByVal Value As Integer)
                mCodInnalzamentoLimite = Value
            End Set
        End Property

        Public Property consensoDati() As Boolean
            Get
                Return mConsensoDati
            End Get
            Set(ByVal Value As Boolean)
                mConsensoDati = Value
            End Set
        End Property

        Public Property dataScadenzaPassword() As Date
            Get
                Return mDataScadenzaPassword
            End Get

            Set(ByVal Value As Date)
                mDataScadenzaPassword = Value
            End Set
        End Property

        Public Property primoAccesso() As Boolean
            Get
                Return mPrimoAccesso
            End Get
            Set(ByVal Value As Boolean)
                mPrimoAccesso = Value
            End Set
        End Property

        Public Property password() As String
            Get
                Return mPassword
            End Get

            Set(ByVal Value As String)
                mPassword = Value
            End Set
        End Property

        Public Property identificativoVP() As String
            Get
                Return mIdentificativoVP
            End Get

            Set(ByVal Value As String)
                mIdentificativoVP = Value
            End Set
        End Property

        Public Property dataInizioRapporto() As Date
            Get
                Return mDataInizioRapporto
            End Get
            Set(ByVal Value As Date)
                mDataInizioRapporto = Value
            End Set
        End Property

        Public Property dataFineRapporto() As Date
            Get
                Return mDataFineRapporto
            End Get
            Set(ByVal Value As Date)
                mDataFineRapporto = Value
            End Set
        End Property

#End Region

#Region "PUBBLICO"

        Public Sub New(ByRef objSlot As BusinessSlot)
            Me.Anagrafica(objSlot)
        End Sub

        Public Sub New(ByRef objSlotIn As BusinessSlot, ByVal idAnagraficaIn As String)
            Me.Anagrafica(objSlotIn, idAnagraficaIn)
        End Sub

#End Region

#Region "PRIVATO"

        Private Sub Anagrafica(ByRef objSlot As BusinessSlot)

            Me.currentSlot = objSlot

            Me.mIdAnagrafica = String.Empty
            Me.mCodAnagraficaConiuge = String.Empty
            Me.mAnagraficaConiuge = Nothing
            Me.mCodStatoSalute = 0
            Me.mCodProfessione = 0
            Me.mCognome = String.Empty
            Me.mNome = String.Empty
            Me.mSesso = String.Empty
            Me.mDataNascita = Date.MinValue
            Me.mLuogoNascita = String.Empty
            Me.mCodiceFiscale = String.Empty
            Me.mDataInserimento = Date.MinValue
            Me.mDataDecesso = Date.MinValue
            Me.mTelefonoAbitazione = String.Empty
            Me.mTelefonoUfficio = String.Empty
            Me.mTelefonoCellulare = String.Empty
            Me.mFax = String.Empty
            Me.mEMail = String.Empty
            Me.mIndirizzo = String.Empty
            Me.mCap = String.Empty
            Me.mLocalita = String.Empty
            Me.mCodProvincia = String.Empty
            Me.mNote = String.Empty
            Me.mCodProvNascita = String.Empty
            Me.mCodInnalzamentoLimite = 0
            Me.mConsensoDati = False
            Me.mDataScadenzaPassword = Date.MinValue
            Me.mPrimoAccesso = False
            Me.mPassword = String.Empty
            Me.mIdentificativoVP = String.Empty
            Me.mDataInizioRapporto = Date.MinValue
            Me.mDataFineRapporto = Date.MinValue

        End Sub

        Private Sub Anagrafica(ByRef objSlotIn As BusinessSlot, ByVal idAnagraficaIn As String)

            Dim qRet As Integer

            Try
                Me.Anagrafica(objSlotIn)

                'Controlla codice
                If String.IsNullOrEmpty(idAnagraficaIn) Then
                    Throw New FaschimHandledException("Id Anagrafica non valorizzato")
                End If

                'Esegue query
                Me.currentSlot.dbConnection.SQL = String.Format("select * from anagrafica where IdAnagrafica = {0}", stringaPerSql(idAnagraficaIn))
                qRet = Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet)

                If qRet > 0 Then
                    Me.ImpostaAttributi()
                ElseIf qRet = 0 Then
                    Throw New FaschimHandledException(String.Format("Nessuna anagrafica trovata per Id {0}", idAnagraficaIn))
                Else
                    Throw New FaschimHandledException(String.Format("Errore SQL: {0}", Me.currentSlot.dbConnection.StrErr))
                End If

            Catch ex As Exception
                Me.currentSlot.ElencoEccezioni.AddEccezione(1, ex.Message, Me.GetType.Name)
            End Try

        End Sub

        Private Sub ImpostaAttributi()

            Dim aRow As DataRow = Me.localDataSet.Tables(0).Rows(0)

            Me.mIdAnagrafica = aRow("IdAnagrafica")
            Me.mCodStatoSalute = aRow("CodStatoSalute")
            Me.mCodAnagraficaConiuge = DBGet(aRow("CodAnagrafica"), String.Empty)
            Me.mCodProfessione = DBGet(aRow("CodProfessione"), 0)
            Me.mCognome = aRow("Cognome")
            Me.mNome = aRow("Nome")
            Me.mSesso = aRow("Sesso")
            Me.mDataNascita = DBGet(aRow("DataNascita"), Date.MinValue)
            Me.mLuogoNascita = DBGet(aRow("LuogoNascita"), String.Empty)
            Me.mCodiceFiscale = DBGet(aRow("CodiceFiscale"), String.Empty)
            Me.mDataInserimento = DBGet(aRow("DataInserimento"), Date.MinValue)
            Me.mDataDecesso = DBGet(aRow("DataDecesso"), Date.MinValue)
            Me.mTelefonoAbitazione = DBGet(aRow("TelefonoCasa"), String.Empty)
            Me.mTelefonoUfficio = DBGet(aRow("TelefonoUfficio"), String.Empty)
            Me.mTelefonoCellulare = DBGet(aRow("TelefonoMobile"), String.Empty)
            Me.mFax = DBGet(aRow("Fax"), String.Empty)
            Me.mEMail = DBGet(aRow("Email"), String.Empty)
            Me.mIndirizzo = DBGet(aRow("Indirizzo"), String.Empty)
            Me.mCap = DBGet(aRow("Cap"), String.Empty)
            Me.mLocalita = DBGet(aRow("Localita"), String.Empty)
            Me.mCodProvincia = DBGet(aRow("Provincia"), 0)
            Me.mNote = DBGet(aRow("Note"), String.Empty)
            Me.mCodProvNascita = DBGet(aRow("ProvinciaNascita"), 0)
            Me.mCodInnalzamentoLimite = DBGet(aRow("CodInnalzamentoLimite"), 0)
            Me.mConsensoDati = Convert.ToInt32(aRow("ConsensoDati"))
            Me.mDataScadenzaPassword = DBGet(aRow("DataScadenzaPassword"), Date.MinValue)
            Me.mPrimoAccesso = DBGet(aRow("PrimoAccesso"), False)
            Me.mPassword = DBGet(aRow("EncPassword"), String.Empty)
            Me.mIdentificativoVP = DBGet(aRow("IdentificativoVP"), String.Empty)
            Me.mDataInizioRapporto = DBGet(aRow("DataInizioRapporto"), Date.MinValue)
            Me.mDataFineRapporto = DBGet(aRow("DataFineRapporto"), Date.MinValue)

        End Sub

#End Region

#Region "METODI CLASSE"

        Public Function contaCodiciFiscali(ByRef objSlotIn As BusinessSlot, ByVal codiceFiscaleIn As String) As Integer

            Dim qRet As Integer

            Try
                'Controlla codice
                If String.IsNullOrEmpty(codiceFiscaleIn) Then
                    Throw New FaschimHandledException("Codice Fiscale non valorizzato")
                End If

                'Esegue query
                Me.currentSlot.dbConnection.SQL = String.Format("SELECT * FROM Anagrafica WHERE CodiceFiscale = {0}", stringaPerSql(codiceFiscaleIn))
                qRet = Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet)

                If qRet = 0 Then
                    Throw New FaschimHandledException(String.Format("Nessun record trovato per CodiceFiscale {0}", codiceFiscaleIn))
                ElseIf qRet < 0 Then
                    Throw New FaschimHandledException(String.Format("Errore SQL: {0}", Me.currentSlot.dbConnection.StrErr))
                End If

                Return qRet

            Catch ex As Exception
                Me.currentSlot.ElencoEccezioni.AddEccezione(1, ex.Message, Me.GetType.Name)
            End Try
        End Function

#End Region
    End Class
End Namespace
