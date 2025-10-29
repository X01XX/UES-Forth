\ Print a region-list corresponding to the session domain list.
: .region-list-corr ( reg-lst0 )
    \ Check arg.
    assert-tos-is-list
    dup list-get-length
    session-get-number-domains-xt execute
    <> abort" Lists have different length?"

    list-get-links                                      \ link0
    session-get-domain-list-xt execute list-get-links   \ link0 d-link
    ." ("
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ link0 d-link domx
        domain-set-current-xt
        execute                     \ link0 d-link

        over link-get-data          \ link0 d-link reg0
        .region                     \ link0 d-link

        swap link-get-next      \ d-link link0
        swap link-get-next      \ link0 d-link
        dup if
            space
        then
    repeat
                                \ link0
    drop
    ." )"
;

\ Return true if TOS is a superset of its corresponding region in NOS.
: region-list-corr-superset ( lst1 lst0 -- bool )
    \ cr ." region-list-corr-superset: " dup .region-list-corr space ." sup " over .region-list-corr
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    over list-get-length
    over list-get-length
    session-get-number-domains-xt execute
    3<> abort" Lists have different length?"

    \ Init links for loop.
    swap list-get-links                     \ lst0 link1
    swap list-get-links                     \ link1 link0
    session-get-domain-list-xt execute
    list-get-links                          \ link1 link0 d-link

    begin
        ?dup
    while
                                    \ link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ link1 link0 d-link

        \ Compare regions.
        #2 pick link-get-data       \ link1 link0 d-link reg2
        #2 pick link-get-data       \ link1 link0 d-link reg2 reg1
        region-superset-of          \ link1 link0 d-link bool
        if
        else
            \ Non-superset found.
            3drop
            false
            \ space ." bool: " dup .bool cr
            exit
        then

        \ Prep for next cycle.
                                    \ link1 link0 d-link
        rot link-get-next           \ link0 d-link link1
        rot link-get-next           \ d-link link1 link0
        rot link-get-next           \ link1 link0 d-link
    repeat
                                    \ link1 link0
    2drop                           \
    true                           \ bool
    \ space ." bool: " dup .bool cr
;

\ Return true if TOS is a subset of its corresponding region in NOS.
: region-list-corr-subset ( lst1 lst0 -- bool )
    swap region-list-corr-superset
;

: region-list-corr-intersects ( lst1 lst0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    over list-get-length
    over list-get-length
    session-get-number-domains-xt execute
    3<> abort" Lists have different length?"

    \ Init links for loop.
    swap list-get-links                     \ lst0 link1
    swap list-get-links                     \ link1 link0
    session-get-domain-list-xt execute
    list-get-links                          \ link1 link0 d-link

    begin
        ?dup
    while
                                    \ link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ link1 link0 d-link

        \ Check regions
        #2 pick link-get-data       \ link1 link0 d-link reg1
        #2 pick link-get-data       \ link1 link0 d-link reg1 reg0
        region-intersects           \ link1 link0 d-link bool
        is-false if
            3drop
            false
            exit
        then

        \ Prep for next cycle.
                                    \ link1 link0 d-link
        rot link-get-next           \ link0 d-link link1
        rot link-get-next           \ d-link link1 link0
        rot link-get-next           \ link1 link0 d-link
    repeat
                                    \ link1 link0
    2drop                           \
    true
;

\   Return lst0 minus lst1, a list of region-list-corr.
: region-list-corr-subtract ( lst1 lst0 -- lst-of-region-list-corr t | f )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    over list-get-length
    over list-get-length
    session-get-number-domains-xt execute
    3<> abort" Lists have different length?"

    \ Check for a superset subtrahend.
    2dup swap
    region-list-corr-superset           \ lst1 lst0 bool
    abort" Subtrahend is a superset?"

    \ Check that the two lists intersect.
    2dup region-list-corr-intersects    \ lst1 lst0 bool
    0= if
        2drop
        false
        exit
    then

    \ Save lst0
    tuck                                \ lst0 lst1 lst0

    \ Init return list, and counter.
    list-new -rot                   \ lst0 ret-lst lst1 lst0
    0 >r                            \ lst0 ret-lst lst1 lst0, r: \ ctr

    \ Init links for loop.
    swap list-get-links                     \ lst0 ret-lst lst0 link1
    swap list-get-links                     \ lst0 ret-lst link1 link0
    session-get-domain-list-xt execute
    list-get-links                          \ lst0 ret-lst link1 link0 d-link

    begin
        ?dup
    while
                                    \ lst0 ret-lst link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ lst0 ret-lst link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ lst0 ret-lst link1 link0 d-link

        \ Subtract two regions.
        #2 pick link-get-data       \ lst0 ret-lst link1 link0 d-link reg1
        #2 pick link-get-data       \ lst0 ret-lst link1 link0 d-link reg1 reg0

        \ Check for superset subtrahend.
        2dup swap                   \ lst0 ret-lst link1 link0 d-link reg1 reg0 reg0 reg1
        region-superset-of          \ lst0 ret-lst link1 link0 d-link reg1 reg0 bool
        if
            \ No action on superset subtrahend.
            \ But it is known that not all subtrahend regions are supersets,
            \ due to the earlier test.
            2drop                   \ lst0 ret-lst link1 link0 d-link
        else
            \ If the subtrahend is not a superset, it must intersect,
            \ due to the earlier test.

            \ cr dup .region space ." - " over .region
            region-subtract             \ lst0 ret-lst link1 link0 d-link reg-lst
            \ space ." = " dup .region-list cr
        
            dup list-get-length 0= abort" region subtraction failed?"

            \ Generate result region-list-corrs
            dup list-get-links          \ lst0 ret-lst link1 link0 d-link reg-lst link
            begin
                ?dup
            while
                dup link-get-data       \ lst0 ret-lst link1 link0 d-link reg-lst link | regx
                r@                      \ lst0 ret-lst link1 link0 d-link reg-lst link | regx ctr
                #8 pick                 \ lst0 ret-lst link1 link0 d-link reg-lst link | regx ctr lst0
                region-list-copy-except \ lst0 ret-lst link1 link0 d-link reg-lst link | reg-lst2
                dup struct-inc-use-count
                #6 pick                 \ lst0 ret-lst link1 link0 d-link reg-lst link | reg-lst2 ret-lst
                list-push               \ lst0 ret-lst link1 link0 d-link reg-lst link

                link-get-next
            repeat
                                        \ lst0 ret-lst link1 link0 d-link reg-lst
            region-list-deallocate      \ lst0 ret-lst link1 link0 d-link
        then

        \ Prep for next cycle.
        r> 1+ >r                    \ lst0 ret-lst link1 link0 d-link, r: \ ctr+
                                    \ lst0 ret-lst link1 link0 d-link
        rot link-get-next           \ lst0 ret-lst link0 d-link link1
        rot link-get-next           \ lst0 ret-lst d-link link1 link0
        rot link-get-next           \ lst0 ret-lst link1 link0 d-link
    repeat

    \ Clean up.
    r> drop                         \ lst0 ret-lst link1 link0, r: \
                                    \ lst0 ret-lst link1 link0
    2drop                           \ lst0 ret-lst
    nip                             \ ret-lst
    true
;

\ Return a region-list-corr from a string.
: region-list-corr-from-string ( addr n -- rlcorr t | f )
    \ Get tokens.
    parse-string                \ addr0 cnt0 cnt2

    \ Check number tokens.
    session-get-number-domains-xt execute   \ addr0 cnt0 cnt2 domain-count
    <> if                       \ addr0 cnt0
        0 do
            2drop
        loop
        false
        exit
    then

    \ Process each region, skip invalid regions.
                                            \ addr0 cnt0
    list-new                                \ addr0 cnt0 ret-lst
    session-get-domain-list-xt execute
    list-get-links                          \ addr0 cnt0 ret-lst d-link
    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ addr0 cnt0 ret-lst d-link domx
        domain-set-current-xt
        execute                     \ addr0 cnt0 ret-lst d-link

        \ Get one region.
        2swap                       \ ret-lst d-link addr0 cnt0
        region-from-string          \ ret-lst d-link, regx t | f
        if
            #2 pick                 \ ret-lst d-link regx ret-lst
            region-list-push-end    \ ret-lst d-link
        then

        link-get-next
    repeat

    \ Check results.                \ ret-lst
    dup list-get-length             \ ret-lst len
    session-get-number-domains-xt execute   \ ret-lst len dnum
    <> if
        region-list-deallocate
        false
        exit
    then
                                    \ ret-lst
    true
;

\ Return a region-list-corr from a string, or abort.
: region-list-corr-from-string-a ( addr n -- rlcorr )
    region-list-corr-from-string    \ rlcorr t | f
    is-false abort" region-list-corr-from-string failed?"
;

\ Return true if two region-list-corrs are equal.
: region-list-corr-eq ( lst1 lst0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    over list-get-length
    over list-get-length
    session-get-number-domains-xt execute
    3<> abort" Lists have different length?"

    swap list-get-links     \ lst0 link1
    swap list-get-links     \ link1 link0

    begin
        ?dup
    while
        over link-get-data  \ link1 link0 reg1
        over link-get-data  \ link1 link0 reg1 reg0
        region-eq           \ link1 link0 bool
        is-false if
            2drop
            false
            exit
        then

        swap link-get-next
        swap link-get-next
    repeat
                            \ link1
    drop
    true
;

\ Return a list of maximum sized regions.
: region-list-corr-max-regions ( -- lst )
    \ Init return list.
    list-new                                \ ret-lst
    \ Prep for loop.
    session-get-domain-list-xt execute
    list-get-links                          \ ret-lst d-link

    begin
        ?dup
    while
        \ Set current domain.
        dup link-get-data           \ lst0 ret-lst link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ lst0 ret-lst link1 link0 d-link

        \ Add next region.
        dup link-get-data       \ ret-lst d-lisk domx
        domain-get-max-region-xt
        execute                 \ ret-lst d-lisk regx
        #2 pick                 \ ret-lst d-lisk regx ret-lst
        region-list-push-end    \ ret-lst d-lisk

        link-get-next           \ ret-lst d-link
    repeat
                                \ ret-lst
;

\ Return the complement of a region-list-corr, a list of region-list-corr.
: region-list-corr-complement ( lst0 -- lst )
    \ Check arg.
    assert-tos-is-list

    region-list-corr-max-regions    \ lst0 lst-max
    tuck                            \ lst-max lst0 lst-max
    region-list-corr-subtract       \ lst-max, lst t | f
    is-false abort" subtract failed?"

    swap                            \ lst lst-max
    region-list-deallocate          \ lst
;

\ Return the numbr of bits different between two region-list-corr.
: region-list-corr-distance ( lst1 lst0 -- nb )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    over list-get-length
    over list-get-length
    session-get-number-domains-xt execute
    3<> abort" Lists have different length?"

    \ Init counter.
    0 -rot                  \ cnt lst1 lst0

    \ Prep for loop.
    swap list-get-links     \ cnt lst0 link1
    swap list-get-links     \ cnt link1 link0

    begin
        ?dup
    while
        \ Add one region pair distance.
        rot                     \ link1 link0 cnt
        #2 pick link-get-data   \ link1 link0 cnt reg1
        #2 pick link-get-data   \ link1 link0 cnt reg1 reg0
        region-distance         \ link1 link0 cnt dist
        +                       \ link1 link0 cnt
        -rot                    \ cnt link1 link0

        \ Point to next pair.
        swap link-get-next
        swap link-get-next
    repeat
                                \ cnt link1
    drop                        \ cnt
;

\ Return true if two region-list-corr are adjacent.
: region-list-corr-adjacent ( lst1 lst2 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    over list-get-length
    over list-get-length
    session-get-number-domains-xt execute
    3<> abort" Lists have different length?"

    region-list-corr-distance   \ nb
    1 =
;

: region-list-corr-intersection ( lst1 lst0 -- rlc t | f )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-list
    over list-get-length
    over list-get-length
    session-get-number-domains-xt execute
    3<> abort" Lists have different length?"

    \ Init return list.
    list-new -rot                   \ ret-lst lst1 lst0

    \ Init links for loop.
    swap list-get-links                     \ ret-lst  lst0 link1
    swap list-get-links                     \ ret-lst  link1 link0
    session-get-domain-list-xt execute
    list-get-links                          \ ret-lst  link1 link0 d-link

    begin
        ?dup
    while
                                    \ ret-lst  link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ ret-lst  link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ ret-lst  link1 link0 d-link

        \ Check regions
        #2 pick link-get-data       \ ret-lst  link1 link0 d-link reg1
        #2 pick link-get-data       \ ret-lst  link1 link0 d-link reg1 reg0
        region-intersection         \ ret-lst  link1 link0 d-link, reg-int t | f
        if                          \ ret-lst  link1 link0 d-link reg-int
            #4 pick                 \ ret-lst  link1 link0 d-link reg-int ret-lst
            region-list-push-end    \ ret-lst  link1 link0 d-link
        else
            3drop                   \ ret-lst
            region-list-deallocate  \
            false                   \ bool
            exit
        then

        \ Prep for next cycle.
                                    \ ret-lst link1 link0 d-link
        rot link-get-next           \ ret-lst  link0 d-link link1
        rot link-get-next           \ ret-lst  d-link link1 link0
        rot link-get-next           \ ret-lst  link1 link0 d-link
    repeat
                                    \ ret-lst link1 link0
    2drop                           \ ret-lst
    true
;
