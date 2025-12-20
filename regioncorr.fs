\ Implement a struct and functions for a region list corresponding to domains.

#47317 constant regioncorr-id
    #2 constant regioncorr-struct-number-cells

\ Struct fields
0                                   constant regioncorr-header-disp \ 16-bits [0] struct id [1] use count.
regioncorr-header-disp    cell+     constant regioncorr-list-disp   \ Region list corresponding, in bits used, to the session domain list.


0 value regioncorr-mma \ Storage for region mma instance.

\ Init region mma, return the addr of allocated memory.
: regioncorr-mma-init ( num-items -- ) \ sets regioncorr-mma.
    dup 1 < 
    abort" regioncorr-mma-init: Invalid number of items."

    cr ." Initializing RegionCorr store."
    regioncorr-struct-number-cells swap mma-new to regioncorr-mma
;

\ Check region mma usage.
: assert-regioncorr-mma-none-in-use ( -- )
    regioncorr-mma mma-in-use 0<> 
    abort" regioncorr-mma use GT 0"
;

\ Check instance type.
: is-allocated-regioncorr ( addr -- flag )
    \ Insure the given addr cannot be an invalid addr.
    dup regioncorr-mma mma-within-array 0=
    if
        drop false exit
    then

    struct-get-id   \ Here the fetch could abort on an invalid address, like a random number.
    regioncorr-id =
;

\ Check TOS for regioncorr, unconventional, leaves stack unchanged.
: assert-tos-is-regioncorr ( arg0 -- arg0 )
    dup is-allocated-regioncorr
    is-false if
        s" TOS is not an allocated regioncorr"
        .abort-xt execute
    then
;

\ Check NOS for regioncorr, unconventional, leaves stack unchanged.
: assert-nos-is-regioncorr ( arg1 arg0 -- arg1 arg0 )
    over is-allocated-regioncorr
    is-false if
        s" NOS is not an allocated regioncorr"
        .abort-xt execute
    then
;

\ Check 3OS for regioncorr, unconventional, leaves stack unchanged.
: assert-3os-is-regioncorr ( arg2 arg1 arg0 -- arg1 arg0 )
    #2 pick is-allocated-regioncorr
    is-false if
        s" 3OS is not an allocated regioncorr"
        .abort-xt execute
    then
;

\ Start accessors.

\ Return the first field from a region instance.
: regioncorr-get-list ( regc0 -- lst )
    \ Check arg.
    assert-tos-is-regioncorr

    regioncorr-list-disp +    \ Add offset.
    @                         \ Fetch the field.
;

\ Set the first field from a region instance, use only in this file.
: _regioncorr-set-list ( lst1 regc0 -- )
    \ Check args.
    assert-tos-is-regioncorr

    \ Store list
    over struct-inc-use-count

    regioncorr-list-disp +    \ Add offset.
    !                         \ Set first field.
;

\ End accessors.

\ Create a regioncorr-list-corr from a regioncorr-list-corr-list on the stack.
: regioncorr-new ( lst0 -- addr)
    \ check arg.
    assert-tos-is-list
    assert-tos-is-region-list

    \ Allocate space.
    regioncorr-mma mma-allocate   \ lst0 rlc

    \ Store id.
    regioncorr-id over            \ lst0 rlc id rlc
    struct-set-id                 \ lst0 rlc

    \ Init use count.
    0 over struct-set-use-count   \ lst0 rlc

    tuck                          \ rlc lst0 rlc
    _regioncorr-set-list          \ rlc
;

\ Print a region-list corresponding to the session domain list.
: .regioncorr ( regc0 -- )
    \ Check arg.
    assert-tos-is-regioncorr

    regioncorr-get-list       \ lst
    list-get-links                  \ link0
    session-get-domain-list-xt      \ link0 xt
    execute                         \ link0 dom-lst 
    list-get-links                  \ link0 d-link
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

        swap link-get-next          \ d-link link0
        swap link-get-next          \ link0 d-link
        dup if
            space
        then
    repeat
                                    \ link0
    drop
    ." )"
;

\ Deallocate the given rlc, if its use count is 1 or 0.
: regioncorr-deallocate ( regc0 -- )
    \ Check arg.
    assert-tos-is-regioncorr

    dup struct-get-use-count            \ regc0 count

    #2 <
    if
        \ Deallocate fields.
        dup regioncorr-get-list   \ regc0 rlc-lst
        region-list-deallocate

        \ Deallocate instance.
        regioncorr-mma mma-deallocate
    else
        struct-dec-use-count
    then
;

\ Return true if TOS is a superset of its corresponding region in NOS.
: regioncorr-superset ( regc1 regc0 -- bool )
    \ cr ." regioncorr-superset: " dup .regioncorr space ." sup " over .regioncorr
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    \ Init links for loop.
    regioncorr-get-list list-get-links swap \ link0 regc1
    regioncorr-get-list list-get-links swap \ link1 link0
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
    true                            \ bool
    \ space ." bool: " dup .bool cr
;

\ Return true if TOS is a superset of its corresponding region in NOS.
: regioncorr-superset-states ( regc1 regc0 -- bool )
    \ cr ." regioncorr-superset: " dup .regioncorr space ." sup " over .regioncorr
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ link0 regc1
    regioncorr-get-list list-get-links swap   \ link1 link0
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
        #2 pick link-get-data       \ link1 link0 d-link sta2
        #2 pick link-get-data       \ link1 link0 d-link sta2 reg1
        region-superset-of-state    \ link1 link0 d-link bool
        if
        else
            \ Non-superset found.
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
    true                            \ bool
;

\ Return true if TOS is a subset of its corresponding region in NOS.
: regioncorr-subset ( regc1 regc0 -- bool )
    swap regioncorr-superset
;

: regioncorr-intersects ( regc1 regc0 -- bool )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ link0 regc1
    regioncorr-get-list list-get-links swap   \ link1 link0
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

\ Return a new regioncorr, with one item replaced.
: regioncorr-copy-except ( reg2 cnt1 rc0 -- rc )
    \ Check args.
    assert-tos-is-regioncorr
    assert-3os-is-region

    regioncorr-get-list         \ reg2 cnt1 reg-lst
    list-copy-except-struct     \ reg-lst2
    regioncorr-new              \ rc
;

\   Return regc0 minus regc1, a list of regioncorr.
: regioncorr-subtract ( regc1 regc0 -- list-of-rlc t | f )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    \ Check for a superset subtrahend.
    2dup swap
    regioncorr-superset           \ regc1 regc0 bool
    abort" Subtrahend is a superset?"

    \ Check that the two lists intersect.
    2dup regioncorr-intersects    \ regc1 regc0 bool
    0= if
        2drop
        false
        exit
    then

    \ Save regc0
    tuck                                \ regc0 regc1 regc0

    \ Init return list, and counter.
    list-new -rot                   \ regc0 ret-lst regc1 regc0
    0 >r                            \ regc0 ret-lst regc1 regc0, r: \ ctr

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ regc0 ret-lst link0 regc1
    regioncorr-get-list list-get-links swap   \ regc0 ret-lst link1 link0
    session-get-domain-list-xt execute
    list-get-links                          \ regc0 ret-lst link1 link0 d-link

    begin
        ?dup
    while
                                    \ regc0 ret-lst link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ regc0 ret-lst link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ regc0 ret-lst link1 link0 d-link

        \ Subtract two regioncorrs.
        #2 pick link-get-data       \ regc0 ret-lst link1 link0 d-link reg1
        #2 pick link-get-data       \ regc0 ret-lst link1 link0 d-link reg1 reg0

        \ Check for superset subtrahend.
        2dup swap                   \ regc0 ret-lst link1 link0 d-link reg1 reg0 reg0 reg1
        region-superset-of          \ regc0 ret-lst link1 link0 d-link reg1 reg0 bool
        if
            \ No action on superset subtrahend.
            \ But it is known that not all subtrahend regions are supersets,
            \ due to the earlier test.
            2drop                   \ regc0 ret-lst link1 link0 d-link
        else
            \ If the subtrahend is not a superset, it must intersect,
            \ due to the earlier test.

            \ cr dup .region space ." - " over .region
            region-subtract             \ regc0 ret-lst link1 link0 d-link reg-lst
            \ space ." = " dup .region-list cr

            dup list-get-length 0= abort" region subtraction failed?"

            \ Generate result regioncorrs
            dup list-get-links          \ regc0 ret-lst link1 link0 d-link reg-lst link
            begin
                ?dup
            while
                dup link-get-data       \ regc0 ret-lst link1 link0 d-link reg-lst link | regx
                r@                      \ regc0 ret-lst link1 link0 d-link reg-lst link | regx ctr
                #8 pick                 \ regc0 ret-lst link1 link0 d-link reg-lst link | regx ctr regc0
                regioncorr-copy-except  \ regc0 ret-lst link1 link0 d-link reg-lst link | reg-lst2
                #6 pick                 \ regc0 ret-lst link1 link0 d-link reg-lst link | reg-lst2 ret-lst
                list-push-struct        \ regc0 ret-lst link1 link0 d-link reg-lst link

                link-get-next
            repeat
                                                    \ regc0 ret-lst link1 link0 d-link reg-lst
            region-list-deallocate   \ regc0 ret-lst link1 link0 d-link
        then

        \ Prep for next cycle.
        r> 1+ >r                    \ regc0 ret-lst link1 link0 d-link, r: \ ctr+
                                    \ regc0 ret-lst link1 link0 d-link
        rot link-get-next           \ regc0 ret-lst link0 d-link link1
        rot link-get-next           \ regc0 ret-lst d-link link1 link0
        rot link-get-next           \ regc0 ret-lst link1 link0 d-link
    repeat

    \ Clean up.
    r> drop                         \ regc0 ret-lst link1 link0, r: \
                                    \ regc0 ret-lst link1 link0
    2drop                           \ regc0 ret-lst
    nip                             \ ret-lst
    true
;

\ Return a regioncorr from a parsed string.
: regioncorr-from-parsed-string ( addr n -- rlc t | f )

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
    regioncorr-new              \ regcorr
    true
;

\ Return a regioncorr from a string.
: regioncorr-from-string ( addr n -- rlc t | f )
    \ Get tokens.
    parse-string                \ addr0 cnt0 cnt2

    regioncorr-from-parsed-string
;

\ Return a regioncorr from a string, or abort.
: regioncorr-from-string-a ( addr n -- rlc )
    regioncorr-from-string    \ rlcorr t | f
    is-false abort" regioncorr-from-string failed?"
;

\ Return true if two regioncorrs are equal.
: regioncorr-eq ( regc1 regc0 -- bool )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    regioncorr-get-list list-get-links swap   \ link0 regc1
    regioncorr-get-list list-get-links swap   \ link1 link0

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

\ Return the complement of a regioncorr, a list of regioncorr.
: regioncorr-complement ( regc0 -- lst )
    \ Check arg.
    assert-tos-is-regioncorr

    current-session                     \ regc0 sess
    session-calc-max-regions-xt execute \ regc0 lst-max
    tuck                                \ lst-max regc0 lst-max
    regioncorr-subtract                 \ lst-max, lst t | f
    is-false abort" subtract failed?"

    swap                                \ lst lst-max
    regioncorr-deallocate               \ lst
;

\ Return the numbr of bits different between two regioncorr.
: regioncorr-distance ( regc1 regc0 -- nb )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    \ Init counter.
    0 -rot                  \ cnt regc1 regc0

    \ Prep for loop.
    regioncorr-get-list list-get-links swap   \ cnt link0 regc1
    regioncorr-get-list list-get-links swap   \ cnt link1 link0

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

\ Return true if two regioncorr are adjacent.
: regioncorr-adjacent ( regc1 lst2 -- bool )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    regioncorr-distance   \ nb
    1 =
;

: regioncorr-intersection ( regc1 regc0 -- rlc t | f )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-regioncorr

    \ Init return list.
    list-new -rot                   \ ret-lst regc1 regc0

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ ret-lst link0 regc1
    regioncorr-get-list list-get-links swap   \ ret-lst link1 link0
    session-get-domain-list-xt execute
    list-get-links                          \ ret-lst link1 link0 d-link

    begin
        ?dup
    while
                                    \ ret-lst link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ ret-lst link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ ret-lst link1 link0 d-link

        \ Check regions
        #2 pick link-get-data       \ ret-lst link1 link0 d-link reg1
        #2 pick link-get-data       \ ret-lst link1 link0 d-link reg1 reg0
        region-intersection         \ ret-lst link1 link0 d-link, reg-int t | f
        if                          \ ret-lst link1 link0 d-link reg-int
            #4 pick                 \ ret-lst link1 link0 d-link reg-int ret-lst
            region-list-push-end    \ ret-lst link1 link0 d-link
        else
            3drop                   \ ret-lst
            region-list-deallocate  \
            false                   \ bool
            exit
        then

        \ Prep for next cycle.
                                    \ ret-lst link1 link0 d-link
        rot link-get-next           \ ret-lst link0 d-link link1
        rot link-get-next           \ ret-lst d-link link1 link0
        rot link-get-next           \ ret-lst link1 link0 d-link
    repeat
                                    \ ret-lst link1 link0
    2drop                           \ ret-lst
    regioncorr-new
    true
;

: regioncorr-translate-states ( sta-regc1 regc0 -- slc )
    \ Check args.
    assert-tos-is-regioncorr
    assert-nos-is-list
    over list-get-length
    session-get-number-domains-xt execute
    3<> abort" Lists have different length?"

    \ Init return list.
    list-new -rot                   \ ret-lst sta-regc1 reg-regc1

    \ Init links for loop.
    regioncorr-get-list list-get-links swap   \ ret-lst link0 sta-regc1
    list-get-links swap                             \ ret-lst link1 link0
    swap list-get-links                             \ ret-lst link1 link0
    session-get-domain-list-xt
    execute                         \ ret-lst link1 link0 dom-lst
    list-get-links                  \ ret-lst link1 link0 d-link

    begin
        ?dup
    while
                                    \ ret-lst  link1 link0 d-link

        \ Set current domain.
        dup link-get-data           \ ret-lst  link1 link0 d-link domx
        domain-set-current-xt
        execute                     \ ret-lst  link1 link0 d-link

        \ Check state/region
        #2 pick link-get-data       \ ret-lst  link1 link0 d-link sta1
        #2 pick link-get-data       \ ret-lst  link1 link0 d-link sta1 reg0

        region-translate-state      \ ret-lst  link1 link0 d-link sta0

        #4 pick                     \ ret-lst  link1 link0 d-link sta0 ret-lst
        list-push-end               \ ret-lst  link1 link0 d-link

        \ Prep for next cycle.
                                    \ ret-lst link1 link0 d-link
        link-get-next rot           \ ret-lst  link0 d-link+ link1
        link-get-next rot           \ ret-lst  d-link+ link1+ link0
        link-get-next rot           \ ret-lst  link1+ link0+ d-link+
    repeat
                                    \ ret-lst link1 link0
    2drop                           \ ret-lst
    regioncorr-new
;
