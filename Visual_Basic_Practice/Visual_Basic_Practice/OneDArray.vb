Module OneDArray
    Sub Main()
        OneDArray.Arr_Str()
        OneDArray.Arr_Int()
        OneDArray.Exception_Bla()
    End Sub

    Sub Arr_Str()
        Dim arr() As String = {"A", "E", "I", "O", "U"}

        For i As Integer = 0 To arr.Length - 1
            Console.Write(arr(i) + " ")
        Next
        Console.WriteLine()
    End Sub

    Sub Arr_Int()
        Dim arr() As Integer = {1, 4, 5, 6, 7}
        Dim sum As Integer = 0
        For i As Integer = 0 To arr.Length - 1
            sum += i
        Next
        Console.WriteLine("The sum of the array is " & sum)
    End Sub

    Sub Exception_Bla()
        Try
            Dim n As Integer = 0
            Dim division As Integer = 4 / n
            Console.WriteLine(division)
        Catch ex As Exception
            Console.WriteLine(" USER MESSAGE = > The problem is the kusa (in Odia) is zero")
            Console.WriteLine("System Message = > " & ex.Message())
        End Try
    End Sub
End Module
