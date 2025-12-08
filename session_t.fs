
\ Test using a non-intersecting step, the first, to find a plan, using forward-chaining.
\ Implementing the example in the sections of theory.html named "Choosing the next rule" and "A reason to not choose a rule".
: session-test-domain-get-plan-fc
    current-session-new

    \ Init session and domain.
    #4 domain-new dup                   \ dom dom
    current-session                     \ dom dom sess
    session-add-domain                  \ dom

    current-session session-process-rlcrates

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
    2 over domain-find-action                   \ dom, act2 t | f
    0= abort" can't find act2?"
    %0011 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act2
    %1011 %1001 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act3.
    3 over domain-find-action                   \ dom, act3 t | f
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
        dup plan-get-length 4 <> abort" plan not 4 steps long?"
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

    cr ." session-test-domain-get-plan-fc - Ok" cr
;

: session-test-domain-get-plan-bc
    current-session-new

    \ Init session and domain.
    #4 domain-new dup                   \ dom dom
    current-session                     \ dom dom sess
    session-add-domain                  \ dom

    current-session session-process-rlcrates

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
    2 over domain-find-action                   \ dom, act2 t | f
    0= abort" can't find act2?"
    %0011 %0001 sample-new dup #2 pick action-add-sample sample-deallocate  \ dom act2
    %1011 %1001 sample-new dup rot action-add-sample sample-deallocate      \ dom

    \ Set up group for act3.
    3 over domain-find-action                   \ dom, act3 t | f
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
        dup plan-get-length 4 <> abort" plan not 4 steps long?"
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

    cr ." session-test-domain-get-plan-bc - Ok" cr
;

: session-tests
    session-test-domain-get-plan-fc
    session-test-domain-get-plan-bc
;
