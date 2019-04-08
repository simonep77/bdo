

Imports System.Collections

Namespace VecchieClassi
    ''' <summary>
    ''' Classe elenco di ultima generazione
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class BaseElencoNew
        Inherits Paginazione
        Implements IEnumerable


        Private mLista As ArrayList = New ArrayList()


#Region "PROPRIETA'"

        ''' <summary>
        ''' Accesso per indice
        ''' </summary>
        ''' <param name="index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public ReadOnly Property Item(ByVal index As Integer) As BaseClass
            Get
                Return Me.mLista(index)
            End Get
        End Property

        ''' <summary>
        ''' Numero elementi
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Count() As Integer
            Get
                Return Me.mLista.Count
            End Get
        End Property
#End Region


#Region "COSTRUTTORI"

        Public Sub New(ByVal slotIn As BusinessSlot)
            Me.currentSlot = slotIn
        End Sub

#End Region

#Region "METODI PUBBLICI"

        ''' <summary>
        ''' Svuota lista
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Clear()
            Me.mLista.Clear()
        End Sub

#End Region


#Region "METODI PRIVATI"

        ''' <summary>
        ''' Deve essere implementato da qualsiasi classe derivata
        ''' </summary>
        ''' <remarks></remarks>
        Protected MustOverride Sub impostaAttributi()

        Protected Sub AddToLista(ByVal oggettoIn As Object)
            Me.mLista.Add(oggettoIn)
        End Sub

        ''' <summary>
        ''' Esegue la query impostata nel businessslot, paginata se necessario (pagina > 0)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub eseguiRicerca()
            Dim qRet As Integer = 0

            'Resetta liste dati interne
            Me.localDataSet.Reset()
            Me.mLista.Clear()

            'Esegue
            If Me.Pagina <= 0 Then
                qRet = Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet)
            Else
                qRet = Me.currentSlot.dbConnection.OpenQuery(Me.localDataSet, Me.Posizione, Me.Offset, Me.TotRecord)
            End If

            If qRet < 0 Then
                'Errore
                Throw New EFaschimError("Errore query: {0}", Me.currentSlot.dbConnection.StrErr)
            ElseIf qRet > 0 Then
                'Trovati dati, chiama routine di impostazione
                Me.impostaAttributi()
            End If

        End Sub

#End Region


        ''' <summary>
        ''' Implementazione dell' enumeratore
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return Me.mLista.GetEnumerator()
        End Function
    End Class

End Namespace