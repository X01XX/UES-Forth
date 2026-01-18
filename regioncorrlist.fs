\ Functions for a list of RegionCorrs.

\ Check if tos is a list, if non-empty, with the first item being a regioncorr.
: assert-tos-is-regioncorr-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-regioncorr
        drop
    then
;

\ Check if nos is a list, if non-empty, with the first item being a regioncorr.
: assert-nos-is-regioncorr-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-regioncorr
        drop
    then
;

\ Deallocate an regc list.
: regioncorr-list-deallocate ( regc-lst0 -- )
    \ cr ." regioncorr-list-deallocate: " .stack-structs-xt execute cr
    \ Check arg.
    assert-tos-is-regioncorr-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ regc-lst0 uc
    #2 < if
        \ Deallocate regc instances in the list.
        [ ' regioncorr-deallocate ] literal over    \ regc-lst0 xt regc-lst0
        list-apply                                  \ regc-lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

' regioncorr-list-deallocate to regioncorr-list-deallocate-xt

: regioncorr-lol-deallocate ( lst0 -- )
    \ cr ." regioncorr-lol-deallocate: " .stack-structs-xt execute cr
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate regc instances in the list.
        [ ' regioncorr-list-deallocate ] literal over      \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

: .regioncorr-list ( regc-lst -- )
    \ Check arg.
    assert-tos-is-regioncorr-list

    s" (" type
    [ ' .regioncorr ] literal swap    \ xt regioncorr-list
    list-apply                        \
    s" )" type
;

\ Return true if a regioncorr-list (list of lists) contains
\ a superset of a given regc.
: regioncorr-list-any-superset ( regc1 regc-lst0 -- bool )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    list-get-links                  \ regc1 link0

    begin
        ?dup
    while
        over                        \ regc1 link0 regc1
        over link-get-data          \ regc1 link0 regc1 regcx
        regioncorr-superset         \ regc1 link0 bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                    \ regc1
    drop                            \
    false                           \ bool
;

\ For a regioncorr-list, remove subsets of a given regc.
: regioncorr-list-remove-subsets ( regc1 regioncorr-list0 -- )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    begin
        [ ' regioncorr-subset ] literal \ regc1 regioncorr-list0 xt
        #2 pick #2 pick                 \ regc1 regioncorr-list0 xt regc1 regioncorr-list0
        list-remove                     \ regc1 regioncorr-list0, regcx t | f
        if
            regioncorr-deallocate       \ regc1 regioncorr-list0
        else
            2drop
            exit
        then
    again
;

\ Push an regc into a regioncorr-list.
: regioncorr-list-push  ( regc1 regioncorr-list0 -- )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    list-push-struct                     \
;

\ Add a regc to an regc list, removing subsets.
\ A duplicate in the list will be like a superset, the code will not push.
\ Return true if the push succeeds.
\ You may want to deallocate the regc if the push fails.
: regioncorr-list-push-nosubs ( regc1 regioncorr-list0 -- bool )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    \ Skip if any supersets/eq are in the list.
    2dup regioncorr-list-any-superset      \ regc1 regioncorr-list0 bool
    if
        2drop
        false
        exit
    then

    2dup regioncorr-list-remove-subsets

    regioncorr-list-push
    true
;

\ Return an regioncorr-list with no duplicates or subsets.
: regioncorr-list-copy-nosubs ( regc-list0 -- regc-list )
    \ Check arg.
    assert-tos-is-regioncorr-list

    list-new swap                   \ ret-lst regc-list0

    list-get-links                  \ ret-lst link

    begin
        ?dup
    while
        dup link-get-data           \ ret-lst link regcx
        #2 pick                     \ ret-lst link regcx ret-lst
        regioncorr-list-push-nosubs \ ret-lst link bool
        drop                        \ ret-lst link

        link-get-next               \ ret-lst link
    repeat
                                    \ ret-lst
;

\ Return true if a regioncorr-list contains at least one regc
\ that intersects with a given regc.
: regioncorr-list-any-intersection-regc ( regc1 regc-lst0 -- bool )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    list-get-links                  \ regc1 link0
    begin
        ?dup
    while
        over                        \ regc1 link0 regc1
        over link-get-data          \ regc1 link0 regc1 regc0
        regioncorr-intersects       \ regc1 link0 bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                    \ regc1
    drop                            \
    false
;

: regioncorr-list-subtract-regioncorr ( regc1 regc-lst0 -- regc-lst )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    \ cr ." subtract " over .regioncorr space ." from " dup .regioncorr-list
    \ Init return list.
    list-new -rot                           \ ret-lst regc1 regc-lst0

    \ Get first regc-lst0 link.
    list-get-links                          \ ret-lst regc1 link0

    \ For each regc in regc-lst0 ...
    begin
        ?dup
    while
        \ Check if regc1 is a superset of regc0.
        dup link-get-data               \ ret-lst regc1 link0 regc0
        #2 pick                         \ ret-lst regc1 link0 regc0 regc1
        regioncorr-superset             \ ret-lst regc1 link0 bool
        if
            \ Don't add regc0 to return list.
        else
            \ Check if regc1 intersects regc0.
            dup link-get-data           \ ret-lst regc1 link0 regc0
            #2 pick                     \ ret-lst regc1 link0 regc0 regc1
            regioncorr-intersects       \ ret-lst regc1 link0 bool
            if
                \ Subtract regc1 from regc0.
                over                    \ ret-lst regc1 link0 regc1
                over link-get-data      \ ret-lst regc1 link0 regc1 regc0
                regioncorr-subtract     \ ret-lst regc1 link0, left-lst t | f
                if
                    \ Add whats left to the return list.
                    dup                 \ ret-lst regc1 link0 left-lst left-lst
                    list-get-links      \ ret-lst regc1 link0 left-lst left-link
                    begin
                        ?dup
                    while
                        dup link-get-data               \ ret-lst regc1 link0 left-lst left-link left-regc
                        #5 pick                         \ ret-lst regc1 link0 left-lst left-link left-regc ret-lst
                        regioncorr-list-push-nosubs     \ ret-lst regc1 link0 left-lst left-link bool
                        drop
                        link-get-next           \ ret-lst regc1 link0 left-lst left-link
                    repeat
                                                \ ret-lst regc1 link0 left-lst
                    regioncorr-list-deallocate  \ ret-lst regc1 link0
                then
            else
                \ Add regc0 to return list.
                dup link-get-data               \ ret-lst regc1 link0 regc0
                #3 pick                         \ ret-lst regc1 link0 regc0 ret-lst
                regioncorr-list-push-nosubs     \ ret-lst regc1 link0 bool
                drop
            then
        then

        link-get-next
    repeat
                            \ ret-lst regc1
    drop                    \ ret-lst
    \ space ." giving " dup .regioncorr-list cr
;

\ Return TOS regc-lst minus NOS regc-lst.
: regioncorr-list-subtract ( regc-lst1 regc-lst0 -- regc-lst )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr-list
    \ cr ." regioncorr-list " dup .regioncorr-list space ." - " over .regioncorr-list

    \ Prep for loop.
    regioncorr-list-copy-nosubs \ regc-lst1 regc-lst0'
    swap                        \ regc-lst0' regc-lst1
    list-get-links              \ regc-lst0' link1

    \ Subtract each regc in regc-lst1 from regc-lst0'.
    begin
        ?dup
    while
        \ Prep for process.
        swap                    \ link1 regc-lst0'

        \ Subtract an regc.
        over link-get-data                  \ link1 regc-lst0' regc1
        over                                \ link1 regc-lst0' regc1 regc-lst0'
        regioncorr-list-subtract-regioncorr \ link1 regc-lst0' regc-lst0''

        \ Clean up previous regc-lst0
        swap                        \ link1 regc-lst0'' regc-lst0'
        regioncorr-list-deallocate  \ link1 regc-lst0''

        \ Prep for next cycle.
        swap                        \ regc-lst0'' link1

        link-get-next
    repeat
                            \ regc-lst0'
    \ space ." = " dup .regioncorr-list
;

\ Return the complement of a regc.
: regioncorr-list-complement ( regc-lst0 -- regc-lst )
    \ Check arg.
    assert-tos-is-regioncorr-list

    \ Max list of maximum regions.
    list-new                            \ regc-lst0 max-regc-lst
    current-session                     \ regc-lst0 max-regc-lst sess
    session-calc-max-regions-xt execute \ regc-lst0 max-regc-lst regc-max
    over                                \ regc-lst0 max-regc-lst regc-max max-regc-lst
    regioncorr-list-push                \ regc-lst0 max-regc-lst

    \ Save max regc
    tuck                                \ max-regc-lst regc-lst0 max-regc-lst

    \ Subtract regc from max regions.
    regioncorr-list-subtract            \ max-regc-lst ret-regc-lst

    \ Clean up.
    swap regioncorr-list-deallocate     \ ret-regc-lst
;

\ Return true if two regioncorr-lists are equal.
: regioncorr-list-eq ( regc-lst1 regc-lst0 -- bool )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr-list

    \ Check length first.
    over list-get-length                    \ regc-lst1 regc-lst0 len1
    over list-get-length                    \ regc-lst1 regc-lst0 len1 len0
    <> if
        2drop
        false
        exit
    then

    \ Check each regc.
    list-get-links                          \ regc-lst1 link0

    begin
        ?dup
    while
        [ ' regioncorr-eq ] literal   \ regc-lst1 link0 xt
        over link-get-data                  \ regc-lst1 link0 xt regc0
        #3 pick                             \ regc-lst1 link0 xt regc0 regc-lst1
        list-member                         \ regc-lst1 link0 bool
        is-false if
            2drop
            false
            exit
        then

        link-get-next
    repeat
                                \ regc-lst1
    drop                        \
    true
;

\ Return a normalized regioncorr-list.
: regioncorr-list-normalize ( regc-lst0 -- regc-lst )
    \ Check arg.
    assert-tos-is-regioncorr-list

    regioncorr-list-complement      \ regc-lst0'
    dup                             \ regc-lst0' regc-lst0'
    regioncorr-list-complement      \ regc-lst0' regc-lst0''

    \ Clean up.
    swap                            \ regc-lst0'' regc-lst0'
    regioncorr-list-deallocate      \ regc-lst0''
;

\ Push a regc into a regioncorr-list, if there are no duplicates in the list.
\ Return true if the regc is added to the list.
: regioncorr-list-push-nodups ( regc1 regc-lst0 -- flag )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    \ Return if any regc in the list is a duplicate of regc1.
    2dup                                    \ regc1 regc-lst0 regc1 regc-lst0
    [ ' regioncorr-eq ] literal             \ regc1 regc-lst0 regc1 regc-lst0 xt
    -rot                                    \ regc1 regc-lst0 xt regc1 regc-lst0
    list-member                             \ regc1 regc-lst0 flag
    if
        2drop
        false
        exit
    then
                                            \ regc1 regc-lst0

    \ regc1 list0
    regioncorr-list-push
    true
;

\ Return a copy of an regioncorr-list, with duplicates removed.
: regioncorr-list-copy-nodups ( regc-lst -- regc-lst )
    \ Check arg.
    assert-tos-is-regioncorr-list

    list-new swap               \ ret lst0
    list-get-links              \ ret link

    begin
        ?dup
    while
        dup link-get-data       \ ret link regx
        #2 pick                 \ ret link regx ret
        regioncorr-list-push-nodups    \ ret link flag
        drop

        link-get-next           \ ret link
    repeat
;

\ Return all two-regc intersections from an regioncorr-list.
\ Duplicates will be suppresed, but propr subsets are Ok.
: regioncorr-list-intersections-nodups ( regc-lst -- regc-lst )
    \ Check arg.
    assert-tos-is-regioncorr-list

    \ Init return list.
    list-new swap                           \ ret-lst reg-lst0
    list-get-links                          \ ret-lst link0

    \ For each region.
    begin
        ?dup
    while
        \ Get link to following regions.
        \ Having direct access to the list links makes this logic effortless,
        \ compared to using indices at a higher level.
        dup link-get-next                   \ ret-lst link0 link+

        \ For each following region.
        begin
            ?dup
        while
            over link-get-data              \ ret-lst link0 link+ regc0
            over link-get-data              \ ret-lst link0 link+ regc0 regc+
            regioncorr-intersection         \ ret-lst link0 link+, regc-int t | f
            if                              \ ret-lst link0 link+ regc-int
                dup                         \ ret-lst link0 link+ regc-int regc-int
                #4 pick                     \ ret-lst link0 link+ regc-int regc-int ret-lst
                regioncorr-list-push-nodups \ ret-lst link0 link+ regc-int bool
                if
                    drop                    \ ret-lst link0 link+
                else
                    regioncorr-deallocate  \ ret-lst link0 link+
                then
            then

            link-get-next                   \ ret-lst link0 link+
        repeat

        link-get-next                       \ ret-lst link0
    repeat
                                            \ ret-lst
;

\ Append NOS regc-lst to TOS-regioncorr-list.
: regioncorr-list-append ( regc-lst1 regc-lst0 -- )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr-list

    swap                    \ regc-lst0 regc-lst1
    list-get-links          \ regc-lst0 link
    begin
        ?dup
    while
        dup link-get-data       \ regc-lst0 link regcx
        #2 pick                 \ regc-lst0 link regcx regc-lst0
        regioncorr-list-push    \ regc-lst0 link

        link-get-next           \ regc-lst0 link
    repeat
                                \ regc-lst0
    drop
;

\ Append NOS regc-lst to TOS-regioncorr-list, no duplicates.
: regioncorr-list-append-nodups ( regc-lst1 regc-lst0 -- )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr-list

    swap                            \ regc-lst0 regc-lst1
    list-get-links                  \ regc-lst0 link
    begin
        ?dup
    while
        dup link-get-data           \ regc-lst0 link regcx
        #2 pick                     \ regc-lst0 link regcx regc-lst0
        regioncorr-list-push-nodups \ regc-lst0 link flag
        drop                        \ regc-lst0 link

        link-get-next               \ regc-lst0 link
    repeat
                                    \ regc-lst0
    drop
;

\ Return fragments of a given regioncorr-list.
\ The fragments will account for all parts of the given regioncorr-list.
\ All fragments will be within all regions of the given regioncorr-list that they intersect.
\ Intermediate regions may be proper subsets, but duplicates will be avoided.
: regioncorr-list-intersection-fragments ( lst0 -- frag-lst )
    \ Check arg.
    assert-tos-is-regioncorr-list

    \ Insure-no-duplicates.
    list-new swap                           \ ret-lst lst0
    regioncorr-list-copy-nodups                    \ ret-lst lst0'

    begin
        dup list-is-empty 0=
    while
        dup                                 \ ret-lst lst0' lst0'

        \ Get intersections.
        regioncorr-list-intersections-nodups    \ ret-lst lst0' int-lst

        \ Get whats left over.
        2dup swap                           \ ret-lst lst0' int-lst int-lst lst0'
        regioncorr-list-subtract                   \ ret-lst lst0' int-lst left-over-lst
        \ cr ." list: " #2 pick .regioncorr-list
        \ space ." - " over .regioncorr-list
        \ space ." = " dup .regioncorr-list
        \ cr

        \ Add left over to result list.
        dup                                 \ ret-lst lst0' int-lst left-over-lst left-over-lst
        #4 pick                             \ ret-lst lst0' int-lst left-over-lst left-over-lst ret-lst
        regioncorr-list-append-nodups              \ ret-lst lst0' int-lst left-over-lst

        \ Clean up, intersections become the next cycle lst0'.
        regioncorr-list-deallocate                 \ ret-lst lst0' int-lst
        swap                                \ ret-lst int-lst lst0'
        regioncorr-list-deallocate                 \ ret-lst int-lst
    repeat
                                            \ ret-lst lst (empty)
    list-deallocate                         \ ret-lst
;

\ Return the least different value to two given regcs, not counting equal regcs.
: regioncorr-list-least-difference ( regc2 regc1 regc-lst0 -- u )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    \ Init return count.
    #2 pick #2 pick             \ regc2 regc1 regc-lst0 regc2 regc1
    regioncorr-distance         \ regc2 regc1 regc-lst0 u
    dup 0= abort" two rcls intersect?"
    cr ." regc1 " #2 pick .regioncorr
    space ." and " #3 pick .regioncorr
    space ." is " dup . cr

    swap                        \ regc2 regc1 dist regc-lst0

    \ Prep for loop.
    list-get-links              \ regc2 regc1 dist link

    begin
        2dup
    while
        \ Check regcx ne regc1.
        dup link-get-data       \ regc2 regc1 dist link regcx
        #3 pick                 \ regc2 regc1 dist link regcx regc1
        <> if
            \ Check regcx ne regc2
            dup link-get-data       \ regc2 regc1 dist link regcx
            #4 pick                 \ regc2 regc1 dist link regcx regc2
            <> if
                \ Calc distance to regc2.
                dup link-get-data     \ regc2 regc1 dist link regcx
                #4 pick               \ regc2 regc1 dist link regcx regc2
                regioncorr-distance   \ regc2 regc1 dist link dist2

                \ Calc distance to regc1.
                over link-get-data          \ regc2 regc1 dist link dist2 regcx
                cr ." for " dup .regioncorr
                #4 pick                     \ regc2 regc1 dist link dist2 regcx regc1
                regioncorr-distance         \ regc2 regc1 dist link dist2 dist1

                \ Add distance 1 and 2.
                +                           \ regc2 regc1 dist link dist12

                \ Compare with current least dist.
                dup                         \ regc2 regc1 dist link dist12 dist12
                #3 pick                     \ regc2 regc1 dist link dist12 dist12 dist
                space ." compare cur dist " dup . space ." with new " over . cr
                <                           \ regc2 regc1 dist link dist12 bool
                if                          \ regc2 regc1 dist link dist12
                    \ Update current least distance.
                    rot drop                \ regc2 regc1 link dist12
                    swap                    \ regc2 regc1 dist12 link
                else
                    drop                    \ regc2 regc1 dist link
                then
            then
        then

        link-get-next
    repeat
                                \ regc2 regc1 dist
    nip nip                     \ dist
;

\ Return true if a regc intersects at least one regc in an regioncorr-list.
: regioncorr-list-intersects ( regc1 lst0 -- bool )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    list-get-links              \ regc1 link

    begin
        ?dup
    while
        dup link-get-data       \ regc1 link regcx
        #2 pick                 \ regc1 link regcx regc1
        regioncorr-intersects   \ regc1 link bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                \ regc1
    drop
    false
;

\ Return regc that intersects both regc-to and regc-from.
: regioncorr-list-intersects-both ( regc-to regc-from lst0 -- regc t | f )
    \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr
    assert-3os-is-regioncorr

    list-get-links                      \ regc-to regc-from link

    begin
        ?dup
    while
        dup link-get-data               \ regc-to regc-from link regcx
        #3 pick over                    \ regc-to regc-from link regcx regc-to regcx
        regioncorr-intersects           \ regc-to regc-from link regcx bool
        if
            #2 pick over                \ regc-to regc-from link regcx regc-from regcx
            regioncorr-intersects       \ regc-to regc-from link regcx bool
            if                          \ regc-to regc-from link regcx
                2nip nip                \ regcx
                true
                exit
            then
        then
                                        \ regc-to regc-from link regcx
        drop                            \ regc-to regc-from link

        link-get-next
    repeat
                                        \ regc-to regc-from
    2drop
    false
;

\ Return a copy of a regioncorr-list, except for a given regioncorr of a given index.
\ Used by regioncorr subtraction.
: regioncorr-list-copy-except ( regc2 inx1 lst0 -- lst )
    \ Check args.
    assert-tos-is-regioncorr-list
    over 0 < abort" index out of range"
    over over list-get-length < is-false abort" index out of range"
    assert-3os-is-regioncorr

    \ Init return list.
    list-new swap                   \ regc2 inx1 ret-lst lst0

    \ Init index counter
    0 swap                          \ regc2 inx1 ret-lst ctr lst0

    \ Prep for loop.
    list-get-links                  \ regc2 inx1 ret-lst ctr link

    begin
        ?dup
    while
        over                        \ regc2 inx1 ret-lst ctr link ctr
        #4 pick                     \ regc2 inx1 ret-lst ctr link ctr inx1
        = if                        \ regc2 inx1 ret-lst ctr link
            #4 pick                 \ regc2 inx1 ret-lst ctr link regc2
            #3 pick                 \ regc2 inx1 ret-lst ctr link regc2 ret-lst
            region-list-push-end    \ regc2 inx1 lst-ret ctr link
        else                        \ regc2 inx1 ret-lst ctr link
            dup link-get-data       \ regc2 inx1 ret-lst ctr link region
            #3 pick                 \ regc2 inx1 ret-lst ctr link region ret-lst
            region-list-push-end    \ regc2 inx1 ret-lst ctr link
        then

        \ Inc counter.
        swap 1+ swap

        link-get-next               \ regc2 inx1 ret-lst ctr link
    repeat
                                    \ regc2 inx1 ret-lst ctr
    drop                            \ regc2 inx1 ret-lst
    nip nip                         \ ret-lst
;

' regioncorr-list-copy-except to regioncorr-list-copy-except-xt

: .regioncorr-lol ( regc-lol -- ) \ Print a regioncorr list-of-lists.
    \ Check arg.
    assert-tos-is-list

    list-get-links                     \ lol-link
    
    begin
        ?dup
    while
        cr
        dup link-get-data               \ lol-link regclst
        [ ' .regioncorr-list ] literal execute
        cr

        link-get-next
    repeat
;

: regioncorr-list-closest-regioncorrs ( regc1 regc-lsn0 -- regc-lst ) \ Return a list of regc in list closest to a given regc.
        \ Check args.
    assert-tos-is-regioncorr-list
    assert-nos-is-regioncorr

    \ Get min difference of all items.
    #9999999                            \ | min
    over list-get-links                 \ | min link
    begin
        ?dup
    while
        #3 pick                         \ | min link regc1
        over link-get-data              \ | min link regc1 regcx
        regioncorr-distance             \ | min link u
        rot min swap                    \ | min link

        link-get-next
    repeat
                                        \ | min
    \ cr ." min dist: " dup . cr

    \ Init return list.
    list-new swap                       \ | ret-lst min

    #2 pick list-get-links              \ | ret-lst min link
    begin
        ?dup
    while
        \ Check if current item has the minimum distance.
        #4 pick                         \ | ret-lst min link regc1
        over link-get-data              \ | ret-lst min link regc1 regcx
        regioncorr-distance             \ | ret-lst min link u
        #2 pick                         \ | ret-lst min link u min
        =                               \ | ret-lst min link bool
        if
            \ Save item in return list.
            dup link-get-data           \ | ret-lst min link regcx
            #3 pick                     \ | ret-lst min link regcx ret-lst
            list-push-struct            \ | ret-lst min link
        then

        link-get-next
    repeat

                                        \ | ret-lst min
    2nip drop                           \ ret-lst
;
