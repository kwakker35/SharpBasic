REM Wordle clone — 6 attempts to guess a secret 5-letter word
REM Feedback per guess:
REM   [X]  correct letter, correct position  (green)
REM   (X)  correct letter, wrong position    (yellow)
REM   .X.  letter not in word               (grey)

REM -------------------------------------------------------
REM Word list (5-letter words)
REM -------------------------------------------------------
DIM wordList[12] AS STRING
LET wordList[0] = "CRANE"
LET wordList[1] = "BLAND"
LET wordList[2] = "FLINT"
LET wordList[3] = "GRIME"
LET wordList[4] = "STOMP"
LET wordList[5] = "PLUCK"
LET wordList[6] = "HAZEL"
LET wordList[7] = "GROAN"
LET wordList[8] = "SWIFT"
LET wordList[9] = "BLAZE"
LET wordList[10] = "CRIMP"
LET wordList[11] = "DOWRY"

LET secret AS STRING = wordList[INT(RND() * 12)]
LET maxGuesses AS INTEGER = 6
LET guessNum AS INTEGER = 0
LET won AS BOOLEAN = FALSE

REM -------------------------------------------------------
REM FUNCTION: check if a string is exactly 5 alphabetic chars
REM -------------------------------------------------------
FUNCTION IsValidGuess(g AS STRING) AS BOOLEAN
    IF LEN(g) <> 5 THEN
        RETURN FALSE
    END IF
    FOR k = 1 TO 5
        LET ch AS STRING = MID$(g, k, 1)
        LET isAlpha AS BOOLEAN = FALSE
        FOR a = 0 TO 25
            IF ch = MID$("ABCDEFGHIJKLMNOPQRSTUVWXYZ", a + 1, 1) THEN
                LET isAlpha = TRUE
            END IF
        NEXT a
        IF isAlpha = FALSE THEN
            RETURN FALSE
        END IF
    NEXT k
    RETURN TRUE
END FUNCTION

REM -------------------------------------------------------
REM FUNCTION: score a guess against the secret
REM Returns a 5-char string: G=green, Y=yellow, B=black(grey)
REM -------------------------------------------------------
FUNCTION ScoreGuess(guess AS STRING, answer AS STRING) AS STRING
    DIM result[5] AS STRING
    DIM answerUsed[5] AS BOOLEAN

    REM Initialise
    FOR k = 0 TO 4
        LET result[k] = "B"
        LET answerUsed[k] = FALSE
    NEXT k

    REM Pass 1: greens
    FOR k = 0 TO 4
        IF MID$(guess, k + 1, 1) = MID$(answer, k + 1, 1) THEN
            LET result[k] = "G"
            LET answerUsed[k] = TRUE
        END IF
    NEXT k

    REM Pass 2: yellows
    FOR k = 0 TO 4
        IF result[k] <> "G" THEN
            LET gch AS STRING = MID$(guess, k + 1, 1)
            FOR j = 0 TO 4
                IF answerUsed[j] = FALSE THEN
                    IF MID$(answer, j + 1, 1) = gch THEN
                        LET result[k] = "Y"
                        LET answerUsed[j] = TRUE
                    END IF
                END IF
            NEXT j
        END IF
    NEXT k

    REM Build result string
    LET score AS STRING = result[0] & result[1] & result[2] & result[3] & result[4]
    RETURN score
END FUNCTION

REM -------------------------------------------------------
REM SUB: pretty-print a scored guess
REM -------------------------------------------------------
SUB PrintResult(guess AS STRING, score AS STRING)
    LET line AS STRING = ""
    FOR k = 0 TO 4
        LET ch AS STRING = MID$(guess, k + 1, 1)
        LET s AS STRING = MID$(score, k + 1, 1)
        IF s = "G" THEN
            LET line = line & "[" & ch & "]"
        ELSE
            IF s = "Y" THEN
                LET line = line & "(" & ch & ")"
            ELSE
                LET line = line & "." & ch & "."
            END IF
        END IF
    NEXT k
    PRINT line
END SUB

REM -------------------------------------------------------
REM Main game loop
REM -------------------------------------------------------
PRINT "=== WORDLE ==="
PRINT "Guess the 5-letter word. You have 6 attempts."
PRINT ""
PRINT "Legend:  [X] correct position   (X) wrong position   .X. not in word"
PRINT ""

WHILE guessNum < maxGuesses AND won = FALSE
    PRINT "Guess " & STR$(guessNum + 1) & " of " & STR$(maxGuesses) & ":"
    INPUT rawInput
    LET guess AS STRING = UPPER$(TRIM$(rawInput))

    IF IsValidGuess(guess) = FALSE THEN
        PRINT "Please enter exactly 5 letters."
    ELSE
        LET guessNum = guessNum + 1
        LET score AS STRING = ScoreGuess(guess, secret)
        CALL PrintResult(guess, score)
        PRINT ""

        IF score = "GGGGG" THEN
            LET won = TRUE
        END IF
    END IF
WEND

IF won = TRUE THEN
    PRINT "Brilliant! You got it in " & STR$(guessNum) & " guess(es)."
ELSE
    PRINT "Hard luck. The word was: " & secret
END IF
