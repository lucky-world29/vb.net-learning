'Console.OutputEncoding = System.Text.Encoding.UTF8
Module TwoDArray
    Sub Main()
        Console.OutputEncoding = System.Text.Encoding.UTF8
        'Dim matrix(,) As Integer = {
        '    {3, 5, 6},
        '    {4, 53, 34}
        '    }

        'For i As Integer = 0 To matrix.GetLength(0) - 1
        '    For j As Integer = 0 To matrix.GetLength(1) - 1
        '        Console.Write(matrix(i, j) & " ")
        '    Next
        '    Console.WriteLine()
        'Next
        Try
            Dim a As Integer = Convert.ToInt32(Console.ReadLine())
            Dim b As Integer = Convert.ToInt32(Console.ReadLine())
            Dim result As Integer = Sum(a, b)
            Console.WriteLine("The sum is 😁 " & result)
        Catch ex As Exception
            Console.WriteLine("Please enter a valid input here ")
            Console.WriteLine("Are u fucking blind ... 😒 ")

        End Try

    End Sub

    '========
    Function Sum(x As Integer, y As Integer) As Integer
        Dim res As Integer = x + y
        Return res
    End Function




End Module
