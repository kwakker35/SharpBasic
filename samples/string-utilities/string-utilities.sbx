REM String utilities showcase — all the built-in string functions

LET s AS STRING = "  Hello, SharpBASIC World!  "

PRINT "Original:   [" & s & "]"
PRINT "TRIM$:      [" & TRIM$(s) & "]"
PRINT "UPPER$:     [" & UPPER$(TRIM$(s)) & "]"
PRINT "LOWER$:     [" & LOWER$(TRIM$(s)) & "]"
PRINT "LEN:        " & STR$(LEN(TRIM$(s)))
PRINT ""

LET word AS STRING = "SharpBASIC"
PRINT "Word: " & word
PRINT "LEFT$(6):  " & LEFT$(word, 6)
PRINT "RIGHT$(6): " & RIGHT$(word, 6)
PRINT "MID$(7,5): " & MID$(word, 7, 5)
PRINT ""

REM Build a simple title-casing demo
LET first AS STRING = UPPER$(LEFT$("alice", 1)) & LOWER$(RIGHT$("alice", LEN("alice") - 1))
PRINT "Title-cased 'alice': " & first
