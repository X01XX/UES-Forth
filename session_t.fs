
\ Test using a non-intersecting step, the first, to find a plan, using forward-chaining.
\ Implementing the example in the sections of theory.html named "Choosing the next rule" and "A reason to not choose a rule".
: session-test-domain-get-plan-fc
    \ Init session.
    current-session-new

    \ Init domain.
    #4 domain-new dup                   \ dom dom
    current-session                     \ dom dom sess
    session-add-domain                  \ dom

    \ Add act1, act2 and act3.
    [ ' noop ] literal over domain-add-action   \ dom
    [ ' noop ] literal over domain-add-action   \ dom
    [ ' noop ] literal over domain-add-action   \ dom

    \ Set up group for act0.
    0 over domain-find-action                   \ dom, act0 t | f
    0= abort" can't find act0?"
    %0010 %0110 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act0
    %0000 %0100 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act1.
    1 over domain-find-action                   \ dom, act1 t | f
    0= abort" can't find act1?"
    %0001 %0000 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act1
    %1001 %1000 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act2.
    #2 over domain-find-action                  \ dom, act2 t | f
    0= abort" can't find act2?"
    %0011 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act2
    %1011 %1001 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act3.
    #3 over domain-find-action                  \ dom, act3 t | f
    0= abort" can't find act3?"
    %0110 %0010 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act3
    %0111 %0011 sample-new dup rot action-add-sample sample-deallocate      \ dom

    %0111 %0111 region-new                      \ dom reg-to
    %0100 %0100 region-new                      \ dom reg-to reg-from

    #3                                          \ dom reg-to reg-from | #3
    #2 pick #2 pick                             \ dom reg-to reg-from | #3 reg-to reg-from
    #5 pick                                     \ dom reg-to reg-from | #3 reg-to reg-from dom
    domain-get-plan-fc                          \ dom reg-to reg-from, plan t | f
    if
        cr ." plan " dup .plan cr
        dup plan-get-length #4 <> abort" plan not 4 steps long?"
        %0100 over plan-get-initial-region region-superset-of-state 0= abort" plan does not start at 4?"
        %0111 over plan-get-result-region region-superset-of-state 0= abort" plan does not end at 7?"
        plan-deallocate
    else
        cr ." no plan found" cr
        abort
    then
    region-deallocate
    region-deallocate
    drop

    current-session .session

    current-session-deallocate

    cr ." session-test-domain-get-plan-fc: Ok" cr
;

: session-test-domain-get-plan-bc
    \ Init session.
    current-session-new

    \ Init domain.
    #4 domain-new dup                   \ dom dom
    current-session                     \ dom dom sess
    session-add-domain                  \ dom

    \ Add act1, act2 and act3.
    [ ' noop ] literal over domain-add-action   \ dom
    [ ' noop ] literal over domain-add-action   \ dom
    [ ' noop ] literal over domain-add-action   \ dom

    \ Set up group for act0.
    0 over domain-find-action                   \ dom, act0 t | f
    0= abort" can't find act0?"
    %0010 %0110 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act0
    %0000 %0100 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act1.
    1 over domain-find-action                   \ dom, act1 t | f
    0= abort" can't find act1?"
    %0001 %0000 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act1
    %1001 %1000 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act2.
    #2 over domain-find-action                  \ dom, act2 t | f
    0= abort" can't find act2?"
    %0011 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act2
    %1011 %1001 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act3.
    #3 over domain-find-action                  \ dom, act3 t | f
    0= abort" can't find act3?"
    %0110 %0010 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act3
    %0111 %0011 sample-new dup rot action-add-sample sample-deallocate      \ dom

    current-session .session

    %0111 %0111 region-new                      \ dom reg-to
    %0100 %0100 region-new                      \ dom reg-to reg-from
    #3                                          \ dom reg-to reg-from | #3
    #2 pick #2 pick                             \ dom reg-to reg-from | #3 reg-to reg-from
    #5 pick                                     \ dom reg-to reg-from | #3 reg-to reg-from dom
    domain-get-plan-bc                          \ dom reg-to reg-from, plan t | f
    if
        cr ." plan " dup .plan cr
        dup plan-get-length #4 <> abort" plan not 4 steps long?"
        %0100 over plan-get-initial-region region-superset-of-state 0= abort" plan does not start at 4?"
        %0111 over plan-get-result-region region-superset-of-state 0= abort" plan does not end at 7?"
        plan-deallocate
    else
        cr ." no plan found" cr
        abort
    then
    region-deallocate
    region-deallocate
    drop

    current-session .session

    current-session-deallocate

    cr ." session-test-domain-get-plan-bc: Ok" cr
;

: session-test-domain-asymmetric-chaining
    \ Init session.
    current-session-new

    \ Init domain.
    #4 domain-new dup                   \ dom dom
    current-session                     \ dom dom sess
    session-add-domain                  \ dom

    \ Add act1, act2 and act3.
    [ ' noop ] literal over domain-add-action   \ dom
    [ ' noop ] literal over domain-add-action   \ dom
    [ ' noop ] literal over domain-add-action   \ dom

    \ Set up group for act0.
    0 over domain-find-action                   \ dom, act0 t | f
    0= abort" can't find act0?"
    %0010 %0110 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act0
    %0000 %0100 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act1.
    1 over domain-find-action                   \ dom, act1 t | f
    0= abort" can't find act1?"
    %0001 %0000 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act1
    %1001 %1000 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act2.
    #2 over domain-find-action                  \ dom, act2 t | f
    0= abort" can't find act2?"
    %0011 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act2
    %1011 %1001 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act3.
    #3 over domain-find-action                  \ dom, act3 t | f
    0= abort" can't find act3?"
    %0110 %0010 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act3
    %0111 %0011 sample-new dup rot action-add-sample sample-deallocate      \ dom

    current-session .session

    %0111 %0111 region-new                      \ dom reg-to
    %0100 %0100 region-new                      \ dom reg-to reg-from
    2dup                                        \ dom reg-to reg-from | reg-to reg-from
    #4 pick                                     \ dom reg-to reg-from | reg-to reg-from dom
    domain-asymmetric-chaining                  \ dom reg-to reg-from, plan t | f
    if
        cr ." plan " dup .plan cr
        dup plan-get-length #4 <> abort" plan not 4 steps long?"
        %0100 over plan-get-initial-region region-superset-of-state 0= abort" plan does not start at 4?"
        %0111 over plan-get-result-region region-superset-of-state 0= abort" plan does not end at 7?"
        plan-deallocate
    else
        cr ." no plan found" cr
        abort
    then
    region-deallocate
    region-deallocate
    drop

    current-session .session

    current-session-deallocate

    cr ." session-test-domain-asymmetric-chaining: Ok" cr
;

: session-test-change-to
    \ Init session.
    current-session-new

    \ Init domain 0.
    #4 domain-new dup                               \ dom0 dom0
    current-session                                 \ dom0 dom0 sess
    session-add-domain                              \ dom0

    \ Add act1, act2, act3, and act4.
    [ ' xor-1 ] literal over domain-add-action      \ dom0
    [ ' xor-2 ] literal over domain-add-action      \ dom0
    [ ' xor-4 ] literal over domain-add-action      \ dom0
    [ ' xor-8 ] literal over domain-add-action      \ dom0

    \ Set up group for act1.
    1 over domain-find-action                       \ dom0, act1 t | f
    0= abort" can't find act1?"
    %0000 over action-get-sample sample-deallocate  \ dom0 act1
    %1111 swap action-get-sample sample-deallocate  \ dom0

    \ Set up group for act2.
    #2 over domain-find-action                      \ dom0, act2 t | f
    0= abort" can't find act2?"
    %0000 over action-get-sample sample-deallocate  \ dom0 act2
    %1111 swap action-get-sample sample-deallocate  \ dom0

    \ Set up group for act3.
    #3 over domain-find-action                      \ dom0, act3 t | f
    0= abort" can't find act3?"
    %0000 over action-get-sample sample-deallocate  \ dom0 act3
    %1111 swap action-get-sample sample-deallocate  \ dom0

    \ Set up group for act4.
    #4 over domain-find-action                      \ dom0, act4 t | f
    0= abort" can't find act4?"
    %0000 over action-get-sample sample-deallocate  \ dom0 act4
    %1111 swap action-get-sample sample-deallocate  \ dom0
    drop

    \ Init domain 1.
    #5 domain-new dup                               \ dom1 dom1
    current-session                                 \ dom1 dom1 sess
    session-add-domain                              \ dom1

    \ Add act1, act2, act3, act4 and act5.
    [ ' xor-1 ] literal over domain-add-action      \ dom1
    [ ' xor-2 ] literal over domain-add-action      \ dom1
    [ ' xor-4 ] literal over domain-add-action      \ dom1
    [ ' xor-8 ] literal over domain-add-action      \ dom1
    [ ' xor-16 ] literal over domain-add-action     \ dom1

    \ Set up group for act1.
    1 over domain-find-action                       \ dom1, act1 t | f
    0= abort" can't find act1?"
    %00000 over action-get-sample sample-deallocate \ dom1 act1
    %11111 swap action-get-sample sample-deallocate \ dom1

    \ Set up group for act2.
    #2 over domain-find-action                      \ dom1, act2 t | f
    0= abort" can't find act2?"
    %00000 over action-get-sample sample-deallocate \ dom1 act2
    %11111 swap action-get-sample sample-deallocate \ dom1

    \ Set up group for act3.
    #3 over domain-find-action                      \ dom1, act3 t | f
    0= abort" can't find act3?"
    %00000 over action-get-sample sample-deallocate \ dom1 act3
    %11111 swap action-get-sample sample-deallocate \ dom1

    \ Set up group for act4.
    #4 over domain-find-action                      \ dom1, act4 t | f
    0= abort" can't find act3?"
    %00000 over action-get-sample sample-deallocate \ dom1 act4
    %11111 swap action-get-sample sample-deallocate \ dom1

    \ Set up group for act5.
    #5 over domain-find-action                      \ dom1, act5 t | f
    0= abort" can't find act5?"
    %00000 over action-get-sample sample-deallocate \ dom1 act5
    %11111 swap action-get-sample sample-deallocate \ dom1
    drop                                                                        \
    
    current-session                                 \ sess
    s" (X1X1 01X1X)" regioncorr-from-string-a       \ sess regc
    -1 #2 rate-new                                  \ sess regc rt
    regioncorrrate-new                              \ sess regc-rt
    \ cr ." regioncorrrate: " dup .regioncorrrate cr
    over session-add-regioncorrrate                  \ sess

    s" (1XX1 01X1X)" regioncorr-from-string-a       \ sess regc
    #-2 0 rate-new                                  \ sess
    regioncorrrate-new                              \ sess regc-rt
    \ cr ." regioncorrrate: " dup .regioncorrrate cr
    over session-add-regioncorrrate                 \ sess

    0 over session-find-domain                      \ sess, dom t | f
    is-false abort" domain 0 not found?"
    over session-set-current-domain                 \ sess

    \ 0
\    s" (0011 01111)" regioncorr-from-string-a       \ sess regc-to
\    s" (0000 01111)" regioncorr-from-string-a       \ sess regc-to regc-from

    \ -1
    \ Set domain 0 current state.
    0 over session-find-domain                      \ sess, dom t | f
    is-false abort" domain0 not found?"
    %1000 swap domain-set-current-state             \ sess

    \ Set domain1 current state.
    1 over session-find-domain                      \ sess, dom t | f
    is-false abort" domain1 not found?"
    %01111 swap domain-set-current-state             \ sess

    dup .session

    \ Create from/to regioncorr instances.
    s" (0111 01111)" regioncorr-from-string-a       \ sess regc-to
    s" (1000 01111)" regioncorr-from-string-a       \ sess regc-to regc-from

    \ -2
\    s" (1001 01111)" regioncorr-from-string-a       \ sess regc-to
\    s" (0000 01111)" regioncorr-from-string-a       \ sess regc-to regc-from

    \ -3
\    s" (1101 01111)" regioncorr-from-string-a       \ sess regc-to
\    s" (0000 01111)" regioncorr-from-string-a       \ sess regc-to regc-from

    over                                            \ sess regc-to regc-from regc-to
    #3 pick                                         \ sess regc-to regc-from regc-to sess
    session-change-to                               \ sess regc-to regc-from bool
    if
        cr ." change succeeded" cr
    else
        cr ." change failed?" cr
        abort
    then
                                                    \ sess regc-to regc-from
    regioncorr-deallocate
    regioncorr-deallocate
    drop

    current-session-deallocate

    cr ." session-test-change-to: Ok" cr
;

: session-test-corner-needs
    \ Init session.
    current-session-new                             \
    current-session                                 \ sess

    \ Init domain 0.
    #4 domain-new                                   \ sess dom0
    2dup swap                                       \ sess dom0 dom0 sess
    session-add-domain                              \ sess dom0

    \ Get act0.
    0 over domain-find-action                       \ sess dom0, act0 t | f
    is-false abort" act0 not found?"

    \ Add arbitrary samples
    #5 #5 sample-new                                \ sess dom0 act0 smpl5
    2dup swap action-add-sample                     \ sess dom0 act0 smpl5
    2dup swap action-add-sample                     \ sess dom0 act0 smpl5
    2dup swap action-add-sample                     \ sess dom0 act0 smpl5
    2dup swap action-add-sample                     \ sess dom0 act0 smpl5

    cr over .action cr

    #5 #2 pick action-find-square
    is-false abort" square not found?"
    .square 

    sample-deallocate
    3drop
    current-session-deallocate

    cr ." session-test-corner-needs: Ok" cr
;

: session-tests
    session-test-domain-get-plan-fc
    session-test-domain-get-plan-bc
    session-test-domain-asymmetric-chaining
    session-test-change-to
;
