\ Tests for the region-list struct functions.

: region-list-test-region-intersections-n

    s" (X10X)" region-list-from-string-a        \ lst1

    s" (1XX1 0XX1)" region-list-from-string-a   \ lst1

    2dup region-list-intersections-n            \ lst1 lst2 lst3

    s" (X101)" region-list-from-string-a        \ lst1 lst2 lst3 lst4
    2dup region-list-eq                         \ lst1 lst2 lst3 lst4 bool
    0= abort" X101 not found"

    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-test-region-intersections-n: Ok"
;

: region-list-test-subtract-n

    \ Make subtrahend list.
    s" (1XX1 0XX1)" region-list-from-string-a   \ lst1 

    \ Make minuend list.
    s" (0000 X10X)" region-list-from-string-a   \ lst1 lst2

    \ Subtract regions.
    2dup region-list-subtract-n                 \ lst1 lst2 lst3

    \ Check results.
    s" (X100 0X00)" region-list-from-string-a   \ lst1 lst2 lst3 lst4

    2dup region-list-eq                         \ lst1 lst2 lst3 lst4 bool
    is-false abort" region-list-test-subtract-n: region-lists invaid?"

    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-subtract-n: Ok"
;

: region-list-test-states
    \ Make region-list list, using regions made with duplicate states.
    list-new                                \ lst1
    #4  #7 region-new over region-list-push \ lst1
    #4 #13 region-new over region-list-push \ lst1
    #7 #13 region-new over region-list-push \ lst1

    dup region-list-states                  \ lst1 lst2

    dup list-get-length
    #3 <>
    abort" List length not 3?"

    [ ' = ] literal #4 #2 pick list-member
    0= abort" 4 not in list?"

    [ ' = ] literal #7 #2 pick list-member
    0= abort" 7 not in list?"

    [ ' = ] literal #13 #2 pick list-member
    0= abort" 13 not in list?"

    list-deallocate
    region-list-deallocate

    cr ." region-list-test-states: Ok"
;

: region-list-test-state-in-one-region

    \ Make a region-list.
    s" (X1X1 X10X 01XX)" region-list-from-string-a  \ lst1

    #2 over                                 \ lst1 2 lst1
    region-list-state-in-one-region         \ lst1 flag
    abort" 2 in one region?"

    #4 over                                 \ lst1 4 lst1
    region-list-state-in-one-region         \ lst1 | flag
    abort" 4 in one region?"

    #6 over                                 \ lst1 6 lst1
    region-list-state-in-one-region         \ lst1 flag
    0= abort" 6 not in one region?"

    #7 over                                 \ lst1 7 lst1
    region-list-state-in-one-region         \ lst1 | flag
    abort" 7 in one region?"

    #12 over                                \ lst1 12 lst1
    region-list-state-in-one-region         \ lst1 flag
    0= abort" 12 not in one region?"

    #13 over                                \ lst1 13 lst1
    region-list-state-in-one-region         \ lst1 flag
    abort" 13 in one region?"

    #15 over                                \ lst1 15 lst1
    region-list-state-in-one-region         \ lst1 | flag
    0= abort" 15 not in one region?"

    region-list-deallocate

    cr ." region-list-test-state-in-one-region: Ok"
;

: region-list-test-states-in-one-region

    \ Make state list.
    list-new                                \ sta-lst
     #2 over list-push
     #4 over list-push
     #6 over list-push
     #7 over list-push
    #12 over list-push
    #13 over list-push
    #15 over list-push

    \ Make a region-list.
    s" (X1X1 X10X 01XX)" region-list-from-string-a  \ sta-lst rge-lst

    2dup region-list-states-in-one-region   \ sta-lst reg-lst sta-lst2

    \ cr ." states in one region: " dup .list-raw cr

    dup list-get-length
    #3 <>
    abort" List length not 3?"

    [ ' = ] literal #6 #2 pick list-member
    0= abort" 6 not in list?"

    [ ' = ] literal #12 #2 pick list-member
    0= abort" 12 not in list?"

    [ ' = ] literal #15 #2 pick list-member
    0= abort" 15 not in list?"

    list-deallocate
    region-list-deallocate
    list-deallocate

    cr ." region-list-test-states-in-one-region: Ok"
;

: region-list-test-intersection-fragments
    \ Make a region-list.
    s" (XX1X 1XXX X1X1 X1X1)" region-list-from-string-a \ lst1

    dup                                     \ lst1 lst1
    region-list-intersection-fragments      \ lst1 lst2

    \ Check.
    s" (1111 101x 1x10 0111 1101 0101 100x 1x00 0x10 001x)" region-list-from-string-a   \ lst1 lst2 lst3
    2dup region-list-eq                     \ lst1 lst2 lst3 bool
    is-false abort" region-list-test-intersection-fragments: 1 region lists ne?"
   
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    \ Test with subset.

    \ Make a region-list.
    s" (011X XX1X)" region-list-from-string-a   \ lst1

    \ cr ." lst1: " dup .region-list cr

    dup                                     \ lst1 lst1
    region-list-intersection-fragments      \ lst1 lst2

    \ cr ." fragments 2: " dup .region-list     \ lst1 lst2

    \ Check length.
    dup list-get-length                     \ lst1 lst2 len
    #3 <> abort" Result length not 3?"

    \ Check list.
    s" (011X 1X1X X01X)" region-list-from-string-a  \ lst1 lst2 lst3

    2dup region-list-eq                             \ lst1 lst2 lst3
    is-false abort" region-list-test-intersection-fragments: 2 region lists ne?"

    \ Clean up.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-intersection-fragments: Ok"
;

: region-list-test-normalize
    s" (X101 X100 0111 00X1)" region-list-from-string-a \ lst1

    dup region-list-normalize               \ lst1 lst2
    \ cr ." rslt: " dup .region-list cr

    \ Check results.
    s" (X10X 0XX1)" region-list-from-string-a   \ lst1 lst2 lst3
    2dup region-list-eq                         \ lst1 lst2 lst3 bool
    is-false abort" region-list-test-normalize: 1 region lists ne?"

    \ Clean up.
    region-list-deallocate                  \ lst1 lst2
    region-list-deallocate                  \ lst1
    region-list-deallocate                  \

    cr ." region-list-test-normalize: Ok"
;

: region-list-test-copy-except
    s" (X100 0111 00X1)" region-list-from-string-a \ lst1

    s" X111" region-from-string-a           \ lst1 regx
    1                                       \ lst1 regx 1
    #2 pick                                 \ lst1 regx lst1
    region-list-copy-except                 \ lst1 lst2

    \ Check results.
    s" (X100 X111 00X1)" region-list-from-string-a  \ lst1 lst2 lst3
    2dup region-list-eq                             \ lst1 lst2 lst3 bool
    is-false abort" region-list-test-copy-except: region lists ne?"

    \ Clean up.
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate

    cr ." region-list-test-copy-except: Ok"
;

: region-list-test-push-nosubs
    \ Test 1, pushing a region that is a subset of regions in a list.
    s" 10x1" region-from-string-a               \ reg'
    s" (xx01 10xx)" region-list-from-string-a   \ reg' reg-lst'
    2dup
    region-list-push-nosubs                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed?" cr
        abort
    else
        cr ." reg NOT pushed"
        dup list-get-length                     \ reg1` reg-lst' len
        #2 = if
        else
            cr ." list len ne 2?" cr
            abort
        then
    then

    region-list-deallocate
    region-deallocate

    \ Test 2, pushing a region that is not a subset of regions in a list.
    s" 0xx0" region-from-string-a               \ reg'
    s" (xx01 10xx)" region-list-from-string-a   \ reg' reg-lst'
    2dup
    region-list-push-nosubs                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed" cr
        cr dup .region-list cr
        dup list-get-length                     \ reg1` reg-lst' len
        #3 = if
        else
            cr ." list len ne 3?" cr
            abort
        then
    else
        cr ." reg NOT pushed?"
        abort
    then

    region-list-deallocate
    drop

    \ Test 3, pushing a region that is not a superset of a region in a list.
    s" 1xxx" region-from-string-a               \ reg'
    s" (xx01 10xx)" region-list-from-string-a   \ reg' reg-lst'
    2dup
    region-list-push-nosubs                     \ reg1` reg-lst' bool
    if
        cr ." reg pushed" cr
        cr dup .region-list cr
        dup list-get-length                     \ reg1` reg-lst' len
        #2 = if
        else
            cr ." list len ne 2?" cr
            abort
        then
    else
        cr ." reg NOT pushed?"
        abort
    then

    region-list-deallocate
    drop

    cr ." region-list-test-push-nosubs: Ok"
;

: region-list-test-unmask-defining-region ( dom -- )
    \ Init a list of incomptible pairs.
    \ The case of the X-bit allows controlling the states represented.
    \ X0X0 = (1010, 0000), X0x0 = (1000, 0010).

    \ List of incompatible state pairs (inc-), as regions.
    s" (00X1 X100)" region-list-from-string-a       \ inc-lst'

    cr cr ." Incompatible pair list: " dup .region-list cr

    \ Check if list is empty, production placeholder.
    dup list-is-empty
    if
        list-deallocate
        exit
    then

    \ Init logical structure (ls-) list.
    #2 pick                                         \ dom0 inc-lst' max-reg' dom
    domain-get-max-region-xt execute                \ dom0 inc-lst' max-reg'
    list-new                                        \ dom0 inc-lst' max-reg' ls-lst'
    tuck list-push-struct                           \ dom0 inc-lst' ls-lst'

    \ Process each incompatible pair in inc-lst.
    over list-get-links                             \ dom0 inc-lst' ls-lst' link
    begin
        ?dup
    while
        \ Make calculation using region states.
        dup link-get-data                           \ dom0 inc-lst' ls-lst' link regx
        region-get-states                           \ dom0 inc-lst' ls-lst' link s0 s1
        #5 pick                                     \ dom0 inc-lst' ls-lst' link s0 s1 dom0
        domain-state-pair-complement                \ dom0 inc-lst' ls-lst' link cmp-lst'

        \ Intersect calculation with results.
        dup                                         \ dom0 inc-lst' ls-lst' link cmp-lst' cmp-lst'
        #3 pick                                     \ dom0 inc-lst' ls-lst' link cmp-lst' cmp-lst' ls-lst'
        region-list-intersections-nosubs            \ dom0 inc-lst' ls-lst' link cmp-lst' ls-lst''

        \ Clean up.
        swap region-list-deallocate                 \ dom0 inc-lst' ls-lst' link ls-lst''
        rot                                         \ dom0 inc-lst' link ls-lst'' ls-lst'
        region-list-deallocate                      \ dom0 inc-lst' link ls-lst''
        swap                                        \ dom0 inc-lst' ls-lst'' link
        
        link-get-next
    repeat
                                                    \ dom0 inc-lst' ls-lst'

    \ Init defining region (df-) list.
    list-new                                        \ dom0 inc-lst' ls-lst' df-lst'

    cr ." Result"
    cr ." Region  Contains"
    cr ." ------  --------"
    over list-get-links                             \ dom0 inc-lst' ls-lst' df-lst' ls-link
    begin
        ?dup
    while
        dup link-get-data                           \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg
        cr dup .region #4 spaces

        \ Check each incompatible-pair state to see if its in a logical-structure region.
        #4 pick                                     \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-lst'
        list-get-links                              \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link
        begin
            ?dup
        while
            \ Get region states.
            dup link-get-data                       \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link inc-reg
            region-get-states                       \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link s0 s1

            \ Check and print states.
            dup                                     \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link s0 s1 s1
            #4 pick                                 \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link s0 s1 s1 ls-reg
            region-superset-of-state                \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link s0 s1 bool
            if
                space hex.
            else
                drop
            then
                                                    \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link s0
            dup                                     \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link s0 s0
            #3 pick                                 \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link s0 s0 ls-reg
            region-superset-of-state                \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg inc-link s0 bool
            if
                space hex.
            else
                drop
            then

            link-get-next
        repeat
                                                    \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg

        \ Check if the region is defining.
                                                    \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg
        #3 pick                                     \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg ls-lst'
        region-list-region-is-defining              \ dom0 inc-lst' ls-lst' df-lst' ls-link bool
        if
            space ." defining"
            dup link-get-data                       \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg
            #2 pick                                 \ dom0 inc-lst' ls-lst' df-lst' ls-link ls-reg df-lst'
            list-push-struct                        \ dom0 inc-lst' ls-lst' df-lst' ls-link
        then

        link-get-next
    repeat
                                                    \ dom0 inc-lst' ls-lst' df-lst'

    cr cr ." Defining regions: " dup .region-list cr

    \ Check for any left-over regions.
    #3 pick                                         \ dom0 inc-lst' ls-lst' df-lst' dom
    domain-get-max-region-xt execute                \ dom0 inc-lst' ls-lst' df-lst' max-reg'
    list-new                                        \ dom0 inc-lst' ls-lst' df-lst' max-reg' max-lst'
    tuck list-push-struct                           \ dom0 inc-lst' ls-lst' df-lst' max-lst'
    over                                            \ dom0 inc-lst' ls-lst' df-lst' max-lst' df-lst'
    over                                            \ dom0 inc-lst' ls-lst' df-lst' max-lst' df-lst' max-lst'
    region-list-subtract                            \ dom0 inc-lst' ls-lst' df-lst' max-lst' lft-lst'

    \ Clean up.
    swap region-list-deallocate                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst'

    cr ." Left-over regions: " dup .region-list cr

    \ Check if list is empty, production placeholder.
    dup list-is-empty
    if
        region-list-deallocate
        region-list-deallocate
        region-list-deallocate
        region-list-deallocate
        drop
        exit
    then

    \ Find non-defining regions.
                                                    \ dom0 inc-lst' ls-lst' df-lst' lft-lst'
    \ Init non-defining (ndf-) list.
    list-new                                        \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst'
    \ For each logical-structure region...
    #3 pick                                         \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-lst'
    list-get-links                                  \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-link
    begin
        ?dup
    while
        [ ' = ] literal                             \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-link xt
        over link-get-data                          \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-link xt ls-reg
        #5 pick                                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-link xt ls-reg df-lst'
        list-member                                 \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-link bool
        if
        else
            dup link-get-data                       \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-link ls-reg
            #2 pick                                 \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-link ls-reg ndf-lst'
            list-push-struct                        \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ls-link
        then

        link-get-next
    repeat

    \ Find non-defining regions (ndf-) that intersect left-over (lft- )regions.
                                                    \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst'
    cr ." non defining regions: " dup .region-list cr

    list-new                                        \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst'
    over list-get-links                             \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst' ndf-link
    begin
        ?dup
    while
        dup link-get-data                           \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst' ndf-link ndf-reg
        #4 pick                                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst' ndf-link ndf-reg lft-lst'
        region-list-any-intersection-of             \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst' ndf-link bool
        if
            dup link-get-data                       \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst' ndf-link ndf-reg
            #2 pick                                 \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst' ndf-link ndf-reg ndfint-lst'
            list-push-struct                        \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst' ndf-link
        then

        link-get-next
    repeat

                                                    \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndf-lst' ndfint-lst'
    cr ." non-defining regions intersecting left-over regions: " dup .region-list

    \ Clean up.
    swap region-list-deallocate                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst'

    cr cr ." non-defining regions intersecting left-over regions, number intersections with incompatible pair regions" cr

    \ Test list empty, placeholder for production.
    dup list-is-empty
    if
        region-list-deallocate
        region-list-deallocate
        region-list-deallocate
        region-list-deallocate
        region-list-deallocate
        drop
    then

    \ Find non-defining, intersecting, regions count the number of intersects with incompatible pairs.

    \ Init max counter.
    0                                               \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt
    over list-get-links                             \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-link
    begin
        ?dup
    while
        dup link-get-data                           \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-link ndfint-reg
        #7 pick                                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-link ndfint-reg inc-lst'
        region-list-number-intersections            \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-link u
        over link-get-data cr .region space dup . cr

        \ Update max count.
        rot                                         \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' ndfint-link u cnt
        max swap                                    \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-link

        link-get-next
    repeat
                                                    \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt
    cr ." max count: " dup . cr

    \ Test max value, placeholder for production.
    dup 0= if
        drop
        region-list-deallocate
        region-list-deallocate
        region-list-deallocate
        region-list-deallocate
        region-list-deallocate
        drop
        exit
    then

    \ Get max number intersection regions.
    list-new                                        \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2'
    #2 pick list-get-links                          \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link
    begin
        ?dup
    while
        dup link-get-data                           \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link ndfint-reg

        \ Check if region has the maximum number of intersections.
        #8 pick                                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link ndfint-reg inc-lst'
        region-list-number-intersections            \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link int-cnt
        #3 pick                                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link int-cnt cnt
        =                                           \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link bool
        if
            \ Add the region to the result list.
            dup link-get-data                       \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link ndfint-reg
            #2 pick                                 \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link ndfint-reg ndfint-lst2'
            list-push-struct                        \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2' ndfint-link
        then

        link-get-next
    repeat
                                                    \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' cnt ndfint-lst2'
    cr ." max count regions: " dup .region-list cr

    \ Clean up.
    nip                                             \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst' ndfint-lst2'
    swap region-list-deallocate                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst2'

    \ Get states from incompatible list in each selected region.
    dup list-get-links                              \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst2' ndfint-link
    begin
        ?dup
    while
        dup link-get-data                           \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst2' ndfint-link regx
    cr ." reg: " dup .region
        #6 pick                                     \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst2' ndfint-link regx inc-lst'
        region-list-states-in                       \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst2' ndfint-link sta-lst'
    space ." states in: " dup .value-list cr

        \ TODO
        \ Check external-adjacent states, given region and state-in, to try forming a "logical corner" to unmask a new defining region.
        \ A compatible, external-adjacent, state stops consideraton for the state-in.
        \ An existing external-adjacent state may need more samples.
        \ A missing external-adjacent state needs a first sample.
        list-deallocate
        
        link-get-next
    repeat
                                                    \ dom0 inc-lst' ls-lst' df-lst' lft-lst' ndfint-lst2'
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    drop

    cr ." region-list-test-unmask-defining-region: Ok" cr
;

: region-list-tests
    current-session-new                             \ sess

    \ Init domain 0.
    #4 over domain-new                              \ sess dom0
    tuck                                            \ dom0 sess dom0
    swap                                            \ dom0 dom0 sess
    session-add-domain                              \ dom0

    region-list-test-region-intersections-n
    region-list-test-subtract-n
    region-list-test-states
    region-list-test-state-in-one-region
    region-list-test-states-in-one-region
    region-list-test-intersection-fragments
    region-list-test-normalize
    region-list-test-copy-except
    region-list-test-push-nosubs
    dup region-list-test-unmask-defining-region

    drop

    current-session-deallocate
;

