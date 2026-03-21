REM Guess the number — WHILE loop, RND, and conditional logic

LET secret AS INTEGER = INT(RND() * 100) + 1
LET guess AS INTEGER = 0
LET attempts AS INTEGER = 0

PRINT "=== Guess the Number ==="
PRINT "I'm thinking of a number between 1 and 100."
PRINT ""

WHILE guess <> secret
    PRINT "Your guess:"
    INPUT guessStr
    LET guess = VAL(guessStr)
    LET attempts = attempts + 1

    IF guess < secret THEN
        PRINT "Too low! Try higher."
    ELSE
        IF guess > secret THEN
            PRINT "Too high! Try lower."
        ELSE
            PRINT ""
            PRINT "Correct! The number was " & STR$(secret) & "."
            PRINT "You got it in " & STR$(attempts) & " attempts."
        END IF
    END IF
WEND
