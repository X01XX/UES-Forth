\ Functions for a PathStep list.

\ Check if tos is a list, if non-empty, with the first item being a pathstep.
: assert-tos-is-pathstep-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-pathstep
        drop
    then
;

\ Check if nos is a list, if non-empty, with the first item being a pathstep.
: assert-nos-is-pathstep-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-pathstep
        drop
    then
;

\ Check if 4os is a list, if non-empty, with the first item being a pathstep.
: assert-4os-is-pathstep-list ( 4os 3os nos tos -- 4os 3os nos tos )
    #3 pick assert-tos-is-list
    dup list-is-empty
    if
        drop
    else
        list-get-links link-get-data
        assert-tos-is-pathstep
        drop
    then
;

: pathstep-list-deallocate ( plnstp-lst0 -- )
    \ Check arg.
    assert-tos-is-pathstep-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ plnstp-lst0 uc
    #2 < if
        \ Deallocate pathsteps in the list.
        [ ' pathstep-deallocate ] literal over      \ plnstp-lst0 xt plnstp-lst0
        list-apply                                  \ plnstp-lst0

        \ Deallocate the list.
        list-deallocate
    else
        struct-dec-use-count
    then
;

\ Deallocate a list of lists of pathstep.
: pathstep-lol-deallocate ( plnstp-lol0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ plnstp-lol0 uc
    #2 < if
        \ Deallocate pathstep instances in the list.
        [ ' pathstep-list-deallocate ] literal over \ plnstp-lol0 xt plnstp-lol0
        list-apply                                  \ plnstp-lol0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

: .pathstep-list ( pthstp-lst -- )
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

\ Return a list of pathsteps that have initial regions intersecting a given regioncorr.
: pathstep-list-initial-region-intersection ( regc1 pthstp-lst0 -- pthstp-lst )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr

    \ Init return list.
    list-new -rot                           \ ret-lst regc1 pthstp-lst0

    \ Prep for loop.
    list-get-links                          \ ret-lst regc1 link

    begin
        ?dup
    while
        dup link-get-data                   \ ret-lst regc1 link pthstpx
        dup pathstep-get-initial-regions    \ ret-lst regc1 link pthstpx pthstp-i
        #3 pick                             \ ret-lst regc1 link pthstpx pthstp-i regc1
        regioncorr-intersects               \ ret-lst regc1 link pthstpx bool
        if                                  \ ret-lst regc1 link pthstpx
            #3 pick                         \ ret-lst regc1 link pthstpx ret-lst
            pathstep-list-push              \ ret-lst regc1 link
        else
            drop                            \ ret-lst regc1 link
        then

        link-get-next
    repeat
                                            \ ret-lst regc1
    drop                                    \ ret-lst
;

\ Return steps that may be used to get from regc-from to regc-to.
\ The given pathstep-list will contain only pathsteps that have initial-regions that intersect regc-from.
\ A chosen pathstep will have a needed change.
: pathstep-list-get-steps-fc2 ( regc-to regc-from pthstp-lst1 -- pthstp-lst )
    \ cr ." pathstep-list-get-steps-fc: start:       " .stack-structs-xt execute cr
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    \ Get changes-needed.
    #2 pick                         \ regc-to regc-from pthstp-lst1 regc-to
    #2 pick                         \ regc-to regc-from pthstp-lst1 regc-to regc-from
    changescorr-new-regc-to-regc    \ regc-to regc-from pthstp-lst1 cngsc-needed'
    cr ." changes needed: " dup .changescorr cr
    -rot                            \ regc-to cngs-needed' regc-from pthstp-lst1

    \ Init return pathstep list.
    list-new swap                   \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst1
    list-get-links                  \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link

    begin
        ?dup
    while
        dup link-get-data           \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx

        \ Check changes intersection
        dup pathstep-get-changes    \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx pthstp-cngsc
        #5 pick                     \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx pthstp-cngsc cngsc-needed
        \ cr ." ccompare changes: path: " over .changescorr space ." needed: " dup .changescorr cr
        changescorr-intersect       \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx bool

        if
            dup pathstep-get-initial-regions    \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx pthstp-i-regsc
            #4 pick                             \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx pthstp-i-regsc regc-from
            regioncorr-intersects               \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx bool
            if
                #4 pick                                 \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx cngsc-needed
                over                                    \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx cngsc-needed pthstpx
                pathstep-calc-number-unwanted-changes   \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx

                #2 pick                                 \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx ret-lst
                pathstep-list-push                      \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link

            else
                drop                                    \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link
            then
        else
            drop                                \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link
        then

        link-get-next               \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link-nxt
    repeat
                                    \ regc-to cngsc-needed' regc-from ret-lst
    nip                             \ regc-to cngsc-needed' ret-lst
    swap changescorr-deallocate     \ regc-to ret-lst
    nip                             \ ret-lst
;

\ Return steps that may be used to get from regc-from to regc-to.
\ A chosen pathstep will have an initial region that intersects regc-from,
\ and either a needed change, or regc-to also intersects the pathsetp initial region.
: pathstep-list-get-steps-fc ( regc-to regc-from pthstp-lst1 -- pthstp-lst )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    2dup                                        \ regc-to regc-from pthstp-lst1 regc-from pthstp-lst1
    pathstep-list-initial-region-intersection   \ regc-to regc-from pthstp-lst1 pthstp-lst2'

    #3 pick                                     \ regc-to regc-from pthstp-lst1 pthstp-lst2' regc-to
    over                                        \ regc-to regc-from pthstp-lst1 pthstp-lst2' regc-to pthstp-lst2'
    pathstep-list-initial-region-intersection   \ regc-to regc-from pthstp-lst1 pthstp-lst2' pthstp-lst3'

    dup list-is-empty
    if
        pathstep-list-deallocate                \ regc-to regc-from pthstp-lst1 pthstp-lst2'
        #3 pick                                 \ regc-to regc-from pthstp-lst1 pthstp-lst2' regc-to
        #3 pick                                 \ regc-to regc-from pthstp-lst1 pthstp-lst2' regc-to regc-from
        #2 pick                                 \ regc-to regc-from pthstp-lst1 pthstp-lst2' regc-to regc-from pthstp-lst2'
        pathstep-list-get-steps-fc2             \ regc-to regc-from pthstp-lst1 pthstp-lst2' pthstp-lst3'
        
    then
    swap pathstep-list-deallocate               \ regc-to regc-from pthstp-lst1 pthstp-lst3'
    2nip nip                                    \ pthstp-lst3
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
