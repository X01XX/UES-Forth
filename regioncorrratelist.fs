\ Functions for a list of Region-list-core + rate, regioncorrrate, structures.

\ Check if tos is a list, if non-empty, with the first item being a regioncorrrate.
: assert-tos-is-regioncorrrate-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-regioncorrrate
        drop
    then
;

\ Deallocate a regioncorrrate list.
: regioncorrrate-list-deallocate ( regcr-lst0 -- )
    assert-tos-is-regioncorrrate-list

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

\ Push a regioncorrrate to a regioncorrrate-list.
: regioncorrrate-list-push ( regcr1 regcr-lst0 -- )
    \ Check args.
    assert-tos-is-regioncorrrate-list
    assert-nos-is-regioncorrrate

    \ Check for 0/0 rate.
    over regioncorrrate-rate-all-zero      \ regc2 regcr-lst0 bool
    abort" rate is 0/0 ?"

    \ Check for dup regc.
    [ ' regioncorrrate-eq ] literal \ regc1 regcr-lst0 xt
    #2 pick                         \ regc1 regcr-lst0 xt regc1
    #2 pick                         \ regc1 regcr-lst0 xt regc1 regcr-lst0

    list-member                     \ regc1 regcr-lst0 bool
    abort" Duplicate regioncorr in list?"

    over struct-inc-use-count
    list-push
;

\ Print a regioncorrrate-list
: .regioncorrrate-list ( regcr-lst0 -- )
    \ Check args.
    assert-tos-is-regioncorrrate-list

    .regioncorrrate-xt swap .list
;

: regioncorrrate-list-to-regioncorr-list ( regcr-lst0 -- regc-lst )
    \ Check args.
    assert-tos-is-regioncorrrate-list

    \ Init return list.
    list-new swap                       \ ret-lst regcr-lst0

    \ Prep for loop.
    list-get-links                      \ ret-lst link

    begin
        ?dup
    while
        dup link-get-data               \ ret-lst link regcrx
        regioncorrrate-get-regioncorr   \ ret-lst link regc
        #2 pick                         \ ret-lst link regc ret-lst
        list-push-struct                \ ret-lst link

        link-get-next
    repeat
                            \ ret-lst
;

: regioncorrrate-list-match-rate-negative ( n lst0 -- regc-lst )
    \ Check args.
    assert-tos-is-regioncorrrate-list

    \ Init return list.
    list-new -rot                           \ ret-lst n lst0

    \ Prep for loop.
    list-get-links                          \ ret-lst n link

    begin
        ?dup
    while
        \ Compare regioncorrrate negative value with passed value.
        dup link-get-data                   \ ret-lst n link regcrx
        regioncorrrate-get-rate             \ ret-lst n link rate
        rate-get-negative                   \ ret-lst n link n2
        #2 pick                             \ ret-lst n link n2 n
        =                                   \ ret-lst n link bool
        if
            dup link-get-data               \ ret-lst n link regcrx
            regioncorrrate-get-regioncorr   \ ret-lst n link regc
            #3 pick                         \ ret-lst n link regc ret-lst
            list-push-struct                \ ret-lst n link
        then

        link-get-next
    repeat
                                            \ ret-lst n
    drop                                    \ ret-lst
;
