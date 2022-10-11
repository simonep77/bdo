Imports Business.Data.Objects.Core
Imports Business.Data.Objects.Core.Attributes
Imports Business.Data.Objects.Core.Base

Public MustInherit Class AziendaBizFactory
    Inherits BusinessObjFactory(Of AziendaBiz)



    Public Overrides Function Create(dalObj As DataObjectBase) As BusinessObjectBase
        Return New AziendaBiz2(dalObj)
    End Function


End Class
