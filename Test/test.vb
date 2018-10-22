'question1:问一下算法的过程
'question2:目前是否可以以单机多线程来模拟？
'question3:对于字符串流的共识，如何达到？
'question4:将军结点是叛徒的话，它本身需要达到共识吗？

'事件后填表
If e.DataCol.Name = "G" Then
    If e.NewValue = 1 Then
        Dim dr As DataRow
        dr = e.DataTable.DataRows(1)
        For Each dc As DataCol In e.DataTable.DataCols
            If dc.Name <> "G" Then
                dr(dc.Name) = 1
            End If
        Next
    End If
End If
            
'自定义类示例
Public Class XXX

    '计时器对象
    Private WithEvents _CtsTmr As System.Windows.Forms.Timer

    '要执行的自定义函数名称
    '用自定义函数,可以避免把类写死,增加扩展性
    Private _FunName As String

    '参数对象数组,
    '用对象数组,可以适用不同的场合,便于传递
    Private _obj() As Object

    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="funName">要执行函数的名称</param>
    ''' <param name="obj">函数的参数</param>
    ''' <remarks></remarks>
    Sub New(ByVal funName As String, Optional ByVal obj() As Object = Nothing)
        _CtsTmr = New System.Windows.Forms.Timer '新建对象
        _CtsTmr.Interval = 500       '默认的间隔
        _FunName = funName '确定函数名称
        _obj = obj '确定参数集
    End Sub

    ''' <summary>
    ''' 计时器事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CtsTmr_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles _CtsTmr.Tick
        '打包自定义函数的参数
        '次序为:
        'sender
        'e
        '……
        Dim o() As Object = {sender, e}                 '固定的参数
        If _obj IsNot Nothing Then                      '如果还用参数
            Dim i As Integer = _obj.Length + 2          'o 是固定长度,就直接赋值了
            ReDim Preserve o(i - 1)                     '重新定义数组长度
            _obj.CopyTo(o, 2)                           '将参数数组复制到o数组
        End If
        '调用自定义函数,传递参数
        Functions.Execute(_FunName, o)         '调用自定义函数
    End Sub

    '返回计时器,用于各种设置
    Public ReadOnly Property CtsTmr As System.Windows.Forms.Timer
        Get
            Return _CtsTmr
        End Get
    End Property

End Class

'自定义函数内
Dim sender As Object = Args(0)  
Dim e As System.EventArgs = Args(1)  '前两个是默认的
Dim L As WinForm.Label = Args(2)    '第三个我们传递一个Label

L.Text = val(L.Text) + 1

'调用类示例
Dim a As XXX = Vars(e.Form.Name & "_Timer")

If  a IsNot Nothing Then
    a.CtsTmr.Start()
    
End If

Dim a As XXX = Vars(e.Form.Name & "_Timer")

If  a IsNot Nothing Then
    a.CtsTmr.Stop
    
End If


'结构体语句
Public Structure employee
    ' Public members, accessible from throughout declaration region.
    Public firstName As String
    Public middleName As String
    Public lastName As String
    ' Friend members, accessible from anywhere within the same assembly.
    Friend employeeNumber As Integer
    Friend workPhone As Long
    ' Private members, accessible only from within the structure itself.
    Private homePhone As Long
    Private level As Integer
    Private salary As Double
    Private bonus As Double
    ' Procedure member, which can access structure's private members.
    Friend Sub calculateBonus(ByVal rate As Single)
        bonus = salary * CDbl(rate)
    End Sub
    ' Property member to return employee's eligibility.
    Friend ReadOnly Property eligible() As Boolean
        Get
            Return level >= 25
        End Get
    End Property
    ' Event member, raised when business phone number has changed.
    Public Event changedWorkPhone(ByVal newPhone As Long)
End Structure


    Public ReadOnly Property GetDecisions As List(Of String)
        Get
            return decisions
        End Get
    End Property


'''
'''
'测试类代码
Dim b As New Node("1","0")
Dim a As New Traits(1,1,1,b)
Output.Show(a.mSource)

Dim m As New Dictionary(Of Integer, Dictionary(Of Integer, List(Of String)))
Dim a As New Dictionary(Of Integer, List(Of String))
Dim b As New List(Of String)
b.Add("mmm")
b.Add("mmm")
b.Add("mmm")
a.Add(0, b)
m.Add(0, a)
Output.Show(m(0)(0)(1))

If m.ContainsKey(0) = False Then '向量空间不存在的话需要新建一个
    Dim tmp1 As New Dictionary(Of Integer, List(Of String))
    m.Add(0, tmp1)
    Dim tmp2 As New List(Of String)
    m(0).Add(0, tmp2)
Else 
    If m(0).ContainsKey(0) = False Then
        Dim tmp2 As New List(Of String)
        m(0).Add(0, tmp2)
    End If
End If

Output.Show(m(0).ContainsKey(0))

Dim m As New Dictionary(Of Integer, Dictionary(Of Integer, List(Of String)))
Dim a As New Dictionary(Of Integer, List(Of String))
Dim b As New List(Of String)

If m.ContainsKey(0) = False Then '向量空间不存在的话需要新建一个
    Dim tmp1 As New Dictionary(Of Integer, List(Of String))
    m.Add(0, tmp1)
    Dim tmp2 As New List(Of String)
    m(0).Add(0, tmp2)
Else 
    If m(0).ContainsKey(0) = False Then
        Dim tmp2 As New List(Of String)
        m(0).Add(0, tmp2)
    End If
End If
Output.Show(m(0).ContainsKey(0))


Dim mChildren As New Dictionary(Of String, List(Of String)) 'map<CurrentNodePath, vector<NextNodePath>> 记录结点与其子节点的关系
Dim Shared mPathsByRank As New Dictionary(Of Integer, Dictionary(Of Integer, List(Of String))) 'map<Rank, map<SourceProcessId, vector<SendingNodePath>>> 记录每一层的所有结点

Dim tmp As New List(Of Boolean)
For i As Integer = 0 To N - 1
    tmp.Add(True)
Next
GenerateChildren(M, N, tmp, SOURCE, "", 0)


Public Sub GC(ByVal m As Integer, ByVal n As Integer, ByVal ids As List(Of Boolean), ByVal source As Integer, ByVal cur_path As String, ByVal rank As Integer)
        ids(source) = False
        cur_path += Cstr(source)
        If mPathsByRank.ContainsKey(rank) = False Then '向量空间不存在的话需要新建一个
            Dim tmp1 As New Dictionary(Of Integer, List(Of String))
            mPathsByRank.Add(rank, tmp1)
            Dim tmp2 As New List(Of String)
            mPathsByRank(rank).Add(source, tmp2)
        Else 
            If mPathsByRank(rank).ContainsKey(source) = False Then
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
                        Dim tmp As List(Of String)
                        mChildren.Add(cur_path, tmp)
                    End If
                    mChildren(cur_path).Add(cur_path + Cstr(i))
                End If
            Next
        End If
    End Sub

    Dim tmp As New List(Of Boolean)
For i As Integer = 0 To 7
    tmp.Add(True)
Next
mPathsByRank

GC(2, 7, tmp, 1, "", 0)


Dim str As String = "你我"
Dim hex As String = ""
For i As Integer = 0 To str.Length - 1
    hex = hex + Asc(str.Chars(i))
Next

Dim str As String = "你我"

For i As Integer = 0 To str.Length - 1
    Output.Show(str.Chars(i))
Next



Output.Show(str)
Output.Show(hex)

Dim a As String = "1234"

Output.Show(dec2hex(a))
Output.Show(dec2hex1(a))
Output.Show(dec2hex2(a))


'接口必须重写
Public Functions 


'''
'本来想直接调用别人写好的 C++ 文件

Public Declare auto Function dec2hex2 Lib "Dll2.dll" Alias "dec2hex2" (ByVal dec As Char()) As String()
Public Declare auto Function dec2hex1 Lib "Dll2.dll" Alias "dec2hex1" (ByVal dec As String()) As String()
Public Declare auto Function dec2hex Lib "Dll2.dll" Alias "dec2hex" (ByVal dec As String()) As String()
'''
'https://www.baidu.com/s?ie=UTF-8&wd=%E5%B0%BD%E9%87%8F%E4%B8%8D%E8%A6%81%E5%9C%A8Dll%E7%9A%84%E6%8E%A5%E5%8F%A3%E4%B8%AD%E4%BD%BF%E7%94%A8string%E4%BD%9C%E4%B8%BA%E5%8F%82%E6%95%B0
'接口必须重写
Public Function str2chars(ByVal str As String) As Char()
    Dim tmp_list As New List(Of Char)
    For i As Integer = 0 To str.Length - 1
        tmp_list.Add(str.Chars(i))
    Next
    Dim tmp_val() As Char = tmp_list.ToArray
    Return tmp_val
End Function
'''写到一半不想写了，CLS 对应不上数据结构，连CHAR数组也不知道如何转接
'''
Dim a As New UFBInt("11111111111111111")
Dim b As New UFBInt("11111111111111112")
Dim c As New UFBInt("11111111111111111")
Output.Show(a.compareTo(b))

Output.Show(b.compareTo(a))
Output.Show(c.compareTo(a))

Dim str As String = "SAASSAASJNJJNJNJNJNJNAKNKSANK"
str = str2hex(str)
Dim a As New UFBInt(str)
Dim b As Long = CLng( "&H" & 53414153)
Dim c As String = hex(b)

b = b And (-1 << 28)
b = b >> 28

Output.Show(a.data(a.data.Count - 1))
Output.Show(b)
Output.Show(c)

Output.Show(str)
Output.Show(hex2str(str))
Dim k As String = a.tostring()
Output.Show(k)
k = hex2str(k)
Output.Show(k)


Dim a As New UFBInt("2")
Dim b As New UFBInt("1")
Dim c As UFBInt
c = a.modify(b)
Output.Show(c.Tostring())

Dim a As New UFBInt("22222")
Dim b As New UFBInt("22222222222222222222222222222")
Dim c As UFBInt
Output.Show(a.bit_len)
a.at(0)


Dim a As New UFBInt("1")
Output.Show(a.at(32))

Dim a As New UFBInt("1")
Dim b As New UFBInt("1111111111111111111111")
Dim c As UFBInt
c = a.add(b)
Output.Show(c.Tostring())


Dim a As New UFBInt("1")
Dim b As New UFBInt("3")
Dim c As UFBInt
c = b.abs()
Output.Show(a.compareTo(b))

Dim a As New UFBInt("222222220")
Dim b As New UFBInt("4")


Dim a As New UFBInt("7")
Dim b As New UFBInt("96")
Output.Show(a.modInverse(b).toString())

Dim a As New UFBInt("0")
a = RSA.createOddNum(36)
Output.Show(a.Tostring())
a = RSA.createRandomSmaller(a)

Dim a As New UFBInt("10000")
a.shiftrightbybit(10)
Output.Show(a.Tostring())

For i As Integer  = 1 To 100
    Dim a As New UFBInt(hex(i))
    Output.Show(Cstr(RSA.Isprime(a, 500)) & Cstr(i))
Next