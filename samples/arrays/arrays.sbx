REM Arrays — DIM, indexed access, and aggregation

REM Declare and populate a scores array
DIM scores[5] AS INTEGER

LET scores[0] = 74
LET scores[1] = 88
LET scores[2] = 61
LET scores[3] = 95
LET scores[4] = 82

PRINT "=== Exam Scores ==="
PRINT ""

REM Print all scores
FOR i = 0 TO 4
    PRINT "Student " & STR$(i + 1) & ": " & STR$(scores[i])
NEXT i

PRINT ""

REM Calculate total and average
LET total AS INTEGER = 0
FOR i = 0 TO 4
    LET total = total + scores[i]
NEXT i

LET avg AS FLOAT = total / 5
PRINT "Total:   " & STR$(total)
PRINT "Average: " & STR$(avg)

REM Find highest score
LET highest AS INTEGER = scores[0]
FOR i = 1 TO 4
    IF scores[i] > highest THEN
        LET highest = scores[i]
    END IF
NEXT i

PRINT "Highest: " & STR$(highest)
