Module OneDArrayFunction

    Function SumArray(arr() As Integer) As Integer
        Dim sum As Integer = 0

        For Each num As Integer In arr
            sum += num
        Next

        Return sum
    End Function

    Sub Main()

        Console.Write("Enter number of elements: ")
        Dim n As Integer = Convert.ToInt32(Console.ReadLine())

        Dim numbers(n - 1) As Integer ' Array name is "numbers"

        For i As Integer = 0 To n - 1
            Console.Write("Enter number " & (i + 1) & ": ")
            numbers(i) = Convert.ToInt32(Console.ReadLine())
        Next

        Dim result As Integer = SumArray(numbers)

        Console.WriteLine("Sum = " & result)

        Console.ReadLine()

    End Sub

End Module