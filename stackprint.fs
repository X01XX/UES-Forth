\ Print a stack, giving information about structs on the stack.

\ Print the type of one value.
: .stack-structs2 ( addr -- )
    struct-info-list-store                  \ addr snf-lst
    list-get-links                          \ addr snf-link

    begin
        ?dup
    while
        dup link-get-data                   \ addr snf-link snf
        #2 pick swap                        \ addr snf-link addr snf
        struct-info-get-mma-xt execute      \ addr snf-link addr mma
        mma-within-array                    \ addr snf-link bool
        if
            dup  link-get-data              \ addr snf-link snf
            struct-info-get-name-xt execute \ addr snf-link c-addr u
            type                            \ addr snf-link
            drop                            \ addr
            struct-get-id                   \ id
            0= if
                ." -u"
            then

            exit
        then

        link-get-next
    repeat
                                            \ addr
    \ Default
    dup abs 0 <# #S rot sign #> type
;

\ Cycle through each stack item, displaying its struct type.
: .stack-structs
    ." Forth stack: <" depth dup abs 0 <# #S rot sign #> type ." > "
    depth 0=
    if
        exit
    then
    depth 0 do
        depth 1- i - pick

        dup list-mma mma-within-array
        if
            dup struct-get-id
            0= if
                ." List-u "
            else
                ." List-"
                dup list-get-length dup abs 0 <# #S rot sign #> type
                dup list-get-length
                0<> if
                        ." -"
                        dup list-get-links link-get-data
                        .stack-structs2
                    else
                        space
                    then
            then
            drop
        else
            .stack-structs2
        then
        space
    loop
;

' .stack-structs to .stack-structs-xt

: .abort ( c-addr u -- )
    cr type cr
    cr .s cr
    .stack-structs
    abort
;

' .abort to .abort-xt

