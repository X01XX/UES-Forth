\ Functions for square lists.

\ Deallocate a square list.
: square-list-deallocate ( list0 -- )
    [ ' square-deallocate ] literal over list-apply \ Deallocate square instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

\ Return the intersection of two square lists.
: square-list-set-intersection ( list1 list0 -- list-result )
    [ ' square-eq ] literal -rot        \ xt list1 list0
    list-intersection                   \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the union of two square lists.
: square-list-set-union ( list1 list0 -- list-result )
    [ ' square-eq ] literal -rot        \ xt list1 list0
    list-union                          \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Return the difference of two square lists.
: square-list-set-difference ( list1 list0 -- list-result )
    [ ' square-eq ] literal -rot        \ xt list1 list0
    list-difference                     \ list-result
    [ ' struct-inc-use-count ] literal  \ list-result xt
    over list-apply                     \ list-result
;

\ Print a square-list
: .square-list ( list0 -- )
    \ Check args.
    assert-tos-is-list

    [ ' .square ] literal swap .list
;

\ Print a list of square states.
: .square-list-states ( sqrlst0 -- )
    \ Check args.
    assert-tos-is-list

    ." ("
    [ ' .square-state ] literal swap list-apply
    ." )"
;

\ Push a square to a square-list, unless it is already in the list.
: square-list-push-nodups ( sqr1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-square

    2dup
    [ ' square-eq ] literal -rot
    list-member
    if
        2drop
    else
        over struct-inc-use-count
        list-push
    then
;

\ Push a square to a square-list.
: square-list-push ( sqr1 list0 -- )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-square

    over struct-inc-use-count
    list-push
;

\ Remove the first square, idetified by xt, from a square-list, and deallocate.
\ xt signature is ( item list-data -- flag )
\ Return true if an square was removed.
: square-list-remove ( sta1 list0 -- bool )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-value

    [ ' square-state-eq ] literal   \ sta1 lst0 xt
    -rot                            \ xt sta1 lst0

    list-remove                     \ sqr true | false
    if
        square-deallocate
        true
    else
        false
    then
;


\ Return squares in a given region.
: square-list-in-region ( reg1 list0 -- sqr-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    [ ' square-state-in-region ] literal -rot       \ xt reg1 list0
    list-find-all                                   \ ret-list
    [ ' struct-inc-use-count ] literal over         \ ret-list xt ret-list
    list-apply                                      \ ret-list
;

\ Return square states in a region.
: square-list-states-in-region ( reg1 sqr-lst0 -- ret-sta-lst )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-region

    \ Init return list.
    list-new -rot                   \ ret-lst reg1 sqr-lst0
    list-get-links                  \ ret-lst reg1 link
    begin
        ?dup
    while
        dup link-get-data           \ ret-lst reg1 link sqrx
        square-get-state            \ ret-lst reg1 link stax
        2 pick                      \ ret-lst reg1 link stax reg1
        region-superset-of-state    \ ret-lst reg1 link flag
        if
            \ Add state to return list.
            dup link-get-data       \ ret-lst reg1 link sqrxgrep 
            square-get-state        \ ret-lst reg1 link stax
            3 pick                  \ ret-lst reg1 link stax ret-lst
            list-push               \ ret-lst reg1 link
        then

        link-get-next
    repeat
                                    \ ret-lst reg1
    drop                            \ ret-lst
;

\ Find a square in a list, by state, if any.
: square-list-find ( val1 list0 -- sqr true | false )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-value

    [ ' square-state-eq ] literal -rot list-find
;

\ Return true if a square with a given state is a member.
: square-list-member ( val1 list0 -- flag )
    \ Check args.
    assert-tos-is-list
    assert-nos-is-value

    [ ' square-state-eq ] literal -rot list-member
;

\ Return the highest pn value in a non-empty list of squares.
: square-list-highest-pn ( list0 -- pn )
    \ Check arg.
    assert-tos-is-list
    dup list-is-empty
    abort" List is empty?"

    \ Prep for loop.
    list-get-links              \ link
    1 swap                      \ max-pn link

    \ Scan square list.
    begin
        ?dup
    while
        \ Check if current square pn in greater than the current max pn.
        dup link-get-data       \ max-pn link square
        square-get-pn           \ max-pn link sqr-pn
        3 pick                  \ max-pn link sqr-pn max-pn
        <                       \ max-pn link flag
        if                      \ max-pn link
            \ Set the square pn to be the max pn.
            nip                 \ link
            dup link-get-data   \ link square
            square-get-pn       \ link sqr-pn
            swap                \ max-pn link
        then

        link-get-next           \ max-pn link
    repeat
                            \ max-pn
;

\ Return a region built from squares of the highest pn value, in a non-empty list.
: square-list-region ( sqr-lst0 -- reg )
    \ Check arg.
    assert-tos-is-list
    dup list-is-empty
    abort" List is empty?"

    \ Get highest pn value
    dup square-list-highest-pn swap \ pn sqr-lst

    \ Prep for loop.
    list-get-links                  \ pn link
    0 swap                          \ pn reg link

    \ Scan square list.
    begin
        ?dup
    while
        \ Check if square pn is equal to the max pn of the list.
        dup link-get-data               \ pn reg link sqr
        dup square-get-pn               \ pn reg link sqr s-pn
        4 pick                          \ pn reg link sqr s-pn max-pn
        <> if
            drop                        \ pn reg link
        else
            square-get-state            \ pn reg link sta
            rot                         \ pn link sta reg
            dup 0=
            if
                                        \ pn link sta 0
                drop                        \ pn link sta
                dup                         \ pn link sta sta
                region-new                  \ pn link reg
                swap                        \ pn reg link
            else
                                            \ pn link sta reg
                2dup                        \ pn link sta reg sta reg
                region-superset-of-state    \ pn link sta reg flag
                if
                    nip swap                \ pn reg link
                else
                    \ Add state to expand return region.
                    tuck                    \ pn link reg sta reg
                    region-union-state      \ pn link reg reg2
                    swap region-deallocate  \ pn link reg2
                    swap                    \ pn reg2 link
                then
            then
        then
        
        link-get-next           \ pn reg link
    repeat
                                \ pn reg
    nip                         \ reg
;

\ Return rules for a non-empty square-list, having no incompatible squares.
: square-list-get-rules ( list0 -- rulestore true | false )
    \ Check arg.
    assert-tos-is-list
    dup list-is-empty
    abort" List is empty?"

    dup square-list-highest-pn      \ list0 max-pn

    \ Check for 3/U
    dup 3 =
    if
        2drop                       \
        rulestore-new-0             \ rul-str
        true
        exit
    then

    swap                            \ max-pn list0
    \ Init return rulestore.
    0 -rot                          \ rul-str max-pn list0

    \ Prep for loop
    list-get-links                  \ rul-str max-pn link

    begin
        ?dup
    while
        \ Check if the current square pn is equal to the max-pn.
        dup link-get-data           \ rul-str max-pn link sqr
        square-get-pn               \ rul-str max-pn link sqr-pn
        2 pick                      \ rul-str max-pn link sqr-pn max-pn
        =                           \ rul-str max-pn link flag
        
        if                          \ rul-str max-pn link
            \ Update the return rulestore.
            rot                             \ max-pn link rul-str

            over link-get-data              \ max-pn link rul-str sqr
            square-get-rules                \ max-pn link rul-str sqr-ruls
            over                            \ max-pn link rul-str sqr-ruls rul-str
            if                              \ max-pn link rul-str sqr-ruls
                over                        \ max-pn link rul-str sqr-ruls rul-str
                rulestore-union             \ max-pn link rul-str, new-rules true | false
                if                          \ max-pn link rul-str new-rules
                    swap                    \ max-pn link new-rules rul-str
                    rulestore-deallocate    \ max-pn link new-rules
                    -rot                    \ rul-str max-pn link
                else                        \ max-pn link rul-str
                    rulestore-deallocate    \ max-pn link
                    2drop
                    false
                    exit
                then
            else                            \ max-pn link rul-str sqr-ruls
                \ Square rules are the first rules
                \ Init the return rulestore
                nip                         \ max-pn link sqr-ruls
                rulestore-copy              \ max-pn link sqr-ruls (since it may be deallocated later)
                -rot                        \ rul-str max-pn link
            then                            \ rul-str max-pn link
        then

        link-get-next
    repeat
                                \ rul-str max-pn
    drop                        \ rul-str
    true
;
