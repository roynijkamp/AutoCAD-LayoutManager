Imports Autodesk.AutoCAD.ApplicationServices
Imports AcApp = Autodesk.AutoCAD.ApplicationServices.Application


MustInherit Class clsDocData
    Private Shared m_docDataMap As System.Collections.Hashtable
    Private Shared Sub DocumentManager_DocumentToBeDestroyed(ByVal sender As Object, ByVal e As DocumentCollectionEventArgs)
        m_docDataMap.Remove(e.Document)
    End Sub
    Protected Delegate Function CreateFunctionType() As clsDocData
    Protected Shared CreateFunction As CreateFunctionType
    Public Shared ReadOnly Property Current() As clsDocData
        Get
            If m_docDataMap Is Nothing Then
                m_docDataMap = New System.Collections.Hashtable()
                AddHandler AcApp.DocumentManager.DocumentToBeDestroyed,
                AddressOf DocumentManager_DocumentToBeDestroyed
            End If
            Dim active As Document = AcApp.DocumentManager.MdiActiveDocument
            If Not m_docDataMap.ContainsKey(active) Then
                m_docDataMap.Add(active, CreateFunction())
            End If
            Return DirectCast(m_docDataMap(active), clsDocData)
        End Get
    End Property
End Class

Class clsMyDocData
    Inherits clsDocData
    Private m_stuff As String
    Shared Sub New()
        CreateFunction = New CreateFunctionType(AddressOf Create)
    End Sub
    Public Sub New()
        m_stuff = AcApp.DocumentManager.MdiActiveDocument.Window.Text
    End Sub
    Protected Shared Function Create() As clsDocData
        Return New clsMyDocData()
    End Function
    Public Property Stuff() As String
        Get
            Return m_stuff
        End Get
        Set(ByVal value As String)
            m_stuff = value
        End Set
    End Property
    Public Shared Shadows ReadOnly Property Current() As clsMyDocData
        Get
            Return DirectCast(clsDocData.Current, clsMyDocData)
        End Get
    End Property
End Class

