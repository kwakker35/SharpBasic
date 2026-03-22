# SharpBASIC Syntax

VS Code syntax highlighting for SharpBASIC (`.sbx`) files.

## Features

- Keyword highlighting: `IF`, `THEN`, `ELSE`, `END`, `WHILE`, `FOR`, `SUB`, `FUNCTION`, etc.
- Built-in function highlighting: `LEN`, `MID$`, `STR$`, `ABS`, `RND`, `TYPENAME`, etc.
- String literal, numeric literal, and boolean constant colouring
- `REM` line comments (toggle with **Ctrl+/**  on Windows / **Cmd+/** on macOS)
- Bracket matching and auto-closing pairs for `(`, `[`, `"`

## Installation

Install from the `.vsix`:

1. In VS Code open the **Extensions** view (`Ctrl+Shift+X`)
2. Click the `...` menu → **Install from VSIX…**
3. Select `sharpbasic-syntax-0.2.0.vsix`

Or from the terminal:

```
code --install-extension sharpbasic-syntax-0.2.0.vsix
```

## Building the .vsix

Requires Node.js and `vsce`:

```
npm install -g @vscode/vsce
cd vscode-extension/sharpbasic-syntax
vsce package
```
