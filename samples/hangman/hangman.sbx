REM Hangman — word guessing game using SUBs, FUNCTIONs, and arrays

REM -------------------------------------------------------
REM Word list (hardcoded — no file I/O yet)
REM -------------------------------------------------------
DIM words[8] AS STRING
LET words[0] = "INTERPRETER"
LET words[1] = "RECURSION"
LET words[2] = "VARIABLE"
LET words[3] = "BOOLEAN"
LET words[4] = "FUNCTION"
LET words[5] = "ALGORITHM"
LET words[6] = "SHARPBASIC"
LET words[7] = "COMPILER"

REM Pick a random word
LET wordIndex AS INTEGER = INT(RND() * 8)
LET secret AS STRING = words[wordIndex]
LET wordLen AS INTEGER = LEN(secret)

REM -------------------------------------------------------
REM Guessed letters array (26 slots, one per letter A-Z)
REM -------------------------------------------------------
DIM guessed[26] AS BOOLEAN
FOR i = 0 TO 25
    LET guessed[i] = FALSE
NEXT i

LET maxWrong AS INTEGER = 6
LET wrongCount AS INTEGER = 0
LET solved AS BOOLEAN = FALSE

REM -------------------------------------------------------
REM FUNCTION: build the display word, replacing unguessed
REM letters with underscores
REM -------------------------------------------------------
FUNCTION BuildDisplay(word AS STRING) AS STRING
    LET display AS STRING = ""
    LET wLen AS INTEGER = LEN(word)
    FOR k = 1 TO wLen
        LET ch AS STRING = MID$(word, k, 1)
        REM ch is always a letter A-Z, so index = ASC - 65
        REM We can't call ASC() yet, so we check each letter manually
        LET found AS BOOLEAN = FALSE
        FOR a = 0 TO 25
            IF ch = MID$("ABCDEFGHIJKLMNOPQRSTUVWXYZ", a + 1, 1) THEN
                IF guessed[a] = TRUE THEN
                    LET found = TRUE
                END IF
            END IF
        NEXT a
        IF found = TRUE THEN
            LET display = display & ch & " "
        ELSE
            LET display = display & "_ "
        END IF
    NEXT k
    RETURN display
END FUNCTION

REM -------------------------------------------------------
REM FUNCTION: count how many letters are still hidden
REM -------------------------------------------------------
FUNCTION CountHidden(word AS STRING) AS INTEGER
    LET hidden AS INTEGER = 0
    LET wLen AS INTEGER = LEN(word)
    FOR k = 1 TO wLen
        LET ch AS STRING = MID$(word, k, 1)
        LET found AS BOOLEAN = FALSE
        FOR a = 0 TO 25
            IF ch = MID$("ABCDEFGHIJKLMNOPQRSTUVWXYZ", a + 1, 1) THEN
                IF guessed[a] = TRUE THEN
                    LET found = TRUE
                END IF
            END IF
        NEXT a
        IF found = FALSE THEN
            LET hidden = hidden + 1
        END IF
    NEXT k
    RETURN hidden
END FUNCTION

REM -------------------------------------------------------
REM SUB: draw the hangman scaffold
REM -------------------------------------------------------
SUB DrawHangman(wrong AS INTEGER)
    PRINT "  +---+"
    IF wrong >= 1 THEN
        PRINT "  |   O"
    ELSE
        PRINT "  |    "
    END IF
    IF wrong = 2 THEN
        PRINT "  |   |"
    ELSE
        IF wrong = 3 THEN
            PRINT "  |  /|"
        ELSE
            IF wrong >= 4 THEN
                PRINT "  |  /|\\"
            ELSE
                PRINT "  |    "
            END IF
        END IF
    END IF
    IF wrong = 5 THEN
        PRINT "  |  /"
    ELSE
        IF wrong >= 6 THEN
            PRINT "  |  / \\"
        ELSE
            PRINT "  |    "
        END IF
    END IF
    PRINT "  |"
    PRINT "======"
END SUB

REM -------------------------------------------------------
REM Main game loop
REM -------------------------------------------------------
PRINT "=== HANGMAN ==="
PRINT ""

WHILE wrongCount < maxWrong AND solved = FALSE
    CALL DrawHangman(wrongCount)
    PRINT ""
    PRINT "Word: " & BuildDisplay(secret)
    PRINT "Wrong guesses: " & STR$(wrongCount) & " / " & STR$(maxWrong)
    PRINT ""
    PRINT "Guess a letter:"
    INPUT rawGuess
    LET letterGuess AS STRING = UPPER$(LEFT$(TRIM$(rawGuess), 1))

    REM Find the letter's index in the alphabet
    LET letterIndex AS INTEGER = -1
    FOR a = 0 TO 25
        IF letterGuess = MID$("ABCDEFGHIJKLMNOPQRSTUVWXYZ", a + 1, 1) THEN
            LET letterIndex = a
        END IF
    NEXT a

    IF letterIndex = -1 THEN
        PRINT "Please enter a letter A-Z."
    ELSE
        IF guessed[letterIndex] = TRUE THEN
            PRINT "You already guessed " & letterGuess & "."
        ELSE
            LET guessed[letterIndex] = TRUE

            REM Check if the letter is in the word
            LET inWord AS BOOLEAN = FALSE
            FOR k = 1 TO wordLen
                IF MID$(secret, k, 1) = letterGuess THEN
                    LET inWord = TRUE
                END IF
            NEXT k

            IF inWord = FALSE THEN
                LET wrongCount = wrongCount + 1
                PRINT letterGuess & " is not in the word."
            ELSE
                PRINT "Good guess!"
            END IF

            IF CountHidden(secret) = 0 THEN
                LET solved = TRUE
            END IF
        END IF
    END IF
    PRINT ""
WEND

REM -------------------------------------------------------
REM Result
REM -------------------------------------------------------
CALL DrawHangman(wrongCount)
PRINT ""
IF solved = TRUE THEN
    PRINT "You won! The word was: " & secret
ELSE
    PRINT "Game over. The word was: " & secret
END IF
