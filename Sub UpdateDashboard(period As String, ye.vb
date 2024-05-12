Sub UpdateDashboard(period As String, year As Integer)
    Dim wsDashboard As Worksheet
    Dim wsJoiner As Worksheet
    Dim wsMover As Worksheet
    Dim wsLeaver As Worksheet
    Dim periodStart As Date
    Dim periodEnd As Date
    Dim periodDates() As Date
    Dim countsJoiner() As Integer
    Dim countsMover() As Integer
    Dim countsLeaver() As Integer
    Dim dateIndex As Long
    Dim entryIndex As Long
    Dim chartDataRangeJoiner As Range
    Dim chartDataRangeMover As Range
    Dim chartDataRangeLeaver As Range
    
    ' Set references to the worksheets
    Set wsDashboard = ThisWorkbook.Sheets("Dashboard")
    Set wsJoiner = ThisWorkbook.Sheets("Joiner")
    Set wsMover = ThisWorkbook.Sheets("Mover")
    Set wsLeaver = ThisWorkbook.Sheets("Leaver")
    
    ' Determine start and end date of the selected period
    Select Case period
        Case "Q1"
            periodStart = DateSerial(year, 4, 1)
            periodEnd = DateSerial(year, 6, 30)
        Case "Q2"
            periodStart = DateSerial(year, 7, 1)
            periodEnd = DateSerial(year, 9, 30)
        Case "Q3"
            periodStart = DateSerial(year, 10, 1)
            periodEnd = DateSerial(year, 12, 31)
        Case "Q4"
            periodStart = DateSerial(year + 1, 1, 1)
            periodEnd = DateSerial(year + 1, 3, 31)
    End Select
    
    ' Calculate the array of dates within the selected period
    Dim currentDate As Date
    currentDate = periodStart
    dateIndex = 0
    Do While currentDate <= periodEnd
        ReDim Preserve periodDates(dateIndex)
        periodDates(dateIndex) = currentDate
        dateIndex = dateIndex + 1
        Select Case period
            Case "Q1", "Q2", "Q3", "Q4"
                currentDate = DateAdd("d", 1, currentDate)
        End Select
    Loop
    
    ' Initialize counts arrays
    ReDim countsJoiner(0 To UBound(periodDates))
    ReDim countsMover(0 To UBound(periodDates))
    ReDim countsLeaver(0 To UBound(periodDates))
    
    ' Loop through Joiner sheet and count entries within the selected period
    For entryIndex = 2 To wsJoiner.Cells(wsJoiner.Rows.Count, "C").End(xlUp).Row
        If wsJoiner.Cells(entryIndex, "C").Value >= periodStart And wsJoiner.Cells(entryIndex, "C").Value <= periodEnd Then
            For dateIndex = 0 To UBound(periodDates)
                If wsJoiner.Cells(entryIndex, "C").Value = periodDates(dateIndex) Then
                    countsJoiner(dateIndex) = countsJoiner(dateIndex) + 1
                    Exit For
                End If
            Next dateIndex
        End If
    Next entryIndex
    
    ' Loop through Mover sheet and count entries within the selected period
    For entryIndex = 2 To wsMover.Cells(wsMover.Rows.Count, "C").End(xlUp).Row
        If wsMover.Cells(entryIndex, "C").Value >= periodStart And wsMover.Cells(entryIndex, "C").Value <= periodEnd Then
            For dateIndex = 0 To UBound(periodDates)
                If wsMover.Cells(entryIndex, "C").Value = periodDates(dateIndex) Then
                    countsMover(dateIndex) = countsMover(dateIndex) + 1
                    Exit For
                End If
            Next dateIndex
        End If
    Next entryIndex
    
    ' Loop through Leaver sheet and count entries within the selected period
    For entryIndex = 2 To wsLeaver.Cells(wsLeaver.Rows.Count, "C").End(xlUp).Row
        If wsLeaver.Cells(entryIndex, "C").Value >= periodStart And wsLeaver.Cells(entryIndex, "C").Value <= periodEnd Then
            For dateIndex = 0 To UBound(periodDates)
                If wsLeaver.Cells(entryIndex, "C").Value = periodDates(dateIndex) Then
                    countsLeaver(dateIndex) = countsLeaver(dateIndex) + 1
                    Exit For
                End If
            Next dateIndex
        End If
    Next entryIndex
    
    ' Update Dashboard with the counts for Joiner, Mover, and Leaver
    For dateIndex = 0 To UBound(periodDates)
        wsDashboard.Cells(4 + dateIndex, 2).Value = countsJoiner(dateIndex)  ' Update Joiner count
        wsDashboard.Cells(4 + dateIndex, 3).Value = countsMover(dateIndex)   ' Update Mover count
        wsDashboard.Cells(4 + dateIndex, 4).Value = countsLeaver(dateIndex)  ' Update Leaver count
        wsDashboard.Cells(4 + dateIndex, 1).Value = Format(periodDates(dateIndex), "dd-mmm-yyyy")  ' Update Date
    Next dateIndex
    
    ' Update graph data sources for Joiner
    Set chartDataRangeJoiner = wsDashboard.Range(wsDashboard.Cells(4, 2), wsDashboard.Cells(3 + UBound(periodDates), 2))
    wsDashboard.ChartObjects("JoinerChart").Chart.SetSourceData Source:=chartDataRangeJoiner
    
    ' Update graph data sources for Mover
    Set chartDataRangeMover = wsDashboard.Range(wsDashboard.Cells(4, 3), wsDashboard.Cells(3 + UBound(periodDates), 3))
    wsDashboard.ChartObjects("MoverChart").Chart.SetSourceData Source:=chartDataRangeMover
    
    ' Update graph data sources for Leaver
    Set chartDataRangeLeaver = wsDashboard.Range(wsDashboard.Cells(4, 4), wsDashboard.Cells(3 + UBound(periodDates), 4))
    wsDashboard.Chart