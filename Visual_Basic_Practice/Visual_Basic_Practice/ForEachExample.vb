Module ForEachExample
    Sub Main()
        Dim fruits() As String = {"Apple", "Orange", "Pinaple"}

        For Each items As String In fruits
            Console.WriteLine(items)
        Next


    End Sub
End Module
