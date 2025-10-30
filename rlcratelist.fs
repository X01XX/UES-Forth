\ Region-list-core + rate, rlcrate, list functions.

\ Deallocate a rlcrate list.                                                                                                             
: rlcrate-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' rlcrate-deallocate ] literal over       \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

: rlcrate-rlc-eq ( rlcrt1 rlcrt0 -- bool )
    \ Check args.
    assert-tos-is-rlcrate
    assert-nos-is-rlcrate

    \ Get rlc part of rlcrate.
    rlcrate-get-rlc     \ rlcrt1 rlc0
    swap                \ rlc0 rlcrt1
    rlcrate-get-rlc     \ rlc0 rlc1

    region-list-corr-eq
;

\ Push a rlcrate to a rlcrate-list.
: rlcrate-list-push ( rlcrt1 rlcrate-lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-rlcrate

    \ Check for 0/0 rate.
    over rlcrate-rate-all-zero      \ rlc2 rlcrate-lst0 bool
    abort" rate is 0/0 ?"

    \ Check for dup rlc.
    [ ' rlcrate-rlc-eq ] literal    \ rlc1 rlcrate-lst0 xt
    #2 pick                         \ rlc1 rlcrate-lst0 xt rlc1
    #2 pick                         \ rlc1 rlcrate-lst0 xt rlc1 rlcrate-lst0

    list-member                     \ rlc1 rlcrate-lst0 bool
    abort" Duplicate rlc in list?"

    over struct-inc-use-count
    list-push
;
 
\ Print a rlcrate-list
: .rlcrate-list ( list0 -- )
    \ Check args.
    assert-tos-is-list

    .rlcrate-xt swap .list
;

: rlcrate-list-to-rlc-list ( lst0 -- rlc-lst )
    \ Check args.
    assert-tos-is-list

    \ Init return list.
    list-new swap          \ ret-lst lst0

    \ Prep for loop.
    list-get-links          \ ret-lst link

    begin
        ?dup
    while
        dup link-get-data   \ ret-lst link rlcrtx
        rlcrate-get-rlc     \ ret-lst link rlc
        #2 pick             \ ret-lst link rlc ret-lst
        rlc-list-push       \ ret-lst link

        link-get-next
    repeat
                            \ ret-lst
;

: rlcrate-list-match-rate-negative-rlcs ( n lst0 -- rlc-lst )
    \ Check args.
    assert-tos-is-list

    \ Init return list.
    list-new -rot               \ ret-lst n lst0

    \ Prep for loop.
    list-get-links              \ ret-lst n link

    begin
        ?dup
    while
        \ Compare rlcrate negative value with passed value.
        dup link-get-data       \ ret-lst n link rlcrtx
        rlcrate-get-rate        \ ret-lst n link rate
        rate-get-negative       \ ret-lst n link n2
        #2 pick                 \ ret-lst n link n2 n
        =                       \ ret-lst n link bool
        if
            dup link-get-data   \ ret-lst n link rlcrtx
            rlcrate-get-rlc     \ ret-lst n link rlc
            #3 pick             \ ret-lst n link rlc ret-lst
            rlc-list-push       \ ret-lst n link
        then

        link-get-next
    repeat
                            \ ret-lst n
    drop                    \ ret-lst
;
