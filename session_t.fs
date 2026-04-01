
: xor-1 ( cur-sta1 act0 -- smpl )
    drop
    dup 1 xor swap sample-new
;

: xor-2 ( cur-sta1 act0 -- smpl )
    drop
    dup #2 xor swap sample-new
;

: xor-4 ( cur-sta1 act0 -- smpl )
    drop
    dup #4 xor swap sample-new
;

: xor-8 ( cur-sta1 act0 -- smpl )
    drop
    dup #8 xor swap sample-new
;

: xor-16 ( cur-sta1 act0 -- smpl )
    drop
    dup
    #16 xor swap sample-new
;

\ Test using a non-intersecting step, the first, to find a plan, using forward-chaining.
\ Implementing the example in the sections of theory.html named "Choosing the next rule" and "A reason to not choose a rule".
: session-test-domain-get-plan-fc
    \ Init session.
    current-session-new                 \ ses

    \ Init domain.
    #4 current-session domain-new dup   \ sess dom dom
    #2 pick                             \ sess dom dom sess
    session-add-domain                  \ ses dom

    \ Add act1, act2 and act3.
    [ ' noop ] literal over domain-add-action   \ ses dom
    [ ' noop ] literal over domain-add-action   \ ses dom
    [ ' noop ] literal over domain-add-action   \ ses dom

    \ Set up group for act0.
    0 over domain-find-action                   \ ses dom, act0 t | f
    0= abort" can't find act0?"
    %0010 %0110 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act0
    %0000 %0100 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act1.
    1 over domain-find-action                   \ ses dom, act1 t | f
    0= abort" can't find act1?"
    %0001 %0000 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act1
    %1001 %1000 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act2.
    #2 over domain-find-action                  \ ses dom, act2 t | f
    0= abort" can't find act2?"
    %0011 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act2
    %1011 %1001 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act3.
    #3 over domain-find-action                  \ ses dom, act3 t | f
    0= abort" can't find act3?"
    %0110 %0010 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act3
    %0111 %0011 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    dup .domain


    %0111 %0111 region-new                      \ ses dom reg-to
    %0100 %0100 region-new                      \ ses dom reg-to reg-from

    cr ." For reg: " dup .region space ." to: " over .region cr
    #3                                          \ ses dom reg-to reg-from | #3
    #2 pick #2 pick                             \ ses dom reg-to reg-from | #3 reg-to reg-from
    #5 pick                                     \ ses dom reg-to reg-from | #3 reg-to reg-from dom
    domain-get-plan-fc                          \ ses dom reg-to reg-from, plan t | f
    if
        cr ." plan " dup .plan cr
        dup plan-get-length #4 <> abort" plan not 4 steps long?"
        %0100 over plan-get-initial-region region-superset-of-state? 0= abort" plan does not start at 4?"
        %0111 over plan-get-result-region region-superset-of-state? 0= abort" plan does not end at 7?"
        plan-deallocate
    else
        cr ." no plan found" cr
        abort
    then
    region-deallocate
    region-deallocate
    drop

    drop
    \ .session

    current-session-deallocate

    cr ." session-test-domain-get-plan-fc: Ok" cr
;

: session-test-domain-get-plan-bc
    \ Init session.
    current-session-new                 \ ses

    \ Init domain.
    #4 current-session domain-new dup   \ ses dom dom
    #2 pick                             \ ses dom dom ses
    session-add-domain                  \ ses dom

    \ Add act1, act2 and act3.
    [ ' noop ] literal over domain-add-action   \ ses dom
    [ ' noop ] literal over domain-add-action   \ ses dom
    [ ' noop ] literal over domain-add-action   \ ses dom

    \ Set up group for act0.
    0 over domain-find-action                   \ ses dom, act0 t | f
    0= abort" can't find act0?"
    %0010 %0110 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act0
    %0000 %0100 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act1.
    1 over domain-find-action                   \ ses dom, act1 t | f
    0= abort" can't find act1?"
    %0001 %0000 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act1
    %1001 %1000 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act2.
    #2 over domain-find-action                  \ ses dom, act2 t | f
    0= abort" can't find act2?"
    %0011 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act2
    %1011 %1001 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act3.
    #3 over domain-find-action                  \ ses dom, act3 t | f
    0= abort" can't find act3?"
    %0110 %0010 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act3
    %0111 %0011 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    current-session .session

    %0111 %0111 region-new                      \ ses dom reg-to
    %0100 %0100 region-new                      \ ses dom reg-to reg-from
    #3                                          \ ses dom reg-to reg-from | #3
    #2 pick #2 pick                             \ ses dom reg-to reg-from | #3 reg-to reg-from
    #5 pick                                     \ ses dom reg-to reg-from | #3 reg-to reg-from dom
    domain-get-plan-bc                          \ ses dom reg-to reg-from, plan t | f
    if
        cr ." plan " dup .plan cr
        dup plan-get-length #4 <> abort" plan not 4 steps long?"
        %0100 over plan-get-initial-region region-superset-of-state? 0= abort" plan does not start at 4?"
        %0111 over plan-get-result-region region-superset-of-state? 0= abort" plan does not end at 7?"
        plan-deallocate
    else
        cr ." no plan found" cr
        abort
    then
    region-deallocate
    region-deallocate
    drop

    .session

    current-session-deallocate

    cr ." session-test-domain-get-plan-bc: Ok" cr
;

: session-test-domain-asymmetric-chaining
    \ Init session.
    current-session-new                 \ ses

    \ Init domain.
    #4 over                             \ ses #4 ses
    domain-new dup                      \ ses dom dom
    #2 pick                             \ ses dom dom ses
    session-add-domain                  \ ses dom

    \ Add act1, act2 and act3.
    [ ' noop ] literal over domain-add-action   \ ses dom
    [ ' noop ] literal over domain-add-action   \ ses dom
    [ ' noop ] literal over domain-add-action   \ ses dom

    \ Set up group for act0.
    0 over domain-find-action                   \ ses dom, act0 t | f
    0= abort" can't find act0?"
    %0010 %0110 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act0
    %0000 %0100 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act1.
    1 over domain-find-action                   \ ses dom, act1 t | f
    0= abort" can't find act1?"
    %0001 %0000 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act1
    %1001 %1000 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act2.
    #2 over domain-find-action                  \ ses dom, act2 t | f
    0= abort" can't find act2?"
    %0011 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act2
    %1011 %1001 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    \ Set up group for act3.
    #3 over domain-find-action                  \ ses dom, act3 t | f
    0= abort" can't find act3?"
    %0110 %0010 sample-new dup #2 pick action-add-sample sample-deallocate  \ ses dom act3
    %0111 %0011 sample-new dup rot action-add-sample sample-deallocate      \ ses dom

    current-session .session

    %0111 %0111 region-new                      \ ses dom reg-to
    %0100 %0100 region-new                      \ ses dom reg-to reg-from
    2dup                                        \ ses dom reg-to reg-from | reg-to reg-from
    #4 pick                                     \ ses dom reg-to reg-from | reg-to reg-from dom
    domain-asymmetric-chaining                  \ ses dom reg-to reg-from, plan t | f
    if
        cr ." plan " dup .plan cr
        dup plan-get-length #4 <> abort" plan not 4 steps long?"
        %0100 over plan-get-initial-region region-superset-of-state? 0= abort" plan does not start at 4?"
        %0111 over plan-get-result-region region-superset-of-state? 0= abort" plan does not end at 7?"
        plan-deallocate
    else
        cr ." no plan found" cr
        abort
    then
    region-deallocate
    region-deallocate
    drop

    \ drop
    .session

    current-session-deallocate

    cr ." session-test-domain-asymmetric-chaining: Ok" cr
;

: session-test-change-to
    \ Init session.
    current-session-new                             \ ses

    \ Init domain 0.
    #4 current-session domain-new dup               \ ses dom0 dom0
    #2 pick                                         \ ses dom0 dom0 sess
    session-add-domain                              \ ses dom0

    \ Add act1, act2, act3, and act4.
    [ ' xor-1 ] literal over domain-add-action      \ ses dom0
    [ ' xor-2 ] literal over domain-add-action      \ ses dom0
    [ ' xor-4 ] literal over domain-add-action      \ ses dom0
    [ ' xor-8 ] literal over domain-add-action      \ ses dom0

    \ Set up group for act1.
    1 over domain-find-action                       \ ses dom0, act1 t | f
    0= abort" can't find act1?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act1
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act2.
    #2 over domain-find-action                      \ ses dom0, act2 t | f
    0= abort" can't find act2?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act2
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act3.
    #3 over domain-find-action                      \ ses dom0, act3 t | f
    0= abort" can't find act3?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act3
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act4.
    #4 over domain-find-action                      \ ses dom0, act4 t | f
    0= abort" can't find act4?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act4
    %1111 swap action-get-sample sample-deallocate  \ ses dom0
    drop

    \ Init domain 1.
    #5 current-session domain-new dup               \ ses dom1 dom1
    current-session                                 \ ses dom1 dom1 sess
    session-add-domain                              \ ses dom1

    \ Add act1, act2, act3, act4 and act5.
    [ ' xor-1 ] literal over domain-add-action      \ ses dom1
    [ ' xor-2 ] literal over domain-add-action      \ ses dom1
    [ ' xor-4 ] literal over domain-add-action      \ ses dom1
    [ ' xor-8 ] literal over domain-add-action      \ ses dom1
    [ ' xor-16 ] literal over domain-add-action     \ ses dom1

    \ Set up group for act1.
    1 over domain-find-action                       \ ses dom1, act1 t | f
    0= abort" can't find act1?"
    %00000 over action-get-sample sample-deallocate \ ses dom1 act1
    %11111 swap action-get-sample sample-deallocate \ ses dom1

    \ Set up group for act2.
    #2 over domain-find-action                      \ ses dom1, act2 t | f
    0= abort" can't find act2?"
    %00000 over action-get-sample sample-deallocate \ ses dom1 act2
    %11111 swap action-get-sample sample-deallocate \ ses dom1

    \ Set up group for act3.
    #3 over domain-find-action                      \ ses dom1, act3 t | f
    0= abort" can't find act3?"
    %00000 over action-get-sample sample-deallocate \ ses dom1 act3
    %11111 swap action-get-sample sample-deallocate \ ses dom1

    \ Set up group for act4.
    #4 over domain-find-action                      \ ses dom1, act4 t | f
    0= abort" can't find act3?"
    %00000 over action-get-sample sample-deallocate \ ses dom1 act4
    %11111 swap action-get-sample sample-deallocate \ ses dom1

    \ Set up group for act5.
    #5 over domain-find-action                      \ ses dom1, act5 t | f
    0= abort" can't find act5?"
    %00000 over action-get-sample sample-deallocate \ ses dom1 act5
    %11111 swap action-get-sample sample-deallocate \ ses dom1
    drop                                            \ ses

    s" (X1X1 01X1X)" regioncorr-from-string-a       \ ses regc
    -1 #2 rate-new                                  \ ses regc rt
    regioncorrrate-new                              \ ses regc-rt
    \ cr ." regioncorrrate: " dup .regioncorrrate cr
    over session-add-regioncorrrate                 \ ses

    s" (1XX1 01X1X)" regioncorr-from-string-a       \ ses regc
    #-2 0 rate-new                                  \ ses
    regioncorrrate-new                              \ ses regc-rt
    \ cr ." regioncorrrate: " dup .regioncorrrate cr
    over session-add-regioncorrrate                 \ ses

    0 over session-find-domain                      \ sess, dom t | f
    false? abort" domain 0 not found?"
    over session-set-current-domain                 \ ses

    \ 0
\    s" (0011 01111)" regioncorr-from-string-a       \ ses regc-to
\    s" (0000 01111)" regioncorr-from-string-a       \ ses regc-to regc-from

    \ -1
    \ Set domain 0 current state.
    0 over session-find-domain                      \ ses, dom t | f
    false? abort" domain0 not found?"
    %1000 swap domain-set-current-state             \ ses

    \ Set domain1 current state.
    1 over session-find-domain                      \ ses, dom t | f
    false? abort" domain1 not found?"
    %01111 swap domain-set-current-state            \ ses

    dup .session

    \ Create from/to regioncorr instances.
    s" (0111 01111)" regioncorr-from-string-a       \ ses regc-to
    s" (1000 01111)" regioncorr-from-string-a       \ ses regc-to regc-from

    \ -2
\    s" (1001 01111)" regioncorr-from-string-a       \ ses regc-to
\    s" (0000 01111)" regioncorr-from-string-a       \ ses regc-to regc-from

    \ -3
\    s" (1101 01111)" regioncorr-from-string-a       \ ses regc-to
\    s" (0000 01111)" regioncorr-from-string-a       \ ses regc-to regc-from

    over                                            \ ses regc-to regc-from regc-to
    #3 pick                                         \ ses regc-to regc-from regc-to sess

    session-change-to-plans                         \ ses regc-to regc-from, planc-lst t | f
    if
        cr ." Plan found: " dup .plancorr-list cr
        dup plancorr-list-run-plans                 \ ses regc-to regc-from planc-lst' bool
        if
            cr ." Plan suceeded" cr
        else
            cr ." Plan failed?" cr
            abort
        then
        plancorr-list-deallocate                    \ ses regc-to regc-from
    else
        cr ." No plan found?" cr
        abort
    then

                                                    \ ses regc-to regc-from
    regioncorr-deallocate                           \ ses regc-to
    regioncorr-deallocate                           \ ses
    drop

    current-session-deallocate

    cr ." session-test-change-to: Ok" cr
;

\ Test navigation around negative regions to get for 0 to 7.
\ Like 0->8->9->B->F->7, bypassing 0011, X101 and X110.
: session-test-change-to-plans
    \ Init session.
    current-session-new                             \ ses

    \ Init domain 0.
    #4 current-session domain-new dup               \ ses dom0 dom0
    #2 pick                                         \ ses dom0 dom0 sess
    session-add-domain                              \ ses dom0

    \ Add act1, act2, act3, and act4.
    [ ' xor-1 ] literal over domain-add-action      \ ses dom0
    [ ' xor-2 ] literal over domain-add-action      \ ses dom0
    [ ' xor-4 ] literal over domain-add-action      \ ses dom0
    [ ' xor-8 ] literal over domain-add-action      \ ses dom0

    \ Set up group for act1.
    1 over domain-find-action                       \ ses dom0, act1 t | f
    0= abort" can't find act1?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act1
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act2.
    #2 over domain-find-action                      \ ses dom0, act2 t | f
    0= abort" can't find act2?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act2
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act3.
    #3 over domain-find-action                      \ ses dom0, act3 t | f
    0= abort" can't find act3?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act3
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act4.
    #4 over domain-find-action                      \ ses dom0, act4 t | f
    0= abort" can't find act4?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act4
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    swap                                            \ dom0 ses

    \ Set negative region 0011.
    s" (0011)" regioncorr-from-string-a             \ dom0 ses regc
    -1 0 rate-new                                   \ dom0 ses regc rt
    regioncorrrate-new                              \ dom0 ses regc-rt
    over session-add-regioncorrrate                 \ dom0 ses

    \ Set negative region X101
    s" (X101)" regioncorr-from-string-a             \ dom0 ses regc
    -1 0 rate-new                                   \ dom0 ses regc rt
    regioncorrrate-new                              \ dom0 ses regc-rt
    over session-add-regioncorrrate                 \ dom0 ses

    \ Set negative region X110.
    s" (X110)" regioncorr-from-string-a             \ dom0 ses regc
    -1 0 rate-new                                   \ dom0 ses regc rt
    regioncorrrate-new                              \ dom0 ses regc-rt
    over session-add-regioncorrrate                 \ dom0 ses

    \ Set current state to zero.
    0 #2 pick domain-set-current-state              \ dom0 ses

    dup .session

    \ Get plans for 0->7.
    s" (0111)" regioncorr-from-string-a             \ dom0 ses regc'
    dup                                             \ dom0 ses regc' regc'
    #2 pick session-change-to-plans                 \ dom0 ses regc', plnc-lst t | f
    if
        cr ." plan found: " dup .plancorr-list cr
        plancorr-list-deallocate
    else
        cr ." plan NOT found" cr
    then
    regioncorr-deallocate                           \ dom0 ses

    2drop

    current-session-deallocate

    cr ." session-test-change-to-plans: Ok" cr
;

: session-test-calc-path-fc
    cr ." session-test-calc-path-fc: Start" cr
    \ Init session.
    current-session-new                             \ ses

    \ Init domain 0.
    #4 current-session domain-new dup               \ ses dom0 dom0
    #2 pick                                         \ ses dom0 dom0 sess
    session-add-domain                              \ ses dom0

    \ Add act1, act2, act3, and act4.
    [ ' xor-1 ] literal over domain-add-action      \ ses dom0
    [ ' xor-2 ] literal over domain-add-action      \ ses dom0
    [ ' xor-4 ] literal over domain-add-action      \ ses dom0
    [ ' xor-8 ] literal over domain-add-action      \ ses dom0

    \ Set up group for act1.
    1 over domain-find-action                       \ ses dom0, act1 t | f
    0= abort" can't find act1?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act1
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act2.
    #2 over domain-find-action                      \ ses dom0, act2 t | f
    0= abort" can't find act2?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act2
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act3.
    #3 over domain-find-action                      \ ses dom0, act3 t | f
    0= abort" can't find act3?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act3
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act4.
    #4 over domain-find-action                      \ ses dom0, act4 t | f
    0= abort" can't find act4?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act4
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    swap                                            \ dom0 ses

    \ Set negative region X101.
    s" (0110)" regioncorr-from-string-a             \ dom0 ses regc
    -1 0 rate-new                                   \ dom0 ses regc rt
    regioncorrrate-new                              \ dom0 ses regc-rt
    over session-add-regioncorrrate                 \ dom0 ses

    \ Set negative region X110
    s" (0101)" regioncorr-from-string-a             \ dom0 ses regc
    -1 0 rate-new                                   \ dom0 ses regc rt
    regioncorrrate-new                              \ dom0 ses regc-rt
    over session-add-regioncorrrate                 \ dom0 ses

    \ Set current state to zero.
    0 #2 pick domain-set-current-state              \ dom0 ses

    dup .session

    s" (0111)" regioncorr-from-string-a             \ dom0 ses regc-to'
    s" (0100)" regioncorr-from-string-a             \ dom0 ses regc-to' regc-from'

    #3                                              \ dom0 ses regc-to' regc-from' | depth
    #2 pick                                         \ dom0 ses regc-to' regc-from' | depth regc-to
    #2 pick                                         \ dom0 ses regc-to' regc-from' | depth regc-to regc-from
    0                                               \ dom0 ses regc-to' regc-from' | depth regc-to regc-from rate
    #6 pick                                         \ dom0 ses regc-to' regc-from' | depth regc-to regc-from rate ses
    session-find-pathstep-list-by-rate              \ dom0 ses regc-to' regc-from' | depth regc-to regc-from pthstp-lst
    #6 pick                                         \ dom0 ses regc-to' regc-from' | depth regc-to regc-from pthstp-lst ses
    \ cr .s cr
    session-calc-path-fc                            \ dom0 ses regc-to' regc-from' | regc-seq' t | f
    \ cr .s cr
    if                                              \ dom0 ses regc-to' regc-from' | regc-seq'
        cr ." Path: " dup .regioncorr-list-as-path-fc cr

        \ Check results.
        dup list-get-length 4 <> abort" path not legth 4?"

        \ Check first regioncorr.
        2dup list-get-first-item                    \ dom0 ses regc-to' regc-from' | regc-seq' regc-from' first-regc
        regioncorr-eq?                              \ dom0 ses regc-to' regc-from' | regc-seq' bool
        false? abort" first item is not regc-from?"

        \ The two intermediate regioncorrs can vary. 1X00->1X11, 1000->X011, 1000->1X11, X000->X011.

        \ Check last regioncorr.
        #2 pick over                                \ dom0 ses regc-to' regc-from' | regc-seq' regc-to' regc-seq'
        list-get-last-item                          \ dom0 ses regc-to' regc-from' | regc-seq' regc-from' last-regc
        regioncorr-eq?                              \ dom0 ses regc-to' regc-from' | regc-seq' bool
        false? abort" last item is not regc-to?"

        \ Clean up.
        regioncorr-list-deallocate
    else
        cr ." Path not found?"
        abort
    then
                                                    \ dom0 ses regc-to' regc-from'
    regioncorr-deallocate                           \ dom0 ses regc-to'
    regioncorr-deallocate                           \ dom0 ses
    2drop                                           \

    current-session-deallocate

    cr ." session-test-calc-path-fc: Ok" cr
;

: session-test-calc-path-bc
    cr ." session-test-calc-path-bc: Start" cr
    \ Init session.
    current-session-new                             \ ses

    \ Init domain 0.
    #4 current-session domain-new dup               \ ses dom0 dom0
    #2 pick                                         \ ses dom0 dom0 sess
    session-add-domain                              \ ses dom0

    \ Add act1, act2, act3, and act4.
    [ ' xor-1 ] literal over domain-add-action      \ ses dom0
    [ ' xor-2 ] literal over domain-add-action      \ ses dom0
    [ ' xor-4 ] literal over domain-add-action      \ ses dom0
    [ ' xor-8 ] literal over domain-add-action      \ ses dom0

    \ Set up group for act1.
    1 over domain-find-action                       \ ses dom0, act1 t | f
    0= abort" can't find act1?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act1
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act2.
    #2 over domain-find-action                      \ ses dom0, act2 t | f
    0= abort" can't find act2?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act2
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act3.
    #3 over domain-find-action                      \ ses dom0, act3 t | f
    0= abort" can't find act3?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act3
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    \ Set up group for act4.
    #4 over domain-find-action                      \ ses dom0, act4 t | f
    0= abort" can't find act4?"
    %0000 over action-get-sample sample-deallocate  \ ses dom0 act4
    %1111 swap action-get-sample sample-deallocate  \ ses dom0

    swap                                            \ dom0 ses

    \ Set negative region X101.
    s" (X110)" regioncorr-from-string-a             \ dom0 ses regc
    -1 0 rate-new                                   \ dom0 ses regc rt
    regioncorrrate-new                              \ dom0 ses regc-rt
    over session-add-regioncorrrate                 \ dom0 ses

    \ Set negative region X110
    s" (X101)" regioncorr-from-string-a             \ dom0 ses regc
    -1 0 rate-new                                   \ dom0 ses regc rt
    regioncorrrate-new                              \ dom0 ses regc-rt
    over session-add-regioncorrrate                 \ dom0 ses

    \ Set current state to zero.
    0 #2 pick domain-set-current-state              \ dom0 ses

    dup .session

    s" (0111)" regioncorr-from-string-a             \ dom0 ses regc-to'
    s" (0100)" regioncorr-from-string-a             \ dom0 ses regc-to' regc-from'

    #3                                              \ dom0 ses regc-to' regc-from' | depth
    #2 pick                                         \ dom0 ses regc-to' regc-from' | depth regc-to
    #2 pick                                         \ dom0 ses regc-to' regc-from' | depth regc-to regc-from
    0                                               \ dom0 ses regc-to' regc-from' | depth regc-to regc-from rate
    #6 pick                                         \ dom0 ses regc-to' regc-from' | depth regc-to regc-from rate ses
    session-find-pathstep-list-by-rate              \ dom0 ses regc-to' regc-from' | depth regc-to regc-from pthstp-lst
    #6 pick                                         \ dom0 ses regc-to' regc-from' | depth regc-to regc-from pthstp-lst ses
    \ cr .s cr
\    session-calc-path-bc                            \ dom0 ses regc-to' regc-from' | regc-seq' t | f
    \ cr .s cr
    if                                              \ dom0 ses regc-to' regc-from' | regc-seq'
        cr ." Path: " dup .regioncorr-list-as-path-bc cr

        \ Check results.
\        dup list-get-length 4 <> abort" path not legth 4?"
\        
\        2dup regioncorr-list-get-first              \ dom0 ses regc-to' regc-from' | regc-seq' regc-from' first-regc
\        regioncorr-eq?                              \ dom0 ses regc-to' regc-from' | regc-seq' bool
\        false? abort" first item is not regc-from?"

\        #2 pick over                                \ dom0 ses regc-to' regc-from' | regc-seq' regc-to' regc-seq'
\        regioncorr-list-get-last                    \ dom0 ses regc-to' regc-from' | regc-seq' regc-from' last-regc
\        regioncorr-eq?                              \ dom0 ses regc-to' regc-from' | regc-seq' bool
\        false? abort" last item is not regc-to?"

        \ Clean up.
        regioncorr-list-deallocate
    else
        cr ." Path not found?"
    then
                                                    \ dom0 ses regc-to' regc-from'
    regioncorr-deallocate                           \ dom0 ses regc-to'
    regioncorr-deallocate                           \ dom0 ses
    2drop                                           \

    current-session-deallocate

    cr ." session-test-calc-path-bc: Ok" cr
;

: session-tests
    session-test-domain-get-plan-fc
    session-test-domain-get-plan-bc
    session-test-domain-asymmetric-chaining
    session-test-change-to
    session-test-calc-path-fc
    \ session-test-calc-path-bc
;
