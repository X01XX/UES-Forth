\ Functions for a PathStep list.

\ Check if tos is an empty list, or has a pathstep instance as its first item.
: assert-tos-is-pathstep-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty?
    if
        dup list-get-links link-get-data
        assert-tos-is-pathstep
        drop
    then
;

\ Check if nos is an empty list, or has a pathstep instance as its first item.
: assert-nos-is-pathstep-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty?
    if
        over list-get-links link-get-data
        assert-tos-is-pathstep
        drop
    then
;

\ Check if 4os is a list, if non-empty, with the first item being a pathstep.
: assert-4os-is-pathstep-list ( 4os 3os nos tos -- 4os 3os nos tos )
    assert-4os-is-list
    #3 pick list-is-not-empty?
    if
        #3 pick list-get-links link-get-data
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
    \ cr ." pushing: " over .pathstep cr

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
        regioncorr-intersects?              \ ret-lst regc1 link pthstpx bool
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
    cr ." pathstep-list-get-steps-fc: start:       " dup .pathstep-list cr
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
    dup list-get-length
    cr ." pathstep list length: " dec. cr
    list-get-links                  \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link

    begin
        ?dup
    while
        dup link-get-data           \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx

        \ Check changes intersection
        dup pathstep-get-changes    \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx pthstp-cngsc
        #5 pick                     \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx pthstp-cngsc cngsc-needed
        cr ." compare changes: path: " over .changescorr space ." needed: " dup .changescorr space ." to " #2 pick .pathstep cr
        changescorr-intersect?      \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx bool

        if
            dup pathstep-get-initial-regions    \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx pthstp-i-regsc
            #4 pick                             \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx pthstp-i-regsc regc-from
            regioncorr-intersects?              \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx bool
            if
                #4 pick                                 \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx cngsc-needed
                over                                    \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx cngsc-needed pthstpx
                ?pathstep-calc-number-unwanted-changes   \ regc-to cngsc-needed' regc-from ret-lst pthstp-lst-link pthstpx

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

    dup list-is-empty?
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
    #9999                       \ ret-lst pthstp-lst0 min

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
: pathstep-list-remove-item ( inx1 pthstp-lst0 -- pthstpx )
    \ Check arg.
    assert-tos-is-pathstep-list

    list-remove-item        \ pthstpx
    dup struct-dec-use-count
;

: pathstep-list-any-eq-initial-subset-result? ( pthstp1 pthstp-lst0 -- bool )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-pathstep

    [ ' pathstep-eq-initial-subset-result? ] literal    \ pthstp1 pthstp-lst0  xt
    -rot                                                \ xt pthstp1 pthstp-lst0

    list-member                                         \ pthstp2 t | f
;

\ Remove the first eq initial, superset result, pathstep from a pathstep-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a pathstep was removed.
: pathstep-list-remove-eq-initial-superset-result ( pthstp1 pthstp-lst0 -- bool )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-pathstep

    [ ' pathstep-eq-initial-superset-result? ] literal  \ pthstp1 pthstp-lst0  xt
    -rot                                                \ xt pthstp1 pthstp-lst0

    list-remove                                         \ pthstp2 t | f
    if
        \ cr ." Pathstep removed: " dup .pathstep cr
        pathstep-deallocate
        true
    else
        false
    then
;

\ Push a pathstep onto a list, if there are no supersets in the list.
\ If there are no supersets in the list, delete any subsets and push the pathstep.
\ Return true if the pathstep is added to the list.
: pathstep-list-push-nosups ( pthstp1 pthstp-lst0 -- flag )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-pathstep
    \ cr ." Add " over .pathstep space ." to: " dup .pathstep-list cr

    \ Return if any pathstep in the list is a superset of pthstp1.
    2dup                                    \ pthstp1 pthstp-lst0 pthstp1 pthstp-lst0
    [ ' pathstep-eq-initial-subset-result? ] literal    \ pthstp1 pthstp-lst0 pthstp1 pthstp-lst0 xt
    -rot                                    \ pthstp1 pthstp-lst0 xt pthstp1 pthstp-lst0
    list-member                             \ pthstp1 pthstp-lst0 flag
    if
        2drop
        false
        exit
    then
                                            \ pthstp1 pthstp-lst0

    \ Remove all supersets.
    begin
        2dup                                            \ pthstp1 pthstp-lst0 pthstp1 pthstp-lst0
        pathstep-list-remove-eq-initial-superset-result \ pthstp1 pthstp-lst0 | flag
    while
    repeat

    \ Add pathstep to list.                 \ pthstp1 pthstp-lst0
    pathstep-list-push
    true
;

\ Return a list of pathsteps with changes that intersect a given changescorr.
: pathstep-list-intersect-changes ( cngsc1 pthstp-lst0 -- pthstp-lst )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-changescorr

    \ Init return list.
    list-new                            \ cngsc1 pthstp-lst0 | ret-lst

    \ Prep for loop.
    over list-get-links                 \ cngsc1 pthstp-lst0 | ret-lst pthstp-link

    begin
        ?dup
    while
        #3 pick                         \ cngsc1 pthstp-lst0 | ret-lst pthstp-link cngsc1
        over link-get-data              \ cngsc1 pthstp-lst0 | ret-lst pthstp-link cngsc1 pthstpx
        pathstep-intersects-changes     \ cngsc1 pthstp-lst0 | ret-lst pthstp-link bool
        if
            dup link-get-data           \ cngsc1 pthstp-lst0 | ret-lst pthstp-link pthstpx
            #2 pick                     \ cngsc1 pthstp-lst0 | ret-lst pthstp-link pthstpx ret-lst
            pathstep-list-push          \ cngsc1 pthstp-lst0 | ret-lst pthstp-link
        then

        link-get-next
    repeat

    \ Return                            \ cngsc1 pthstp-lst0 | ret-lst
    nip nip
;

\ Update pathsteps in a list with the number of unwanted changes,
\ going from regc-from to pathstep result regions.
\ A pathstep rulecorr change can revert an unwanted change required to translate
\ from regc-from to the pathstep's initial regioncorr.
: pathstep-list-set-number-unwanted-changes-fc ( regc-to regc-from pthstp-lst0 -- )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    \ Get needed changes.
    #2 pick #2 pick                         \ regc-to regc-from pthstp-lst0 | regc-to regc-from
    changescorr-new-regc-to-regc            \ regc-to regc-from pthstp-lst0 | cngsc'

    over                                    \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-lst0
    list-get-links                          \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link

    \ For each pathstep in list.
    begin
        ?dup
    while
        \ Get next pathstep.
        dup link-get-data                       \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx

        \ Get rulec for regc-from to pathstep initial regc.
        dup pathstep-get-initial-regions        \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx pthstpx-initial
        #5 pick                                 \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx pthstpx-initial regc-from
        rulecorr-new-regc-to-regc               \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc-to-pathstpx'

        \ Combine rule to pathstep with pathstep rule.
        over pathstep-get-rules                 \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc-to-pthstpx' pthstp-rulc
        over                                    \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc-to-pthstpx' pthstp-rulc rulc-to-pthstpx'
        rulecorr-combine                        \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc-to-pthstpx' rulc'
        swap rulecorr-deallocate                \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc'

        \ Get unwanted changes.
        dup rulecorr-get-changes                \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc' rul-cngs'
        swap rulecorr-deallocate                \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs'
        #3 pick                                 \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs' cngsc'
        changescorr-invert                      \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs' cngsc-inv'
        2dup changescorr-intersection           \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs' cngsc-inv' cngsc-int'
        swap changescorr-deallocate             \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs' cngsc-int'
        swap changescorr-deallocate             \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx cngsc-int'

        \ Get number unwanted changes.
        dup changescorr-number-changes          \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx cngsc-int' num-cngs
        swap changescorr-deallocate             \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx num-cngs

        \ Set pathstep number unwanted changes.
        swap                                    \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link num-cngs pthstpx
        pathstep-set-number-unwanted-changes    \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link

        link-get-next
    repeat
                                            \ regc-to regc-from pthstp-lst0 | cngsc'
    changescorr-deallocate                  \ regc-to regc-from pthstp-lst0
    3drop                                   \
;

\ Update pathsteps in a list with the number of unwanted changes.
\ going from pathstep initial regions to regc-to.
\ Translating from a pathstep's result regions to regc-to can revert an unwanted change in the pathstep rulecorr.
: pathstep-list-set-number-unwanted-changes-bc ( regc-to regc-from pthstp-lst0 -- )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    \ Get needed changes.
    #2 pick #2 pick                         \ regc-to regc-from pthstp-lst0 | regc-to regc-from
    changescorr-new-regc-to-regc            \ regc-to regc-from pthstp-lst0 | cngsc'

    over                                    \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-lst0
    list-get-links                          \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link

    \ For each pathstep in list.
    begin
        ?dup
    while
        \ Get next pathstep.
        dup link-get-data                       \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx
        \ cr #4 pick ."  from " .regioncorr space ." to " dup .pathstep cr

        \ Get a pathstep result regc to regc-to.
        #5 pick                                 \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx regc-to
        over pathstep-get-result-regions        \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx regc-to pthstpx-initial
        rulecorr-new-regc-to-regc               \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc-from-pathstpx'

        \ Combine rule to pathstep with pathstep rule.
        dup                                     \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc-from-pthstpx' rulc-from-pthstpx'
        #2 pick pathstep-get-rules              \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc-from-pthstpx' rulc-from-pthstpx' pthstp-rulc
        rulecorr-combine                        \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc-from-pthstpx' rulc'
        swap rulecorr-deallocate                \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc'

        \ Get unwanted changes.
        dup rulecorr-get-changes                \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rulc' rul-cngs'
        swap rulecorr-deallocate                \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs'
        #3 pick                                 \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs' cngsc'
        changescorr-invert                      \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs' cngsc-inv'
        2dup changescorr-intersection           \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs' cngsc-inv' cngsc-int'
        swap changescorr-deallocate             \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx rul-cngs' cngsc-int'
        swap changescorr-deallocate             \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx cngsc-int'

        \ Get number unwanted changes.
        dup changescorr-number-changes          \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx cngsc-int' num-cngs
        swap changescorr-deallocate             \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link pthstpx num-cngs

        \ Set pathstep number unwanted changes.
        swap                                    \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link num-cngs pthstpx
        pathstep-set-number-unwanted-changes    \ regc-to regc-from pthstp-lst0 | cngsc' pthstp-link

        link-get-next
    repeat
                                            \ regc-to regc-from pthstp-lst0 | cngsc'
    changescorr-deallocate                  \ regc-to regc-from pthstp-lst0
    3drop                                   \
;

\ Return a list of pathsteps that could be a next step from the current regions
\ to the goal regions.
\ 1) They contain at least one needed change from start to goal regions and
\ 2) Pathstep initial regions are a superset of the start regions, or
\    moving from the start regions to the pathstep's initial regions does not require
\    changes needed to get from the start regions to the goal regions,
\    which would make use of the step premature.
: pathstep-list-possible-next-steps ( regc-to2 regc-from1 pthstp-lst0 -- pthstp-lst t | f )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr
    \ cr ." pathstep-list-possible-next-steps: start: from: "  over .regioncorr space ." to: " #2 pick .regioncorr cr

    \ Get needed changes.
    #2 pick #2 pick                             \ regc-to2 regc-from1 pthstp-lst0 | regc-to2 regc-from1
    changescorr-new-regc-to-regc                \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned'
    \ cr ." pathstep-list-possible-next-steps: changes needed: " dup .changescorr cr

    \ Init return list.
    list-new                                    \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst

    \ Prep for loop.
    #2 pick                                     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-lst0
    list-get-links                              \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link

    \ For each list pathstep.
    begin
        ?dup
    while
        \ Check for needed changes.
        #2 pick                             \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-ned'
        over link-get-data                  \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-ned' pthstpx
        pathstep-intersect-changes?         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
        if
            \ Compare regc-from to pathstep initial regions.
            #4 pick                                 \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-from1
            over link-get-data                      \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-from1 pthstpx
            pathstep-get-initial-regions            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-from1 regc-stp
            regioncorr-superset?                    \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
            if
                \ Check if regc-from is a subset of pathstep result regions.
                #4 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-from1
                over link-get-data              \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-from1 pthstpx
                pathstep-get-result-regions     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-from1 regc-stp-rslt
                regioncorr-superset?            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
                if
                else
                    \ Result regions do not intersect.
                    \ Add pathstep.
                    dup link-get-data       \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx
                    \ cr ." pathstep-list-possible-next-steps: pathstep accepted: " dup .pathstep cr
                    #2 pick                 \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx ret-lst
                    list-push-struct        \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link
                then
            else
                dup link-get-data                   \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx

                \ Check if pathstep contains needed changes.
                pathstep-get-changes                \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx-cngs
                #3 pick                             \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx-cngs cngs-ned'
                changescorr-intersection            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx-cngs-ned'
                dup changescorr-null?               \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx-cngs-ned' bool
                swap changescorr-deallocate         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
                if
                else
                    \ Get changes needed from regc-from to pathstep initial regions.
                    dup link-get-data               \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx
                    pathstep-get-initial-regions    \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx-initial
                    #5 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx-initial regc-from1
                    changescorr-new-regc-to-regc    \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from1'

                    \ Check if any needed changes are required.
                    dup                             \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from1' cngsc-from1'
                    #4 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from1' cngsc-from1' cngsc-ned'
                    changescorr-intersection        \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from1' cngsc-from-int'
                    swap changescorr-deallocate     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from-int'
                    dup changescorr-null?           \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from-int' bool
                    swap changescorr-deallocate     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
                    if
                        \ No needed changes are required to get to the pathstep initial regions, save it
                        dup link-get-data           \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx
                        \ cr ." pathstep-list-possible-next-steps: pathstep accepted: " dup .pathstep cr
                        #2 pick                     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx ret-lst
                        list-push-struct            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link
                    then
                then
            then

        then

        link-get-next
    repeat
                                            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst

    \ Set number unwanted changes.
    #4 pick #4 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst regc-to2 regc-from1
    #2 pick                                 \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst regc-to2 regc-from1 ret-lst
    pathstep-list-set-number-unwanted-changes-fc

    \ Return.                               \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst
    swap changescorr-deallocate             \ regc-to2 regc-from1 pthstp-lst0 | ret-lst
    2nip                                    \ pthstp-lst0 | ret-lst
    nip                                     \ ret-lst

    dup list-is-empty?
    if
        list-deallocate
        false
        \ cr ." pathstep-list-possible-next-steps: false exit 1" cr
    else
        \ Find least-bad options.
        dup pathstep-list-filter-min-number-unwanted-changes    \ ret-lst ret-lst2
        swap pathstep-list-deallocate                           \ ret-lst2
        true
        \ cr ." pathstep-list-possible-next-steps: true exit 2" cr
    then
    \ cr ." pathstep-list-possible-next-steps: end: " .stack-gbl cr
;

\ Return a list of pathsteps that could be a previous step before ataining the goal regions.
\ 1) They contain at least one needed change from start to goal regions and
\ 2) Pathstep result regions intersect the goal region, or
\    moving from the pathstep's result regions to the goal regions does not require
\    changes needed to get from the start regions to the goal regions,
\    which would make use of the step premature, or ... postmature?
: ?pathstep-list-possible-previous-steps ( regc-to2 regc-from1 pthstp-lst0 -- pthstp-lst t | f )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr
    cr ." pathstep-list-possible-previous-steps: from: " over .regioncorr space ." to: " #2 pick .regioncorr cr

    \ Get needed changes.
    #2 pick #2 pick                         \ regc-to2 regc-from1 pthstp-lst0 | regc-to2 regc-from1
    changescorr-new-regc-to-regc            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned'
    cr ." pathstep-list-possible-previous-steps: changes needed: " dup .changescorr cr

    \ Init return list.
    list-new                                \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst

    \ Prep for loop.
    #2 pick                                 \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-lst0
    list-get-links                          \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link

    \ For each list pathstep.
    begin
        ?dup
    while
        \ Check for needed changes.
        #2 pick                             \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-ned'
        over link-get-data                  \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-ned' pthstpx
        pathstep-intersect-changes?         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
        if
            \ Compare regc-from to pathstep initial regions.
            #5 pick                             \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-to2
            over link-get-data                  \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-to2 pthstpx
            pathstep-get-result-regions         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-to2 regc-stp
            regioncorr-intersects?              \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
            if
                \ Regions intersect.
                dup link-get-data           \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx
                #2 pick                     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx ret-lst
                list-push-struct            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link
            else
                #5 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-to2
                over link-get-data              \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-to2 pthstpx
                pathstep-get-result-regions     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link regc-to2 pthstpx-result

                \ Get changes needed from regc-from to pathstep initial regions.
                changescorr-new-regc-to-regc    \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-to2'

                \ Check if any needed changes are required.
                dup                             \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-to2' cngsc-to2'
                #4 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-to2' cngsc-to2' cngsc-ned'
                changescorr-intersection        \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-to2' cngsc-to-int'
                swap changescorr-deallocate     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-to-int'
                dup changescorr-null?           \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-to-int' bool
                swap changescorr-deallocate     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
                if
                    \ No needed changes are required, save it
                    dup link-get-data           \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx
                    #2 pick                     \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx ret-lst
                    list-push-struct            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link
                then
            then
        then

        link-get-next
    repeat
                                            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst

    \ Set number unwanted changes.
    #4 pick #4 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst regc-to2 regc-from1
    #2 pick                                 \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst regc-to2 regc-from1 ret-lst
    pathstep-list-set-number-unwanted-changes-bc

    \ Return.                               \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst
    swap changescorr-deallocate             \ regc-to2 regc-from1 pthstp-lst0 | ret-lst
    2nip                                    \ pthstp-lst0 | ret-lst
    nip                                     \ ret-lst

    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
    \ cr ." pathstep-list-possible-previous-steps: end: " .stack-gbl cr
;


\ Return true if two regioncorrs both intersect at least one pathstep initial regions.
: pathstep-list-intersects-both? ( regc2 regc1 pthstp-lst0 -- bool )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    \ Prep for loop.
    list-get-links                          \ regc2 regc1 pthstp-link

    begin
        ?dup
    while
        over                                \ regc2 regc1 pthstp-link regc1
        over link-get-data                  \ regc2 regc1 pthstp-link regc1 pthstpx
        \ cr ." comparing " #4 pick .regioncorr space ." and " #3 pick .regioncorr space ." to " dup .pathstep cr
        pathstep-get-initial-regions        \ regc2 regc1 pthstp-link regc1 pthstp-init
        regioncorr-intersects?              \ regc2 regc1 pthstp-link bool
        if
            #2 pick                         \ regc2 regc1 pthstp-link regc2
            over link-get-data              \ regc2 regc1 pthstp-link regc2 pthstpx
            pathstep-get-initial-regions    \ regc2 regc1 pthstp-link regc2 pthstp-init
            regioncorr-intersects?          \ regc2 regc1 pthstp-link bool
            if
                2drop drop
                true
                exit
            then
        then

        link-get-next
    repeat
                                            \ regc2 regc1
    2drop
    false
;

\ Return a list of pathsteps where a given regioncorr intersects
\ a pathstep's initial regions, while the intersection is not
\ a subset of the pathstep's result regions.
\ For pathstep forward-chaining.
: pathstep-list-intersecting-fc ( regc1 pthstp-lst0 -- pthstp-lst t | f )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr

    \ Init return list.
    list-new swap                           \ regc1 ret-lst pthstp-link

    \ Prep for loop.
    list-get-links                          \ regc1 ret-lst pthstp-link

    begin
        ?dup
    while
        #2 pick                             \ regc1 ret-lst pthstp-link regc1
        over link-get-data                  \ regc1 ret-lst pthstp-link regc1 pthstpx
        pathstep-get-initial-regions        \ regc1 ret-lst pthstp-link regc1 pthstp-init
        regioncorr-intersection             \ regc1 ret-lst pthstp-link, regc-int' t | f
        if
            \ Check result regions.
            dup                             \ regc1 ret-lst pthstp-link regc-int' regc-int'
            #2 pick                         \ regc1 ret-lst pthstp-link regc-int' regc-int' link
            link-get-data                   \ regc1 ret-lst pthstp-link regc-int' regc-int' pthstpx
            pathstep-get-result-regions     \ regc1 ret-lst pthstp-link regc-int' regc-int' rslts
            regioncorr-superset?            \ regc1 ret-lst pthstp-link regc-int' bool
            swap regioncorr-deallocate      \ regc1 ret-lst pthstp-link bool
            if
            else
                dup link-get-data           \ regc1 ret-lst pthstp-link pthstpx
                #2 pick                     \ regc1 ret-lst pthstp-link pthstpx ret-lst
                pathstep-list-push          \ regc1 ret-lst pthstp-link
            then
        then

        link-get-next
    repeat
                                            \ regc1 ret-lst
    \ Clean up.
    nip

    \ Return.
    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
;

\ Return pathsteps, from a given list, that have at least one change that is in
\ a given changescorr.
: pathstep-list-has-needed-change ( cngs1 pthstp-lst0 -- pthstp-lst t | f )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-changescorr

    \ Init return list.
    list-new                                \ cngs1 pthstp-lst0 | ret-lst

    \ Prep for loop.
    over list-get-links                     \ cngs1 pthstp-lst0 | ret-lst pthstp-link

    begin
        ?dup
    while
        dup link-get-data                   \ cngs1 pthstp-lst0 | ret-lst pthstp-link pthstpx
        pathstep-get-changes                \ cngs1 pthstp-lst0 | ret-lst pthstp-link pthstp-cngs
        #4 pick                             \ cngs1 pthstp-lst0 | ret-lst pthstp-link pthstp-cngs cngs1
        changescorr-intersection            \ cngs1 pthstp-lst0 | ret-lst pthstp-link cngs-int'
        dup changescorr-null?               \ cngs1 pthstp-lst0 | ret-lst pthstp-link cngs-int' bool
        swap changescorr-deallocate         \ cngs1 pthstp-lst0 | ret-lst pthstp-link bool
        if
        else
            dup link-get-data               \ cngs1 pthstp-lst0 | ret-lst pthstp-link pthstpx
            #2 pick                         \ cngs1 pthstp-lst0 | ret-lst pthstp-link pthstpx ret-lst
            pathstep-list-push              \ cngs1 pthstp-lst0 | ret-lst pthstp-link
        then

        link-get-next
    repeat

    \ Clean up.
    nip nip                                 \ ret-lst

    \ Return.
    dup list-is-empty?
    if
        list-deallocate
        false
    else
        true
    then
;

\ Return pathsteps, from a given list, that have at least one change that is needed
\ in moving from one regioncorr to another.
: pathstep-list-has-needed-change-from-to ( regc-to2 regc-from1 pthstp-lst0 -- pthstp-lst t | f)
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    \ Get needed changes.
    -rot                                    \ pthstp-lst0 regc-to2 regc-from1
    changescorr-new-regc-to-regc            \ pthstp-lst0 cngs'
    dup                                     \ pthstp-lst0 cngs' cngs'
    rot                                     \ cngs' cngs' pthstp-lst0
    pathstep-list-has-needed-change         \ cngs', pthstp-lst t | f
    if
        swap changescorr-deallocate
        true
    else
        changescorr-deallocate
        false
    then
;

\ Return a list of pathsteps that contain at least one needed change and
\ do not reguire needed changes to get from regc-from to the pathstep's initial regions.
: pathstep-list-reachable-fc ( regc-to2 regc-from1 pthstp-lst0 -- pthstp-lst t | f )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr
    \ cr ." pathstep-list-reachable-fc: start: " .stack-gbl cr

    \ Get needed changes.
    #2 pick #2 pick                         \ regc-to2 regc-from1 pthstp-lst0 | regc-to2 regc-from1
    changescorr-new-regc-to-regc            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned'
    \ cr ." pathstep-list-reachable-fc: changes needed: " dup .changescorr cr

    \ Init return list.
    list-new                                \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst

    \ Prep for loop.
    #2 pick                                 \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-lst0
    list-get-links                          \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link

    \ For each list pathstep.
    begin
        ?dup
    while
        \ Get changes needed from regc-from to pathstep initial regions.
        dup link-get-data                   \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx
        pathstep-get-initial-regions        \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx-initial
        #5 pick                             \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx-initial regc-from1
        changescorr-new-regc-to-regc        \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from1'

        \ Check if any needed changes are required.
        dup                                 \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from1' cngsc-from1'
        #4 pick                             \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from1' cngsc-from1' cngsc-ned'
        changescorr-intersection            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from1' cngsc-from-int'
        swap changescorr-deallocate         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from-int'
        dup changescorr-null?               \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link cngsc-from-int' bool
        swap changescorr-deallocate         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link bool
        if
            \ No needed changes are required to get to the pathstep initial regions, save it
            dup link-get-data               \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx
            #2 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link pthstpx ret-lst
            list-push-struct                \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst | pthstp-link
        then

        link-get-next
    repeat
                                            \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst
    dup list-is-empty?
    if
        list-deallocate
        changescorr-deallocate             \ regc-to2 regc-from1 pthstp-lst0
        2drop drop
        false
        \ cr ." pathstep-list-reachable-fc: exit 1, fail: " .stack-gbl cr
        exit
    then

    \ Set number unwanted changes.
    #4 pick #4 pick                         \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst regc-to2 regc-from1
    #2 pick                                 \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst regc-to2 regc-from1 ret-lst
    pathstep-list-set-number-unwanted-changes-fc

    \ Return.                               \ regc-to2 regc-from1 pthstp-lst0 | cngsc-ned' ret-lst
    swap changescorr-deallocate             \ regc-to2 regc-from1 pthstp-lst0 | ret-lst
    2nip                                    \ pthstp-lst0 | ret-lst
    nip                                     \ ret-lst
    true
    \ cr ." pathstep-list-reachable-fc: exit 2, Ok: " .stack-gbl cr
;

\ Return a list of pathsteps with result-regions closest to a given regioncorr.
: pathstep-list-closest-result-regions ( regc1 pthstp-lst0 -- pthstp-lst )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr

    \ Init return list.
    list-new                            \ regc1 pthstp-lst0 ret-lst

    \ Init min-distance counter
    #9999                               \ regc1 pthstp-lst0 ret-lst min

    \ Prep for loop, find min distance.
    #2 pick list-get-links              \ regc1 pthstp-lst0 ret-lst min pthstp-link

    begin
        ?dup
    while
        \ Prep for later distance comparison.
        swap                            \ regc1 pthstp-lst0 ret-lst pthstp-link min

        \ Get current pathstep result regions.
        over link-get-data              \ regc1 pthstp-lst0 ret-lst pthstp-link min pathstpx
        pathstep-get-result-regions     \ regc1 pthstp-lst0 ret-lst pthstp-link min pthstp-rslt

        \ Get distance to regc1.
        #5 pick                         \ regc1 pthstp-lst0 ret-lst pthstp-link min pthstp-rslt regc1
        regioncorr-distance             \ regc1 pthstp-lst0 ret-lst pthstp-link min dist

        \ Set minimum distance.
        min                            \ regc1 pthstp-lst0 ret-lst pthstp-link min

        \ Prep for next cycle.
        swap                            \ regc1 pthstp-lst0 ret-lst min pthstp-link

        link-get-next
    repeat
                                        \ regc1 pthstp-lst0 ret-lst min

    \ Prep for loop, gather pathsteps that are at the minimum distance.
    #2 pick list-get-links              \ regc1 pthstp-lst0 ret-lst min pthstp-link

    begin
        ?dup
    while
        \ Get current pathstep result regions.
        dup link-get-data               \ regc1 pthstp-lst0 ret-lst min pthstp-link pathstpx
        pathstep-get-result-regions     \ regc1 pthstp-lst0 ret-lst min pthstp-link pthstp-rslt

        \ Get distance to regc1.
        #5 pick                         \ regc1 pthstp-lst0 ret-lst min pthstp-link pthstp-rslt regc1
        regioncorr-distance             \ regc1 pthstp-lst0 ret-lst min pthstp-link dist

        \ Check if pathstep distance is equal to the minimum.
        #2 pick =                       \ regc1 pthstp-lst0 ret-lst min pthstp-link bool
        if
            dup link-get-data           \ regc1 pthstp-lst0 ret-lst min pthstp-link pathstpx
            #3 pick                     \ regc1 pthstp-lst0 ret-lst min pthstp-link pathstpx ret-lst
            list-push-struct            \ regc1 pthstp-lst0 ret-lst min pthstp-link
        then

        link-get-next
    repeat
                                        \ regc1 pthstp-lst0 ret-lst min
    drop                                \ regc1 pthstp-lst0 ret-lst
    nip nip
;

\ Return the an intersection of a regioncorr and a pathstep-list.
: pathstep-list-intersection ( regc1 pthstp-lst0 -- regc t | f )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr

    list-get-links                      \ regc1 pthstp-link

    begin
        ?dup
    while
        dup link-get-data               \ regc1 pthstp-link pthstpx
        pathstep-get-initial-regions    \ regc1 pthstp-link regc-i
        #2 pick                         \ regc1 pthstp-link regc-i regc1
        regioncorr-intersection         \ regc1 pthstp-link, regc-int t | f
        if
            \ cr ." pathstep-list-intersection: regc1: " #2 pick .regioncorr space ." to: " dup .regioncorr cr
            nip nip
            true
            exit
        then

        link-get-next
    repeat
                                        \ regc1
    drop
    false
;

\ Return true if two regioncorrs are both subset of at least one pathstep initial regions.
: pathstep-list-superset-both? ( regc2 regc1 pthstp-lst0 -- bool )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    \ Prep for loop.
    list-get-links                          \ regc2 regc1 pthstp-link

    begin
        ?dup
    while
        over                                \ regc2 regc1 pthstp-link regc1
        over link-get-data                  \ regc2 regc1 pthstp-link regc1 pthstpx
        \ cr ." comparing " #4 pick .regioncorr space ." and " #3 pick .regioncorr space ." to " dup .pathstep cr
        pathstep-get-initial-regions        \ regc2 regc1 pthstp-link regc1 pthstp-init
        regioncorr-superset?                \ regc2 regc1 pthstp-link bool
        if
            #2 pick                         \ regc2 regc1 pthstp-link regc2
            over link-get-data              \ regc2 regc1 pthstp-link regc2 pthstpx
            pathstep-get-initial-regions    \ regc2 regc1 pthstp-link regc2 pthstp-init
            regioncorr-superset?            \ regc2 regc1 pthstp-link bool
            if
                2drop                       \ regc2
                drop                        \
                true
                exit
            then
        then

        link-get-next
    repeat
                                            \ regc2 regc1
    2drop
    false
;

\ Return a pathstep that has initial regions that match a given regc.
: pathstep-list-find ( regc1 pthstp-lst -- pthstp t | f )
    \ Check args.
    assert-tos-is-pathstep-list
    assert-nos-is-regioncorr

     \ Prep for loop.
    list-get-links                          \ regc1 pthstp-link

    begin
        ?dup
    while
        dup link-get-data                   \ regc1 pthstp-link pthstpx
        pathstep-get-initial-regions        \ regc1 pthstp-link initial
        #2 pick                             \ regc1 pthstp-link initial regc1
        regioncorr-eq?                      \ regc1 pthstp-link bool
        if
            link-get-data                   \ regc1 pthstpx
            nip                             \ pthstpx
            true
            exit
        then

        link-get-next
    repeat
                                            \ regc1
    drop
    false
;
