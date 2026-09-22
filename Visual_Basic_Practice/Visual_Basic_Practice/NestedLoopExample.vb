Module NestedLoopExample
    Sub Main()
        For i As Integer = 1 To 5
            For j As Integer = i To 5
                Console.Write(j)
            Next
            Console.WriteLine()
        Next
    End Sub
End Module
