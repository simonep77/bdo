Namespace VecchieClassi

    Public Class BaseClass

        Private mDataSet As DataSet
        Private mCurrentSlot As BusinessSlot

        Public Property currentSlot() As BusinessSlot
            Get
                Return mCurrentSlot
            End Get
            Set(ByVal Value As BusinessSlot)
                mCurrentSlot = Value
            End Set
        End Property

        Public Property localDataSet() As DataSet
            Get
                Return mDataSet
            End Get
            Set(ByVal Value As DataSet)
                mDataSet = Value
            End Set
        End Property

        Public Sub New()
            mDataSet = New DataSet
        End Sub

    End Class
End Namespace