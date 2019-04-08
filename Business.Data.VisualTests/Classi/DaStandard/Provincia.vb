Imports System.Xml

Namespace VecchieClassi
    Public Class Provincia

#Region " PROPRIETA "

        Private mID As Long
        Private mIdProvincia As String
        Private mDescrizione As String
        Private mCodRegione As Integer
        Private mErrore As Boolean

        Public ReadOnly Property ID() As Long
            Get
                Return Me.mID
            End Get
        End Property

        Public Property codice() As String
            Get
                Return mIdProvincia
            End Get
            Set(ByVal Value As String)
                mIdProvincia = Value
            End Set
        End Property

        Public ReadOnly Property CodiceRegione() As Integer
            Get
                Return Me.mCodRegione
            End Get

        End Property

        Public Property descrizione() As String
            Get
                Return mDescrizione
            End Get
            Set(ByVal Value As String)
                mDescrizione = Value.ToUpper
            End Set
        End Property

        Public Property Errore() As Boolean
            Get
                Return mErrore
            End Get
            Set(ByVal Value As Boolean)
                mErrore = Value
            End Set
        End Property



#End Region

#Region " METODI PUBBLICI "

        Public Sub New()
            Provincia()
        End Sub

        Public Sub New(ByVal codiceIn As String)
            Provincia(codiceIn)
        End Sub

        Public Sub New(ByVal idProvincia As Long, ByVal fake1 As Boolean)
            Provincia(idProvincia, fake1)
        End Sub

#End Region

#Region " METODI PRIVATI "

        Private Sub Provincia()
            mIdProvincia = ""
            mDescrizione = ""
            Me.mID = 0
            Me.mCodRegione = 0

        End Sub

        Private Sub Provincia(ByVal codiceIn As String)

            Dim sQuery As String
            Dim db As New DataBase
            Dim iRet As Long

            Provincia()

            sQuery = "SELECT * "
            sQuery = sQuery & "FROM Province "
            sQuery = sQuery & "WHERE IdProvincia = '" & codiceIn & "'"

            Try
                With db
                    .SQL = sQuery
                    iRet = .OpenQuery()

                    If iRet >= 0 Then
                        Dim xmlDoc As New XmlDocument
                        xmlDoc.LoadXml(.strXML)
                        Me.mID = xmlDoc.GetElementsByTagName("Id").Item(0).InnerText
                        mIdProvincia = xmlDoc.GetElementsByTagName("IdProvincia").Item(0).InnerText
                        mDescrizione = xmlDoc.GetElementsByTagName("DescrizioneProvincia").Item(0).InnerText
                        Me.mCodRegione = xmlDoc.GetElementsByTagName("CodRegione").Item(0).InnerText
                        mErrore = False
                    ElseIf iRet < 0 Then
                        'listaMessaggi.aggiungi(New Messaggio(1, "Registrazione", "CodiceAccesso"))
                        mErrore = True
                    End If

                End With

            Catch ex As Exception
                'listaMessaggi.aggiungi(New Messaggio(1, "Registrazione", "CodiceAccesso"))
                mErrore = True
            End Try

        End Sub

        Private Sub Provincia(ByVal idProvincia As Long, ByVal fake1 As Boolean)

            Dim db As New DataBase
            Dim iRet As Long

            Provincia()

            Try
                With db
                    .SQL = "SELECT * FROM province WHERE id=" & idProvincia
                    iRet = .OpenQuery()

                    If iRet >= 0 Then
                        Dim xmlDoc As New XmlDocument
                        xmlDoc.LoadXml(.strXML)
                        Me.mID = xmlDoc.GetElementsByTagName("Id").Item(0).InnerText
                        mIdProvincia = xmlDoc.GetElementsByTagName("IdProvincia").Item(0).InnerText
                        mDescrizione = xmlDoc.GetElementsByTagName("DescrizioneProvincia").Item(0).InnerText
                        Me.mCodRegione = xmlDoc.GetElementsByTagName("CodRegione").Item(0).InnerText
                        mErrore = False
                    ElseIf iRet < 0 Then
                        'listaMessaggi.aggiungi(New Messaggio(1, "Registrazione", "CodiceAccesso"))
                        mErrore = True
                    End If

                End With

            Catch ex As Exception
                'listaMessaggi.aggiungi(New Messaggio(1, "Registrazione", "CodiceAccesso"))
                mErrore = True
            End Try

        End Sub

#End Region

    End Class
End Namespace

