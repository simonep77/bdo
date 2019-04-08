Imports System.xml
Imports System.IO
Imports System.Configuration
Imports System.Net.Mail

Namespace VecchieClassi
    Module Principale
        'COSTANTI VARIE
        Public Const NUMERO_MESI_LIMITE_INVIO_PRATICA As Integer = 3

        ' -----------------------
        Public Const CODICE_STATO_APERTO As Integer = 1
        Public Const CODICE_STATO_EDITABILE As Integer = 2
        Public Const CODICE_STATO_CHIUSO As Integer = 3

        ' ---- GRADI PARENTELA ----
        Public Const CODICE_INTESTATARIO As Integer = 1
        Public Const CODICE_CONIUGE As Integer = 2
        Public Const CODICE_FIGLIO As Integer = 3

        ' ---- TIPI ENTITA' ----
        Public Const TIPO_ENTITA_IMPRESA_ISCRITTA As Integer = 1
        Public Const TIPO_ENTITA_LAVORATORE_ISCRITTO As Integer = 3
        Public Const TIPO_ENTITA_STRUTTURA_CONVENZIONATA As Integer = 5
        Public Const TIPO_ENTITA_UFFICIO_INTERNO As Integer = 8

        ' ---- SOGGETTI PAGANTI ----
        Public Const SOGGETTO_PAGANTE_AZIENDA As Char = "A"
        Public Const SOGGETTO_PAGANTE_ISCRITTO As Char = "I"

        ' ---- UFFICI --
        Public Const UFFICIO_SCHEDULATORE As Integer = 11
        ' ---- UFFICI INTERNI --
        Public Const UFFICIO_ANAGRAFICA As Integer = 1
        Public Const UFFICIO_TARIFFAZIONE As Integer = 2
        Public Const UFFICIO_PRESTAZIONI As Integer = 3
        Public Const UFFICIO_PROTOCOLLO As Integer = 5

        ' ---- OPERATORI --
        Public Const OPERATORE_SCHEDULATORE As Integer = 36

        ' ---- STATI PRATICA --
        Public Const CODICE_STATO_PRATICA_PROTOCOLLATA As Integer = 1
        Public Const CODICE_STATO_PRATICA_NON_LIQUIDABILE_CARENZA_INSUFFICIENTE As Integer = 2

        Public Const CODICE_STATO_PRATICA_SOSPESA_MOROSITA As Integer = 5
        Public Const CODICE_STATO_PRATICA_NON_LIQUIDABILE_DATA_NON_COPERTA As Integer = 6
        Public Const CODICE_STATO_PRATICA_NON_LIQUIDABILE_SOSPENSIONE As Integer = 7
        Public Const CODICE_STATO_PRATICA_SOSPESA_INT_DOCUMENTAZIONE As Integer = 9
        Public Const CODICE_STATO_PRATICA_SOSPESA_TEMPORANEAMENTE As Integer = 10
        Public Const CODICE_STATO_PRATICA_LIQUIDATA As Integer = 11
        Public Const CODICE_STATO_PRATICA_ANNULLATA As Integer = 12
        Public Const CODICE_STATO_PRATICA_NON_LIQUIDABILE_SCADENZA_TERMINI_PRESENTAZIONE As Integer = 15
        Public Const CODICE_STATO_PRATICA_NON_LIQUIDABILE_DATA_OLTRE_CESSAZIONE As Integer = 16
        Public Const CODICE_STATO_PRATICA_IN_RIVISITAZIONE As Integer = 23
        Public Const CODICE_STATO_PRATICA_IN_LIQUIDAZIONE As Integer = 24
        Public Const CODICE_STATO_PRATICA_TARIFFATA As Integer = 27
        Public Const CODICE_STATO_PRATICA_SOSPESA_PASS_IMPRESA As Integer = 29
        Public Const CODICE_STATO_PRATICA_SBLOCCATA_SEGNALA_RICHIESTE As Integer = 35
        Public Const CODICE_STATO_PRATICA_SBLOCCATA_SEGNALA_RICHIESTE_SCADUTE_UD As Integer = 36
        Public Const CODICE_STATO_PRATICA_RICEVUTA_SAR As Integer = 37
        Public Const CODICE_STATO_PRATICA_ANTICIPO_CONCESSO_IN_ATTESA_INTERVENTO As Integer = 33
        Public Const CODICE_STATO_PRATICA_ANTICIPO_NON_CONCESSO As Integer = 34
        Public Const CODICE_STATO_PRATICA_ANNULLAMENTO_RIVISITAZIONE As Integer = 43
        Public Const CODICE_STATO_PRATICA_SOSPESA_MOBILITA As Integer = 44
        Public Const CODICE_STATO_PRATICA_MOVIMENTO_SU_NOTA As Integer = 45


        ' ---- TIPI STATI PRATICA --
        Public Const CODICE_TIPO_STATO_PRATICA_NON_LIQUIDABILE As Integer = 3

        ' ---- DOCUMENTI INTEGRABILI  --
        Public Const CODICE_DOC_AUTOCERTIFICAZIONE As Integer = 6
        Public Const CODICE_DOC_CONSENSO_DATI As Integer = 14
        Public Const CODICE_DOC_COORDINATE_BANCARIE As Integer = 16
        Public Const CODICE_DOC_PROCURA_INCASSO_ISTRUTTORIA As Integer = 21
        Public Const CODICE_DOC_MODULO_R01 As Integer = 4
        Public Const CODICE_DOC_MODULO_RESP_CONSENSO_SEGR_PROF As Integer = 34
        Public Const CODICE_DOC_COORDINATE_BANCARIE_EX_ASSEGNO As Integer = 41

        ' ---- STATI ISCRITTO ---
        Public Const CODICE_STATO_ISCRITTO_ATTIVO As Integer = 1
        Public Const CODICE_STATO_ISCRITTO_CESSATO As Integer = 2
        Public Const CODICE_STATO_ISCRITTO_SOSPESO As Integer = 3
        Public Const CODICE_STATO_ISCRITTO_SUPERSTITE As Integer = 4
        ' -----------------------

        ' ---- CODICI COPERTURE ---
        Public Const CODICE_COPERTURA_LAVORATORE As Integer = 1
        Public Const CODICE_COPERTURA_LAVORATORE_CONIUGE As Integer = 2
        Public Const CODICE_COPERTURA_LAVORATORE_NUCLEO As Integer = 3
        Public Const CODICE_COPERTURA_SUPERSTITE As Integer = 4
        Public Const CODICE_COPERTURA_SUPERSTITE_SENZANUCLEO As Integer = 12
        Public Const CODICE_COPERTURA_SUPERSTITE_CONNUCLEO As Integer = 13
        Public Const CODICE_COPERTURA_SOSPESO_CONCARICO_SENZANUCLEO As Integer = 5
        Public Const CODICE_COPERTURA_SOSPESO_SENZACARICO As Integer = 6
        Public Const CODICE_COPERTURA_SOSPESO_CONCARICO_CONNUCLEO As Integer = 7
        Public Const CODICE_COPERTURA_LAVORATORE_NUCLEO_MONOCOMPOSTO As Integer = 8
        Public Const CODICE_COPERTURA_LAVORATORE_NUCLEO_PLURICOMPOSTO As Integer = 9
        Public Const CODICE_COPERTURA_SOSPESO_CONCARICO_CONNUCLEOMONOCOMPOSTO As Integer = 10
        Public Const CODICE_COPERTURA_SOSPESO_CONCARICO_CONNUCLEOPLURICOMPOSTO As Integer = 11
        Public Const CODICE_COPERTURA_SOSPESO_CONCARICO As Integer = 50
        ' -----------------------

        ' ------ MODALITA PAGAMENTO -----------------------
        Public Const MODALITA_PAGAMENTO_RID As Integer = 1
        Public Const MODALITA_PAGAMENTO_MAV As Integer = 2
        Public Const MODALITA_PAGAMENTO_BONIFICO As Integer = 3
        Public Const MODALITA_PAGAMENTO_ASSEGNO As Integer = 4
        Public Const MODALITA_PAGAMENTO_FRECCIA As Integer = 5
        ' -------------------------------------------------

        ' ------ CESSAZIONE -----------------------
        Public Const CESSAZIONE_RINUNCIA As Integer = 1
        Public Const CESSAZIONE_RAPPORTO_LAVORO As Integer = 2
        Public Const CESSAZIONE_PASSAGGIO_DIRIGENTE As Integer = 3
        Public Const CESSAZIONE_DECESSO As Integer = 4
        Public Const CESSAZIONE_SUPERAMENTO_ETA As Integer = 5
        Public Const CESSAZIONE_ISCRIZIONE_FONDO As Integer = 6
        Public Const CESSAZIONE_MANCANZA_REQUISITI As Integer = 7
        Public Const CESSAZIONE_ASPETTATIVA As Integer = 8
        Public Const CESSAZIONE_NO_FISCALMENTE_CARICO As Integer = 9
        Public Const CESSAZIONE_PASSAGGIO_IMPRESA As Integer = 10
        Public Const CESSAZIONE_IMPRESA_MOROSA As Integer = 11
        Public Const CESSAZIONE_CONIUGE_ISCRITTO As Integer = 12
        Public Const CESSAZIONE_RAPP_LAV_DOPO_RINUNCIA As Integer = 13
        Public Const CESSAZIONE_65_ANNI_ISCRITTO_DECEDUTO As Integer = 15
        Public Const CESSAZIONE_CEDOLA_NO_NORMA_TRANSITORIA As Integer = 17
        Public Const CESSAZIONE_SILENTE_NORMA_TRANSITORIA As Integer = 18
        Public Const CESSAZIONE_FINE_ACCORDO_MOBILITA As Integer = 19

        ' ------------------------------------------------

        ' ------ SOSPENSIONE -----------------------
        Public Const SOSPENSIONE_ASPETTATIVA As Integer = 1
        Public Const SOSPENSIONE_PERMESSO As Integer = 2
        Public Const SOSPENSIONE_MALATTIA As Integer = 3
        Public Const SOSPENSIONE_MATERNITA As Integer = 4
        Public Const SOSPENSIONE_CASSA_INTEGRAZIONE As Integer = 5
        Public Const SOSPENSIONE_DISTACCO_ESTERO As Integer = 6
        Public Const SOSPENSIONE_SUPERAMENTO_LIMITI As Integer = 7
        ' ------------------------------------------------

        ' ------ SETTORI  -----------------------
        Public Const SETTORE_CHIMICO As Integer = 1
        Public Const SETTORE_CHIMICO_FARMACEUTICO As Integer = 2
        Public Const SETTORE_CERAMICA_ABRASIVI As Integer = 3
        Public Const SETTORE_LUBRIFICANTI_GPL As Integer = 4
        Public Const SETTORE_FIBRE As Integer = 5
        Public Const SETTORE_ORGANIZZAZIONE_SINDACALE As Integer = 6
        Public Const SETTORE_MINERARIO As Integer = 7
        Public Const SETTORE_COIBENTI As Integer = 8
        Public Const SETTORE_CERAMICA As Integer = 9
        Public Const SETTORE_ABRASIVI As Integer = 10

        ' ------ OGGETTI ------------------------------------------
        Public Const OGGETTO_RICHIESTA_DI_RIMBORSO As Integer = 11
        Public Const OGGETTO_RICHIESTA_DI_RIMBORSO_BUSTA As Integer = 133
        Public Const OGGETTO_INTEGRAZIONE_BUSTA As Integer = 134
        Public Const OGGETTO_RIMBORSO_PAGAMENTO As Integer = 15
        Public Const OGGETTO_LETTERA_UD As Integer = 16
        Public Const OGGETTO_INTEGRAZIONE As Integer = 20
        Public Const OGGETTO_COORDINATE_BANCARIE As Integer = 22
        Public Const OGGETTO_CARTELLA_CLINICA As Integer = 28
        Public Const OGGETTO_MODULO_RIMBORSI_R01 As Integer = 32
        Public Const OGGETTO_PRIMO_SOLLECITO_POSTA As Integer = 40
        Public Const OGGETTO_SECONDO_SOLLECITO As Integer = 41
        Public Const OGGETTO_PROCURA_INCASSO As Integer = 82
        Public Const EVASIONE_REGRESSO_UD As Integer = 99
        Public Const OGGETTO_PRIMO_SOLLECITO As Integer = 101
        Public Const OGGETTO_POST_ISCRIZIONE_NUCLEO As Integer = 103
        Public Const OGGETTO_FINE_GRATUITA_NUCLEO As Integer = 120
        Public Const OGGETTO_FINE_GRATUITA_NUCLEO_IMPRESA As Integer = 137
        Public Const OGGETTO_LIMITI_ETA As Integer = 98
        Public Const OGGETTO_RISPOSTA_REVISIONE As Integer = 102

        Public Const OGGETTO_MODULO_RIMBORSI_R01_INTEGRAZIONE As Integer = 121
        Public Const OGGETTO_DIRETTA_RIMBORSO_STRUTTURA As Integer = 123
        Public Const OGGETTO_ACQUISIZIONE_LASTRE As Integer = 125
        Public Const OGGETTO_MODULO_CONFERMA_SI As Integer = 126
        Public Const OGGETTO_MODULO_CONFERMA_NO As Integer = 127
        Public Const OGGETTO_ALTRI_MODULI As Integer = 129
        Public Const OGGETTO_ALTRI_MODULI_NO As Integer = 130

        Public Const OGGETTO_SOSPESO As Integer = 21
        Public Const OGGETTO_SCADENZAUD As Integer = 92
        Public Const OGGETTO_RESPINTAUD As Integer = 96
        Public Const OGGETTO_ANTICIPO_NON_CONCESSO As Integer = 27
        Public Const OGGETTO_ANTICIPO_CONCESSO As Integer = 26
        Public Const OGGETTO_FATTURE_IN_COPIA As Integer = 67
        Public Const OGGETTO_FAM_NON_ISCRITTO As Integer = 24

        Public Const OGGETTO_CESSIONE_ISCRITTO As Integer = 147
        Public Const OGGETTO_ACQUISIZIONE_ISCRITTO As Integer = 148

        Public Const OGGETTO_WELCOME_KIT As Integer = 78
        Public Const OGGETTO_RINVIO_KIT As Integer = 88
        Public Const OGGETTO_RINVIO_CARD As Integer = 116
        Public Const OGGETTO_RINVIO_MAT_INFO As Integer = 146
        Public Const OGGETTO_TERZO_RINVIO_KIT As Integer = 151

        ' ---------------------------------------------------------

        ' ------ TIPI PROTOCOLLI ------------------------------------------
        Public Const COD_TIPOPROTOCOLLO_PRESTAZIONI As Integer = 2
        Public Const COD_TIPOPROTOCOLLO_CESSIONI As Integer = 8
        Public Const COD_TIPOPROTOCOLLO_ACQUISIZIONI As Integer = 9

        ' ------ DIREZIONI PROTOCOLLI ------------------------------------------
        Public Const COD_DIREZIONE_INGRESSO As Integer = 1
        Public Const COD_DIREZIONE_USCITA As Integer = 2

        ' ------ TIPI DOCUMENTI INTEGRABILI ----------
        Public Const TIPO_DOCUMENTO_ISTRUTTORIA As Integer = 1
        Public Const TIPO_DOCUMENTO_LAVORAZIONE As Integer = 2
        Public Const TIPO_DOCUMENTO_ISTR_LAV As Integer = 3
        ' --------------------------------------------

        ' ------ GESTIONE MOROSITA' ------------------------
        Public Const TOLLERANZA_PERCENTUALE_1 As Decimal = 5
        Public Const TOLLERANZA_PERCENTUALE_2 As Decimal = 2
        Public Const TOLLERANZA_IMPORTO As Decimal = 20000
        Public Const TOLLERANZA_MOROSITA_1 As Decimal = 1
        Public Const TOLLERANZA_MOROSITA_2 As Decimal = 3
        ' --------------------------------------------------

        ' --- UNITA' DI LAVORO ---
        ' l'UNITA_DI_LAVORO_ENABLED è da eliminare appena va in esercizio il workflow
        'Public Const UNITA_DI_LAVORO_ENABLED As Boolean = False ' se TRUE abilita la gestione dei pacchi
        Public Const UNITA_DI_LAVORO_STATO_APERTO As Integer = 1
        Public Const UNITA_DI_LAVORO_STATO_PRONTO As Integer = 2
        Public Const UNITA_DI_LAVORO_STATO_IN_LAVORAZIONE As Integer = 3
        Public Const UNITA_DI_LAVORO_STATO_CHIUSO As Integer = 4
        Public Const UNITA_DI_LAVORO_MAX_ELEMENTS As Integer = -1 ' numero massimo di pratiche per pacco


        ' --- CODICI TIPI PRESTAZIONI ----------------------------------
        Public CODICE_TIPO_PRESTAZIONE_RICOVERO As Integer = 6
        Public CODICE_TIPO_PRESTAZIONE_ODONTOIATRIA As Integer = 8
        Public CODICE_TIPO_PRESTAZIONE_NON_RIMBORSABILE As Integer = 11
        Public CODICE_TIPO_PRESTAZIONE_SPESE_ASSEGNO As Integer = 12
        Public CODICE_TIPO_PRESTAZIONE_PRIVATE As Integer = 13
        ' ---------------------------------------------------------

        ' --- CODICI PRESTAZIONI ----------------------------------
        Public CODICE_PRESTAZIONE_STORNO_ASSEGNO As Integer = 10455
        ' ---------------------------------------------------------

        ' --- UNA TANTUM ----------------------------
        Public CODICE_UNA_TANTUM As Integer = 1
        Public STATO_UNA_TANTUM_APERTO As Integer = 1
        Public IMPORTO_UNA_TANTUM As Decimal = 182.75
        ' -------------------------------------------

        ' --- PROVENIENZA ---------------------------
        Public COD_PROVENIENZA_SITO As Integer = 1
        Public COD_PROVENIENZA_ASI2002 As Integer = 2
        ' -------------------------------------------

        ' --- SOLLECITI -----------------------------
        Public COD_PRIMO_SOLLECITO As Integer = 1
        Public COD_SECONDO_SOLLECITO As Integer = 2
        ' -------------------------------------------

        ' --- INNALZAMENTO LIMITI--------------------
        Public LIMITE_ETA_FIGLI As Integer = 26
        Public COD_LIMITE_DISABILITA_TOTALE As Integer = 4
        ' -------------------------------------------

        ' --- TIPI REINVIO KIT ----------------------
        Public Const INVIO_KIT_STANDARD As Integer = 1
        Public Const REINVIO_SOLO_CARD As Integer = 15
        Public Const REINVIO_MATERIALE_INFORMATIVO As Integer = 20
        Public Const REINVIO_KIT_STANDARD As Integer = 47
        Public Const TERZO_REINVIO_KIT As Integer = 48
        ' -------------------------------------------

        ' --- MOTIVI LIQUIDAZIONE -------------------
        Public Const COD_MOTIVO_NON_IN_COPERTURA As Integer = 8
        ' -------------------------------------------
        ' --- TIPI FORMA ASSISTENZA------------------
        Public Const COD_FORMA_ASSISTENZA_INDIRETTA As Integer = 1
        Public Const COD_FORMA_ASSISTENZA_DIRETTA As Integer = 2
        ' -------------------------------------------

        ' --- MOTIVI DI NON ASSISTIBILITA -----------
        Public Const COD_MOTIVO_CARENZA_INSUFFICIENTE As Integer = 1
        Public Const COD_MOTIVO_MOROSITA As Integer = 2
        Public Const COD_MOTIVO_DATA_NON_COPERTA As Integer = 3
        Public Const COD_MOTIVO_SOSPENSIONE_ISCRITTO As Integer = 4
        Public Const COD_MOTIVO_SOSPENSIONE_FAMILIARE As Integer = 5
        Public Const COD_MOTIVO_FIGLIO_NON_FISCALMENTE_A_CARICO As Integer = 6
        ' -------------------------------------------

        ' --- RITENUTA D'ACCONTO -----------
        Public Const RITENUTA_ACCONTO_ALIQUOTA As Decimal = 20.0

        Public NOTHING_DATE As Date = Nothing
        Public Const IMPORTO_TOLLERANZA_MOROSITA As Decimal = 71.01
        Public Const GIORNI_SCADENZA_LETTERE_UD As Integer = 97
        Public Const GIORNI_SPEDIZIONE_UD As Integer = 90 'giorni entro i quali spedire il materiale richiesto dalla UD. oltre questo si rischia di eccedere GIORNI_SCADENZA_LETTERE_UD
        Public Const DATA_RIFERIMENTO_SCADENZA As String = "24-10-2005" 'gg-mm-aaaa
        Public Const PROVINCIA_ESTERO As String = "EE"
        Public Const DATA_INIZIO_REGOLAMENTO_2007 As String = "01/01/2007"
        Public Const DATA_CIRCOLARE_ACCORDO As String = "18/02/2008"
        Public Const DATA_INIZIO_NORMA_TRANSITORIA_2008 As String = "01/07/2008"
        Public Const DATA_FINE_NORMA_TRANSITORIA_2008 As String = "30/06/2009"

        ' --- GENERAZIONE PDF -----------
        Public Const COD_DOMINIO_FASCHIM As Integer = 2


        Public Const COD_AZIONE_GENERAZIONE_SEMPLICE As Integer = 0
        Public Const COD_AZIONE_ACCORPA_MULTIFILE As Integer = 1
        Public Const COD_AZIONE_TEMPORANEO As Integer = 2

        Public Const COD_AZIONE_DETTAGLIO_CREA As Integer = 0
        Public Const COD_AZIONE_DETTAGLIO_SOLALETTURA As Integer = 1
        Public Const COD_AZIONE_DETTAGLIO_SOVRASCRIVI As Integer = 2
        Public Const COD_AZIONE_DETTAGLIO_CREASENONESISTE As Integer = 3

        Public Const COD_STATO_RICHIESTA_DA_PROCESSARE As Integer = 0

        Public Const COD_STAMPA_ASINCRONA_NON_STAMPARE As Integer = 0
        Public Const COD_STAMPA_ASINCRONA_STAMPANTE_DEFAULT As Integer = 1
        Public Const COD_STAMPA_ASINCRONA_STAMPANTE_RICHIESTA As Integer = 2
        Public Const COD_STAMPA_ASINCRONA_STAMPANTE_POSTEL As Integer = 3

        Public Const COD_STATO_STAMPA_NON_STAMPARE As Integer = 0
        Public Const COD_STATO_STAMPA_DA_STAMPARE As Integer = 1
        Public Const COD_STATO_STAMPA_STAMPATA As Integer = 2
        Public Const COD_STATO_STAMPA_RICHIESTA_RISTAMPA As Integer = 3

        Public Const COD_TIPO_NOTIFICA_NESSUNA_NOTIFICA As Integer = 0
        Public Const COD_TIPO_NOTIFICA_EMAIL_SPECIFICATA As Integer = 1
        Public Const COD_TIPO_NOTIFICA_EMAIL_SEZIONE As Integer = 2

        ' --- TIPI FLUSSO -----------
        Public Const COD_TIPO_FLUSSO_AVVISO_POST_ISCRIZIONE_NUCLEO As Integer = 13
        Public Const COD_TIPO_FLUSSO_INFORMATIVA_FINE_GRATUITA_NUCLEO As Integer = 17
        Public Const COD_TIPO_FLUSSO_INFORMATIVA_SOSPENSIONE As Integer = 44
        Public Const COD_TIPO_FLUSSO_SUPERMENTO_LIMITI As Integer = 12

        ' --- CODICI MOROSIA -------
        Public Const COD_TIPO_MOROSITA_IMPRESA_NON_MOROSA As String = "0"
        Public Const COD_TIPO_MOROSITA_IMPRESA_MOROSA_A As String = "1A"
        Public Const COD_TIPO_MOROSITA_IMPRESA_MOROSA_B As String = "1B"
        Public Const COD_TIPO_MOROSITA_ISCRITTO_MOROSO_A As String = "2A"
        Public Const COD_TIPO_MOROSITA_ISCRITTO_MOROSO_B As String = "2B"
        Public Const COD_TIPO_MOROSITA_IMPRESA_MOROSA_MANCATOVERSAMENTO As String = "3A"
        Public Const COD_TIPO_MOROSITA_ISCRITTO_MOROSO_MANCATOVERSAMENTO As String = "3I"
        Public Const COD_TIPO_MOROSITA_IMPRESA_NON_MOROSA_VERSAMENTOSUPERIORE As String = "4A"
        Public Const COD_TIPO_MOROSITA_ISCRITTO_NON_MOROSO_VERSAMENTOSUPERIORE As String = "4I"
        Public Const COD_TIPO_MOROSITA_ISCRITTO_PAGANTE_NON_MOROSO_VERSAMENTOSUPERIORE As String = "5I"
        Public Const COD_TIPO_MOROSITA_ISCRITTO_IN_MOBILITA_MOROSO As String = "6I"
        Public Const COD_TIPO_MOROSITA_ISCRITTO_NON_MOROSO_AZIENDA_MOROSA As String = "7I"

        Public Const COD_TIPO_MOROSITA_ERRORE_GENERICO As String = "-1"
        Public Const COD_TIPO_MOROSITA_ISCRITTO_IN_MOBILITA As String = "-2"

        '               -1  -> errore generico
        '               -2  -> Iscritto in Mobilità
        '               0   -> Impresa Non Morosa
        '               1A  -> Impresa Morosa (tipo A)
        '               1B  -> Impresa Morosa (tipo B)
        '               2A  -> Iscritto Moroso (tipo A)
        '               2B  -> Iscritto Moroso (tipo B)
        '               3A  -> Impresa Morosa (Mancato Versamento)
        '               3I  -> Iscritto Moroso (Mancato Versamento)
        '               4A  -> Impresa Non Morosa (Versamento Superiore)
        '               4I  -> Iscritto Non Moroso (Versamento Superiore)
        '               5I  -> Iscritto Pagante Non Moroso (Versamento Superiore)
        '               6I  -> Iscritto in Mobilità Moroso


        Public Structure SessionInfo
            Dim TipoSoggetto As String
            Dim UserName As String
            Dim CodiceUfficio As String
            Dim SessionNumber As String
            Dim IP As String
        End Structure

        Public CurrSessionInfo As SessionInfo

        Public Function formattaDataOra(ByVal dataOra As String) As String
            Dim dataOraFormattata As String
            Dim giorno As String
            Dim mese As String
            Dim anno As String
            Dim ora As String
            Dim sepData As String = "/"

            If dataOra.Trim = "" Then Return ""

            giorno = dataOra.Substring(8, 2)
            mese = dataOra.Substring(5, 2)
            anno = dataOra.Substring(0, 4)
            ora = dataOra.Substring(11, 8)

            dataOraFormattata = giorno & sepData & mese & sepData & anno & " " & ora

            Return dataOraFormattata
        End Function

        Public Function formattaData(ByVal data As Date) As String
            Return Format(data, "yyyy/MM/dd")
        End Function

        Public Function formattaData2(ByVal data As Date) As String
            Return Format(data, "yyyy-MM-dd")
        End Function

        Public Function formattaDataHMS(ByVal data As Date) As String
            Return Format(data, "yyyy/MM/dd HH:mm:ss")
        End Function

        Public Function stringaPerSql(ByVal sValue As String) As String

            If sValue.Trim = "" Then Return "null"
            Return "'" & sValue.Replace("'", "''").Trim & "'"

        End Function

        ''' <summary>
        ''' Come StringaPerSql ma senza trim e senza NULL automatico
        ''' </summary>
        ''' <param name="sValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function StringaSql(ByVal sValue As String) As String

            Return String.Concat("'", sValue.Replace("'", "''"), "'")

        End Function

        Public Function importiPerSql(ByVal dValue As Decimal) As String

            Return dValue.ToString.Replace(",", ".")

        End Function

        Public Function oraPerSql(ByVal value As String) As String

            If value.Trim = "" Then Return "null"
            Return "'" & value.ToString.Replace(".", ":") & "'"

        End Function

        Public Function LikePerSql(ByVal sValue As String, Optional ByVal sLato As String = "") As String
            Dim sRitorno As String = ""
            If sValue = "" Then
                Return "LIKE '%%'"
            Else
                Select Case sLato
                    Case ""
                        sRitorno = "LIKE '%" & sValue.Replace("'", "''") & "%'"
                    Case "D" 'Aperto a destra
                        sRitorno = "LIKE '" & sValue.Replace("'", "''") & "%'"
                    Case "S" 'Aperto a sinistra
                        sRitorno = "LIKE '%" & sValue.Replace("'", "''") & "'"
                End Select
            End If
            Return sRitorno
        End Function

        Public Function numeroPerSql(ByVal nValue As Integer) As String

            If nValue = 0 Then Return "null"
            Return nValue

        End Function

        Public Function dataPerSql(ByVal dValue As Date) As String
            If dValue = NOTHING_DATE Then Return "null"
            Return "'" & formattaData(dValue) & "'"
        End Function
        Public Function dataPerSql2(ByVal dValue As Date) As String
            If dValue = NOTHING_DATE Then Return "null"
            Return "'" & formattaData2(dValue) & "'"
        End Function

        Public Function dataOraPerSql(ByVal dValue As Date) As String
            If dValue = NOTHING_DATE Then Return "null"
            Return "'" & formattaDataHMS(dValue) & "'"
        End Function

        Public Function dataOraPerSqlServer(ByVal dValue As Date) As String
            If dValue = NOTHING_DATE Then Return "null"
            If dValue.Hour = 0 And dValue.Minute = 0 And dValue.Second = 0 Then
                Return "CONVERT(DATETIME,'" & dValue.ToString("yyyy-MM-dd") & "', 102)"
            Else
                Return "CONVERT(DATETIME,'" & dValue.ToString("yyyy-MM-dd H.mm.ss").Replace(".", ":") & "', 102)"
            End If
        End Function


        'Public Function controllaCIN(ByVal cin As String, ByVal abi As String, ByVal cab As String, ByVal numeroConto As String) As Integer

        '    Dim b As String
        '    Dim i As Integer
        '    Dim s As Integer
        '    Dim c As Integer
        '    Dim k As Integer
        '    Dim kcin As Integer
        '    Dim dispari() As Integer = {1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23}

        '    b = cin & abi & cab & numeroConto

        '    s = 0

        '    'controlla cin solo se valorizzati abi cab numero cc
        '    If abi.Trim = "" Or cab.Trim = "" Or numeroConto.Trim = "" Then
        '        Return 0
        '    End If

        '    'ciclo tra caratteri della stringa
        '    For i = 0 To b.Length - 1

        '        'estrae il prossimo carattere c
        '        c = Asc(b.Substring(i, 1))

        '        If 48 <= c And c <= 57 Then 'per cifra 0-9
        '            If i = 0 Then
        '                Return 74 'Il CIN non può contenere cifre
        '            End If
        '            k = c - 48
        '        ElseIf 65 <= c And c <= 90 Then 'per lettera A-Z
        '            If 1 <= i And i <= 10 Then
        '                Return 75 'ABI e CAB non possono contenere lettere
        '            End If
        '            k = c - 65
        '        Else
        '            Return 76 'Sono ammesse solo cifre e lettere maiuscole
        '        End If

        '        'Calcola la somma di controllo s e il codice di controllo CIN
        '        If i = 0 Then 'codice di controllo
        '            kcin = k
        '        ElseIf i Mod 2 = 0 Then 'posizione pari
        '            s = s + k
        '        Else 'posizione dispari
        '            s = s + dispari(k)
        '        End If
        '    Next

        '    If s Mod 26 <> kcin Then
        '        Return 1 'Il codice di controllo è errato
        '    End If

        '    Return 0

        'End Function

        Public Function aggiungiNodoXML(ByRef sDocXML As String, ByVal sNodeXML As String, ByVal sKeyNode As String, ByVal sKeyValue As String, ByVal nodeName As String, ByVal childName As String) As Boolean

            Dim docXML As New XmlDocument
            Dim fieldXML As New XmlDocument
            Dim nodeList As XmlNodeList
            Dim childList As XmlNodeList
            Dim newChildList As XmlNodeList
            Dim newNode As XmlNode
            Dim row As Integer
            Dim col As Integer
            Dim i As Integer
            Dim bFound As Boolean

            docXML.LoadXml(sDocXML)
            fieldXML.LoadXml(sNodeXML)
            nodeList = docXML.GetElementsByTagName(nodeName)

            For row = 0 To nodeList.Count - 1
                childList = nodeList.Item(row).ChildNodes
                For col = 0 To childList.Count - 1
                    If childList.Item(col).Name = sKeyNode And childList.Item(col).InnerText = sKeyValue Then
                        newChildList = fieldXML.GetElementsByTagName(childName)
                        For i = 0 To newChildList.Count - 1
                            newNode = docXML.ImportNode(newChildList.Item(i), True)
                            nodeList.Item(row).AppendChild(newNode)
                        Next
                        bFound = True
                        Exit For
                    End If
                Next
                If bFound Then Exit For
            Next

            sDocXML = docXML.InnerXml

        End Function

        Public Function appendiNodoXML(ByRef sDocXML As String, ByVal sNodeXML As String, ByVal nodeName As String, ByVal childName As String) As Boolean

            Dim docXML As New XmlDocument
            Dim fieldXML As New XmlDocument
            Dim nodeList As XmlNodeList
            Dim newChildList As XmlNodeList
            Dim newNode As XmlNode
            Dim i As Integer

            docXML.LoadXml(sDocXML)
            fieldXML.LoadXml(sNodeXML)
            nodeList = docXML.GetElementsByTagName(nodeName)

            newChildList = fieldXML.GetElementsByTagName(childName)
            For i = 0 To newChildList.Count - 1
                newNode = docXML.ImportNode(newChildList.Item(i), True)
                nodeList.Item(nodeList.Count - 1).AppendChild(newNode)
            Next

            sDocXML = docXML.InnerXml
        End Function

        Public Function appendiNodoCreato(ByRef sDocXML As String, ByVal nodeName As String, ByVal keyNodeName As String, ByVal keyNodeValue As String, ByVal childName As String, ByVal childValue As String) As Boolean
            'FUNZIONE CHE CREA UN NUOVO NODO DA AGGIUNGERE AL sDocXML DI INPUT
            Dim docXML As New XmlDocument
            Dim fieldXML As New XmlDocument
            Dim nodeList, childList As XmlNodeList
            Dim nodoChild As XmlElement
            Dim newChildList As XmlNodeList
            Dim newNode As XmlNode
            Dim row As Integer
            Dim col As Integer
            Dim bFound As Boolean

            docXML.LoadXml(sDocXML)
            nodeList = docXML.GetElementsByTagName(nodeName)
            'Creo un nuovo nodo --> Il TAG ha il nome = 'childName' passato da parametro e' di Tipo Element e non contiene attributi
            nodoChild = fieldXML.CreateNode(XmlNodeType.Element, childName, "")
            'Inserisco il valore al nuovo nodo 
            nodoChild.InnerText = childValue
            'Creo un File XML con il nuovo TAG senza una radice 
            fieldXML.AppendChild(nodoChild)
            'Appendo il nuovo documento xml all'originale in corrispondenza del codice con il valore specificato
            For row = 0 To nodeList.Count - 1
                childList = nodeList.Item(row).ChildNodes
                For col = 0 To childList.Count - 1
                    If childList.Item(col).Name = keyNodeName And childList.Item(col).InnerText = keyNodeValue Then
                        newChildList = fieldXML.GetElementsByTagName(childName)
                        newNode = docXML.ImportNode(newChildList.Item(0), True)
                        nodeList.Item(row).AppendChild(newNode)
                        bFound = False
                        Exit For
                    End If
                Next
                If bFound Then Exit For
            Next
            sDocXML = docXML.InnerXml
            bFound = True
        End Function

        Public Function aggiungiNodoXMLRefObj(ByRef masterXML As String, ByVal refObjXML As String, ByVal keyNodeName As String, ByVal keyValue As String, ByVal masterNodeName As String, ByVal refObjNodeName As String) As Boolean

            Dim docXML As New XmlDocument
            Dim fieldXML As New XmlDocument
            Dim nodeList As XmlNodeList
            Dim childList As XmlNodeList
            Dim newChildList As XmlNodeList
            Dim newNode As XmlNode
            Dim row As Integer
            Dim col As Integer
            Dim bFound As Boolean

            docXML.LoadXml(masterXML)
            fieldXML.LoadXml(refObjXML)
            nodeList = docXML.GetElementsByTagName(masterNodeName)

            For row = 0 To nodeList.Count - 1
                childList = nodeList.Item(row).ChildNodes
                For col = 0 To childList.Count - 1
                    If childList.Item(col).Name = keyNodeName And childList.Item(col).InnerText = keyValue Then
                        'childList.Item(col).InnerText = ""
                        nodeList.Item(row).RemoveChild(childList.Item(col))
                        newChildList = fieldXML.GetElementsByTagName(refObjNodeName)
                        newNode = docXML.ImportNode(newChildList.Item(0), True)
                        'childList.Item(col).AppendChild(newNode)
                        nodeList.Item(row).AppendChild(newNode)
                        bFound = True
                        Exit For
                    End If
                Next
                If bFound Then Exit For
            Next

            masterXML = docXML.InnerXml

        End Function

        Function inviaMail(ByVal sMessaggio As String, ByVal sMailTo As String, ByVal sSubj As String, Optional ByVal pathAllegati As String = "", Optional ByVal customHeaders As Specialized.NameValueCollection = Nothing, Optional ByVal sMailCC As String = "", Optional ByVal sMailCCN As String = "", Optional ByVal sSign As String = "", Optional ByVal sFileSign As String = "") As Boolean

            Try
                Dim smtpClient As New System.Net.Mail.SmtpClient

                'Configurazione client SMTP
                smtpClient.Host = ConfigurationManager.AppSettings("mailSmtp")
                smtpClient.Port = ConfigurationManager.AppSettings("smtpserverport")

                If ConfigurationManager.AppSettings("usaAutenticazioneSmtp") = "1" Then
                    'Autenticazione
                    smtpClient.UseDefaultCredentials = False
                    smtpClient.Credentials = New System.Net.NetworkCredential(ConfigurationManager.AppSettings("sendusername"), ConfigurationManager.AppSettings("sendpassword"))
                End If

                'Impostazione mail
                Using mail As New MailMessage

                    mail.From = New System.Net.Mail.MailAddress(ConfigurationManager.AppSettings("mailFrom"))

                    'Destinatari To
                    For Each indirizzo As String In sMailTo.Split(New Char() {";"}, StringSplitOptions.RemoveEmptyEntries)
                        mail.To.Add(indirizzo.ToLower())
                    Next

                    'Destinatari CC
                    For Each indirizzoCC As String In sMailCC.Split(New Char() {";"}, StringSplitOptions.RemoveEmptyEntries)
                        mail.CC.Add(indirizzoCC.ToLower())
                    Next

                    'Destinatari CCN
                    For Each indirizzoCCN As String In sMailCCN.Split(New Char() {";"}, StringSplitOptions.RemoveEmptyEntries)
                        mail.Bcc.Add(indirizzoCCN.ToLower())
                    Next

                    mail.Subject = sSubj
                    mail.IsBodyHtml = True
                    mail.Priority = System.Net.Mail.MailPriority.High

                    'Imposta Body
                    Dim bodyBuild As New System.Text.StringBuilder()
                    bodyBuild.Append("<html><body style='font-family:Arial;font-size:10pt'>")
                    bodyBuild.Append(Date.Now.ToString("dd/MM/yyyy"))
                    bodyBuild.Append("<br><br>")
                    bodyBuild.Append(sMessaggio)

                    If sSign.Trim <> "" Then
                        sSign = "<br><br><br><cite>" & sSign & "</cite><br>" & _
                                "<img border='0' src='" & sFileSign & "'>" & _
                                "</p><p>&nbsp;</p>"
                        bodyBuild.Append(sSign)
                    End If

                    bodyBuild.Append("</body></html>")
                    mail.Body = bodyBuild.ToString()

                    'Aggiunge Header Personalizzati se presenti
                    If customHeaders IsNot Nothing Then
                        mail.Headers.Add(customHeaders)
                    End If

                    'Creazione allegati
                    For Each sFile As String In pathAllegati.Trim().Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                        mail.Attachments.Add(New System.Net.Mail.Attachment(sFile))
                    Next

                    'Invio mail
                    smtpClient.Send(mail)

                End Using

                Return True

            Catch ex As Exception

                Return False

            End Try

        End Function
        ''' <summary>
        ''' La funzione blocca l'invio delle e-mail per il numero di secondi specificato nel file di configurazione
        ''' </summary>
        ''' <param name="numRecIn"></param>
        ''' <remarks></remarks>
        Public Sub bloccoEmail(ByVal numRecIn As Integer)

            If ((numRecIn Mod ConfigurationManager.AppSettings("numBloccoEmail")) = 0) Then
                Dim secondiResidui As Integer = ConfigurationManager.AppSettings("secAttesaBlocco")
                While secondiResidui > 0
                    System.Threading.Thread.Sleep(1000)
                    secondiResidui -= 1
                End While
            End If

        End Sub



        Function inviaFax(ByVal sMessaggio As String, ByVal sNumberTo As String, Optional ByVal pathAllegati As String = "") As Boolean

            Dim mail As New MailMessage
            Dim arrayAllegati() As String
            Dim count As Integer
            Dim stmpClient As New SmtpClient

            Try

                mail.From = New MailAddress(ConfigurationManager.AppSettings("mailFaxFrom"))
                mail.To.Add(sNumberTo & "@fax.fax")
                mail.Subject = ConfigurationManager.AppSettings("mailFaxSubject")
                mail.IsBodyHtml = True
                mail.Body = "<html><body>" & sMessaggio & "</body></html>"
                mail.Priority = MailPriority.High

                If pathAllegati.Trim <> "" Then
                    arrayAllegati = pathAllegati.Split(",")

                    For count = 0 To arrayAllegati.Length - 1
                        mail.Attachments.Add(New Attachment(arrayAllegati(count)))
                    Next

                End If

                stmpClient.Host = ConfigurationManager.AppSettings("mailFaxSmtp")


                stmpClient.Send(mail)

                Return True

            Catch ex As Exception

                Return False

            End Try

        End Function

        Public Sub scriviLogWS(ByVal logItem As String, Optional ByVal fileName As String = "")
            Dim fileLogWS As String
            Dim sw As StreamWriter = Nothing

            Try

                If fileName.Trim <> "" Then
                    fileLogWS = ConfigurationManager.AppSettings("fileLogDir") & fileName
                Else
                    fileLogWS = ConfigurationManager.AppSettings("fileLogDir") & ConfigurationManager.AppSettings("fileLogWS")
                End If

                sw = New StreamWriter(fileLogWS, True)
                sw.WriteLine(Now & " -> " & logItem)
            Catch ex As Exception

            Finally
                If sw IsNot Nothing Then
                    sw.Close()
                End If
            End Try
        End Sub

        Public Sub scriviLogWS_noDate(ByVal logItem As String, Optional ByVal fileName As String = "")
            Dim fileLogWS As String
            Dim sw As StreamWriter = Nothing

            Try

                If fileName.Trim <> "" Then
                    fileLogWS = ConfigurationManager.AppSettings("fileLogDir") & fileName
                Else
                    fileLogWS = ConfigurationManager.AppSettings("fileLogDir") & ConfigurationManager.AppSettings("fileLogWS")
                End If

                sw = New StreamWriter(fileLogWS, True)
                sw.WriteLine(logItem)
            Catch ex As Exception

            Finally
                If sw IsNot Nothing Then
                    sw.Close()
                End If
            End Try
        End Sub


        Public Sub calcolaAnnoTrimestre(ByVal data As Date, ByRef anno As Integer, ByRef trimestre As Integer)
            Dim mese As Integer

            If data = NOTHING_DATE Then
                anno = 0
                trimestre = 0
                Exit Sub
            End If

            '******** MODIFICATO IL 29/03/2005 PER GESTIRE CASI COME IL SEGUENTE: ****
            '******** DATA ISCRIZIONE DA 1/04/2004 A 1/07/2004                    ****
            data = data.AddMonths(-9)
            If DateTime.Compare(data, New Date(2004, 4, 1)) < 0 Then
                data = New Date(2004, 4, 1)
            End If
            '*************************************************************************

            'calcola anno
            anno = data.Year

            'calcola trimestre
            mese = data.Month
            If mese >= 1 And mese <= 3 Then
                trimestre = 1
            ElseIf mese >= 4 And mese <= 6 Then
                trimestre = 2
            ElseIf mese >= 7 And mese <= 9 Then
                trimestre = 3
            ElseIf mese >= 10 And mese <= 12 Then
                trimestre = 4
            End If
        End Sub

     

        Public Function getDataCessazione(ByVal dataIn As Date) As Date
            Dim d As Date

            dataIn = dataIn.AddMonths(1) 'mese successivo di dataIn

            d = New Date(dataIn.Year, dataIn.Month, 1).AddDays(-1) 'ultimo giorno del mese di dataIn

            Return d
        End Function

        Public Function getDataCessazioneRinuncia(ByVal dataIn As Date) As Date
            Dim d As Date

            d = New Date(dataIn.Year, 12, 31)

            Return d
        End Function

        Public Function getDataIscrizione(ByVal dataIn As Date) As Date
            Dim d As Date

            d = New Date(dataIn.Year, dataIn.Month, 1)

            Return d
        End Function

        Public Function getDataInizioRapporto(ByVal dataInizioRapportoIn As Date) As Date
            ' Restituisce sempre la data di Inizio Rapporto al primo giorno del mese

            Return New Date(dataInizioRapportoIn.Year, dataInizioRapportoIn.Month, 1)

        End Function

        Public Function getDataFineRapporto(ByVal dataFineRapportoIn As Date) As Date
            'Restituisce sempre la data di fine Rapporto all'ultimo giorno del mese di dataFineRapportoIn

            If dataFineRapportoIn = NOTHING_DATE Then
                Return dataFineRapportoIn
            Else
                dataFineRapportoIn = dataFineRapportoIn.AddMonths(1) 'mese successivo di dataFineRapportoIn
                Return New Date(dataFineRapportoIn.Year, dataFineRapportoIn.Month, 1).AddDays(-1)
            End If

        End Function

        Function trasformFileXML(ByVal sXML As String, ByVal fileXSL As String, ByRef sHTML As String) As Boolean

            Try
                ' Load the XML document to transform.
                Dim sReader As New IO.StringReader(sXML)
                Dim xReader As New XmlTextReader(sReader)
                Dim stWrite As New System.IO.StringWriter

                Try
                    ' Load the stylesheet and perform the transform.
                    Dim xslt As New Xsl.XslCompiledTransform
                    xslt.Load(fileXSL)

                    xslt.Transform(xReader, Nothing, stWrite)
                    sHTML = stWrite.ToString
                    Return True

                Finally
                    stWrite.Close()
                    xReader.Close()
                End Try

            Catch ex As Exception
                Return False

            End Try

        End Function

        Public Function CheckSoggettoPagante(ByVal SoggettoPagante As Char) As String

            Dim soggPag As Char = ""
            Dim retValue As String = ""

            If SoggettoPagante.Equals(SOGGETTO_PAGANTE_AZIENDA) Then
                soggPag = SOGGETTO_PAGANTE_AZIENDA
            ElseIf SoggettoPagante.Equals(SOGGETTO_PAGANTE_ISCRITTO) Then
                soggPag = SOGGETTO_PAGANTE_ISCRITTO
            End If

            If soggPag <> Nothing Then
                retValue = " AND SoggettoPagante='" & soggPag & "'"
            End If

            Return retValue
        End Function

        Function LeggiImpostazioni(ByVal db As DataBase, ByVal Chiave As String) As String
            Dim xml As String
            Dim rValue As Long

            xml = ""
            Try
                With db
                    .SQL = "SELECT Valore FROM Impostazioni " & _
                     " WHERE Chiave = " & stringaPerSql(Chiave)

                    rValue = .OpenQuery()
                    If rValue > 0 Then
                        Dim xmlDoc As New XmlDocument
                        Dim NodoTable As XmlNodeList

                        xml = .strXML
                        xmlDoc.LoadXml(xml)

                        NodoTable = xmlDoc.GetElementsByTagName("Table")
                        Return NodoTable.Item(0).ChildNodes.Item(0).InnerText
                    Else
                        Return ""
                    End If
                End With
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Function ScriviImpostazioni(ByVal db As DataBase, ByVal Chiave As String, ByVal Valore As String) As Boolean
            db.SQL = "UPDATE Impostazioni Set Valore = " & stringaPerSql(Valore) & _
              " WHERE Chiave = " & stringaPerSql(Chiave)

            If db.ExecSQL() < 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Function WriteBinaryString(ByVal FilePath As String, ByVal FileName As String, ByVal BinaryString As String) As Boolean
            If BinaryString.Trim().Length() = 0 Or FileName.Trim().Length() = 0 Then Return False

            FileName = FileName.Replace(" ", "_")

            Dim outputWrite As String
            Dim BinaryData() As Byte
            Dim outFile As System.IO.FileStream
            BinaryData = System.Convert.FromBase64String(BinaryString)

            outputWrite = FilePath & FileName
            Try
                outFile = New System.IO.FileStream(outputWrite, System.IO.FileMode.Create, System.IO.FileAccess.Write)
                outFile.Write(BinaryData, 0, BinaryData.Length - 1)
                outFile.Close()
                Return True
            Catch exp As Exception
                System.Console.WriteLine("{0}", exp.Message)
                Return False
            End Try

        End Function

        Public Function calcPathFromProtocol(ByVal numeroProtocollo As String) As String
            'return substr( $numeroProtocollo, 0, 4)."/".substr( $numeroProtocollo, 4, 2)."/".substr( $numeroProtocollo, 6, 2)."/";
            Return (numeroProtocollo.Substring(0, 4) & "/" & numeroProtocollo.Substring(4, 2) & "/" & numeroProtocollo.Substring(6, 2) & "/")
        End Function

        Public Function StringToBytes(ByVal str As String) As Byte()
            Dim i As Long
            Dim TempBytes() As Byte = Nothing
            For i = 0 To str.Length - 1
                ReDim Preserve TempBytes(i)
                TempBytes(i) = Asc(Mid(str, i + 1, 1))
            Next

            Return TempBytes

        End Function

        'ritorna stringa univoca
        Public Function calcolaTempFileName() As String
            Return "FASCHIM" & DateTime.Now.ToString("yyyyMMddhhmmssFFF")
        End Function

        Public Function checkCodiceFiscaleNoComuni(ByVal codice_c_f As String, _
                                              ByVal cognome As String, _
                                              ByVal nome As String, _
                                              ByVal anno As String, _
                                              ByVal mese As String, _
                                              ByVal giorno As String, _
                                              ByVal sesso As String) As Integer

            '*******************************************************************************
            '*                                                                             *
            '*                         Descrizione del algoritmo                           *
            '*                                                                             *
            '*******************************************************************************
            '*                                                                             *
            '* Il codice fiscale e formato dalle parti: cognome di 3 bytes, nome di        *
            '* 3 bytes, data di nascita e sesso di 5 bytes, comune di nascita di 4 bytes,  *
            '* check digit di 1 byte.                                                      *
            '* La parte "cognome" e determinata dalle prime tre consonanti del cognome,    *
            '* o dalle prime due consonanti  e dalla prima vocale, o dalla prima consonan- *
            '* te e dalle prime due vocali, o dalla prima consonante, dalla prima vocale   *
            '* e da "x".                                                                   *
            '* La parte "nome" e determinata dalla prima, dalle terza e dalle quarta con-  *
            '* sonante del nome, o dalle prime tre consonanti, o dalle prime due consonan- *
            '* ti  e dalla prima vocale, o dalla prima consonante e dalle prime due        *
            '* vocali, o dalla prima consonante, dalla prima vocale e da "x".              *
            '* La parte "data di nascita e sesso" e determinata dall'anno di nascita       *
            '* senza millesimo che deve essere maggiore di  zero, dalla decodifica del     *
            '* mese che deve essere compreso tra 1 e 12,  e  dal giorno che deve essere    *
            '* maggiore di zero. Solo per le donne viene sommato 40 al giorno di nascita.  *
            '* La  parte "comune di nascita"  e determinata dalla decodifica del  comune.  *
            '*                                                                             *
            '* Per calcolare il check digit si utilizzano due tabelle che associano un va- *
            '* lore numerico ai caratteri, una per i caratteri in posizione pari e una per *
            '* i caratteri in posizione dispari.                                           *
            '* Si sommano tutti i valori trovati e si divide per 26. AI risultato cosi     *
            '* ottenuto viene sommato 1. La lettera dell'alfabeto inglese corrispondente   *
            '* e' il check digit.                                                          *
            '*                                                                             *
            '*******************************************************************************
            '* ESEMPIO :      PNS MNC 67C45 H501                                           *
            '*             P   S    N    6    C   5    5    1                              *
            '*      PARI = 3 + 12 + 20 + 15 + 5 + 13 + 13 + 0 = 81                         *
            '*             N    M    C   7   4   H   0                                     *
            '*   DISPARI = 13 + 12 + 2 + 7 + 4 + 7 + 0 = 45                                *
            '*                                                                             *
            '*   N = 81+45 = 126                                                           *
            '*                                                                             *
            '*   126 = 26*4 + 22  = 22   W e' check                                        *
            '*                                                                             *
            '*******************************************************************************

            '*******************************************************************************
            '*                           CODICI DEGLI ERRORI                               *
            '*******************************************************************************
            '*              -1  - errore GENERICO nella compilazione dei dati              *
            '*                    (numero caratteri<>16)                                   *
            '*              -2  - CHECK DIGIT non corrisponde                              *
            '*              -3  - parte COGNOME non corrisponde                            *
            '*              -4  - parte NOME non corrisponde                               *
            '*              -5  - parte DATA NASCITA E SESSO non corrisponde               *
            '*         !!!!!-6  - parte PROVINCIA DI NASCITA non corrisponde               *
            '*                    (la sigla della provincia passata come parametro non     *
            '*                     corrisponde al codice del comune risultante dal         *
            '*                     codice fiscale)                                         *
            '*         !!!!!-7  - parte COMUNE DI NASCITA non corrisponde                  *
            '*                    (il comune di nascita passato come parametro non         *
            '*                    corrisponde al codice del comune risultante dal codice   *
            '*                    fiscale()                                                *
            '*         !!!!!-10 - il codice fiscale passato come argomento contiene un     *
            '*                    codice comune non esistente nel database                 *
            '*******************************************************************************

            Dim c_f(15) As String
            Dim errore As Integer
            Dim pari As Integer
            Dim dispari As Integer
            Dim risultato As Integer
            Dim codice_cognome As String
            Dim sommacognome As Integer
            Dim i As Integer, j As Integer
            codice_c_f = LCase(codice_c_f)

            REM si devono eliminari gli apici e gli spazi
            cognome = Replace(LCase(cognome), "'", "")
            cognome = Replace(LCase(cognome), " ", "")
            nome = Replace(LCase(nome), "'", "")
            nome = Replace(LCase(nome), " ", "")

            sesso = LCase(sesso)

            Dim cod() As String = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"}
            Dim decod_pari() As String = {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25"}
            Dim decod_dispari() As String = {"01", "00", "05", "07", "09", "13", "15", "17", "19", "21", "01", "00", "05", "07", "09", "13", "15", "17", "19", "21", "02", "04", "18", "20", "11", "03", "06", "08", "12", "14", "16", "10", "22", "25", "24", "23"}
            Dim cod_digit() As String = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"}
            Dim decod_digit() As String = {"01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26"}
            Dim vocali() As String = {"a", "e", "i", "o", "u"}
            Dim mesi() As String = {"a", "b", "c", "d", "e", "h", "l", "m", "p", "r", "s", "t"}

            errore = 0
            pari = 0
            dispari = 0

            '---------------------------Generazione della parte COGNOME---------------------
            Dim cognome_array() As String = Nothing
            Dim cognome_bin() As Integer = Nothing
            Dim cod_cognome As String = ""
            codice_cognome = ""
            sommacognome = 0

            ReDim cognome_bin(Len(cognome))
            ReDim cognome_array(Len(cognome))

            For i = 1 To Len(cognome)
                cognome_array(i) = Mid(cognome, i, 1)
                cognome_bin(i) = 1
                For j = 0 To 4
                    If cognome_array(i) = vocali(j) Then
                        cognome_bin(i) = 0
                    End If
                Next
                sommacognome = sommacognome + cognome_bin(i)
            Next

            For i = 1 To Len(cognome)
                If cognome_bin(i) = 1 Then
                    cod_cognome = cod_cognome & cognome_array(i)
                End If
            Next

            For i = 0 To Len(cognome)
                If cognome_bin(i) = 0 Then
                    cod_cognome = cod_cognome & cognome_array(i)
                End If
            Next

            If Len(cognome) = 2 Then
                cod_cognome = Left(cod_cognome, 2) & "x"
            ElseIf Len(cognome) = 1 Then
                cod_cognome = Left(cod_cognome, 2) & "xx"
            Else
                cod_cognome = Left(cod_cognome, 3)
            End If

            '--------------------fine   Generazione della parte COGNOME---------------------


            '---------------------------Generazione della parte NOME------------------------
            Dim nome_array() As String = Nothing
            Dim nome_bin() As Integer = Nothing
            Dim codice_nome As String = ""
            Dim cod_nome As String = ""
            Dim sommanome As Integer = 0
            codice_nome = ""
            sommanome = 0

            ReDim nome_bin(Len(nome))
            ReDim nome_array(Len(nome))

            For i = 1 To Len(nome)
                nome_array(i) = Mid(nome, i, 1)
                nome_bin(i) = 1
                For j = 0 To 4
                    If nome_array(i) = vocali(j) Then
                        nome_bin(i) = 0
                    End If
                Next
                sommanome = sommanome + nome_bin(i)
            Next

            For i = 1 To Len(nome)
                If nome_bin(i) = 1 Then
                    cod_nome = cod_nome & nome_array(i)
                End If
            Next

            For i = 0 To Len(nome)
                If nome_bin(i) = 0 Then
                    cod_nome = cod_nome & nome_array(i)
                End If
            Next

            If Len(nome) = 2 Then
                cod_nome = Left(cod_nome, 2) & "x"
            ElseIf Len(nome) = 1 Then
                cod_nome = Left(cod_nome, 2) & "xx"
            ElseIf sommanome >= 4 Then
                cod_nome = Left(cod_nome, 1) & Mid(cod_nome, 3, 2)
            Else
                cod_nome = Left(cod_nome, 3)
            End If
            '--------------------fine   Generazione della parte NOME------------------------



            '------------ Generazione della parte DATA DI NASCITA - SESSO-------------------
            Dim cod_anno As String, cod_mese As String, cod_giorno As String
            Dim cod_data_sesso As String
            cod_anno = Right(anno, 2)
            cod_mese = mesi(mese - 1)

            If sesso = "f" Then
                cod_giorno = giorno + 40
            Else
                'Per i giorni compresi tra 1 e 9 deve essere inserito lo '0'
                cod_giorno = giorno.PadLeft(2, "0")
            End If

            cod_data_sesso = cod_anno & cod_mese & cod_giorno
            '-----------fine Generazione della parte DATA DI NASCITA - SESSO----------------



            ''------------ Generazione della parte COMUNE PROVINCIA-------------------

            'REM il codice del comune di nascita risultante dal codice fiscale passato
            'REM come argomento
            'Dim cod_comune_nascita As String
            ''Dim strComuneInDB As String
            ''Dim strSiglaProvInDB As String

            '' +++++++++++++ Inizio modifica ++++++++++++++++++++++++++++
            Dim codice_c_f_mod As String
            codice_c_f_mod = decodeChar(codice_c_f, 7, 8, 10, 11, 13, 14, 15)
            '' ++++++++++++ Fine modifica ++++++++++++++++++++++++++++++

            'cod_comune_nascita = Mid(codice_c_f_mod, 12, 4)
            ''cod_comune_nascita = Mid(codice_c_f, 12, 4)

            'Dim elencoComuni As New ElencoComuniFiscali(objSlotIn, "", cod_comune_nascita, "")

            ''verifico se trovo un comune (il codice comune non esiste)
            'If elencoComuni.localElenco.Count = 0 Then
            '    Return (-10)
            'End If

            ''If adoRs.EOF Then
            ''    REM l'errore in questo caso è nello stesso codice fiscale passato come
            ''    REM argomento: il suo codice comune non esiste nel database ed è quindi
            ''    REM impossibile il raffronto con il comune passato come argomento
            ''    controllaCodFiscDettaglio = -10
            ''    adoRs = Nothing
            ''    adoConn = Nothing
            ''    Exit Function
            ''Else
            ''    REM Comune per esteso
            ''    strComuneInDB = Trim(adoRs("Comune"))
            ''    REM la sigla
            ''    strSiglaProvInDB = Trim(adoRs("Codice_prv"))
            ''End If
            ''adoRs = Nothing
            ''adoConn = Nothing

            ''-------------------------------- CONTROLLI ------------------------------------
            errore = 0

            Try
                If Len(codice_c_f) <> 16 Then ' ### lunghezza del codice no giusta ###
                    errore = -1
                    Throw New FaschimHandledException
                End If

                If Left(codice_c_f, 3) <> cod_cognome Then ' ### parte cognome ###
                    errore = -3
                    Throw New FaschimHandledException
                End If

                If Mid(codice_c_f, 4, 3) <> cod_nome Then '### parte nome ###
                    errore = -4
                    Throw New FaschimHandledException
                End If

                '++++++++++++ Inizio modifica +++++++++++++++++++++++++++++
                If Mid(codice_c_f_mod, 7, 5) <> cod_data_sesso Then ' ### parte data nascita e sesso ###
                    errore = -5
                    Throw New FaschimHandledException
                End If

                'controllo di corrispondenza fra sigla provincia risultante dal codice fiscale passato
                'e quella passata come parametro a sé stante
                'For Each comuneFisc As ComuneFiscale In elencoComuni.localElenco
                '    If comuneFisc.Provincia.IdProvincia = Provincia Then
                '        If comuneFisc.Descrizione.Trim.ToUpper = comune.Trim.ToUpper Then
                '            'imposto l'errore a 0
                '            errore = 0
                '            Exit For
                '        Else
                '            errore = -7
                '        End If
                '    Else
                '        errore = -6
                '    End If

                'Next

                'verifico se ha passato i controlli di corrispondenza provincia - comune
                If errore <> 0 Then
                    Throw New FaschimHandledException
                End If

                '### controllo del check digit ###
                For i = 1 To 15
                    c_f(i) = Mid(codice_c_f, i, 1)
                Next

                'controllo del check digit - calcolo delle cifre pari
                For i = 2 To 14 Step 2
                    For j = 0 To 35
                        If c_f(i) = cod(j) Then
                            pari = pari + decod_pari(j)
                        End If
                    Next
                Next

                'controllo del check digit - calcolo delle cifre dispari
                For i = 1 To 15 Step 2
                    For j = 0 To 35
                        If c_f(i) = cod(j) Then
                            dispari = dispari + decod_dispari(j)
                        End If
                    Next
                Next

                'controllo del check digit - risultato della parte progressivo
                risultato = pari + dispari
                risultato = risultato - Int(risultato / 26) * 26 + 1

                If cod_digit(risultato - 1) <> Right(codice_c_f, 1) Then
                    errore = -2
                End If

            Catch ex As FaschimHandledException
                'Serve solo per uscire dal ciclo
            End Try

            Return errore
        End Function

        Private Function decodeChar(ByVal s As String, ByVal ParamArray vPos() As Integer) As String
            ' decodifica dei caratteri per la gestione delle omocodie...
            Dim iPos As Integer
            decodeChar = s
            For iPos = 0 To UBound(vPos)
                If Not IsNumeric(Mid(s, vPos(iPos), 1)) Then
                    Select Case LCase(Mid(s, vPos(iPos), 1))
                        Case "l"
                            decodeChar = Left(s, vPos(iPos) - 1) & "0" & Right(s, Len(s) - vPos(iPos))
                        Case "m"
                            decodeChar = Left(s, vPos(iPos) - 1) & "1" & Right(s, Len(s) - vPos(iPos))
                        Case "n"
                            decodeChar = Left(s, vPos(iPos) - 1) & "2" & Right(s, Len(s) - vPos(iPos))
                        Case "p"
                            decodeChar = Left(s, vPos(iPos) - 1) & "3" & Right(s, Len(s) - vPos(iPos))
                        Case "q"
                            decodeChar = Left(s, vPos(iPos) - 1) & "4" & Right(s, Len(s) - vPos(iPos))
                        Case "r"
                            decodeChar = Left(s, vPos(iPos) - 1) & "5" & Right(s, Len(s) - vPos(iPos))
                        Case "s"
                            decodeChar = Left(s, vPos(iPos) - 1) & "6" & Right(s, Len(s) - vPos(iPos))
                        Case "t"
                            decodeChar = Left(s, vPos(iPos) - 1) & "7" & Right(s, Len(s) - vPos(iPos))
                        Case "u"
                            decodeChar = Left(s, vPos(iPos) - 1) & "8" & Right(s, Len(s) - vPos(iPos))
                        Case "v"
                            decodeChar = Left(s, vPos(iPos) - 1) & "9" & Right(s, Len(s) - vPos(iPos))
                    End Select
                End If
            Next

        End Function

        'Somma o sottrae i giorni lavorativi da una data in input
        Public Function AddWorkingDays(ByVal DateIn As DateTime, ByVal ShiftDate As Integer) As DateTime
            Dim stepDay As Integer = IIf(ShiftDate >= 0, 1, -1)
            Dim datDate As DateTime = DateIn

            For i As Integer = stepDay To ShiftDate Step stepDay
                datDate = datDate.AddDays(stepDay)
                While datDate.DayOfWeek = DayOfWeek.Saturday OrElse datDate.DayOfWeek = DayOfWeek.Sunday
                    datDate = datDate.AddDays(stepDay)
                End While
            Next

            Return datDate
        End Function

        Public Function replaceCaratteriEstesi(ByVal stringaIn As String) As String
            'La funzione sostituisci i caretteri estesi con caratteri non estesi.
            '
            'INPUT:
            'stringaIn: stringa su cui eseguire la sostituzione
            '
            'OUTPUT:
            'stringa con eseguita la sostituzione 
            Dim retString As String

            retString = stringaIn

            retString = retString.Replace(Chr(224), Chr(97)) 'carattere à con a
            retString = retString.Replace(Chr(225), Chr(97)) 'carattere á con a
            retString = retString.Replace(Chr(192), Chr(65)) 'carattere À con A
            retString = retString.Replace(Chr(193), Chr(65)) 'carattere Á con A

            retString = retString.Replace(Chr(232), Chr(101)) 'carattere è con e 
            retString = retString.Replace(Chr(233), Chr(101)) 'carattere é con e
            retString = retString.Replace(Chr(200), Chr(69)) 'carattere È con E
            retString = retString.Replace(Chr(201), Chr(69)) 'carattere É con E

            retString = retString.Replace(Chr(236), Chr(105)) 'carattere ì con i
            retString = retString.Replace(Chr(237), Chr(105)) 'carattere í con i
            retString = retString.Replace(Chr(204), Chr(73)) 'carattere Ì con I
            retString = retString.Replace(Chr(205), Chr(73)) 'carattere Í con I

            retString = retString.Replace(Chr(242), Chr(111)) 'carattere ò con o
            retString = retString.Replace(Chr(243), Chr(111)) 'carattere ó con o
            retString = retString.Replace(Chr(210), Chr(79)) 'carattere Ò con O
            retString = retString.Replace(Chr(211), Chr(79)) 'carattere Ó con O

            retString = retString.Replace(Chr(249), Chr(117)) 'carattere ù con u
            retString = retString.Replace(Chr(250), Chr(117)) 'carattere ú con u
            retString = retString.Replace(Chr(217), Chr(85)) 'carattere Ù con U
            retString = retString.Replace(Chr(218), Chr(85)) 'carattere Ú con U

            Return retString
        End Function

        Public Function randomNumber(ByVal MaxNumber As Integer, Optional ByVal MinNumber As Integer = 0) As Integer

            'initialize random number generator
            'Dim r As New Random(System.DateTime.Now.Millisecond)
            Dim r As New Random()

            'if passed incorrect arguments, swap them
            'can also throw exception or return 0

            If MinNumber > MaxNumber Then
                Dim t As Integer = MinNumber
                MinNumber = MaxNumber
                MaxNumber = t
            End If

            Return r.Next(MinNumber, MaxNumber)

        End Function
        ''' <summary>
        ''' Confronta il primo parametro con il secondo: se sono uguali e dello stesso tipo ritorna DBNull
        ''' altrimenti ritorna il primo
        ''' </summary>
        ''' <param name="dato">
        ''' Valore da testare
        ''' </param>
        ''' <param name="nullTest">
        ''' Parametro di test
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DBSet(ByVal dato As Object, ByVal nullTest As Object) As Object
            'Se input nullo allora ritorna NULL
            If dato Is Nothing Then
                Return DBNull.Value
            End If

            'Verifica se i tipi sono uguali
            If dato.GetType().IsInstanceOfType(nullTest) Then
                If dato = nullTest Then
                    Return DBNull.Value
                End If
            End If

            'Test passato ritorna il valore
            Return dato
        End Function

        ''' <summary>
        ''' Se il primo parametro è DBNull ritorna il secondo altrimenti ritorna il primo
        ''' </summary>
        ''' <param name="campoDB"></param>
        ''' <param name="defaultVal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DBGet(ByVal campoDB As Object, ByVal defaultVal As Object) As Object
            'Se input nullo allora ritorna NULL
            If campoDB Is Nothing Then
                Return defaultVal
            End If

            'Verifica se il campo è DBNull ritorna defaultVal
            If Convert.IsDBNull(campoDB) Then
                Return defaultVal
            End If


            'Non è NULL, ritorna il valore del campo
            Return campoDB
        End Function


        ''' <summary>
        ''' Ritorna un dataset caricato a partire da un testo XML
        ''' </summary>
        ''' <param name="xmlIn"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DatasetFromXml(ByVal xmlIn As String) As DataSet
            Dim ds As New DataSet()
            Using sr As New IO.StringReader(xmlIn)
                ds.ReadXml(sr)
            End Using

            Return ds
        End Function

 

        Public Function getDataFineMese(ByVal dataIn As Date) As Date
            'Restituisce la data all'ultimo giorno del mese
            If dataIn = NOTHING_DATE Then
                Return dataIn
            Else
                dataIn = dataIn.AddMonths(1) 'mese successivo di dataIn
                Return New Date(dataIn.Year, dataIn.Month, 1).AddDays(-1) 'ultimo giorno del mese di dataIn
            End If

        End Function

        Public Function getDataFineAnno(ByVal dataIn As Date) As Date
            Dim d As Date

            d = New Date(dataIn.Year, 12, 31)

            Return d
        End Function

        Public Function getDataInizioMese(ByVal dataIn As Date) As Date
            ' Restituisce sempre la data al primo giorno del mese
            Dim d As Date

            d = New Date(dataIn.Year, dataIn.Month, 1)

            Return d
        End Function


        Function EmailAddressCheck(ByVal emailAddress As String) As Boolean

            'Rendo la mail UpperCase
            'Per garantire la corretta verifica tramite regular expression
            'senza considerare la differenza tra lettere maiuscole e minuscole
            If emailAddress <> "" Then
                emailAddress = emailAddress.ToUpper
            End If

            Dim pattern As String = "\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b"
            Dim emailAddressMatch As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(emailAddress, pattern)
            If emailAddressMatch.Success Then
                EmailAddressCheck = True
            Else
                EmailAddressCheck = False
            End If

        End Function

    End Module




End Namespace
