\ Functions for a list of Region-list-core + rate, regioncorrrate, structures.

\ Check if tos is an empty list, or has a regioncorrrate instance as its first item.
: assert-tos-is-regioncorrrate-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-regioncorrrate
        drop
    then
;

\ Check if nos is an empty list, or has a regioncorrrate instance as its first item.
: assert-nos-is-regioncorrrate-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-regioncorrrate
        drop
    then
;

\ Deallocate a regioncorrrate list.
: regioncorrrate-list-deallocate ( regcr-lst0 -- )
    \ Check arg.
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

\ Return true if a regioncorrrate is a member of a list.
: regioncorrrate-list-member ( regcr1 regcr-lst0 -- )
    \ Check args.
    assert-tos-is-regioncorrrate-list
    assert-nos-is-regioncorrrate

    \ Check for dup regc.
    [ ' regioncorrrate-eq ] literal \ regc1 regcr-lst0 xt
    #2 pick                         \ regc1 regcr-lst0 xt regc1
    #2 pick                         \ regc1 regcr-lst0 xt regc1 regcr-lst0

    list-member                     \ regc1 regcr-lst0 bool
    nip nip                         \ bool
;

\ Push a regioncorrrate to a regioncorrrate-list.
: regioncorrrate-list-push ( regcr1 regcr-lst0 -- )
    \ Check args.
    assert-tos-is-regioncorrrate-list
    assert-nos-is-regioncorrrate

    \ Check for 0/0 rate.
    over regioncorrrate-rate-all-zero      \ regc2 regcr-lst0 bool
    abort" rate is 0/0 ?"

    2dup regioncorrrate-list-member        \ regc2 regcr-lst0 bool
    abort" Duplicate regioncorr in list?"

    list-push-struct
;

\ Print a regioncorrrate-list
: .regioncorrrate-list ( regcr-lst0 -- )
    \ Check arg.
    assert-tos-is-regioncorrrate-list

    .regioncorrrate-xt swap .list
;

\ Return a regioncorr list, without the rates.
: regioncorrrate-list-to-regioncorr-list ( regcr-lst0 -- regc-lst )
    \ Check arg.
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

\ Return regioncorr-list of regioncorrrates the have a given negative rate.
: regioncorrrate-list-match-rate-negative ( n lst0 -- regc-lst )
    \ Check arg.
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

\ Return a rate for a given regioncorr.
: regioncorrrate-rate-regioncorr ( regc1 regcr-lst0 -- rate )
    \ Check args.
    assert-tos-is-regioncorrrate-list
    assert-nos-is-regioncorr

    0 0 rate-new swap           \ regc1 ret-rate regcr-lst0
    list-get-links              \ regc1 ret-rate link

    begin
        ?dup
    while
        dup link-get-data                   \ regc1 ret-rate link regctx
        #3 pick                             \ regc1 ret-rate link regctx regc1
        over regioncorrrate-get-regioncorr  \ regc1 ret-rate link regctx regc1 regcx
        regioncorr-superset                 \ regc1 ret-rate link regctx bool
        if
            regioncorrrate-get-rate         \ regc1 ret-rate link ratex
            #2 pick                         \ regc1 ret-rate link ratex rate
            rate-add                        \ regc1 ret-rate link
        else
            drop
        then

        link-get-next
    repeat
                                            \ regc1 ret-rate
    nip                                     \ ret-rate
;

: regioncorrrate-list-more-positive-regioncorrs ( rate1 regcr-lst0 -- regcr-lst ) \ Return a list of more-positive regioncorrrates.
    \ Check args.
    assert-tos-is-regioncorrrate-list
    assert-nos-is-rate

    \ Init return list.
    list-new -rot                           \ ret-lst rate1 regcr-lst0

    \ Scan items in the list.
    list-get-links                          \ ret-lst rate1 link
    begin
        ?dup
    while
        over                                \ ret-lst rate1 link rate1
        over link-get-data                  \ ret-lst rate1 link rate1 reginocorrrate
        regioncorrrate-get-rate             \ ret-lst rate1 link rate1 ratex
        dup rate-get-negative               \ ret-lst rate1 link rate1 ratex u-neg
        0= if
            rate-more-positive                  \ ret-lst rate1 link bool
            if
                dup link-get-data               \ ret-lst rate1 link regcrx
                #3 pick                         \ ret-lst rate1 link regcx ret-lst
                list-push-struct                \ ret-lst rate1 link
            then
        else                                    \ ret-lst rate1 link rate1 ratex
            2drop                               \ ret-lst rate1 link
        then

        link-get-next
    repeat
                                            \ ret-lst rate1
    drop
;
