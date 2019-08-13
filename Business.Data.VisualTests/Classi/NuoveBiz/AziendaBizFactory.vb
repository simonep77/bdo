Public Class AziendaBizFactory
    Inherits Bdo.Objects.BusinessObjFactory(Of AziendaBiz)



    Public Overrides Function Create(dalObj As Bdo.Objects.Base.DataObjectBase) As Bdo.Objects.Base.BusinessObjectBase
        Return dalObj.tobi AziendaBiz(dalObj)
    End Function
End Class
