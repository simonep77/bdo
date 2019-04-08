Namespace VecchieClassi
    Public Class ListaIscrittiPerPolizza
        Inherits BaseElenco

#Region "PUBBLICO"

        Public Sub New(ByRef objSlotIn As BusinessSlot)
            Me.ListaIscrittiPerPolizza(objSlotIn)
        End Sub


        ''' <summary>
        ''' Cerca componenti nucleo
        ''' </summary>
        ''' <param name="codPolizzaIn"></param>
        ''' <remarks></remarks>
        Public Sub CercaPerPolizza(ByVal codPolizzaIn As Integer)

            Me.currentSlot.dbConnection.SQL = String.Format("SELECT IdIscrittiPolizza FROM iscrittiperpolizza WHERE IdPolizza={0} ORDER BY CodGradoParentela", codPolizzaIn)
            Me.EseguiRicerca()

            ImpostaAttributi()
        End Sub

        Public Sub CercaPerAnagrafica(ByVal codAnagraficaIn As String, Optional ByVal orderByData As Boolean = False)

            Dim sb As New System.Text.StringBuilder

            sb.Append("SELECT IdIscrittiPolizza FROM IscrittiPerPolizza ")
            sb.AppendFormat("WHERE IdAnagrafica = '{0}' ", codAnagraficaIn)
            If orderByData Then
                sb.Append("ORDER BY DataIscrizione")
            End If

            Me.currentSlot.dbConnection.SQL = sb.ToString
            Me.EseguiRicerca()

            ImpostaAttributi()
        End Sub

        Public Sub CercaPerPolizzaAnagrafica(ByVal codPolizzaIn As Integer, ByVal codAnagraficaIn As String)

            Me.currentSlot.dbConnection.SQL = String.Format("SELECT IdIscrittiPolizza FROM IscrittiPerPolizza WHERE IdPolizza={0} AND IdAnagrafica = '{1}'", codPolizzaIn, codAnagraficaIn)
            Me.EseguiRicerca()

            ImpostaAttributi()
        End Sub

        ''' <summary>
        ''' Esegue ricerca componenti nucleo per protocollazione richieste
        ''' </summary>
        ''' <param name="codAnagrafica"></param>
        ''' <param name="nome"></param>
        ''' <param name="cognome"></param>
        ''' <remarks></remarks>
        Public Sub CercaPerProtocolloRichiesta(ByVal codAnagrafica As String, ByVal nome As String, ByVal cognome As String)

            Dim sb As New System.Text.StringBuilder
            sb.Append("SELECT ip2.IdIscrittiPolizza ")
            sb.Append("FROM iscrittiperpolizza ip ")
            sb.Append("INNER JOIN iscrittiperpolizza ip2 ON ip2.IdPolizza=ip.IdPolizza ")
            sb.AppendFormat("WHERE ip.IdAnagrafica = {0} ", stringaPerSql(codAnagrafica))
            sb.Append("AND ip.IdPolizza IN ")
            sb.Append("( ")
            sb.Append("SELECT ip.IdPolizza FROM anagrafica an ")
            sb.Append("INNER JOIN iscrittiperpolizza ip ON ip.IdAnagrafica=an.IdAnagrafica ")
            sb.AppendFormat("WHERE an.Cognome = '{0}' ", cognome)
            sb.AppendFormat("AND an.Nome = '{0}' ", nome)
            sb.Append(") ")
            sb.Append("AND  ")
            sb.Append("( ")
            sb.AppendFormat("ip2.codstatoiscritto <> {0} ", CODICE_STATO_ISCRITTO_CESSATO)
            sb.Append("OR ")
            sb.AppendFormat("(ip2.codstatoiscritto = {0} AND abs(datediff(ifnull(ip2.DataCessazione, ip.DataCessazione), now())) < {1}) ", CODICE_STATO_ISCRITTO_CESSATO, "366")
            sb.Append(") ")
            sb.Append(" ORDER BY ip2.CodGradoParentela, ip2.IdPolizza DESC")

            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            ImpostaAttributi()
        End Sub

        ''' <summary>
        ''' Cerca tutte le posizioni per il codice anagrafico fornito
        ''' </summary>
        ''' <param name="codAnagrafica"></param>
        ''' <param name="includiCambioCodici">
        ''' Include eventuali reiscrizioni/cambio codici
        ''' </param>
        ''' <remarks></remarks>
        Public Sub CercaTuttePosizioni(ByVal codAnagrafica As String, Optional ByVal includiCambioCodici As Boolean = False)

            Dim sb As New System.Text.StringBuilder
            sb.Append("SELECT ip.IdIscrittiPolizza, ip.DataIscrizione ")
            sb.Append("FROM iscrittiperpolizza ip ")
            sb.AppendFormat("WHERE ip.IdAnagrafica = {0} ", StringaSql(codAnagrafica))

            'Se richi
            If includiCambioCodici Then
                sb.Append("UNION ")
                sb.Append("SELECT ip.IdIscrittiPolizza, ip.DataIscrizione ")
                sb.Append("FROM storiacodiciiscritti sc ")
                sb.Append("INNER JOIN storiacodiciiscritti sc2 ON sc2.idStoria=sc.idStoria ")
                sb.Append("INNER JOIN iscrittiperpolizza ip ON ip.IdAnagrafica=sc2.codAnagraficaA ")
                sb.AppendFormat("WHERE sc.codanagraficaDa = {0} ", StringaSql(codAnagrafica))
            End If

            sb.Append("ORDER BY DataIscrizione ASC ")

            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            Me.ImpostaAttributi()
        End Sub
        ''' <summary>
        ''' Cerca tutte le posizioni per il codice anagrafico fornito
        ''' </summary>
        ''' <param name="codAnagrafica"></param>
        ''' <param name="dallaPiuRecente">
        ''' indica l'ordinamento per data iscrizione
        ''' </param>
        ''' <remarks></remarks>
        Public Sub CercaTuttePosizioniByCodFiscale(ByVal codAnagrafica As String, ByVal dallaPiuRecente As Boolean)

            Dim sb As New System.Text.StringBuilder

            sb.Append("SELECT IP.Idiscrittipolizza ")
            sb.Append("FROM anagrafica AN  ")
            sb.Append("INNER JOIN anagrafica AN1 ON an1.codicefiscale = an.codicefiscale  ")
            sb.Append("INNER JOIN iscrittiperpolizza IP ON ip.idanagrafica = an1.idanagrafica ")
            sb.AppendFormat("WHERE AN.idanagrafica={0} ", stringaPerSql(codAnagrafica))

            If dallaPiuRecente Then
                sb.Append("ORDER BY ip.dataiscrizione DESC ")
            Else
                sb.Append("ORDER BY ip.dataiscrizione ASC ")
            End If

            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            Me.ImpostaAttributi()
        End Sub

        ''' <summary>
        ''' Cerca tutte le posizioni cessate precedenti alla mia
        ''' </summary>
        ''' <param name="cognome"></param>
        ''' <param name="nome"></param>
        ''' <param name="sesso"></param>
        ''' <param name="dataNascita"></param>
        ''' <remarks></remarks>
        Public Sub CercaCessatiPrecedentiPerDatiAnagrafici(ByVal cognome As String, ByVal nome As String, ByVal sesso As String, ByVal dataNascita As Date, ByVal codPolizzaIn As Long, ByVal dataIscrizioneIn As Date, ByVal codGradoParentelaIn As Integer, ByVal idAnagraficaIn As String)

            Dim sb As New System.Text.StringBuilder
            sb.Append("SELECT IP.IdIscrittiPolizza ")
            sb.Append("FROM Anagrafica AN ")
            sb.Append("INNER JOIN IscrittiPerPolizza IP ON AN.IdAnagrafica = IP.IdAnagrafica ")
            sb.AppendFormat("WHERE AN.Cognome = {0} ", stringaPerSql(cognome))
            sb.AppendFormat("AND AN.Nome = {0} ", stringaPerSql(nome))
            sb.AppendFormat("AND AN.Sesso = {0} ", stringaPerSql(sesso))
            sb.AppendFormat("AND AN.DataNascita = {0} ", dataPerSql(dataNascita))
            sb.AppendFormat("AND IP.CodStatoIscritto <> {0} ", CODICE_STATO_ISCRITTO_ATTIVO)
            If codGradoParentelaIn = CODICE_INTESTATARIO Then
                sb.AppendFormat("AND IP.IdPolizza <> {0} ", codPolizzaIn)
            Else
                sb.AppendFormat("AND (IP.IdPolizza <> {0} OR (IP.IdPolizza = {0} AND IP.IdAnagrafica <> {1})) ", codPolizzaIn, stringaPerSql(idAnagraficaIn))
            End If
            If dataIscrizioneIn <> Nothing Then
                sb.AppendFormat("AND IP.DataCessazione < {0} ", dataPerSql(dataIscrizioneIn))
            End If
            sb.Append("ORDER BY IP.DataIscrizione DESC")

            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            Me.ImpostaAttributi()
        End Sub

        ''' <summary>
        ''' Cerca tutte posizioni per dati anagrafici con parametri
        ''' </summary>
        ''' <param name="cognome"></param>
        ''' <param name="nome"></param>
        ''' <param name="dataNascita"></param>
        ''' <param name="codPolizzaIn"></param>
        ''' <param name="dataCessazioneIn"></param>
        ''' <param name="dataIscrizioneIn"></param>
        ''' <remarks></remarks>

        Public Sub CercaPosizioniPerDatiAnagraficiParam(ByVal cognome As String, ByVal nome As String, ByVal dataNascita As Date, _
                                 Optional ByVal dataIscrizioneIn As Date = Nothing, Optional ByVal dataCessazioneIn As Date = Nothing, _
                                 Optional ByVal codPolizzaIn As Long = Nothing, Optional ByVal getPosizioneVicina As Boolean = False)

            Dim sb As New System.Text.StringBuilder
            sb.Append(" SELECT IP.IdIscrittiPolizza ")
            sb.Append(" FROM Anagrafica AN ")
            sb.Append(" INNER JOIN IscrittiPerPolizza IP ON AN.IdAnagrafica = IP.IdAnagrafica ")
            sb.AppendFormat(" WHERE AN.Cognome = {0} ", stringaPerSql(cognome))
            sb.AppendFormat(" AND AN.Nome = {0} ", stringaPerSql(nome))
            sb.AppendFormat(" AND AN.DataNascita = {0} ", dataPerSql(dataNascita))

            If codPolizzaIn <> Nothing Then
                sb.AppendFormat(" AND IP.IdPolizza <> {0} ", codPolizzaIn)
            End If
            If dataIscrizioneIn <> Nothing Then
                sb.AppendFormat(" AND IP.DataCessazione < {0} ", dataPerSql(dataIscrizioneIn))
            End If
            If dataCessazioneIn <> Nothing Then
                sb.AppendFormat(" AND IP.DataIscrizione > {0} ", dataPerSql(dataCessazioneIn))
            End If

            If getPosizioneVicina Then
                If dataIscrizioneIn <> Nothing Then
                    sb.Append(" ORDER BY IP.DataCessazione DESC")
                ElseIf dataCessazioneIn <> Nothing Then
                    sb.Append(" ORDER BY IP.DataIscrizione ASC")
                End If
            End If


            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            Me.ImpostaAttributi()
        End Sub

        ''' <summary>
        ''' Cerca tutte le posizioni avendo in input vari dati anagrafici
        ''' </summary>
        ''' <param name="cognome"></param>
        ''' <param name="nome"></param>
        ''' <param name="sesso"></param>
        ''' <param name="dataNascita"></param>
        ''' <remarks></remarks>
        Public Sub CercaPerDatiAnagrafici(ByVal cognome As String, ByVal nome As String, ByVal sesso As String, ByVal dataNascita As Date)

            Dim sb As New System.Text.StringBuilder
            sb.Append("SELECT IP.IdIscrittiPolizza ")
            sb.Append("FROM Anagrafica AN ")
            sb.Append("INNER JOIN IscrittiPerPolizza IP ON AN.IdAnagrafica = IP.IdAnagrafica ")
            sb.AppendFormat("WHERE AN.Cognome = {0} ", stringaPerSql(cognome))
            sb.AppendFormat("AND AN.Nome = {0} ", stringaPerSql(nome))
            sb.AppendFormat("AND AN.Sesso = {0} ", stringaPerSql(sesso))
            sb.AppendFormat("AND AN.DataNascita = {0} ", dataPerSql(dataNascita))
            sb.Append("ORDER BY IP.DataIscrizione DESC")

            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            Me.ImpostaAttributi()
        End Sub

        ''' <summary>
        ''' Cerca tutte le posizioni avendo in input il codice fiscale
        ''' </summary>
        ''' <param name="codiceFiscale"></param>
        ''' <remarks></remarks>
        Public Sub CercaPerCodiceFiscale(ByVal codiceFiscale As String)

            Dim sb As New System.Text.StringBuilder
            sb.Append("SELECT IP.IdIscrittiPolizza ")
            sb.Append("FROM Anagrafica AN ")
            sb.Append("INNER JOIN IscrittiPerPolizza IP ON AN.IdAnagrafica = IP.IdAnagrafica ")
            sb.AppendFormat("WHERE AN.CodiceFiscale = {0} ", stringaPerSql(codiceFiscale))
            sb.Append("ORDER BY IP.DataIscrizione DESC")

            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            Me.ImpostaAttributi()
        End Sub


        ''' <summary>
        ''' Cerca tutti i familiari di una polizza
        ''' </summary>
        ''' <param name="codPolizzaIn"></param>
        ''' <remarks></remarks>
        Public Sub CercaFamiliari(ByVal codPolizzaIn As Integer)

            Me.currentSlot.dbConnection.SQL = String.Format("SELECT IdIscrittiPolizza FROM IscrittiPerPolizza WHERE IdPolizza={0} AND CodGradoParentela <> {1}", codPolizzaIn, CODICE_INTESTATARIO)
            Me.EseguiRicerca()

            ImpostaAttributi()
        End Sub

        ''' <summary>
        ''' Cerca l'ultima posizione per il codice anagrafico fornito
        ''' </summary>
        ''' <param name="codAnagrafica"></param>
        ''' <remarks></remarks>
        Public Sub CercaUltimaPosizionePerAnagrafica(ByVal codAnagrafica As String)

            Dim sb As New System.Text.StringBuilder
            sb.Append("SELECT MAX(IdIscrittiPolizza) as IdIscrittiPolizza ")
            sb.Append("FROM IscrittiPerPolizza ")
            sb.AppendFormat("WHERE IdAnagrafica = {0} ", StringaSql(codAnagrafica))

            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            Me.ImpostaAttributi()
        End Sub


        Public Sub CercaPerTest()

            Dim sb As New System.Text.StringBuilder
            sb.Append("SELECT IdIscrittiPolizza ")
            sb.Append("FROM IscrittiPerPolizza ")

            Me.currentSlot.dbConnection.SQL = sb.ToString()
            Me.EseguiRicerca()

            Me.ImpostaAttributi()
        End Sub

#End Region

#Region "PRIVATO"

        Private Sub ListaIscrittiPerPolizza(ByRef objSlotIn As BusinessSlot)
            Me.currentSlot = objSlotIn
        End Sub

        Private Sub ImpostaAttributi()
            Dim dr As DataRow

            For Each dr In Me.localDataSet.Tables(0).Rows
                Me.localElenco.Add(New IscrittiPerPolizza(Me.currentSlot, dr.Item("IdIscrittiPolizza")))
            Next
        End Sub

#End Region






    End Class

End Namespace
