# Cell - Excel Formulas on Steroids

This is Cell, a small scripting language highly inspired by the Excel _(or any other spreadsheet software)_ formulas.

    $$ set_range($0:5, 10),
    .. print(sum($0:5))
    60
    $$ # Yes, ranges are inclusive.

And everything that can be written into a Cell script is translated into a bunch of functions:

    $$ INSPECT(5 + 4 * $0)
    ADD(5, MUL(4, GET_CELL(0)))
    $$ INSPECT($0:5)
    GET_RANGE(0, 5)

This readme is still under construction, stay tuned!
