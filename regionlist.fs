\ Functions for region lists.

\ Check if tos is an empty list, or has a region instance as its first item.
: assert-tos-is-region-list ( tos -- tos )
    assert-tos-is-list
    dup list-is-not-empty
    if
        dup list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Check if nos is an empty list, or has a region instance as its first item.
: assert-nos-is-region-list ( nos tos -- nos tos )
    assert-nos-is-list
    over list-is-not-empty
    if
        over list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Check if 3os is a list, if non-empty, with the first item being a region.
: assert-3os-is-region-list ( nos tos -- nos tos )
    assert-3os-is-list
    #2 pick list-is-not-empty
    if
        #2 pick list-get-links link-get-data
        assert-tos-is-region
        drop
    then
;

\ Deallocate a region list.
: region-list-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-region-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate region instances in the list.
        [ ' region-deallocate ] literal over        \ lst0 xt lst0
        list-apply                                  \ lst0
    then

    \ Deallocate the list.
    list-deallocate                                 \
;

\ Return the intersection of two region lists.
: region-list-set-intersection ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    [ ' region-eq ] literal -rot        \ xt list1 list0
    list-intersection-struct            \ list-result
;

\ Return the union of two region lists.
: region-list-set-union ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    [ ' region-eq ] literal -rot        \ xt list1 list0
    list-union-struct                   \ list-result
;

\ ' region-list-set-union to region-list-set-union-xt

\ Return the difference of two region lists.
: region-list-set-difference ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    [ ' region-eq ] literal -rot        \ xt list1 list0
    list-difference-struct              \ list-result
;

\ Print a region-list
: .region-list ( list0 -- )
    \ Check arg.
    assert-tos-is-region-list

    [ ' .region ] literal swap .list
;

\ Push a region to a region-list.
: region-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    list-push-struct
;

' region-list-push to region-list-push-xt

\ Push a region into a list, if there are no duplicates in the list.
\ Return true if the region is added to the list.
: region-list-push-nodups ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    \ Return if any region in the list is a duplicate of reg1.
    2dup                                    \ reg1 list0 reg1 list0
    [ ' region-eq ] literal                 \ reg1 list0 reg1 list0 xt
    -rot                                    \ reg1 list0 xt reg1 list0
    list-member                             \ reg1 list0 flag
    if
        2drop
        false
        exit
    then
                                            \ reg1 list0

    \ reg1 list0
    region-list-push
    true
;

\ Push a region to the end of a region-list.
: region-list-push-end ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    list-push-end-struct
;

\ Remove a region from a region-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a region was removed.
: region-list-remove ( reg list -- bool )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-eq ] literal     \ reg1 list0  xt
    -rot                        \ xt reg1 list0

    list-remove                 \ reg2 true | false
    if
        region-deallocate
        true
    else
        false
    then
;

\ Remove the first subset region from a region-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a region was removed.
: region-list-remove-subset ( reg list -- bool )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-subset-of ] literal      \ reg1 list0  xt
    -rot                                \ xt reg1 list0

    list-remove                         \ reg2 true | false
    if
        \ cr ." sub reg removed: " dup .region cr
        region-deallocate
        true
    else
        false
    then
;

\ Remove the first subset region from a region-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a region was removed.
: region-list-remove-superset ( reg list -- bool )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-superset-of ] literal    \ reg1 list0  xt
    -rot                                \ xt reg1 list0

    list-remove                         \ reg2 true | false
    if
        \ cr ." sup reg removed: " dup .region cr
        region-deallocate
        true
    else
        false
    then
;

\ Push a region onto a list, if there are no supersets in the list.
\ If there are no supersets in the list, delete any subsets and push the region.
\ Return true if the region is added to the list.
: region-list-push-nosubs ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    \ Return if any region in the list is a superset of reg1.
    2dup                                    \ reg1 list0 reg1 list0
    [ ' region-superset-of ] literal        \ reg1 list0 reg1 list0 xt
    -rot                                    \ reg1 list0 xt reg1 list0
    list-member                             \ reg1 list0 flag
    if
        2drop
        false
        exit
    then
                                            \ reg1 list0

    begin
        2dup                                \ reg1 list0 reg1 list0
        region-list-remove-subset           \ reg1 list0 | flag
    while
    repeat

    \ reg1 list0
    region-list-push
    true
;

\ Push a region onto a list, if there are no subsets in the list.
\ If there are no subsets in the list, delete any supersets and push the region.
\ Return true if the region is added to the list.
: region-list-push-nosups ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    \ Return if any region in the list is a superset of reg1.
    2dup                                    \ reg1 list0 reg1 list0
    [ ' region-subset-of ] literal          \ reg1 list0 reg1 list0 xt
    -rot                                    \ reg1 list0 xt reg1 list0
    list-member                             \ reg1 list0 flag
    if
        2drop
        false
        exit
    then
                                            \ reg1 list0

    begin
        2dup                                \ reg1 list0 reg1 list0
        region-list-remove-superset         \ reg1 list0 | flag
    while
    repeat

    \ reg1 list0
    region-list-push
    true
;

\ Return a copy of a region-list.
: region-list-copy ( lst0 -- lst-copy )
    \ Check arg.
    assert-tos-is-region-list

    list-new swap           \ lst-n lst0

    list-get-links          \ lst-n link

    begin
        dup
    while
        dup link-get-data       \ lst-n link region
        #2 pick                 \ lst-n link region lst-n
        region-list-push-end    \ lst-n link

        link-get-next       \ lst-n link
    repeat
    \ lst-n 0
    drop
;

\ Return a TOS region-list minus the NOS region.
\ In many cases, you should run region-list-subtract-region-n instead.
\ Or do intersections\subtractions, followed by region-list-normalize.
: region-list-subtract-region ( reg1 lst0 -- lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    \ Init return list.
    list-new -rot                   \ ret-lst reg1 lst0

    \ Scan through the given list.
    list-get-links                  \ ret-lst reg1 link
    begin
        dup
    while
        over                        \ ret-lst reg1 link reg1
        over link-get-data          \ ret-lst reg1 link reg1 reg2

        \ Test if equal
        2dup region-eq              \ ret-lst reg1 link reg1 reg2 flag
        if
           2drop
            \ Skip, region does not appear in the result.
        else
            \ Check if they intersect
            2dup region-intersects  \ ret-lst reg1 link reg1 reg2 flag
            if
                \ They intersect, there will be same remainder.
                region-subtract     \ ret-lst reg1 link remainder-lst
                \ Add remainders to the return list
                dup list-get-links  \ ret-lst reg1 link r-lst link
                begin
                    dup
                while
                    dup link-get-data       \ ret-lst reg1 link r-lst link reg2
                    #5 pick                 \ ret-lst reg1 link r-lst link reg2 ret-lst
                    region-list-push-nosubs \ ret-lst reg1 link r-lst link flag
                    drop                    \ ret-lst reg1 link r-lst link
                    link-get-next
                repeat
                                            \ ret-lst reg1 link r-lst 0
                drop region-list-deallocate \ ret-lst reg1 link
            else
                \ Add whole region to the result.
                nip                         \ ret-lst reg1 link reg2
                #3 pick                     \ ret-lst reg1 link reg2 ret-lst
                region-list-push-nosubs     \ ret-lst reg1 link flag
                drop                        \ ret-lst reg1 link
            then
        then

        link-get-next           \ ret-lst reg1 next-link-or-0
    repeat
                                \ ret-lst reg1 0
    2drop                       \ ret-lst
;

\ ' region-list-subtract-region to region-list-subtract-region-xt

\ From the TOS region-list, subtract the NOS region-list.
\ In some cases, you should run region-list-subtract-n instead.
\ Or do intersections\subtractions, followed by region-list-normalize.
: region-list-subtract ( lst1 lst0 -- lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    \ Make a list that way be returned empty, or deallocated.
    region-list-copy                \ lst1 lst0

    swap                            \ lst0 lst1

    \ Process each region in lst1.
    list-get-links                  \ lst0 link
    begin
        dup
    while
        dup link-get-data           \ lst0 link region
        rot                         \ link region lst0
        swap                        \ link lst0 region
        over                        \ link lst0 region lst0
        region-list-subtract-region \ link lst0 lst0-new
        -rot                        \ lst0-new link lst0
        region-list-deallocate      \ lst0-new link
        link-get-next
    repeat
    \ lst0-new link(0)
    drop
;

\ Return a region-list complement, that is max-region minus region-list.
: region-list-complement ( lst0 -- lst1 )
    \ Check arg.
    assert-tos-is-region-list

    list-new                    \ lst0 lst1
    current-max-region          \ lst0 lst1 regM
    over region-list-push       \ lst0 lst1
    2dup                        \ lst0 lst1 lst0 lst1
    region-list-subtract        \ lst0 lst1 lst2
    swap region-list-deallocate \ lst0 lst2
    nip                         \ lst2
;

\ Return a region-list, normalized.
\ That is, combine adjacent regions.
: region-list-normalize ( lst0 -- lst )
    \ Check arg.
    assert-tos-is-region-list

    \ Normalize, by double complement.
    region-list-complement          \ lst
    dup                             \ lst lst
    region-list-complement          \ lst lst'

    \ Clean up.
    swap region-list-deallocate     \ lst'
;

\ Return a list of region intersections with a region-list, no subsets.
\ In many cases, you should run region-list-intersections-n instead.
\ Or do intersections\subtractions, followed by region-list-normalize.
: region-list-intersections-nosubs ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    \ list1 list0
    list-get-links                  \ list1 link0
    list-new -rot                   \ ret-list list1 link0
    begin
        dup
    while
                                    \ ret-list list1 link0
        dup link-get-data           \ ret-list list1 link0 data0
        #2 pick list-get-links      \ ret-list list1 link0 data0 link1

        begin
            dup
        while
            dup link-get-data       \ ret-list list1 link0 data0 link1 data1
            #2 pick                 \ ret-list list1 link0 data0 link1 data1 data0
            region-intersection     \ ret-list list1 link0 data0 link1, reg-int true | false
            if
                \ cr ." reg int: " dup .region cr
                                        \ ret-list list1 link0 data0 link1 reg-int
                dup                     \ ret-list list1 link0 data0 link1 reg-int reg-int
                #6 pick                 \ ret-list list1 link0 data0 link1 reg-int reg-int ret-list
                region-list-push-nosubs \ ret-list list1 link0 data0 link1 reg-int flag
                if
                    drop
                else
                    region-deallocate
                then
            then
                                    \ ret-list list1 link0 data0 link1
            link-get-next           \ ret-list list1 link0 data0 link1-next
        repeat
        2drop                       \ ret-list list1 link0
        link-get-next               \ ret-list list1 link0-next
    repeat
    \ ret-list list1 0
    2drop                           \ ret-list
;

\ Return true if a region is in a region-list.
: region-list-member ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-eq ] literal -rot list-member
;

\ Return true if a region-list contains a superset, or equal, region.
: region-list-any-superset-of ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-superset-of ] literal -rot list-member
;

\ Return true if a region-list contains a subset, or equal, region.
: region-list-any-subset-of ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-subset-of ] literal -rot list-member
;

\ Return true if a region-list contains a intersection of a region.
: region-list-any-intersection-of ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-intersects ] literal -rot list-member
;

: region-list-intersections-of-region ( reg1 lst0 -- reg-lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    [ ' region-intersects ] literal -rot list-find-all-struct
;

\ Return a list of regions that use a given state.
: region-list-uses-state ( sta1 reg-lst0 -- list )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-value

   [ ' region-uses-state ] literal -rot list-find-all-struct    \ lst
;

: region-list-intersections-n ( lst1 lst0 -- lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    region-list-intersections-nosubs    \ ret0
    dup region-list-normalize           \ ret1 ret2
    swap region-list-deallocate         \ ret2
;

\ Subtract NOS region from TOS list.
: region-list-subtract-region-n ( reg1 lst0 -- lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    region-list-subtract-region     \ ret0
    dup region-list-normalize       \ ret1 ret2
    swap region-list-deallocate     \ ret2
;


\ Subtract the NOS list from the TOS list, with normalization.
: region-list-subtract-n ( lst1 lst0 -- lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    region-list-subtract            \ ret0
    dup region-list-normalize       \ ret1 ret2
    swap region-list-deallocate     \ ret2
;

\ Return a list of states, no dups, used to form regions in a list.
: region-list-states ( reg-lst0 -- list )
    \ Check arg.
    assert-tos-is-region-list

    \ Init return list
    list-new swap               \ ret-lst reg-lst

    \ Scan region list
    list-get-links              \ ret-lst link
    begin
        ?dup
    while
        dup link-get-data       \ ret-lst link reg

        \ Check region-state-0.
        dup region-get-state-0  \ ret-lst link reg sta0
        #3 pick                 \ ret-lst link reg sta0 ret-lst
        [ ' = ] literal -rot    \ ret-lst link reg xt sta0 ret-lst
        list-member             \ ret-lst link reg flag
        0= if
            dup                 \ ret-lst link reg reg
            region-get-state-0  \ ret-lst link reg sta0
            #3 pick             \ ret-lst link reg sta0 ret-lst
            list-push           \ ret-lst link reg
        then

        \ Check region-state-1.
        dup region-get-state-1  \ ret-lst link reg sta1
        #3 pick                 \ ret-lst link reg sta1 ret-lst
        [ ' = ] literal -rot    \ ret-lst link reg xt sta0 ret-lst
        list-member             \ ret-lst link reg flag
        0= if
            region-get-state-1  \ ret-lst link sta1
            #2 pick             \ ret-lst link sta1 ret-lst
            list-push           \ ret-lst link
        else
            drop                \ ret-lst link
        then

        link-get-next
    repeat
                                \ ret-lst
;

\ Return true if a square is in exactly one region.
: region-list-state-in-one-region ( sta1 reg-lst0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-value

    \ Get first link of list.
    list-get-links              \ sta0 link

    \ Init counter.
    0 -rot                      \ ctr sta0 link

    \ Scan list.
    begin
        ?dup
    while
        \ Check if the current list item is a superset of the state.
        2dup                        \ ctr sta lnk sta lnk
        link-get-data               \ ctr sta lnk sta region
        region-superset-of-state    \ ctr sta lnk flag
        if
            \ Get counter.
            rot             \ sta lnk ctr

            \ Check if counter is already 1.
            dup 0<>
            if
                \ Counter is 1, return false.
                3drop false
                exit
            then

            \ Increment the counter.
            1+              \ sta lnk ctr
            \ Store counter.
            -rot            \ ctr sta lnk
        then

        link-get-next       \ ctr sta lnk
    repeat
                            \ ctr sta
    drop                    \ ctr  s/b 0 or 1.
    0<>                     \ flag
;

\ Return true if to region-lists are equal.
: region-list-eq ( lst1 lst0 -- flag )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    \ Check list lengths.
    over list-get-length
    over list-get-length                \ lst1 lst0 len1 len0
    <>
    if
        2drop
        false
        exit
    then

    \  Check list contents.
    list-get-links                      \ lst1 link
    begin
        ?dup
    while
        \ Get current region.
        dup link-get-data               \ lst1 link data

        \ Check if its in the other list.
        [ ' region-eq ] literal swap    \ lst1 link xt data
        #3 pick                         \ lst1 link xt data lst1
        list-member                     \ lst1 link flag

        0= if
            2drop
            false
            exit
        then

        link-get-next                   \ lst1 link
    repeat
                                        \ lst1
    drop
    true
;

\ Return a list of regions a state is in.
: region-list-regions-state-in ( sta1 lst0 -- reg-lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-value

    \ Init return list.
    list-new -rot                       \ ret-lst sta lst0

    \ Prep for loop.
    list-get-links                      \ ret-lst sta link

    \ Check each region.
    begin
        ?dup
    while
        \ Check the current region.
        over                            \ ret-lst sta link sta1
        over link-get-data              \ ret-lst sta link sta1 regx
        region-superset-of-state        \ ret-lst sta link flag
        if
            \ Add the region to the return list.
            dup link-get-data           \ ret-lst sta link regx
            #3 pick                     \ ret-lst sta link regx ret-lst
            list-push-struct            \ ret-lst sta link
        then

        link-get-next
    repeat

    drop                                \ ret-lst
;

\ Return the number of regions a state is in.
: region-list-number-regions-state-in ( sta1 lst0 -- u )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-value

    \ Init count.
    0 -rot                              \ cnt sta lst0

    \ Prep for loop.
    list-get-links                      \ cnt sta link

    \ Check each region.
    begin
        ?dup
    while
        \ Check the current region.
        over                            \ cnt sta link sta1
        over link-get-data              \ cnt sta link sta1 regx
        region-superset-of-state        \ cnt sta link flag
        if                              \ cnt sta link
            \ Inc counter
            rot 1 + -rot                \ cnt sta link
        then

        link-get-next
    repeat

    drop                                \ cnt
;

\ Return true if a state is in any region.
: region-list-any-superset-state ( sta1 lst0 -- bool )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-value

    \ Prep for loop.
    list-get-links                      \ sta1 link

    \ Check each region.
    begin
        ?dup
    while
        \ Check the current region.
        2dup                            \ sta1 link sta1 link
        link-get-data                   \ sta1 link sta1 regx
        region-superset-of-state        \ sta1 link flag
        if                              \ sta1 link
            2drop
            true
            exit
        then

        link-get-next
    repeat
                                        \ sta1
    drop
    false
;

\ Copy a region-list, removing subsets.
: region-list-copy-nosubs ( lst0 - lst )
    \ Check arg.
    assert-tos-is-region-list

    list-new swap               \ ret lst0
    list-get-links              \ ret link

    begin
        ?dup
    while
        dup link-get-data       \ ret link regx
        #2 pick                 \ ret link regx ret
        region-list-push-nosubs \ ret link flag
        drop

        link-get-next           \ ret link
    repeat
;

\ Copy a region-list, removing duplicates.
: region-list-copy-nodups ( lst0 - lst )
    \ Check arg.
    assert-tos-is-region-list

    list-new swap               \ ret lst0
    list-get-links              \ ret link

    begin
        ?dup
    while
        dup link-get-data       \ ret link regx
        #2 pick                 \ ret link regx ret
        region-list-push-nodups \ ret link flag
        drop

        link-get-next           \ ret link
    repeat
;

\ Append nos region-list to the tos region-list.
: region-list-append ( lst1 lst0 -- )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    swap                    \ lst0 lst1
    list-get-links          \ lst0 link
    begin
        ?dup
    while
        dup link-get-data   \ lst0 link nedx
        #2 pick             \ lst0 link nedx lst0
        region-list-push    \ lst0 link

        link-get-next
    repeat
                        \ lst0
    drop
;

\ Append nos region-list to the tos region-list, no duplicates.
: region-list-append-nodups ( lst1 lst0 -- )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    swap                        \ lst0 lst1
    list-get-links              \ lst0 link
    begin
        ?dup
    while
        dup link-get-data       \ lst0 link nedx
        #2 pick                 \ lst0 link nedx lst0
        region-list-push-nodups \ lst0 link flag
        drop

        link-get-next
    repeat
                                \ lst0
    drop
;

\ Return a list of region-pair intersections.
\ Duplicates are avoided, but proper subsets are Ok.
: region-list-intersections-nodups ( reg-lst0 -- reg-lst)
    \ Check arg.
    assert-tos-is-region-list

    \ Init return list.
    list-new swap                       \ ret-lst reg-lst0
    list-get-links                      \ ret-lst link0

    \ For each region.
    begin
        ?dup
    while
        \ Get link to following regions.
        \ Having direct access to the list links makes this logic effortless,
        \ compared to using indices at a higher level.
        dup link-get-next               \ ret-lst link0 link+

        \ For each following region.
        begin
            ?dup
        while
            over link-get-data          \ ret-lst link0 link+ reg0
            over link-get-data          \ ret-lst link0 link+ reg0 reg+
            region-intersection         \ ret-lst link0 link+, reg-int t | f
            if                          \ ret-lst link0 link+ reg-int
                dup                     \ ret-lst link0 link+ reg-int reg-int
                #4 pick                 \ ret-lst link0 link+ reg-int reg-int ret-lst
                region-list-push-nodups \ ret-lst link0 link+ reg-int bool
                if
                    drop                \ ret-lst link0 link+
                else
                    region-deallocate   \ ret-lst link0 link+
                then
            then

            link-get-next               \ ret-lst link0 link+
        repeat

        link-get-next                   \ ret-lst link0
    repeat
                                        \ ret-lst
;

\ Return fragments of a given region-list.
\ The fragments will account for all parts of the given region-list.
\ All fragments will be within all regions of the given region-list that they intersect.
\ Intermediate regions may be proper subsets, but duplicates will be avoided.
: region-list-intersection-fragments ( lst0 -- frag-lst )
    \ Check arg.
    assert-tos-is-region-list

    \ Insure-no-duplicates.
    list-new swap                           \ ret-lst lst0'
    region-list-copy-nodups                 \ ret-lst lst0'

    begin
        dup list-is-empty 0=
    while
        dup                                 \ ret-lst lst0' lst0'

        \ Get intersections.
        region-list-intersections-nodups    \ ret-lst lst0' int-lst

        \ Get whats left over.
        2dup swap                           \ ret-lst lst0' int-lst int-lst lst0'
        region-list-subtract                \ ret-lst lst0' int-lst left-over-lst
        \ cr ." list: " #2 pick .region-list
        \ space ." - " over .region-list
        \ space ." = " dup .region-list
        \ cr

        \ Add left over to result list.
        dup                                 \ ret-lst lst0' int-lst left-over-lst left-over-lst
        #4 pick                             \ ret-lst lst0' int-lst left-over-lst left-over-lst ret-lst
        region-list-append-nodups           \ ret-lst lst0' int-lst left-over-lst

        \ Clean up, intersections become the next cycle lst0'.
        region-list-deallocate              \ ret-lst lst0' int-lst
        swap                                \ ret-lst int-lst lst0'
        region-list-deallocate              \ ret-lst int-lst
    repeat
                                            \ ret-lst lst (empty)
    list-deallocate                         \ ret-lst
;

\ Return a copy of a region-list, except for a given region ia a given index.
\ Used by regioncorr subtraction.
: region-list-copy-except ( reg2 inx1 lst0 -- lst )
    \ Check args.
    assert-tos-is-region-list
    over 0 < abort" index out of range"
    over over list-get-length < is-false abort" index out of range"
    assert-3os-is-region

    \ Init return list.
    list-new swap                   \ reg2 inx1 ret-lst lst0

    \ Init index counter
    0 swap                          \ reg2 inx1 ret-lst ctr lst0

    \ Prep for loop.
    list-get-links                  \ reg2 inx1 ret-lst ctr link

    begin
        ?dup
    while
        over                        \ reg2 inx1 ret-lst ctr link ctr
        #4 pick                     \ reg2 inx1 ret-lst ctr link ctr inx1
        = if                        \ reg2 inx1 ret-lst ctr link
            #4 pick                 \ reg2 inx1 ret-lst ctr link reg2
            #3 pick                 \ reg2 inx1 ret-lst ctr link reg2 ret-lst
            region-list-push-end    \ reg2 inx1 lst-ret ctr link
        else                        \ reg2 inx1 ret-lst ctr link
            dup link-get-data       \ reg2 inx1 ret-lst ctr link region
            #3 pick                 \ reg2 inx1 ret-lst ctr link region ret-lst
            region-list-push-end    \ reg2 inx1 ret-lst ctr link
        then

        \ Inc counter.
        swap 1+ swap

        link-get-next               \ reg2 inx1 ret-lst ctr link
    repeat
                                    \ reg2 inx1 ret-lst ctr
    drop                            \ reg2 inx1 ret-lst
    nip nip                         \ ret-lst
;

: region-list-from-parsed-string ( [addr n]+ tkn-cnt -- reg-lst t | f ) \ Return a region-list from a parsed string.

    \ Save number tokens.
    dup >r

    \ Process each region, skip invalid regions.
    \ So clear stack of all tokens, valid or not.
                                            \ addr0 cnt0 tkn-cnt
    list-new swap                           \ addr0 cnt0 ret-lst tkn-cnt
    0 do                                    \ addrx cntx ret-lst
        rot rot                             \ addry cnty ret-lst addrx cntx
        region-from-string                  \ addry cnty ret-lst, regx t | f
        if
            \ Add the region.
            over list-push-end-struct       \ addry cnty ret-lst
        then
    loop
                                            \ ret-lst
    \ Test if there were any invalid regions.
    dup list-get-length                     \ ret-lst len
    r>                                      \ ret-lst len tkn-cnt
    =                                       \ ret-lst bool
    if
        true
    else
        region-list-deallocate
        false
    then
;

: region-list-from-string ( str-addr str-n -- reg-lst t | f )  \ Return a region-list from a string, like (x000 xx10).
    \ Get tokens.
    parse-string                \ [str-addr str-n ]+ tkn-cnt

    region-list-from-parsed-string
;

: region-list-from-string-a ( str-addr str-n -- regc )   \ Return a region-list from a string, or abort.
    region-list-from-string    \ reg-lst t | f
    is-false abort" region-list-from-string failed?"
;

: region-list-union-nosubs ( reg-lst1 reg-lst0 -- reg-lst ) \ Combine two reigion-lists, deleteng subsets.
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    \ Inti return list.
    list-new                \ reg-lst1 reg-lst0 ret-lst

    \ Prep for loop 1.
    swap list-get-links     \ reg-lst1 ret-lst link0

    begin
        ?dup
    while
        dup link-get-data       \ reg-lst1 ret-lst link0 reg0x
        #2 pick                 \ reg-lst1 ret-lst link0 reg0x ret-lst
        region-list-push-nosubs \ reg-lst1 ret-lst link0 bool
        drop

        link-get-next
    repeat
                                \ reg-lst1 ret-lst
    \ Prep for loop 2.
    swap list-get-links         \ ret-lst link1

    begin
        ?dup
    while
        dup link-get-data       \ ret-lst link1 reg1x
        #2 pick                 \ ret-lst link1 reg1x ret-lst
        region-list-push-nosubs \ ret-lst link1 bool
        drop

        link-get-next
    repeat
                                \ ret-lst
;

\ Check if a region is a defining region.
: region-list-region-is-defining ( reg1 reg-lst0 -- bool )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    over                                    \ reg1 reg-lst0 reg1
    region-copy                             \ reg1 reg-lst0 reg1c
    list-new tuck                           \ reg-lst0 rem-lst reg1 rem-lst
    list-push-struct                        \ reg-lst0 rem-lst

    \ Prep for loop.
    swap list-get-links                     \ reg1 rem-lst ls-link

    begin
        ?dup
    while
        dup link-get-data                   \ reg1 rem-lst ls-link regx

        \ Check if the region is the same as the given region. If so, skip it.
        #3 pick                             \ reg1 rem-lst ls-link regx reg1
        region-eq                           \ reg1 rem-lst ls-link bool
        if
        else
            dup link-get-data               \ reg1 rem-lst ls-link regx
            #2 pick                         \ reg1 rem-lst ls-link regx rem-lst
            region-list-subtract-region     \ reg1 rem-lst ls-link rem-lst'

            rot                             \ reg1 ls-link rem-lst' rem-lst
            region-list-deallocate          \ reg1 ls-link rem-lst'
            swap                            \ reg1 rem-lst' ls-link
        then

        link-get-next
    repeat
                                        \ reg1 rem-lst

    dup list-is-empty                   \ reg1 rem-lst bool
    if
        list-deallocate
        drop
        false
    else
        region-list-deallocate
        drop
        true
    then
;

\ Return the number of intersections for a given region.
: region-list-number-intersections ( reg1 lst0 -- reg-lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    \ Init count.
    0 -rot                              \ cnt reg1 lst0

    \ Prep for loop.
    list-get-links                      \ cnt reg1 link

    \ Check each region.
    begin
        ?dup
    while
        \ Check the current region.
        over                            \ cnt reg1 link reg1
        over link-get-data              \ cnt reg1 link reg1 regx
        region-intersects               \ cnt reg1 link flag
        if                              \ cnt reg1 link
            \ Inc counter
            rot 1 + -rot                \ cnt reg1 link
        then

        link-get-next
    repeat

    drop                                \ cnt
;

\ Return a list of stats, from a given list, that are a subset of any region.
: region-list-states-not-in ( sta-lst1 reg-lst0 -- sta-lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-value-list

    \ Init return list.
    list-new                            \ sta-lst1 reg-lst0 ret-lst

    \ Prep for loop.
    #2 pick list-get-links              \ sta-lst1 reg-lst0 ret-lst sta-link

    \ For each state in sta-lst1.
    begin
        ?dup
    while
        dup link-get-data               \ sta-lst1 reg-lst0 ret-lst sta-link stax
        #3 pick                         \ sta-lst1 reg-lst0 ret-lst sta-link stax reg-lst0
        region-list-any-superset-state  \ sta-lst1 reg-lst0 ret-lst sta-link bool
        if
        else
            dup link-get-data           \ sta-lst1 reg-lst0 ret-lst sta-link stax
            #2 pick                     \ sta-lst1 reg-lst0 ret-lst sta-link stax ret-lst
            list-push                   \ sta-lst1 reg-lst0 ret-lst sta-link
        then

        link-get-next
    repeat
                                        \ sta-lst1 reg-lst0 ret-lst
    nip nip
    \ cr ." region-list-states-not-in: returns: " dup .value-list cr
;

\ Return the union of all region x masks.
: region-list-union-x-masks ( lst0 -- msk )
    \ Check arg.
    assert-tos-is-region-list

    \ Init return mask.
    0 swap                              \ ret-msk lst0

    \ Prep for loop.
    list-get-links                      \ ret-msk link

    \ Check each region.
    begin
        ?dup
    while
        \ Check the current region.
        dup link-get-data               \ ret-msk link regx
        region-x-mask                   \ ret-msk link reg-x
        rot or                          \ link ret-msk'
        swap                            \ ret-msk' link

        link-get-next
    repeat
;

\ Return a list of regions that have one state in a given region.
: region-list-state-in-region ( reg1 reg-lst0 -- reg-lst )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region

    \ Init return list.
    list-new swap                       \ reg1 ret-lst reg-lst0
    list-get-links                      \ reg1 ret-lst reg-link
    begin
        ?dup
    while
        dup link-get-data               \ reg1 ret-lst reg-link regx
        region-get-states               \ reg1 ret-lst reg-link s1 s0
        #4 pick                         \ reg1 ret-lst reg-link s1 s0 reg1
        region-superset-of-state        \ reg1 ret-lst reg-link s1 bool
        if
            drop                        \ reg1 ret-lst reg-link
            dup link-get-data           \ reg1 ret-lst reg-link regx
            #2 pick                     \ reg1 ret-lst reg-link regx ret-lst
            list-push-struct            \ reg1 ret-lst reg-link
        else                            \ reg1 ret-lst reg-link s1
            #3 pick                     \ reg1 ret-lst reg-link s1 reg1
            region-superset-of-state    \ reg1 ret-lst reg-link bool
            if
                dup link-get-data       \ reg1 ret-lst reg-link regx
                #2 pick                 \ reg1 ret-lst reg-link regx ret-lst
                list-push-struct        \ reg1 ret-lst reg-link
            then
        then

        link-get-next
    repeat
                                        \ reg1 ret-lst
    nip
;

\ Deallocate a list of region lists.
: region-list-lol-deallocate ( lst0 -- )
    \ Check arg.
    assert-tos-is-list

    \ Check if the list will be deallocated for the last time.
    dup struct-get-use-count                        \ lst0 uc
    #2 < if
        \ Deallocate region-list instances in the list.
        [ ' region-list-deallocate ] literal over   \ lst0 xt lst0
        list-apply                                  \ lst0

        \ Deallocate the list.
        list-deallocate                             \
    else
        struct-dec-use-count
    then
;

\ Return true if a (tos) region-list is a subset of another (nos) list.
: region-list-subset-of ( reg-sup-lst reg-sub-lst -- bool )
    \ Check args.
    assert-tos-is-region-list
    assert-nos-is-region-list

    list-get-links          \ reg-sup-lst sub-link
    begin
        ?dup
    while
        dup link-get-data   \ reg-sup-lst sub-link sub-reg
        #2 pick             \ reg-sup-lst sub-link sub-reg reg-sup-lst
        region-list-member  \ reg-sup-lst sub-link bool
        if
        else
            2drop
            false
            exit
        then

        link-get-next
    repeat
                            \ reg-sup-lst
    drop
    true
;
