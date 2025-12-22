\ Functions for a list of Region-list-core + rate, regioncorrrate, structures.

\ Deallocate a regioncorrrate list.
: regioncorrrate-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' regioncorrrate-deallocate ] literal over       \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

: regioncorrrate-regioncorr-eq ( regcrt1 regcrt0 -- bool )
    \ Check args.
    assert-tos-is-regioncorrrate
    assert-nos-is-regioncorrrate

    \ Get regioncorr part of regioncorrrate.
    regioncorrrate-get-regioncorr   \ regctr1 regc0
    swap                            \ regc0 regcrt1
    regioncorrrate-get-regioncorr   \ regc0 regc1

    regioncorr-eq
;

\ Push a regioncorrrate to a regioncorrrate-list.
: regioncorrrate-list-push ( regcrt1 regcrt-lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-regioncorrrate

    \ Check for 0/0 rate.
    over regioncorrrate-rate-all-zero      \ regc2 regcrt-lst0 bool
    abort" rate is 0/0 ?"

    \ Check for dup regc.
    [ ' regioncorrrate-regioncorr-eq ] literal    \ regc1 regcrt-lst0 xt
    #2 pick                         \ regc1 regcrt-lst0 xt regc1
    #2 pick                         \ regc1 regcrt-lst0 xt regc1 regcrt-lst0

    list-member                     \ regc1 regcrt-lst0 bool
    abort" Duplicate regioncorr in list?"

    over struct-inc-use-count
    list-push
;

\ Print a regioncorrrate-list
: .regioncorrrate-list ( list0 -- )
    \ Check args.
    assert-tos-is-list

    .regioncorrrate-xt swap .list
;

: regioncorrrate-list-to-regioncorr-list ( regcrt-lst0 -- regc-lst )
    \ Check args.
    assert-tos-is-list

    \ Init return list.
    list-new swap                       \ ret-lst lst0

    \ Prep for loop.
    list-get-links                      \ ret-lst link

    begin
        ?dup
    while
        dup link-get-data               \ ret-lst link regcrtx
        regioncorrrate-get-regioncorr   \ ret-lst link regc
        #2 pick                         \ ret-lst link regc ret-lst
        list-push-struct                \ ret-lst link

        link-get-next
    repeat
                            \ ret-lst
;

: regioncorrrate-list-match-rate-negative ( n lst0 -- rlc-lst )
    \ Check args.
    assert-tos-is-list

    \ Init return list.
    list-new -rot                           \ ret-lst n lst0

    \ Prep for loop.
    list-get-links                          \ ret-lst n link

    begin
        ?dup
    while
        \ Compare regioncorrrate negative value with passed value.
        dup link-get-data                   \ ret-lst n link rlcrtx
        regioncorrrate-get-rate             \ ret-lst n link rate
        rate-get-negative                   \ ret-lst n link n2
        #2 pick                             \ ret-lst n link n2 n
        =                                   \ ret-lst n link bool
        if
            dup link-get-data               \ ret-lst n link rlcrtx
            regioncorrrate-get-regioncorr   \ ret-lst n link regc
            #3 pick                         \ ret-lst n link regc ret-lst
            list-push-struct                \ ret-lst n link
        then

        link-get-next
    repeat
                                            \ ret-lst n
    drop                                    \ ret-lst
;
