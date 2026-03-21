REM FizzBuzz — the classic loop and conditional exercise
REM Print 1-30: Fizz for multiples of 3, Buzz for 5, FizzBuzz for both

FOR i = 1 TO 30
    IF i MOD 15 = 0 THEN
        PRINT "FizzBuzz"
    ELSE
        IF i MOD 3 = 0 THEN
            PRINT "Fizz"
        ELSE
            IF i MOD 5 = 0 THEN
                PRINT "Buzz"
            ELSE
                PRINT STR$(i)
            END IF
        END IF
    END IF
NEXT i
