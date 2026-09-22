Module DoUntilExample
    Sub Main()
        Dim i As Integer = 1

        Do Until i > 5
            Console.WriteLine(i)
            i += 1
            'i++   No possible ra
        Loop
    End Sub
End Module
