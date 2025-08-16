\ Functions for region lists.

\ Deallocate a region list.
: region-list-deallocate ( list0 -- )
    [ ' region-deallocate ] literal over list-apply \ Deallocate region instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

' region-list-deallocate to region-list-deallocate-xt

\ Return the intersection of two region lists.
: region-list-set-intersection ( list1 list0 -- list-result )
    [ ' region-eq ] literal -rot        \ xt list1 list0
    list-intersection                   \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the union of two region lists.
: region-list-set-union ( list1 list0 -- list-result )
    [ ' region-eq ] literal -rot        \ xt list1 list0
    list-union                          \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

' region-list-set-union to region-list-set-union-xt

\ Return the difference of two region lists.
: region-list-set-difference ( list1 list0 -- list-result )
    [ ' region-eq ] literal -rot        \ xt list1 list0
    list-difference                     \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Print a region-list
: .region-list ( list0 -- )
    \ Check args.
    assert-tos-is-list
    [ ' .region ] literal swap .list
;

\ Push a region to a region-list.
: region-list-push ( reg1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    over struct-inc-use-count
    list-push
;

' region-list-push to region-list-push-xt

\ Remove a region from a region-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if a region was removed.
: region-list-remove ( xt reg list -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    list-remove         \ reg flag
    if  
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
    assert-tos-is-list
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
        [ ' region-subset-of ] literal -rot \ reg1 list0 xt reg1 list0
        region-list-remove                  \ reg1 list0 | flag
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
    assert-tos-is-list
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
        [ ' region-superset-of ] literal -rot \ reg1 list0 xt reg1 list0
        region-list-remove                  \ reg1 list0 | flag
    while
    repeat

    \ reg1 list0
    region-list-push
    true
;

\ Return a copy of a region-list.
: region-list-copy ( lst0 -- lst-copy )
    \ Check arg.
    assert-tos-is-list

    list-new swap           \ lst-n lst0

    list-get-links          \ lst-n link

    begin
        dup
    while
        dup link-get-data   \ lst-n link region
        2 pick              \ lst-n link region lst-n
        region-list-push    \ lst-n link
        
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
    assert-tos-is-list
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
                    \ cr ." at 2" cr
                    dup link-get-data       \ ret-lst reg1 link r-lst link reg2
                    5 pick                  \ ret-lst reg1 link r-lst link reg2 ret-lst
                    region-list-push-nosubs \ ret-lst reg1 link r-lst link flag
                    drop                    \ ret-lst reg1 link r-lst link
                    link-get-next
                repeat
                                            \ ret-lst reg1 link r-lst 0
                drop region-list-deallocate \ ret-lst reg1 link
            else
                \ Add whole region to the result.
                nip                         \ ret-lst reg1 link reg2
                3 pick                      \ ret-lst reg1 link reg2 ret-lst
                region-list-push-nosubs     \ ret-lst reg1 link flag
                drop                        \ ret-lst reg1 link
            then
        then

        link-get-next           \ ret-lst reg1 next-link-or-0
    repeat
                                \ ret-lst reg1 0
    2drop                       \ ret-lst
;

' region-list-subtract-region to region-list-subtract-region-xt

\ From the TOS region-list, subtract the NOS region-list.
\ In many cases, you should run region-list-subtract-n instead.
\ Or do intersections\subtractions, followed by region-list-normalize.
: region-list-subtract ( lst1 lst0 -- lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

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
    assert-tos-is-list

    list-new                    \ lst0 lst1
    domain-max-region-xt execute           \ lst0 lst1 regM
    over region-list-push       \ lst0 lst1
    2dup                        \ lst0 lst1 lst0 lst1
    region-list-subtract        \ lst0 lst1 lst2
    swap region-list-deallocate \ lst0 lst2
    nip                         \ lst2
;

\ Return a nomalized list.
\ That is with maximum unions and overlaps.
: region-list-normalize ( lst0 -- lst )
    \ Check arg.
    assert-tos-is-list

    region-list-complement      \ lst1
    dup region-list-complement  \ lst1 lst2
    swap                        \ lst2 lst1
    region-list-deallocate      \ lst2
;

\ Return a list of region intersections with a region-list, no subsets.
\ In many cases, you should run region-list-region-intersections-n instead.
\ Or do intersections\subtractions, followed by region-list-normalize.
: region-list-region-intersections ( list1 list0 -- list-result )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ list1 list0
    list-get-links                  \ list1 link0
    list-new -rot                   \ ret-list list1 link0
    begin
        dup
    while
                                    \ ret-list list1 link0
        dup link-get-data           \ ret-list list1 link0 data0
        2 pick list-get-links       \ ret-list list1 link0 data0 link1

        begin
            dup
        while
            dup link-get-data       \ ret-list list1 link0 data0 link1 data1
            2 pick                  \ ret-list list1 link0 data0 link1 data1 data0
            region-intersection     \ ret-list list1 link0 data0 link1 | reg-int true | false
            if
                                        \ ret-list list1 link0 data0 link1 | reg-int
                dup                     \ ret-list list1 link0 data0 link1 | reg-int reg-int
                6 pick                  \ ret-list list1 link0 data0 link1 | reg-int reg-int ret-list
                region-list-push-nosubs \ ret-list list1 link0 data0 link1 | reg-int flag
                if
                    drop
                else
                    dup struct-inc-use-count
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
    assert-tos-is-list
    assert-nos-is-region

    [ ' region-eq ] literal -rot list-member
;

\ Return true if a region-list contains a superset, or equal, region.
: region-list-any-superset-of ( reg1 list0 -- flag )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    [ ' region-superset-of ] literal -rot list-member
;

\ Return a list of regions that use a given state.
: region-list-uses-state ( sta1 reg-lst0 -- list )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-value

   [ ' region-uses-state ] literal -rot list-find-all       \ lst
   [ ' struct-inc-use-count ] literal over list-apply       \ lst
;

: region-list-region-intersections-n ( lst1 lst0 -- lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    region-list-region-intersections    \ ret0
    dup region-list-normalize           \ ret1 ret2
    swap region-list-deallocate         \ ret2
;

\ Subtract NOS region from TOS list.
: region-list-subtract-region-n ( reg1 lst0 -- lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    region-list-subtract-region     \ ret0
    dup region-list-normalize       \ ret1 ret2
    swap region-list-deallocate     \ ret2
;


\ Subtract the NOS list from the TOS list, with normalization.
: region-list-subtract-n ( lst1 lst0 -- lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    region-list-subtract            \ ret0
    dup region-list-normalize       \ ret1 ret2
    swap region-list-deallocate     \ ret2
;

\ Return a list of states, no dups, used to form regions in a list.
: region-list-states ( reg-lst0 -- list )
    \ Check arg.
    assert-tos-is-list

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
        3 pick                  \ ret-lst link reg sta0 ret-lst
        [ ' = ] literal -rot    \ ret-lst link reg xt sta0 ret-lst
        list-member             \ ret-lst link reg flag
        0= if
            dup                 \ ret-lst link reg reg
            region-get-state-0  \ ret-lst link reg sta0
            3 pick              \ ret-lst link reg sta0 ret-lst
            list-push           \ ret-lst link reg
        then

        \ Check region-state-1.
        dup region-get-state-1  \ ret-lst link reg sta1
        3 pick                  \ ret-lst link reg sta1 ret-lst
        [ ' = ] literal -rot    \ ret-lst link reg xt sta0 ret-lst
        list-member             \ ret-lst link reg flag
        0= if
            region-get-state-1  \ ret-lst link sta1
            2 pick              \ ret-lst link sta1 ret-lst
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
    assert-tos-is-list
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
                2drop drop false
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

\ Return a list of states only in one region of a region-list.
: region-list-states-in-one-region ( sta-lst1 reg-lst0 -- sta-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list

    \ State list to TOS, to scan through.
    swap                        \ reg-lst0 sta-lst1

    \ Init return list.
    list-new swap               \ reg-lst1 ret-lst sta-lst

    \ Get first link.
    list-get-links              \ reg-lst1 ret-lst link

    begin
        ?dup
    while
        dup link-get-data               \ reg-lst1 ret-lst link data
        3 pick                          \ reg-lst1 ret-lst link data reg-lst
        region-list-state-in-one-region \ reg-lst1 ret-lst link flag
        if
            dup link-get-data           \ reg-lst1 ret-lst link data
            2 pick                      \ reg-lst1 ret-lst link data ret-lst
            list-push                   \ reg-lst1 ret-lst link
        then

        link-get-next                   \ reg-lst1 ret-lst link
    repeat
                                        \ reg-lst1 ret-lst
    nip                                 \ ret-lst
;

\ Return a list of defining regions, that is
\ regions that are not completely overlapped by other regions.
\ : region-list-defining-regions ( lst0 -- lst )
\    \ Check arg.
\    assert-tos-is-list

\    lst-new swap                \ lst-n lst0
\    dup list-get-links          \ lst-n lst0 link0
    \ Check each region.
\    begin
\        ?dup
\    while
\        dup link-get-data       \ lst-n lst0 link0 data0

        \ Check against other regions.
\        dup                     \ lst-n lst0 link0 data0 data0
\        list-new tuck list-push \ lst-n lst0 link0 data0 lst1

\        link-get-next
\    repeat                      \ lst-n lst0
\    drop
\ ;

