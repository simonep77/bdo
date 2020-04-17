Imports Bdo.Objects.Base

Public MustInherit Class AziendaBizFactory
    Inherits Bdo.Objects.BusinessObjFactory(Of AziendaBiz)



    Public Overrides Function Create(dalObj As DataObjectBase) As BusinessObjectBase
        Return New AziendaBiz2(dalObj)
    End Function


End Class
