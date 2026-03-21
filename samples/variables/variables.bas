REM Demonstrates variable declaration and the four built-in types

LET name AS STRING = "SharpBASIC"
LET version AS FLOAT = 0.1
LET year AS INTEGER = 2026
LET isOpen AS BOOLEAN = TRUE

PRINT "Language: " & name
PRINT "Version:  " & STR$(version)
PRINT "Year:     " & STR$(year)
PRINT "Open source: " & STR$(isOpen)

REM TYPENAME tells you the runtime type of any value
PRINT "Type of name:    " & TYPENAME(name)
PRINT "Type of version: " & TYPENAME(version)
PRINT "Type of year:    " & TYPENAME(year)
PRINT "Type of isOpen:  " & TYPENAME(isOpen)
