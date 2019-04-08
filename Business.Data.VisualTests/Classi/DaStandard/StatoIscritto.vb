Imports System.Xml

Namespace VecchieClassi
    Public Class StatoIscritto

#Region " PROPRIETA "

        Private mIdStatoIscritto As Integer
        Private mDescrizione As String
        Private mErrore As String

        Public Property codice() As Integer
            Get
                Return mIdStatoIscritto
            End Get
            Set(ByVal Value As Integer)
                mIdStatoIscritto = Value
            End Set
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
            StatoIscritto()
        End Sub

        Public Sub New(ByVal codiceIn As Integer)
            StatoIscritto(codiceIn)
        End Sub

#End Region

#Region " METODI PRIVATI "

        Private Sub StatoIscritto()
            mIdStatoIscritto = 0
            mDescrizione = ""
        End Sub

        Private Sub StatoIscritto(ByVal codiceIn As Integer)

            Dim sQuery As String
            Dim db As New DataBase
            Dim iRet As Long

            StatoIscritto()

            sQuery = "SELECT * "
            sQuery = sQuery & "FROM statiiscritto "
            sQuery = sQuery & "WHERE IdStatoIscritto = " & codiceIn

            Try
                With db
                    .SQL = sQuery
                    iRet = .OpenQuery
                    If iRet > 0 Then
                        Dim xmlDoc As New XmlDocument
                        xmlDoc.LoadXml(.strXML)
                        mIdStatoIscritto = CInt(xmlDoc.GetElementsByTagName("IdStatoIscritto").Item(0).InnerText)
                        mDescrizione = xmlDoc.GetElementsByTagName("DescrizioneStatoIscritto").Item(0).InnerText
                        mErrore = False
                    ElseIf iRet = 0 Then
                        'listaMessaggi.aggiungi(New Messaggio(1, "Professione", ""))
                        mErrore = True
                    ElseIf iRet < 0 Then
                        'listaMessaggi.aggiungi(New Messaggio(1, "Professione", ""))
                        mErrore = True
                    End If

                End With

            Catch ex As Exception
                'listaMessaggi.aggiungi(New Messaggio(1, "Professione", ""))
                mErrore = True
            End Try

        End Sub

#End Region

    End Class

End Namespace

