
\ For use outside of the GPL 3.0 license, except for stack.fs mm_array.fs link.fs list.fs tools.fs stackprint.fs,
\ contact the Wisconsin Alumni Research Foundation (WARF).

\ Struct IDs.
\
\ Each number is prime, 5 digits, and fits within 16 bits.
\ The digit pairs [0][1], [1][2], [2][3], and [3][4] are prime. 
\ No digit appears more than twice. (avoids 111)
\ No digit appears consecutively.   (avoids 11)
\
\ Struct ids in use.
\ Link   List   Region Rule   RuleStore Square
\ 17137, 17971, 19317, 23131, 23173,    23197
\
\ Sample Action Session Domain Need   Changes
\ 23719, 29717, 31319,  31379, 19717, 31973
\
\ Step   Plan   Group  Rate   RlcRate
\ 37171, 37379, 43717, 41719, 41737
\
\ Memory
\ 47317
\
\ Struct ids not yet used:
\ 53171, 53173, 53197, 53717, 53719,
\ 53731, 59797, 61379, 61717, 61979.

\ Start a clean vocabulary.
cr ." Starting vocabulary UES," cr
vocabulary UES

\ Put new words into the UES vocabulary.
UES definitions

decimal

include xtindirect.fs
include bool.fs

include tools.fs
include mm_array.fs     \ includes stack.fs
include link.fs
include list.fs

\ Application.
include globals.fs

include value.fs

include region.fs
include region2.fs

include regionlist.fs

include state.fs
include statelistcorr.fs

include changes.fs
include changeslist.fs
include changes2.fs

include sample.fs

include rule.fs
include rulestore.fs
include rulelist.fs
include rulelistcorr.fs

include square.fs

include valuelist.fs
include squarelist.fs

include need.fs
include needlist.fs

include step.fs
include steplist.fs

include regionlistcorr.fs
include rlclist.fs
include rate.fs
include rlcrate.fs
include rlcratelist.fs

include plan.fs
include planlist.fs
include plan_t.fs
include planlistcorr.fs

include group.fs
include grouplist.fs

include action.fs
include actionlist.fs
include actionxts.fs

include domain.fs
include domainlist.fs

include session.fs

include input.fs

include stackprint.fs

cs

: memory-use
    cr ." Memory use:"
    cr #4 spaces ." Link mma:         " link-mma .mma-usage
    cr #4 spaces ." List mma:         " list-mma .mma-usage
    cr #4 spaces ." Region mma:       " region-mma .mma-usage
    cr #4 spaces ." Rule mma:         " rule-mma .mma-usage
    cr #4 spaces ." RuleStore mma:    " rulestore-mma .mma-usage
    cr #4 spaces ." Square mma:       " square-mma .mma-usage
    cr #4 spaces ." Sample mma:       " sample-mma .mma-usage
    cr #4 spaces ." Changes mma:      " changes-mma .mma-usage
    cr #4 spaces ." Group mma:        " group-mma .mma-usage
    cr #4 spaces ." Need mma:         " need-mma .mma-usage
    cr #4 spaces ." Step mma:         " step-mma .mma-usage
    cr #4 spaces ." Plan mma:         " plan-mma .mma-usage
    cr #4 spaces ." Rate mma:         " rate-mma .mma-usage
    cr #4 spaces ." RlcRate mma:      " rlcrate-mma .mma-usage
    cr #4 spaces ." Action mma:       " action-mma .mma-usage
    cr #4 spaces ." Domain mma:       " domain-mma .mma-usage
    cr #4 spaces ." Session mma:      " session-mma .mma-usage
    cr #4 spaces ." dstack: " .s
;

' memory-use to memory-use-xt

\ Check that no struct instances are in use, stack is clear.
: test-none-in-use
    assert-link-mma-none-in-use
    assert-list-mma-none-in-use
    assert-region-mma-none-in-use
    assert-rule-mma-none-in-use
    assert-rulestore-mma-none-in-use
    assert-square-mma-none-in-use
    assert-group-mma-none-in-use
    assert-sample-mma-none-in-use
    assert-need-mma-none-in-use
    assert-step-mma-none-in-use
    assert-plan-mma-none-in-use
    assert-action-mma-none-in-use
    assert-changes-mma-none-in-use
    assert-rate-mma-none-in-use
    assert-rlcrate-mma-none-in-use

    depth 0<> 
    if  
        cr ." stack not empty " .s cr
    then
;

include square_t.fs
include squarelist_t.fs
include region_t.fs
include regionlist_t.fs
include rule_t.fs
include action_t.fs
include rulestore_t.fs
include state_t.fs
include input_t.fs
include regionlistcorr_t.fs
include rlclist_t.fs
include session_t.fs

cr ." main.fs"

\ Init array-stacks.
#2000 link-mma-init
#202 list-mma-init
#403 region-mma-init
#404 rule-mma-init
#405 rulestore-mma-init
#306 square-mma-init
#250 sample-mma-init
#150 changes-mma-init
#100 group-mma-init
#200 need-mma-init
#150 step-mma-init
#150 plan-mma-init
 #50 action-mma-init
 #25 domain-mma-init
#100 rate-mma-init
#100 rlcrate-mma-init
#005 session-mma-init
cr

\ Free heap memory before exiting.
: free-heap ( -- )
    cr ." Freeing heap memory"
    list-mma mma-free
    link-mma mma-free
    region-mma mma-free
    rule-mma mma-free
    rulestore-mma mma-free
    square-mma mma-free
    sample-mma mma-free
    changes-mma mma-free
    group-mma mma-free
    need-mma mma-free
    step-mma mma-free
    plan-mma mma-free
    action-mma mma-free
    domain-mma mma-free
    rate-mma mma-free
    rlcrate-mma mma-free
    session-mma mma-free
;

: init-main ( -- )
    \ Set up session.
    current-session-new                         \ session instance added to session stack.
    current-session                             \ sess

    \ Add domain 0
    #4 domain-new                                \ sess dom

    \ Add actions to domain 0
    [ ' domain-0-act-1-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-2-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-3-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-4-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-5-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-6-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    \ Add a domain
    over session-add-domain                    \ sess

    \ Add domain 1
    #5 domain-new                               \ sess dom1

    \ Add actions to domain 1
    [ ' domain-1-act-1-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-2-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-3-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-4-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-5-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-6-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    \ Add last domain
    over session-add-domain                     \ sess dom1

    s" (X1X1 01X1X)" region-list-corr-from-string-a \ sess rlc
    -1 #2 rate-new                              \ sess rlc rt
    rlcrate-new                                 \ sess rlc-rt
    \ cr ." rlcrate: " dup .rlcrate cr
    over session-add-rlcrate                    \ sess

    s" (1XX1 01X1X)" region-list-corr-from-string-a \ sess rlc
    #-2 0 rate-new                               \ sess
    rlcrate-new                                 \ sess rlc-rt
    \ cr ." rlcrate: " dup .rlcrate cr
    over session-add-rlcrate                    \ sess

    drop                                        \
;

0 value step-num
: main ( -- )
    init-main
    0 to step-num
    true
    begin
    while
        \ Inc step num.

        step-num 1+ to step-num
        
        \ Print header.
        cr ." ***************************"
        cr ." Step: " step-num .
        space ." Current state: "
        current-session .session-current-states
        space ." Reachable "
        current-session .session-reachable-regions
        cr

        #80 s" Enter command: > " get-user-input
        \ cr .s cr
        depth 1 <>
        if
            ." depth not equal one? " .s
            abort
        then
    repeat
    
    \ Clean up
    memory-use
    cr cr ." Deallocating ..." cr
    current-session-deallocate
    memory-use
    test-none-in-use
;

\ Set up a test domain and action.
\ To supply number bits, max region, ms-bit, all-bits, domain-id, action-id.
\
\ To run tests outside of all-tests, run this first, followed by test-end
\
: test-init

    \ Set up session.
    current-session-new                 \ Session instance added to session stack.

    \ Set up a source for domain-inst-id, num-bits, ms-bit, all-bits, max-region, action-id.
    #4 domain-new                       \ dom
    current-session                     \ dom sess
    session-add-domain                  \ dom

    #5 domain-new                       \ dom
    current-session                     \ dom sess
    session-add-domain                  \ dom

    \ Set current domain to the first.
    \ Most tests assume a 4-bit domain.
    0 set-domain
;

: test-end
    memory-use
    cr cr ." Deallocating ..." cr
    current-session-deallocate

    memory-use
    session-stack
    stack-empty?
    if
        test-none-in-use
    then
;

: all-tests
    test-none-in-use

    test-init

    square-tests
    depth 0<> abort" Square stack tests not empty"

    square-list-tests
    depth 0<> abort" Square-list tests stack not empty"

    region-tests
    depth 0<> abort" Region tests stack not empty"

    region-list-tests
    depth 0<> abort" Region-list tests stack not empty"

    rule-tests
    depth 0<> abort" Rule tests stack not empty"

    action-tests
    depth 0<> abort" Action tests stack not empty"

    rulestore-tests
    depth 0<> abort" Rulestore tests stack not empty"

    state-tests
    depth 0<> abort" State tests stack not empty"

    input-tests
    depth 0<> abort" Input tests stack not empty"

    \ Tests that assume a 4-bit domain-0 and a 5-bit domain-1 should be last,
    \ as they may change the current domain.
    region-list-corr-tests
    depth 0<> abort" region-list-corr tests stack not empty"

    rlc-list-tests
    depth 0<> abort" rlc-list tests stack not empty"

    session-tests
    depth 0<> abort" Session tests stack not empty"

    test-end
;
