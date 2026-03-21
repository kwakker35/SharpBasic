REM Tic-Tac-Toe — two-player game with ASCII board
REM Board stored as a flat 9-element array: index = row*3 + col

DIM board[9] AS STRING

REM Initialise board with cell numbers
LET board[0] = "1"
LET board[1] = "2"
LET board[2] = "3"
LET board[3] = "4"
LET board[4] = "5"
LET board[5] = "6"
LET board[6] = "7"
LET board[7] = "8"
LET board[8] = "9"

REM -------------------------------------------------------
REM SUB: draw the board
REM -------------------------------------------------------
SUB DrawBoard()
    PRINT ""
    PRINT " " & board[0] & " | " & board[1] & " | " & board[2]
    PRINT "---+---+---"
    PRINT " " & board[3] & " | " & board[4] & " | " & board[5]
    PRINT "---+---+---"
    PRINT " " & board[6] & " | " & board[7] & " | " & board[8]
    PRINT ""
END SUB

REM -------------------------------------------------------
REM FUNCTION: check if a player has won
REM -------------------------------------------------------
FUNCTION CheckWin(mark AS STRING) AS BOOLEAN
    REM Rows
    IF board[0] = mark AND board[1] = mark AND board[2] = mark THEN RETURN TRUE END IF
    IF board[3] = mark AND board[4] = mark AND board[5] = mark THEN RETURN TRUE END IF
    IF board[6] = mark AND board[7] = mark AND board[8] = mark THEN RETURN TRUE END IF
    REM Columns
    IF board[0] = mark AND board[3] = mark AND board[6] = mark THEN RETURN TRUE END IF
    IF board[1] = mark AND board[4] = mark AND board[7] = mark THEN RETURN TRUE END IF
    IF board[2] = mark AND board[5] = mark AND board[8] = mark THEN RETURN TRUE END IF
    REM Diagonals
    IF board[0] = mark AND board[4] = mark AND board[8] = mark THEN RETURN TRUE END IF
    IF board[2] = mark AND board[4] = mark AND board[6] = mark THEN RETURN TRUE END IF
    RETURN FALSE
END FUNCTION

REM -------------------------------------------------------
REM FUNCTION: check if the board is full (draw)
REM -------------------------------------------------------
FUNCTION IsBoardFull() AS BOOLEAN
    LET full AS BOOLEAN = TRUE
    FOR i = 0 TO 8
        IF board[i] = "X" OR board[i] = "O" THEN
        ELSE
            LET full = FALSE
        END IF
    NEXT i
    RETURN full
END FUNCTION

REM -------------------------------------------------------
REM Main game loop
REM -------------------------------------------------------
PRINT "=== TIC-TAC-TOE ==="
PRINT "Players: X and O"
PRINT "Enter the cell number (1-9) shown on the board."

LET currentMark AS STRING = "X"
LET gameOver AS BOOLEAN = FALSE

WHILE gameOver = FALSE
    CALL DrawBoard()
    PRINT "Player " & currentMark & ", choose a cell (1-9):"
    INPUT cellStr
    LET cell AS INTEGER = VAL(cellStr)

    IF cell < 1 OR cell > 9 THEN
        PRINT "Invalid choice. Enter a number from 1 to 9."
    ELSE
        LET idx AS INTEGER = cell - 1
        IF board[idx] = "X" OR board[idx] = "O" THEN
            PRINT "That cell is already taken."
        ELSE
            LET board[idx] = currentMark

            IF CheckWin(currentMark) = TRUE THEN
                CALL DrawBoard()
                PRINT "Player " & currentMark & " wins!"
                LET gameOver = TRUE
            ELSE
                IF IsBoardFull() = TRUE THEN
                    CALL DrawBoard()
                    PRINT "It's a draw!"
                    LET gameOver = TRUE
                ELSE
                    IF currentMark = "X" THEN
                        LET currentMark = "O"
                    ELSE
                        LET currentMark = "X"
                    END IF
                END IF
            END IF
        END IF
    END IF
WEND
