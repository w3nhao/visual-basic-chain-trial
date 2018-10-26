'首先不能使用UINT64，因为溢出会报错
'VB的整型数字定义到数字除法会出现四舍五入，向下取整必须使用Int()函数


'   1.加减乘除四则运算
'   2.模运算
'   3.幂运算，幂模运算
'   4.拓展欧几里得算法求乘法逆元

Public Class BigInt 'Unfinished BigInt
    Public data As New List(Of UInt32) '大整数按 32bit 一个单元存储
    Public is_neg As Boolean = False
    Public Shared ZERO As UInt32 = 0
    Public Shared ONE As UInt32 = 1
    Public Shared TWO As UInt32 = 2
    Public Shared TEN As UInt32 = 10
    Public Shared NUM_ONE As UInt32 = 1
    Public Shared NUM_MINUSONE As UInt32 = "&H" & "FFFFFFFF"
    Public Shared BASE As Long = "&H" & "100000000"

    '输入字串须为十六进制字串
    Public Sub New(ByVal str As String)
        If Left(str, 1) = "-" AndAlso str.Length > 0 Then
            If str.Length > 1 Then
                is_neg = True
            End If
            str = str.Substring(1)
        End If

        Dim additional_zero As String
        For i As Integer = 0 To 7 - (str.Length Mod 8) '数长不为8的倍数,补足0
            additional_zero = additional_zero + "0"
        Next
        str = additional_zero + str

        For i As Integer = 0 To str.Length - 1 Step 8 '8位一个base
            Dim base As UInt32 = CLng("&H" & str.Substring(i, 8))
            data.Add(base)
        Next

        data.Reverse() '反转使高位在后
        trim()

    End Sub

    Public Sub New(ByRef bigint As BigInt)
        data.AddRange(bigint.data)
        is_neg = bigint.is_neg
    End Sub

    Public Sub New(ByRef val As UInt32)
        data.Add(val)
        is_neg = False
    End Sub

    '大整数的二进制信息
    Public Function getBitLen() As Integer
        Dim bit_len As Integer
        bit_len = (32 * data.Count) '粗略得出长度
        Dim tmp_top As UInt32 = data(data.Count - 1)

        If tmp_top = 0 Then '细致调整长度
            bit_len = bit_len - 32
        Else
            For i As Integer = 31 To 0 Step -1
                If (NUM_ONE << i) And tmp_top Then
                    Exit For
                Else
                    bit_len = bit_len - 1 '长度减一
                End If
            Next
        End If
        Return bit_len
    End Function

    '复制大数的值
    Public Sub copyVal(ByRef bigint As BigInt)
        data.Clear()
        data.AddRange(bigint.data)
    End Sub

    '去掉高位的0
    Public Sub trim()
        Do While data.Count > 1 AndAlso data(data.Count - 1) = 0
            data.RemoveAt(data.Count - 1)
        Loop
    End Sub

    '查询大整数的第 id 位 bit 为 0 还是 1
    Public Function at(ByVal id As Integer) As Boolean '检测大数的第id个二进制位为1还是0
        Dim index As Integer = id >> 5 '一个大整数位为 32bit = 2 ^ 5bit ,右移 5 位相当于除以 32 并取整
        Dim shift As Integer = id Mod 32 '即为 id & 0x002F 只取id的低5位, 相当于 id mod 32
        Dim tmp As UInt32 = data(index)
        Return (tmp And (NUM_ONE << shift))
    End Function

    '实现按二进制位左移
    Public Sub shiftLeftByBit(ByVal len As Integer)
        Dim index As Integer = len >> 5
        Dim shift As Integer = len Mod 32
        For i As Integer = 0 To index - 1
            data.Insert(0, 0)
        Next
        If shift <> 0 Then
            data.Add(0) '加多一位
            Dim temp As UInt32 = 0
            For i As Integer = 0 To data.Count - 1
                Dim tmp As UInt32 = data(i)
                data(i) = (tmp << shift) Or temp '整体左移后加上低一位大整数内的高位
                temp = (tmp And (NUM_MINUSONE << (32 - shift))) >> (32 - shift) '获取该大整数内的高位
            Next
        End If
        trim()
    End Sub
    '实现按二进制位右移
    Public Sub shiftRightByBit(ByVal len As Integer)
        Dim bit_len As Integer = getBitLen()
        If len >= bit_len Then
            data.Clear()
            data.Add(0)
        Else
            Dim index As Integer = len >> 5
            Dim shift As Integer = len Mod 32
            For i As Integer = 0 To index - 1
                data.RemoveAt(0)
            Next
            If shift <> 0 Then
                Dim temp As UInt32 = 0
                For i As Integer = data.Count - 1 To 0 Step -1
                    Dim tmp As UInt32 = data(i)
                    data(i) = (tmp >> shift) Or temp '整体右移后加上低一位大整数内的高位
                    temp = (tmp And (NUM_MINUSONE >> (32 - shift))) << (32 - shift) '获取该大整数内的低位
                Next
            End If
        End If
        trim()
    End Sub

    Public Function toString() As String
        Dim str As String

        data.Reverse()
        For Each i As UInt32 In data
            If i <> 0 Then
                Dim tmp_hex As String = CStr(Hex(i))
                For j As Integer = 0 To 7 - CStr(Hex(i)).Length
                    tmp_hex = "0" + tmp_hex
                Next
                str = str & tmp_hex
            Else
                str = str + "00000000"
            End If
        Next
        data.Reverse()

        Do While Left(str, 1) = "0" AndAlso str.Length > 0 '去掉高位没用的0
            str = str.TrimStart("0")
        Loop
        If str = Nothing Then
            str = "0"
        Else
            If is_neg = True Then
                str = "-" & str
            End If
        End If
        Return str
    End Function

    '返回大整数的绝对值
    Public Function abs() As BigInt
        Dim ans As New BigInt("0")
        ans.data.Clear()
        ans.data.AddRange(data)
        Return ans
    End Function

    '返回是否相等
    Public Function equals(ByVal val As UInt32) As Boolean
        Dim tmp As New BigInt(val)
        If data.Count = tmp.data.Count Then
            For i As Integer = 0 To data.Count - 1
                If data(i) <> tmp.data(i) Then
                    Return False
                End If
            Next
            Return True
        Else
            Return False
        End If
    End Function

    '实现比较大小, -1 本整数比较小、 0 表示相等、 1 表示本整数比较大
    Public Function compareTo(ByRef val As BigInt) As Integer
        If is_neg <> val.is_neg Then '符号不同，负数必小
            If is_neg = True Then
                Return -1
            Else
                Return 1
            End If
        End If

        Dim flag As Integer = 0
        If data.Count < val.data.Count Then
            flag = -1
        ElseIf data.Count > val.data.Count Then
            flag = 1
        Else
            For i As Integer = data.Count - 1 To 0 Step -1
                If data(i) > val.data(i) Then
                    flag = 1
                    Exit For
                End If
                If data(i) < val.data(i) Then
                    flag = -1
                    Exit For
                End If
            Next
        End If
        If is_neg = True Then
            flag = flag * (-1)
        End If
        Return flag
    End Function

    '实现相加
    Public Function add(ByRef val As BigInt) As BigInt
        If is_neg = val.is_neg Then
            Dim carry As Integer = 0 '设置进位
            Dim tmp_sum As Long '每一位的临时值 
            Dim a As Long '每一位的临时值 
            Dim b As Long '每一位的临时值 
            Dim m_len As Integer = data.Count - val.data.Count '哪个数短给哪个数高位补0,方便等下逐位运算
            If m_len > 0 Then
                Dim addend As New BigInt(val)
                Do While m_len > 0
                    m_len = m_len - 1
                    addend.data.Add(0)
                Loop
                For i As Integer = 0 To data.Count - 1
                    a = addend.data(i)
                    b = data(i)
                    tmp_sum = a + b + carry
                    carry = Int(tmp_sum / BASE)
                    tmp_sum = tmp_sum Mod BASE
                    data(i) = tmp_sum
                Next
            Else '为了节省运行内存，复制上面的代码
                Do While m_len < 0
                    m_len = m_len + 1
                    data.Add(0)
                Loop
                For i As Integer = 0 To data.Count - 1
                    a = val.data(i)
                    b = data(i)
                    tmp_sum = a + b + carry
                    carry = Int(tmp_sum / BASE)
                    tmp_sum = tmp_sum Mod BASE
                    data(i) = tmp_sum
                Next
            End If

            If carry = 1 Then '加到最后还有一个进位
                data.Add(1)
            End If
        Else '异号情况暂不考虑
            Dim a As BigInt = abs()
            Dim b As BigInt = val.abs()
            Dim flag As Integer = a.compareTo(b)
            If flag = -1 Then '绝对值相等结果为 0 ，否则用绝对值大的减去小的， 符号随绝对值大的
                copyVal(b.subtract(a))
                is_neg = val.is_neg
            ElseIf flag = 0 Then
                data.Clear()
                data.Add(0)
                is_neg = False
            Else
                copyVal(a.subtract(b))
            End If
        End If
        Return Me
    End Function


    '实现减法
    Public Function subtract(ByRef val As BigInt) As BigInt '减法不太常用，可以写的浪费内存一些
        Dim a As BigInt = abs()
        Dim b As BigInt = val.abs()
        If is_neg = val.is_neg Then '如果同号
            Dim flag As Integer = a.compareTo(b)
            If flag = 1 Then ' a 的绝对值大于 b 的绝对值，直接减
                Dim tmp_sum As Long '每一位的临时值
                Dim c As Long '每一位的临时值
                Dim d As Long '每一位的临时值
                Dim borrow As Integer = 0 '生成借位
                Dim attached_cnt As Integer = data.Count - val.data.Count
                For i As Integer = 0 To attached_cnt - 1
                    b.data.Add(0)
                Next
                For i As Integer = 0 To data.Count - 1
                    c = data(i)
                    d = b.data(i)
                    tmp_sum = c - d - borrow
                    If tmp_sum < 0 Then
                        borrow = 1
                    Else
                        borrow = 0
                    End If
                    tmp_sum = (tmp_sum + BASE) Mod BASE
                    data(i) = tmp_sum
                Next
                trim()
            ElseIf flag = 0 Then
                data.Clear()
                data.Add(0)
                is_neg = False
            Else ' a 的绝对值小于 b 的绝对值
                copyVal(b.subtract(a))
                is_neg = Not (is_neg)
            End If
        Else '如果异号
            copyVal(a.add(b))
        End If
        Return Me
    End Function

    '实现乘法
    Public Function multiply(ByRef val As BigInt) As BigInt
        If equals(ZERO) OrElse val.equals(ZERO) Then
            Return New BigInt("0")
        End If

        Dim ans As New BigInt("0")
        If data.Count >= val.data.Count Then '位数少的做乘数
            Dim tmp As New BigInt(Me) '被乘数
            tmp.is_neg = False
            Dim last_shift_pos As Integer = 0
            Dim bit_len As Integer = val.getBitLen()
            For i As Integer = 0 To bit_len - 1 '按乘数的二进制位左移
                If val.at(i) = True Then
                    tmp.shiftLeftByBit(i - last_shift_pos)
                    last_shift_pos = i
                    ans = ans.add(tmp)
                End If
            Next
        Else
            Dim tmp As New BigInt(val) '被乘数
            tmp.is_neg = False
            Dim last_shift_pos As Integer = 0
            Dim bit_len As Integer = getBitLen()
            For i As Integer = 0 To bit_len - 1 '按乘数的二进制位左移
                If at(i) = True Then
                    tmp.shiftLeftByBit(i - last_shift_pos)
                    last_shift_pos = i
                    ans = ans.add(tmp)
                End If
            Next
        End If
        If is_neg <> val.is_neg Then
            ans.is_neg = True
        End If
        Return ans
    End Function


    '实现除法
    Public Function divideAndReminder(ByVal val As BigInt, ByRef m As BigInt) As BigInt
        If val.equals(ZERO) Then
            MessageBox.Show("除数不能为0！", "Warning")
            Return Nothing
        End If
        Dim a As BigInt = abs()
        Dim b As BigInt = val.abs()
        Dim flag As Integer = a.compareTo(b)
        If flag = 0 Then
            If is_neg = val.is_neg Then
                Return New BigInt("1")
            Else
                Return New BigInt("-1")
            End If
        End If
        If flag = -1 Then
            m = Me
            Return New BigInt("0")
        End If

        Dim ans As New BigInt("0")
        Do While True 'a 的绝对值大于 b 的绝对值
            Dim len As Integer = a.getBitLen() - b.getBitLen()
            Dim tmp As BigInt
            Do While len >= 0
                tmp = New BigInt(b)
                tmp.shiftLeftByBit(len)
                If tmp.compareTo(a) <> 1 Then '找到最大的左移位数使得当前的a大于等于b
                    Exit Do
                End If
                len = len - 1
            Loop
            If len < 0 Then '当前的 a 小于 b 了
                Exit Do
            End If
            Dim num As UInt32 = 0
            Do While tmp.compareTo(a) <> 1
                a.subtract(tmp)
                num = num + 1 '统计当前的 a 最多大于等于几个移位后的 b
            Loop
            tmp = New BigInt(num)
            If len <> 0 Then
                tmp.shiftLeftByBit(len) '移位后表明当前的 a 是 b的几倍
            End If
            ans.add(tmp)
        Loop

        If is_neg = val.is_neg Then
            ans.is_neg = False
        Else
            ans.is_neg = True
        End If
        m.data.Clear()
        m.data.AddRange(a.data)
        m.is_neg = is_neg
        Return ans
    End Function

    '除法
    Public Function divide(ByRef val As BigInt) As BigInt
        Dim tmp As New BigInt("0")
        Dim ans As BigInt = divideAndReminder(val, tmp)
        Return ans
    End Function

    '取余
    Public Function remainder(ByRef val As BigInt) As BigInt
        Dim ans As New BigInt("0")
        divideAndReminder(val, ans)
        Return ans
    End Function

    '取模
    Public Function modify(ByRef m As BigInt) As BigInt
        Dim ans As BigInt = remainder(m)
        If ans.is_neg = True Then
            ans = ans.add(m)
        End If
        Return ans
    End Function

    '幂乘
    Public Function pow(ByRef exponent As BigInt) As BigInt
        Dim ans As New BigInt("1")
        Dim bit_len = exponent.getBitLen()
        For i As Integer = bit_len - 1 To 0 Step -1
            ans = ans.multiply(ans)
            If exponent.at(i) Then '快速幂
                ans = multiply(ans) '从高位开始，位权累加效应
            End If
        Next
        Return ans
    End Function

    '幂模
    Public Function modPow(ByRef exponent As BigInt, ByRef m As BigInt) As BigInt
        If m.equals(ZERO) Then
            MessageBox.Show("模数不能为0！", "Warning")
            Return Nothing
        End If
        Dim ans As New BigInt("1")
        Dim bit_len = exponent.getBitLen()
        For i As Integer = bit_len - 1 To 0 Step -1
            ans = ans.multiply(ans).modify(m)
            If exponent.at(i) Then '快速幂
                ans = multiply(ans).modify(m) '从高位开始，位权累加效应
            End If
        Next
        Return ans
    End Function

    'EXGCD
    Public Function modInverse(ByRef m As BigInt) As BigInt
        If is_neg = True OrElse m.is_neg = True Then

            Return Nothing
        End If

        If equals(ZERO) OrElse m.equals(ZERO) Then
            Return New BigInt("0")
        End If

        '初等变换
        Dim a As New List(Of BigInt)
        Dim b As New List(Of BigInt)
        Dim tmp As New List(Of BigInt)
        a.Add(New BigInt("0"))
        a.Add(New BigInt("1"))
        a.Add(New BigInt(Me))
        b.Add(New BigInt("1"))
        b.Add(New BigInt("0"))
        b.Add(New BigInt(m))
        tmp.Add(New BigInt("0"))
        tmp.Add(New BigInt("0"))
        tmp.Add(New BigInt("0"))

        tmp(2) = a(2).modify(b(2))
        Do While True
            If tmp(2).equals(ZERO) = True Then
                Exit Do
            End If
            Dim temp As BigInt = a(2).divide(b(2))
            For i As Integer = 0 To 2
                tmp(i) = a(i)
                tmp(i).subtract(temp.multiply(b(i)))
                a(i) = b(i)
                b(i) = tmp(i)
            Next
            tmp(2) = a(2).modify(b(2))
        Loop

        If b(2).equals(ONE) Then
            If b(1).is_neg Then
                b(1).add(m)
            End If
            Return b(1)
        End If
        Return New BigInt("0")
    End Function

    '''
    ''
    '重载运算
    Public Shared Operator +(ByVal a As BigInt, ByVal b As BigInt) As BigInt
        Return a.add(b)
    End Operator

    Public Shared Operator -(ByVal a As BigInt, ByVal b As BigInt) As BigInt
        Return a.subtract(b)
    End Operator

    Public Shared Operator *(ByVal a As BigInt, ByVal b As BigInt) As BigInt
        Return a.multiply(b)
    End Operator

    Public Shared Operator /(ByVal a As BigInt, ByVal b As BigInt) As BigInt
        Return a.divide(b)
    End Operator

    Public Shared Operator Mod(ByVal a As BigInt, ByVal b As BigInt) As BigInt
        Return a.modify(b)
    End Operator

End Class
