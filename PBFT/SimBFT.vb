'仍需实现：随意发送字符串
'单元格变化判别源进程
'读取用户数得出进程数目
'用户隔离
'


'全局定义
'注意：若源进程非坏进程，达到共识的所有进程决策均与源进程一致。否则其他进程的共识与源进程不一致。

Public ONE As String = "1"
Public ZERO As String = "0"
Public UNKNOWN As String = "?" '单一用其代替

'决策树所使用的信息结点
Public Class Node
    Public input_value As String
    Public output_value As String

    Sub New(ByVal input As String, ByVal output As String)
        input_value = input
        output_value = output
    End Sub

End Class

'将其他信息快速转换为源信息结点
Public Function CSrcNode(ByVal input As Object) As Node
    return New Node(Cstr(input), UNKNOWN)
End Function

'定义整个拓扑特征
Public Class Traits
    Public mSource As Integer
    Public mM As Integer
    Public mN As Integer
    Public mSrcVal As Node
    Public mFaultyProcesses As New List(Of Integer) 

    Sub New(ByVal source As Integer, ByVal m As Integer, ByVal n As Integer, ByVal srcval As Node)
        mSource = source
        mM = m
        mN = n
        mSrcVal = srcval
        SetFaultyNode(m, n)
    End Sub

    Public Sub SetFaultyNode(ByVal m As Integer, ByVal n As Integer) '随机设置坏进程
        If mFaultyProcesses IsNot Nothing Then
            mFaultyProcesses.Clear
        End If
        For i As Integer = 1 To m
            Dim j As Integer = Rand.Next(0, n)
            Do While mFaultyProcesses.Contains(j) = True '检测是否已经存在这个坏进程
                j = Rand.Next(0, n) '若该进程已为坏进程，则再随机设置数值
            Loop
            mFaultyProcesses.Add(j)
        Next
    End Sub

    '获取前一轮信息的行为模式,可以更改
    Public Function GetValue(ByVal value As String, ByVal source As Integer) As String
        If mFaultyProcesses.Contains(source) = False Then
            Return value
        Else
            Return Cstr(Rand.Next(2)) '随机发0或1
        End If
    End Function

    '取任意值以打破0与1信息数量相同的情况
    Public Function GetDefault() As String
        Return ONE
    End Function

End Class

Public Class Process
    Private mId As Integer '每个进程的ID
    Private mNodes As New Dictionary(Of String, Node) 'map<Path, Node> 存储树中所有的具体结点
    Private Shared mTraits As Traits
    Private Shared mChildren As New Dictionary(Of String, List(Of String)) 'map<CurrentNodePath, vector<NextNodePath>> 记录结点与其子节点的关系
    Private Shared mPathsByRank As New Dictionary(Of Integer, Dictionary(Of Integer, List(Of String))) 'map<Rank, map<SourceProcessId, vector<SendingNodePath>>> 记录每一层的所有结点

    Private Sub GenerateChildren(ByVal m As Integer, ByVal n As Integer, ByVal ids As List(Of Boolean), ByVal source As Integer, ByVal cur_path As String, ByVal rank As Integer)
        ids(source) = False
        cur_path += Cstr(source)
        If mPathsByRank.ContainsKey(rank) = False Then '先检测是否存在以防止非法访问
            Dim tmp1 As New Dictionary(Of Integer, List(Of String))
            mPathsByRank.Add(rank, tmp1)
            Dim tmp2 As New List(Of String)
            mPathsByRank(rank).Add(source, tmp2)
        Else 
            If mPathsByRank(rank).ContainsKey(source) = False Then '先检测是否存在以防止非法访问
                Dim tmp2 As New List(Of String)
                mPathsByRank(rank).Add(source, tmp2)
            End If
        End If
        mPathsByRank(rank)(source).Add(cur_path)
        If rank < m Then
            For i As Integer = 0 To ids.Count - 1
                If ids(i) = True Then
                    GenerateChildren(m, n, ids, i, cur_path, rank + 1)
                    If mChildren.ContainsKey(cur_path) = False Then
                        Dim tmp As New List(Of String)
                        mChildren.Add(cur_path, tmp)
                    End If
                    mChildren(cur_path).Add(cur_path + Cstr(i))
                End If
            Next
        End If
        ids(source) = True '引用的ids不知道什么原因在整个递归中成为了静态变量,返回时要把id调回true
    End Sub

    Private Sub ReceiveMessage(ByRef path As String, ByVal node As Node)
        mNodes(path) = node
    End Sub

    Private Function GetMajority(ByRef path As String) As String
        Dim counts As New Dictionary(Of String, Integer)
        counts.Add(ONE, 0)
        counts.Add(ZERO, 0)
        counts.Add(UNKNOWN, 0)
        If mChildren.ContainsKey(path) Then    '先检测是否存在以防止非法访问
            For Each child As String In mChildren(path)
                If mNodes.ContainsKey(child) Then    '先检测是否存在以防止非法访问
                    If mNodes(child).output_value IsNot Nothing Then
                    counts(mNodes(child).output_value) = counts(mNodes(child).output_value) + 1 '直接自增不符合语法,只好写得这么复杂
                    End If
                End If
            Next
            If counts(ONE) > (mChildren(path).Count / 2) Then
                Return ONE
            End If
            If counts(ZERO) > (mChildren(path).Count / 2) Then
                Return ZERO
            End If
            If counts(ONE) = counts(ZERO) AndAlso counts(ONE) = (mChildren(path).Count / 2) Then '统计时若1与0数量相等，返回默认值
                Return mTraits.GetDefault()
            End If
        End If
    End Function

    Sub New(ByVal id As Integer, ByVal source As Integer, ByVal m As Integer, ByVal n As Integer, ByVal srcval As Node)
        mId = id
        If mTraits Is Nothing Then
            mTraits = New Traits(source, m, n, srcval)
        End If
        If mChildren.Count = 0 Then
            Dim tmp As New List(Of Boolean)
            For i As Integer = 0 To mTraits.mN - 1
                tmp.Add(True)
            Next
            GenerateChildren(mTraits.mM, mTraits.mN, tmp, mTraits.mSource, "", 0)
        End If
        If mId = mTraits.mSource Then
            mNodes.Add("", mTraits.mSrcVal)
        End If
    End Sub

    Public Sub SendMessage(ByVal r0und As Integer, ByRef processes As List(Of Process))
        If mPathsByRank(r0und).ContainsKey(mId) Then '若该轮存在mId号进程发出的信息
            For Each src_node_next_path As String In mPathsByRank(r0und)(mId)
                If mNodes.ContainsKey(src_node_next_path.SubString(0, src_node_next_path.Length - 1)) Then '若结点树中存在该路径相关的结点
                    Dim src_node = mNodes((src_node_next_path.SubString(0, src_node_next_path.Length - 1))) '得出发信结点信息
                    For Each a_process As Process In processes  
                       If processes.IndexOf(a_process) <> mTraits.mSource Then '如果不是源进程则收信
                           Dim tmp_node As New Node(mTraits.GetValue(src_node.input_value, mId), UNKNOWN)
                           a_process.ReceiveMessage(src_node_next_path, tmp_node)
                       End If
                   Next
                End If
            Next
        End If
    End Sub

    Public Function Decide() As String
        If mId = mTraits.mSource Then '若是将军
            If mTraits.mFaultyProcesses.Contains(mId) = False Then '将军不是坏进程
                return mNodes("").input_value
            Else
                return mNodes("").output_value
            End If
        End If
        If mTraits.mFaultyProcesses.Contains(mId) = False Then '若不是坏进程
            For i As Integer = 0 To mTraits.mN - 1 '最后一层直接把输入值作为输出值
                If mPathsByRank(mTraits.mM).ContainsKey(i) Then '先检测是否存在以防止非法访问
                    For Each path As String In mPathsByRank(mTraits.mM)(i)
                        If mNodes.ContainsKey(path) Then '先检测是否存在以防止非法访问
                            mNodes(path).output_value = mNodes(path).input_value
                        End If
                    Next
                End If
            Next
            For r0und As Integer = mTraits.mM - 1 To 0 Step -1
                For i As Integer = 0 To mTraits.mN - 1 
                    If mPathsByRank(r0und).ContainsKey(i) Then '先检测是否存在以防止非法访问
                        For Each path As String In mPathsByRank(r0und)(i)
                            If mNodes.ContainsKey(path) Then '先检测是否存在以防止非法访问
                                mNodes(path).output_value = GetMajority(path)
                            End If
                        Next
                    End If
                Next
            Next
        End If
        return mNodes(mPathsByRank(0)(mTraits.mSource)(0)).output_value
    End Function

End Class

    

Public Function SimBFT(ByVal source As Integer, ByVal m As Integer, ByVal n As Integer,Optional ByVal srcval As String = "1") As List(Of String)
    
    Dim processes As New List(Of Process)
    Dim decisions As New List(Of String) 'vector<desicions> 存储所有的决策
    Dim srcval_node As Node '定义源信息结点
    srcval_node = CSrcNode(srcval) '将源信息转换为结点
    For i As Integer = 0 To n - 1
        Dim tmp_process As New Process(i, source, m, n, srcval_node)
        processes.Add(tmp_process)
    Next
    For i As Integer = 0 To m '一共m轮
        For Each a_process As Process In processes '给别的进程发信
            a_process.SendMessage(i, processes)
        Next
    Next

    For Each a_process As Process In processes
        decisions.Add(a_process.Decide())
    Next
    return decisions
End Function

