REM Simple four-function calculator

PRINT "=== SharpBASIC Calculator ==="
PRINT "Enter two numbers and an operator (+, -, *, /)"
PRINT ""

PRINT "First number:"
INPUT aStr
LET a AS FLOAT = VAL(aStr)

PRINT "Operator:"
INPUT op

PRINT "Second number:"
INPUT bStr
LET b AS FLOAT = VAL(bStr)

PRINT ""

IF op = "+" THEN
    PRINT STR$(a) & " + " & STR$(b) & " = " & STR$(a + b)
ELSE
    IF op = "-" THEN
        PRINT STR$(a) & " - " & STR$(b) & " = " & STR$(a - b)
    ELSE
        IF op = "*" THEN
            PRINT STR$(a) & " * " & STR$(b) & " = " & STR$(a * b)
        ELSE
            IF op = "/" THEN
                IF b = 0 THEN
                    PRINT "Error: division by zero"
                ELSE
                    PRINT STR$(a) & " / " & STR$(b) & " = " & STR$(a / b)
                END IF
            ELSE
                PRINT "Unknown operator: " & op
            END IF
        END IF
    END IF
END IF
