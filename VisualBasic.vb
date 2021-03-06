Sub Main()
    Application.ScreenUpdating = False
    Application.DisplayAlerts = False
    Application.DisplayStatusBar = False
    Application.Calculation = xlCalculationManual
    Application.EnableEvents = False
    
    CopyData
    Sheets(1).Name = "4"
    Copy
    ActiveSheet.Name = "2"
    CoursesCompleted
    Sheets(3).Delete 'Pivot Table
    AverageGradeStudent
    Sheets(3).Delete 'Pivot Table2
    AverageGradeCourse
    Copy
    ActiveSheet.Name = "3"
    CompletionDates
    Sheets(4).Delete 'Pivot Table3
    Part1Part2
    ScoreVsCompletion

    Sheets("PT4").Move After:=Sheets(Sheets.Count)
    Sheets(9).Delete 'Only Complete
    Sheets(4).Delete 'Parts
    Sheets(2).Delete 'Pivot Table 5
    Sheets(1).Activate
    
    Application.ScreenUpdating = True
    Application.DisplayStatusBar = True
    Application.Calculation = xlCalculationAutomatic
    Application.EnableEvents = True
End Sub
Sub CopyData()
    'Data from workbook is copied into another workbook
    'New workbook is saved into same directory as original
    Dim x As Workbook
    Dim y As Workbook
    
    Set x = Workbooks.Open("C:\Users\samantha.feng\Documents\Sam Feng\Docebo Reports\Copy of Nick_custom_July_14_2016_2016_07_14_18_27.xls")
    Set y = Workbooks.Add
        With y
            .Title = "1"
            .SaveAs Filename:=x.Path & "\Report.xls"
        End With
    
    With x.Sheets(1).UsedRange
        y.Sheets(1).Range("A1").Resize( _
            .Rows.Count, .Columns.Count) = .Value
    End With
    Set y = Nothing
    x.Close
    Set x = Nothing
End Sub
Sub AddPhoto()
    'Add LOGiQ3 logo to a chart
    ActiveSheet.Shapes.AddPicture Filename:="C:\Users\samantha.feng\Documents\Sam Feng\LOGiQ3 Logo.png", linktofile:=msoFalse, _
        savewithdocument:=msoCTrue, Left:=1, Top:=450, Width:=100, Height:=37.7
End Sub
Sub AddPhotoNice()
    'Add LOGiQ3 logo to a chart
    ActiveSheet.Shapes.AddPicture Filename:="C:\Users\samantha.feng\Documents\Sam Feng\Logiq3_logo_500px.png", linktofile:=msoFalse, _
        savewithdocument:=msoCTrue, Left:=1, Top:=440, Width:=130, Height:=45
End Sub
Sub LegalPhoto()
    'Add LOGiQ3 logo to a chart with legal sized paper
    Dim logo As Shape
    Set logo = ActiveSheet.Shapes.AddPicture(Filename:="C:\Users\samantha.feng\Documents\Sam Feng\LOGiQ3 Logo.png", linktofile:=msoFalse, _
        savewithdocument:=msoCTrue, Left:=1, Top:=450, Width:=78.6, Height:=37.7)
    With logo
        .LockAspectRatio = msoTrue
    End With
    Set logo = Nothing
End Sub
Sub Timestamp()
    'Add timestamp to a chart
    Dim text As Shape
    Set text = ActiveChart.Shapes.AddTextbox(msoTextOrientationHorizontal, 3, _
        0, 172, 28)
    With text
        With .TextFrame.Characters
            .text = "Date Generated: " & Date
            .Font.Size = 11: .Font.Bold = True: .Font.Color = RGB(115, 123, 127)
        End With
    End With
    Set text = Nothing
End Sub
Sub Part1Part2()
    'Displays comparison between part 1 marks and part 2 marks per student
    Sheets(1).Name = "Parts"
    Sheets(1).Activate
    OnlyComplete
        
    Dim N As Long
    Dim temp As String
    
    N = Cells(Rows.Count, 1).End(xlUp).Row
    For i = 2 To N
        If Right(Cells(i, "D"), 3) = "CSJ" Then
            Range("O" & i).Value = "Part 2"
            temp = Cells(i, "D")
            temp = Left(temp, Len(temp) - 3)
            Cells(i, "D").Value = temp
        Else: Range("O" & i).Value = "Part 1"
        End If
    Next i
    Range("O1").Value = "Part"
    
    Sheets.Add
    ActiveSheet.Name = "PT4"
    
    ActiveWorkbook.PivotCaches. _
        Create( _
        SourceType:=xlDatabase, _
        SourceData:=Sheets("Parts").Range("A1:O" & Rows.Count)). _
            CreatePivotTable _
            TableDestination:=Worksheets("PT4").Range("A1"), _
            TableName:="PivotTable4"
            
    Set pivot = ActiveSheet.PivotTables("PivotTable4")
    With pivot
        .PivotFields("Name").Orientation = xlRowField
        .PivotFields("Part").Orientation = xlColumnField
        .AddDataField Sheets("PT4").PivotTables("PivotTable4").PivotFields("Final score"), "Average of Final score", xlAverage
        .ColumnGrand = False
        .RowGrand = False
        .PivotFields("Name").PivotItems("(blank)").Visible = False
    End With
    Set pivot = Nothing
    
    Dim lastRow As Long
    lastRow = Range("B" & Rows.Count).End(xlUp).Row
    
    Dim cht As Chart
    Set cht = Charts.Add
    With cht
        .Name = "PartsChart"
        .ChartType = xlLineMarkers
        .SetSourceData Source:=Sheets("PT4").Range("A1:D" & lastRow), _
            PlotBy:=xlColumns
        .HasTitle = True: .HasLegend = True
        .ChartTitle.text = "Average Score by Part"
        .ShowAllFieldButtons = False
        With .FullSeriesCollection(1).Format
            .Line.Visible = msoFalse
            .Fill.ForeColor.RGB = RGB(205, 58, 49)
        End With
        With .FullSeriesCollection(2).Format
            .Line.Visible = msoFalse
            .Fill.ForeColor.RGB = RGB(111, 201, 196)
        End With
        With .Axes(xlCategory, xlPrimary)
            .HasTitle = True
            .AxisTitle.Characters.text = "Name"
            .TickLabels.Font.Color = RGB(115, 123, 127)
            .AxisTitle.Font.Size = 16
        End With
        With .Axes(xlValue, xlPrimary)
            .HasTitle = True: .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Average of Final Score"
            .MinimumScale = 0: .MaximumScale = 100
        End With
    End With
    Set cht = Nothing
    
    AddPhoto
    Timestamp
End Sub
Sub ScoreVsCompletion()
    'Displays chart that shows average final score vs courses completed
    Sheets.Add
    ActiveSheet.Name = "Pivot Table5"
    
    ActiveWorkbook.PivotCaches. _
        Create( _
        SourceType:=xlDatabase, _
        SourceData:=Sheets("Only Complete").Range("A1:N" & Rows.Count)). _
            CreatePivotTable _
            TableDestination:=Worksheets("Pivot Table5").Range("A1"), _
            TableName:="PivotTable5"

    Set pivot = Sheets("Pivot Table5").PivotTables("PivotTable5")
    With pivot
        .PivotFields("Name").Orientation = xlRowField
        .AddDataField Sheets("Pivot Table5").PivotTables("PivotTable5"). _
            PivotFields("Final score"), "Average of Final Score", xlAverage
        .AddDataField Sheets("Pivot Table5").PivotTables("PivotTable5"). _
            PivotFields("Status"), "Completed Courses", xlCount
        .ColumnGrand = False
        .RowGrand = False
        .PivotFields("Name").PivotItems("(blank)").Visible = False
    End With
    Set pivot = Nothing
    
    Dim lastRow As Long
    lastRow = Range("C" & Rows.Count).End(xlUp).Row
    
    Dim cht As Chart
    Set cht = Charts.Add
    With cht
        .Name = "FSvS"
        .ChartType = xlColumnClustered
        .SetSourceData Source:=Sheets("Pivot Table5").Range("A1:C" & lastRow), _
            PlotBy:=xlColumns
        .HasTitle = True: .HasLegend = True
        .Legend.Position = xlLegendPositionBottom
        .ChartTitle.text = "Average Final Score vs. Syllabus Completion"
         With .FullSeriesCollection(1)
            .Format.Fill.ForeColor.RGB = RGB(111, 201, 196)
            .ChartType = xlColumnClustered
            .AxisGroup = 1
        End With
        With .FullSeriesCollection(2)
            .Format.Line.ForeColor.RGB = RGB(205, 58, 49)
            .ChartType = xlLine
            .AxisGroup = 2
        End With
        With .Axes(xlCategory, xlPrimary)
            .HasTitle = True
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Student Name"
            .TickLabels.Font.Color = RGB(115, 123, 127)
        End With
        With .Axes(xlValue, xlPrimary)
            .HasTitle = True
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Average Grade"
            .TickLabels.Font.Color = RGB(115, 123, 127)
            .MinimumScale = 0: .MaximumScale = 100
        End With
        With .Axes(xlValue, xlSecondary)
            .HasTitle = True
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Courses Completed"
            .TickLabels.Font.Color = RGB(115, 123, 127)
        End With
    End With
    Set cht = Nothing
    
    AddPhoto
    Timestamp
End Sub
Sub CoursesCompleted()
    'Displays a chart that shows count of courses complete per student
    OnlyComplete
    ActiveSheet.Name = "Only Complete"
    
    Sheets.Add
    ActiveSheet.Name = "Pivot Table"
    
    ActiveWorkbook.PivotCaches. _
        Create( _
        SourceType:=xlDatabase, _
        SourceData:=Sheets("Only Complete").Range("A1:N" & Rows.Count)). _
            CreatePivotTable _
            TableDestination:=Worksheets("Pivot Table").Range("A1"), _
            TableName:="PivotTable1"

    Set pivot = ActiveSheet.PivotTables("PivotTable1")
    With pivot
        .PivotFields("Name").Orientation = xlRowField
        .AddDataField Sheets("Pivot Table").PivotTables("PivotTable1").PivotFields("Status"), "Count of Statuses", xlCount
        .ColumnGrand = False: .RowGrand = False
        .PivotFields("Name").PivotItems("(blank)").Visible = False
    End With
    Set pivot = Nothing
    
    Dim lastRow As Long
    lastRow = Range("B" & Rows.Count).End(xlUp).Row
    
    Dim cht As Chart
    Set cht = Charts.Add
    With cht
        .Name = "CC"
        .ChartType = xlColumnClustered
        .SetSourceData Source:=Sheets("Pivot Table").Range("A1:B" & lastRow), _
            PlotBy:=xlColumns
        .ChartTitle.text = "Number of Courses Completed"
        .HasLegend = False
        With .Axes(xlCategory, xlPrimary)
            '.TickLabels.Font.Size = 14
            .HasTitle = True
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Name"
            .TickLabels.Font.Color = RGB(115, 123, 127)
        End With
        With .Axes(xlValue, xlPrimary)
            .HasTitle = True
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Number of Courses"
            '.TickLabels.Font.Size = 14
            .TickLabels.Font.Color = RGB(115, 123, 127)
            .MaximumScale = 60
        End With
        With .FullSeriesCollection(1).Format.Fill
            .Visible = msoTrue: .Solid: .Transparency = 0
            .ForeColor.RGB = RGB(111, 201, 196)
        End With
    End With
    Set cht = Nothing
    
    AddPhoto
    Timestamp
End Sub
Sub AverageGradeStudent()
    'Creates a chart that displays average grade by student
    Sheets.Add
    ActiveSheet.Name = "Pivot Table2"
    
    ActiveWorkbook.PivotCaches. _
        Create( _
        SourceType:=xlDatabase, _
        SourceData:=Sheets("Only Complete").Range("A1:N" & Rows.Count)). _
            CreatePivotTable _
            TableDestination:=Worksheets("Pivot Table2").Range("A1"), _
            TableName:="PivotTable2"

    Set pivot = ActiveSheet.PivotTables("PivotTable2")
    With pivot
        .PivotFields("Name").Orientation = xlRowField
        .AddDataField Sheets("Pivot Table2").PivotTables("PivotTable2"). _
            PivotFields("Final score"), "Average of Final Score", xlAverage
        .ColumnGrand = True: .RowGrand = True
        .PivotFields("Name").PivotItems("(blank)").Visible = False
    End With
    Set pivot = Nothing
    
    Dim lastRow As Long
    lastRow = Range("B" & Rows.Count).End(xlUp).Row
    
    Dim cht As Chart
    Set cht = Charts.Add
    With cht
        .Name = "AGS"
        .ChartType = xlColumnClustered
        .SetSourceData Source:=Sheets("Pivot Table2").Range("A1:B" & lastRow), _
            PlotBy:=xlColumns
        .HasLegend = False
        .ChartTitle.text = "Average Grade by Student"
        With .Axes(xlCategory, xlPrimary)
            .HasTitle = True
            .TickLabels.Font.Size = 14
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Student Name"
            .TickLabels.Font.Color = RGB(115, 123, 127)
        End With
        With .Axes(xlValue, xlPrimary)
            .HasTitle = True
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Average Grade"
            .TickLabels.Font.Size = 14
            .TickLabels.Font.Color = RGB(115, 123, 127)
            .MinimumScale = 0: .MaximumScale = 100
        End With
        With .FullSeriesCollection(1).Format.Fill
            .ForeColor.RGB = RGB(111, 201, 196)
        End With
    End With
    Set cht = Nothing
    
    AddPhoto
    Timestamp
End Sub
Sub AverageGradeCourse()
    'Creates a chart that displays average grade by course
    Sheets.Add
    ActiveSheet.Name = "Pivot Table3"
    
    ActiveWorkbook.PivotCaches. _
        Create( _
        SourceType:=xlDatabase, _
        SourceData:=Sheets("Only Complete").Range("A1:N" & Rows.Count)). _
            CreatePivotTable _
            TableDestination:=Worksheets("Pivot Table3").Range("A1"), _
            TableName:="PivotTable3"

    Set pivot = ActiveSheet.PivotTables("PivotTable3")
    With pivot
        .PivotFields("Course code").Orientation = xlRowField
        .AddDataField Sheets("Pivot Table3").PivotTables("PivotTable3"). _
            PivotFields("Final score"), "Average of Final Score", xlAverage
        .ColumnGrand = True
        .RowGrand = True
        .PivotFields("Course code").PivotItems("(blank)").Visible = False
    End With
    Set pivot = Nothing
    
    Dim lastRow As Long
    lastRow = Range("B" & Rows.Count).End(xlUp).Row
    
    Dim cht As Chart
    Set cht = Charts.Add
    With cht
        .Name = "AGC"
        .ChartType = xlColumnClustered
        .SetSourceData Source:=Sheets("Pivot Table3").Range("A1:B" & lastRow), _
            PlotBy:=xlColumns
        .HasTitle = True: .HasLegend = False
        .ChartTitle.text = "Average Grade by Course"
        With .Axes(xlCategory, xlPrimary)
            .HasTitle = True
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Course Name"
            .TickLabels.Font.Color = RGB(115, 123, 127)
        End With
        With .Axes(xlValue, xlPrimary)
            .HasTitle = True
            .AxisTitle.Font.Size = 16
            .AxisTitle.Characters.text = "Average Grade"
            .MinimumScale = 0: .MaximumScale = 100
            .TickLabels.Font.Color = RGB(115, 123, 127)
        End With
        With .FullSeriesCollection(1).Format.Fill
            .ForeColor.RGB = RGB(111, 201, 196)
        End With
    End With
    Set cht = Nothing
    
    ActiveSheet.PageSetup.PaperSize = xlPaperLegal
    
    LegalPhoto
    Timestamp
End Sub
Sub CompletionDates()
    'Displays first access dates and completion dates
    'Deletes unnecessary columns, and formats the text
    'Worksheets("3").Activate
    OnlyComplete

    Range("N:N,M:M,L:L,J:J,G:G,F:F,C:C,A:A").Delete
    
    ActiveSheet.Name = "Dates"
    
    Rows(1).Insert Shift:=xlDown
    Range("A1").Value = "Start and Completion Dates"
    Rows(1).Insert Shift:=xlDown
    Range("A1").RowHeight = 44
    
    With Range("A2")
        With .Font
            .Name = "Calibri": .Size = 17: .Bold = True
            .Color = RGB(115, 123, 127)
        End With
        With .Borders(xlBottom)
            .LineStyle = xlContinuous: .Weight = xlThick
            .Color = RGB(115, 123, 127)
        End With
    End With
    With Range("A3:F3")
        With .Font
            .Name = "Calibri": .Size = 14: .Bold = True
            .Color = RGB(115, 123, 127)
        End With
        With .Borders(xlBottom)
            .LineStyle = xlContinuous: .Weight = xlThick
            .Color = RGB(115, 123, 127)
        End With
    End With
    With Range("A1")
        With .Borders(xlBottom)
            .LineStyle = xlContinuous: .Weight = xlThick
            .Color = RGB(115, 123, 127)
        End With
    End With
    
    Columns("A:F").AutoFit
    
    AddPhoto2
End Sub
Sub AddPhoto2()
    'Adds logo into dates completed sheet
    ActiveSheet.Shapes.AddPicture Filename:="C:\Users\samantha.feng\Documents\Sam Feng\LOGiQ3 Logo.png", linktofile:=msoFalse, _
        savewithdocument:=msoCTrue, Left:=54, Top:=0, Width:=110, Height:=43
End Sub
Sub TextToNum()
    'Convert final score and number of sessions into numbers
    Dim rng As Range
    Dim lastRow As Long
    
    lastRow = Range("L" & Rows.Count).End(xlUp).Row
    
    Set rng = Range("L2:M" & lastRow)
    
    Range("P2").Value = "1"
    Range("P2").Copy
    
    With rng
        .PasteSpecial Paste:=xlPasteAll, Operation:=xlMultiply, _
        SkipBlanks:=False, Transpose:=False
    End With
    
    Set rng = Nothing
End Sub
Sub Copy()
    'Used to create a copy of the first worksheet
    'Copy created after the first worksheet
    Worksheets(1).Copy After:=Worksheets(1)
End Sub
Sub OnlyComplete()
    'Filtered for entries with the status complete
    NameGenerator
    TextToNum
    
    Dim rng As Range
    Dim lastRow As Long
    
    lastRow = Range("K" & Rows.Count).End(xlUp).Row
    
    Set rng = Range("K2:K" & lastRow)
    With rng
        .AutoFilter Field:=1, Criteria1:="<>*Completed*"
        .SpecialCells(xlCellTypeVisible).EntireRow.Delete
    End With
    
    Set rng = Nothing
    
    AutoFilterMode = False
End Sub
Sub NameGenerator()
    'Compile last name and first name into one row
    Dim N As Long
    N = Cells(Rows.Count, 1).End(xlUp).Row
    For i = 2 To N
        Cells(i, 3) = Cells(i, 2) & " " & Cells(i, 3)
    Next i
    
    Range("C1").Value = "Name"
    Columns(2).EntireColumn.Delete
End Sub
