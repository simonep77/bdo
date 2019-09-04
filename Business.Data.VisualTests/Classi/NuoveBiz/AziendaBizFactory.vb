Public Class AziendaBizFactory
    Inherits Bdo.Objects.BusinessObjFactory(Of AziendaBiz)



    Public Overrides Function Create(dalObj As Bdo.Objects.Base.DataObjectBase) As Bdo.Objects.Base.BusinessObjectBase
        Return New AziendaBiz2(dalObj)
    End Function
End Class
