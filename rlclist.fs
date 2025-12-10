\ Functions for Region List Corresponding (to domains) (RLC) lists.

\ Deallocate an rlc list.
: rlc-list-deallocate ( lst0 -- )
    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate rlc instances in the list.
        [ ' region-list-deallocate ] literal over           \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

: .rlc-list ( rlc-lst -- )
    \ Check arg.
    assert-tos-is-list

    s" (" type
    [ ' .region-list-corr ] literal swap    \ xt rlc-list
    list-apply                              \
    s" )" type
;

\ Return true if a region-list-corr-list (list of lists) contains
\ a superset of a gives rlc.
: rlc-list-any-superset ( rlc1 rlc-lst0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    list-get-links                  \ rlc1 link0

    begin
        ?dup
    while
        over                        \ rlc1 link0 rlc1
        over link-get-data          \ rlc1 link0 rlc1 rlcx
        region-list-corr-superset   \ rlc1 link0 bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                    \ rlc1
    drop                            \
    false                           \ bool
;

\ For a rlc-list, remove subsets of a given rlc.
: rlc-list-remove-subsets ( rlc1 rlc-list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    begin
        [ ' region-list-corr-subset ] literal   \ rlc1 rlc-list0 xt
        #2 pick #2 pick                         \ rlc1 rlc-list0 xt rlc1 rlc-list0
        list-remove                             \ rlc1 rlc-list0, rlcx t | f
        if
            region-list-deallocate              \ rlc1 rlc-list0
        else
            2drop
            exit
        then
    again
;

\ Push an rlc into a rlc-list.
: rlc-list-push  ( rlc1 rlc-list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Inc list use count.
    over struct-inc-use-count

    list-push                           \
;

\ Add a rlc to an rlc list, removing subsets.
\ A duplicate in the list will be like a superset, the code will not push.
\ Return true if the push succeeds.
\ You may want to deallocate the rlc if the push fails.
: rlc-list-push-nosubs ( rlc1 rlc-list0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Skip if any supersets/eq are in the list.
    2dup rlc-list-any-superset      \ rlc1 rlc-list0 bool
    if
        2drop
        false
        exit
    then

    2dup rlc-list-remove-subsets

    rlc-list-push
    true
;

\ Return an rlc-list with no duplicates or subsets.
: rlc-list-copy-nosubs ( list0 -- rlc-list )
    \ Check arg.
    assert-tos-is-list

    list-new swap               \ ret-lst rlc-list-0

    list-get-links              \ ret-lst link

    begin
        ?dup
    while
        dup link-get-data       \ ret-lst link rlcx
        #2 pick                 \ ret-lst link rlcx ret-lst
        rlc-list-push-nosubs    \ ret-lst link bool
        drop                    \ ret-lst link

        link-get-next           \ ret-lst link
    repeat
                                \ ret-lst
;

\ Return true if a rlclist contains at least one rlc
\ that intersects with a given rlc. 
: rlc-list-any-intersection-rlc ( rlc1 rlc-lst0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    list-get-links                  \ rlc1 link0
    begin
        ?dup
    while
        over                        \ rlc1 link0 rlc1
        over link-get-data          \ rlc1 link0 rlc1 rlc0
        region-list-corr-intersects \ rlc1 link0 bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                    \ rlc1
    drop                            \
    false
;

: rlc-list-subtract-rlc ( rlc1 rlc-lst0 -- rlc-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ cr ." subtract " over .region-list-corr space ." from " dup .rlc-list
    \ Init return list.
    list-new -rot                           \ ret-lst rlc1 rlc-lst0

    \ Get first rlc-lst0 link.
    list-get-links                          \ ret-lst rlc1 link0

    \ For each rlc in rlc-lst0 ...
    begin
        ?dup
    while
        \ Check if rlc1 is a superset of rlc0.
        dup link-get-data                       \ ret-lst rlc1 link0 rlc0
        #2 pick                                 \ ret-lst rlc1 link0 rlc0 rlc1
        region-list-corr-superset               \ ret-lst rlc1 link0 bool
        if
            \ Don't add rlc0 to return list.
        else
            \ Check if rlc1 intersects rlc0.
            dup link-get-data                   \ ret-lst rlc1 link0 rlc0
            #2 pick                             \ ret-lst rlc1 link0 rlc0 rlc1
            region-list-corr-intersects         \ ret-lst rlc1 link0 bool
            if
                \ Subtract rlc1 from rlc0.
                over                            \ ret-lst rlc1 link0 rlc1
                over link-get-data              \ ret-lst rlc1 link0 rlc1 rlc0
                region-list-corr-subtract       \ ret-lst rlc1 link0, left-lst t | f 
                if
                    \ Add whats left to the return list.
                    dup                         \ ret-lst rlc1 link0 left-lst left-lst
                    list-get-links              \ ret-lst rlc1 link0 left-lst left-link
                    begin
                        ?dup
                    while
                        dup link-get-data       \ ret-lst rlc1 link0 left-lst left-link left-rlc
                        #5 pick                 \ ret-lst rlc1 link0 left-lst left-link left-rlc ret-lst
                        rlc-list-push-nosubs    \ ret-lst rlc1 link0 left-lst left-link bool
                        drop
                        link-get-next           \ ret-lst rlc1 link0 left-lst left-link
                    repeat
                                                \ ret-lst rlc1 link0 left-lst
                    rlc-list-deallocate         \ ret-lst rlc1 link0
                then
            else
                \ Add rlc0 to return list.
                dup link-get-data               \ ret-lst rlc1 link0 rlc0
                #3 pick                         \ ret-lst rlc1 link0 rlc0 ret-lst
                rlc-list-push-nosubs            \ ret-lst rlc1 link0 bool
                drop
            then
        then

        link-get-next
    repeat
                            \ ret-lst rlc1
    drop                    \ ret-lst
    \ space ." giving " dup .rlc-list cr
;

\ Return TOS rlc-lst minus NOS rlc-lst.
: rlc-list-subtract ( rlc-lst1 rlc-lst0 -- rlc-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    \ cr ." rlc-list " dup .rlc-list space ." - " over .rlc-list

    \ Prep for loop.
    rlc-list-copy-nosubs        \ rlc-lst1 rlc-lst0'
    swap                        \ rlc-lst0' rlc-lst1
    list-get-links              \ rlc-lst0' link1

    \ Subtract each rlc in rlc-lst1 from rlc-lst0'.
    begin
        ?dup
    while
        \ Prep for process.
        swap                    \ link1 rlc-lst0'

        \ Subtract an rlc.
        over link-get-data      \ link1 rlc-lst0' rlc1
        over                    \ link1 rlc-lst0' rlc1 rlc-lst0'
        rlc-list-subtract-rlc   \ link1 rlc-lst0' rlc-lst0''

        \ Clean up previous rlc-lst0
        swap                    \ link1 rlc-lst0'' rlc-lst0'
        rlc-list-deallocate     \ link1 rlc-lst0''

        \ Prep for next cycle.
        swap                    \ rlc-lst0'' link1

        link-get-next
    repeat
                            \ rlc-lst0'
    \ space ." = " dup .rlc-list
;

\ Return the comlement of a rlc.
: rlc-list-complement ( rlc-lst0 -- rlc-lst )
    \ Check arg.
    assert-tos-is-list

    \ Max list of maximum regions.
    list-new                            \ rlc-lst0 max-rlc-lst
    current-session                     \ rlc-lst0 max-rlc-lst sess
    session-max-regions-xt execute      \ rlc-lst0 max-rlc-lst rlc-max
    over                                \ rlc-lst0 max-rlc-lst rlc-max max-rlc-lst
    rlc-list-push                       \ rlc-lst0 max-rlc-lst

    \ Save max rlc
    tuck                                \ max-rlc-lst rlc-lst0 max-rlc-lst

    \ Subtract rlc from max regions.
    rlc-list-subtract                   \ max-rlc-lst ret-rlc-lst

    \ Clean up.
    swap rlc-list-deallocate            \ ret-rlc-lst
;

\ Return true if two rlc-lists are equal.
: rlc-list-eq ( rlc-lst1 rlc-lst0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Check length first.
    over list-get-length                    \ rlc-lst1 rlc-lst0 len1
    over list-get-length                    \ rlc-lst1 rlc-lst0 len1 len0
    <> if
        2drop
        false
        exit
    then

    \ Check each rlc.
    list-get-links                          \ rlc-lst1 link0

    begin
        ?dup
    while
        [ ' region-list-corr-eq ] literal   \ rlc-lst1 link0 xt
        over link-get-data                  \ rlc-lst1 link0 xt rlc0
        #3 pick                             \ rlc-lst1 link0 xt rlc0 rlc-lst1
        list-member                         \ rlc-lst1 link0 bool
        is-false if
            2drop
            false
            exit
        then

        link-get-next
    repeat
                                \ rlc-lst1
    drop                        \
    true
;

\ Return a normalized rlc-list.
: rlc-list-normalize ( rlc-lst0 -- rlc-lst )
    \ Check arg.
    assert-tos-is-list

    rlc-list-complement     \ rlc-lst0'
    dup                     \ rlc-lst0' rlc-lst0'
    rlc-list-complement     \ rlc-lst0' rlc-lst0''

    \ Clean up.
    swap                    \ rlc-lst0'' rlc-lst0'
    rlc-list-deallocate     \ rlc-lst0''
;

\ Push a rlc into a rlc-list, if there are no duplicates in the list.
\ Return true if the rlc is added to the list.
: rlc-list-push-nodups ( rlc1 rlc-lst0 -- flag )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Return if any rlc in the list is a duplicate of rlc1.
    2dup                                    \ rlc1 rlc-lst0 rlc1 rlc-lst0
    [ ' region-list-corr-eq ] literal       \ rlc1 rlc-lst0 rlc1 rlc-lst0 xt
    -rot                                    \ rlc1 rlc-lst0 xt rlc1 rlc-lst0
    list-member                             \ rlc1 rlc-lst0 flag
    if
        2drop
        false
        exit
    then
                                            \ rlc1 rlc-lst0

    \ rlc1 list0
    rlc-list-push
    true
;

\ Return a copy of an rlc-list, with duplicates removed.
: rlc-list-copy-nodups ( rlc-lst -- rlc-lst )
    \ Check arg.
    assert-tos-is-list

    list-new swap               \ ret lst0
    list-get-links              \ ret link

    begin
        ?dup
    while
        dup link-get-data       \ ret link regx
        #2 pick                 \ ret link regx ret
        rlc-list-push-nodups    \ ret link flag
        drop

        link-get-next           \ ret link
    repeat
;

\ Return all two-rlc intersections from an rlc-list.
\ Duplicates will be suppresed, but propr subsets are Ok. 
: rlc-list-intersections ( rlc-lst -- rlc-lst )
    \ Check arg.
    assert-tos-is-list

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
            over link-get-data              \ ret-lst link0 link+ rlc0
            over link-get-data              \ ret-lst link0 link+ rlc0 rlc+
            region-list-corr-intersection   \ ret-lst link0 link+, rlc-int t | f
            if                              \ ret-lst link0 link+ rlc-int
                dup                         \ ret-lst link0 link+ rlc-int rlc-int
                #4 pick                     \ ret-lst link0 link+ rlc-int rlc-int ret-lst
                rlc-list-push-nodups        \ ret-lst link0 link+ rlc-int bool
                if
                    drop                    \ ret-lst link0 link+
                else
                    region-deallocate       \ ret-lst link0 link+
                then
            then

            link-get-next                   \ ret-lst link0 link+
        repeat

        link-get-next                       \ ret-lst link0
    repeat
                                            \ ret-lst
;

\ Append NOS rlc-lst to TOS-rlc-list.
: rlc-list-append ( rlc-lst1 rlc-lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    swap                    \ rlc-lst0 rlc-lst1
    list-get-links          \ rlc-lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ rlc-lst0 link rlcx
        #2 pick             \ rlc-lst0 link rlcx rlc-lst0
        rlc-list-push       \ rlc-lst0 link

        link-get-next       \ rlc-lst0 link
    repeat
                            \ rlc-lst0
    drop
;

\ Append NOS rlc-lst to TOS-rlc-list, no duplicates.
: rlc-list-append-nodups ( rlc-lst1 rlc-lst0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    swap                        \ rlc-lst0 rlc-lst1
    list-get-links              \ rlc-lst0 link
    begin
        ?dup
    while
        dup link-get-data       \ rlc-lst0 link rlcx
        #2 pick                 \ rlc-lst0 link rlcx rlc-lst0
        rlc-list-push-nodups    \ rlc-lst0 link flag
        drop                    \ rlc-lst0 link

        link-get-next           \ rlc-lst0 link
    repeat
                                \ rlc-lst0
    drop
;

\ Return fragments of a given rlc-list.
\ The fragments will account for all parts of the given rlc-list.
\ All fragments will be within all regions of the given rlc-list that they intersect.
\ Intermediate regions may be proper subsets, but duplicates will be avoided.
: rlc-list-intersection-fragments ( lst0 -- frag-lst )
    \ Check arg.
    assert-tos-is-list

    \ Insure-no-duplicates.
    list-new swap                           \ ret-lst lst0
    rlc-list-copy-nodups                    \ ret-lst lst0'

    begin
        dup list-is-empty 0=
    while
        dup                                 \ ret-lst lst0' lst0'

        \ Get intersections.
        rlc-list-intersections              \ ret-lst lst0' int-lst

        \ Get whats left over.
        2dup swap                           \ ret-lst lst0' int-lst int-lst lst0'
        rlc-list-subtract                   \ ret-lst lst0' int-lst left-over-lst
        \ cr ." list: " #2 pick .rlc-list
        \ space ." - " over .rlc-list
        \ space ." = " dup .rlc-list
        \ cr

        \ Add left over to result list.
        dup                                 \ ret-lst lst0' int-lst left-over-lst left-over-lst 
        #4 pick                             \ ret-lst lst0' int-lst left-over-lst left-over-lst ret-lst
        rlc-list-append-nodups              \ ret-lst lst0' int-lst left-over-lst

        \ Clean up, intersections become the next cycle lst0'.
        rlc-list-deallocate                 \ ret-lst lst0' int-lst
        swap                                \ ret-lst int-lst lst0'
        rlc-list-deallocate                 \ ret-lst int-lst
    repeat
                                            \ ret-lst lst (empty)
    list-deallocate                         \ ret-lst
;

\ Return true if any rlc, in an rlc list, is a superset of a given state-list-corr.
: rlc-list-any-superset-states ( slc1 rlc0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    list-get-links                          \ slc1 link
    begin
        ?dup
    while
        over                                \ slc1 link slc1
        over link-get-data                  \ slc1 link slc1 rlcx
        region-list-corr-superset-states    \ slc1 link bool
        if
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                            \ slc1
    drop                                    \
    false                                   \ bool
;

\ Return true if a rlclist has at least one superset of the initial and result
\ states of a regioncorr.
\ : _rlc-list-any-superset-regioncorr ( reglstcor1 rlcl0 -- bool )
    \ Check args.
\    assert-tos-is-list
\    assert-nos-is-list

\    \ Check for superest of regioncorr initial states.
\    over regioncorr-get-initial         \ smpc1 rlcl0 smpc1-i
\    over                                \ smpc1 rlcl0 smpc1-i rlcl0
\    rlc-list-any-superset-states        \ smpc1 rlcl0 bool
\    is-false if
\        2drop
\        false
\        exit
\    then

\    \ Check for superest of regioncorr result states.
\    swap regioncorr-get-result      \ rlcl0 smpc1-r
\    swap                            \ smpc1-r rlgl0
\    rlc-list-any-superset-states    \ bool
\ ;

\ Return true if an rlc, in a rlc list, is a superset of both states in a regioncorr.
\ : _rlc-list-one-superset-regioncorr ( reglstcor1 rlc0 -- bool )
    \ Check args.
\    assert-tos-is-list
\    assert-nos-is-list

\    list-get-links                              \ smplc1 link
\    begin
\        ?dup
\    while
\        over regioncorr-get-initial             \ smplc1 link smpl-i
\        over link-get-data                      \ smplc1 link smpl-i rlcx
\        region-list-corr-superset-states        \ smplc1 link bool
\        if                                      \ smplc1 link
\            over regioncorr-get-result          \ smplc1 link smpl-r
\            over link-get-data                  \ smplc1 link smpl-r rlcx
\            region-list-corr-superset-states    \ smplc1 link bool
\            if                                  \ smplc1 link
\                2drop                           \
\                true                            \ bool
\                exit
\            then
\        then

\        link-get-next
\    repeat
\                                                \ smplc1
\    drop                                        \
\    false                                       \ bool
\ ;

\ Return a list of rlcs superset of a state-list-corr.
: rlc-list-superset-states-rlc-list ( slc1 rlc0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ Init return list.
    list-new -rot                           \ ret-lst slc1 rlc0

    list-get-links                          \ ret-lst slc1 link
    begin
        ?dup
    while
        over                                \ ret-lst slc1 link slc1
        over link-get-data                  \ ret-lst slc1 link slc1 rlcx
        region-list-corr-superset-states    \ ret-lst slc1 link bool
        if                                  \ ret-lst slc1 link
            dup link-get-data               \ ret-lst slc1 link rlcx
            #3 pick                         \ ret-lst slc1 link rlcx ret-lst
            rlc-list-push                   \ ret-lst slc1 link
        then

        link-get-next
    repeat
                                            \ ret-lst slc1
    drop                                    \ ret-lst
;

\ Return the least different value to two given rlcs, not counting equal rlcs.
: rlc-list-least-difference ( rlc2 rlc1 rlc-lst0 -- u )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    assert-3os-is-list

    \ Init return count.
    #2 pick #2 pick             \ rlc2 rlc1 rlc-lst0 rlc2 rlc1
    region-list-corr-distance   \ rlc2 rlc1 rlc-lst0 u
    dup 0= abort" two rcls intersect?"
    cr ." rlc1 " #2 pick .region-list-corr
    space ." and " #3 pick .region-list-corr
    space ." is " dup . cr
    
    swap                        \ rlc2 rlc1 dist rlc-lst0

    \ Prep for loop.
    list-get-links              \ rlc2 rlc1 dist link

    begin
        2dup
    while
        \ Check rlcx ne rlc1.
        dup link-get-data       \ rlc2 rlc1 dist link rlcx
        #3 pick                 \ rlc2 rlc1 dist link rlcx rlc1
        <> if
            \ Check rlcx ne rlc2
            dup link-get-data       \ rlc2 rlc1 dist link rlcx
            #4 pick                 \ rlc2 rlc1 dist link rlcx rlc2
            <> if
                \ Calc distance to rlc2.
                dup link-get-data           \ rlc2 rlc1 dist link rlcx
                #4 pick                     \ rlc2 rlc1 dist link rlcx rlc2
                region-list-corr-distance   \ rlc2 rlc1 dist link dist2
                
                \ Calc distance to rlc1.
                over link-get-data          \ rlc2 rlc1 dist link dist2 rlcx
                cr ." for " dup .region-list-corr
                #4 pick                     \ rlc2 rlc1 dist link dist2 rlcx rlc1
                region-list-corr-distance   \ rlc2 rlc1 dist link dist2 dist1

                \ Add distance 1 and 2.
                +                           \ rlc2 rlc1 dist link dist12

                \ Compare with current least dist.
                dup                         \ rlc2 rlc1 dist link dist12 dist12
                #3 pick                     \ rlc2 rlc1 dist link dist12 dist12 dist
                space ." compare cur dist " dup . space ." with new " over . cr
                <                           \ rlc2 rlc1 dist link dist12 bool
                if                          \ rlc2 rlc1 dist link dist12
                    \ Update current least distance.
                    rot drop                \ rlc2 rlc1 link dist12
                    swap                    \ rlc2 rlc1 dist12 link
                else
                    drop                    \ rlc2 rlc1 dist link
                then
            then
        then

        link-get-next
    repeat
                                \ rlc2 rlc1 dist
    nip nip                     \ dist
;
