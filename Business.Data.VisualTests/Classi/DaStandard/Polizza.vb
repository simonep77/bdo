Namespace VecchieClassi
    Public Class Polizza

        Inherits BaseClass

        Private mIdPolizza As Integer
        Private mCodCombinazioneCopertureAssicurative As Integer
        Private mCodDurataPolizze As Integer
        Private mCodAzienda As String
        Private mCodConvenzioneCollettiva As Integer
        Private mCodModalitaPagamento As Integer
        Private mCodFrazionamentoPagamento As Integer
        Private mDataDecorrenza As Date
        Private mDataScadenza As Date

        '///////////////////////////////////////////////////////
        Private mCodPaese As String
        Private mCheckDigit As String
        Private mCIN As String
        Private mABI As String
        Private mCAB As String
        Private mNumeroCC As String
        '///////////////////////////////////////////////////////
        Private mCodPaeseRichiesta As String
        Private mCheckDigitRichiesta As String
        Private mCINRichiesta As String
        Private mABIRichiesta As String
        Private mCABRichiesta As String
        Private mNumeroCCRichiesta As String
        '///////////////////////////////////////////////////////
        Private mDataModificaIBAN As Date

        Private mIndirizzoCorrispondenza As String
        Private mCapCorrispondenza As String
        Private mLocalitaCorrispondenza As String
        Private mCodProvCorrispondenza As String
        Private mProvCorrispondenza As Provincia
        Private mMesiCarenza As Integer
        Private mNote As String
        Private mDataDaFondo As Date
        Private mCodSito As Integer

#Region "PROPERTY"

        Public Property IdPolizza() As Integer
            Get
                Return mIdPolizza
            End Get
            Set(ByVal Value As Integer)
                mIdPolizza = Value
            End Set
        End Property

        Public Property codCombinazioneCopertureAssicurative() As Integer
            Get
                Return mCodCombinazioneCopertureAssicurative
            End Get
            Set(ByVal Value As Integer)
                mCodCombinazioneCopertureAssicurative = Value
            End Set
        End Property

        Public Property codDurataPolizze() As Integer
            Get
                Return mCodDurataPolizze
            End Get
            Set(ByVal Value As Integer)
                mCodDurataPolizze = Value
            End Set
        End Property

        Public Property CodAzienda() As String
            Get
                Return mCodAzienda.ToUpper()
            End Get
            Set(ByVal Value As String)
                mCodAzienda = Value
            End Set
        End Property

        Public Property codConvenzioneCollettiva() As Integer
            Get
                Return mCodConvenzioneCollettiva
            End Get
            Set(ByVal Value As Integer)
                mCodConvenzioneCollettiva = Value
            End Set
        End Property

        Public Property codModalitaPagamento() As Integer
            Get
                Return mCodModalitaPagamento
            End Get
            Set(ByVal Value As Integer)
                mCodModalitaPagamento = Value
            End Set
        End Property

        Public Property codFrazionamentoPagamento() As Integer
            Get
                Return mCodFrazionamentoPagamento
            End Get
            Set(ByVal Value As Integer)
                mCodFrazionamentoPagamento = Value
            End Set
        End Property

        Public Property dataDecorrenza() As Date
            Get
                Return mDataDecorrenza
            End Get

            Set(ByVal Value As Date)
                mDataDecorrenza = Value
            End Set
        End Property

        Public Property dataScadenza() As Date
            Get
                Return mDataScadenza
            End Get

            Set(ByVal Value As Date)
                mDataScadenza = Value
            End Set
        End Property

        Public Property codPaese() As String
            Get
                Return mCodPaese
            End Get
            Set(ByVal Value As String)
                mCodPaese = Value
            End Set
        End Property

        Public Property checkDigit() As String
            Get
                Return mCheckDigit
            End Get
            Set(ByVal Value As String)
                mCheckDigit = Value
            End Set
        End Property

        Public Property CIN() As String
            Get
                Return mCIN
            End Get
            Set(ByVal Value As String)
                mCIN = Value
            End Set
        End Property

        Public Property ABI() As String
            Get
                Return mABI
            End Get
            Set(ByVal Value As String)
                mABI = Value
            End Set
        End Property

        Public Property CAB() As String
            Get
                Return mCAB
            End Get
            Set(ByVal Value As String)
                mCAB = Value
            End Set
        End Property

        Public Property numeroCC() As String
            Get
                Return mNumeroCC
            End Get
            Set(ByVal Value As String)
                mNumeroCC = Value
            End Set
        End Property

        Public Property dataModificaIBAN() As Date
            Get
                Return mDataModificaIBAN
            End Get

            Set(ByVal Value As Date)
                mDataModificaIBAN = Value
            End Set
        End Property

        Public Property indirizzoCorrispondenza() As String
            Get
                Return mIndirizzoCorrispondenza
            End Get

            Set(ByVal Value As String)
                mIndirizzoCorrispondenza = Value.ToUpper
            End Set
        End Property

        Public Property capCorrispondenza() As String
            Get
                Return mCapCorrispondenza
            End Get

            Set(ByVal Value As String)
                mCapCorrispondenza = Value.ToUpper
            End Set
        End Property

        Public Property localitaCorrispondenza() As String
            Get
                Return mLocalitaCorrispondenza
            End Get

            Set(ByVal Value As String)
                mLocalitaCorrispondenza = Value.ToUpper
            End Set
        End Property

        Public Property codProvCorrispondenza() As String
            Get
                Return mCodProvCorrispondenza
            End Get

            Set(ByVal Value As String)
                mCodProvCorrispondenza = Value.ToUpper
            End Set
        End Property



        Public Property mesiCarenza() As Integer
            Get
                Return mMesiCarenza
            End Get
            Set(ByVal Value As Integer)
                mMesiCarenza = Value
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

        Public Property dataDaFondo() As Date
            Get
                Return mDataDaFondo
            End Get

            Set(ByVal Value As Date)
                mDataDaFondo = Value
            End Set
        End Property

        Public Property codPaeseRichiesta() As String
            Get
                Return mCodPaeseRichiesta
            End Get
            Set(ByVal Value As String)
                mCodPaeseRichiesta = Value
            End Set
        End Property

        Public Property checkDigitRichiesta() As String
            Get
                Return mCheckDigitRichiesta
            End Get
            Set(ByVal Value As String)
                mCheckDigitRichiesta = Value
            End Set
        End Property

        Public Property CINRichiesta() As String
            Get
                Return mCINRichiesta
            End Get
            Set(ByVal Value As String)
                mCINRichiesta = Value
            End Set
        End Property

        Public Property ABIRichiesta() As String
            Get
                Return mABIRichiesta
            End Get
            Set(ByVal Value As String)
                mABIRichiesta = Value
            End Set
        End Property

        Public Property CABRichiesta() As String
            Get
                Return mCABRichiesta
            End Get
            Set(ByVal Value As String)
                mCABRichiesta = Value
            End Set
        End Property

        Public Property numeroCCRichiesta() As String
            Get
                Return mNumeroCCRichiesta
            End Get
            Set(ByVal Value As String)
                mNumeroCCRichiesta = Value
            End Set
        End Property

        Public Property codSito() As Integer
            Get
                Return mCodSito
            End Get
            Set(ByVal Value As Integer)
                mCodSito = Value
            End Set
        End Property

        Public ReadOnly Property IbanStr() As String
            Get
                Return String.Concat(Me.mCodPaese, Me.mCheckDigit, Me.mCIN, Me.mABI, Me.mCAB, Me.mNumeroCC)
            End Get
        End Property

        Public ReadOnly Property IbanRichiestaStr() As String
            Get
                Return String.Concat(Me.mCodPaeseRichiesta, Me.mCheckDigitRichiesta, Me.mCINRichiesta, Me.mABIRichiesta, Me.mCABRichiesta, Me.mNumeroCCRichiesta)
            End Get
        End Property

#End Region

#Region "PUBBLICO"

        Public Sub New(ByRef objSlot As BusinessSlot)
            Me.Polizza(objSlot)
        End Sub

        Public Sub New(ByRef objSlotIn As BusinessSlot, ByVal idPolizzaIn As Integer)
            Me.Polizza(objSlotIn, idPolizzaIn)
        End Sub

        Public Sub SalvaModalitaPagamento()

            Dim sQuery As String = " UPDATE polizze SET " & _
                                    " CodModalitaPagamento = " & numeroPerSql(Me.codModalitaPagamento) & _
                                    " WHERE Idpolizza = " & Me.IdPolizza

            Me.currentSlot.dbConnection.SQL = sQuery

            If Me.currentSlot.dbConnection.ExecSQL() < 0 Then
                Throw New EFaschimError("Errore DB: {0}", Me.currentSlot.dbConnection.StrErr)
            End If

        End Sub
        Public Sub SalvaIBANTemp()

            Dim sQuery As String = " UPDATE polizze SET " & _
                                    " codPaeseRichiesta = " & StringaSql(Me.codPaeseRichiesta) & ", " & _
                                    " checkDigitRichiesta = " & StringaSql(Me.checkDigitRichiesta) & ", " & _
                                    " CINRichiesta = " & StringaSql(Me.CINRichiesta) & ", " & _
                                    " ABIRichiesta = " & StringaSql(Me.ABIRichiesta) & ", " & _
                                    " CABRichiesta = " & StringaSql(Me.CABRichiesta) & ", " & _
                                    " CCRichiesta = " & StringaSql(Me.numeroCCRichiesta) & _
                                    " WHERE Idpolizza = " & Me.IdPolizza

            Me.currentSlot.dbConnection.SQL = sQuery

            If Me.currentSlot.dbConnection.ExecSQL() < 0 Then
                Throw New EFaschimError("Errore DB: {0}", Me.currentSlot.dbConnection.StrErr)
            End If

        End Sub

        Public Sub SalvaIBANDef()

            'Se iban temp e iban anag sono uguali non effettua il salvataggio

            Dim sQuery As String = " UPDATE polizze SET " & _
                                    " codPaese = " & stringaPerSql(Me.codPaese) & ", " & _
                                    " checkDigit = " & stringaPerSql(Me.checkDigit) & ", " & _
                                    " CIN= " & stringaPerSql(Me.CIN) & ", " & _
                                    " ABI = " & stringaPerSql(Me.ABI) & ", " & _
                                    " CAB = " & stringaPerSql(Me.CAB) & ", " & _
                                    " NumeroCC = " & stringaPerSql(Me.numeroCC) & ", " & _
                                    " DataModificaIBAN = " & dataOraPerSql(Me.dataModificaIBAN) & _
                                    " WHERE Idpolizza = " & Me.IdPolizza

            Me.currentSlot.dbConnection.SQL = sQuery

            If Me.currentSlot.dbConnection.ExecSQL() < 0 Then
                Throw New EFaschimError("Errore DB: {0}", Me.currentSlot.dbConnection.StrErr)
            End If

        End Sub

        Public Sub Elimina()

            Dim qRet As Integer = 0

            'Controlli id
            If Me.mIdPolizza = 0 Then
                Throw New EFaschimError("ID Polizza non valido per questa operazione")
            End If

            'Prepara ed esegue sql
            Me.currentSlot.dbConnection.SQL = String.Format("DELETE FROM Polizze WHERE IdPolizza = {0}", Me.mIdPolizza)
            qRet = Me.currentSlot.dbConnection.ExecSQL()
            If qRet < 0 Then
                Throw New EFaschimError("{0} - Errore SQL: {1}", Me.GetType.Name, Me.currentSlot.dbConnection.StrErr)
            End If

        End Sub


#End Region

#Region "PRIVATO"

        Private Sub Polizza(ByRef objSlot As BusinessSlot)

            Me.currentSlot = objSlot

            Me.mIdPolizza = 0
            Me.mCodCombinazioneCopertureAssicurative = 0
            Me.mCodDurataPolizze = 0
            Me.mCodAzienda = ""
            Me.mCodConvenzioneCollettiva = 0
            Me.mCodModalitaPagamento = 0
            Me.mCodFrazionamentoPagamento = 0
            Me.mDataDecorrenza = Date.MinValue
            Me.mDataScadenza = Date.MinValue
            Me.mCodPaese = ""
            Me.mCheckDigit = ""
            Me.mCIN = ""
            Me.mABI = ""
            Me.mCAB = ""
            Me.mNumeroCC = ""
            Me.mDataModificaIBAN = Date.MinValue
            Me.mIndirizzoCorrispondenza = ""
            Me.mCapCorrispondenza = ""
            Me.mLocalitaCorrispondenza = ""
            Me.mCodProvCorrispondenza = ""
            Me.mMesiCarenza = 0
            Me.mNote = ""
            Me.mDataDaFondo = Date.MinValue
            Me.mCodPaeseRichiesta = ""
            Me.mCheckDigitRichiesta = ""
            Me.mCINRichiesta = ""
            Me.mABIRichiesta = ""
            Me.mCABRichiesta = ""
            Me.mNumeroCCRichiesta = ""
            Me.mCodSito = 0

        End Sub

        Private Sub Polizza(ByRef objSlotIn As BusinessSlot, ByVal idPolizzaIn As Integer)

            Dim qRet As Integer

            Try
                Me.Polizza(objSlotIn)

                'Controlla codice
                If idPolizzaIn <= 0 Then
                    Throw New FaschimHandledException("Id Polizza non valorizzato")
                End If

                'Esegue query
                Me.currentSlot.dbConnection.SQL = String.Format("select * from polizze where IdPolizza = {0}", idPolizzaIn)
                qRet = Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet)

                If qRet > 0 Then
                    Me.ImpostaAttributi()
                ElseIf qRet = 0 Then
                    Throw New FaschimHandledException(String.Format("Nessun record trovato per Id {0}", idPolizzaIn))
                Else
                    Throw New FaschimHandledException(String.Format("Errore SQL: {0}", Me.currentSlot.dbConnection.StrErr))
                End If

            Catch ex As Exception
                Me.currentSlot.ElencoEccezioni.AddEccezione(1, ex.Message, Me.GetType.Name)
            End Try

        End Sub

        Private Sub ImpostaAttributi()

            Dim aRow As DataRow = Me.localDataSet.Tables(0).Rows(0)

            Me.mIdPolizza = aRow("IdPolizza")
            Me.mCodCombinazioneCopertureAssicurative = aRow("CodCombinazioneCopertureAssicurative")
            Me.mCodDurataPolizze = aRow("CodDurataPolizza")
            Me.mCodAzienda = DBGet(aRow("CodAzienda"), "")
            Me.mCodConvenzioneCollettiva = aRow("CodConvenzioneCollettiva")
            Me.mCodModalitaPagamento = aRow("CodModalitaPagamento")
            Me.mCodFrazionamentoPagamento = aRow("CodFrazionamentoPagamento")
            Me.mDataDecorrenza = DBGet(aRow("DataDecorrenza"), Date.MinValue)
            Me.mDataScadenza = DBGet(aRow("DataScadenza"), Date.MinValue)
            Me.mCodPaese = DBGet(aRow("CodPaese"), "")
            Me.mCheckDigit = DBGet(aRow("CheckDigit"), "")
            Me.mCIN = DBGet(aRow("CIN"), "")
            Me.mABI = DBGet(aRow("ABI"), "")
            Me.mCAB = DBGet(aRow("CAB"), "")
            Me.mNumeroCC = DBGet(aRow("NumeroCC"), "")
            Me.mDataModificaIBAN = DBGet(aRow("DataModificaIBAN"), Date.MinValue)
            Me.mIndirizzoCorrispondenza = DBGet(aRow("IndirizzoCorrispondenza"), "")
            Me.mCapCorrispondenza = DBGet(aRow("CapCorrispondenza"), "")
            Me.mLocalitaCorrispondenza = DBGet(aRow("LocalitaCorrispondenza"), "")
            Me.mCodProvCorrispondenza = DBGet(aRow("ProvinciaCorrispondenza"), "")
            Me.mMesiCarenza = DBGet(aRow("MesiCarenza"), 0)
            Me.mNote = DBGet(aRow("Note"), "")
            Me.mDataDaFondo = DBGet(aRow("DataDaFondo"), Date.MinValue)
            Me.mCodPaeseRichiesta = DBGet(aRow("CodPaeseRichiesta"), "")
            Me.mCheckDigitRichiesta = DBGet(aRow("CheckDigitRichiesta"), "")
            Me.mCINRichiesta = DBGet(aRow("CINRichiesta"), "")
            Me.mABIRichiesta = DBGet(aRow("ABIRichiesta"), "")
            Me.mCABRichiesta = DBGet(aRow("CABRichiesta"), "")
            Me.mNumeroCCRichiesta = DBGet(aRow("CCRichiesta"), "")
            Me.mCodSito = DBGet(aRow("CodSito"), 0)

        End Sub

#End Region

    End Class

End Namespace

