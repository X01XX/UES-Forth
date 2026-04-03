\ Split a non-zero changes instance into separate single-bit changes.
: changes-split ( cngs0 -- cngs-lst )
    \ Check args.
    assert-tos-is-changes

    list-new swap               \ ret cngs
    dup changes-get-m01         \ ret cngs0 m01
    begin
        ?dup
    while
        isolate-a-bit           \ ret cngs0 m01' bit
        0 swap changes-new      \ ret cngs0 m01' cngsx
        #3 pick                 \ ret cngs0 m01' cngsx ret
        changes-list-push       \ ret cngs0 m01'
    repeat

    changes-get-m10             \ ret m10
    begin
        ?dup
    while
        isolate-a-bit           \ ret m10' bit
        0 changes-new           \ ret m10' cngsx
        #2 pick                 \ ret m10' cngsx ret
        changes-list-push       \ ret m10'
    repeat
;
