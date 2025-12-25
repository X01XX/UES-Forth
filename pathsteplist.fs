\ Functions for a PathStep list.

\ Check if tos is a list, if non-empty, with the first item being a pathstep.
: assert-tos-is-pathstep-list ( tos -- )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-pathstep
        drop
    then
;

\ Check if nos is a list, if non-empty, with the first item being a pathstep.
: assert-nos-is-pathstep-list ( nos tos -- )
    assert-nos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-pathstep
        drop
    then
;

: pathstep-list-deallocate ( rulc-lst0 -- )
    \ Check arg.
    assert-tos-is-pathstep-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ rulc-lst0 uc
    #2 < if
        \ Deallocate pathsteps in the list.
        [ ' pathstep-deallocate ] literal over      \ rulc-lst0 xt rulc-lst0
        list-apply                                  \ rulc-lst0

        \ Deallocate the list.
        list-deallocate
    else
        struct-dec-use-count
    then
;

\ Deallocate a list of lists of pathstep.
: pathstep-lol-deallocate ( rulc-lol0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ rulc-lol0 uc
    #2 < if
        \ Deallocate pathstep instances in the list.
        [ ' pathstep-list-deallocate ] literal over \ rulc-lol0 xt rulc-lol0
        list-apply                                  \ rulc-lol0

        \ Deallocate the list. 
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

: .pathstep-list ( rlc-lst -- )
    \ Check arg.
    assert-tos-is-pathstep-list

    s" (" type
    [ ' .pathstep ] literal swap    \ xt pathstep-list
    list-apply                      \
    s" )" type
;

\ Push a pathstep.
: pathstep-list-push ( pthstp1 pthstp-lst0 -- )
   \ Check arg.
    assert-tos-is-pathstep-list
    assert-nos-is-pathstep

    list-push-struct
;

\ Push a pathstep to end of list.
: pathstep-list-push-end ( pthstp1 pthstp-lst0 -- )
   \ Check arg.
    assert-tos-is-pathstep-list
    assert-nos-is-pathstep

    list-push-end-struct
;

: pathstep-list-get-steps-fc ( cngsc-needed regc-from pthstp-lst1 -- pthstp-lst )
    \ cr ." pathstep-list-get-steps-fc: start:       " .stack-structs-xt execute cr
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-changescorr

    \ Init return pathstep list.
    list-new swap                   \ cngsc-needed regc-from ret-lst pthstp-lst1
    list-get-links                  \ cngsc-needed regc-from ret-lst pthstp-lst-link

    begin
        ?dup
    while
        dup link-get-data           \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx
        dup pathstep-get-changes    \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx pthstp-cngsc
        #5 pick                     \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx pthstp-cngsc cngsc-needed
        changescorr-intersect       \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx bool

        if
            dup pathstep-get-initial-regions    \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx pthstp-i-regsc
            #4 pick                             \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx pthstp-i-regsc regc-from
            regioncorr-intersects               \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx bool
            if
                #4 pick                                 \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx cngsc-needed
                over                                    \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx cngsc-needed pthstpx
                pathstep-calc-number-unwanted-changes   \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx

                #2 pick                                 \ cngsc-needed regc-from ret-lst pthstp-lst-link pthstpx ret-lst
                pathstep-list-push                      \ cngsc-needed regc-from ret-lst pthstp-lst-link

            else
                drop                            \ cngsc-needed regc-from ret-lst pthstp-lst-link
            then
        else
            drop                    \ cngsc-needed regc-from ret-lst pthstp-lst-link
        then

        link-get-next               \ cngsc-needed regc-from ret-lst pthstp-lst-link-nxt
    repeat
                                    \ cngsc-needed regc-from ret-lst
    nip nip                         \ ret-lst
;

: pathstep-list-filter-min-number-unwanted-changes ( pthstp-lst0 -- pthstp-lst )
    \ Check args.
    assert-tos-is-pathstep-list

    \ Init return list.
    list-new swap               \ ret-lst pthstp-lst0

    \ Init min-num.
    9999                        \ ret-lst pthstp-lst0 min

    \ Prep for loop.
    over list-get-links         \ ret-lst pthstp-lst0 min link

    \ Find minimum number-unwanted-changes.
    begin
        ?dup
    while
        dup link-get-data                       \ ret-lst pthstp-lst0 min link pthstpx
        pathstep-get-number-unwanted-changes    \ ret-lst pthstp-lst0 min link num
        rot min swap                            \ ret-lst pthstp-lst0 min link

        link-get-next
    repeat

                                                \ ret-lst pthstp-lst0 min
    \ Get pathsteps with min number-unwanted-changes.
    swap list-get-links                         \ ret-lst min link

    begin
        ?dup
    while
        dup link-get-data                       \ ret-lst min link pathstpx
        dup
        pathstep-get-number-unwanted-changes    \ ret-lst min link pathstpx num
        #3 pick =                               \ ret-lst min link pathstpx bool
        if                                      \ ret-lst min link pathstepx
            #3 pick                             \ ret-lst min link pathstepx ret-lst
            pathstep-list-push                  \ ret-lst min link
        else
            drop                                \ ret-lst min link
        then
        
        link-get-next
    repeat
                                                \ ret-lst min
    drop
;

\ Remove a pathstep from a pathstep-list.
: pathstep-list-remove-item ( inx1 pthstp-lst0 -- pthstpx true | false )
    \ Check arg.
    assert-tos-is-pathstep-list

    list-remove-item        \ pthstpx true | false
    if
        dup struct-dec-use-count
        true
    else
        false
    then
;
