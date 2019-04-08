'Imports System.Resources
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Data.Common
Imports MySql.Data.MySqlClient


Namespace VecchieClassi


    Public Class DataBase

#Region "Proprietà"
        Private cn As MySqlConnection

        'Private cn As Odbc.OdbcConnection
        Private mSQL As String
        Private mStrErr As String
        Private mRecordsAffected As Long = 0
        Private mLogInfo As String
        Private mActiveTransaction As MySqlTransaction
        Private mParams As List(Of MySqlParameter) = New List(Of MySqlParameter)

        'Chiave Web.Config dove prendere la connectionstring
        Private mConnStrKEY As String

        'Private mActiveTransaction As Odbc.OdbcTransaction
        Private mstrXML As String
        Private mAttivaDebug As Boolean = False
        Private mContaQuery As Integer = 0
        Private mFileDebug As String = "DebugDatabase.log"

        'Non esegue comandi Transazione 
        Private mControllaTransazioni As Boolean = False
        Private mNumeroTransazioniControllate As Integer = 0

        Public Property AttivaDebug() As Boolean
            Get
                Return mAttivaDebug
            End Get
            Set(ByVal value As Boolean)
                mAttivaDebug = value
            End Set
        End Property

        Public ReadOnly Property ContaQuery() As Integer
            Get
                Return mContaQuery
            End Get
        End Property

        Public Property FileDebug() As String
            Get
                Return mFileDebug
            End Get
            Set(ByVal value As String)
                mFileDebug = value
            End Set
        End Property

        Public Property SQL() As String
            Get
                Return mSQL
            End Get
            Set(ByVal Value As String)
                mSQL = Value
            End Set
        End Property

        Public ReadOnly Property StrErr() As String
            Get
                Return mStrErr
            End Get
        End Property

        Public ReadOnly Property RecordsAffected() As Long
            Get
                Return mRecordsAffected
            End Get
        End Property

        Public Property strXML() As String
            Get
                Return mstrXML
            End Get
            Set(ByVal Value As String)
                mstrXML = Value
            End Set
        End Property

        Public Property LogInfo() As String
            Get
                Return mLogInfo
            End Get
            Set(ByVal Value As String)
                mLogInfo = Value
            End Set
        End Property

        Public ReadOnly Property ControllaTransazioni() As Boolean
            Get
                Return mControllaTransazioni
            End Get
        End Property
#End Region

#Region "Funzioni Pubbliche"

        Public Sub New()
            Me.mConnStrKEY = "connectionString"

        End Sub

        Public Sub New(ByVal connectionKEY As String)
            Me.New()
            If connectionKEY.Trim <> "" Then
                Me.mConnStrKEY = connectionKEY
            End If
        End Sub

        ''' <summary>
        ''' Aggiunge parametro SQL
        ''' </summary>
        ''' <param name="chiave"></param>
        ''' <param name="valore"></param>
        ''' <remarks></remarks>
        Public Sub AddParametro(ByVal chiave As String, ByVal valore As Object)
            Me.mParams.Add(New MySqlParameter(chiave, valore))
        End Sub

        ''' <summary>
        ''' Ottiene ultimo id inserito
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LastInsertId() As Long
            Me.SQL = "SELECT LAST_INSERT_ID()"

            Dim oRet As Object = Me.ExecScalar()

            If Convert.IsDBNull(oRet) Then
                Return 0
            Else
                Return Convert.ToInt64(oRet)
            End If
        End Function


        Public Function ExecScalar() As Object

            Dim cmd As MySqlCommand
            Dim retObj As Object = Nothing

            Try
                If mActiveTransaction Is Nothing Then OpenConnection()
                cmd = cn.CreateCommand
                cmd.CommandText = mSQL
                Me.ImpostaParametri(cmd)

                If Not mActiveTransaction Is Nothing Then cmd.Transaction = mActiveTransaction
                mRecordsAffected = 1
                retObj = cmd.ExecuteScalar()
                cmd.Dispose()

                'Se il risultato è null imposta -1
                'retObj = IIf(Convert.IsDBNull(retObj), -1, retObj)

                Return retObj
            Catch ex As Exception

                mStrErr = ex.Message
                writeLog(mSQL, ex.Message)
                Return -1
            Finally
                If mActiveTransaction Is Nothing Then CloseConnection()
            End Try
        End Function


        Public Function ExecSQL() As Int32

            Dim iRet As Long
            Dim cmd As MySqlCommand
            'Dim cmd As Odbc.OdbcCommand

            Try
                If mActiveTransaction Is Nothing Then OpenConnection()
                cmd = cn.CreateCommand
                cmd.CommandText = mSQL
                Me.ImpostaParametri(cmd)

                If Not mActiveTransaction Is Nothing Then cmd.Transaction = mActiveTransaction
                mRecordsAffected = cmd.ExecuteNonQuery()
                cmd.Dispose()
                iRet = mRecordsAffected

            Catch ex As Exception

                iRet = -1
                mStrErr = ex.Message
                writeLog(mSQL, ex.Message)

            Finally
                If mActiveTransaction Is Nothing Then CloseConnection()
            End Try

            Return iRet

        End Function

        Public Function OpenQuery() As Int32

            Dim iRet As Long
            Dim da As MySqlDataAdapter
            'Dim da As Odbc.OdbcDataAdapter
            Dim ds As DataSet

            Try
                If mActiveTransaction Is Nothing Then OpenConnection()
                'OpenAdapter(da)
                da = New MySqlDataAdapter(mSQL, cn)
                'da = New Odbc.OdbcDataAdapter(mSQL, cn)
                ds = New DataSet
                mRecordsAffected = da.Fill(ds)
                strXML = ds.GetXml
                ds.Dispose()
                iRet = mRecordsAffected

            Catch ex As Exception
                mStrErr = ex.Message
                iRet = -1
                writeLog(mSQL, ex.Message)

            Finally
                If mActiveTransaction Is Nothing Then CloseConnection()
            End Try

            Return iRet

        End Function

        Public Function OpenQuery(ByRef ds As DataSet) As Int32

            Dim iRet As Long
            Dim da As MySqlDataAdapter

            Try
                Dim cmd As New MySqlCommand()
                If mActiveTransaction Is Nothing Then OpenConnection()

                cmd.Connection = cn
                cmd.CommandText = mSQL
                cmd.Transaction = mActiveTransaction
                Me.ImpostaParametri(cmd)

                da = New MySqlDataAdapter(cmd)
                If ds Is Nothing Then
                    ds = New DataSet
                End If
                mRecordsAffected = da.Fill(ds)
                da.Dispose()
                iRet = mRecordsAffected

            Catch ex As Exception
                mStrErr = ex.Message
                iRet = -1
                writeLog(mSQL, ex.Message)

            Finally
                If mActiveTransaction Is Nothing Then CloseConnection()
            End Try

            Return iRet

        End Function


        'Query con parametri per paginazione
        Public Function openQuery(ByRef clDataSet As DataSet, ByVal posizione As Integer, ByVal offset As Integer, ByRef totRecord As Long) As Int32

            Dim iRet As Long
            Dim da As MySqlDataAdapter
            Dim cmd As MySqlCommand
            Dim reader As MySqlDataReader
            Dim sqlCount As String
            Dim start, orderIndex As Integer

            Try
                If mActiveTransaction Is Nothing Then OpenConnection()

                cmd = New MySqlCommand
                cmd.Connection = cn
                cmd.Transaction = mActiveTransaction

                'Conta record: ver.5 usa subselect
                If cn.ServerVersion.Chars(0) = "5" Then
                    'calcolo totale records
                    orderIndex = mSQL.ToUpper.IndexOf(" ORDER ")
                    If orderIndex > 0 Then
                        sqlCount = "SELECT COUNT(*) from (" + mSQL.Substring(0, orderIndex) + ") as tbl"
                    Else
                        sqlCount = "SELECT COUNT(*) from (" + mSQL + ") as tbl"
                    End If

                    'esegue query per conteggio ed utilizza il metodo executescalar per ritornare solo la prima colonna della prima riga
                    cmd.CommandText = sqlCount
                    totRecord = CLng(cmd.ExecuteScalar)
                Else 'ver.4 non usa subselect
                    'calcolo totale records
                    sqlCount = "SELECT COUNT(*) as Totale "
                    start = mSQL.ToUpper.IndexOf(" FROM ")

                    If mSQL.ToUpper.IndexOf(" ORDER ") > 0 Then
                        'sqlCount = sqlCount & mSQL.Substring(mSQL.IndexOf(" FROM "), mSQL.IndexOf(" ORDER ") - mSQL.IndexOf(" FROM "))
                        sqlCount = sqlCount & mSQL.Substring(start, mSQL.ToUpper.IndexOf(" ORDER ") - start)

                    Else
                        'sqlCount = sqlCount & mSQL.Substring(mSQL.IndexOf(" FROM "))
                        sqlCount = sqlCount & mSQL.Substring(start)

                    End If

                    'TODO: non posso utilizzare lo scalar anche qui?
                    cmd.CommandText = sqlCount
                    reader = cmd.ExecuteReader()
                    reader.Read()
                    totRecord = reader("totale")
                    reader.Close()
                End If


                'Ora esegue query richiesta impostando LIMIT e OFFSET
                cmd.CommandText = String.Format(mSQL + " LIMIT {0} OFFSET {1}", offset, posizione)

                ''cmd.CommandText = mSQL_SELECT & mSQL_FROM & mSQL_ORDER
                da = New MySqlDataAdapter(cmd)
                clDataSet.Reset()
                iRet = da.Fill(clDataSet)

                da.Dispose()
                cmd.Dispose()

            Catch ex As Exception
                mStrErr = ex.Message
                iRet = -1
            Finally
                If mActiveTransaction Is Nothing Then CloseConnection()
            End Try

            Return iRet

        End Function


        Public Function OpenTrans() As Int32
            'Se il controllo transazione viene impostato a True, tutte le chiamate a funzioni di transazione
            'non vengono eseguite
            If Me.mControllaTransazioni Then
                Return 0
            End If

            Try
                OpenConnection()

                'Apre una nuova transazione solo se non ce n'è una aperta
                If Me.mActiveTransaction Is Nothing Then
                    mActiveTransaction = cn.BeginTransaction
                End If

                Return 0
            Catch ex As Exception
                mStrErr = ex.Message
                Return -1
            End Try
        End Function

        Public Function CommitTrans() As Int32
            'Se il controllo transazione viene impostato a True, tutte le chiamate a funzioni di transazione
            'non vengono eseguite
            If Me.mControllaTransazioni Then
                Return 0
            End If

            Try
                If Me.mActiveTransaction IsNot Nothing Then
                    Me.mActiveTransaction.Commit()
                End If

                Return 0
            Catch ex As Exception
                mStrErr = ex.Message
                Return -1
            Finally
                mActiveTransaction = Nothing
                CloseConnection()
            End Try
        End Function

        Public Function RollBackTrans() As Int32
            'Se il controllo transazione viene impostato a True, tutte le chiamate a funzioni di transazione
            'non vengono eseguite
            If Me.mControllaTransazioni Then
                Return 0
            End If

            Try
                If Me.mActiveTransaction IsNot Nothing Then
                    Me.mActiveTransaction.Rollback()
                End If

                Return 0
            Catch ex As Exception
                mStrErr = ex.Message
                Return -1
            Finally
                mActiveTransaction = Nothing
                CloseConnection()
            End Try
        End Function

        ''' <summary>
        ''' Attiva il controllo delle transazioni: successivamente a questa chiamata le funzioni di transazione 
        ''' non verranno eseguite
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub IniziaControlloTransazione()

            If Me.mControllaTransazioni Then
                Throw New EFaschimError("Il controllo della transazione risulta gia' attivato")
            End If

            Me.mControllaTransazioni = True

        End Sub

        ''' <summary>
        ''' Termina il controllo delle transazioni: successivamente a questa chiamata le funzioni di transazione 
        ''' verranno rieseguite
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub TerminaControlloTransazione()

            If Not Me.mControllaTransazioni Then
                Throw New EFaschimError("Il controllo della transazione risulta gia' terminato")
            End If

            Me.mControllaTransazioni = False

        End Sub


#End Region

#Region "Funzioni Private"

        Private Sub ImpostaParametri(ByVal cmd As MySqlCommand, Optional ByVal svuotaLista As Boolean = True)
            cmd.Parameters.Clear()

            For Each param As MySqlParameter In Me.mParams
                cmd.Parameters.Add(param)
            Next

            If svuotaLista Then
                Me.mParams.Clear()
            End If
        End Sub

        Private Function OpenConnection() As Int32

            If cn Is Nothing Then
                'crea conn con connectionstring specifica
                cn = New MySqlConnection(ConfigurationManager.AppSettings(Me.mConnStrKEY))
            End If
            If Not cn.State = ConnectionState.Open Then
                cn.Open()
            End If

        End Function

        Private Function CloseConnection() As Int32
            Try
                cn.Close()
                Return 0
            Catch ex As Exception
                mStrErr = ex.Message
                Return -1
            End Try
        End Function

        'Private Function OpenAdapter(ByRef ClientDataAdpt As MySqlDataAdapter) As Int32

        '    'Try
        '    '    Dim da As New MySqlDataAdapter(mSQL, cn)
        '    '    ClientDataAdpt = da
        '    '    Return 0

        '    'Catch ex As Exception
        '    '    mStrErr = ex.Message
        '    '    Return -1
        '    'End Try

        'End Function

        Private Sub writeLog(ByVal sOperazione As String, ByVal sEsito As String)

            scriviLogWS(LogInfo & ";" & sOperazione & ";" & "ESITO: " & sEsito, "logSQL_" & Format(Now, "ddMMyyyy") & ".txt")

        End Sub

       

#End Region

    End Class

End Namespace