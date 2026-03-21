REM Fibonacci — recursive FUNCTION, demonstrating call stacks

FUNCTION Fibonacci(n AS INTEGER) AS INTEGER
    IF n <= 1 THEN
        RETURN n
    END IF
    RETURN Fibonacci(n - 1) + Fibonacci(n - 2)
END FUNCTION

PRINT "=== Fibonacci Sequence ==="
PRINT ""

FOR i = 0 TO 15
    PRINT "F(" & STR$(i) & ") = " & STR$(Fibonacci(i))
NEXT i

PRINT ""
PRINT "Classic check: F(10) = " & STR$(Fibonacci(10))
